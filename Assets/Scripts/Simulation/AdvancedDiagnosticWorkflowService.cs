using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public enum DiagnosticProbeKind { VisualInspection, CableContinuity, PowerBudget, MemoryIntegrity, StorageHealth, ThermalLoad, CoolingAndFans, BootTrace, NetworkHealth }
    public enum DiagnosticSeverity { Pass, Advisory, Warning, Critical }

    public sealed class DiagnosticEvidence
    {
        public DiagnosticProbeKind probe;
        public DiagnosticSeverity severity;
        public string headline;
        public string summary;
        public float confidence;
        public readonly List<string> findings = new List<string>();
        public readonly List<string> likelyCauses = new List<string>();
        public bool Passed => severity == DiagnosticSeverity.Pass || severity == DiagnosticSeverity.Advisory;
    }

    public sealed class AdvancedDiagnosticWorkflowService
    {
        private readonly InventoryService inventory;
        private readonly PowerThermalService powerThermal;
        private readonly BootService boot;

        public AdvancedDiagnosticWorkflowService(InventoryService inv, PowerThermalService thermal, BootService bootService)
        {
            inventory = inv; powerThermal = thermal; boot = bootService;
        }

        public DiagnosticEvidence Run(MachineState m, DiagnosticProbeKind probe)
        {
            if (m == null) return Critical(probe, "NO DEVICE", "No machine is available on the selected bench.", "Select an active customer device.");
            DiagnosticEvidence e;
            switch (probe)
            {
                case DiagnosticProbeKind.VisualInspection: e = Visual(m); break;
                case DiagnosticProbeKind.CableContinuity: e = Continuity(m); break;
                case DiagnosticProbeKind.PowerBudget: e = Power(m); break;
                case DiagnosticProbeKind.MemoryIntegrity: e = Memory(m); break;
                case DiagnosticProbeKind.StorageHealth: e = Storage(m); break;
                case DiagnosticProbeKind.ThermalLoad: e = Thermals(m); break;
                case DiagnosticProbeKind.CoolingAndFans: e = Cooling(m); break;
                case DiagnosticProbeKind.BootTrace: e = BootTrace(m); break;
                default: e = Network(m); break;
            }
            Record(m, e);
            return e;
        }

        public List<DiagnosticEvidence> RunNonDestructiveSuite(MachineState m)
        {
            DiagnosticProbeKind[] probes =
            {
                DiagnosticProbeKind.VisualInspection, DiagnosticProbeKind.CableContinuity,
                DiagnosticProbeKind.PowerBudget, DiagnosticProbeKind.MemoryIntegrity,
                DiagnosticProbeKind.StorageHealth, DiagnosticProbeKind.ThermalLoad,
                DiagnosticProbeKind.CoolingAndFans, DiagnosticProbeKind.NetworkHealth
            };
            return probes.Select(p => Run(m, p)).ToList();
        }

        public DiagnosticEvidence Summarize(IEnumerable<DiagnosticEvidence> results)
        {
            List<DiagnosticEvidence> list = results == null ? new List<DiagnosticEvidence>() : results.Where(x => x != null).ToList();
            if (list.Count == 0) return Critical(DiagnosticProbeKind.VisualInspection, "NO RESULTS", "No diagnostic evidence has been collected.", "Run at least one probe.");
            int critical = list.Count(x => x.severity == DiagnosticSeverity.Critical);
            int warning = list.Count(x => x.severity == DiagnosticSeverity.Warning);
            int advisory = list.Count(x => x.severity == DiagnosticSeverity.Advisory);
            DiagnosticSeverity severity = critical > 0 ? DiagnosticSeverity.Critical : warning > 0 ? DiagnosticSeverity.Warning : advisory > 0 ? DiagnosticSeverity.Advisory : DiagnosticSeverity.Pass;
            DiagnosticEvidence e = new DiagnosticEvidence
            {
                probe = DiagnosticProbeKind.VisualInspection,
                severity = severity,
                headline = critical > 0 ? "ROOT-CAUSE WORK REQUIRED" : warning > 0 ? "DIAGNOSTIC WARNINGS" : advisory > 0 ? "SERVICE ADVISORIES" : "DIAGNOSTIC SUITE PASS",
                summary = list.Count + " probes · " + critical + " critical · " + warning + " warning · " + advisory + " advisory",
                confidence = Mathf.Clamp01(.55f + list.Count * .055f)
            };
            foreach (string cause in list.SelectMany(x => x.likelyCauses).Where(x => !string.IsNullOrEmpty(x)).Distinct().Take(8)) e.likelyCauses.Add(cause);
            foreach (DiagnosticEvidence r in list.Where(x => x.severity == DiagnosticSeverity.Warning || x.severity == DiagnosticSeverity.Critical)) e.findings.Add(r.probe + ": " + r.summary);
            return e;
        }

        private DiagnosticEvidence Visual(MachineState m)
        {
            DiagnosticEvidence e = Pass(DiagnosticProbeKind.VisualInspection, "VISUAL CHECK PASS", "No obvious visual defect detected.");
            if (m.dust > .65f) Add(e, DiagnosticSeverity.Warning, "Heavy dust accumulation: " + Mathf.RoundToInt(m.dust * 100f) + "%.", "Restricted airflow / contamination");
            else if (m.dust > .30f) Add(e, DiagnosticSeverity.Advisory, "Moderate dust accumulation: " + Mathf.RoundToInt(m.dust * 100f) + "%.", "Preventive cleaning due");
            if (m.maintenance != null && m.maintenance.corrosion > .25f) Add(e, DiagnosticSeverity.Critical, "Visible corrosion indicator above service threshold.", "Liquid / environmental contamination");
            if (m.sidePanel != null && m.sidePanel.clipDamaged) Add(e, DiagnosticSeverity.Warning, "Side-panel retaining clip is damaged.", "Mechanical enclosure damage");
            foreach (ItemInstance item in Installed(m))
            {
                HardwareDefinition d = inventory.Def(item); string label = d == null ? item.definitionId : d.brand + " " + d.model;
                if (item.damage != DamageType.None) Add(e, DiagnosticSeverity.Critical, label + ": " + item.damage + ".", "Physical component damage");
                else if (item.condition < .45f) Add(e, DiagnosticSeverity.Warning, label + " condition " + Mathf.RoundToInt(item.condition * 100f) + "%.", "Wear / aging");
            }
            Normalize(e); return e;
        }

        private DiagnosticEvidence Continuity(MachineState m)
        {
            DiagnosticEvidence e = Pass(DiagnosticProbeKind.CableContinuity, "CONTINUITY PASS", "Required desktop signal paths appear connected.");
            if (m.category != DeviceCategory.Desktop) { e.severity = DiagnosticSeverity.Advisory; e.headline = "LIMITED CONTINUITY MAP"; e.summary = "Desktop harness map does not apply to this device category."; return e; }
            CableState c = m.cables ?? new CableState();
            if (!c.atx24) Add(e, DiagnosticSeverity.Critical, "24-pin ATX path is open.", "Missing ATX main power cable");
            if (!c.cpuEps) Add(e, DiagnosticSeverity.Critical, "CPU EPS path is open.", "Missing CPU power cable");
            if (!c.frontPanel) Add(e, DiagnosticSeverity.Warning, "Front-panel power-switch path is open.", "Front-panel header wiring");
            if (!c.cpuFan) Add(e, DiagnosticSeverity.Warning, "CPU_FAN tach/control path is open.", "Cooler header connection");
            HardwareDefinition gpu = inventory.Def(inventory.Get(m.gpuItemId));
            if (gpu != null && gpu.powerWatts >= 180f && !c.gpuPower) Add(e, DiagnosticSeverity.Critical, "GPU auxiliary power path is open.", "GPU power cable");
            bool sata = (m.storageItemIds ?? new List<string>()).Any(x => inventory.Def(inventory.Get(x))?.storageInterface == "SATA");
            if (sata && (!c.sataPower || !c.sataData)) Add(e, DiagnosticSeverity.Critical, "SATA device has incomplete power/data continuity.", "SATA harness");
            Normalize(e); return e;
        }

        private DiagnosticEvidence Power(MachineState m)
        {
            float demand = powerThermal.EstimatePower(m), capacity = powerThermal.PsuCapacity(m);
            float headroom = capacity - demand;
            DiagnosticEvidence e = Pass(DiagnosticProbeKind.PowerBudget, "POWER BUDGET PASS", "Estimated load " + demand.ToString("0") + " W / PSU " + capacity.ToString("0") + " W.");
            if (capacity <= 0f) Add(e, DiagnosticSeverity.Critical, "No valid PSU capacity detected.", "PSU missing / unidentified");
            else if (capacity < demand * 1.10f) Add(e, DiagnosticSeverity.Critical, "Power headroom only " + headroom.ToString("0") + " W; POST/load margin unsafe.", "Undersized PSU");
            else if (capacity < demand * 1.25f) Add(e, DiagnosticSeverity.Warning, "Power headroom is below preferred 25% engineering margin.", "Marginal PSU sizing");
            PowerRuntimeState p = m.powerState;
            if (p != null)
            {
                if (p.ocpTriggered) Add(e, DiagnosticSeverity.Critical, "OCP event is stored in power telemetry.", "Over-current / transient overload");
                if (!p.stable) Add(e, DiagnosticSeverity.Critical, "Power telemetry reports unstable rails.", "PSU / connector / load instability");
                if (p.rail12V > 0f && (p.rail12V < 11.4f || p.rail12V > 12.6f)) Add(e, DiagnosticSeverity.Critical, "12 V rail measured " + p.rail12V.ToString("0.00") + " V.", "12 V rail regulation fault");
                if (p.rippleMv > 120f) Add(e, DiagnosticSeverity.Critical, "Ripple " + p.rippleMv.ToString("0") + " mV exceeds 120 mV.", "PSU ripple / capacitor degradation");
                else if (p.rippleMv > 80f) Add(e, DiagnosticSeverity.Warning, "Ripple elevated at " + p.rippleMv.ToString("0") + " mV.", "Aging PSU / transient loading");
            }
            Normalize(e); return e;
        }

        private DiagnosticEvidence Memory(MachineState m)
        {
            DiagnosticEvidence e = Pass(DiagnosticProbeKind.MemoryIntegrity, "MEMORY CHECK PASS", (m.ramItemIds?.Count ?? 0) + " DIMM(s) inspected.");
            if (m.ramItemIds == null || m.ramItemIds.Count == 0) Add(e, DiagnosticSeverity.Critical, "No memory installed.", "Missing DIMM");
            else
            {
                foreach (string id in m.ramItemIds)
                {
                    ItemInstance item = inventory.Get(id); HardwareDefinition d = inventory.Def(item); string label = d == null ? id : d.model;
                    if (item == null) { Add(e, DiagnosticSeverity.Critical, "Inventory reference " + id + " is missing.", "Corrupt/missing DIMM instance"); continue; }
                    if (item.fault == FaultType.UnstableMemory || item.fault == FaultType.DeadPart) Add(e, DiagnosticSeverity.Critical, label + " reports " + item.fault + ".", "Faulty memory module");
                    if (item.condition < .50f) Add(e, DiagnosticSeverity.Warning, label + " condition " + Mathf.RoundToInt(item.condition * 100f) + "%.", "DIMM degradation");
                }
            }
            if (m.bios != null && !m.bios.lastTrainingPassed) Add(e, DiagnosticSeverity.Critical, "Firmware memory training failed: " + m.bios.lastTrainingMessage + ".", "Memory timing / module / slot instability");
            if (m.benchmarkState != null && m.benchmarkState.memoryErrors > 0) Add(e, DiagnosticSeverity.Critical, m.benchmarkState.memoryErrors + " memory error(s) recorded by stress telemetry.", "Memory integrity failure");
            Normalize(e); return e;
        }

        private DiagnosticEvidence Storage(MachineState m)
        {
            DiagnosticEvidence e = Pass(DiagnosticProbeKind.StorageHealth, "STORAGE HEALTH PASS", (m.storageItemIds?.Count ?? 0) + " drive(s) inspected.");
            if (m.storageItemIds == null || m.storageItemIds.Count == 0) Add(e, DiagnosticSeverity.Advisory, "No storage device installed.", "No boot/storage media");
            else foreach (string id in m.storageItemIds)
            {
                ItemInstance item = inventory.Get(id); HardwareDefinition d = inventory.Def(item); string label = d == null ? id : d.model;
                if (item == null) { Add(e, DiagnosticSeverity.Critical, "Drive reference " + id + " is missing.", "Missing storage instance"); continue; }
                if (item.fault == FaultType.BadStorage || item.fault == FaultType.DeadPart) Add(e, DiagnosticSeverity.Critical, label + " reports " + item.fault + ".", "Storage media failure");
                if (item.condition < .40f) Add(e, DiagnosticSeverity.Critical, label + " health " + Mathf.RoundToInt(item.condition * 100f) + "%.", "Storage wear / failure risk");
                else if (item.condition < .65f || item.wear > .70f) Add(e, DiagnosticSeverity.Warning, label + " wear/condition is outside preferred service range.", "Storage wear");
                if (d != null && d.enduranceTBW > 0 && item.writtenGB / 1024f > d.enduranceTBW * .85f) Add(e, DiagnosticSeverity.Warning, label + " host writes approach rated endurance.", "Flash endurance consumption");
            }
            Normalize(e); return e;
        }

        private DiagnosticEvidence Thermals(MachineState m)
        {
            powerThermal.Simulate(m, true);
            DiagnosticEvidence e = Pass(DiagnosticProbeKind.ThermalLoad, "THERMAL LOAD PASS", "CPU " + m.cpuTempC.ToString("0") + "°C · GPU " + m.gpuTempC.ToString("0") + "°C · " + m.systemPowerW.ToString("0") + " W.");
            if (m.cpuTempC >= 96f) Add(e, DiagnosticSeverity.Critical, "CPU reached " + m.cpuTempC.ToString("0") + "°C.", "CPU cooling / paste / airflow");
            else if (m.cpuTempC >= 88f) Add(e, DiagnosticSeverity.Warning, "CPU temperature elevated at " + m.cpuTempC.ToString("0") + "°C.", "Cooling margin");
            if (m.gpuTempC >= 94f) Add(e, DiagnosticSeverity.Critical, "GPU reached " + m.gpuTempC.ToString("0") + "°C.", "GPU cooling / case airflow");
            else if (m.gpuTempC >= 86f) Add(e, DiagnosticSeverity.Warning, "GPU temperature elevated at " + m.gpuTempC.ToString("0") + "°C.", "GPU/case airflow margin");
            if (!m.thermalPasteApplied) Add(e, DiagnosticSeverity.Critical, "CPU thermal interface material is missing.", "Missing thermal compound");
            if (m.thermalState != null && (m.thermalState.cpuThrottling || m.thermalState.gpuThrottling)) Add(e, DiagnosticSeverity.Critical, "Stored thermal telemetry reports throttling.", "Thermal saturation");
            Normalize(e); return e;
        }

        private DiagnosticEvidence Cooling(MachineState m)
        {
            DiagnosticEvidence e = Pass(DiagnosticProbeKind.CoolingAndFans, "COOLING PATH PASS", (m.fanItemIds?.Count ?? 0) + " case fan(s) plus CPU cooling path inspected.");
            if (string.IsNullOrEmpty(m.coolerItemId)) Add(e, DiagnosticSeverity.Critical, "No CPU cooler installed.", "Missing cooling device");
            if (m.category == DeviceCategory.Desktop && (m.cables == null || !m.cables.cpuFan)) Add(e, DiagnosticSeverity.Critical, "CPU_FAN signal is absent.", "Fan header / tach connection");
            foreach (string id in m.fanItemIds ?? new List<string>())
            {
                ItemInstance item = inventory.Get(id); HardwareDefinition d = inventory.Def(item); string label = d == null ? id : d.model;
                if (item != null && item.fault == FaultType.FanFailure) Add(e, DiagnosticSeverity.Critical, label + " has stored fan failure.", "Failed case fan");
                if (item != null && item.condition < .45f) Add(e, DiagnosticSeverity.Warning, label + " bearing/condition below 45%.", "Fan bearing wear");
            }
            if (m.dust > .55f) Add(e, DiagnosticSeverity.Warning, "Dust restriction is high enough to affect airflow.", "Blocked filter/heatsink");
            if (m.thermalState != null && m.thermalState.airflowCfm > 0 && m.thermalState.airflowCfm < 20f) Add(e, DiagnosticSeverity.Warning, "Simulated airflow only " + m.thermalState.airflowCfm.ToString("0") + " CFM.", "Insufficient airflow");
            Normalize(e); return e;
        }

        private DiagnosticEvidence BootTrace(MachineState m)
        {
            ActionResult r = boot.PowerOn(m);
            DiagnosticEvidence e = r.ok ? Pass(DiagnosticProbeKind.BootTrace, "POST / BOOT PASS", r.message) : Critical(DiagnosticProbeKind.BootTrace, "POST / BOOT FAILURE", r.message, CauseFromPost(m.postCode));
            e.findings.Add("POST code: " + m.postCode + " · boot state: " + m.bootState);
            e.confidence = r.ok ? .94f : .88f;
            Normalize(e); return e;
        }

        private DiagnosticEvidence Network(MachineState m)
        {
            if (m.category != DeviceCategory.NAS && m.category != DeviceCategory.Server && m.category != DeviceCategory.Router)
                return new DiagnosticEvidence { probe = DiagnosticProbeKind.NetworkHealth, severity = DiagnosticSeverity.Advisory, headline = "NETWORK PROBE N/A", summary = "No dedicated network-appliance workflow is attached to this device.", confidence = .95f };
            NetworkLabState n = m.network ?? new NetworkLabState();
            DiagnosticEvidence e = Pass(DiagnosticProbeKind.NetworkHealth, "NETWORK HEALTH PASS", "Link " + (n.linkUp ? "UP" : "DOWN") + " · " + n.throughputMbps.ToString("0") + " Mbps · " + (n.packetLoss * 100f).ToString("0.0") + "% loss.");
            if (!n.linkUp) Add(e, DiagnosticSeverity.Critical, "Network link is down.", "Physical link / transceiver / cable");
            if (n.packetLoss > .02f) Add(e, DiagnosticSeverity.Critical, "Packet loss exceeds 2%.", "Link integrity / congestion");
            else if (n.packetLoss > .005f) Add(e, DiagnosticSeverity.Warning, "Packet loss is elevated.", "Link quality");
            if (n.latencyMs > 50) Add(e, DiagnosticSeverity.Warning, "Latency elevated at " + n.latencyMs + " ms.", "Network path / load");
            if (m.category == DeviceCategory.NAS && n.arrayDegraded) Add(e, DiagnosticSeverity.Critical, "Storage array is degraded.", "RAID member failure");
            Normalize(e); return e;
        }

        private IEnumerable<ItemInstance> Installed(MachineState m)
        {
            IEnumerable<string> ids = new[] { m.caseItemId, m.motherboardItemId, m.cpuItemId, m.gpuItemId, m.psuItemId, m.coolerItemId }
                .Concat(m.ramItemIds ?? new List<string>()).Concat(m.storageItemIds ?? new List<string>()).Concat(m.fanItemIds ?? new List<string>());
            foreach (string id in ids.Where(x => !string.IsNullOrEmpty(x)).Distinct()) { ItemInstance i = inventory.Get(id); if (i != null) yield return i; }
        }

        private static string CauseFromPost(string code)
        {
            switch (code)
            {
                case "DRAM": return "Memory module / slot / training"; case "VGA": case "VGA-P": return "GPU / video / auxiliary power";
                case "EPS": case "24P": case "PWR": case "OCP": return "Power delivery / PSU / cabling"; case "CPU": return "CPU presence / socket";
                case "FAN": case "CPUF": case "TEMP": return "CPU cooling / thermal protection"; case "FPIO": return "Front-panel wiring"; case "HW": return "Critical component self-test"; default: return "POST dependency chain";
            }
        }

        private static DiagnosticEvidence Pass(DiagnosticProbeKind p, string h, string s) => new DiagnosticEvidence { probe = p, severity = DiagnosticSeverity.Pass, headline = h, summary = s, confidence = .82f };
        private static DiagnosticEvidence Critical(DiagnosticProbeKind p, string h, string s, string cause) { DiagnosticEvidence e = new DiagnosticEvidence { probe = p, severity = DiagnosticSeverity.Critical, headline = h, summary = s, confidence = .80f }; if (!string.IsNullOrEmpty(cause)) e.likelyCauses.Add(cause); return e; }
        private static void Add(DiagnosticEvidence e, DiagnosticSeverity severity, string finding, string cause)
        {
            if ((int)severity > (int)e.severity) e.severity = severity; e.findings.Add(finding); if (!string.IsNullOrEmpty(cause) && !e.likelyCauses.Contains(cause)) e.likelyCauses.Add(cause);
        }
        private static void Normalize(DiagnosticEvidence e)
        {
            if (e.findings.Count == 0) return; e.summary = string.Join(" ", e.findings.Take(3)); e.confidence = Mathf.Clamp01(.72f + e.findings.Count * .045f);
            if (e.severity == DiagnosticSeverity.Critical) e.headline = e.probe.ToString().ToUpperInvariant() + " · CRITICAL";
            else if (e.severity == DiagnosticSeverity.Warning) e.headline = e.probe.ToString().ToUpperInvariant() + " · WARNING";
            else if (e.severity == DiagnosticSeverity.Advisory) e.headline = e.probe.ToString().ToUpperInvariant() + " · ADVISORY";
        }
        private static void Record(MachineState m, DiagnosticEvidence e)
        {
            if (m.history == null) m.history = new List<string>(); string row = "DIAG " + e.probe + " [" + e.severity + "]: " + e.summary; m.lastDiagnostic = row; m.history.Add(row); if (m.history.Count > 120) m.history.RemoveRange(0, m.history.Count - 120);
        }
    }
}
