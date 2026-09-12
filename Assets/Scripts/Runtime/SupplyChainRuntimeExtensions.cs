namespace ForgeBench
{
    public static class SupplyChainRuntimeExtensions
    {
        private static SupplyChainService S(GameRuntime g)=>new SupplyChainService(g.State,g.Catalog,g.Inventory,g.Economy,g.Shipping);
        private static void Apply(GameRuntime g,ActionResult r){if(g==null)return;if(r.ok)g.Saves?.Save(g.State,1);g.UI?.Refresh();g.Notify(r.message,r.ok);}
        public static void SupplyOrder(this GameRuntime g,string definitionId,int quantity=1,int speedTier=1)=>Apply(g,S(g).Order(definitionId,quantity,speedTier));
        public static void SupplyReceive(this GameRuntime g,string shipmentId)=>Apply(g,S(g).Receive(shipmentId));
        public static void SupplyReturnMismatch(this GameRuntime g,string shipmentId)=>Apply(g,S(g).ReturnSupplierMismatch(shipmentId));
        public static void SupplyCancel(this GameRuntime g,string shipmentId)=>Apply(g,S(g).CancelInTransit(shipmentId));
        public static SupplyChainService Supply(this GameRuntime g)=>g==null?null:S(g);
    }
}
