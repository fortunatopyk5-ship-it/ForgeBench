using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Deterministic reliability layer. It never rolls UnityEngine.Random, so saving and
    /// reloading the same game day cannot create a different failure. Wear is derived
    /// from heat, dust, power quality, condition, usage and maintenance state.
    /// </summary>
    public sealed class ReliabilitySimulationService
    {
        private readonly GameState state;private readonly HardwareCatalog catalog;private readonly InventoryService inventory;
        public ReliabilitySimulationService(GameState s,HardwareCatalog c,InventoryService i){state=s;catalog=c;inventory=i;}

        public List<string> AdvanceMachineOneDay(MachineState m)
        {
            List<string> events=new List<string>();if(m==null)return events;if(m.maintenance==null)m.maintenance=new MaintenanceState();
            float workshopDirt=1f-Mathf.Clamp01(state.workshop.cleanliness);float dustGain=.006f+workshopDirt*.018f;m.dust=Mathf.Clamp01(m.dust+dustGain);m.maintenance.filterDust=Mathf.Clamp01(m.maintenance.filterDust+dustGain*1.15f);m.maintenance.thermalPasteAgeDays++;m.maintenance.serviceAgeDays++;
            IEnumerable<string> ids=Installed(m).Where(x=>!string.IsNullOrEmpty(x));foreach(string id in ids){ItemInstance item=inventory.Get(id);HardwareDefinition d=inventory.Def(item);if(item==null||d==null)continue;float heat=Mathf.Clamp01((item.lastTempC-45f)/55f);float dust=Mathf.Max(item.dust,m.dust);float powerStress=m.powerState==null?0f:Mathf.Clamp01((m.powerState.rippleMv-45f)/110f)+(m.powerState.stable?0f:.12f);float daily=.00020f+d.heatWatts/1000000f+heat*.00045f+dust*.00018f+powerStress*.00030f;item.wear=Mathf.Clamp01(item.wear+daily);item.condition=Mathf.Clamp01(item.condition-daily*.55f);item.dust=Mathf.Clamp01(item.dust+dustGain*.55f);if(d.category==PartCategory.Storage){item.readGB+=Mathf.Max(.2f,d.storageGB*.0015f);item.writtenGB+=Mathf.Max(.08f,d.storageGB*.00065f);}int key=StableKey(item.instanceId,state.day,m.machineId);EvaluateFault(item,d,m,key,events);}
            if(m.portable!=null&&m.category!=DeviceCategory.Desktop){float cycle=.00055f+.00015f*Mathf.Clamp01(m.portable.waterDamage);m.portable.batteryHealth=Mathf.Max(.05f,m.portable.batteryHealth-cycle);}
            if(m.liquidLoop!=null&&m.liquidLoop.coolantLitres>0f&&m.liquidLoop.coolantAgeDays>365){m.liquidLoop.flowLpm=Mathf.Max(0f,m.liquidLoop.flowLpm-.004f);}
            if(events.Count>0){foreach(string e in events)m.history.Add("Reliability: "+e);Trim(m.history,80);}return events;
        }

        private void EvaluateFault(ItemInstance item,HardwareDefinition d,MachineState m,int key,List<string> events)
        {
            if(item.fault!=FaultType.None)return;float vulnerability=(1f-item.condition)*.65f+item.wear*.35f+item.dust*.15f;int threshold=Mathf.Clamp(Mathf.RoundToInt(vulnerability*18f),0,16);if(key%1000>=threshold)return;
            FaultType fault=FaultType.None;
            switch(d.category){case PartCategory.RAM:fault=FaultType.UnstableMemory;break;case PartCategory.Storage:fault=FaultType.BadStorage;break;case PartCategory.Fan:fault=FaultType.FanFailure;break;case PartCategory.Battery:fault=FaultType.BatteryWear;break;case PartCategory.Display:fault=FaultType.DisplayFault;break;case PartCategory.GPU:case PartCategory.CPU:case PartCategory.Cooler:fault=FaultType.Overheating;break;default:fault=(key/7)%3==0?FaultType.ConnectorDamage:FaultType.Firmware;break;}item.fault=fault;events.Add(d.model+" developed "+fault);if(fault==FaultType.FanFailure||fault==FaultType.Overheating)m.stressStable=false;
        }

        public ActionResult PreventiveMaintenance(MachineState m)
        {
            if(m==null)return ActionResult.Fail("No device available for preventive maintenance.");m.dust=Mathf.Max(0f,m.dust-.65f);if(m.maintenance==null)m.maintenance=new MaintenanceState();m.maintenance.filterDust=Mathf.Max(0f,m.maintenance.filterDust-.8f);m.maintenance.serviceAgeDays=0;m.maintenance.cleaningCycles++;foreach(string id in Installed(m)){ItemInstance it=inventory.Get(id);if(it==null)continue;it.dust=Mathf.Max(0f,it.dust-.75f);if(it.fault==FaultType.Dust)it.fault=FaultType.None;}m.history.Add("Preventive maintenance completed");return ActionResult.Success("Filters, vents, contacts and service points inspected and cleaned.");
        }

        public string ReliabilityReport(MachineState m)
        {
            if(m==null)return "No device.";List<string> lines=new List<string>();foreach(string id in Installed(m)){ItemInstance it=inventory.Get(id);HardwareDefinition d=inventory.Def(it);if(it==null||d==null)continue;lines.Add(d.model+": condition "+Mathf.RoundToInt(it.condition*100)+"%, wear "+Mathf.RoundToInt(it.wear*100)+"%, dust "+Mathf.RoundToInt(it.dust*100)+"%, fault "+it.fault);}return string.Join("\n",lines);
        }

        private static IEnumerable<string> Installed(MachineState m){yield return m.caseItemId;yield return m.motherboardItemId;yield return m.cpuItemId;yield return m.gpuItemId;yield return m.psuItemId;yield return m.coolerItemId;foreach(string x in m.ramItemIds)yield return x;foreach(string x in m.storageItemIds)yield return x;foreach(string x in m.fanItemIds)yield return x;}
        private static int StableKey(string a,int day,string b){unchecked{int h=17;string s=(a??"")+"|"+day+"|"+(b??"");for(int i=0;i<s.Length;i++)h=h*31+s[i];return h==int.MinValue?0:Mathf.Abs(h);}}
        private static void Trim(List<string> l,int max){if(l!=null&&l.Count>max)l.RemoveRange(0,l.Count-max);}
    }
}
