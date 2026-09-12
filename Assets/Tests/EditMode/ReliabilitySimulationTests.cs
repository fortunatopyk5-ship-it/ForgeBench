using System.Linq;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class ReliabilitySimulationTests
    {
        private static HardwareCatalog Catalog(){HardwareCatalog c=new HardwareCatalog();c.Load();return c;}
        private static string Add(InventoryService inv,MachineState m,string id){ItemInstance i=inv.Create(id);i.reserved=true;i.note="Installed in "+m.machineId;return i.instanceId;}
        private static MachineState Build(GameState state,HardwareCatalog c,InventoryService inv)
        {
            MachineState m=new MachineState{machineId="REL-1",dust=.25f};HardwareDefinition pcCase=c.ByCategory(PartCategory.Case).First();HardwareDefinition board=c.ByCategory(PartCategory.Motherboard).First();HardwareDefinition cpu=c.ByCategory(PartCategory.CPU).FirstOrDefault(x=>x.socket==board.socket)??c.ByCategory(PartCategory.CPU).First();HardwareDefinition psu=c.ByCategory(PartCategory.PSU).First();HardwareDefinition cooler=c.ByCategory(PartCategory.Cooler).First();HardwareDefinition disk=c.ByCategory(PartCategory.Storage).First();m.caseItemId=Add(inv,m,pcCase.id);m.motherboardItemId=Add(inv,m,board.id);m.cpuItemId=Add(inv,m,cpu.id);m.psuItemId=Add(inv,m,psu.id);m.coolerItemId=Add(inv,m,cooler.id);m.storageItemIds.Add(Add(inv,m,disk.id));state.machines.Add(m);return m;
        }

        [Test]
        public void DailyReliability_IncreasesWearAndStorageCounters()
        {
            GameState state=new GameState();HardwareCatalog c=Catalog();InventoryService inv=new InventoryService(state,c);MachineState m=Build(state,c,inv);ItemInstance cpu=inv.Get(m.cpuItemId);ItemInstance disk=inv.Get(m.storageItemIds[0]);float wear=cpu.wear,writes=disk.writtenGB;ReliabilitySimulationService s=new ReliabilitySimulationService(state,c,inv);s.AdvanceMachineOneDay(m);Assert.Greater(cpu.wear,wear);Assert.Less(cpu.condition,1f);Assert.Greater(disk.writtenGB,writes);Assert.AreEqual(1,m.maintenance.thermalPasteAgeDays);
        }

        [Test]
        public void DailyReliability_IsDeterministicForEquivalentState()
        {
            GameState a=new GameState{day=17};HardwareCatalog c=Catalog();InventoryService ia=new InventoryService(a,c);MachineState ma=Build(a,c,ia);GameState b=new GameState{day=17};InventoryService ib=new InventoryService(b,c);MachineState mb=Build(b,c,ib);ReliabilitySimulationService sa=new ReliabilitySimulationService(a,c,ia),sb=new ReliabilitySimulationService(b,c,ib);sa.AdvanceMachineOneDay(ma);sb.AdvanceMachineOneDay(mb);Assert.AreEqual(ia.Get(ma.cpuItemId).wear,ib.Get(mb.cpuItemId).wear,.000001f);Assert.AreEqual(ia.Get(ma.cpuItemId).fault,ib.Get(mb.cpuItemId).fault);Assert.AreEqual(ma.dust,mb.dust,.000001f);
        }

        [Test]
        public void PreventiveMaintenance_ReducesDustAndResetsServiceAge()
        {
            GameState state=new GameState();HardwareCatalog c=Catalog();InventoryService inv=new InventoryService(state,c);MachineState m=Build(state,c,inv);m.dust=.9f;m.maintenance.filterDust=.85f;m.maintenance.serviceAgeDays=120;foreach(string id in new[]{m.cpuItemId,m.storageItemIds[0]})inv.Get(id).dust=.8f;ReliabilitySimulationService s=new ReliabilitySimulationService(state,c,inv);ActionResult r=s.PreventiveMaintenance(m);Assert.IsTrue(r.ok);Assert.Less(m.dust,.9f);Assert.Less(m.maintenance.filterDust,.85f);Assert.AreEqual(0,m.maintenance.serviceAgeDays);Assert.AreEqual(1,m.maintenance.cleaningCycles);Assert.Less(inv.Get(m.cpuItemId).dust,.8f);
        }
    }
}
