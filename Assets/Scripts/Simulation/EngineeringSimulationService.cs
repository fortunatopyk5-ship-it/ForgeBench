using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Cross-system engineering simulation used by POST, firmware tuning, airflow,
    /// power validation, storage health, OS integrity and repeatable benchmark/stress
    /// tests. Results are written to persistent MachineState sub-models.
    /// </summary>
    public sealed class EngineeringSimulationService
    {
        private readonly InventoryService inventory;
        private readonly PowerThermalService baseThermal;
        private readonly BootService boot;
        private readonly WorkshopState workshop;

        public EngineeringSimulationService(InventoryService i, PowerThermalService p, BootService b, WorkshopState w)
        {
            inventory=i;baseThermal=p;boot=b;workshop=w;
        }

        private HardwareDefinition D(string id)=>inventory.Def(inventory.Get(id));
        private static void Ensure(MachineState m)
        {
            if(m.osState==null)m.osState=new OsRuntimeState();if(m.osState.history==null)m.osState.history=new List<string>();
            if(m.benchmarkState==null)m.benchmarkState=new BenchmarkRunState();if(m.benchmarkState.history==null)m.benchmarkState.history=new List<string>();
            if(m.thermalState==null)m.thermalState=new ThermalRuntimeState();if(m.thermalState.history==null)m.thermalState.history=new List<string>();
            if(m.powerState==null)m.powerState=new PowerRuntimeState();if(m.powerState.history==null)m.powerState.history=new List<string>();
            if(m.maintenance==null)m.maintenance=new MaintenanceState();if(m.maintenance.history==null)m.maintenance.history=new List<string>();
            if(m.history==null)m.history=new List<string>();
        }

        public ActionResult TrainMemory(MachineState m)
        {
            if(m==null)return ActionResult.Fail("No machine is available for memory training.");Ensure(m);
            HardwareDefinition board=D(m.motherboardItemId);if(board==null)return FailTraining(m,"Motherboard not detected.");
            if(m.ramItemIds.Count==0)return FailTraining(m,"No DIMMs installed.");
            List<HardwareDefinition> modules=m.ramItemIds.Select(D).Where(x=>x!=null).ToList();
            if(modules.Count!=m.ramItemIds.Count)return FailTraining(m,"One or more DIMMs have invalid catalog data.");
            if(modules.Any(x=>x.memoryType!=board.memoryType))return FailTraining(m,"Mixed/incompatible memory generation.");
            int slotLimit=RamSlotRules.SlotCount(board);if(m.ramItemIds.Count>slotLimit)return FailTraining(m,"Installed DIMM count exceeds board slot count.");
            if(!RamSlotRules.IsSecured(m,board))return FailTraining(m,"Close both retention latches on every installed DIMM.");
            bool slotLayout=RamSlotRules.IsRecommendedPair(m,board);
            int native=modules.Min(x=>Mathf.Max(1,x.speed));int target=m.bios.memoryProfileEnabled?Mathf.Max(native,m.bios.memorySpeedOverride):native;
            float quality=modules.Average(x=>(float)x.quality)/100f;float condition=m.ramItemIds.Select(inventory.Get).Where(x=>x!=null).Average(x=>x.condition);
            bool unstableFault=m.ramItemIds.Select(inventory.Get).Any(x=>x!=null&&(x.fault==FaultType.UnstableMemory||x.damage==DamageType.BurnedConnector));
            float voltage=m.bios.memoryVoltage;float required=target>6000?1.35f:target>4800?1.25f:1.20f;
            bool pass=slotLayout&&!unstableFault&&condition>.40f&&quality>.35f&&voltage+0.02f>=required;
            m.bios.lastTrainingPassed=pass;
            m.bios.memorySpeedOverride=target;
            m.bios.lastTrainingMessage=pass?"Training passed at "+target+" MT/s" : (!slotLayout?"Training failed: use recommended dual-channel slots":unstableFault?"Training failed: DIMM electrical fault":voltage<required?"Training failed: insufficient DRAM voltage":"Training failed: module condition/quality");
            m.history.Add(m.bios.lastTrainingMessage);
            return pass?ActionResult.Success(m.bios.lastTrainingMessage):ActionResult.Fail(m.bios.lastTrainingMessage);
        }

        private static ActionResult FailTraining(MachineState m,string text){m.bios.lastTrainingPassed=false;m.bios.lastTrainingMessage=text;m.history.Add("Memory training: "+text);return ActionResult.Fail(text);}

        public ActionResult AnalyzeAirflow(MachineState m)
        {
            if(m==null)return ActionResult.Fail("No machine is available for airflow analysis.");Ensure(m);
            float total=0f;int valid=0;
            foreach(string id in m.fanItemIds){HardwareDefinition f=D(id);if(f==null)continue;total+=f.airflowCfm>0?f.airflowCfm:Mathf.Max(18f,f.performance*.28f);valid++;}
            HardwareDefinition cooler=D(m.coolerItemId);if(cooler!=null&&cooler.tags.Contains("air")){total+=Mathf.Max(22f,cooler.airflowCfm);valid++;}
            int intake=(valid+1)/2,exhaust=valid/2;float dust=Mathf.Clamp01(m.dust);float effective=total*(1f-dust*.52f);
            float balance=valid==0?0f:(intake-exhaust)/(float)valid;
            m.thermalState.intakeFans=intake;m.thermalState.exhaustFans=exhaust;m.thermalState.airflowCfm=effective;m.thermalState.pressureBalance=balance;
            m.thermalState.history.Add("Airflow "+effective.ToString("0")+" CFM; intake "+intake+" / exhaust "+exhaust+"; pressure "+balance.ToString("+0.00;-0.00;0.00"));
            if(valid==0)return ActionResult.Fail("No active case/cooler airflow detected.");
            if(effective<35f)return ActionResult.Fail("Airflow is restricted: "+effective.ToString("0")+" CFM effective.");
            return ActionResult.Success("Airflow "+effective.ToString("0")+" CFM · pressure balance "+balance.ToString("+0.00;-0.00;0.00")+".");
        }

        public ActionResult AnalyzePower(MachineState m)
        {
            if(m==null)return ActionResult.Fail("No machine is available for power analysis.");Ensure(m);
            HardwareDefinition psu=D(m.psuItemId);if(psu==null){m.powerState.stable=false;return ActionResult.Fail("No PSU installed.");}
            ItemInstance psuItem=inventory.Get(m.psuItemId);float capacity=Mathf.Max(1,psu.psuWattage);float steady=baseThermal.EstimatePower(m);float transient=steady*(D(m.gpuItemId)!=null?1.28f:1.12f);float load=steady/capacity;float condition=psuItem==null?1f:Mathf.Clamp01(psuItem.condition);
            float efficiency=psu.efficiencyClass>=5?.92f:psu.efficiencyClass>=3?.88f:.83f;efficiency-=Mathf.Max(0f,load-.75f)*.08f;
            float droop=Mathf.Max(0f,load-.55f)*.42f+(1f-condition)*.24f;float rail12=12f-droop;float ripple=24f+load*42f+(1f-condition)*70f;
            bool ocp=transient>capacity*1.08f;bool stable=!ocp&&rail12>=11.55f&&ripple<=120f;
            m.powerState.rail12V=rail12;m.powerState.rail5V=5f-Mathf.Max(0f,load-.8f)*.10f;m.powerState.rail33V=3.3f-Mathf.Max(0f,load-.8f)*.06f;m.powerState.rippleMv=ripple;m.powerState.efficiency=Mathf.Clamp01(efficiency);m.powerState.headroomW=capacity-steady;m.powerState.transientPeakW=transient;m.powerState.ocpTriggered=ocp;m.powerState.stable=stable;
            m.powerState.history.Add("12V "+rail12.ToString("0.00")+" V · ripple "+ripple.ToString("0")+" mV · steady "+steady.ToString("0")+" W · transient "+transient.ToString("0")+" W");
            return stable?ActionResult.Success("Power rails stable. 12V "+rail12.ToString("0.00")+" V · ripple "+ripple.ToString("0")+" mV · headroom "+m.powerState.headroomW.ToString("0")+" W."):ActionResult.Fail(ocp?"PSU transient load exceeds safe capacity / OCP threshold.":"Power quality outside tolerance: 12V "+rail12.ToString("0.00")+" V, ripple "+ripple.ToString("0")+" mV.");
        }

        public ActionResult ThermalSoak(MachineState m,int minutes=10)
        {
            if(m==null)return ActionResult.Fail("No machine is available for thermal testing.");Ensure(m);minutes=Mathf.Clamp(minutes,1,60);
            AnalyzeAirflow(m);AnalyzePower(m);baseThermal.Simulate(m,true);
            float airflow=Mathf.Max(15f,m.thermalState.airflowCfm);float ambient=m.thermalState.ambientC;float dust=Mathf.Clamp01(m.dust);
            m.thermalState.caseAirC=ambient+Mathf.Clamp(m.systemPowerW/airflow*.42f,2f,24f)+dust*6f;
            m.thermalState.vrmC=Mathf.Clamp(m.thermalState.caseAirC+18f+m.systemPowerW*.055f,30f,125f);
            float storageLoad=m.storageItemIds.Count==0?0f:m.storageItemIds.Select(D).Where(x=>x!=null).Average(x=>x.storageInterface=="NVMe"?12f:7f);
            m.thermalState.storageC=Mathf.Clamp(m.thermalState.caseAirC+storageLoad,25f,95f);
            if(m.liquidLoop!=null&&m.liquidLoop.coolantLitres>.2f)m.thermalState.coolantC=Mathf.Clamp(ambient+8f+m.systemPowerW/Mathf.Max(.4f,m.liquidLoop.flowLpm)*.022f,25f,70f);
            m.thermalState.cpuThrottling=m.cpuTempC>=95f;m.thermalState.gpuThrottling=m.gpuTempC>=91f;
            m.benchmarkState.peakCpuC=Mathf.Max(m.benchmarkState.peakCpuC,m.cpuTempC);m.benchmarkState.peakGpuC=Mathf.Max(m.benchmarkState.peakGpuC,m.gpuTempC);m.benchmarkState.peakPowerW=Mathf.Max(m.benchmarkState.peakPowerW,m.systemPowerW);m.benchmarkState.stressMinutes=Mathf.Max(m.benchmarkState.stressMinutes,minutes);
            m.thermalState.history.Add(minutes+" min soak: CPU "+m.cpuTempC.ToString("0")+"C GPU "+m.gpuTempC.ToString("0")+"C VRM "+m.thermalState.vrmC.ToString("0")+"C case "+m.thermalState.caseAirC.ToString("0")+"C");
            bool pass=!m.thermalState.cpuThrottling&&!m.thermalState.gpuThrottling&&m.thermalState.vrmC<105f&&m.powerState.stable;
            return pass?ActionResult.Success(minutes+" min thermal soak passed. CPU "+m.cpuTempC.ToString("0")+"°C · GPU "+m.gpuTempC.ToString("0")+"°C · VRM "+m.thermalState.vrmC.ToString("0")+"°C."):ActionResult.Fail("Thermal/power soak failed: "+(m.thermalState.cpuThrottling?"CPU throttling; ":"")+(m.thermalState.gpuThrottling?"GPU throttling; ":"")+(m.thermalState.vrmC>=105f?"VRM overheating; ":"")+(!m.powerState.stable?"power unstable":""));
        }

        public ActionResult StorageSmart(MachineState m)
        {
            if(m==null)return ActionResult.Fail("No machine is available for storage diagnostics.");Ensure(m);if(m.storageItemIds.Count==0)return ActionResult.Fail("No storage devices installed.");
            List<string> rows=new List<string>();bool pass=true;
            foreach(string id in m.storageItemIds)
            {
                ItemInstance item=inventory.Get(id);HardwareDefinition d=inventory.Def(item);if(item==null||d==null)continue;
                float endurance=d.enduranceTBW>0?d.enduranceTBW*1024f:Mathf.Max(100000f,d.storageGB*350f);float used=endurance<=0?0:item.writtenGB/endurance;float health=Mathf.Clamp01(Mathf.Min(item.condition,1f-used*.55f-item.wear*.25f));item.condition=Mathf.Min(item.condition,health);item.lastTempC=m.thermalState.storageC;
                bool bad=item.fault==FaultType.BadStorage||item.damage==DamageType.BurnedConnector||health<.35f;pass&=!bad;rows.Add(d.model+" health "+Mathf.RoundToInt(health*100)+"% · "+item.writtenGB.ToString("0")+" GB written · "+item.lastTempC.ToString("0")+"°C"+(bad?" FAIL":" OK"));
            }
            string report=string.Join(" | ",rows);m.lastDiagnostic=report;m.history.Add("Storage SMART: "+report);return pass?ActionResult.Success(report):ActionResult.Fail(report);
        }

        public ActionResult EngineeringPost(MachineState m)
        {
            if(m==null)return ActionResult.Fail("No machine is available for POST.");Ensure(m);
            ActionResult training=TrainMemory(m);if(!training.ok){m.postCode="DRAM";m.bootState=BootState.PostFailed;return training;}
            ActionResult power=AnalyzePower(m);if(!power.ok){m.postCode=m.powerState.ocpTriggered?"OCP":"PWRQ";m.bootState=BootState.PostFailed;return power;}
            IEnumerable<string> installed=new[]{m.motherboardItemId,m.cpuItemId,m.gpuItemId,m.psuItemId,m.coolerItemId}.Concat(m.ramItemIds).Concat(m.storageItemIds).Concat(m.fanItemIds);
            ItemInstance damaged=installed.Select(inventory.Get).FirstOrDefault(i=>i!=null&&i.damage!=DamageType.None);
            if(damaged!=null){m.postCode="DMG";m.bootState=BootState.PostFailed;return ActionResult.Fail("POST blocked by physical damage: "+damaged.damage+" on "+(inventory.Def(damaged)?.model??damaged.definitionId)+".");}
            ActionResult result=boot.PowerOn(m);if(result.ok){foreach(string id in installed){ItemInstance i=inventory.Get(id);if(i!=null)i.powerCycles++;}m.history.Add("Engineering POST passed after memory/power validation");}
            return result;
        }

        public ActionResult OsIntegrityScan(MachineState m)
        {
            if(m==null||!m.osInstalled)return ActionResult.Fail("Install the operating system first.");Ensure(m);
            bool storageBad=m.storageItemIds.Select(inventory.Get).Any(i=>i!=null&&(i.fault==FaultType.BadStorage||i.condition<.35f));
            m.osState.partitionCount=Mathf.Max(1,m.partitioned?2:1);m.osState.freeStorageGB=Mathf.Max(0,m.storageItemIds.Select(D).Where(x=>x!=null).Sum(x=>x.storageGB)-42);m.osState.systemFilesHealthy=!storageBad;m.osState.networkStackReady=m.network==null||m.network.linkUp||m.category==DeviceCategory.Desktop;
            string text="ForgeFS "+(m.osState.systemFilesHealthy?"healthy":"corruption detected")+" · free "+m.osState.freeStorageGB+" GB · drivers r"+m.osState.driverRevision+" · updates "+m.osState.pendingUpdates;
            m.osState.history.Add("Integrity scan: "+text);return m.osState.systemFilesHealthy?ActionResult.Success(text):ActionResult.Fail(text);
        }

        public ActionResult RepairOs(MachineState m)
        {
            if(m==null||!m.osInstalled)return ActionResult.Fail("No installed OS to repair.");Ensure(m);ActionResult smart=StorageSmart(m);if(!smart.ok)return ActionResult.Fail("Replace/repair failing storage before repairing system files.");
            m.osState.systemFilesHealthy=true;m.osState.lastCrashCode=string.Empty;m.osState.pendingUpdates=Mathf.Max(m.osState.pendingUpdates,1);m.osState.history.Add("System image verified and damaged files restored from local recovery source");return ActionResult.Success("System files restored. Apply pending updates and drivers.");
        }

        public ActionResult UpdateSoftware(MachineState m)
        {
            if(m==null||!m.osInstalled)return ActionResult.Fail("Install the OS first.");Ensure(m);if(!m.osState.systemFilesHealthy)return ActionResult.Fail("Repair OS integrity before updating.");
            m.osState.driverRevision++;m.osState.pendingUpdates=0;m.driversInstalled=true;m.osState.networkStackReady=true;m.osState.history.Add("ForgeOS updates and hardware driver revision "+m.osState.driverRevision+" installed");return ActionResult.Success("System updated. Driver revision "+m.osState.driverRevision+" installed.");
        }

        public ActionResult BenchmarkSuite(MachineState m,int stressMinutes=15)
        {
            if(m==null)return ActionResult.Fail("No machine is available for benchmarking.");Ensure(m);if(!m.osInstalled)return ActionResult.Fail("OS required for benchmark suite.");if(!m.driversInstalled)return ActionResult.Fail("Install current drivers before benchmarking.");
            ActionResult train=TrainMemory(m);ActionResult power=AnalyzePower(m);ActionResult therm=ThermalSoak(m,stressMinutes);
            HardwareDefinition cpu=D(m.cpuItemId),gpu=D(m.gpuItemId);List<HardwareDefinition> ram=m.ramItemIds.Select(D).Where(x=>x!=null).ToList();List<HardwareDefinition> disks=m.storageItemIds.Select(D).Where(x=>x!=null).ToList();
            float cpuScore=(cpu?.performance??0)*(1f+m.bios.cpuMultiplierOffset*.015f);float gpuScore=(gpu?.performance??((cpu?.performance??0)*.28f))*(m.bios.resizableBar?1.025f:1f);float memScore=ram.Sum(x=>x.performance)*(m.bios.memoryProfileEnabled?1.07f:1f);float storageScore=disks.Count==0?0:disks.Max(x=>Mathf.Max(x.performance,(x.readMBs+x.writeMBs)/12f));
            int errors=m.ramItemIds.Select(inventory.Get).Count(x=>x!=null&&(x.fault==FaultType.UnstableMemory||x.condition<.45f));if(!train.ok)errors+=8;if(m.bios.memoryVoltage<1.15f&&m.bios.memoryProfileEnabled)errors+=4;
            float total=cpuScore*.40f+gpuScore*.38f+memScore*.11f+storageScore*.11f;if(m.thermalState.cpuThrottling)total*=.84f;if(m.thermalState.gpuThrottling)total*=.88f;if(!power.ok)total*=.80f;
            BenchmarkStatus status=train.ok&&power.ok&&therm.ok&&errors==0?BenchmarkStatus.Passed:(errors<=2&&power.ok?BenchmarkStatus.Warning:BenchmarkStatus.Failed);
            m.benchmarkState.cpuScore=Mathf.Round(cpuScore);m.benchmarkState.gpuScore=Mathf.Round(gpuScore);m.benchmarkState.memoryScore=Mathf.Round(memScore);m.benchmarkState.storageScore=Mathf.Round(storageScore);m.benchmarkState.totalScore=Mathf.Round(total);m.benchmarkState.memoryErrors=errors;m.benchmarkState.stressMinutes=stressMinutes;m.benchmarkState.minimum12V=m.powerState.rail12V;m.benchmarkState.maxRippleMv=m.powerState.rippleMv;m.benchmarkState.status=status;
            m.benchmarkState.lastResult=status+" · total "+m.benchmarkState.totalScore+" · CPU "+m.benchmarkState.cpuScore+" GPU "+m.benchmarkState.gpuScore+" MEM "+m.benchmarkState.memoryScore+" SSD "+m.benchmarkState.storageScore+" · errors "+errors;
            m.benchmarkState.history.Add(m.benchmarkState.lastResult);m.benchmarkScore=m.benchmarkState.totalScore;m.stressStable=status==BenchmarkStatus.Passed;m.history.Add("Engineering benchmark: "+m.benchmarkState.lastResult);
            return status==BenchmarkStatus.Passed?ActionResult.Success(m.benchmarkState.lastResult):ActionResult.Fail(m.benchmarkState.lastResult);
        }

        public void AgeOneDay(MachineState m)
        {
            if(m==null)return;Ensure(m);m.maintenance.serviceAgeDays++;m.maintenance.thermalPasteAgeDays+=m.thermalPasteApplied?1:0;m.maintenance.filterDust=Mathf.Clamp01(m.maintenance.filterDust+.008f);m.dust=Mathf.Clamp01(m.dust+.006f);
            IEnumerable<string> ids=new[]{m.motherboardItemId,m.cpuItemId,m.gpuItemId,m.psuItemId,m.coolerItemId}.Concat(m.ramItemIds).Concat(m.storageItemIds).Concat(m.fanItemIds);
            foreach(string id in ids){ItemInstance item=inventory.Get(id);if(item==null)continue;item.wear=Mathf.Clamp01(item.wear+.00018f+Mathf.Max(0f,item.lastTempC-70f)*.000004f);item.condition=Mathf.Clamp01(Mathf.Min(item.condition,1f-item.wear*.35f));}
        }
    }
}
