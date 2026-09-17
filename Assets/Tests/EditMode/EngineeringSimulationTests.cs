using System.Linq;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class EngineeringSimulationTests
    {
        private static HardwareCatalog Catalog(){HardwareCatalog c=new HardwareCatalog();c.Load();return c;}

        private static MachineState BuildMinimumPc(GameState state,HardwareCatalog catalog,InventoryService inv)
        {
            MachineState m=new MachineState{machineId="TEST-PC",sidePanelInstalled=false};
            HardwareDefinition pcCase=catalog.ByCategory(PartCategory.Case).First();
            HardwareDefinition board=catalog.ByCategory(PartCategory.Motherboard).First();
            HardwareDefinition cpu=catalog.ByCategory(PartCategory.CPU).FirstOrDefault(x=>string.IsNullOrEmpty(board.socket)||x.socket==board.socket)??catalog.ByCategory(PartCategory.CPU).First();
            HardwareDefinition ram=catalog.ByCategory(PartCategory.RAM).FirstOrDefault(x=>string.IsNullOrEmpty(board.memoryType)||x.memoryType==board.memoryType)??catalog.ByCategory(PartCategory.RAM).First();
            HardwareDefinition psu=catalog.ByCategory(PartCategory.PSU).OrderByDescending(x=>x.psuWattage).First();
            HardwareDefinition cooler=catalog.ByCategory(PartCategory.Cooler).First();
            HardwareDefinition storage=catalog.ByCategory(PartCategory.Storage).First();
            m.caseItemId=Install(inv,pcCase.id,m);m.motherboardItemId=Install(inv,board.id,m);m.cpuItemId=Install(inv,cpu.id,m);m.ramItemIds.Add(Install(inv,ram.id,m));m.ramSlotIndices.Add(1);m.psuItemId=Install(inv,psu.id,m);m.coolerItemId=Install(inv,cooler.id,m);m.storageItemIds.Add(Install(inv,storage.id,m));
            m.cables.atx24=true;m.cables.cpuEps=true;m.cables.frontPanel=true;m.cables.cpuFan=true;m.cables.gpuPower=true;m.cables.sataPower=true;m.cables.sataData=true;m.thermalPasteApplied=true;m.thermalPasteQuality=.9f;
            RamSlotRules.Normalize(m, board);
            ComponentMountRules.EnsureInstalled(m, id => inv.Def(inv.Get(id)), true);
            state.machines.Add(m);return m;
        }

        private static string Install(InventoryService inv,string id,MachineState m){ItemInstance i=inv.Create(id);i.reserved=true;i.note="Installed in "+m.machineId;return i.instanceId;}

        private static EngineeringSimulationService Service(GameState state,HardwareCatalog catalog,InventoryService inv)
        {
            PowerThermalService thermal=new PowerThermalService(catalog,inv);BootService boot=new BootService(inv,thermal);return new EngineeringSimulationService(inv,thermal,boot,state.workshop);
        }

        [Test]
        public void MemoryTraining_PersistsResultAndRecommendedSlotLayout()
        {
            GameState state=new GameState();HardwareCatalog catalog=Catalog();InventoryService inv=new InventoryService(state,catalog);MachineState m=BuildMinimumPc(state,catalog,inv);EngineeringSimulationService s=Service(state,catalog,inv);
            ActionResult result=s.TrainMemory(m);
            Assert.IsTrue(result.ok,result.message);Assert.IsTrue(m.bios.lastTrainingPassed);Assert.IsFalse(string.IsNullOrEmpty(m.bios.lastTrainingMessage));
        }

        [Test]
        public void PowerAnalysis_ReportsPersistentRailsAndHeadroom()
        {
            GameState state=new GameState();HardwareCatalog catalog=Catalog();InventoryService inv=new InventoryService(state,catalog);MachineState m=BuildMinimumPc(state,catalog,inv);EngineeringSimulationService s=Service(state,catalog,inv);
            ActionResult result=s.AnalyzePower(m);
            Assert.Greater(m.powerState.rail12V,0f);Assert.Greater(m.powerState.efficiency,0f);Assert.AreEqual(result.ok,m.powerState.stable);
        }

        [Test]
        public void SmartScan_FailsKnownStorageFault()
        {
            GameState state=new GameState();HardwareCatalog catalog=Catalog();InventoryService inv=new InventoryService(state,catalog);MachineState m=BuildMinimumPc(state,catalog,inv);EngineeringSimulationService s=Service(state,catalog,inv);
            ItemInstance disk=inv.Get(m.storageItemIds[0]);disk.fault=FaultType.BadStorage;disk.condition=.30f;
            ActionResult result=s.StorageSmart(m);
            Assert.IsFalse(result.ok);Assert.IsTrue(m.lastDiagnostic.Contains("FAIL"));
        }

        [Test]
        public void Benchmark_RequiresOsAndDriversThenStoresRun()
        {
            GameState state=new GameState();HardwareCatalog catalog=Catalog();InventoryService inv=new InventoryService(state,catalog);MachineState m=BuildMinimumPc(state,catalog,inv);EngineeringSimulationService s=Service(state,catalog,inv);
            Assert.IsFalse(s.BenchmarkSuite(m,15).ok);
            m.partitioned=true;m.osInstalled=true;m.activated=true;m.driversInstalled=true;m.osState.systemFilesHealthy=true;
            ActionResult result=s.BenchmarkSuite(m,15);
            Assert.Greater(m.benchmarkState.totalScore,0f);Assert.AreEqual(15,m.benchmarkState.stressMinutes);Assert.IsFalse(string.IsNullOrEmpty(m.benchmarkState.lastResult));
            Assert.AreEqual(result.ok,m.benchmarkState.status==BenchmarkStatus.Passed);
        }

        [Test]
        public void EngineeringPost_RejectsPhysicalDamageBeforeBoot()
        {
            GameState state=new GameState();HardwareCatalog catalog=Catalog();InventoryService inv=new InventoryService(state,catalog);MachineState m=BuildMinimumPc(state,catalog,inv);EngineeringSimulationService s=Service(state,catalog,inv);
            inv.Get(m.cpuItemId).damage=DamageType.BentPins;
            ActionResult result=s.EngineeringPost(m);
            Assert.IsFalse(result.ok);Assert.AreEqual("DMG",m.postCode);Assert.AreEqual(BootState.PostFailed,m.bootState);
        }
    }
}
