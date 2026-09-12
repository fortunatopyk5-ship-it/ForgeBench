using UnityEngine;

namespace ForgeBench
{
    public static class BusinessRuntimeExtensions
    {
        private static WorkshopBusinessService S(GameRuntime g)=>new WorkshopBusinessService(g.State,g.Catalog,g.Inventory,g.Economy,g.Shipping);
        private static void Apply(GameRuntime g,ActionResult r){if(g==null)return;if(r.ok)g.Saves?.Save(g.State,1);g.UI?.Refresh();g.Notify(r.message,r.ok);}
        public static void BusinessHireHelper(this GameRuntime g)=>Apply(g,S(g).HireHelper());
        public static void BusinessHireSpecialist(this GameRuntime g)=>Apply(g,S(g).HireSpecialist());
        public static void BusinessFireHelper(this GameRuntime g)=>Apply(g,S(g).FireHelper());
        public static void BusinessFireSpecialist(this GameRuntime g)=>Apply(g,S(g).FireSpecialist());
        public static void BusinessToggleAutomation(this GameRuntime g)=>Apply(g,S(g).ToggleDeliveryAutomation());
        public static void BusinessRefurbish(this GameRuntime g,string itemId)=>Apply(g,S(g).Refurbish(itemId));
        public static void BusinessSell(this GameRuntime g,string itemId)=>Apply(g,S(g).Sell(itemId));
        public static void BusinessImproveCleanliness(this GameRuntime g)=>Apply(g,S(g).ImproveCleanliness());
        public static WorkshopBusinessService Business(this GameRuntime g)=>g==null?null:S(g);
    }
}
