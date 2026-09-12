using UnityEngine;

namespace ForgeBench
{
    public static class EngineeringRuntimeExtensions
    {
        private static EngineeringSimulationService S(GameRuntime g)=>new EngineeringSimulationService(g.Inventory,g.PowerThermal,g.Boot,g.State.workshop);
        private static MachineState M(GameRuntime g)=>g==null?null:g.ActiveMachine;

        private static void Apply(GameRuntime g,ActionResult r)
        {
            if(g==null)return;if(r.ok&&g.State!=null&&g.Saves!=null)g.Saves.Save(g.State,1);g.UI?.Refresh();g.World?.RefreshMachine();g.Notify(r.message,r.ok);
        }

        public static void EngineeringPost(this GameRuntime g)=>Apply(g,S(g).EngineeringPost(M(g)));
        public static void EngineeringTrainMemory(this GameRuntime g)=>Apply(g,S(g).TrainMemory(M(g)));
        public static void EngineeringAirflow(this GameRuntime g)=>Apply(g,S(g).AnalyzeAirflow(M(g)));
        public static void EngineeringPower(this GameRuntime g)=>Apply(g,S(g).AnalyzePower(M(g)));
        public static void EngineeringThermal(this GameRuntime g,int minutes=10)=>Apply(g,S(g).ThermalSoak(M(g),minutes));
        public static void EngineeringSmart(this GameRuntime g)=>Apply(g,S(g).StorageSmart(M(g)));
        public static void EngineeringOsScan(this GameRuntime g)=>Apply(g,S(g).OsIntegrityScan(M(g)));
        public static void EngineeringOsRepair(this GameRuntime g)=>Apply(g,S(g).RepairOs(M(g)));
        public static void EngineeringUpdateSoftware(this GameRuntime g)=>Apply(g,S(g).UpdateSoftware(M(g)));
        public static void EngineeringBenchmark(this GameRuntime g,int minutes=15)=>Apply(g,S(g).BenchmarkSuite(M(g),minutes));

        public static void EngineeringAdjustMemoryVoltage(this GameRuntime g,float delta)
        {
            MachineState m=M(g);if(m==null){g.Notify("No active machine.",false);return;}m.bios.memoryVoltage=Mathf.Clamp(m.bios.memoryVoltage+delta,1.05f,1.50f);m.bios.lastTrainingPassed=false;Apply(g,ActionResult.Success("DRAM voltage set to "+m.bios.memoryVoltage.ToString("0.00")+" V. Retrain memory."));
        }
        public static void EngineeringAdjustCpuPower(this GameRuntime g,int delta)
        {
            MachineState m=M(g);if(m==null){g.Notify("No active machine.",false);return;}m.bios.cpuPowerLimitWatts=Mathf.Clamp(m.bios.cpuPowerLimitWatts+delta,35,350);Apply(g,ActionResult.Success("CPU power limit set to "+m.bios.cpuPowerLimitWatts+" W."));
        }
        public static void EngineeringAdjustMultiplier(this GameRuntime g,int delta)
        {
            MachineState m=M(g);if(m==null){g.Notify("No active machine.",false);return;}m.bios.cpuMultiplierOffset=Mathf.Clamp(m.bios.cpuMultiplierOffset+delta,-5,12);Apply(g,ActionResult.Success("CPU multiplier offset "+(m.bios.cpuMultiplierOffset>=0?"+":"")+m.bios.cpuMultiplierOffset+"."));
        }
        public static void EngineeringToggleRebar(this GameRuntime g)
        {
            MachineState m=M(g);if(m==null){g.Notify("No active machine.",false);return;}m.bios.resizableBar=!m.bios.resizableBar;m.bios.above4G=m.bios.resizableBar||m.bios.above4G;Apply(g,ActionResult.Success("Resizable BAR "+(m.bios.resizableBar?"enabled":"disabled")+"."));
        }
        public static void EngineeringToggleSecureBoot(this GameRuntime g)
        {
            MachineState m=M(g);if(m==null){g.Notify("No active machine.",false);return;}m.bios.secureBoot=!m.bios.secureBoot;if(m.bios.secureBoot)m.bios.csm=false;Apply(g,ActionResult.Success("Secure Boot "+(m.bios.secureBoot?"enabled":"disabled")+"."));
        }
        public static void EngineeringResetBios(this GameRuntime g)
        {
            MachineState m=M(g);if(m==null){g.Notify("No active machine.",false);return;}m.bios=new BiosState();m.postCode="OFF";m.bootState=BootState.Off;Apply(g,ActionResult.Success("Firmware settings reset to validated defaults."));
        }
    }
}
