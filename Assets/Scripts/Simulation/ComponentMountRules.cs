using System;
using System.Collections.Generic;

namespace ForgeBench
{
    public static class ComponentMountRules
    {
        public static string Key(PartCategory category)
        {
            switch (category)
            {
                case PartCategory.Motherboard: return "motherboard";
                case PartCategory.PSU: return "psu";
                case PartCategory.GPU: return "gpu";
                case PartCategory.Cooler: return "cooler";
                default: return null;
            }
        }

        public static int Count(PartCategory category, HardwareDefinition part)
        {
            if (category == PartCategory.Motherboard) return part?.formFactor == "MiniITX" ? 4 : part?.formFactor == "mATX" ? 6 : 9;
            return category == PartCategory.GPU ? 2 : 4;
        }

        public static ComponentMountState Find(MachineState machine, PartCategory category)
        {
            string key = Key(category);
            return key == null ? null : machine.componentMounts?.Find(m => m != null && m.mountId == key);
        }

        public static ComponentMountState Ensure(MachineState machine, PartCategory category, HardwareDefinition part, bool legacySecured = false)
        {
            string key = Key(category); if (key == null) return null;
            if (machine.componentMounts == null) machine.componentMounts = new List<ComponentMountState>();
            ComponentMountState mount = Find(machine, category);
            if (mount == null) { mount = new ComponentMountState { mountId = key }; machine.componentMounts.Add(mount); }
            if (mount.fasteners == null) mount.fasteners = new List<FastenerState>();
            int count = Count(category, part);
            while (mount.fasteners.Count > count) mount.fasteners.RemoveAt(mount.fasteners.Count - 1);
            while (mount.fasteners.Count < count) mount.fasteners.Add(null);
            for (int i = 0; i < count; i++)
                if (mount.fasteners[i] == null) mount.fasteners[i] = new FastenerState { fastenerId = key + ":" + i, tightness = legacySecured ? 1f : 0f };
            return mount;
        }

        public static ActionResult CanRemove(MachineState machine, PartCategory category)
        {
            ComponentMountState mount = Find(machine, category);
            if (mount != null && mount.fasteners != null)
                foreach (FastenerState screw in mount.fasteners)
                    if (screw != null && (screw.damaged || screw.tightness > .05f)) return ActionResult.Fail("Release mount fastener " + screw.fastenerId + " before removing the component.");
            return ActionResult.Success("Mount released.");
        }

        public static void EnsureInstalled(MachineState machine, Func<string, HardwareDefinition> definition, bool secured = false)
        {
            var categories = new[] { PartCategory.Motherboard, PartCategory.PSU, PartCategory.GPU, PartCategory.Cooler };
            var ids = new[] { machine.motherboardItemId, machine.psuItemId, machine.gpuItemId, machine.coolerItemId };
            for (int i = 0; i < ids.Length; i++)
                if (!string.IsNullOrEmpty(ids[i])) Ensure(machine, categories[i], definition == null ? null : definition(ids[i]), secured);
        }

        public static string UnsecuredMount(MachineState machine)
        {
            var categories = new[] { PartCategory.Motherboard, PartCategory.PSU, PartCategory.GPU, PartCategory.Cooler };
            var ids = new[] { machine.motherboardItemId, machine.psuItemId, machine.gpuItemId, machine.coolerItemId };
            for (int i = 0; i < ids.Length; i++)
                if (!string.IsNullOrEmpty(ids[i]) && !Secured(machine, categories[i])) return Key(categories[i]);
            return null;
        }

        public static bool Secured(MachineState machine, PartCategory category)
        {
            var mount = Find(machine, category);
            if (mount == null || mount.fasteners == null || mount.fasteners.Count == 0) return false;
            foreach (var screw in mount.fasteners)
                if (screw == null || screw.damaged || screw.tightness < .95f) return false;
            return true;
        }

        public static ActionResult Turn(MachineState machine, PartCategory category, int index, bool tighten, bool hasDriver)
        {
            if (machine == null) return ActionResult.Fail("No device on bench.");
            if (machine.bootState != BootState.Off) return ActionResult.Fail("Power off the PC before working on mount fasteners.");
            if (machine.sidePanelInstalled && !string.IsNullOrEmpty(machine.caseItemId)) return ActionResult.Fail("Remove the side panel to access mounting screws.");
            if (!hasDriver) return ActionResult.Fail("A compatible screwdriver is required.");
            var mount = Find(machine, category);
            if (mount?.fasteners == null || index < 0 || index >= mount.fasteners.Count) return ActionResult.Fail("Mount fastener is unavailable.");
            var screw = mount.fasteners[index];
            if (screw == null || screw.damaged) return ActionResult.Fail("Mount fastener requires repair.");
            screw.tightness = Math.Max(0f, Math.Min(1f, screw.tightness + (tighten ? .25f : -.25f)));
            machine.stressStable = false; machine.benchmarkScore = 0;
            string message = screw.fastenerId + " tightened to " + (int)Math.Round(screw.tightness * 100) + "%.";
            machine.history.Add(message);
            return ActionResult.Success(message);
        }
    }
}
