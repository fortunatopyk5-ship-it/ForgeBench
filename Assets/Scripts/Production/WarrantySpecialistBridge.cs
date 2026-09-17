using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Normalizes a warranty callback after the generic job board accepts it. Standard
    /// desktop callbacks retain the normal desktop seed; callbacks originating from a
    /// specialist discipline are converted back to that discipline so they cannot be
    /// accidentally serviced as a desktop PC.
    /// </summary>
    public sealed class WarrantySpecialistBridge : MonoBehaviour
    {
        private float nextPoll;
        private const string InitMarker = "Warranty specialist return initialized";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<WarrantySpecialistBridge>() != null) return;
            GameObject go = new GameObject("ForgeBench_WarrantySpecialistBridge");
            DontDestroyOnLoad(go);
            go.AddComponent<WarrantySpecialistBridge>();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + .25f;
            GameRuntime game = GameRuntime.Instance;
            JobState job = game?.ActiveJob;
            MachineState machine = game?.ActiveMachine;
            if (game == null || job == null || machine == null || !IsWarranty(job)) return;
            if (machine.history != null && machine.history.Contains(InitMarker)) return;

            SpecialistContractKind? kind = ResolveOriginalSpecialist(game, job);
            if (kind == null && job.deviceCategory != DeviceCategory.Desktop) kind = MapCategory(job.deviceCategory);
            if (kind == null) return; // ordinary desktop comeback uses the normal desktop repair path.

            string specialistMarker = "SPECIALIST:" + kind.Value;
            if (job.requiredPartCategories == null) job.requiredPartCategories = new List<string>();
            job.requiredPartCategories.RemoveAll(x => x.StartsWith("SPECIALIST:", StringComparison.Ordinal));
            job.requiredPartCategories.Add(specialistMarker);
            ClearGenericDesktopSeed(game, job, machine);
            InitializeSpecialistReturn(machine, kind.Value);
            machine.displayName = job.customerName + " · warranty " + job.deviceCategory;
            machine.history.Add(InitMarker);
            machine.history.Add("Comeback discipline restored from original service: " + kind.Value);
            game.Saves?.Save(game.State, 1);
            game.World?.RefreshMachine();
            game.UI?.Refresh();
            game.Notify("Warranty callback routed to " + kind.Value + " specialist workflow.");
        }

        private static SpecialistContractKind? ResolveOriginalSpecialist(GameRuntime game, JobState callback)
        {
            string warranty = callback.requiredPartCategories?.FirstOrDefault(x => x.StartsWith("WARRANTY:", StringComparison.Ordinal));
            if (string.IsNullOrEmpty(warranty)) return null;
            string caseId = warranty.Substring("WARRANTY:".Length);
            WarrantyCaseState c = WarrantyDirector.Instance?.Service?.Cases.FirstOrDefault(x => x.caseId == caseId);
            if (c == null) return null;
            JobState source = game.State.jobs.FirstOrDefault(x => x.jobId == c.sourceJobId);
            string marker = source?.requiredPartCategories?.FirstOrDefault(x => x.StartsWith("SPECIALIST:", StringComparison.Ordinal));
            if (string.IsNullOrEmpty(marker)) return null;
            SpecialistContractKind kind;
            return Enum.TryParse(marker.Substring("SPECIALIST:".Length), out kind) ? kind : (SpecialistContractKind?)null;
        }

        private static SpecialistContractKind? MapCategory(DeviceCategory category)
        {
            switch (category)
            {
                case DeviceCategory.Laptop: return SpecialistContractKind.LaptopBattery;
                case DeviceCategory.Phone:
                case DeviceCategory.Tablet: return SpecialistContractKind.PhoneDisplay;
                case DeviceCategory.Controller:
                case DeviceCategory.Handheld:
                case DeviceCategory.Console: return SpecialistContractKind.ConsoleController;
                case DeviceCategory.NAS: return SpecialistContractKind.NasRecovery;
                case DeviceCategory.Server:
                case DeviceCategory.Router: return SpecialistContractKind.ServerNetwork;
                default: return null;
            }
        }

        private static void ClearGenericDesktopSeed(GameRuntime game, JobState job, MachineState m)
        {
            foreach (ItemInstance item in game.State.inventory.Where(x => x.customerOwned && x.ownerJobId == job.jobId).ToList())
                game.State.inventory.Remove(item);
            m.caseItemId = null; m.motherboardItemId = null; m.cpuItemId = null; m.gpuItemId = null; m.psuItemId = null; m.coolerItemId = null;
            m.ramItemIds = new List<string>(); m.ramSlotIndices = new List<int>(); m.storageItemIds = new List<string>(); m.fanItemIds = new List<string>();
            m.cables = new CableState(); m.sidePanelInstalled = false; m.sidePanel = new PanelState { installed = false };
            m.partitioned = false; m.osInstalled = false; m.activated = false; m.driversInstalled = false; m.thermalPasteApplied = false; m.thermalPasteQuality = 0f;
            m.bootState = BootState.Off; m.postCode = "SPECIAL"; m.benchmarkScore = 0f; m.stressStable = false; m.dust = .16f;
        }

        private static void InitializeSpecialistReturn(MachineState m, SpecialistContractKind kind)
        {
            switch (kind)
            {
                case SpecialistContractKind.LiquidBuild:
                    m.category = DeviceCategory.Desktop;
                    m.liquidLoop = new LiquidLoopState { pumpInstalled = true, reservoirInstalled = true, radiatorMm = 360, fittingCount = 8, tightFittings = 7, coolantLitres = .72f, airFraction = .24f, flowLpm = .58f, pressureKpa = 21f, leakDetected = false, leakTestPassed = false, coolantAgeDays = 18 };
                    m.history.Add("Customer report: loop noise / flow regression after prior liquid-cooling service");
                    break;
                case SpecialistContractKind.BoardRepair:
                    m.category = DeviceCategory.Laptop;
                    m.boardRepair = new BoardRepairState { esdGrounded = false, microscopeInspected = false, powerRailMeasured = false, shortLocated = false, fluxApplied = false, solderQuality = .62f, reworkCycles = 0, padsIntact = true, repaired = false };
                    m.history.Add("Customer report: intermittent no-power recurrence after board repair");
                    break;
                case SpecialistContractKind.LaptopBattery:
                    m.category = DeviceCategory.Laptop;
                    m.portable = new PortableDeviceState { screwsRemaining = 8, backCoverRemoved = false, batteryDisconnected = false, batteryHealth = .61f, chargingPortHealth = .69f, displayHealth = .96f, adhesiveIntegrity = .92f, @sealed = true, sealQuality = .90f };
                    m.history.Add("Customer report: charging/runtime regression after portable service");
                    break;
                case SpecialistContractKind.PhoneDisplay:
                    if (m.category != DeviceCategory.Tablet) m.category = DeviceCategory.Phone;
                    m.portable = new PortableDeviceState { screwsRemaining = 2, backCoverRemoved = false, batteryDisconnected = false, batteryHealth = .84f, chargingPortHealth = .91f, displayHealth = .68f, adhesiveIntegrity = .74f, @sealed = true, sealQuality = .66f, waterDamage = .05f };
                    m.history.Add("Customer report: display/seal quality regression");
                    break;
                case SpecialistContractKind.ConsoleController:
                    m.category = DeviceCategory.Controller;
                    m.portable = new PortableDeviceState { screwsRemaining = 6, backCoverRemoved = false, batteryDisconnected = false, batteryHealth = .78f, chargingPortHealth = .88f, displayHealth = 1f, @sealed = true, sealQuality = .92f, controllerDrift = .19f };
                    m.history.Add("Customer report: analog drift returned after service");
                    break;
                case SpecialistContractKind.NasRecovery:
                    m.category = DeviceCategory.NAS;
                    m.network = new NetworkLabState { linkUp = true, dhcp = true, ipAddress = "192.168.1.10", throughputMbps = 420f, packetLoss = .035f, latencyMs = 22, raidLevel = 5, disksTotal = 4, disksHealthy = 3, arrayDegraded = true, scrubComplete = false };
                    m.history.Add("Customer report: array degraded again and transfer speed dropped");
                    break;
                case SpecialistContractKind.ServerNetwork:
                    m.category = DeviceCategory.Server;
                    m.network = new NetworkLabState { linkUp = true, dhcp = true, ipAddress = "192.168.1.10", throughputMbps = 510f, packetLoss = .045f, latencyMs = 64, raidLevel = 1, disksTotal = 2, disksHealthy = 2, arrayDegraded = false, scrubComplete = false };
                    m.history.Add("Customer report: network configuration / throughput regression");
                    break;
            }
        }

        private static bool IsWarranty(JobState j) => j?.requiredPartCategories != null && j.requiredPartCategories.Any(x => x.StartsWith("WARRANTY:", StringComparison.Ordinal));
    }
}
