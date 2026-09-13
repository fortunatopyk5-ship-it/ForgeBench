using System.Linq;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class JobDispatchTests
    {
        private static HardwareCatalog Catalog() { HardwareCatalog c = new HardwareCatalog(); c.Load(); return c; }
        private static JobDispatchService Service(GameState s, HardwareCatalog c, InventoryService inv)
        {
            EconomyService economy = new EconomyService(s);
            return new JobDispatchService(s, new JobService(s, c, inv), new SpecialistJobService(s, c, inv, economy));
        }

        [Test]
        public void DefaultWorkshop_AllowsTwoStandardContracts()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); JobService jobs = new JobService(s, c, inv); jobs.EnsureOffers(4); JobDispatchService dispatch = new JobDispatchService(s, jobs, new SpecialistJobService(s, c, inv, new EconomyService(s)));
            JobState a = s.jobs.First(x => x.stage == JobStage.Offered); Assert.IsTrue(dispatch.AcceptStandard(a).ok);
            JobState b = s.jobs.First(x => x.stage == JobStage.Offered); Assert.IsTrue(dispatch.AcceptStandard(b).ok);
            Assert.AreEqual(2, dispatch.ActiveJobs.Count); Assert.AreEqual(2, dispatch.UsedUnits); Assert.AreEqual(0, dispatch.FreeUnits);
        }

        [Test]
        public void CapacityBlocksThirdStandardContractUntilWorkshopExpands()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); JobService jobs = new JobService(s, c, inv); jobs.EnsureOffers(4); JobDispatchService dispatch = new JobDispatchService(s, jobs, new SpecialistJobService(s, c, inv, new EconomyService(s)));
            Assert.IsTrue(dispatch.AcceptStandard(s.jobs.First(x => x.stage == JobStage.Offered)).ok); Assert.IsTrue(dispatch.AcceptStandard(s.jobs.First(x => x.stage == JobStage.Offered)).ok);
            ActionResult blocked = dispatch.AcceptStandard(s.jobs.First(x => x.stage == JobStage.Offered)); Assert.IsFalse(blocked.ok);
            s.workshop.benchLevel = 2; ActionResult expanded = dispatch.AcceptStandard(s.jobs.First(x => x.stage == JobStage.Offered)); Assert.IsTrue(expanded.ok); Assert.AreEqual(3, dispatch.ActiveJobs.Count);
        }

        [Test]
        public void FocusReordersActiveQueueWithoutChangingStages()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); JobService jobs = new JobService(s, c, inv); jobs.EnsureOffers(4); JobDispatchService dispatch = new JobDispatchService(s, jobs, new SpecialistJobService(s, c, inv, new EconomyService(s)));
            JobState first = s.jobs.First(x => x.stage == JobStage.Offered); dispatch.AcceptStandard(first); JobState second = s.jobs.First(x => x.stage == JobStage.Offered); dispatch.AcceptStandard(second);
            JobStage before = first.stage; Assert.IsTrue(dispatch.Focus(first).ok); Assert.AreSame(first, s.jobs.First(x => JobDispatchService.IsActive(x))); Assert.AreEqual(before, first.stage); Assert.IsTrue(JobDispatchService.IsActive(second));
        }

        [Test]
        public void TriagePrefersReadyThenOverdueWork()
        {
            GameState s = new GameState { day = 10 }; HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); JobDispatchService dispatch = Service(s, c, inv);
            JobState overdue = new JobState { jobId = "A", title = "Overdue", stage = JobStage.InProgress, dueDay = 7, reward = 500 };
            JobState ready = new JobState { jobId = "B", title = "Ready", stage = JobStage.ReadyToSubmit, dueDay = 12, reward = 300 };
            s.jobs.Add(overdue); s.jobs.Add(ready);
            Assert.AreSame(ready, dispatch.RecommendedJob()); ready.stage = JobStage.InProgress; Assert.AreSame(overdue, dispatch.RecommendedJob()); Assert.AreEqual(DispatchRisk.Overdue, dispatch.Risk(overdue));
        }

        [Test]
        public void ComplexSpecialistConsumesTwoDispatchUnits()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); JobDispatchService dispatch = Service(s, c, inv);
            Assert.AreEqual(2, JobDispatchService.SpecialistWorkload(SpecialistContractKind.BoardRepair));
            ActionResult accepted = dispatch.AcceptSpecialist(SpecialistContractKind.BoardRepair); Assert.IsTrue(accepted.ok, accepted.message); Assert.AreEqual(2, dispatch.UsedUnits); Assert.IsTrue(SpecialistJobService.IsSpecialist(dispatch.ActiveJobs.Single()));
        }

        [Test]
        public void ActiveStagesAreRestoredAfterLegacyGuardBypass()
        {
            GameState s = new GameState(); HardwareCatalog c = Catalog(); InventoryService inv = new InventoryService(s, c); JobService jobs = new JobService(s, c, inv); jobs.EnsureOffers(4); JobDispatchService dispatch = new JobDispatchService(s, jobs, new SpecialistJobService(s, c, inv, new EconomyService(s)));
            JobState first = s.jobs.First(x => x.stage == JobStage.Offered); Assert.IsTrue(dispatch.AcceptStandard(first).ok); first.stage = JobStage.InProgress;
            JobState second = s.jobs.First(x => x.stage == JobStage.Offered); Assert.IsTrue(dispatch.AcceptStandard(second).ok); Assert.AreEqual(JobStage.InProgress, first.stage); Assert.IsTrue(JobDispatchService.IsActive(second));
        }
    }
}
