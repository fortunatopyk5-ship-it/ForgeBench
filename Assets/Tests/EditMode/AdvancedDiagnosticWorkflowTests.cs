using System;
using System.Linq;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class AdvancedDiagnosticWorkflowTests
    {
        private static HardwareCatalog Catalog() { HardwareCatalog c = new HardwareCatalog(); c.Load(); return c; }
        private static MachineState Machine(GameState state, HardwareCatalog catalog, InventoryService inv)
        {
            MachineState m = new MachineState { machineId = "DIAG-PC", category = DeviceCategory.Desktop, sidePanelInstalled = true, thermalPasteApplied = true, thermalPasteQuality = .9f, cableManagementScore = .8f };
            HardwareDefinition board = catalog.ByCategory(PartCategory.Motherboard).First();
            HardwareDefinition cpu = catalog.ByCategory(PartCategory.CPU).First(x => string.IsNullOrEmpty(board.socket) || x.socket == board.socket);
            HardwareDefinition ram = catalog.ByCategory(PartCategory.RAM).First(x => string.IsNullOrEmpty(board.memoryType) || x.memoryType == board.memoryType);
            HardwareDefinition pcCase = catalog.ByCategory(PartCategory.Case).First();
            HardwareDefinition psu = catalog.ByCategory(PartCategory.PSU).OrderByDescending(x => x.psuWattage).First();
            HardwareDefinition cooler = catalog.ByCategory(PartCategory.Cooler).First();
            HardwareDefinition storage = catalog.ByCategory(PartCategory.Storage).First();
            m.caseItemId = Install(inv, pcCase.id, m); m.motherboardItemId = Install(inv, board.id, m); m.cpuItemId = Install(inv, cpu.id, m); m.ramItemIds.Add(Install(inv, ram.id, m));
            m.psuItemId = Install(inv, psu.id, m); m.coolerItemId = Install(inv, cooler.id, m); m.storageItemIds.Add(Install(inv, storage.id, m));
            m.cables.atx24 = true; m.cables.cpuEps = true; m.cables.frontPanel = true; m.cables.cpuFan = true; m.cables.gpuPower = true; m.cables.sataPower = true; m.cables.sataData = true;
            state.machines.Add(m); return m;
        }
        private static string Install(InventoryService inv, string id, MachineState m) { ItemInstance i = inv.Create(id); i.reserved = true; i.note = "Installed in " + m.machineId; return i.instanceId; }
        private static AdvancedDiagnosticWorkflowService Service(GameState s, HardwareCatalog c, InventoryService inv)
        {
            PowerThermalService thermal = new PowerThermalService(c, inv); BootService boot = new BootService(inv, thermal); return new AdvancedDiagnosticWorkflowService(inv, thermal, boot);
        }

        [Test]
        public void Continuity_ReportsMissingCriticalPowerPaths()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = Machine(s, c, inv); AdvancedDiagnosticWorkflowService svc = Service(s, c, inv);
            m.cables.atx24 = false; m.cables.cpuEps = false;
            DiagnosticEvidence e = svc.Run(m, DiagnosticProbeKind.CableContinuity);
            Assert.AreEqual(DiagnosticSeverity.Critical, e.severity); Assert.IsTrue(e.findings.Any(x => x.Contains("24-pin"))); Assert.IsTrue(e.findings.Any(x => x.Contains("EPS")));
        }

        [Test]
        public void MemoryProbe_UsesPersistentItemFaultState()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = Machine(s, c, inv); AdvancedDiagnosticWorkflowService svc = Service(s, c, inv);
            inv.Get(m.ramItemIds[0]).fault = FaultType.UnstableMemory; inv.Get(m.ramItemIds[0]).condition = .42f;
            DiagnosticEvidence e = svc.Run(m, DiagnosticProbeKind.MemoryIntegrity);
            Assert.AreEqual(DiagnosticSeverity.Critical, e.severity); Assert.IsTrue(e.likelyCauses.Any(x => x.IndexOf("memory", StringComparison.OrdinalIgnoreCase) >= 0));
        }

        [Test]
        public void ThermalProbe_RunsRealSimulationAndRejectsMissingPaste()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = Machine(s, c, inv); AdvancedDiagnosticWorkflowService svc = Service(s, c, inv);
            m.thermalPasteApplied = false;
            DiagnosticEvidence e = svc.Run(m, DiagnosticProbeKind.ThermalLoad);
            Assert.AreEqual(DiagnosticSeverity.Critical, e.severity); Assert.Greater(m.cpuTempC, 0f); Assert.IsTrue(m.lastDiagnostic.Contains("ThermalLoad"));
        }

        [Test]
        public void StorageProbe_CombinesFaultAndConditionEvidence()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = Machine(s, c, inv); AdvancedDiagnosticWorkflowService svc = Service(s, c, inv);
            ItemInstance disk = inv.Get(m.storageItemIds[0]); disk.fault = FaultType.BadStorage; disk.condition = .31f; disk.wear = .88f;
            DiagnosticEvidence e = svc.Run(m, DiagnosticProbeKind.StorageHealth);
            Assert.AreEqual(DiagnosticSeverity.Critical, e.severity); Assert.GreaterOrEqual(e.findings.Count, 2);
        }

        [Test]
        public void NetworkProbe_RejectsDegradedNasArray()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); AdvancedDiagnosticWorkflowService svc = Service(s, c, inv);
            MachineState m = new MachineState { machineId = "NAS", category = DeviceCategory.NAS, network = new NetworkLabState { linkUp = true, throughputMbps = 900f, packetLoss = 0f, latencyMs = 4, arrayDegraded = true } };
            s.machines.Add(m); DiagnosticEvidence e = svc.Run(m, DiagnosticProbeKind.NetworkHealth);
            Assert.AreEqual(DiagnosticSeverity.Critical, e.severity); Assert.IsTrue(e.likelyCauses.Any(x => x.Contains("RAID")));
        }

        [Test]
        public void Summary_PrioritizesCriticalEvidenceAndAggregatesCauses()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); MachineState m = Machine(s, c, inv); AdvancedDiagnosticWorkflowService svc = Service(s, c, inv);
            m.cables.atx24 = false; inv.Get(m.ramItemIds[0]).fault = FaultType.UnstableMemory;
            DiagnosticEvidence summary = svc.Summarize(new[] { svc.Run(m, DiagnosticProbeKind.CableContinuity), svc.Run(m, DiagnosticProbeKind.MemoryIntegrity), svc.Run(m, DiagnosticProbeKind.StorageHealth) });
            Assert.AreEqual(DiagnosticSeverity.Critical, summary.severity); Assert.Greater(summary.confidence, .6f); Assert.Greater(summary.likelyCauses.Count, 0);
        }
    }
}
