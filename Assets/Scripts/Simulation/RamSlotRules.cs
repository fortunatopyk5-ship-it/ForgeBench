using System;
using System.Collections.Generic;

namespace ForgeBench
{
    public static class RamSlotRules
    {
        private static readonly int[] FourSlotOrder = { 1, 3, 0, 2, 4, 5, 6, 7 };
        private static readonly int[] SequentialOrder = { 0, 1, 2, 3, 4, 5, 6, 7 };

        public static int SlotCount(HardwareDefinition board)
        {
            int fallback = board != null && board.formFactor == "MiniITX" ? 2 : 4;
            return Math.Max(1, Math.Min(8, board != null && board.dimmSlots > 0 ? board.dimmSlots : fallback));
        }

        public static int NextFree(MachineState machine, HardwareDefinition board)
        {
            int count = SlotCount(board);
            foreach (int slot in count >= 4 ? FourSlotOrder : SequentialOrder)
                if (slot < count && (machine.ramSlotIndices == null || !machine.ramSlotIndices.Contains(slot))) return slot;
            return -1;
        }

        // Repair only missing/invalid mappings. Preserve every valid placement and
        // every item; over-capacity items remain explicitly unplaced (-1).
        public static void Normalize(MachineState machine, HardwareDefinition board)
        {
            if (machine.ramItemIds == null) machine.ramItemIds = new List<string>();
            if (machine.ramSlotIndices == null) machine.ramSlotIndices = new List<int>();
            var indices = machine.ramSlotIndices;
            while (indices.Count > machine.ramItemIds.Count) indices.RemoveAt(indices.Count - 1);
            while (indices.Count < machine.ramItemIds.Count) indices.Add(-1);
            int count = SlotCount(board);
            var used = new HashSet<int>();
            for (int i = 0; i < indices.Count; i++)
                if (indices[i] < 0 || indices[i] >= count || !used.Add(indices[i])) indices[i] = -1;
            for (int i = 0; i < indices.Count; i++)
                if (indices[i] < 0) indices[i] = NextFree(machine, board);
            EnsureLatches(machine, board);
        }

        public static void EnsureLatches(MachineState machine, HardwareDefinition board)
        {
            if (machine.ramLatches == null) machine.ramLatches = new List<RamLatchState>();
            int count = SlotCount(board);
            while (machine.ramLatches.Count > count) machine.ramLatches.RemoveAt(machine.ramLatches.Count - 1);
            while (machine.ramLatches.Count < count) machine.ramLatches.Add(null);
            for (int slot = 0; slot < count; slot++)
                if (machine.ramLatches[slot] == null)
                {
                    bool empty = machine.ramSlotIndices == null || !machine.ramSlotIndices.Contains(slot);
                    machine.ramLatches[slot] = new RamLatchState { topOpen = empty, bottomOpen = empty };
                }
        }

        public static ActionResult ToggleLatch(MachineState machine, HardwareDefinition board, int slot, bool top)
        {
            if (machine == null || board == null) return ActionResult.Fail("No motherboard installed.");
            if (machine.bootState != BootState.Off) return ActionResult.Fail("Power off the PC before operating DIMM latches.");
            if (slot < 0 || slot >= SlotCount(board)) return ActionResult.Fail("Invalid DIMM slot.");
            EnsureLatches(machine, board);
            var latch = machine.ramLatches[slot];
            if (top) latch.topOpen = !latch.topOpen; else latch.bottomOpen = !latch.bottomOpen;
            machine.bios.lastTrainingPassed = false;
            machine.stressStable = false;
            machine.benchmarkScore = 0;
            return ActionResult.Success("DIMM " + (slot + 1) + (top ? " top" : " bottom") + " latch " + ((top ? latch.topOpen : latch.bottomOpen) ? "opened." : "closed."));
        }

        public static bool IsSecured(MachineState machine, HardwareDefinition board)
        {
            if (!IsValid(machine, board)) return false;
            EnsureLatches(machine, board);
            foreach (int slot in machine.ramSlotIndices)
                if (machine.ramLatches[slot].topOpen || machine.ramLatches[slot].bottomOpen) return false;
            return true;
        }

        public static ActionResult CanRemove(MachineState machine, HardwareDefinition board, int itemIndex)
        {
            if (machine.bootState != BootState.Off) return ActionResult.Fail("Power off the PC before removing RAM.");
            if (itemIndex < 0 || itemIndex >= machine.ramSlotIndices.Count) return ActionResult.Success("Unplaced module can be recovered.");
            int slot = machine.ramSlotIndices[itemIndex];
            if (slot < 0 || slot >= SlotCount(board)) return ActionResult.Success("Unplaced module can be recovered.");
            EnsureLatches(machine, board);
            var latch = machine.ramLatches[slot];
            return latch.topOpen && latch.bottomOpen ? ActionResult.Success("RAM released.") : ActionResult.Fail("Open both DIMM latches before removing RAM.");
        }

        public static bool IsValid(MachineState machine, HardwareDefinition board)
        {
            if (machine.ramItemIds == null || machine.ramSlotIndices == null ||
                machine.ramItemIds.Count != machine.ramSlotIndices.Count) return false;
            int count = SlotCount(board);
            var used = new HashSet<int>();
            foreach (int slot in machine.ramSlotIndices)
                if (slot < 0 || slot >= count || !used.Add(slot)) return false;
            return true;
        }

        public static ActionResult Install(MachineState machine, HardwareDefinition board, string instanceId, int requestedSlot = -1)
        {
            if (board == null) return ActionResult.Fail("Install a motherboard before RAM.");
            if (machine == null || string.IsNullOrEmpty(instanceId)) return ActionResult.Fail("No RAM component selected.");
            if (machine.bootState != BootState.Off) return ActionResult.Fail("Power off the PC before installing RAM.");
            if (machine.ramItemIds != null && machine.ramItemIds.Contains(instanceId)) return ActionResult.Fail("This RAM module is already installed.");
            Normalize(machine, board);
            if (machine.ramItemIds.Count >= SlotCount(board)) return ActionResult.Fail("All DIMM slots are occupied.");
            int slot = requestedSlot == -1 ? NextFree(machine, board) : requestedSlot;
            if (slot < 0 || slot >= SlotCount(board)) return ActionResult.Fail("This DIMM slot does not exist on the motherboard.");
            if (machine.ramSlotIndices.Contains(slot)) return ActionResult.Fail("This DIMM slot is already occupied.");
            if (!machine.ramLatches[slot].topOpen || !machine.ramLatches[slot].bottomOpen) return ActionResult.Fail("Open both DIMM latches before inserting RAM.");
            machine.ramItemIds.Add(instanceId);
            machine.ramSlotIndices.Add(slot);
            return ActionResult.Success("RAM installed in DIMM " + (slot + 1) + ".");
        }

        public static bool IsRecommendedPair(MachineState machine, HardwareDefinition board)
        {
            if (!IsValid(machine, board)) return false;
            if (machine.ramItemIds.Count != 2) return true;
            int first = SlotCount(board) >= 4 ? 1 : 0;
            int second = SlotCount(board) >= 4 ? 3 : 1;
            return machine.ramSlotIndices.Contains(first) && machine.ramSlotIndices.Contains(second);
        }
    }
}
