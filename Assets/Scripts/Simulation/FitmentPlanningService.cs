using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public enum FitmentSeverity { Info, Advisory, Warning, Blocking }

    public sealed class FitmentFinding
    {
        public FitmentSeverity severity; public string area; public string message;
        public FitmentFinding(FitmentSeverity s, string a, string m) { severity = s; area = a; message = m; }
    }

    public sealed class FitmentReport
    {
        public string subject;
        public bool pass;
        public float score;
        public int blockers;
        public int warnings;
        public readonly List<FitmentFinding> findings = new List<FitmentFinding>();
        public string Summary => pass ? "FITMENT PASS · " + score.ToString("0") + "/100" : "BLOCKED · " + blockers + " issue(s)";
    }

    /// <summary>
    /// Non-destructive engineering fitment planner. It combines the canonical compatibility
    /// service with dimensional, slot, port, memory, PCIe and PSU-margin checks.
    /// </summary>
    public sealed class FitmentPlanningService
    {
        private readonly InventoryService inventory;
        private readonly CompatibilityService compatibility;
        private readonly PowerThermalService power;
        public FitmentPlanningService(InventoryService inv, CompatibilityService comp, PowerThermalService thermal) { inventory = inv; compatibility = comp; power = thermal; }
        private HardwareDefinition D(string id) => inventory.Def(inventory.Get(id));

        public FitmentReport InspectCurrent(MachineState m)
        {
            FitmentReport r = new FitmentReport { subject = m == null ? "No device" : m.displayName };
            if (m == null) { Block(r, "Bench", "No machine selected."); return Finish(r); }
            foreach (string issue in compatibility.ExplainSystem(m)) Block(r, "Compatibility", issue + ".");
            HardwareDefinition board = D(m.motherboardItemId), pcCase = D(m.caseItemId), cpu = D(m.cpuItemId), gpu = D(m.gpuItemId), cooler = D(m.coolerItemId), psu = D(m.psuItemId);
            if (board == null) Block(r, "Motherboard", "No motherboard installed.");
            if (pcCase == null && m.category == DeviceCategory.Desktop) Block(r, "Chassis", "No desktop chassis installed.");
            if (cpu == null && m.category == DeviceCategory.Desktop) Block(r, "CPU", "No processor installed.");
            CheckMemory(r, m, board, null);
            CheckStorage(r, m, board, null);
            CheckFans(r, m, board, null);
            CheckPcie(r, board, gpu);
            CheckCooling(r, pcCase, cooler);
            CheckPower(r, m, psu, null);
            if (pcCase != null && gpu != null && pcCase.lengthMm > 0f && gpu.lengthMm > pcCase.lengthMm) Block(r, "GPU clearance", gpu.lengthMm.ToString("0") + " mm GPU exceeds " + pcCase.lengthMm.ToString("0") + " mm chassis clearance.");
            if (pcCase != null && cooler != null && cooler.tags != null && cooler.tags.Contains("air") && pcCase.heightMm > 0f && cooler.heightMm > pcCase.heightMm) Block(r, "Cooler clearance", cooler.heightMm.ToString("0") + " mm cooler exceeds " + pcCase.heightMm.ToString("0") + " mm chassis limit.");
            if (board != null && !RamSlotRules.IsValid(m, board)) Block(r, "Memory layout", "DIMM positions are missing, duplicated or outside this board's slots.");
            else if (board != null && !RamSlotRules.IsSecured(m, board)) Block(r, "Memory retention", "Close both retention latches on each installed DIMM.");
            else if (board != null && m.ramItemIds.Count == 2 && !RamSlotRules.IsRecommendedPair(m, board)) Advise(r, "Memory layout", "Two-DIMM configuration is not in this board's preferred dual-channel slots.");
            return Finish(r);
        }

        public FitmentReport EvaluateCandidate(MachineState m, ItemInstance candidate)
        {
            HardwareDefinition c = inventory.Def(candidate); FitmentReport r = new FitmentReport { subject = c == null ? "Unknown candidate" : c.brand + " " + c.model };
            if (m == null) { Block(r, "Bench", "No machine selected."); return Finish(r); }
            if (candidate == null || c == null) { Block(r, "Inventory", "Candidate definition is missing."); return Finish(r); }
            ActionResult canonical = compatibility.CanInstall(m, candidate); if (!canonical.ok) Block(r, "Compatibility", canonical.message);
            HardwareDefinition board = c.category == PartCategory.Motherboard ? c : D(m.motherboardItemId);
            HardwareDefinition pcCase = c.category == PartCategory.Case ? c : D(m.caseItemId);
            HardwareDefinition gpu = c.category == PartCategory.GPU ? c : D(m.gpuItemId);
            HardwareDefinition cooler = c.category == PartCategory.Cooler ? c : D(m.coolerItemId);
            HardwareDefinition psu = c.category == PartCategory.PSU ? c : D(m.psuItemId);
            CheckMemory(r, m, board, c.category == PartCategory.RAM ? c : null);
            CheckStorage(r, m, board, c.category == PartCategory.Storage ? c : null);
            CheckFans(r, m, board, c.category == PartCategory.Fan ? c : null);
            CheckPcie(r, board, gpu);
            CheckCooling(r, pcCase, cooler);
            CheckPower(r, m, psu, c);
            if (pcCase != null && gpu != null && pcCase.lengthMm > 0f && gpu.lengthMm > pcCase.lengthMm) Block(r, "GPU clearance", gpu.lengthMm.ToString("0") + " mm GPU exceeds " + pcCase.lengthMm.ToString("0") + " mm chassis clearance.");
            else if (c.category == PartCategory.GPU && pcCase != null && pcCase.lengthMm > 0f && pcCase.lengthMm - c.lengthMm < 20f) Warn(r, "GPU clearance", "Only " + (pcCase.lengthMm - c.lengthMm).ToString("0") + " mm length margin remains for cabling/front radiator.");
            if (pcCase != null && cooler != null && cooler.tags != null && cooler.tags.Contains("air") && pcCase.heightMm > 0f && cooler.heightMm > pcCase.heightMm) Block(r, "Cooler clearance", cooler.heightMm.ToString("0") + " mm cooler exceeds " + pcCase.heightMm.ToString("0") + " mm chassis limit.");
            if (candidate.customerOwned && candidate.ownerJobId != m.ownerJobId) Block(r, "Ownership", "Part belongs to a different customer job.");
            if (candidate.condition < .50f) Warn(r, "Condition", "Candidate condition is only " + Mathf.RoundToInt(candidate.condition * 100f) + "%.");
            if (candidate.fault != FaultType.None) Block(r, "Reliability", "Candidate carries unresolved fault " + candidate.fault + ".");
            if (candidate.damage != DamageType.None) Block(r, "Damage", "Candidate carries unresolved damage " + candidate.damage + ".");
            if (r.findings.Count == 0) Info(r, "Planner", "No dimensional, electrical or interface conflict detected.");
            return Finish(r);
        }

        private void CheckMemory(FitmentReport r, MachineState m, HardwareDefinition board, HardwareDefinition added)
        {
            if (board == null) return; int installedCount = m.ramItemIds == null ? 0 : m.ramItemIds.Count; int count = installedCount + (added == null ? 0 : 1);
            if (board.dimmSlots > 0 && count > board.dimmSlots) Block(r, "DIMM slots", count + " modules exceed motherboard's " + board.dimmSlots + " slots.");
            int total = (m.ramItemIds ?? new List<string>()).Sum(id => D(id)?.capacityGB ?? 0) + (added?.capacityGB ?? 0);
            if (board.maxMemoryGB > 0 && total > board.maxMemoryGB) Block(r, "Memory capacity", total + " GB exceeds board maximum " + board.maxMemoryGB + " GB.");
            if (added != null && !string.IsNullOrEmpty(board.memoryType) && added.memoryType != board.memoryType) Block(r, "Memory generation", added.memoryType + " does not match " + board.memoryType + ".");
            if (count == 1 && board.memoryChannels >= 2) Advise(r, "Memory channels", "One DIMM leaves multi-channel memory bandwidth unused.");
        }

        private void CheckStorage(FitmentReport r, MachineState m, HardwareDefinition board, HardwareDefinition added)
        {
            if (board == null) return; IEnumerable<HardwareDefinition> defs = (m.storageItemIds ?? new List<string>()).Select(D).Where(x => x != null); int nvme = defs.Count(x => x.storageInterface == "NVMe") + (added != null && added.storageInterface == "NVMe" ? 1 : 0); int sata = defs.Count(x => x.storageInterface == "SATA") + (added != null && added.storageInterface == "SATA" ? 1 : 0);
            if (board.m2Slots > 0 && nvme > board.m2Slots) Block(r, "M.2 slots", nvme + " NVMe drives exceed " + board.m2Slots + " M.2 slots.");
            if (board.sataPorts > 0 && sata > board.sataPorts) Block(r, "SATA ports", sata + " SATA drives exceed " + board.sataPorts + " ports.");
            if (added != null && added.storageInterface == "NVMe" && !board.connectors.Contains("M2")) Block(r, "Storage interface", "Motherboard has no M.2/NVMe connector metadata.");
            if (added != null && added.storageInterface == "SATA" && !board.connectors.Contains("SATA")) Block(r, "Storage interface", "Motherboard has no SATA connector metadata.");
        }

        private void CheckFans(FitmentReport r, MachineState m, HardwareDefinition board, HardwareDefinition added)
        {
            if (board == null || board.fanHeaders <= 0) return; int fans = (m.fanItemIds?.Count ?? 0) + (added == null ? 0 : 1); int headersNeeded = fans + (string.IsNullOrEmpty(m.coolerItemId) ? 0 : 1);
            if (headersNeeded > board.fanHeaders) Warn(r, "Fan headers", headersNeeded + " fan/control leads for " + board.fanHeaders + " headers; splitter/hub required.");
        }

        private static void CheckPcie(FitmentReport r, HardwareDefinition board, HardwareDefinition gpu)
        {
            if (board == null || gpu == null) return; if (board.pcieX16Slots > 0 && board.pcieX16Slots < 1) Block(r, "PCIe slot", "No x16 graphics slot available.");
            if (board.pcieGeneration > 0 && gpu.pcieGeneration > board.pcieGeneration) Advise(r, "PCIe generation", "GPU Gen " + gpu.pcieGeneration + " will negotiate down to board Gen " + board.pcieGeneration + ".");
        }

        private static void CheckCooling(FitmentReport r, HardwareDefinition pcCase, HardwareDefinition cooler)
        {
            if (pcCase == null || cooler == null) return; if (cooler.tags != null && cooler.tags.Contains("aio") && pcCase.radiatorSupportMm > 0 && cooler.radiatorSupportMm > 0 && cooler.radiatorSupportMm > pcCase.radiatorSupportMm) Block(r, "Radiator", cooler.radiatorSupportMm + " mm radiator exceeds chassis support " + pcCase.radiatorSupportMm + " mm.");
        }

        private void CheckPower(FitmentReport r, MachineState m, HardwareDefinition psu, HardwareDefinition candidate)
        {
            float demand = power.EstimatePower(m); if (candidate != null && candidate.category != PartCategory.PSU)
            {
                HardwareDefinition old = ExistingOfCategory(m, candidate.category); demand = Mathf.Max(0f, demand - (old?.powerWatts ?? 0f) + candidate.powerWatts);
            }
            float cap = psu?.psuWattage ?? 0f; if (cap <= 0f) { if (candidate != null && candidate.category != PartCategory.PSU) Advise(r, "Power", "No PSU installed yet; power margin cannot be certified."); return; }
            float ratio = demand <= 0f ? 9f : cap / demand; if (ratio < 1.10f) Block(r, "PSU headroom", "Estimated " + demand.ToString("0") + " W load leaves less than 10% PSU margin on " + cap.ToString("0") + " W unit.");
            else if (ratio < 1.25f) Warn(r, "PSU headroom", "Estimated " + demand.ToString("0") + " W load leaves less than preferred 25% margin.");
            else Info(r, "Power", "Estimated load " + demand.ToString("0") + " W / PSU " + cap.ToString("0") + " W (" + Mathf.RoundToInt((ratio - 1f) * 100f) + "% headroom)." );
        }

        private HardwareDefinition ExistingOfCategory(MachineState m, PartCategory c)
        {
            switch (c)
            {
                case PartCategory.Case: return D(m.caseItemId); case PartCategory.Motherboard: return D(m.motherboardItemId); case PartCategory.CPU: return D(m.cpuItemId); case PartCategory.GPU: return D(m.gpuItemId); case PartCategory.PSU: return D(m.psuItemId); case PartCategory.Cooler: return D(m.coolerItemId); default: return null;
            }
        }

        private static FitmentReport Finish(FitmentReport r)
        {
            r.blockers = r.findings.Count(x => x.severity == FitmentSeverity.Blocking); r.warnings = r.findings.Count(x => x.severity == FitmentSeverity.Warning); int advisory = r.findings.Count(x => x.severity == FitmentSeverity.Advisory); r.pass = r.blockers == 0; r.score = Mathf.Clamp(100f - r.blockers * 30f - r.warnings * 8f - advisory * 3f, 0f, 100f); return r;
        }
        private static void Block(FitmentReport r, string a, string m) => r.findings.Add(new FitmentFinding(FitmentSeverity.Blocking, a, m));
        private static void Warn(FitmentReport r, string a, string m) => r.findings.Add(new FitmentFinding(FitmentSeverity.Warning, a, m));
        private static void Advise(FitmentReport r, string a, string m) => r.findings.Add(new FitmentFinding(FitmentSeverity.Advisory, a, m));
        private static void Info(FitmentReport r, string a, string m) => r.findings.Add(new FitmentFinding(FitmentSeverity.Info, a, m));
    }
}
