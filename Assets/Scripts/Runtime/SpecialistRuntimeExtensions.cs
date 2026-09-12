using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Runtime action surface for specialist simulations. Every successful action is
    /// persisted immediately, marks a specialist contract in progress and refreshes
    /// the visible world/workstation state.
    /// </summary>
    public static class SpecialistRuntimeExtensions
    {
        private static MachineState M(GameRuntime g) => g == null ? null : g.ActiveMachine;
        private static LiquidCoolingService Liquid(GameRuntime g) => new LiquidCoolingService(g.Inventory, g.State.workshop);
        private static BoardRepairService Board(GameRuntime g) => new BoardRepairService(g.State.workshop);
        private static PortableRepairService Portable(GameRuntime g) => new PortableRepairService(g.Inventory, g.State.workshop);
        private static NetworkLabService Network(GameRuntime g) => new NetworkLabService(g.Inventory, g.State.workshop);
        private static SpecialistJobService Jobs(GameRuntime g) => new SpecialistJobService(g.State, g.Catalog, g.Inventory, g.Economy);

        private static void Apply(GameRuntime g, ActionResult result)
        {
            if (g == null) return;
            if (result.ok)
            {
                JobState j=g.ActiveJob;
                if(j!=null&&SpecialistJobService.IsSpecialist(j)&&j.stage==JobStage.Accepted)j.stage=JobStage.InProgress;
                if(g.State!=null&&g.Saves!=null)g.Saves.Save(g.State,1);
            }
            g.UI?.Refresh();
            g.World?.RefreshMachine();
            g.Notify(result.message, result.ok);
        }

        public static void AcceptSpecialistContract(this GameRuntime g, SpecialistContractKind kind) => Apply(g, Jobs(g).Accept(kind));
        public static void SubmitSpecialistContract(this GameRuntime g) => Apply(g, Jobs(g).ValidateAndComplete(g.ActiveJob, g.ActiveMachine));

        public static void OrderSpecialistPart(this GameRuntime g, PartCategory category)
        {
            if(g==null)return;
            HardwareDefinition part=g.Catalog.ByCategory(category).OrderBy(p=>p.price).ThenByDescending(p=>p.quality).FirstOrDefault();
            if(part==null){g.Notify("No "+category+" service part exists in the catalog.",false);return;}
            g.Buy(part.id,1);
        }

        public static void LiquidInstallPump(this GameRuntime g) => Apply(g, Liquid(g).InstallPump(M(g)));
        public static void LiquidInstallReservoir(this GameRuntime g) => Apply(g, Liquid(g).InstallReservoir(M(g)));
        public static void LiquidInstallRadiator(this GameRuntime g, int mm) => Apply(g, Liquid(g).InstallRadiator(M(g), mm));
        public static void LiquidAddFittings(this GameRuntime g) => Apply(g, Liquid(g).AddFittings(M(g), 2));
        public static void LiquidTightenFitting(this GameRuntime g) => Apply(g, Liquid(g).TightenNextFitting(M(g)));
        public static void LiquidFill(this GameRuntime g) => Apply(g, Liquid(g).Fill(M(g), .25f));
        public static void LiquidBleed(this GameRuntime g) => Apply(g, Liquid(g).Bleed(M(g)));
        public static void LiquidLeakTest(this GameRuntime g) => Apply(g, Liquid(g).LeakTest(M(g)));

        public static void BoardGroundEsd(this GameRuntime g) => Apply(g, Board(g).GroundEsd(M(g)));
        public static void BoardInspect(this GameRuntime g) => Apply(g, Board(g).Inspect(M(g)));
        public static void BoardMeasure(this GameRuntime g) => Apply(g, Board(g).MeasureRail(M(g)));
        public static void BoardLocateShort(this GameRuntime g) => Apply(g, Board(g).LocateShort(M(g)));
        public static void BoardApplyFlux(this GameRuntime g) => Apply(g, Board(g).ApplyFlux(M(g)));
        public static void BoardRework(this GameRuntime g) => Apply(g, Board(g).Rework(M(g)));
        public static void BoardVerify(this GameRuntime g) => Apply(g, Board(g).Verify(M(g)));

        public static void PortableRemoveScrew(this GameRuntime g) => Apply(g, Portable(g).RemoveScrew(M(g)));
        public static void PortableRemoveCover(this GameRuntime g) => Apply(g, Portable(g).RemoveBackCover(M(g)));
        public static void PortableDisconnectBattery(this GameRuntime g) => Apply(g, Portable(g).DisconnectBattery(M(g)));
        public static void PortableSeparateDisplay(this GameRuntime g) => Apply(g, Portable(g).SeparateDisplay(M(g)));
        public static void PortableReplaceBattery(this GameRuntime g) => Apply(g, Portable(g).ReplaceBattery(M(g)));
        public static void PortableReplaceDisplay(this GameRuntime g) => Apply(g, Portable(g).ReplaceDisplay(M(g)));
        public static void PortableServicePort(this GameRuntime g) => Apply(g, Portable(g).ServiceChargingPort(M(g)));
        public static void PortableCalibrate(this GameRuntime g) => Apply(g, Portable(g).CalibrateController(M(g)));
        public static void PortableReseal(this GameRuntime g) => Apply(g, Portable(g).Reseal(M(g)));

        public static void NetworkConnect(this GameRuntime g) => Apply(g, Network(g).ConnectLink(M(g)));
        public static void NetworkDhcp(this GameRuntime g) => Apply(g, Network(g).ConfigureDhcp(M(g)));
        public static void NetworkStatic(this GameRuntime g) => Apply(g, Network(g).ConfigureStatic(M(g)));
        public static void NetworkTest(this GameRuntime g) => Apply(g, Network(g).ThroughputTest(M(g)));
        public static void NetworkRaid(this GameRuntime g, int level) => Apply(g, Network(g).ConfigureRaid(M(g), level));
        public static void NetworkReplaceDisk(this GameRuntime g) => Apply(g, Network(g).ReplaceFailedDisk(M(g)));
        public static void NetworkScrub(this GameRuntime g) => Apply(g, Network(g).Scrub(M(g)));
    }
}
