using System;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Pure presentation rules shared by the procedural hardware renderer and tests.
    /// Gameplay state remains authoritative; this class only decides how that state is shown.
    /// </summary>
    public static class HardwarePresentationLayout
    {
        public static int DimmSlotCount(HardwareDefinition board)
        {
            return RamSlotRules.SlotCount(board);
        }

        public static int PcieSlotCount(HardwareDefinition board)
        {
            return Mathf.Clamp(board != null && board.pcieX16Slots > 0 ? board.pcieX16Slots : 1, 1, 4);
        }

        public static int M2SlotCount(HardwareDefinition board)
        {
            return Mathf.Clamp(board != null && board.m2Slots > 0 ? board.m2Slots : 1, 1, 5);
        }

        public static int RamItemIndexAtSlot(MachineState machine, int slot, int visibleSlots)
        {
            if (machine == null || machine.ramItemIds == null || slot < 0 || slot >= visibleSlots) return -1;
            if (machine.ramSlotIndices != null && machine.ramSlotIndices.Count > 0)
            {
                int count = Mathf.Min(machine.ramItemIds.Count, machine.ramSlotIndices.Count);
                for (int i = 0; i < count; i++)
                    if (machine.ramSlotIndices[i] == slot) return i;
                return -1;
            }
            // Pre-slot-metadata state only: deterministic sequential fallback avoids hiding RAM.
            return slot < machine.ramItemIds.Count ? slot : -1;
        }

        public static int NextRecommendedRamSlot(MachineState machine, HardwareDefinition board)
        {
            int slots = DimmSlotCount(board);
            int[] preferred = slots >= 4
                ? new[] { 1, 3, 0, 2, 4, 5, 6, 7 }
                : new[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            foreach (int slot in preferred)
            {
                if (slot >= slots) continue;
                if (RamItemIndexAtSlot(machine, slot, slots) < 0) return slot;
            }
            return -1;
        }

        public static int GpuFanCount(HardwareDefinition gpu)
        {
            if (gpu == null) return 0;
            if (gpu.lengthMm >= 285f || gpu.powerWatts >= 245f) return 3;
            if (gpu.lengthMm >= 205f || gpu.powerWatts >= 115f) return 2;
            return 1;
        }

        public static float GpuVisualLength(HardwareDefinition gpu)
        {
            if (gpu == null) return .54f;
            return Mathf.Lerp(.42f, .69f, Mathf.InverseLerp(165f, 360f, Mathf.Max(165f, gpu.lengthMm)));
        }

        public static bool IsAio(HardwareDefinition cooler)
        {
            return cooler != null && cooler.tags != null && cooler.tags.Contains("aio");
        }

        public static int AioFanCount(HardwareDefinition cooler)
        {
            if (!IsAio(cooler)) return 0;
            int radiator = cooler.radiatorSupportMm;
            if (radiator >= 360) return 3;
            if (radiator >= 240) return 2;
            return 1;
        }

        public static bool IsNvme(HardwareDefinition storage)
        {
            return storage != null && string.Equals(storage.storageInterface, "NVMe", StringComparison.OrdinalIgnoreCase);
        }

        public static int CaseFanVisualSlots(MachineState machine)
        {
            int installed = machine?.fanItemIds?.Count ?? 0;
            return Mathf.Clamp(Mathf.Max(3, installed), 3, 6);
        }

        public static float FanVisualDiameter(HardwareDefinition fan)
        {
            int mm = fan != null && fan.fanSizeMm > 0 ? fan.fanSizeMm : 120;
            return Mathf.Clamp(.22f * mm / 120f, .18f, .27f);
        }
    }
}
