using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class SpecialistSimulationTests
    {
        private static HardwareCatalog Catalog()
        {
            HardwareCatalog c=new HardwareCatalog();
            c.Load();
            return c;
        }

        [Test]
        public void LiquidLoop_RequiresCompleteTightBledLoopBeforePass()
        {
            GameState state=new GameState();
            HardwareCatalog catalog=Catalog();
            InventoryService inv=new InventoryService(state,catalog);
            LiquidCoolingService service=new LiquidCoolingService(inv,state.workshop);
            MachineState m=new MachineState();

            Assert.IsTrue(service.InstallPump(m).ok);
            Assert.IsTrue(service.InstallReservoir(m).ok);
            Assert.IsTrue(service.InstallRadiator(m,240).ok);
            Assert.IsTrue(service.AddFittings(m,2).ok);
            Assert.IsTrue(service.AddFittings(m,2).ok);
            Assert.IsTrue(service.AddFittings(m,2).ok);
            Assert.IsFalse(service.LeakTest(m).ok,"Loose dry loop must not pass leak validation.");
            for(int i=0;i<6;i++)Assert.IsTrue(service.TightenNextFitting(m).ok);
            Assert.IsTrue(service.Fill(m,.25f).ok);
            Assert.IsTrue(service.Fill(m,.25f).ok);
            Assert.IsTrue(service.Fill(m,.25f).ok);
            Assert.IsTrue(service.Bleed(m).ok);
            Assert.IsTrue(service.LeakTest(m).ok);
            Assert.IsTrue(m.liquidLoop.leakTestPassed);
            Assert.GreaterOrEqual(m.liquidLoop.flowLpm,.8f);
        }

        [Test]
        public void BoardRepair_EnforcesEsdDiagnosisReworkVerificationOrder()
        {
            WorkshopState workshop=new WorkshopState{level=3,benchLevel=3,boardRepairLevel=1};
            BoardRepairService service=new BoardRepairService(workshop);
            MachineState m=new MachineState();
            m.history.Add("Customer symptom: probable short on core rail");

            Assert.IsFalse(service.MeasureRail(m).ok,"Powered probing before ESD/microscope must be blocked.");
            Assert.IsTrue(service.GroundEsd(m).ok);
            Assert.IsTrue(service.Inspect(m).ok);
            Assert.IsTrue(service.MeasureRail(m).ok);
            Assert.Less(m.boardRepair.measuredRailV,.5f);
            Assert.IsTrue(service.LocateShort(m).ok);
            Assert.IsTrue(service.ApplyFlux(m).ok);
            Assert.IsTrue(service.Rework(m).ok);
            Assert.IsTrue(service.Verify(m).ok);
            Assert.IsTrue(m.boardRepair.repaired);
            Assert.IsTrue(m.boardRepair.padsIntact);
        }

        [Test]
        public void PortableRepair_BlocksCoverUntilScrewsRemovedAndRestoresSeal()
        {
            GameState state=new GameState();
            HardwareCatalog catalog=Catalog();
            InventoryService inv=new InventoryService(state,catalog);
            PortableRepairService service=new PortableRepairService(inv,state.workshop);
            MachineState m=new MachineState{category=DeviceCategory.Laptop};
            m.portable.screwsRemaining=2;

            Assert.IsFalse(service.RemoveBackCover(m).ok);
            Assert.IsTrue(service.RemoveScrew(m).ok);
            Assert.IsTrue(service.RemoveScrew(m).ok);
            Assert.IsTrue(service.RemoveBackCover(m).ok);
            Assert.IsTrue(service.DisconnectBattery(m).ok);
            Assert.IsTrue(service.ServiceChargingPort(m).ok);
            Assert.IsTrue(service.Reseal(m).ok);
            Assert.IsTrue(m.portable.sealed);
            Assert.IsFalse(m.portable.batteryDisconnected);
        }

        [Test]
        public void NasRecovery_RequiresReplacementThenScrub()
        {
            GameState state=new GameState();
            HardwareCatalog catalog=Catalog();
            InventoryService inv=new InventoryService(state,catalog);
            HardwareDefinition storage=catalog.ByCategory(PartCategory.Storage)[0];
            inv.Create(storage.id);
            NetworkLabService service=new NetworkLabService(inv,new WorkshopState{diagnosticsLevel=2});
            MachineState m=new MachineState{category=DeviceCategory.NAS};
            m.network=new NetworkLabState{raidLevel=5,disksTotal=4,disksHealthy=3,arrayDegraded=true,packetLoss=.03f};

            Assert.IsTrue(service.ConnectLink(m).ok);
            Assert.IsTrue(service.ConfigureDhcp(m).ok);
            Assert.IsFalse(service.Scrub(m).ok);
            Assert.IsTrue(service.ReplaceFailedDisk(m).ok);
            Assert.IsTrue(service.Scrub(m).ok);
            Assert.IsTrue(service.ThroughputTest(m).ok);
            Assert.IsFalse(m.network.arrayDegraded);
            Assert.IsTrue(m.network.scrubComplete);
            Assert.Greater(m.network.throughputMbps,600f);
        }

        [Test]
        public void SpecialistContract_CannotCompleteBeforeDisciplineAcceptanceCriteria()
        {
            GameState state=new GameState();
            HardwareCatalog catalog=Catalog();
            InventoryService inv=new InventoryService(state,catalog);
            EconomyService economy=new EconomyService(state);
            SpecialistJobService jobs=new SpecialistJobService(state,catalog,inv,economy);

            Assert.IsTrue(jobs.Accept(SpecialistContractKind.BoardRepair).ok);
            JobState j=state.jobs.Find(x=>x.stage==JobStage.Accepted);
            MachineState m=state.machines.Find(x=>x.machineId==j.machineId);
            Assert.IsFalse(jobs.ValidateAndComplete(j,m).ok);
            m.boardRepair.repaired=true;m.boardRepair.padsIntact=true;
            Assert.IsTrue(jobs.ValidateAndComplete(j,m).ok);
            Assert.AreEqual(JobStage.Completed,j.stage);
            Assert.IsNull(state.machines.Find(x=>x.machineId==m.machineId));
        }
    }
}
