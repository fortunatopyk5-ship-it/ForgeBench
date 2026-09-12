using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public enum InspectionSeverity { Info, Warning, Blocking }

    public sealed class InspectionFinding
    {
        public InspectionSeverity severity;
        public string system;
        public string message;
        public InspectionFinding(InspectionSeverity s,string area,string text){severity=s;system=area;message=text;}
    }

    public sealed class PreflightReport
    {
        public bool pass;
        public int blocking;
        public int warnings;
        public float qualityScore;
        public List<InspectionFinding> findings=new List<InspectionFinding>();
        public string Summary=>pass?"READY FOR CUSTOMER ACCEPTANCE":"NOT READY · "+blocking+" blocking issue(s)";
    }

    /// <summary>
    /// Read-only acceptance inspection spanning mechanical, electrical, compatibility,
    /// thermals, software, reliability and specialist contract state. It never silently
    /// repairs a device: every blocking reason must be resolved through its real system.
    /// </summary>
    public sealed class PreflightInspectionService
    {
        private readonly GameRuntime game;
        public PreflightInspectionService(GameRuntime g){game=g;}

        public PreflightReport Inspect(JobState job,MachineState m)
        {
            PreflightReport r=new PreflightReport();
            if(job==null){Block(r,"Job","No active customer contract.");return Finish(r,m);}
            if(m==null){Block(r,"Device","Customer device is missing from the workshop.");return Finish(r,m);}

            bool specialist=SpecialistJobService.IsSpecialist(job);
            if(!specialist)InspectDesktop(job,m,r);else InspectSpecialist(job,m,r);
            InspectReliability(m,r);
            InspectHistory(m,r);
            return Finish(r,m);
        }

        private void InspectDesktop(JobState j,MachineState m,PreflightReport r)
        {
            if(m.category!=DeviceCategory.Desktop&&j.type!=JobType.Network)Warn(r,"Device","Non-desktop device is using the standard desktop acceptance path.");
            foreach(string issue in game.Compatibility.ExplainSystem(m))Block(r,"Compatibility",issue+".");

            if(string.IsNullOrEmpty(m.caseItemId))Block(r,"Mechanical","Case/chassis missing.");
            if(string.IsNullOrEmpty(m.motherboardItemId))Block(r,"Assembly","Motherboard missing.");
            if(string.IsNullOrEmpty(m.cpuItemId))Block(r,"Assembly","CPU missing.");
            if(m.ramItemIds==null||m.ramItemIds.Count==0)Block(r,"Assembly","No system memory installed.");
            if(string.IsNullOrEmpty(m.psuItemId))Block(r,"Power","Power supply missing.");
            if(string.IsNullOrEmpty(m.coolerItemId))Block(r,"Cooling","CPU cooling solution missing.");
            if(m.storageItemIds==null||m.storageItemIds.Count==0)Warn(r,"Storage","No storage device installed.");

            if(!string.IsNullOrEmpty(m.caseItemId))
            {
                game.Assembly.EnsureCaseHardware(m);
                if(!m.sidePanelInstalled)Block(r,"Mechanical","Side panel is not installed.");
                float torque=game.Assembly.TighteningQuality(m);if(torque<.95f)Block(r,"Mechanical","Side-panel fastener tightening quality is "+Mathf.RoundToInt(torque*100)+"%.");
                if(m.sidePanel!=null&&m.sidePanel.clipDamaged)Warn(r,"Mechanical","Side-panel retaining clip is damaged.");
                if(m.sidePanel?.fasteners!=null&&m.sidePanel.fasteners.Any(x=>x.damaged))Warn(r,"Mechanical","One or more enclosure fasteners are damaged.");
            }

            if(!m.thermalPasteApplied)Block(r,"Cooling","Thermal interface material is missing.");
            else if(m.thermalPasteQuality<.55f)Warn(r,"Cooling","Thermal paste coverage quality is low ("+Mathf.RoundToInt(m.thermalPasteQuality*100)+"%).");

            CableState c=m.cables??new CableState();
            if(!c.atx24)Block(r,"Cabling","24-pin ATX power not connected.");
            if(!c.cpuEps)Block(r,"Cabling","CPU EPS power not connected.");
            if(!c.frontPanel)Block(r,"Cabling","Front-panel power header not connected.");
            if(!c.cpuFan)Block(r,"Cabling","CPU_FAN signal missing.");
            HardwareDefinition gpu=game.Inventory.Def(game.Inventory.Get(m.gpuItemId));
            if(gpu!=null&&gpu.powerWatts>=180f&&!c.gpuPower)Block(r,"Cabling","GPU auxiliary power is not connected.");
            bool sata=(m.storageItemIds??new List<string>()).Any(id=>game.Inventory.Def(game.Inventory.Get(id))?.storageInterface=="SATA");
            if(sata&&!c.sataPower)Block(r,"Cabling","SATA power is missing.");
            if(sata&&!c.sataData)Block(r,"Cabling","SATA data path is missing.");
            if(m.cableManagementScore<.45f)Warn(r,"Cabling","Cable routing quality is poor and may obstruct airflow.");

            if(m.postCode!="A0")Block(r,"POST","Successful POST code A0 has not been reached (current "+m.postCode+").");
            if(m.bios!=null&&!m.bios.lastTrainingPassed)Block(r,"BIOS","Memory training failed: "+m.bios.lastTrainingMessage+".");
            if(m.powerState!=null)
            {
                if(!m.powerState.stable)Block(r,"Power","Power rail simulation reports instability.");
                if(m.powerState.ocpTriggered)Block(r,"Power","PSU over-current protection was triggered.");
                if(m.powerState.rail12V<11.4f||m.powerState.rail12V>12.6f)Block(r,"Power","12V rail outside acceptance range: "+m.powerState.rail12V.ToString("0.00")+" V.");
                if(m.powerState.rippleMv>120f)Block(r,"Power","Power ripple exceeds 120 mV: "+m.powerState.rippleMv.ToString("0")+" mV.");
                else if(m.powerState.rippleMv>80f)Warn(r,"Power","Power ripple is elevated: "+m.powerState.rippleMv.ToString("0")+" mV.");
            }

            if(m.thermalState!=null)
            {
                if(m.thermalState.cpuThrottling)Block(r,"Thermal","CPU thermal throttling detected.");
                if(m.thermalState.gpuThrottling)Block(r,"Thermal","GPU thermal throttling detected.");
                if(m.thermalState.vrmC>100f)Block(r,"Thermal","VRM temperature is "+m.thermalState.vrmC.ToString("0")+"°C.");
                if(m.thermalState.storageC>78f)Warn(r,"Thermal","Storage temperature is high: "+m.thermalState.storageC.ToString("0")+"°C.");
            }
            if(m.cpuTempC>96f)Block(r,"Thermal","CPU peak temperature exceeds 96°C.");else if(m.cpuTempC>88f)Warn(r,"Thermal","CPU temperature is close to the thermal limit.");
            if(m.gpuTempC>94f)Block(r,"Thermal","GPU peak temperature exceeds 94°C.");else if(m.gpuTempC>86f)Warn(r,"Thermal","GPU temperature is elevated.");

            if(j.requireOs&&!m.osInstalled)Block(r,"Software","Operating system is required but not installed.");
            if(j.requireDrivers&&!m.driversInstalled)Block(r,"Software","Required drivers are not installed.");
            if(m.osState!=null)
            {
                if(m.osInstalled&&!m.osState.systemFilesHealthy)Block(r,"Software","Operating-system integrity check failed.");
                if(m.osState.crashCount>=3)Warn(r,"Software","OS has recorded "+m.osState.crashCount+" crashes.");
                if(m.osState.pendingUpdates>5)Warn(r,"Software","Several system updates remain pending.");
            }

            if(j.targetBenchmark>0&&m.benchmarkScore<j.targetBenchmark)Block(r,"Performance","Benchmark "+m.benchmarkScore.ToString("0")+" / required "+j.targetBenchmark+".");
            if(j.requireStable&&!m.stressStable)Block(r,"Stability","Stress validation has not passed.");
            if(m.benchmarkRun!=null&&m.benchmarkRun.status==BenchmarkStatus.Failed)Block(r,"Stability","Latest engineering benchmark status is FAILED.");
            if(m.benchmarkRun!=null&&m.benchmarkRun.memoryErrors>0)Block(r,"Memory","Stress test recorded "+m.benchmarkRun.memoryErrors+" memory error(s).");
            if(j.requireClean&&m.dust>.12f)Block(r,"Cleanliness","Dust level "+Mathf.RoundToInt(m.dust*100)+"% exceeds job limit.");
            else if(m.dust>.45f)Warn(r,"Cleanliness","Device has heavy dust accumulation.");
            if(j.hiddenPreferenceLowNoise&&m.maxNoiseDb>0f&&m.noiseDb>j.maxNoiseDb)Warn(r,"Customer preference","Noise "+m.noiseDb.ToString("0")+" dB exceeds optional target "+j.maxNoiseDb.ToString("0")+" dB.");
        }

        private void InspectSpecialist(JobState j,MachineState m,PreflightReport r)
        {
            string marker=j.requiredPartCategories?.FirstOrDefault(x=>x.StartsWith("SPECIALIST:",StringComparison.Ordinal));
            SpecialistContractKind kind;if(string.IsNullOrEmpty(marker)||!Enum.TryParse(marker.Substring("SPECIALIST:".Length),out kind)){Block(r,"Specialist","Specialist workflow marker is invalid.");return;}
            switch(kind)
            {
                case SpecialistContractKind.LiquidBuild:
                    LiquidLoopState l=m.liquidLoop;if(l==null){Block(r,"Liquid loop","Loop state missing.");break;}if(!l.pumpInstalled)Block(r,"Liquid loop","Pump missing.");if(!l.reservoirInstalled)Block(r,"Liquid loop","Reservoir missing.");if(l.radiatorMm<=0)Block(r,"Liquid loop","Radiator missing.");if(l.fittingCount<=0||l.tightFittings<l.fittingCount)Block(r,"Liquid loop","Not all fittings are tightened.");if(l.coolantLitres<.35f)Block(r,"Liquid loop","Insufficient coolant volume.");if(l.airFraction>.18f)Block(r,"Liquid loop","Excessive air remains in loop.");if(l.leakDetected)Block(r,"Liquid loop","Leak detected.");if(!l.leakTestPassed)Block(r,"Liquid loop","Isolated leak test has not passed.");if(l.flowLpm<.8f)Block(r,"Liquid loop","Flow below 0.8 L/min.");break;
                case SpecialistContractKind.BoardRepair:
                    BoardRepairState b=m.boardRepair;if(b==null){Block(r,"Board repair","Board-repair state missing.");break;}if(!b.esdGrounded)Block(r,"Board repair","ESD protection not verified.");if(!b.microscopeInspected)Block(r,"Board repair","Microscope inspection incomplete.");if(!b.powerRailMeasured)Block(r,"Board repair","Power rail not measured.");if(!b.shortLocated)Block(r,"Board repair","Short circuit not localized.");if(!b.padsIntact)Block(r,"Board repair","PCB pad damage remains.");if(!b.repaired)Block(r,"Board repair","Electrical repair not verified.");if(b.solderQuality<.70f)Warn(r,"Board repair","Solder joint quality below 70%.");break;
                case SpecialistContractKind.LaptopBattery:
                    PortableDeviceState lp=m.portable;if(lp==null){Block(r,"Laptop","Portable state missing.");break;}if(lp.batteryHealth<.90f)Block(r,"Laptop","Replacement battery health below 90%.");if(lp.chargingPortHealth<.75f)Block(r,"Laptop","Charging path not fully restored.");if(!lp.sealed)Block(r,"Laptop","Device not reassembled.");if(lp.sealQuality<.70f)Warn(r,"Laptop","Enclosure seal quality is low.");break;
                case SpecialistContractKind.PhoneDisplay:
                    PortableDeviceState ph=m.portable;if(ph==null){Block(r,"Phone","Portable state missing.");break;}if(ph.displayHealth<.95f)Block(r,"Phone","Display acceptance below 95%.");if(!ph.sealed)Block(r,"Phone","Phone not resealed.");if(ph.sealQuality<.78f)Block(r,"Phone","Seal quality below water-resistance acceptance threshold.");if(ph.waterDamage>.20f)Warn(r,"Phone","Water-damage indicator remains elevated.");break;
                case SpecialistContractKind.ConsoleController:
                    PortableDeviceState co=m.portable;if(co==null){Block(r,"Controller","Controller state missing.");break;}if(co.controllerDrift>.06f)Block(r,"Controller","Analog drift remains "+co.controllerDrift.ToString("0.00")+".");if(!co.sealed)Block(r,"Controller","Controller not reassembled.");break;
                case SpecialistContractKind.NasRecovery:
                case SpecialistContractKind.ServerNetwork:
                    NetworkLabState n=m.network;if(n==null){Block(r,"Network","Network state missing.");break;}if(!n.linkUp)Block(r,"Network","Physical/data link is down.");if(n.packetLoss>.02f)Block(r,"Network","Packet loss "+(n.packetLoss*100f).ToString("0.0")+"% exceeds 2%.");if(n.latencyMs>50)Warn(r,"Network","Latency is elevated at "+n.latencyMs+" ms.");if(n.throughputMbps<(kind==SpecialistContractKind.NasRecovery?650f:700f))Block(r,"Network","Throughput below contract target.");if(kind==SpecialistContractKind.NasRecovery&&(n.arrayDegraded||!n.scrubComplete))Block(r,"Storage array","RAID recovery/scrub incomplete.");if(kind==SpecialistContractKind.ServerNetwork&&n.dhcp)Block(r,"Network","Server commissioning requires static addressing.");break;
            }
        }

        private void InspectReliability(MachineState m,PreflightReport r)
        {
            foreach(string id in Installed(m))
            {
                ItemInstance item=game.Inventory.Get(id);HardwareDefinition d=game.Inventory.Def(item);if(item==null||d==null)continue;
                if(item.fault!=FaultType.None)Block(r,"Reliability",d.model+" still has fault "+item.fault+".");
                if(item.damage!=DamageType.None)Block(r,"Damage",d.model+" has unresolved "+item.damage+".");
                if(item.condition<.35f)Block(r,"Reliability",d.model+" condition is critically low ("+Mathf.RoundToInt(item.condition*100)+"%).");
                else if(item.condition<.60f)Warn(r,"Reliability",d.model+" condition is "+Mathf.RoundToInt(item.condition*100)+"%.");
                if(item.wear>.82f)Warn(r,"Reliability",d.model+" wear is "+Mathf.RoundToInt(item.wear*100)+"%.");
            }
            if(m.maintenance!=null)
            {
                if(m.maintenance.corrosion>.25f)Block(r,"Maintenance","Corrosion level exceeds acceptance threshold.");
                if(m.maintenance.esdIncident)Warn(r,"Maintenance","An ESD incident is recorded in device history.");
                if(m.maintenance.filterDust>.70f)Warn(r,"Maintenance","Dust filter loading is high.");
                if(m.maintenance.thermalPasteAgeDays>730)Warn(r,"Maintenance","Thermal compound age exceeds two years.");
            }
        }

        private static void InspectHistory(MachineState m,PreflightReport r)
        {
            if(m.history==null||m.history.Count==0)Info(r,"Traceability","No service history entries recorded for this device.");
            else Info(r,"Traceability",m.history.Count+" device/service history event(s) available.");
        }

        private static PreflightReport Finish(PreflightReport r,MachineState m)
        {
            r.blocking=r.findings.Count(x=>x.severity==InspectionSeverity.Blocking);r.warnings=r.findings.Count(x=>x.severity==InspectionSeverity.Warning);r.pass=r.blocking==0;float baseScore=100f-r.blocking*18f-r.warnings*4f;if(m!=null){baseScore-=Mathf.Clamp01(m.dust)*8f;baseScore+=Mathf.Clamp01(m.cableManagementScore-.5f)*8f;baseScore+=Mathf.Clamp01(m.aestheticScore-.5f)*4f;}r.qualityScore=Mathf.Clamp(baseScore,0f,100f);return r;
        }
        private static IEnumerable<string> Installed(MachineState m){if(m==null)yield break;yield return m.caseItemId;yield return m.motherboardItemId;yield return m.cpuItemId;yield return m.gpuItemId;yield return m.psuItemId;yield return m.coolerItemId;if(m.ramItemIds!=null)foreach(string x in m.ramItemIds)yield return x;if(m.storageItemIds!=null)foreach(string x in m.storageItemIds)yield return x;if(m.fanItemIds!=null)foreach(string x in m.fanItemIds)yield return x;}
        private static void Block(PreflightReport r,string s,string m)=>r.findings.Add(new InspectionFinding(InspectionSeverity.Blocking,s,m));private static void Warn(PreflightReport r,string s,string m)=>r.findings.Add(new InspectionFinding(InspectionSeverity.Warning,s,m));private static void Info(PreflightReport r,string s,string m)=>r.findings.Add(new InspectionFinding(InspectionSeverity.Info,s,m));
    }
}
