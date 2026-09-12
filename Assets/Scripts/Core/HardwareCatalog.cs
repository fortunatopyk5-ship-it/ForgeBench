using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public sealed class HardwareCatalog
    {
        private readonly Dictionary<string, HardwareDefinition> byId = new Dictionary<string, HardwareDefinition>(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyCollection<HardwareDefinition> All => byId.Values;

        public void Load()
        {
            byId.Clear();
            TextAsset asset = Resources.Load<TextAsset>("Data/hardware");
            if (asset == null) throw new InvalidOperationException("Missing Resources/Data/hardware.json");
            HardwareCatalogData data = JsonUtility.FromJson<HardwareCatalogData>(asset.text);
            if (data == null || data.parts == null || data.parts.Count == 0) throw new InvalidOperationException("Hardware catalog is empty or invalid.");
            foreach (HardwareDefinition part in data.parts)
            {
                if (part == null || string.IsNullOrWhiteSpace(part.id)) continue;
                NormalizeExtendedSpecs(part);
                byId[part.id] = part;
            }
        }

        private static void NormalizeExtendedSpecs(HardwareDefinition p)
        {
            if (p.connectors == null) p.connectors = new List<string>();
            if (p.tags == null) p.tags = new List<string>();
            int q=Mathf.Clamp(p.quality,1,100);
            switch(p.category)
            {
                case PartCategory.Case:
                    if(p.driveBays25<=0)p.driveBays25=p.formFactor=="MiniITX"?2:4;
                    if(p.driveBays35<=0)p.driveBays35=p.formFactor=="MiniITX"?1:2;
                    if(p.radiatorSupportMm<=0)p.radiatorSupportMm=p.formFactor=="MiniITX"?240:(q>=78?360:280);
                    if(p.gpuSlotWidth<=0)p.gpuSlotWidth=p.formFactor=="MiniITX"?2.5f:(q>=80?4.0f:3.2f);
                    break;
                case PartCategory.Motherboard:
                    if(p.dimmSlots<=0)p.dimmSlots=p.formFactor=="MiniITX"?2:4;
                    if(p.maxMemoryGB<=0)p.maxMemoryGB=p.memoryType=="DDR5"?192:128;
                    if(p.memoryChannels<=0)p.memoryChannels=2;
                    if(p.m2Slots<=0)p.m2Slots=p.formFactor=="MiniITX"?2:(q>=80?4:2);
                    if(p.sataPorts<=0)p.sataPorts=p.formFactor=="MiniITX"?4:6;
                    if(p.pcieX16Slots<=0)p.pcieX16Slots=p.formFactor=="MiniITX"?1:2;
                    if(p.fanHeaders<=0)p.fanHeaders=p.formFactor=="MiniITX"?3:(q>=80?7:5);
                    if(string.IsNullOrEmpty(p.firmwareTier))p.firmwareTier=q>=82?"enthusiast":q>=68?"advanced":"standard";
                    AddConnector(p,"ATX24");AddConnector(p,"EPS8");AddConnector(p,"FRONT_PANEL");AddConnector(p,"CPU_FAN");AddConnector(p,"SATA");
                    break;
                case PartCategory.CPU:
                    if(p.coreCount<=0)p.coreCount=Mathf.Clamp(Mathf.RoundToInt(p.performance/85f),4,24);
                    if(p.threadCount<=0)p.threadCount=Mathf.Max(p.coreCount,p.coreCount*2);
                    if(p.baseClockMHz<=0)p.baseClockMHz=2800+q*12;
                    if(p.boostClockMHz<=0)p.boostClockMHz=p.baseClockMHz+700+q*6;
                    if(p.idlePowerWatts<=0)p.idlePowerWatts=Mathf.Max(6f,p.powerWatts*.12f);
                    break;
                case PartCategory.GPU:
                    if(p.gpuSlotWidth<=0)p.gpuSlotWidth=p.powerWatts>=300?3.5f:p.powerWatts>=190?2.7f:2.0f;
                    if(p.baseClockMHz<=0)p.baseClockMHz=1200+q*8;
                    if(p.boostClockMHz<=0)p.boostClockMHz=p.baseClockMHz+450+q*5;
                    if(p.idlePowerWatts<=0)p.idlePowerWatts=Mathf.Max(10f,p.powerWatts*.08f);
                    break;
                case PartCategory.Storage:
                    if(p.storageInterface=="NVMe")
                    {
                        if(p.readMBs<=0)p.readMBs=p.pcieGeneration>=4?Mathf.RoundToInt(Mathf.Lerp(4200,7400,q/100f)):Mathf.RoundToInt(Mathf.Lerp(1600,3500,q/100f));
                        if(p.writeMBs<=0)p.writeMBs=Mathf.RoundToInt(p.readMBs*.82f);
                    }
                    else
                    {
                        if(p.readMBs<=0)p.readMBs=540;
                        if(p.writeMBs<=0)p.writeMBs=500;
                    }
                    if(p.enduranceTBW<=0)p.enduranceTBW=Mathf.Max(80,p.storageGB/2);
                    if(p.idlePowerWatts<=0)p.idlePowerWatts=1.2f;
                    break;
                case PartCategory.PSU:
                    if(p.efficiencyClass<=0)p.efficiencyClass=q>=88?92:q>=75?88:84;
                    p.modularPsu=p.modularPsu||q>=72;
                    AddConnector(p,"ATX24");AddConnector(p,"EPS8");AddConnector(p,"SATA_POWER");
                    if(p.psuWattage>=500)AddConnector(p,"PCIE8");
                    if(p.psuWattage>=850)AddConnector(p,"12V2x6");
                    break;
                case PartCategory.Cooler:
                    if(p.airflowCfm<=0)p.airflowCfm=Mathf.Lerp(38f,82f,q/100f);
                    if(p.maxRpm<=0)p.maxRpm=Mathf.RoundToInt(Mathf.Lerp(1300,2300,q/100f));
                    if(p.fanSizeMm<=0)p.fanSizeMm=p.tags.Contains("aio")?120:120;
                    if(p.tags.Contains("aio")&&p.radiatorSupportMm<=0)p.radiatorSupportMm=p.performance>=160?360:p.performance>=120?280:240;
                    break;
                case PartCategory.Fan:
                    if(p.fanSizeMm<=0)p.fanSizeMm=120;
                    if(p.maxRpm<=0)p.maxRpm=Mathf.RoundToInt(Mathf.Lerp(1100,2200,q/100f));
                    if(p.airflowCfm<=0)p.airflowCfm=Mathf.Lerp(35f,78f,q/100f);
                    if(p.staticPressure<=0)p.staticPressure=Mathf.Lerp(.9f,2.7f,q/100f);
                    break;
            }
        }

        private static void AddConnector(HardwareDefinition p,string connector)
        {
            if(!p.connectors.Contains(connector))p.connectors.Add(connector);
        }

        public HardwareDefinition Get(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            HardwareDefinition result;
            return byId.TryGetValue(id, out result) ? result : null;
        }

        public List<HardwareDefinition> ByCategory(PartCategory category)
        {
            return byId.Values.Where(p => p.category == category).OrderBy(p => p.price).ToList();
        }

        public List<HardwareDefinition> Search(string text, PartCategory? category = null)
        {
            text = (text ?? string.Empty).Trim().ToLowerInvariant();
            return byId.Values.Where(p => (!category.HasValue || p.category == category) &&
                (text.Length == 0 || (p.brand + " " + p.model + " " + p.id).ToLowerInvariant().Contains(text)))
                .OrderBy(p => p.price).ToList();
        }
    }
}
