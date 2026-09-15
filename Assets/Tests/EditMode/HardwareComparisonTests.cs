using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class HardwareComparisonTests
    {
        private static HardwareCatalog Catalog() { HardwareCatalog c = new HardwareCatalog(); c.Load(); return c; }
        private static string Install(InventoryService inv, string id, MachineState m) { ItemInstance i = inv.Create(id); i.reserved = true; i.note = "Installed in " + m.machineId; return i.instanceId; }

        private static MachineState BaseMachine(GameState s, HardwareCatalog c, InventoryService inv)
        {
            HardwareDefinition board = c.ByCategory(PartCategory.Motherboard).First(); HardwareDefinition cpu = c.ByCategory(PartCategory.CPU).Where(x => x.socket == board.socket).OrderBy(x => x.performance).First(); HardwareDefinition ram = c.ByCategory(PartCategory.RAM).First(x => x.memoryType == board.memoryType); HardwareDefinition pcCase = c.ByCategory(PartCategory.Case).OrderByDescending(x => x.lengthMm).First(); HardwareDefinition psu = c.ByCategory(PartCategory.PSU).OrderBy(x => x.psuWattage).First(); HardwareDefinition cooler = c.ByCategory(PartCategory.Cooler).First();
            MachineState m = new MachineState { machineId = "CMP-PC", ownerJobId = "CMP-JOB", category = DeviceCategory.Desktop };
            m.caseItemId = Install(inv, pcCase.id, m); m.motherboardItemId = Install(inv, board.id, m); m.cpuItemId = Install(inv, cpu.id, m); m.ramItemIds.Add(Install(inv, ram.id, m)); m.ramSlotIndices.Add(1); m.psuItemId = Install(inv, psu.id, m); m.coolerItemId = Install(inv, cooler.id, m); s.machines.Add(m); return m;
        }

        private static HardwareComparisonService Service(GameState s, HardwareCatalog c, InventoryService inv)
        {
            CompatibilityService comp = new CompatibilityService(c, inv); PowerThermalService power = new PowerThermalService(c, inv); return new HardwareComparisonService(inv, new FitmentPlanningService(inv, comp, power));
        }

        [Test]
        public void DamagedCandidate_IsBlockedAndSuitabilityIsBounded()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = BaseMachine(s, c, inv); HardwareComparisonService svc = Service(s, c, inv); HardwareDefinition sata = c.ByCategory(PartCategory.Storage).First(x => x.storageInterface == "SATA"); ItemInstance candidate = inv.Create(sata.id); candidate.damage = DamageType.BurnedConnector;
            HardwareComparisonReport r = svc.Compare(m, candidate);
            Assert.AreEqual(HardwareComparisonVerdict.Blocked, r.verdict); Assert.IsFalse(r.fitment.pass); Assert.That(r.suitabilityScore, Is.InRange(0f, 100f));
        }

        [Test]
        public void StorageCandidate_IsClassifiedAsExpansion()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = BaseMachine(s, c, inv); HardwareComparisonService svc = Service(s, c, inv); HardwareDefinition sata = c.ByCategory(PartCategory.Storage).First(x => x.storageInterface == "SATA"); ItemInstance candidate = inv.Create(sata.id);
            HardwareComparisonReport r = svc.Compare(m, candidate);
            Assert.IsTrue(r.fitment.pass, string.Join(" | ", r.fitment.findings.Select(x => x.message))); Assert.AreEqual(HardwareComparisonVerdict.Expansion, r.verdict); Assert.IsTrue(r.additive); Assert.IsTrue(r.metrics.Any(x => x.label == "Capacity"));
        }

        [Test]
        public void PsuComparison_SeparatesTechnicalEfficiencyAndValueDimensions()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = BaseMachine(s, c, inv); HardwareComparisonService svc = Service(s, c, inv); HardwareDefinition strongest = c.ByCategory(PartCategory.PSU).OrderByDescending(x => x.psuWattage).ThenByDescending(x => x.efficiencyClass).First(); ItemInstance candidate = inv.Create(strongest.id);
            HardwareComparisonReport r = svc.Compare(m, candidate);
            Assert.AreNotEqual("No installed reference", r.baselineName); Assert.IsTrue(r.metrics.Any(x => x.label == "Capacity" && x.kind == HardwareMetricKind.Capacity)); Assert.IsTrue(r.metrics.Any(x => x.label == "Efficiency" && x.kind == HardwareMetricKind.Efficiency)); Assert.IsTrue(r.metrics.Any(x => x.label == "Price" && x.kind == HardwareMetricKind.Value)); Assert.That(r.technicalDelta, Is.InRange(-100f, 100f)); Assert.That(r.efficiencyDelta, Is.InRange(-100f, 100f)); Assert.That(r.valueDelta, Is.InRange(-100f, 100f));
        }

        [Test]
        public void ExpensiveTechnicalUpgrade_IsNotDowngradedByPriceMetric()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = BaseMachine(s, c, inv); HardwareComparisonService svc = Service(s, c, inv); HardwareDefinition strongest = c.ByCategory(PartCategory.PSU).OrderByDescending(x => x.psuWattage).ThenByDescending(x => x.price).First(); ItemInstance candidate = inv.Create(strongest.id);
            HardwareComparisonReport r = svc.Compare(m, candidate);
            if (r.fitment.pass && r.technicalDelta >= 7f) Assert.AreEqual(HardwareComparisonVerdict.Upgrade, r.verdict); Assert.AreNotEqual(HardwareComparisonVerdict.Downgrade, r.verdict, "Cost/value must not turn a technical upgrade into a technical downgrade.");
        }

        [Test]
        public void RequiredCategory_IsMarkedJobRelevant()
        {
            JobState job = new JobState { title = "Upgrade workstation", requiredPartCategories = new List<string> { "GPU" } };
            Assert.IsTrue(HardwareComparisonService.IsJobRelevant(job, PartCategory.GPU)); Assert.IsFalse(HardwareComparisonService.IsJobRelevant(job, PartCategory.Storage));
        }

        [Test]
        public void GamingLanguage_MapsToGpuRelevance()
        {
            JobState job = new JobState { title = "Gaming performance upgrade", description = "Improve graphics performance" };
            Assert.IsTrue(HardwareComparisonService.IsJobRelevant(job, PartCategory.GPU));
        }

        [Test]
        public void FaultedCandidate_CannotBePresentedAsUpgrade()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = BaseMachine(s, c, inv); HardwareComparisonService svc = Service(s, c, inv); HardwareDefinition psu = c.ByCategory(PartCategory.PSU).OrderByDescending(x => x.psuWattage).First(); ItemInstance candidate = inv.Create(psu.id); candidate.fault = FaultType.DeadPart;
            HardwareComparisonReport r = svc.Compare(m, candidate);
            Assert.AreEqual(HardwareComparisonVerdict.Blocked, r.verdict); Assert.IsTrue(r.fitment.findings.Any(x => x.area == "Reliability"));
        }
    }
}
