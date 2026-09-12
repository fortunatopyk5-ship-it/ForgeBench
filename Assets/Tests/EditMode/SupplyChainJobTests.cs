using System.Linq;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class SupplyChainJobTests
    {
        private static HardwareCatalog Catalog(){HardwareCatalog c=new HardwareCatalog();c.Load();return c;}

        [Test]
        public void AdvancedGenerator_CreatesPersistentVariedOffers()
        {
            GameState s=new GameState{reputation=260,day=7};AdvancedJobGeneratorService g=new AdvancedJobGeneratorService(s);g.EnsureOffers(3);var offers=s.jobs.Where(x=>x.stage==JobStage.Offered&&AdvancedJobGeneratorService.IsAdvanced(x)).ToList();Assert.AreEqual(3,offers.Count);Assert.IsTrue(offers.All(x=>x.dueDay>s.day));Assert.IsTrue(offers.All(x=>x.reward>0&&x.budget>0));Assert.GreaterOrEqual(offers.Select(x=>x.title).Distinct().Count(),2);
        }

        [Test]
        public void SupplierOrder_ReservesCapacityAndPersistsQuotedUnitPrice()
        {
            GameState s=new GameState{money=10000f};HardwareCatalog c=Catalog();InventoryService inv=new InventoryService(s,c);EconomyService eco=new EconomyService(s);ShippingService sh=new ShippingService(s,c,inv,eco);SupplyChainService supply=new SupplyChainService(s,c,inv,eco,sh);HardwareDefinition cpu=c.ByCategory(PartCategory.CPU).First();ActionResult result=supply.Order(cpu.id,2,1);Assert.IsTrue(result.ok,result.message);Assert.AreEqual(1,s.shipments.Count);Assert.AreEqual(2,s.shipments[0].lines[0].quantity);Assert.Greater(s.shipments[0].lines[0].unitPrice,0f);Assert.AreEqual(ShipmentStatus.InTransit,s.shipments[0].status);
        }

        [Test]
        public void Receiving_RejectsWhenWarehouseCapacityExceeded()
        {
            GameState s=new GameState{money=10000f};HardwareCatalog c=Catalog();InventoryService inv=new InventoryService(s,c);EconomyService eco=new EconomyService(s);ShippingService sh=new ShippingService(s,c,inv,eco);SupplyChainService supply=new SupplyChainService(s,c,inv,eco,sh);HardwareDefinition ram=c.ByCategory(PartCategory.RAM).First();int cap=new WorkshopBusinessService(s,c,inv,eco,sh).WarehouseCapacity;for(int i=0;i<cap;i++)inv.Create(ram.id);ShipmentState shipment=new ShipmentState{shipmentId="CAP",orderedDay=1,deliveryDay=1,status=ShipmentStatus.Delivered};shipment.lines.Add(new ShipmentLine{definitionId=ram.id,quantity=1,unitPrice=ram.price});s.shipments.Add(shipment);Assert.IsFalse(supply.Receive("CAP").ok);Assert.AreEqual(ShipmentStatus.Delivered,shipment.status);
        }
    }
}
