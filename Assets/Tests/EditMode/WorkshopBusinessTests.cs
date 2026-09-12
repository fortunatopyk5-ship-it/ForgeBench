using System.Linq;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class WorkshopBusinessTests
    {
        private static HardwareCatalog Catalog(){HardwareCatalog c=new HardwareCatalog();c.Load();return c;}
        private static WorkshopBusinessService Service(GameState s,HardwareCatalog c,InventoryService i,EconomyService e){return new WorkshopBusinessService(s,c,i,e,new ShippingService(s,c,i,e));}

        [Test]
        public void Hiring_ChargesOnboardingAndAddsDailyPayroll()
        {
            GameState state=new GameState{money=2000f};HardwareCatalog catalog=Catalog();InventoryService inv=new InventoryService(state,catalog);EconomyService eco=new EconomyService(state);WorkshopBusinessService business=Service(state,catalog,inv,eco);float before=state.money;
            Assert.IsTrue(business.HireHelper().ok);Assert.AreEqual(1,state.workshop.helperStaff);Assert.Less(state.money,before);Assert.AreEqual(18f,business.DailyPayroll,.01f);
        }

        [Test]
        public void DeliveryAutomation_RequiresLevelAndHelper()
        {
            GameState state=new GameState{money=2000f};HardwareCatalog catalog=Catalog();InventoryService inv=new InventoryService(state,catalog);EconomyService eco=new EconomyService(state);WorkshopBusinessService business=Service(state,catalog,inv,eco);
            Assert.IsFalse(business.ToggleDeliveryAutomation().ok);state.workshop.level=2;Assert.IsTrue(business.HireHelper().ok);Assert.IsTrue(business.ToggleDeliveryAutomation().ok);Assert.IsTrue(state.workshop.deliveryAutomation);
        }

        [Test]
        public void RefurbishAndSell_UsesRealInventoryAndLedger()
        {
            GameState state=new GameState{money=2000f};HardwareCatalog catalog=Catalog();InventoryService inv=new InventoryService(state,catalog);EconomyService eco=new EconomyService(state);WorkshopBusinessService business=Service(state,catalog,inv,eco);HardwareDefinition part=catalog.ByCategory(PartCategory.RAM).First();ItemInstance item=inv.Create(part.id);item.condition=.55f;string id=item.instanceId;
            Assert.IsTrue(business.Refurbish(id).ok);Assert.Greater(item.condition,.55f);float cash=state.money;Assert.IsTrue(business.Sell(id).ok);Assert.Greater(state.money,cash);Assert.IsNull(inv.Get(id));Assert.IsTrue(state.ledger.Count>=2);
        }

        [Test]
        public void WarehouseCapacity_GrowsWithStorageAndHelpers()
        {
            GameState state=new GameState();HardwareCatalog catalog=Catalog();InventoryService inv=new InventoryService(state,catalog);EconomyService eco=new EconomyService(state);WorkshopBusinessService business=Service(state,catalog,inv,eco);int start=business.WarehouseCapacity;state.workshop.storageLevel+=2;state.workshop.helperStaff=2;Assert.Greater(business.WarehouseCapacity,start);
        }
    }
}
