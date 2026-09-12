using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public static class MaintenanceRuntimeExtensions
    {
        public static void RunPreventiveMaintenance(this GameRuntime game)
        {
            if(game==null||game.State==null)return;MachineState m=game.ActiveMachine;if(m==null){game.Notify("No device available for preventive maintenance.",false);return;}
            ItemInstance cleaning=game.Inventory.Available(PartCategory.Consumable).FirstOrDefault(i=>game.Inventory.Def(i)?.tags.Contains("cleaning")==true);
            if(cleaning==null)
            {
                if(game.State.workshop.level<2){game.Notify("Cleaning consumable is required at the current workshop level.",false);return;}
                ActionResult supply=game.Economy.Spend(6f,"Workshop preventive-service supplies");if(!supply.ok){game.Notify(supply.message,false);return;}
            }
            else game.Inventory.Consume(cleaning.instanceId);
            ReliabilitySimulationService service=new ReliabilitySimulationService(game.State,game.Catalog,game.Inventory);ActionResult r=service.PreventiveMaintenance(m);if(r.ok){game.Saves?.Save(game.State,1);game.World?.RefreshMachine();game.UI?.Refresh();}game.Notify(r.message,r.ok);MaintenancePanel.RefreshIfOpen();
        }

        public static string ReliabilityReport(this GameRuntime game)
        {
            if(game==null||game.ActiveMachine==null)return "No active device.";return new ReliabilitySimulationService(game.State,game.Catalog,game.Inventory).ReliabilityReport(game.ActiveMachine);
        }
    }
}
