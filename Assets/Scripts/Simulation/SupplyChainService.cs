using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Deterministic supplier market, warehouse-aware receiving and returns.</summary>
    public sealed class SupplyChainService
    {
        private readonly GameState state;private readonly HardwareCatalog catalog;private readonly InventoryService inventory;private readonly EconomyService economy;private readonly ShippingService shipping;
        public SupplyChainService(GameState s,HardwareCatalog c,InventoryService i,EconomyService e,ShippingService sh){state=s;catalog=c;inventory=i;economy=e;shipping=sh;}

        private ProgressionService Progression => new ProgressionService(state,catalog);

        public float Quote(HardwareDefinition d)
        {
            if(d==null)return 0f;int seed=Mathf.Abs((d.id??string.Empty).GetHashCode());float cycle=.92f+((seed+state.day*7)%23)/100f;float repDiscount=Mathf.Clamp(state.reputation/2500f,0f,.12f);float certification=Progression.ProcurementMultiplier;return Mathf.Max(1f,d.price*cycle*(1f-repDiscount)*certification);
        }

        public bool IsMarketUnlocked(HardwareDefinition d)
        {
            if(d==null)return false;ProgressionService p=Progression;p.RefreshPartUnlocks();return state.unlockedPartIds.Count==0||state.unlockedPartIds.Contains(d.id);
        }

        public ActionResult Order(string definitionId,int quantity,int speedTier)
        {
            HardwareDefinition d=catalog.Get(definitionId);if(d==null||quantity<1||quantity>20)return ActionResult.Fail("Invalid supplier order.");
            if(!IsMarketUnlocked(d))return ActionResult.Fail(d.model+" is not unlocked by your current technician certification.");
            WorkshopBusinessService business=new WorkshopBusinessService(state,catalog,inventory,economy,shipping);int inbound=state.shipments.Where(x=>x.status==ShipmentStatus.InTransit||x.status==ShipmentStatus.Delivered).SelectMany(x=>x.lines).Sum(x=>x.quantity);if(business.WarehouseUsed+inbound+quantity>business.WarehouseCapacity)return ActionResult.Fail("Order would exceed warehouse capacity including inbound stock.");
            float unit=Quote(d);float shippingCost=speedTier<=0?3f:speedTier==1?9f:24f;float total=unit*quantity+shippingCost;ActionResult pay=economy.Spend(total,"Supplier order: "+d.model+" x"+quantity);if(!pay.ok)return pay;
            ShipmentState sh=new ShipmentState{shipmentId="S"+state.nextShipmentSerial++.ToString("D5"),orderedDay=state.day,deliveryDay=state.day+(speedTier<=0?3:speedTier==1?1:0),speedTier=speedTier,status=ShipmentStatus.InTransit};sh.lines.Add(new ShipmentLine{definitionId=d.id,quantity=quantity,unitPrice=unit});int eventKey=Mathf.Abs((d.id+state.day+sh.shipmentId).GetHashCode())%100;if(speedTier<=0&&eventKey<6){sh.delayed=true;sh.deliveryDay++;}if(eventKey==97)sh.wrongItemEvent=true;state.shipments.Add(sh);return ActionResult.Success(d.model+" ×"+quantity+" ordered for $"+total.ToString("0")+". ETA day "+sh.deliveryDay+".");
        }

        public ActionResult Receive(string shipmentId)
        {
            ShipmentState s=state.shipments.FirstOrDefault(x=>x.shipmentId==shipmentId);if(s==null||s.status!=ShipmentStatus.Delivered)return ActionResult.Fail("Shipment has not arrived.");WorkshopBusinessService b=new WorkshopBusinessService(state,catalog,inventory,economy,shipping);int qty=s.lines.Sum(x=>x.quantity);if(b.WarehouseUsed+qty>b.WarehouseCapacity)return ActionResult.Fail("Warehouse full. Sell stock or upgrade storage before receiving.");
            if(s.wrongItemEvent)return ActionResult.Fail("Receiving inspection found a supplier mismatch. Return the shipment instead of stocking it.");return shipping.Receive(shipmentId);
        }

        public ActionResult ReturnSupplierMismatch(string shipmentId)
        {
            ShipmentState s=state.shipments.FirstOrDefault(x=>x.shipmentId==shipmentId);if(s==null||s.status!=ShipmentStatus.Delivered||!s.wrongItemEvent)return ActionResult.Fail("Shipment is not eligible for mismatch return.");float refund=s.lines.Sum(x=>x.quantity*x.unitPrice);s.status=ShipmentStatus.Returned;economy.Earn(refund,"Supplier mismatch refund "+s.shipmentId);return ActionResult.Success("Shipment returned. Refunded $"+refund.ToString("0")+".");
        }

        public ActionResult CancelInTransit(string shipmentId)
        {
            ShipmentState s=state.shipments.FirstOrDefault(x=>x.shipmentId==shipmentId);if(s==null||s.status!=ShipmentStatus.InTransit)return ActionResult.Fail("Only in-transit orders can be cancelled.");if(s.deliveryDay<=state.day)return ActionResult.Fail("Order is already at the local receiving stage.");float goods=s.lines.Sum(x=>x.quantity*x.unitPrice);float restock=goods*.08f;float refund=Mathf.Max(0f,goods-restock);s.status=ShipmentStatus.Returned;economy.Earn(refund,"Cancelled supplier order "+s.shipmentId);return ActionResult.Success("Order cancelled. $"+refund.ToString("0")+" refunded after restocking fee.");
        }

        public IEnumerable<HardwareDefinition> Market(PartCategory? category=null,int max=40)
        {
            ProgressionService p=Progression;p.RefreshPartUnlocks();IEnumerable<HardwareDefinition> q=catalog.All.Where(IsMarketUnlocked);if(category.HasValue)q=q.Where(x=>x.category==category.Value);return q.OrderBy(x=>Quote(x)).ThenByDescending(x=>x.quality).Take(max);
        }
    }
}
