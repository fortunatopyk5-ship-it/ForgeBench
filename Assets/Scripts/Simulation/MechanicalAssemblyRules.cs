using System;

namespace ForgeBench
{
    public static class MechanicalAssemblyRules
    {
        private static bool Has(string id) => !string.IsNullOrEmpty(id);
        private static ActionResult Access(MachineState machine, PartCategory category)
        {
            if (machine == null) return ActionResult.Fail("No device on bench.");
            if (machine.bootState != BootState.Off) return ActionResult.Fail("Power off the PC before servicing components.");
            if (category != PartCategory.Case && machine.sidePanelInstalled && Has(machine.caseItemId)) return ActionResult.Fail("Remove the side panel before accessing internal components.");
            return ActionResult.Success("Component accessible.");
        }

        public static ActionResult CanInstall(MachineState machine, PartCategory category)
        {
            ActionResult access = Access(machine, category); if (!access.ok) return access;
            if (category == PartCategory.CPU || category == PartCategory.RAM || category == PartCategory.GPU)
                if (!Has(machine.motherboardItemId)) return ActionResult.Fail("Install a motherboard before this component.");
            if (category == PartCategory.CPU && Has(machine.coolerItemId)) return ActionResult.Fail("Remove the CPU cooler before replacing the processor.");
            if (category == PartCategory.CPU && !machine.cpuRetentionOpen) return ActionResult.Fail("Open the CPU retention lever before inserting the processor.");
            if (category == PartCategory.Cooler)
            {
                if (Has(machine.coolerItemId)) return ActionResult.Fail("Remove the existing cooler before fitting another.");
                if (!Has(machine.cpuItemId)) return ActionResult.Fail("Install the CPU before its cooler.");
                if (machine.cpuRetentionOpen) return ActionResult.Fail("Close the CPU retention lever before installing the cooler.");
                if (!machine.thermalPasteApplied || machine.thermalPasteQuality <= 0f) return ActionResult.Fail("Apply fresh thermal compound before installing the cooler.");
            }
            if (category == PartCategory.Case) return CanRemove(machine, category, null);
            return ActionResult.Success("Installation prerequisites satisfied.");
        }

        public static ActionResult CanRemove(MachineState machine, PartCategory category, Func<string, HardwareDefinition> definition)
        {
            ActionResult access = Access(machine, category); if (!access.ok) return access;
            ActionResult mount = ComponentMountRules.CanRemove(machine, category); if (!mount.ok) return mount;
            if (category == PartCategory.CPU && Has(machine.coolerItemId)) return ActionResult.Fail("Remove the CPU cooler before releasing the processor.");
            if (category == PartCategory.CPU && !machine.cpuRetentionOpen) return ActionResult.Fail("Open the CPU retention lever before removing the processor.");
            if (category == PartCategory.Motherboard)
            {
                if (Has(machine.coolerItemId) || Has(machine.cpuItemId) || Has(machine.gpuItemId) || machine.ramItemIds.Count > 0)
                    return ActionResult.Fail("Remove CPU, cooler, RAM and graphics card before removing the motherboard.");
                if (definition != null)
                    foreach (string id in machine.storageItemIds)
                        if (definition(id)?.storageInterface == "NVMe") return ActionResult.Fail("Remove motherboard-mounted NVMe drives first.");
            }
            if (category == PartCategory.Case && (Has(machine.motherboardItemId) || Has(machine.cpuItemId) || Has(machine.gpuItemId) ||
                Has(machine.psuItemId) || Has(machine.coolerItemId) || machine.ramItemIds.Count > 0 || machine.storageItemIds.Count > 0 || machine.fanItemIds.Count > 0))
                return ActionResult.Fail("Remove every internal component before replacing the chassis.");
            return ActionResult.Success("Mechanical attachments released.");
        }

        public static ActionResult CanApplyPaste(MachineState machine)
        {
            ActionResult access = Access(machine, PartCategory.CPU); if (!access.ok) return access;
            if (!Has(machine.cpuItemId) || !Has(machine.motherboardItemId)) return ActionResult.Fail("Install the motherboard and CPU first.");
            if (Has(machine.coolerItemId)) return ActionResult.Fail("Remove the CPU cooler to reach the thermal interface.");
            if (machine.cpuRetentionOpen) return ActionResult.Fail("Close the CPU retention lever before applying thermal compound.");
            return ActionResult.Success("CPU thermal interface accessible.");
        }

        public static ActionResult ToggleCpuRetention(MachineState machine)
        {
            ActionResult access = Access(machine, PartCategory.CPU); if (!access.ok) return access;
            if (!Has(machine.motherboardItemId)) return ActionResult.Fail("Install a motherboard before operating its CPU retention lever.");
            if (Has(machine.coolerItemId)) return ActionResult.Fail("Remove the cooler to reach the CPU retention lever.");
            machine.cpuRetentionOpen = !machine.cpuRetentionOpen;
            machine.stressStable = false; machine.benchmarkScore = 0;
            string message = machine.cpuRetentionOpen ? "CPU retention lever opened." : "CPU retention lever locked.";
            machine.history.Add(message);
            return ActionResult.Success(message);
        }

        public static void BreakThermalInterface(MachineState machine)
        {
            machine.thermalPasteApplied = false;
            machine.thermalPasteQuality = 0f;
            machine.stressStable = false;
            machine.benchmarkScore = 0;
        }
    }
}
