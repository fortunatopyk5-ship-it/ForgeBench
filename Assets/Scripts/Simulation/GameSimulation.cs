using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public sealed class InventoryService
    {
        private readonly GameState state;
        private readonly HardwareCatalog catalog;
        public InventoryService(GameState s, HardwareCatalog c) { state = s; catalog = c; }

        public ItemInstance Create(string definitionId, bool customerOwned = false)
        {
            if (catalog.Get(definitionId) == null) throw new ArgumentException("Unknown part: " + definitionId);
            ItemInstance item = new ItemInstance
            {
                instanceId = "I" + state.nextItemSerial++.ToString("D6"),
                definitionId = definitionId,
                customerOwned = customerOwned,
                condition = 1f
            };
            state.inventory.Add(item);
            return item;
        }

        public ItemInstance Get(string instanceId) => state.inventory.FirstOrDefault(i => i.instanceId == instanceId);
        public HardwareDefinition Def(ItemInstance item) => item == null ? null : catalog.Get(item.definitionId);
        public List<ItemInstance> Available(PartCategory category) => state.inventory.Where(i => !i.reserved && catalog.Get(i.definitionId)?.category == category).ToList();
        public int CountAvailable(string definitionId) => state.inventory.Count(i => !i.reserved && i.definitionId == definitionId);
        public bool Consume(string instanceId)
        {
            ItemInstance item = Get(instanceId);
            if (item == null || item.reserved || item.customerOwned) return false;
            return state.inventory.Remove(item);
        }
    }

    public sealed class CompatibilityService
    {
        private readonly HardwareCatalog catalog;
        private readonly InventoryService inventory;
        public CompatibilityService(HardwareCatalog c, InventoryService i) { catalog = c; inventory = i; }
        private HardwareDefinition D(string itemId) => inventory.Def(inventory.Get(itemId));

        private static bool CaseAccepts(string caseFf, string boardFf)
        {
            if (string.IsNullOrEmpty(caseFf) || string.IsNullOrEmpty(boardFf)) return true;
            if (caseFf == "EATX") return true;
            if (caseFf == "ATX") return boardFf == "ATX" || boardFf == "mATX" || boardFf == "MiniITX";
            if (caseFf == "mATX") return boardFf == "mATX" || boardFf == "MiniITX";
            return caseFf == boardFf;
        }

        public ActionResult CanInstall(MachineState m, ItemInstance item)
        {
            HardwareDefinition p = inventory.Def(item);
            if (p == null) return ActionResult.Fail("Unknown component data.");
            HardwareDefinition board = D(m.motherboardItemId);
            HardwareDefinition pcCase = D(m.caseItemId);

            switch (p.category)
            {
                case PartCategory.Case:
                    if (board != null && !CaseAccepts(p.formFactor, board.formFactor)) return ActionResult.Fail("Case does not support motherboard form factor " + board.formFactor + ".");
                    break;
                case PartCategory.Motherboard:
                    if (m.ramItemIds.Count > RamSlotRules.SlotCount(p)) return ActionResult.Fail("Installed RAM exceeds this motherboard's DIMM slot count.");
                    if (pcCase != null && !CaseAccepts(pcCase.formFactor, p.formFactor)) return ActionResult.Fail("Motherboard form factor does not fit this case.");
                    HardwareDefinition cpu = D(m.cpuItemId);
                    if (cpu != null && cpu.socket != p.socket) return ActionResult.Fail("CPU socket mismatch: " + cpu.socket + " vs " + p.socket + ".");
                    foreach (string ramId in m.ramItemIds)
                    {
                        HardwareDefinition ram = D(ramId);
                        if (ram != null && ram.memoryType != p.memoryType) return ActionResult.Fail("Installed RAM generation is incompatible.");
                    }
                    break;
                case PartCategory.CPU:
                    if (board != null && board.socket != p.socket) return ActionResult.Fail("CPU socket " + p.socket + " does not match board " + board.socket + ".");
                    break;
                case PartCategory.RAM:
                    if (board == null) return ActionResult.Fail("Install a motherboard before RAM.");
                    if (board != null && board.memoryType != p.memoryType) return ActionResult.Fail("RAM " + p.memoryType + " does not match board " + board.memoryType + ".");
                    int dimmCapacity = RamSlotRules.SlotCount(board);
                    if (m.ramItemIds.Count >= dimmCapacity) return ActionResult.Fail("All " + dimmCapacity + " DIMM slots are occupied.");
                    break;
                case PartCategory.GPU:
                    if (pcCase != null && p.lengthMm > pcCase.lengthMm) return ActionResult.Fail("GPU is too long for this case.");
                    break;
                case PartCategory.Cooler:
                    if (pcCase != null && p.tags.Contains("air") && p.heightMm > pcCase.heightMm) return ActionResult.Fail("CPU cooler is too tall for this case.");
                    break;
                case PartCategory.Storage:
                    if (p.storageInterface == "NVMe" && board == null) return ActionResult.Fail("Install a motherboard before an NVMe drive.");
                    if (m.storageItemIds.Count >= 4) return ActionResult.Fail("No free storage mounting point.");
                    break;
            }
            return ActionResult.Success("Compatible.");
        }

        public List<string> ExplainSystem(MachineState m)
        {
            List<string> issues = new List<string>();
            HardwareDefinition board = D(m.motherboardItemId), cpu = D(m.cpuItemId), pcCase = D(m.caseItemId), gpu = D(m.gpuItemId), cooler = D(m.coolerItemId);
            if (board != null && cpu != null && board.socket != cpu.socket) issues.Add("CPU/board socket mismatch");
            if (board != null)
                foreach (string r in m.ramItemIds) if (D(r)?.memoryType != board.memoryType) issues.Add("RAM generation mismatch");
            if (pcCase != null && board != null && !CaseAccepts(pcCase.formFactor, board.formFactor)) issues.Add("Board does not fit case");
            if (pcCase != null && gpu != null && gpu.lengthMm > pcCase.lengthMm) issues.Add("GPU clearance exceeded");
            if (pcCase != null && cooler != null && cooler.tags.Contains("air") && cooler.heightMm > pcCase.heightMm) issues.Add("Cooler clearance exceeded");
            return issues.Distinct().ToList();
        }
    }

    public sealed class PowerThermalService
    {
        private readonly HardwareCatalog catalog;
        private readonly InventoryService inventory;
        public PowerThermalService(HardwareCatalog c, InventoryService i) { catalog = c; inventory = i; }
        private HardwareDefinition D(string id) => inventory.Def(inventory.Get(id));

        public float EstimatePower(MachineState m)
        {
            float total = 18f;
            string[] singles = { m.motherboardItemId, m.cpuItemId, m.gpuItemId, m.coolerItemId };
            foreach (string id in singles) total += D(id)?.powerWatts ?? 0f;
            foreach (string id in m.ramItemIds) total += D(id)?.powerWatts ?? 0f;
            foreach (string id in m.storageItemIds) total += D(id)?.powerWatts ?? 0f;
            foreach (string id in m.fanItemIds) total += D(id)?.powerWatts ?? 0f;
            if (m.bios.cpuPowerLimitWatts > 0 && D(m.cpuItemId) != null) total += Mathf.Max(0, m.bios.cpuPowerLimitWatts - D(m.cpuItemId).powerWatts) * 0.45f;
            return total;
        }

        public float PsuCapacity(MachineState m) => D(m.psuItemId)?.psuWattage ?? 0;

        public void Simulate(MachineState m, bool stress)
        {
            HardwareDefinition cpu = D(m.cpuItemId), gpu = D(m.gpuItemId), cooler = D(m.coolerItemId);
            float load = stress ? 1f : 0.38f;
            float cpuHeat = (cpu?.heatWatts ?? 40) * load;
            float gpuHeat = (gpu?.heatWatts ?? 0) * load;
            float cooling = Mathf.Max(20, cooler?.performance ?? 35) + m.fanItemIds.Count * 22f;
            float dustPenalty = 1f + Mathf.Clamp01(m.dust) * 0.65f;
            float pastePenalty = m.thermalPasteApplied ? Mathf.Lerp(1.2f, 0.85f, Mathf.Clamp01(m.thermalPasteQuality)) : 1.85f;
            m.cpuTempC = Mathf.Clamp(31f + (cpuHeat / cooling) * 72f * dustPenalty * pastePenalty, 28f, 125f);
            m.gpuTempC = Mathf.Clamp(32f + (gpuHeat / Mathf.Max(55f, cooling * 0.75f)) * 55f * dustPenalty, 28f, 118f);
            float fanNoise = 0f;
            foreach (string f in m.fanItemIds) fanNoise += D(f)?.noiseDb ?? 0;
            m.noiseDb = Mathf.Clamp(24f + Mathf.Sqrt(Mathf.Max(0f, fanNoise)) * 2.2f + (gpu?.noiseDb ?? 0) * 0.35f + (cooler?.noiseDb ?? 0) * 0.45f, 20f, 65f);
            if (m.bios.fanProfile == 0) { m.noiseDb -= 4f; m.cpuTempC += 7f; m.gpuTempC += 4f; }
            if (m.bios.fanProfile == 2) { m.noiseDb += 4f; m.cpuTempC -= 6f; m.gpuTempC -= 4f; }
            m.systemPowerW = EstimatePower(m) * load;
            m.stressStable = m.cpuTempC < 96f && m.gpuTempC < 94f && PsuCapacity(m) >= EstimatePower(m) * 1.18f;
        }
    }

    public sealed class BootService
    {
        private readonly InventoryService inventory;
        private readonly PowerThermalService sim;
        public BootService(InventoryService i, PowerThermalService s) { inventory = i; sim = s; }
        private HardwareDefinition D(string id) => inventory.Def(inventory.Get(id));

        public ActionResult PowerOn(MachineState m)
        {
            m.bootState = BootState.Posting;
            if (D(m.motherboardItemId) == null) return Fail(m, "00", "No motherboard detected.");
            if (D(m.cpuItemId) == null) return Fail(m, "CPU", "CPU missing.");
            if (m.cpuRetentionOpen) return Fail(m, "CPU-LOCK", "CPU retention lever is not locked.");
            if (m.ramItemIds.Count == 0) return Fail(m, "DRAM", "No memory installed.");
            if (!RamSlotRules.IsSecured(m, D(m.motherboardItemId))) return Fail(m, "DRAM", "RAM positions or retention latches are not secured.");
            if (D(m.psuItemId) == null) return Fail(m, "PWR", "Power supply missing.");
            if (D(m.coolerItemId) == null) return Fail(m, "FAN", "CPU cooler missing.");
            if (!m.thermalPasteApplied) return Fail(m, "TEMP", "Thermal interface material missing.");
            if (!m.cables.atx24) return Fail(m, "24P", "24-pin ATX power is disconnected.");
            if (!m.cables.cpuEps) return Fail(m, "EPS", "CPU EPS power is disconnected.");
            if (!m.cables.frontPanel) return Fail(m, "FPIO", "Front-panel power switch is not wired.");
            if (!m.cables.cpuFan) return Fail(m, "CPUF", "CPU_FAN header has no signal.");
            HardwareDefinition gpu = D(m.gpuItemId), cpu = D(m.cpuItemId);
            bool igpu = cpu != null && cpu.tags.Contains("igpu");
            if (gpu == null && !igpu) return Fail(m, "VGA", "No video adapter available.");
            if (gpu != null && (gpu.connectors.Contains("PCIE8") || gpu.connectors.Contains("12V2x6")) && !m.cables.gpuPower) return Fail(m, "VGA-P", "GPU auxiliary power is disconnected.");
            if (sim.PsuCapacity(m) < sim.EstimatePower(m) * 1.1f) return Fail(m, "OCP", "PSU capacity is insufficient for POST load.");
            foreach (string id in AllInstalled(m))
            {
                ItemInstance it = inventory.Get(id);
                if (it != null && (it.condition < 0.25f || it.fault == FaultType.DeadPart)) return Fail(m, "HW", "A critical component failed self-test.");
                if (it != null && it.fault == FaultType.UnstableMemory) return Fail(m, "DRAM", "Memory training failed.");
            }
            m.postCode = "A0";
            m.bootState = m.storageItemIds.Count == 0 ? BootState.Bios : (m.osInstalled ? BootState.OperatingSystem : BootState.Bios);
            m.history.Add("POST passed: A0");
            sim.Simulate(m, false);
            return ActionResult.Success(m.osInstalled ? "POST passed; operating system booted." : "POST passed; firmware opened because no bootable OS is installed.");
        }

        private static IEnumerable<string> AllInstalled(MachineState m)
        {
            yield return m.caseItemId; yield return m.motherboardItemId; yield return m.cpuItemId; yield return m.gpuItemId; yield return m.psuItemId; yield return m.coolerItemId;
            foreach (string x in m.ramItemIds) yield return x;
            foreach (string x in m.storageItemIds) yield return x;
            foreach (string x in m.fanItemIds) yield return x;
        }

        private static ActionResult Fail(MachineState m, string code, string text)
        {
            m.postCode = code; m.bootState = BootState.PostFailed; m.history.Add("POST " + code + ": " + text); return ActionResult.Fail(text + " [POST " + code + "]");
        }
    }

    public sealed class BenchmarkService
    {
        private readonly InventoryService inventory;
        private readonly PowerThermalService sim;
        public BenchmarkService(InventoryService i, PowerThermalService s) { inventory = i; sim = s; }
        private HardwareDefinition D(string id) => inventory.Def(inventory.Get(id));

        public ActionResult Run(MachineState m)
        {
            if (!m.osInstalled) return ActionResult.Fail("Install the operating system before benchmarking.");
            if (!m.driversInstalled) return ActionResult.Fail("Performance driver package is missing.");
            sim.Simulate(m, true);
            HardwareDefinition cpu = D(m.cpuItemId), gpu = D(m.gpuItemId);
            int ram = m.ramItemIds.Sum(id => D(id)?.performance ?? 0);
            int storage = m.storageItemIds.Count == 0 ? 0 : m.storageItemIds.Max(id => D(id)?.performance ?? 0);
            float score = (cpu?.performance ?? 0) * 0.45f + (gpu?.performance ?? (cpu?.performance ?? 0) * 0.35f) * 0.42f + ram * 0.08f + storage * 0.05f;
            if (m.bios.memoryProfileEnabled) score *= 1.06f;
            score *= Mathf.Lerp(0.76f, 1.02f, m.cableManagementScore);
            if (m.cpuTempC > 90) score *= 0.86f;
            if (m.gpuTempC > 88) score *= 0.89f;
            if (!m.stressStable) score *= 0.82f;
            m.benchmarkScore = Mathf.Round(score);
            m.history.Add("Benchmark " + m.benchmarkScore + ", CPU " + m.cpuTempC.ToString("0") + "C, GPU " + m.gpuTempC.ToString("0") + "C");
            return ActionResult.Success("Benchmark: " + m.benchmarkScore + " pts; CPU " + m.cpuTempC.ToString("0") + "°C; GPU " + m.gpuTempC.ToString("0") + "°C; " + (m.stressStable ? "stable" : "unstable") + ".");
        }
    }

    public sealed class EconomyService
    {
        private readonly GameState state;
        public EconomyService(GameState s) { state = s; }
        public ActionResult Spend(float amount, string reason)
        {
            if (amount < 0) return ActionResult.Fail("Invalid transaction.");
            if (state.money + 0.001f < amount) return ActionResult.Fail("Not enough cash.");
            state.money -= amount; Record(-amount, reason); return ActionResult.Success("Paid $" + amount.ToString("0.00") + ".");
        }
        public void Earn(float amount, string reason) { state.money += amount; Record(amount, reason); }
        private void Record(float amount, string reason) { state.ledger.Add(new LedgerEntry { day = state.day, reason = reason, amount = amount, balanceAfter = state.money }); }
        public void DailyCosts()
        {
            float cost = 9f + state.workshop.level * 3f + state.workshop.helperStaff * 18f + state.workshop.specialistStaff * 32f;
            state.money -= cost; Record(-cost, "Rent, utilities and staff");
        }
    }

    public sealed class ShippingService
    {
        private readonly GameState state;
        private readonly HardwareCatalog catalog;
        private readonly InventoryService inventory;
        private readonly EconomyService economy;
        private readonly System.Random rng = new System.Random(7331);
        public ShippingService(GameState s, HardwareCatalog c, InventoryService i, EconomyService e) { state = s; catalog = c; inventory = i; economy = e; }

        public ActionResult Buy(string definitionId, int quantity = 1, int speedTier = 1)
        {
            HardwareDefinition d = catalog.Get(definitionId);
            if (d == null || quantity <= 0) return ActionResult.Fail("Unknown store item.");
            float shipping = speedTier <= 0 ? 2f : speedTier == 1 ? 8f : 22f;
            float total = d.price * quantity + shipping;
            ActionResult pay = economy.Spend(total, "Order: " + d.model);
            if (!pay.ok) return pay;
            ShipmentState sh = new ShipmentState
            {
                shipmentId = "S" + state.nextShipmentSerial++.ToString("D5"), orderedDay = state.day,
                deliveryDay = state.day + (speedTier <= 0 ? 3 : speedTier == 1 ? 1 : 0), speedTier = speedTier,
                status = ShipmentStatus.InTransit
            };
            sh.lines.Add(new ShipmentLine { definitionId = definitionId, quantity = quantity, unitPrice = d.price });
            if (speedTier <= 0 && rng.NextDouble() < 0.07) { sh.delayed = true; sh.deliveryDay += 1; }
            state.shipments.Add(sh);
            return ActionResult.Success(d.model + " ordered. Delivery day " + sh.deliveryDay + ".");
        }

        public void Tick()
        {
            foreach (ShipmentState s in state.shipments)
                if (s.status == ShipmentStatus.InTransit && s.deliveryDay <= state.day) s.status = ShipmentStatus.Delivered;
        }

        public ActionResult Receive(string shipmentId)
        {
            ShipmentState s = state.shipments.FirstOrDefault(x => x.shipmentId == shipmentId);
            if (s == null || s.status != ShipmentStatus.Delivered) return ActionResult.Fail("Shipment is not ready for receiving.");
            foreach (ShipmentLine line in s.lines)
                for (int n = 0; n < line.quantity; n++) inventory.Create(line.definitionId);
            s.status = ShipmentStatus.Received;
            return ActionResult.Success("Shipment received into inventory.");
        }
    }

    public sealed class JobService
    {
        private readonly GameState state;
        private readonly HardwareCatalog catalog;
        private readonly InventoryService inventory;
        private readonly System.Random rng = new System.Random(20260912);
        private readonly string[] customers = { "Mira Novak", "Oleh Koval", "Nina Vale", "Artem Hrin", "Sofia Ray", "Danylo Moss", "Ira Voss", "Leo Kern" };
        public JobService(GameState s, HardwareCatalog c, InventoryService i) { state = s; catalog = c; inventory = i; }

        public void EnsureOffers(int count = 4)
        {
            while (state.jobs.Count(j => j.stage == JobStage.Offered) < count) state.jobs.Add(Generate());
        }

        private JobState Generate()
        {
            int serial = state.nextJobSerial++;
            int tier = Mathf.Clamp(1 + state.reputation / 150, 1, 5);
            JobType type = (JobType)(serial % 5);
            JobState j = new JobState
            {
                jobId = "J" + serial.ToString("D5"), customerName = customers[serial % customers.Length], type = type, stage = JobStage.Offered,
                deviceCategory = DeviceCategory.Desktop, dueDay = state.day + 4 + tier, budget = type == JobType.Repair ? 520 + tier * 120 : 720 + tier * 360,
                targetBenchmark = type == JobType.Cleaning ? 0 : type == JobType.Repair ? 225 + tier * 20 : type == JobType.Software ? 220 + tier * 25 : 360 + tier * 125, maxNoiseDb = 49 - tier,
                reward = 260 + tier * 150, requireOs = type != JobType.Cleaning, requireDrivers = type != JobType.Cleaning,
                requireClean = type == JobType.Cleaning || serial % 3 == 0, requireStable = type != JobType.Cleaning, requireNoFaults = type == JobType.Repair,
                hiddenPreferenceLowNoise = serial % 4 == 0
            };
            j.title = type == JobType.CustomBuild ? "Balanced creator PC" : type == JobType.Upgrade ? "Performance upgrade" : type == JobType.Software ? "Driver and stability service" : type == JobType.Cleaning ? "Deep clean and service" : "Unstable desktop repair";
            j.description = "Budget $" + j.budget.ToString("0") + "; target " + j.targetBenchmark + " pts; deadline day " + j.dueDay + ".";
            if (j.hiddenPreferenceLowNoise) j.optionalObjectives.Add("Keep noise below " + j.maxNoiseDb.ToString("0") + " dB");
            return j;
        }

        public ActionResult Accept(JobState j)
        {
            if (j == null || j.stage != JobStage.Offered) return ActionResult.Fail("Job is no longer available.");
            if (state.jobs.Any(x => x.stage == JobStage.Accepted || x.stage == JobStage.InProgress || x.stage == JobStage.ReadyToSubmit)) return ActionResult.Fail("Finish the active job before accepting another.");
            MachineState m = new MachineState { machineId = "M-" + j.jobId, displayName = j.customerName + " PC", ownerJobId = j.jobId, category = j.deviceCategory, dust = j.requireClean ? 0.75f : 0.12f };
            if (j.type != JobType.CustomBuild) SeedCustomerDesktop(j, m);
            state.machines.Add(m); j.machineId = m.machineId; j.stage = JobStage.Accepted;
            return ActionResult.Success("Job accepted. " + (j.type == JobType.CustomBuild ? "An empty build frame is ready." : "The customer's configured device is now at the main bench."));
        }

        private string CustomerPart(JobState j, MachineState m, string definitionId)
        {
            ItemInstance item=inventory.Create(definitionId,true);item.reserved=true;item.ownerJobId=j.jobId;item.note="Customer part in "+m.machineId;return item.instanceId;
        }

        private void SeedCustomerDesktop(JobState j, MachineState m)
        {
            m.caseItemId=CustomerPart(j,m,"case_1");m.motherboardItemId=CustomerPart(j,m,"board_5");m.cpuItemId=CustomerPart(j,m,"cpu_7");
            m.ramItemIds.Add(CustomerPart(j,m,"ram_1"));m.gpuItemId=CustomerPart(j,m,"gpu_6");m.storageItemIds.Add(CustomerPart(j,m,"storage_1"));
            RamSlotRules.Normalize(m, inventory.Def(inventory.Get(m.motherboardItemId)));
            m.psuItemId=CustomerPart(j,m,"psu_2");m.coolerItemId=CustomerPart(j,m,"cooler_1");m.fanItemIds.Add(CustomerPart(j,m,"fan_1"));
            m.cables.atx24=true;m.cables.cpuEps=true;m.cables.gpuPower=true;m.cables.sataPower=true;m.cables.sataData=true;m.cables.frontPanel=true;m.cables.cpuFan=true;
            m.thermalPasteApplied=true;m.thermalPasteQuality=.72f;m.cableManagementScore=.62f;m.sidePanelInstalled=true;m.partitioned=true;m.osInstalled=true;m.activated=true;m.driversInstalled=j.type!=JobType.Software;m.postCode="A0";m.bootState=BootState.OperatingSystem;
            if(j.type==JobType.Cleaning)m.dust=.86f;
            if(j.type==JobType.Repair)
            {
                int mode=j.jobId.Sum(ch=>(int)ch)%3;ItemInstance faulty=mode==0?inventory.Get(m.ramItemIds[0]):mode==1?inventory.Get(m.storageItemIds[0]):inventory.Get(m.gpuItemId);
                faulty.fault=mode==0?FaultType.UnstableMemory:mode==1?FaultType.BadStorage:FaultType.DeadPart;faulty.condition=.48f;m.history.Add("Customer symptom: intermittent crashes / boot instability");
            }
            if(j.type==JobType.Upgrade)m.history.Add("Customer request: preserve data while raising performance score");
            if(j.type==JobType.Software)m.history.Add("Customer request: resolve missing/outdated performance drivers");
        }

        public ActionResult Validate(JobState j, MachineState m)
        {
            if (j == null || m == null) return ActionResult.Fail("Job or device state missing.");
            if (SpecialistJobService.IsSpecialist(j)) return ActionResult.Fail("Specialist contracts must be delivered through the specialist workstation.");
            List<string> miss = new List<string>();
            if (j.requireOs && !m.osInstalled) miss.Add("OS not installed");
            if (j.requireDrivers && !m.driversInstalled) miss.Add("drivers missing");
            if (j.requireStable && !m.stressStable) miss.Add("stress test not stable");
            if (j.requireNoFaults)
            {
                IEnumerable<string> ids=new[]{m.motherboardItemId,m.cpuItemId,m.gpuItemId,m.psuItemId,m.coolerItemId}.Concat(m.ramItemIds).Concat(m.storageItemIds).Concat(m.fanItemIds);
                if(ids.Select(inventory.Get).Any(it=>it!=null&&it.fault!=FaultType.None))miss.Add("faulty component remains installed");
            }
            if (m.benchmarkScore < j.targetBenchmark) miss.Add("benchmark " + m.benchmarkScore + "/" + j.targetBenchmark);
            if (j.requireClean && m.dust > 0.12f) miss.Add("device still dusty");
            if (m.bootState == BootState.PostFailed) miss.Add("POST failure remains");
            if (miss.Count > 0) return ActionResult.Fail("Cannot submit: " + string.Join("; ", miss) + ".");
            j.stage = JobStage.ReadyToSubmit;
            return ActionResult.Success("All mandatory job criteria pass.");
        }
    }

    public sealed class DiagnosticsService
    {
        private readonly InventoryService inventory;
        private readonly PowerThermalService sim;
        public DiagnosticsService(InventoryService i, PowerThermalService s) { inventory = i; sim = s; }
        public ActionResult Scan(MachineState m)
        {
            List<string> findings = new List<string>();
            foreach (ItemInstance i in inventory.Available(PartCategory.Tool)) { }
            IEnumerable<string> ids = new [] { m.motherboardItemId, m.cpuItemId, m.gpuItemId, m.psuItemId, m.coolerItemId }.Concat(m.ramItemIds).Concat(m.storageItemIds);
            foreach (string id in ids)
            {
                ItemInstance it = inventory.Get(id);
                if (it == null) continue;
                HardwareDefinition def=inventory.Def(it);string label=def==null?it.definitionId:(def.brand+" "+def.model);
                if (it.fault != FaultType.None) findings.Add(label + ": " + it.fault);
                if (it.condition < .6f) findings.Add(label + ": worn " + (it.condition * 100f).ToString("0") + "%");
            }
            sim.Simulate(m, false);
            if (m.cpuTempC > 82) findings.Add("CPU temperature high");
            if (m.dust > .4f) findings.Add("Heavy dust accumulation");
            if (findings.Count == 0) findings.Add("No obvious hardware fault found");
            m.lastDiagnostic = string.Join(" | ", findings);
            return ActionResult.Success(m.lastDiagnostic);
        }
    }
}
