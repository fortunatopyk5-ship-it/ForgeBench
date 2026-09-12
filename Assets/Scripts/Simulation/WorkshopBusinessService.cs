using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Business layer built on the persistent workshop, inventory and ledger models.
    /// Capacity, payroll and resale values are deterministic and tied to progression.
    /// </summary>
    public sealed class WorkshopBusinessService
    {
        private readonly GameState state;
        private readonly HardwareCatalog catalog;
        private readonly InventoryService inventory;
        private readonly EconomyService economy;
        private readonly ShippingService shipping;

        public WorkshopBusinessService(GameState s,HardwareCatalog c,InventoryService i,EconomyService e,ShippingService sh)
        {state=s;catalog=c;inventory=i;economy=e;shipping=sh;}

        public int WarehouseCapacity => 45+state.workshop.storageLevel*45+state.workshop.helperStaff*8;
        public int WarehouseUsed => state.inventory.Count(x=>!x.customerOwned);
        public float Utilization => WarehouseCapacity<=0?1f:WarehouseUsed/(float)WarehouseCapacity;
        public float DailyPayroll => state.workshop.helperStaff*18f+state.workshop.specialistStaff*32f;
        public float DailyOverhead => 9f+state.workshop.level*3f+DailyPayroll;

        public ActionResult HireHelper()
        {
            int cap=1+state.workshop.level; if(state.workshop.helperStaff>=cap)return ActionResult.Fail("Helper staff capacity reached for this workshop level.");
            float onboarding=90f+state.workshop.helperStaff*45f;ActionResult pay=economy.Spend(onboarding,"Hire workshop helper");if(!pay.ok)return pay;
            state.workshop.helperStaff++;state.milestones.Add("Hired helper #"+state.workshop.helperStaff);return ActionResult.Success("Workshop helper hired. Daily wage $18; warehouse/receiving capacity improved.");
        }

        public ActionResult HireSpecialist()
        {
            if(state.workshop.level<3)return ActionResult.Fail("Specialists require workshop level 3.");
            int cap=Mathf.Max(1,state.workshop.level-2);if(state.workshop.specialistStaff>=cap)return ActionResult.Fail("Specialist staff capacity reached.");
            float onboarding=240f+state.workshop.specialistStaff*110f;ActionResult pay=economy.Spend(onboarding,"Hire repair specialist");if(!pay.ok)return pay;
            state.workshop.specialistStaff++;state.workshop.boardRepairLevel=Mathf.Max(state.workshop.boardRepairLevel,1+state.workshop.specialistStaff/2);state.milestones.Add("Hired specialist #"+state.workshop.specialistStaff);return ActionResult.Success("Repair specialist hired. Daily wage $32; advanced repair quality improved.");
        }

        public ActionResult FireHelper()
        {
            if(state.workshop.helperStaff<=0)return ActionResult.Fail("No helper staff to release.");state.workshop.helperStaff--;return ActionResult.Success("One helper released. Daily payroll reduced by $18.");
        }
        public ActionResult FireSpecialist()
        {
            if(state.workshop.specialistStaff<=0)return ActionResult.Fail("No specialist staff to release.");state.workshop.specialistStaff--;return ActionResult.Success("One specialist released. Daily payroll reduced by $32.");
        }

        public ActionResult ToggleDeliveryAutomation()
        {
            if(!state.workshop.deliveryAutomation)
            {
                if(state.workshop.level<2||state.workshop.helperStaff<1)return ActionResult.Fail("Delivery automation requires workshop level 2 and at least one helper.");
                ActionResult pay=economy.Spend(180f,"Install receiving automation");if(!pay.ok)return pay;state.workshop.deliveryAutomation=true;state.milestones.Add("Automated receiving installed");return ActionResult.Success("Receiving automation enabled. Helpers will process delivered shipments each game day.");
            }
            state.workshop.deliveryAutomation=false;return ActionResult.Success("Receiving automation disabled.");
        }

        public ActionResult Refurbish(string itemId)
        {
            ItemInstance item=inventory.Get(itemId);if(item==null||item.customerOwned||item.reserved)return ActionResult.Fail("Only unreserved workshop-owned inventory can be refurbished.");
            HardwareDefinition d=inventory.Def(item);if(d==null)return ActionResult.Fail("Missing part definition.");
            if(item.damage==DamageType.BentPins||item.damage==DamageType.BurnedConnector||item.damage==DamageType.LiquidContamination)
            {
                if(state.workshop.boardRepairLevel<1)return ActionResult.Fail("This physical damage requires the board-repair station.");
            }
            float cost=Mathf.Max(3f,d.price*.035f);ActionResult pay=economy.Spend(cost,"Refurbish "+d.model);if(!pay.ok)return pay;
            float skill=.06f+state.workshop.benchLevel*.025f+state.workshop.specialistStaff*.045f;item.condition=Mathf.Clamp01(item.condition+skill);item.dust=0f;item.wear=Mathf.Max(0f,item.wear-.04f);
            if(item.condition>.72f&&item.fault==FaultType.Dust)item.fault=FaultType.None;
            if(item.condition>.82f&&state.workshop.boardRepairLevel>=2)item.damage=DamageType.None;
            item.note="Refurbished in workshop";return ActionResult.Success(d.model+" refurbished to "+Mathf.RoundToInt(item.condition*100f)+"% condition.");
        }

        public ActionResult Sell(string itemId)
        {
            ItemInstance item=inventory.Get(itemId);if(item==null||item.customerOwned||item.reserved)return ActionResult.Fail("Item cannot be sold while reserved or customer-owned.");
            HardwareDefinition d=inventory.Def(item);if(d==null)return ActionResult.Fail("Missing part definition.");
            float faultPenalty=item.fault==FaultType.None?1f:.38f;float damagePenalty=item.damage==DamageType.None?1f:.45f;float condition=Mathf.Clamp(item.condition,.05f,1f);float demand=.88f+((state.day+Mathf.Abs(d.id.GetHashCode()))%17)/100f;float value=d.price*(.32f+.38f*condition)*faultPenalty*damagePenalty*demand;
            if(!state.inventory.Remove(item))return ActionResult.Fail("Inventory changed before sale could complete.");economy.Earn(value,"Sold used "+d.model);return ActionResult.Success("Sold "+d.model+" for $"+value.ToString("0")+".");
        }

        public List<ItemInstance> SaleCandidates(int max=20)
        {
            return state.inventory.Where(x=>!x.customerOwned&&!x.reserved).OrderBy(x=>x.condition).ThenBy(x=>catalog.Get(x.definitionId)?.price??0f).Take(max).ToList();
        }

        public ActionResult ProcessAutomatedReceiving()
        {
            if(!state.workshop.deliveryAutomation||state.workshop.helperStaff<=0)return ActionResult.Fail("Receiving automation is not staffed.");
            int free=Mathf.Max(0,WarehouseCapacity-WarehouseUsed);int received=0;
            foreach(ShipmentState s in state.shipments.Where(x=>x.status==ShipmentStatus.Delivered).ToList())
            {
                int qty=s.lines.Sum(l=>l.quantity);if(qty>free)break;ActionResult r=shipping.Receive(s.shipmentId);if(r.ok){received+=qty;free-=qty;}
            }
            return received>0?ActionResult.Success("Helpers received "+received+" item(s) into warehouse."):ActionResult.Fail("No delivered shipments could be auto-received or warehouse is full.");
        }

        public ActionResult ImproveCleanliness()
        {
            if(state.workshop.helperStaff<=0)return ActionResult.Fail("Hire helper staff for daily workshop maintenance.");
            float before=state.workshop.cleanliness;state.workshop.cleanliness=Mathf.Clamp01(before+.035f*state.workshop.helperStaff);return ActionResult.Success("Workshop cleanliness "+Mathf.RoundToInt(before*100f)+"% → "+Mathf.RoundToInt(state.workshop.cleanliness*100f)+"%.");
        }
    }
}
