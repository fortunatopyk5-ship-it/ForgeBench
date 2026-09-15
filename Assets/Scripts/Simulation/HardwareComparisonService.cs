using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public enum HardwareComparisonVerdict { Blocked, Downgrade, Sidegrade, Upgrade, Expansion, NewCapability }

    public sealed class HardwareComparisonMetric
    {
        public string label;
        public string baselineText;
        public string candidateText;
        public float changePercent;
        public float contribution;
        public float weight;
        public bool higherIsBetter;
    }

    public sealed class HardwareComparisonReport
    {
        public string baselineName;
        public string candidateName;
        public PartCategory category;
        public HardwareComparisonVerdict verdict;
        public float suitabilityScore;
        public float relativeScore;
        public bool additive;
        public FitmentReport fitment;
        public readonly List<HardwareComparisonMetric> metrics = new List<HardwareComparisonMetric>();
        public string Summary
        {
            get
            {
                if (verdict == HardwareComparisonVerdict.Blocked) return "BLOCKED · " + (fitment?.blockers ?? 0) + " fitment issue(s)";
                if (verdict == HardwareComparisonVerdict.Expansion) return "EXPANSION · suitability " + suitabilityScore.ToString("0") + "/100";
                if (verdict == HardwareComparisonVerdict.NewCapability) return "NEW CAPABILITY · suitability " + suitabilityScore.ToString("0") + "/100";
                string sign = relativeScore > .05f ? "+" : string.Empty;
                return verdict.ToString().ToUpperInvariant() + " · " + sign + relativeScore.ToString("0") + " engineering delta";
            }
        }
    }

    /// <summary>
    /// Non-destructive A/B hardware comparison. It consumes the canonical inventory and fitment
    /// services so the comparison cannot silently disagree with install-time compatibility rules.
    /// </summary>
    public sealed class HardwareComparisonService
    {
        private readonly InventoryService inventory;
        private readonly FitmentPlanningService fitment;

        public HardwareComparisonService(InventoryService inv, FitmentPlanningService planner)
        {
            inventory = inv;
            fitment = planner;
        }

        public HardwareComparisonReport Compare(MachineState machine, ItemInstance candidate)
        {
            HardwareComparisonReport r = new HardwareComparisonReport();
            HardwareDefinition c = inventory.Def(candidate);
            r.candidateName = c == null ? "Unknown candidate" : Display(c);
            if (machine == null || candidate == null || c == null)
            {
                r.verdict = HardwareComparisonVerdict.Blocked;
                r.fitment = new FitmentReport { subject = r.candidateName, pass = false, blockers = 1, score = 0f };
                r.fitment.findings.Add(new FitmentFinding(FitmentSeverity.Blocking, "Comparison", "Machine or candidate is unavailable."));
                return r;
            }

            r.category = c.category;
            r.additive = IsAdditive(c.category);
            r.fitment = fitment.EvaluateCandidate(machine, candidate);
            ItemInstance baseline = FindInstalled(machine, c.category);
            HardwareDefinition b = inventory.Def(baseline);
            r.baselineName = b == null ? "No installed reference" : Display(b);

            BuildMetrics(r, b, baseline, c, candidate);
            r.relativeScore = ScoreRelative(r.metrics);
            float quality = Mathf.Clamp(c.quality, 0, 100);
            float condition = Mathf.Clamp01(candidate.condition) * 100f;
            float reliabilityPenalty = candidate.fault == FaultType.None && candidate.damage == DamageType.None ? 0f : 35f;
            r.suitabilityScore = Mathf.Clamp((r.fitment?.score ?? 0f) * .70f + quality * .20f + condition * .10f - reliabilityPenalty, 0f, 100f);

            if (r.fitment == null || !r.fitment.pass) r.verdict = HardwareComparisonVerdict.Blocked;
            else if (r.additive) r.verdict = HardwareComparisonVerdict.Expansion;
            else if (b == null) r.verdict = HardwareComparisonVerdict.NewCapability;
            else if (r.relativeScore >= 7f) r.verdict = HardwareComparisonVerdict.Upgrade;
            else if (r.relativeScore <= -7f) r.verdict = HardwareComparisonVerdict.Downgrade;
            else r.verdict = HardwareComparisonVerdict.Sidegrade;
            return r;
        }

        private ItemInstance FindInstalled(MachineState m, PartCategory c)
        {
            if (m == null) return null;
            switch (c)
            {
                case PartCategory.Case: return inventory.Get(m.caseItemId);
                case PartCategory.Motherboard: return inventory.Get(m.motherboardItemId);
                case PartCategory.CPU: return inventory.Get(m.cpuItemId);
                case PartCategory.GPU: return inventory.Get(m.gpuItemId);
                case PartCategory.PSU: return inventory.Get(m.psuItemId);
                case PartCategory.Cooler: return inventory.Get(m.coolerItemId);
                case PartCategory.RAM: return Best(m.ramItemIds);
                case PartCategory.Storage: return Best(m.storageItemIds);
                case PartCategory.Fan: return Best(m.fanItemIds);
                default: return null;
            }
        }

        private ItemInstance Best(IEnumerable<string> ids)
        {
            if (ids == null) return null;
            return ids.Select(inventory.Get).Where(x => x != null)
                .OrderByDescending(x => inventory.Def(x)?.performance ?? 0)
                .ThenByDescending(x => inventory.Def(x)?.quality ?? 0)
                .FirstOrDefault();
        }

        private static bool IsAdditive(PartCategory c)
        {
            return c == PartCategory.RAM || c == PartCategory.Storage || c == PartCategory.Fan || c == PartCategory.Network;
        }

        private void BuildMetrics(HardwareComparisonReport r, HardwareDefinition b, ItemInstance bi, HardwareDefinition c, ItemInstance ci)
        {
            HashSet<string> used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Action<string, float, float, string, bool, float> add = (label, before, after, suffix, high, weight) => Add(r, used, label, before, after, suffix, high, weight);
            switch (c.category)
            {
                case PartCategory.CPU:
                    add("Performance", b?.performance ?? 0, c.performance, " pts", true, 2.6f); add("Cores", b?.coreCount ?? 0, c.coreCount, string.Empty, true, 1.5f); add("Boost clock", b?.boostClockMHz ?? 0, c.boostClockMHz, " MHz", true, 1.1f); break;
                case PartCategory.GPU:
                    add("Performance", b?.performance ?? 0, c.performance, " pts", true, 2.7f); add("VRAM", b?.vramGB ?? 0, c.vramGB, " GB", true, 1.5f); add("Boost clock", b?.boostClockMHz ?? 0, c.boostClockMHz, " MHz", true, .8f); break;
                case PartCategory.RAM:
                    add("Capacity", b?.capacityGB ?? 0, c.capacityGB, " GB", true, 1.8f); add("Memory speed", b?.speed ?? 0, c.speed, " MT/s", true, 1.7f); break;
                case PartCategory.Storage:
                    add("Capacity", b?.storageGB ?? 0, c.storageGB, " GB", true, 1.6f); add("Read", b?.readMBs ?? 0, c.readMBs, " MB/s", true, 1.5f); add("Write", b?.writeMBs ?? 0, c.writeMBs, " MB/s", true, 1.3f); add("Endurance", b?.enduranceTBW ?? 0, c.enduranceTBW, " TBW", true, 1.0f); break;
                case PartCategory.PSU:
                    add("Capacity", b?.psuWattage ?? 0, c.psuWattage, " W", true, 2.0f); add("Efficiency", b?.efficiencyClass ?? 0, c.efficiencyClass, "%", true, 1.5f); break;
                case PartCategory.Cooler:
                    add("Cooling performance", b?.performance ?? 0, c.performance, " pts", true, 2.2f); add("Airflow", b?.airflowCfm ?? 0, c.airflowCfm, " CFM", true, 1.2f); add("Radiator support", b?.radiatorSupportMm ?? 0, c.radiatorSupportMm, " mm", true, .7f); break;
                case PartCategory.Fan:
                    add("Airflow", b?.airflowCfm ?? 0, c.airflowCfm, " CFM", true, 2.0f); add("Static pressure", b?.staticPressure ?? 0, c.staticPressure, " mmH2O", true, 1.5f); break;
                case PartCategory.Motherboard:
                    add("DIMM slots", b?.dimmSlots ?? 0, c.dimmSlots, string.Empty, true, 1.0f); add("Max memory", b?.maxMemoryGB ?? 0, c.maxMemoryGB, " GB", true, 1.2f); add("M.2 slots", b?.m2Slots ?? 0, c.m2Slots, string.Empty, true, 1.1f); add("SATA ports", b?.sataPorts ?? 0, c.sataPorts, string.Empty, true, .7f); add("Fan headers", b?.fanHeaders ?? 0, c.fanHeaders, string.Empty, true, .8f); add("PCIe generation", b?.pcieGeneration ?? 0, c.pcieGeneration, string.Empty, true, 1.0f); break;
                case PartCategory.Case:
                    add("GPU clearance", b?.lengthMm ?? 0, c.lengthMm, " mm", true, 1.4f); add("Cooler clearance", b?.heightMm ?? 0, c.heightMm, " mm", true, 1.0f); add("Radiator support", b?.radiatorSupportMm ?? 0, c.radiatorSupportMm, " mm", true, 1.2f); add("2.5-inch bays", b?.driveBays25 ?? 0, c.driveBays25, string.Empty, true, .6f); add("3.5-inch bays", b?.driveBays35 ?? 0, c.driveBays35, string.Empty, true, .6f); break;
                case PartCategory.Battery:
                    add("Capacity", b?.batteryMah ?? 0, c.batteryMah, " mAh", true, 2.1f); break;
                case PartCategory.Display:
                    add("Refresh rate", b?.displayHz ?? 0, c.displayHz, " Hz", true, 1.8f); break;
                default:
                    add("Performance", b?.performance ?? 0, c.performance, " pts", true, 1.8f); add("Speed", b?.speed ?? 0, c.speed, string.Empty, true, 1.0f); break;
            }
            add("Quality", b?.quality ?? 0, c.quality, "/100", true, 1.2f);
            add("Power draw", b?.powerWatts ?? 0, c.powerWatts, " W", false, .55f);
            add("Noise", b?.noiseDb ?? 0, c.noiseDb, " dB", false, .55f);
            add("Price", b?.price ?? 0, c.price, "$", false, .55f);
            if (bi != null) add("Condition", bi.condition * 100f, ci.condition * 100f, "%", true, 1.0f);
        }

        private static void Add(HardwareComparisonReport r, HashSet<string> used, string label, float before, float after, string suffix, bool higherIsBetter, float weight)
        {
            if (!used.Add(label)) return;
            if (Mathf.Abs(before) < .0001f && Mathf.Abs(after) < .0001f) return;
            float delta = Mathf.Abs(before) < .0001f ? 0f : Mathf.Clamp((after - before) / Mathf.Max(1f, Mathf.Abs(before)) * 100f, -200f, 200f);
            float favorable = higherIsBetter ? delta : -delta;
            float w = Mathf.Max(.01f, weight);
            r.metrics.Add(new HardwareComparisonMetric { label = label, baselineText = Format(before, suffix), candidateText = Format(after, suffix), changePercent = delta, contribution = Mathf.Clamp(favorable, -100f, 100f) * w, weight = w, higherIsBetter = higherIsBetter });
        }

        private static float ScoreRelative(List<HardwareComparisonMetric> metrics)
        {
            if (metrics == null || metrics.Count == 0) return 0f;
            float total = 0f, weight = 0f;
            foreach (HardwareComparisonMetric m in metrics) { total += m.contribution; weight += Mathf.Max(.01f, m.weight); }
            return weight <= .001f ? 0f : Mathf.Clamp(total / weight, -100f, 100f);
        }

        private static string Format(float value, string suffix)
        {
            if (suffix == "$") return "$" + value.ToString(value >= 100f ? "0" : "0.0");
            string format = Mathf.Abs(value - Mathf.Round(value)) < .01f ? "0" : "0.0";
            return value.ToString(format) + suffix;
        }

        private static string Display(HardwareDefinition d) { return d == null ? "Unknown" : (d.brand + " " + d.model).Trim(); }
    }
}
