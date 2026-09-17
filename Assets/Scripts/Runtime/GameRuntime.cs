using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public sealed class GameRuntime : MonoBehaviour
    {
        public static GameRuntime Instance { get; private set; }
        public HardwareCatalog Catalog { get; private set; }
        public GameState State { get; private set; }
        public GameEvents Events { get; private set; }
        public InventoryService Inventory { get; private set; }
        public CompatibilityService Compatibility { get; private set; }
        public PowerThermalService PowerThermal { get; private set; }
        public BootService Boot { get; private set; }
        public BenchmarkService Benchmark { get; private set; }
        public EconomyService Economy { get; private set; }
        public ShippingService Shipping { get; private set; }
        public JobService Jobs { get; private set; }
        public DiagnosticsService Diagnostics { get; private set; }
        public AssemblyService Assembly { get; private set; }
        public CableConnectionService Cabling { get; private set; }
        public bool TightenMountFasteners { get; private set; } = true;
        public CustomizationService Customization { get; private set; }
        public SaveService Saves { get; private set; }
        public GameUI UI { get; private set; }
        public WorkshopWorld World { get; private set; }
        public FeedbackService Feedback { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoBoot()
        {
            if (FindAnyObjectByType<GameRuntime>() != null) return;
            GameObject go = new GameObject("ForgeBenchRuntime");
            go.AddComponent<GameRuntime>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Catalog = new HardwareCatalog();
            Catalog.Load();
            Saves = new SaveService(Catalog.Get);
            Events = new GameEvents();
            State = new GameState();
            RebuildServices();
            ApplySettings();
        }

        private void Start()
        {
            gameObject.AddComponent<CrashLogService>();
            Feedback = gameObject.AddComponent<FeedbackService>(); Feedback.Initialize();
            World = gameObject.AddComponent<WorkshopWorld>();
            UI = gameObject.AddComponent<GameUI>();
            if (Saves.HasSave(1))
            {
                string message;
                GameState loaded = Saves.Load(1, out message);
                if (loaded != null) { State = loaded; RebuildServices(); Notify(message); }
                else { NewGame(); Notify(message, false); }
            }
            else NewGame();
            World.Build();
            UI.Build();
            Refresh();
        }

        private void RebuildServices()
        {
            Inventory = new InventoryService(State, Catalog);
            Compatibility = new CompatibilityService(Catalog, Inventory);
            PowerThermal = new PowerThermalService(Catalog, Inventory);
            Boot = new BootService(Inventory, PowerThermal);
            Benchmark = new BenchmarkService(Inventory, PowerThermal);
            Economy = new EconomyService(State);
            Shipping = new ShippingService(State, Catalog, Inventory, Economy);
            Jobs = new JobService(State, Catalog, Inventory);
            Diagnostics = new DiagnosticsService(Inventory, PowerThermal);
            Assembly = new AssemblyService(Inventory);
            Cabling = new CableConnectionService(id => Inventory.Def(Inventory.Get(id)));
            Customization = new CustomizationService(Inventory);
            foreach (MachineState machine in State.machines)
            {
                RamSlotRules.Normalize(machine, Inventory.Def(Inventory.Get(machine.motherboardItemId)));
                ComponentMountRules.EnsureInstalled(machine, id => Inventory.Def(Inventory.Get(id)));
                Assembly.EnsureCaseHardware(machine);
            }
        }

        public void NewGame()
        {
            State = new GameState { saveId = Guid.NewGuid().ToString("N") };
            RebuildServices();
            Inventory.Create("tool_driver");
            Inventory.Create("cons_paste");
            Inventory.Create("cons_air");
            Jobs.EnsureOffers(4);
            Economy.Earn(0, "Workshop opened");
            Refresh();
            Autosave();
            Notify("New workshop created. Open JOBS and accept a contract.");
        }

        public JobState ActiveJob => State.jobs.FirstOrDefault(j => j.stage == JobStage.Accepted || j.stage == JobStage.InProgress || j.stage == JobStage.ReadyToSubmit);
        public MachineState ActiveMachine
        {
            get
            {
                JobState j = ActiveJob;
                return j == null ? State.machines.FirstOrDefault() : State.machines.FirstOrDefault(m => m.machineId == j.machineId);
            }
        }

        public void AcceptJob(string jobId)
        {
            Result(Jobs.Accept(State.jobs.FirstOrDefault(j => j.jobId == jobId)));
        }

        public void Buy(string definitionId, int speedTier = 1) => Result(Shipping.Buy(definitionId, 1, speedTier));

        public void AdvanceDay()
        {
            State.day++;
            Economy.DailyCosts();
            Shipping.Tick();
            foreach (MachineState m in State.machines) m.dust = Mathf.Clamp01(m.dust + 0.012f);
            foreach (JobState j in State.jobs.Where(j => j.stage == JobStage.Accepted || j.stage == JobStage.InProgress))
                if (State.day > j.dueDay + 2) { j.stage = JobStage.Failed; State.reputation = Mathf.Max(0, State.reputation - 15); }
            Jobs.EnsureOffers(4);
            Autosave();
            Refresh();
            Notify("Day " + State.day + ". Deliveries and recurring costs processed.");
        }

        public void ReceiveAll()
        {
            List<ShipmentState> ready = State.shipments.Where(s => s.status == ShipmentStatus.Delivered).ToList();
            if (ready.Count == 0) { Notify("No delivered packages waiting.", false); return; }
            foreach (ShipmentState s in ready) Shipping.Receive(s.shipmentId);
            Autosave(); Refresh(); Notify(ready.Count + " shipment(s) received.");
        }

        private ActionResult BenchCustodyGuard()
        {
            JobState job = ActiveJob;
            if (job == null) return ActionResult.Success("No active customer custody record.");
            ServiceIntakeService service = ServiceIntakeDirector.Instance?.Service;
            return service == null ? ActionResult.Success("Intake service unavailable; legacy bench path allowed.") : service.CanBeginWork(job.jobId);
        }

        private ActionResult ReleaseCustodyGuard()
        {
            JobState job = ActiveJob;
            if (job == null) return ActionResult.Success("No active customer custody record.");
            ServiceIntakeService service = ServiceIntakeDirector.Instance?.Service;
            return service == null ? ActionResult.Success("Intake service unavailable; legacy release path allowed.") : service.CanRelease(job.jobId);
        }

        public void Install(string instanceId, int ramSlot = -1)
        {
            MachineState m = ActiveMachine;
            ItemInstance item = Inventory.Get(instanceId);
            if (m == null) { Notify("Accept a job first.", false); return; }
            ActionResult custody = BenchCustodyGuard(); if (!custody.ok) { Notify(custody.message, false); return; }
            if (item == null || item.reserved) { Notify("Part is unavailable.", false); return; }
            if (m.bootState != BootState.Off) { Notify("Power off the PC before installing components.", false); return; }
            HardwareDefinition requested = Inventory.Def(item);
            if (requested == null) { Notify("Component data unavailable.", false); return; }
            ActionResult mechanical = MechanicalAssemblyRules.CanInstall(m, requested.category);
            if (!mechanical.ok) { Notify(mechanical.message, false); return; }
            if (requested != null && requested.category == PartCategory.Motherboard && !string.IsNullOrEmpty(m.motherboardItemId) && m.ramItemIds.Count > 0) { Notify("Remove RAM before replacing its motherboard.", false); return; }
            if (requested != null && requested.category != PartCategory.Case && !Assembly.InternalsAccessible(m)) { Notify("Remove the side panel before accessing internal components.", false); return; }
            if (requested != null && requested.category == PartCategory.Case && (!string.IsNullOrEmpty(m.motherboardItemId) || !string.IsNullOrEmpty(m.cpuItemId) || m.ramItemIds.Count > 0 || !string.IsNullOrEmpty(m.gpuItemId))) { Notify("Remove internal components before replacing the case.", false); return; }
            ActionResult compatible = Compatibility.CanInstall(m, item);
            if (!compatible.ok) { Notify(compatible.message, false); return; }
            HardwareDefinition d = Inventory.Def(item);
            if (d == null) { Notify("Part definition missing.", false); return; }

            if (d.category == PartCategory.RAM)
            {
                ActionResult placement = RamSlotRules.Install(m, Inventory.Def(Inventory.Get(m.motherboardItemId)), item.instanceId, ramSlot);
                if (!placement.ok) { Notify(placement.message, false); return; }
            }
            else if (d.category == PartCategory.Storage) m.storageItemIds.Add(item.instanceId);
            else if (d.category == PartCategory.Fan) m.fanItemIds.Add(item.instanceId);
            else
            {
                string old = GetSingleSlot(m, d.category);
                if (!string.IsNullOrEmpty(old))
                {
                    ActionResult mechanicalRelease = MechanicalAssemblyRules.CanRemove(m, d.category, id => Inventory.Def(Inventory.Get(id)));
                    if (!mechanicalRelease.ok) { Notify(mechanicalRelease.message, false); return; }
                    ActionResult released = Cabling.CanRemove(m, Inventory.Def(Inventory.Get(old)));
                    if (!released.ok) { Notify(released.message, false); return; }
                    ItemInstance oldItem = Inventory.Get(old); if (oldItem != null) { oldItem.reserved = false; oldItem.note = oldItem.customerOwned ? "Removed customer part for " + m.ownerJobId : string.Empty; }
                }
                SetSingleSlot(m, d.category, item.instanceId);
                if (d.category == PartCategory.Motherboard) { m.cpuRetentionOpen = true; m.ramLatches.Clear(); RamSlotRules.Normalize(m, d); }
            }
            item.reserved = true;
            ComponentMountRules.Ensure(m, d.category, d);
            if (d.category == PartCategory.CPU) MechanicalAssemblyRules.BreakThermalInterface(m);
            CableConnectionService.InvalidateConnections(m, d.category);
            item.note = "Installed in " + m.machineId;
            if (d.category == PartCategory.Case)
            {
                m.sidePanelInstalled = true;
                m.sidePanel = new PanelState { installed = true };
                Assembly.EnsureCaseHardware(m);
            }
            m.bootState = BootState.Off;
            m.benchmarkScore = 0;
            m.stressStable = false;
            m.history.Add("Installed " + d.brand + " " + d.model);
            MarkInProgress(); Autosave(); Refresh(); Notify(d.model + " installed.");
        }

        public void InstallBestAvailable(PartCategory category, int ramSlot = -1)
        {
            MachineState m = ActiveMachine;
            if (m == null) { Notify("Accept a job first.", false); return; }
            List<ItemInstance> candidates = Inventory.Available(category).OrderByDescending(i => Inventory.Def(i)?.performance ?? 0).ThenByDescending(i => Inventory.Def(i)?.quality ?? 0).ToList();
            foreach (ItemInstance item in candidates)
            {
                if (Compatibility.CanInstall(m, item).ok) { Install(item.instanceId, ramSlot); return; }
            }
            Notify("No compatible " + category + " is available in inventory.", false);
        }

        public void Remove(string instanceId)
        {
            MachineState m = ActiveMachine;
            ItemInstance item = Inventory.Get(instanceId);
            if (m == null || item == null) return;
            ActionResult custody = BenchCustodyGuard(); if (!custody.ok) { Notify(custody.message, false); return; }
            HardwareDefinition removing = Inventory.Def(item);
            if (removing == null) { Notify("Component data unavailable.", false); return; }
            ActionResult mechanicalRelease = MechanicalAssemblyRules.CanRemove(m, removing.category, id => Inventory.Def(Inventory.Get(id)));
            if (!mechanicalRelease.ok) { Notify(mechanicalRelease.message, false); return; }
            ActionResult cableRelease = Cabling.CanRemove(m, removing);
            if (!cableRelease.ok) { Notify(cableRelease.message, false); return; }
            if (removing != null && removing.category == PartCategory.Motherboard && m.ramItemIds.Count > 0) { Notify("Remove RAM before removing its motherboard.", false); return; }
            if (removing != null && removing.category != PartCategory.Case && !Assembly.InternalsAccessible(m)) { Notify("Remove the side panel before removing internal components.", false); return; }
            bool removed = false;
            int ramIndex = m.ramItemIds.IndexOf(instanceId);
            if (ramIndex >= 0)
            {
                ActionResult released = RamSlotRules.CanRemove(m, Inventory.Def(Inventory.Get(m.motherboardItemId)), ramIndex);
                if (!released.ok) { Notify(released.message, false); return; }
                m.ramItemIds.RemoveAt(ramIndex);
                if (m.ramSlotIndices != null && ramIndex < m.ramSlotIndices.Count) m.ramSlotIndices.RemoveAt(ramIndex);
                removed = true;
            }
            else if (m.storageItemIds.Remove(instanceId) || m.fanItemIds.Remove(instanceId) || ClearSingleIfMatches(m, instanceId)) removed = true;
            if (removed)
            {
                if (removing.category == PartCategory.CPU || removing.category == PartCategory.Cooler) MechanicalAssemblyRules.BreakThermalInterface(m);
                if (removing != null) CableConnectionService.InvalidateConnections(m, removing.category);
                item.reserved = false; item.note = string.Empty; m.bootState = BootState.Off; m.benchmarkScore = 0; m.stressStable = false;
                if (removing != null && removing.category == PartCategory.Case) { m.sidePanelInstalled = false; m.sidePanel = new PanelState { installed = false }; }
                if (removing != null && removing.category == PartCategory.Storage && m.storageItemIds.Count == 0) { m.partitioned=false;m.osInstalled=false;m.activated=false;m.driversInstalled=false; }
                Autosave(); Refresh(); Notify("Component removed.");
            }
        }

        public void ToggleRamLatch(int slot, bool top)
        {
            MachineState machine = ActiveMachine;
            if (machine == null) return;
            ActionResult custody = BenchCustodyGuard(); if (!custody.ok) { Notify(custody.message, false); return; }
            if (!Assembly.InternalsAccessible(machine)) { Notify("Remove the side panel to reach DIMM latches.", false); return; }
            ActionResult result = RamSlotRules.ToggleLatch(machine, Inventory.Def(Inventory.Get(machine.motherboardItemId)), slot, top);
            if (result.ok) machine.history.Add(result.message);
            Result(result);
        }

        public void ToggleCpuRetention()
        {
            ActionResult custody = BenchCustodyGuard(); if (!custody.ok) { Notify(custody.message, false); return; }
            Result(MechanicalAssemblyRules.ToggleCpuRetention(ActiveMachine));
        }

        public void ToggleMountToolMode()
        {
            TightenMountFasteners = !TightenMountFasteners;
            Refresh(); Notify(TightenMountFasteners ? "Screwdriver: tighten." : "Screwdriver: loosen.");
        }

        public void TurnMountFastener(PartCategory category, int index)
        {
            ActionResult custody = BenchCustodyGuard(); if (!custody.ok) { Notify(custody.message, false); return; }
            bool hasDriver = Inventory.Available(PartCategory.Tool).Any(item => item.definitionId == "tool_driver" && item.condition > .1f && item.fault == FaultType.None);
            Result(ComponentMountRules.Turn(ActiveMachine, category, index, TightenMountFasteners, hasDriver));
        }

        public void PowerOff()
        {
            MachineState machine = ActiveMachine;
            if (machine == null) return;
            machine.bootState = BootState.Off;
            machine.history.Add("PC powered off for service.");
            Autosave(); Refresh(); Notify("PC powered off.");
        }

        public void LoosenFastener() { MachineState m=ActiveMachine; if(m==null){Notify("No device on bench.",false);return;} Result(Assembly.LoosenNext(m)); }
        public void TightenFastener() { MachineState m=ActiveMachine; if(m==null){Notify("No device on bench.",false);return;} Result(Assembly.TightenNext(m)); }
        public void ToggleSidePanel() { MachineState m=ActiveMachine; if(m==null){Notify("No device on bench.",false);return;} Result(Assembly.TogglePanel(m)); }
        public void CycleRgbPreset() { MachineState m=ActiveMachine; if(m==null){Notify("No device on bench.",false);return;} Result(Customization.CyclePreset(m)); }
        public void SetRgbColor(Color color) { MachineState m=ActiveMachine; if(m==null)return; Customization.SetColor(m,color); }
        public void CommitCustomization() { if(ActiveMachine==null)return; Autosave(); World?.RefreshMachine(); UI?.Refresh(); Notify("Customization saved."); }
        public void CycleRgbEffect() { MachineState m=ActiveMachine; if(m==null){Notify("No device on bench.",false);return;} Result(Customization.CycleEffect(m)); }
        public void CycleCableColor() { MachineState m=ActiveMachine; if(m==null){Notify("No device on bench.",false);return;} Result(Customization.CycleCableColor(m)); }

        public void ToggleCable(CableCircuit circuit)
        {
            MachineState m = ActiveMachine;
            if (m == null) return;
            ActionResult custody = BenchCustodyGuard(); if (!custody.ok) { Notify(custody.message, false); return; }
            ActionResult action = Cabling.SetConnection(m, circuit, !CableConnectionService.Connected(m, circuit));
            if (action.ok) { UpdateCableRoutingScore(m); MarkInProgress(); }
            Result(action);
        }

        private void UpdateCableRoutingScore(MachineState m)
        {
            int total = 0, connected = 0;
            foreach (CableCircuit circuit in Enum.GetValues(typeof(CableCircuit)))
            {
                if (circuit == CableCircuit.Rgb || !Cabling.Present(m, circuit)) continue;
                total++; if (CableConnectionService.Connected(m, circuit)) connected++;
            }
            m.cableManagementScore = total == 0 ? 0f : Mathf.Clamp01((float)connected / total * (.62f + State.workshop.benchLevel * .07f));
        }

        public void ConnectCables()
        {
            MachineState m = ActiveMachine;
            if (m == null) { Notify("No device on bench.", false); return; }
            ActionResult custody = BenchCustodyGuard(); if (!custody.ok) { Notify(custody.message, false); return; }
            List<string> unresolved = new List<string>(); int present = 0;
            foreach (CableCircuit circuit in Enum.GetValues(typeof(CableCircuit)))
            {
                if (!Cabling.Present(m, circuit)) continue;
                present++;
                ActionResult action = Cabling.SetConnection(m, circuit, true);
                if (!action.ok) unresolved.Add(action.message);
            }
            if (present == 0) { Notify("Install components before connecting cables.", false); return; }
            UpdateCableRoutingScore(m); MarkInProgress(); Autosave(); Refresh();
            Notify(unresolved.Count == 0 ? "Required cables connected." : string.Join(" ", unresolved), unresolved.Count == 0);
        }

        public void ApplyThermalPaste()
        {
            MachineState m = ActiveMachine;
            ActionResult access = MechanicalAssemblyRules.CanApplyPaste(m);
            if (!access.ok) { Notify(access.message, false); return; }
            ActionResult custody = BenchCustodyGuard(); if (!custody.ok) { Notify(custody.message, false); return; }
            ItemInstance paste = Inventory.Available(PartCategory.Consumable).FirstOrDefault(i => Inventory.Def(i)?.tags.Contains("paste") == true);
            if (paste == null) { Notify("Thermal compound is not in inventory.", false); return; }
            if (!Inventory.Consume(paste.instanceId)) { Notify("Thermal compound could not be consumed.", false); return; }
            m.thermalPasteApplied = true; m.thermalPasteQuality = Mathf.Clamp01(0.78f + State.workshop.benchLevel * 0.05f);
            Autosave(); Refresh(); Notify("Thermal compound applied with even coverage.");
        }

        public void CleanDevice()
        {
            MachineState m = ActiveMachine;
            if (m == null) { Notify("No device on bench.", false); return; }
            ActionResult custody = BenchCustodyGuard(); if (!custody.ok) { Notify(custody.message, false); return; }
            ItemInstance air = Inventory.Available(PartCategory.Consumable).FirstOrDefault(i => Inventory.Def(i)?.tags.Contains("cleaning") == true);
            if (air == null) { Notify("You need cleaning consumable or an upgraded air blower.", false); return; }
            m.dust = Mathf.Max(0f, m.dust - 0.9f); m.history.Add("Dust cleaned");
            Autosave(); Refresh(); Notify("Dust removed from filters, fans and heatsinks.");
        }

        public void PowerOn() { MachineState m = ActiveMachine; if (m == null) { Notify("No device on bench.", false); return; } Result(Boot.PowerOn(m)); }

        public void SetMemoryProfile(bool enabled)
        {
            MachineState m = ActiveMachine; if (m == null) return;
            m.bios.memoryProfileEnabled = enabled;
            if (enabled && m.ramItemIds.Count > 0) m.bios.memorySpeedOverride = m.ramItemIds.Max(id => Inventory.Def(Inventory.Get(id))?.speed ?? 0);
            Autosave(); Refresh(); Notify("Memory profile " + (enabled ? "enabled" : "disabled") + ".");
        }

        public void CycleFanProfile()
        {
            MachineState m = ActiveMachine; if (m == null) return;
            m.bios.fanProfile = (m.bios.fanProfile + 1) % 3;
            PowerThermal.Simulate(m, false); Autosave(); Refresh(); Notify("Fan profile: " + (m.bios.fanProfile == 0 ? "Silent" : m.bios.fanProfile == 1 ? "Balanced" : "Performance") + ".");
        }

        public void InstallOS()
        {
            MachineState m = ActiveMachine;
            if (m == null) { Notify("No device on bench.", false); return; }
            if (m.postCode != "A0") { Notify("Pass POST before installing the OS.", false); return; }
            if (m.storageItemIds.Count == 0) { Notify("No storage device detected.", false); return; }
            m.partitioned = true; m.osInstalled = true; m.activated = true; m.bootState = BootState.OperatingSystem; m.history.Add("ForgeOS installed");
            Autosave(); Refresh(); Notify("ForgeOS installed, partitioned and activated.");
        }

        public void InstallDrivers()
        {
            MachineState m = ActiveMachine;
            if (m == null || !m.osInstalled) { Notify("Install the OS first.", false); return; }
            m.driversInstalled = true; m.history.Add("Chipset/GPU/network drivers installed");
            Autosave(); Refresh(); Notify("Hardware drivers installed.");
        }

        public void RunBenchmark() { MachineState m = ActiveMachine; if (m == null) { Notify("No device on bench.", false); return; } Result(Benchmark.Run(m)); }
        public void Diagnose() { MachineState m = ActiveMachine; if (m == null) { Notify("No device on bench.", false); return; } Result(Diagnostics.Scan(m)); }

        public void ValidateAndSubmit()
        {
            JobState j = ActiveJob; MachineState m = ActiveMachine;
            if (m != null && !string.IsNullOrEmpty(m.caseItemId))
            {
                Assembly.EnsureCaseHardware(m);
                if (!m.sidePanelInstalled) { Notify("Install the side panel before returning the PC to the customer.", false); return; }
                if (Assembly.TighteningQuality(m) < .95f) { Notify("Tighten all side-panel fasteners before delivery.", false); return; }
            }
            ActionResult release = ReleaseCustodyGuard(); if (!release.ok) { Notify(release.message, false); Refresh(); return; }
            ActionResult validation = Jobs.Validate(j, m);
            if (!validation.ok) { Notify(validation.message, false); Refresh(); return; }
            float bonus = 0f;
            if (j.hiddenPreferenceLowNoise && m.noiseDb <= j.maxNoiseDb) bonus += j.reward * .12f;
            if (State.day <= j.dueDay) bonus += j.reward * .08f;
            j.stage = JobStage.Completed; j.completionMessage = "Completed on day " + State.day;
            Economy.Earn(j.reward + bonus, "Completed " + j.jobId);
            State.reputation += 20 + Mathf.RoundToInt(bonus / 10f); State.experience += 100;
            State.machines.Remove(m);
            foreach (ItemInstance item in State.inventory.Where(x => x.reserved && x.note == "Installed in " + m.machineId).ToList())
            {
                if (!item.customerOwned) State.inventory.Remove(item); else { item.reserved = false; item.note = string.Empty; }
            }
            foreach (ItemInstance customerPart in State.inventory.Where(x => x.customerOwned && x.ownerJobId == j.jobId).ToList()) State.inventory.Remove(customerPart);
            Jobs.EnsureOffers(4);
            Autosave(); Refresh(); Notify("Job delivered. Earned $" + (j.reward + bonus).ToString("0") + ". Reputation increased.");
        }

        public void UpgradeWorkshop()
        {
            float cost = 350f * State.workshop.level;
            ActionResult pay = Economy.Spend(cost, "Workshop upgrade");
            if (!pay.ok) { Notify(pay.message, false); return; }
            State.workshop.level++; State.workshop.benchLevel++; State.workshop.storageLevel++;
            if (State.workshop.level >= 2) State.workshop.diagnosticsLevel = 2;
            if (State.workshop.level >= 3) State.workshop.boardRepairLevel = 1;
            State.milestones.Add("Workshop level " + State.workshop.level);
            Autosave(); Refresh(); Notify("Workshop upgraded to level " + State.workshop.level + ".");
        }

        public void Save(int slot = 1) => Result(Saves.Save(State, slot), false);
        public void Load(int slot = 1)
        {
            string message; GameState loaded = Saves.Load(slot, out message);
            if (loaded == null) { Notify(message, false); return; }
            State = loaded; RebuildServices(); ApplySettings(); Refresh(); Notify(message);
        }

        public void SetLanguage(string language)
        {
            State.settings.language = language == "en" ? "en" : "uk";
            Autosave(); Refresh(); Notify(State.settings.language == "uk" ? "Мову змінено на українську." : "Language switched to English.");
        }

        public void ToggleBatterySaver()
        {
            State.settings.batterySaver = !State.settings.batterySaver; ApplySettings(); Autosave(); Refresh();
            Notify("Battery saver " + (State.settings.batterySaver ? "enabled" : "disabled") + ".");
        }

        public void ToggleHaptics() { State.settings.haptics=!State.settings.haptics;Autosave();Refresh();Notify("Haptics "+(State.settings.haptics?"enabled":"disabled")+"."); }
        public void ToggleReducedMotion() { State.settings.reducedMotion=!State.settings.reducedMotion;Autosave();Refresh();Notify("Reduced motion "+(State.settings.reducedMotion?"enabled":"disabled")+"."); }
        public void ToggleCaptions() { State.settings.captions=!State.settings.captions;Autosave();Refresh();Notify("Captions "+(State.settings.captions?"enabled":"disabled")+"."); }
        public void AdjustTextScale(float delta) { State.settings.textScale=Mathf.Clamp(State.settings.textScale+delta,.8f,1.4f);Autosave();Refresh();Notify("Text scale "+Mathf.RoundToInt(State.settings.textScale*100)+"%."); }
        public void AdjustLookSensitivity(float delta) { State.settings.lookSensitivity=Mathf.Clamp(State.settings.lookSensitivity+delta,.45f,2.2f);Autosave();Refresh();Notify("Look sensitivity "+State.settings.lookSensitivity.ToString("0.00")+"."); }
        public void CycleColorblindMode() { State.settings.colorblindMode=(State.settings.colorblindMode+1)%4;Autosave();Refresh();Notify("Color-assist mode "+State.settings.colorblindMode+"."); }

        private void ApplySettings()
        {
            if (State?.settings == null) return;
            Application.targetFrameRate = State.settings.batterySaver ? 30 : Mathf.Clamp(State.settings.fpsLimit, 30, 120);
            QualitySettings.vSyncCount = 0;
        }

        private void Result(ActionResult result, bool autosave = true)
        {
            if (result.ok && autosave) Autosave();
            Refresh(); Notify(result.message, result.ok);
        }

        private void MarkInProgress() { JobState j = ActiveJob; if (j != null && j.stage == JobStage.Accepted) j.stage = JobStage.InProgress; }
        private void Autosave() { if (State != null) Saves.Save(State, 1); }
        private void Refresh() { Events?.Publish("state.changed"); UI?.Refresh(); World?.RefreshMachine(); }
        public void Notify(string text, bool success = true) { UI?.ShowToast(text, success); Feedback?.Play(success); Debug.Log((success ? "[ForgeBench] " : "[ForgeBench ERROR] ") + text); }

        private static string GetSingleSlot(MachineState m, PartCategory category)
        {
            switch (category)
            {
                case PartCategory.Case: return m.caseItemId;
                case PartCategory.Motherboard: return m.motherboardItemId;
                case PartCategory.CPU: return m.cpuItemId;
                case PartCategory.GPU: return m.gpuItemId;
                case PartCategory.PSU: return m.psuItemId;
                case PartCategory.Cooler: return m.coolerItemId;
                default: return null;
            }
        }
        private static void SetSingleSlot(MachineState m, PartCategory category, string value)
        {
            switch (category)
            {
                case PartCategory.Case: m.caseItemId = value; break;
                case PartCategory.Motherboard: m.motherboardItemId = value; break;
                case PartCategory.CPU: m.cpuItemId = value; break;
                case PartCategory.GPU: m.gpuItemId = value; break;
                case PartCategory.PSU: m.psuItemId = value; break;
                case PartCategory.Cooler: m.coolerItemId = value; break;
            }
        }
        private static bool ClearSingleIfMatches(MachineState m, string id)
        {
            if (m.caseItemId == id) { m.caseItemId = null; return true; }
            if (m.motherboardItemId == id) { m.motherboardItemId = null; return true; }
            if (m.cpuItemId == id) { m.cpuItemId = null; return true; }
            if (m.gpuItemId == id) { m.gpuItemId = null; return true; }
            if (m.psuItemId == id) { m.psuItemId = null; return true; }
            if (m.coolerItemId == id) { m.coolerItemId = null; return true; }
            return false;
        }

        private void OnApplicationPause(bool pause) { if (pause && State != null) Autosave(); }
        private void OnApplicationQuit() { if (State != null) Autosave(); }
    }
}
