using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Deterministic supplier market with finite daily stock, supplier reliability,
    /// volume pricing, progression-aware quotes, warehouse reservation, receiving,
    /// cancellation and inspection returns. Market state is derived from save/day and
    /// outstanding shipments so reloads cannot reroll price or stock.
    /// </summary>
    public sealed class SupplyChainService
    {
        private readonly GameState state;private readonly HardwareCatalog catalog;private readonly InventoryService inventory;private readonly EconomyService economy;private readonly ShippingService shipping;
        public SupplyChainService(GameState s,HardwareCatalog c,InventoryService i,EconomyService e,ShippingService sh){state=s;catalog=c;inventory=i;economy=e;shipping=sh;}
        private ProgressionService Progression=>new ProgressionService(state,catalog);

        public string SupplierName(HardwareDefinition d)
        {
            if(d==null)return "Unknown supplier";
            switch(d.category)
            {
                case PartCategory.CPU:case PartCategory.Motherboard:case PartCategory.RAM:return "Vector Silicon Distribution";
                case PartCategory.GPU:case PartCategory.PSU:case PartCategory.Cooler:return "Northbridge Components";
                case PartCategory.Storage:case PartCategory.Network:return "Packet & Storage Wholesale";
                case PartCategory.Case:case PartCategory.Fan:return "Forge Chassis Supply";
                case PartCategory.Battery:case PartCategory.Display:case PartCategory.Controller:return "Mobile Parts Cooperative";
                case PartCategory.Tool:case PartCategory.Consumable:return "Workshop Essentials";
                default:return "ForgeBench General Supply";
            }
        }

        public float SupplierReliability(HardwareDefinition d)
        {
            if(d==null)return .70f;
            int seed=StableHash(SupplierName(d));
            float baseReliability=.82f+(seed%12)/100f;
            float reputationBonus=Mathf.Clamp(state.reputation/5000f,0f,.05f);
            return Mathf.Clamp01(baseReliability+reputationBonus);
        }

        public int BaseDailyStock(HardwareDefinition d)
        {
            if(d==null)return 0;
            int seed=StableHash((d.id??string.Empty)+"|stock|"+state.day);
            int categoryBias=(d.category==PartCategory.GPU||d.category==PartCategory.CPU)?0:(d.category==PartCategory.Consumable||d.category==PartCategory.Fan?5:2);
            int scarcity=d.quality>=8?2:d.quality>=6?1:0;
            return Mathf.Clamp(5+(seed%10)+categoryBias-scarcity,3,22);
        }

        public int AvailableStock(HardwareDefinition d)
        {
            if(d==null)return 0;
            int committed=state.shipments.Where(x=>x.status==ShipmentStatus.InTransit||x.status==ShipmentStatus.Delivered).SelectMany(x=>x.lines).Where(x=>x.definitionId==d.id).Sum(x=>x.quantity);
            return Mathf.Max(0,BaseDailyStock(d)-committed);
        }

        public float VolumeDiscount(int quantity)
        {
            if(quantity>=16)return .11f;
            if(quantity>=8)return .075f;
            if(quantity>=4)return .04f;
            return 0f;
        }

        public float Quote(HardwareDefinition d)
        {
            if(d==null)return 0f;
            int seed=StableHash(d.id??string.Empty);float cycle=.92f+((seed+state.day*7)%23)/100f;
            float repDiscount=Mathf.Clamp(state.reputation/2500f,0f,.12f);float certification=Progression.ProcurementMultiplier;
            float reliabilityPremium=Mathf.Lerp(.97f,1.025f,SupplierReliability(d));
            return Mathf.Max(1f,d.price*cycle*(1f-repDiscount)*certification*reliabilityPremium);
        }

        public float UnitQuote(HardwareDefinition d,int quantity)=>Quote(d)*(1f-VolumeDiscount(quantity));

        public bool IsMarketUnlocked(HardwareDefinition d)
        {
            if(d==null)return false;ProgressionService p=Progression;p.RefreshPartUnlocks();return state.unlockedPartIds.Count==0||state.unlockedPartIds.Contains(d.id);
        }

        public ActionResult Order(string definitionId,int quantity,int speedTier)
        {
            HardwareDefinition d=catalog.Get(definitionId);if(d==null||quantity<1||quantity>20)return ActionResult.Fail("Invalid supplier order.");
            if(!IsMarketUnlocked(d))return ActionResult.Fail(d.model+" is not unlocked by your current technician certification.");
            int available=AvailableStock(d);if(quantity>available)return ActionResult.Fail("Supplier stock insufficient: "+available+" unit(s) available today.");
            WorkshopBusinessService business=new WorkshopBusinessService(state,catalog,inventory,economy,shipping);int inbound=state.shipments.Where(x=>x.status==ShipmentStatus.InTransit||x.status==ShipmentStatus.Delivered).SelectMany(x=>x.lines).Sum(x=>x.quantity);if(business.WarehouseUsed+inbound+quantity>business.WarehouseCapacity)return ActionResult.Fail("Order would exceed warehouse capacity including inbound stock.");
            float unit=UnitQuote(d,quantity);float shippingCost=speedTier<=0?3f:speedTier==1?9f:24f;float total=unit*quantity+shippingCost;ActionResult pay=economy.Spend(total,"Supplier order: "+d.model+" x"+quantity);if(!pay.ok)return pay;
            ShipmentState sh=new ShipmentState{shipmentId="S"+state.nextShipmentSerial++.ToString("D5"),orderedDay=state.day,deliveryDay=state.day+(speedTier<=0?3:speedTier==1?1:0),speedTier=speedTier,status=ShipmentStatus.InTransit};sh.lines.Add(new ShipmentLine{definitionId=d.id,quantity=quantity,unitPrice=unit});
            int eventKey=StableHash((d.id??"")+"|"+state.day+"|"+sh.shipmentId)%1000;float reliability=SupplierReliability(d);int delayThreshold=Mathf.RoundToInt((1f-reliability)*220f);int mismatchThreshold=Mathf.RoundToInt((1f-reliability)*28f);
            if(speedTier<=0&&eventKey<delayThreshold){sh.delayed=true;sh.deliveryDay++;}
            if(eventKey>=1000-mismatchThreshold)sh.wrongItemEvent=true;
            state.shipments.Add(sh);
            float discount=VolumeDiscount(quantity);
            return ActionResult.Success(d.model+" ×"+quantity+" ordered from "+SupplierName(d)+" for $"+total.ToString("0")+(discount>0?" (volume discount "+Mathf.RoundToInt(discount*100f)+"%)":"")+". ETA day "+sh.deliveryDay+".");
        }

        public ActionResult Receive(string shipmentId)
        {
            ShipmentState s=state.shipments.FirstOrDefault(x=>x.shipmentId==shipmentId);if(s==null||s.status!=ShipmentStatus.Delivered)return ActionResult.Fail("Shipment has not arrived.");WorkshopBusinessService b=new WorkshopBusinessService(state,catalog,inventory,economy,shipping);int qty=s.lines.Sum(x=>x.quantity);if(b.WarehouseUsed+qty>b.WarehouseCapacity)return ActionResult.Fail("Warehouse full. Sell stock or upgrade storage before receiving.");
            if(s.wrongItemEvent)return ActionResult.Fail("Receiving inspection found a supplier mismatch. Return the shipment instead of stocking it.");return shipping.Receive(shipmentId);
        }

        public ActionResult ReturnSupplierMismatch(string shipmentId)
        {
            ShipmentState s=state.shipments.FirstOrDefault(x=>x.shipmentId==shipmentId);if(s==null||s.status!=ShipmentStatus.Delivered||!s.wrongItemEvent)return ActionResult.Fail("Shipment is not eligible for mismatch return.");float refund=s.lines.Sum(x=>x.quantity*x.unitPrice);s.status=ShipmentStatus.Returned;economy.Earn(refund,"Supplier mismatch refund "+s.shipmentId);return ActionResult.Success("Shipment returned after receiving inspection. Refunded $"+refund.ToString("0")+".");
        }

        public ActionResult CancelInTransit(string shipmentId)
        {
            ShipmentState s=state.shipments.FirstOrDefault(x=>x.shipmentId==shipmentId);if(s==null||s.status!=ShipmentStatus.InTransit)return ActionResult.Fail("Only in-transit orders can be cancelled.");if(s.deliveryDay<=state.day)return ActionResult.Fail("Order is already at the local receiving stage.");float goods=s.lines.Sum(x=>x.quantity*x.unitPrice);float restock=goods*.08f;float refund=Mathf.Max(0f,goods-restock);s.status=ShipmentStatus.Returned;economy.Earn(refund,"Cancelled supplier order "+s.shipmentId);return ActionResult.Success("Order cancelled. $"+refund.ToString("0")+" refunded after restocking fee.");
        }

        public IEnumerable<HardwareDefinition> Market(PartCategory? category=null,int max=40)
        {
            ProgressionService p=Progression;p.RefreshPartUnlocks();IEnumerable<HardwareDefinition> q=catalog.All.Where(IsMarketUnlocked);if(category.HasValue)q=q.Where(x=>x.category==category.Value);return q.OrderByDescending(x=>AvailableStock(x)>0).ThenBy(x=>Quote(x)).ThenByDescending(x=>x.quality).Take(max);
        }

        private static int StableHash(string value)
        {
            unchecked{uint h=2166136261u;string s=value??string.Empty;for(int i=0;i<s.Length;i++){h^=s[i];h*=16777619u;}return (int)(h&0x7FFFFFFF);}
        }
    }
}
