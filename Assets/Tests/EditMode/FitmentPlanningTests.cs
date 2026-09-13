using System.Linq;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class FitmentPlanningTests
    {
        private static HardwareCatalog Catalog() { HardwareCatalog c = new HardwareCatalog(); c.Load(); return c; }
        private static string Install(InventoryService inv, string id, MachineState m) { ItemInstance i = inv.Create(id); i.reserved = true; i.note = "Installed in " + m.machineId; return i.instanceId; }
        private static MachineState BaseMachine(GameState s, HardwareCatalog c, InventoryService inv)
        {
            HardwareDefinition board = c.ByCategory(PartCategory.Motherboard).First(); HardwareDefinition cpu = c.ByCategory(PartCategory.CPU).First(x => x.socket == board.socket); HardwareDefinition ram = c.ByCategory(PartCategory.RAM).First(x => x.memoryType == board.memoryType);
            MachineState m = new MachineState { machineId = "FIT-PC", ownerJobId = "J1", category = DeviceCategory.Desktop };
            m.caseItemId = Install(inv, c.ByCategory(PartCategory.Case).OrderByDescending(x => x.lengthMm).First().id, m); m.motherboardItemId = Install(inv, board.id, m); m.cpuItemId = Install(inv, cpu.id, m); m.ramItemIds.Add(Install(inv, ram.id, m)); m.psuItemId = Install(inv, c.ByCategory(PartCategory.PSU).OrderByDescending(x => x.psuWattage).First().id, m); m.coolerItemId = Install(inv, c.ByCategory(PartCategory.Cooler).First().id, m); s.machines.Add(m); return m;
        }
        private static FitmentPlanningService Service(GameState s, HardwareCatalog c, InventoryService inv) { CompatibilityService comp = new CompatibilityService(c, inv); PowerThermalService p = new PowerThermalService(c, inv); return new FitmentPlanningService(inv, comp, p); }

        [Test]
        public void Candidate_FromDifferentCustomer_IsBlocked()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = BaseMachine(s, c, inv); FitmentPlanningService svc = Service(s, c, inv);
            ItemInstance candidate = inv.Create(c.ByCategory(PartCategory.Storage).First().id, true); candidate.ownerJobId = "OTHER";
            FitmentReport r = svc.EvaluateCandidate(m, candidate);
            Assert.IsFalse(r.pass); Assert.IsTrue(r.findings.Any(x => x.area == "Ownership" && x.severity == FitmentSeverity.Blocking));
        }

        [Test]
        public void Candidate_WithKnownFault_IsBlocked()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = BaseMachine(s, c, inv); FitmentPlanningService svc = Service(s, c, inv);
            ItemInstance candidate = inv.Create(c.ByCategory(PartCategory.Storage).First().id); candidate.fault = FaultType.BadStorage;
            FitmentReport r = svc.EvaluateCandidate(m, candidate);
            Assert.IsFalse(r.pass); Assert.IsTrue(r.findings.Any(x => x.area == "Reliability"));
        }

        [Test]
        public void CpuSocketMismatch_IsRejectedByCanonicalCompatibility()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = BaseMachine(s, c, inv); FitmentPlanningService svc = Service(s, c, inv); HardwareDefinition board = inv.Def(inv.Get(m.motherboardItemId)); HardwareDefinition wrong = c.ByCategory(PartCategory.CPU).FirstOrDefault(x => x.socket != board.socket); Assert.IsNotNull(wrong, "Catalog needs at least two CPU socket families for this test.");
            ItemInstance candidate = inv.Create(wrong.id); FitmentReport r = svc.EvaluateCandidate(m, candidate);
            Assert.IsFalse(r.pass); Assert.IsTrue(r.findings.Any(x => x.area == "Compatibility"));
        }

        [Test]
        public void CurrentSystemReport_CatchesInstalledSocketMismatch()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = BaseMachine(s, c, inv); FitmentPlanningService svc = Service(s, c, inv); HardwareDefinition board = inv.Def(inv.Get(m.motherboardItemId)); HardwareDefinition wrong = c.ByCategory(PartCategory.CPU).First(x => x.socket != board.socket); ItemInstance old = inv.Get(m.cpuItemId); old.reserved = false; m.cpuItemId = Install(inv, wrong.id, m);
            FitmentReport r = svc.InspectCurrent(m);
            Assert.IsFalse(r.pass); Assert.IsTrue(r.findings.Any(x => x.message.Contains("socket")));
        }

        [Test]
        public void HealthyCompatibleCandidate_ProducesScoredReport()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = BaseMachine(s, c, inv); FitmentPlanningService svc = Service(s, c, inv); ItemInstance candidate = inv.Create(c.ByCategory(PartCategory.Storage).First().id);
            FitmentReport r = svc.EvaluateCandidate(m, candidate);
            Assert.Greater(r.score, 0f); Assert.AreEqual(r.blockers == 0, r.pass);
        }
    }
}
