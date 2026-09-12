using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public enum SpecialistContractKind { LiquidBuild, BoardRepair, LaptopBattery, PhoneDisplay, ConsoleController, NasRecovery, ServerNetwork }

    /// <summary>
    /// Specialist contracts share the same JobState/MachineState persistence model as
    /// normal jobs but use discipline-specific acceptance and validation instead of
    /// pretending every device is a desktop PC.
    /// </summary>
    public sealed class SpecialistJobService
    {
        private readonly GameState state;
        private readonly HardwareCatalog catalog;
        private readonly InventoryService inventory;
        private readonly EconomyService economy;

        public SpecialistJobService(GameState s, HardwareCatalog c, InventoryService i, EconomyService e)
        {
            state=s; catalog=c; inventory=i; economy=e;
        }

        public ActionResult Accept(SpecialistContractKind kind)
        {
            if (state.jobs.Any(j=>j.stage==JobStage.Accepted||j.stage==JobStage.InProgress||j.stage==JobStage.ReadyToSubmit))
                return ActionResult.Fail("Finish the active contract before accepting another specialist job.");

            int serial=state.nextJobSerial++;
            int tier=Mathf.Clamp(1+state.reputation/140,1,6);
            JobState j=CreateContract(kind,serial,tier);
            MachineState m=CreateMachine(j,kind,tier);
            state.jobs.Add(j); state.machines.Add(m); j.machineId=m.machineId; j.stage=JobStage.Accepted;
            return ActionResult.Success(j.title+" accepted. Device moved to the specialist workstation.");
        }

        private JobState CreateContract(SpecialistContractKind kind,int serial,int tier)
        {
            JobState j=new JobState
            {
                jobId="S"+serial.ToString("D5"),
                customerName=Customer(serial),
                stage=JobStage.Offered,
                dueDay=state.day+3+tier,
                budget=300+tier*180,
                reward=320+tier*170,
                targetBenchmark=0,
                maxNoiseDb=55,
                requireOs=true,
                requireDrivers=false,
                requireClean=false,
                requireStable=false,
                requireNoFaults=false
            };
            j.requiredPartCategories.Add("SPECIALIST:"+kind);
            switch(kind)
            {
                case SpecialistContractKind.LiquidBuild:
                    j.type=JobType.CustomBuild;j.deviceCategory=DeviceCategory.Desktop;j.title="Custom liquid-loop workstation";
                    j.description="Assemble, fill, bleed and isolated-leak-test a custom loop. Final approval requires stable flow and no leak.";
                    j.reward+=260;j.optionalObjectives.Add("Leak test must pass before powered hardware testing");break;
                case SpecialistContractKind.BoardRepair:
                    j.type=JobType.BoardRepair;j.deviceCategory=DeviceCategory.Laptop;j.title="Board-level power short repair";
                    j.description="Use ESD control, microscope inspection, rail measurement and controlled hot-air rework. Verify the repaired rail.";
                    j.reward+=380;j.optionalObjectives.Add("Avoid pad damage and excessive rework cycles");break;
                case SpecialistContractKind.LaptopBattery:
                    j.type=JobType.Repair;j.deviceCategory=DeviceCategory.Laptop;j.title="Laptop battery and charging service";
                    j.description="Disassemble safely, isolate battery power, replace the worn pack, service the charge path and reseal.";break;
                case SpecialistContractKind.PhoneDisplay:
                    j.type=JobType.Repair;j.deviceCategory=DeviceCategory.Phone;j.title="Phone display and seal restoration";
                    j.description="Open without live battery power, separate and replace the display, then restore the enclosure seal.";
                    j.reward+=180;break;
                case SpecialistContractKind.ConsoleController:
                    j.type=JobType.Diagnostics;j.deviceCategory=DeviceCategory.Controller;j.title="Controller drift diagnosis and calibration";
                    j.description="Open the controller, isolate power, service the analog assembly and reduce measured drift before reassembly.";break;
                case SpecialistContractKind.NasRecovery:
                    j.type=JobType.Network;j.deviceCategory=DeviceCategory.NAS;j.title="Degraded NAS array recovery";
                    j.description="Restore network link, replace the failed array disk, rebuild/scrub RAID and validate throughput.";
                    j.reward+=300;break;
                default:
                    j.type=JobType.Network;j.deviceCategory=DeviceCategory.Server;j.title="Server network commissioning";
                    j.description="Configure link and addressing, validate packet loss/latency and complete storage-array health checks.";
                    j.reward+=220;break;
            }
            return j;
        }

        private MachineState CreateMachine(JobState j,SpecialistContractKind kind,int tier)
        {
            MachineState m=new MachineState
            {
                machineId="M-"+j.jobId,
                displayName=j.customerName+" · "+j.deviceCategory,
                ownerJobId=j.jobId,
                category=j.deviceCategory,
                dust=.10f,
                sidePanelInstalled=false,
                bootState=BootState.Off,
                postCode="SPECIAL"
            };
            switch(kind)
            {
                case SpecialistContractKind.LiquidBuild:
                    SeedLiquidDesktop(j,m);break;
                case SpecialistContractKind.BoardRepair:
                    m.boardRepair=new BoardRepairState();m.history.Add("Customer symptom: no power; abnormal low core rail suspected short");m.lastDiagnostic="Board power-stage short suspected";break;
                case SpecialistContractKind.LaptopBattery:
                    m.portable=new PortableDeviceState{batteryHealth=.38f,chargingPortHealth=.58f,displayHealth=.96f,screwsRemaining=8,sealed=true,sealQuality=.96f};m.history.Add("Customer symptom: battery runtime below one hour and intermittent charging");break;
                case SpecialistContractKind.PhoneDisplay:
                    m.portable=new PortableDeviceState{batteryHealth=.86f,chargingPortHealth=.92f,displayHealth=.18f,screwsRemaining=2,sealed=true,sealQuality=.92f};m.history.Add("Customer symptom: cracked/no-touch display; preserve battery and board");break;
                case SpecialistContractKind.ConsoleController:
                    m.portable=new PortableDeviceState{batteryHealth=.78f,chargingPortHealth=.88f,displayHealth=1f,screwsRemaining=6,sealed=true,sealQuality=.95f,controllerDrift=.32f};m.history.Add("Customer symptom: severe right-stick drift");break;
                case SpecialistContractKind.NasRecovery:
                    m.network=new NetworkLabState{linkUp=false,dhcp=true,ipAddress="0.0.0.0",throughputMbps=0,packetLoss=.045f,latencyMs=0,raidLevel=5,disksTotal=4,disksHealthy=3,arrayDegraded=true,scrubComplete=false};m.history.Add("Customer symptom: RAID degraded after one disk failure; network performance inconsistent");break;
                case SpecialistContractKind.ServerNetwork:
                    m.network=new NetworkLabState{linkUp=false,dhcp=false,ipAddress="0.0.0.0",throughputMbps=0,packetLoss=.08f,latencyMs=0,raidLevel=1,disksTotal=2,disksHealthy=2,arrayDegraded=false,scrubComplete=false};m.history.Add("Commissioning request: static addressing and validated network/storage health");break;
            }
            return m;
        }

        private void SeedLiquidDesktop(JobState j,MachineState m)
        {
            string caseId=First(PartCategory.Case,p=>p.radiatorSupportMm>=360);
            if(string.IsNullOrEmpty(caseId))caseId=First(PartCategory.Case,p=>true);
            string boardId=First(PartCategory.Motherboard,p=>true);
            string cpuId=First(PartCategory.CPU,p=>true);
            if(!string.IsNullOrEmpty(caseId))m.caseItemId=CustomerPart(j,m,caseId);
            if(!string.IsNullOrEmpty(boardId))m.motherboardItemId=CustomerPart(j,m,boardId);
            if(!string.IsNullOrEmpty(cpuId))m.cpuItemId=CustomerPart(j,m,cpuId);
            m.sidePanelInstalled=false;m.liquidLoop=new LiquidLoopState();m.history.Add("Customer chassis prepared for custom liquid cooling");
        }

        private string First(PartCategory c,Func<HardwareDefinition,bool> filter)
        {
            HardwareDefinition d=catalog.ByCategory(c).FirstOrDefault(filter);return d==null?null:d.id;
        }

        private string CustomerPart(JobState j,MachineState m,string definitionId)
        {
            ItemInstance item=inventory.Create(definitionId,true);item.reserved=true;item.ownerJobId=j.jobId;item.note="Customer part in "+m.machineId;return item.instanceId;
        }

        public ActionResult ValidateAndComplete(JobState j,MachineState m)
        {
            if(j==null||m==null)return ActionResult.Fail("No specialist contract is active.");
            string marker=j.requiredPartCategories==null?null:j.requiredPartCategories.FirstOrDefault(x=>x.StartsWith("SPECIALIST:",StringComparison.Ordinal));
            if(string.IsNullOrEmpty(marker))return ActionResult.Fail("The active contract is not a specialist workflow.");
            SpecialistContractKind kind;
            if(!Enum.TryParse(marker.Substring("SPECIALIST:".Length),out kind))return ActionResult.Fail("Unknown specialist contract type.");
            List<string> miss=new List<string>();
            switch(kind)
            {
                case SpecialistContractKind.LiquidBuild:
                    if(m.liquidLoop==null||!m.liquidLoop.leakTestPassed)miss.Add("liquid-loop leak test not passed");
                    if((m.liquidLoop?.flowLpm??0f)<.8f)miss.Add("coolant flow below 0.8 L/min");
                    if((m.liquidLoop?.airFraction??1f)>.18f)miss.Add("loop still contains excessive air");
                    break;
                case SpecialistContractKind.BoardRepair:
                    if(m.boardRepair==null||!m.boardRepair.repaired)miss.Add("board repair not electrically verified");
                    if(m.boardRepair!=null&&!m.boardRepair.padsIntact)miss.Add("pad damage remains");
                    break;
                case SpecialistContractKind.LaptopBattery:
                    if(m.portable==null||m.portable.batteryHealth<.90f)miss.Add("battery health below 90%");
                    if(m.portable==null||m.portable.chargingPortHealth<.75f)miss.Add("charging path not restored");
                    if(m.portable==null||!m.portable.sealed)miss.Add("device not reassembled");
                    break;
                case SpecialistContractKind.PhoneDisplay:
                    if(m.portable==null||m.portable.displayHealth<.95f)miss.Add("display replacement incomplete");
                    if(m.portable==null||!m.portable.sealed||m.portable.sealQuality<.78f)miss.Add("enclosure seal below acceptance threshold");
                    break;
                case SpecialistContractKind.ConsoleController:
                    if(m.portable==null||m.portable.controllerDrift>.06f)miss.Add("controller drift remains above 0.06");
                    if(m.portable==null||!m.portable.sealed)miss.Add("controller not reassembled");
                    break;
                case SpecialistContractKind.NasRecovery:
                    if(m.network==null||!m.network.linkUp)miss.Add("network link down");
                    if(m.network==null||m.network.arrayDegraded||!m.network.scrubComplete)miss.Add("RAID rebuild/scrub incomplete");
                    if(m.network==null||m.network.throughputMbps<650f)miss.Add("throughput below 650 Mbps");
                    break;
                case SpecialistContractKind.ServerNetwork:
                    if(m.network==null||!m.network.linkUp)miss.Add("network link down");
                    if(m.network==null||m.network.dhcp)miss.Add("static IPv4 configuration required");
                    if(m.network==null||m.network.throughputMbps<700f||m.network.packetLoss>.02f)miss.Add("network validation outside tolerance");
                    break;
            }
            if(miss.Count>0)return ActionResult.Fail("Cannot submit specialist job: "+string.Join("; ",miss)+".");

            float bonus=state.day<=j.dueDay?j.reward*.10f:0f;
            j.stage=JobStage.Completed;j.completionMessage="Specialist workflow completed on day "+state.day;
            economy.Earn(j.reward+bonus,"Completed specialist "+j.jobId);
            state.reputation+=25+Mathf.RoundToInt(bonus/15f);state.experience+=150;
            state.machines.Remove(m);
            foreach(ItemInstance item in state.inventory.Where(x=>x.customerOwned&&x.ownerJobId==j.jobId).ToList())state.inventory.Remove(item);
            state.milestones.Add("Specialist: "+kind+" · "+j.jobId);
            return ActionResult.Success("Specialist contract delivered. Earned $"+(j.reward+bonus).ToString("0")+".");
        }

        public static bool IsSpecialist(JobState j)
        {
            return j!=null&&j.requiredPartCategories!=null&&j.requiredPartCategories.Any(x=>x.StartsWith("SPECIALIST:",StringComparison.Ordinal));
        }

        private static string Customer(int serial)
        {
            string[] names={"Mara Chen","Ihor Lysenko","Noah Vale","Lina Hart","Roman Kirov","Eva Moss","Kai Stern","Svitlana Ray"};
            return names[Mathf.Abs(serial)%names.Length];
        }
    }
}
