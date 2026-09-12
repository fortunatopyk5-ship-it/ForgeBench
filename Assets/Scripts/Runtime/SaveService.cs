using System;
using System.IO;
using UnityEngine;

namespace ForgeBench
{
    public sealed class SaveService
    {
        public const int CurrentSchema = 7;
        private readonly string root;
        public SaveService()
        {
            root = Path.Combine(Application.persistentDataPath, "ForgeBenchSaves");
            Directory.CreateDirectory(root);
        }

        private string PathFor(int slot) => Path.Combine(root, "save_" + Mathf.Clamp(slot, 1, 3) + ".json");
        private string BackupFor(int slot) => PathFor(slot) + ".bak";

        public ActionResult Save(GameState state, int slot)
        {
            try
            {
                state.schemaVersion = CurrentSchema;
                string json = JsonUtility.ToJson(state, true);
                string target = PathFor(slot);
                string temp = target + ".tmp";
                File.WriteAllText(temp, json);
                if (File.Exists(target)) File.Copy(target, BackupFor(slot), true);
                if (File.Exists(target)) File.Delete(target);
                File.Move(temp, target);
                return ActionResult.Success("Saved to slot " + slot + ".");
            }
            catch (Exception ex) { return ActionResult.Fail("Save failed: " + ex.Message); }
        }

        public GameState Load(int slot, out string message)
        {
            string path = PathFor(slot);
            GameState result = TryLoad(path, out message);
            if (result != null) return Migrate(result);
            string backup = BackupFor(slot);
            if (File.Exists(backup))
            {
                result = TryLoad(backup, out message);
                if (result != null) { message = "Primary save was invalid; backup recovered."; return Migrate(result); }
            }
            return null;
        }

        private static GameState TryLoad(string path, out string message)
        {
            message = "Save not found.";
            if (!File.Exists(path)) return null;
            try
            {
                string json = File.ReadAllText(path);
                GameState state = JsonUtility.FromJson<GameState>(json);
                if (state == null || state.schemaVersion <= 0) { message = "Invalid save schema."; return null; }
                message = "Loaded save.";
                return state;
            }
            catch (Exception ex) { message = "Load failed: " + ex.Message; return null; }
        }

        private static GameState Migrate(GameState s)
        {
            if (s.workshop == null) s.workshop = new WorkshopState();
            if (s.settings == null) s.settings = new AccessibilitySettings();
            if (s.inventory == null) s.inventory = new System.Collections.Generic.List<ItemInstance>();
            if (s.machines == null) s.machines = new System.Collections.Generic.List<MachineState>();
            if (s.jobs == null) s.jobs = new System.Collections.Generic.List<JobState>();
            if (s.shipments == null) s.shipments = new System.Collections.Generic.List<ShipmentState>();
            if (s.ledger == null) s.ledger = new System.Collections.Generic.List<LedgerEntry>();
            if (s.unlockedPartIds == null) s.unlockedPartIds = new System.Collections.Generic.List<string>();
            if (s.milestones == null) s.milestones = new System.Collections.Generic.List<string>();
            foreach (MachineState m in s.machines)
            {
                if (m.ramItemIds == null) m.ramItemIds = new System.Collections.Generic.List<string>();
                if (m.ramSlotIndices == null) m.ramSlotIndices = new System.Collections.Generic.List<int>();
                while (m.ramSlotIndices.Count < m.ramItemIds.Count) m.ramSlotIndices.Add(DefaultRamSlot(m.ramSlotIndices.Count, m.ramItemIds.Count));
                while (m.ramSlotIndices.Count > m.ramItemIds.Count) m.ramSlotIndices.RemoveAt(m.ramSlotIndices.Count - 1);
                if (m.storageItemIds == null) m.storageItemIds = new System.Collections.Generic.List<string>();
                if (m.fanItemIds == null) m.fanItemIds = new System.Collections.Generic.List<string>();
                if (m.cables == null) m.cables = new CableState();
                if (m.bios == null) m.bios = new BiosState();
                if (m.sidePanel == null) m.sidePanel = new PanelState { installed = m.sidePanelInstalled };
                if (m.sidePanel.fasteners == null) m.sidePanel.fasteners = new System.Collections.Generic.List<FastenerState>();
                if (m.customization == null) m.customization = new CustomizationState();
                if (m.liquidLoop == null) m.liquidLoop = new LiquidLoopState();
                if (m.liquidLoop.history == null) m.liquidLoop.history = new System.Collections.Generic.List<string>();
                if (m.boardRepair == null) m.boardRepair = new BoardRepairState();
                if (m.boardRepair.history == null) m.boardRepair.history = new System.Collections.Generic.List<string>();
                if (m.portable == null) m.portable = new PortableDeviceState();
                if (m.portable.history == null) m.portable.history = new System.Collections.Generic.List<string>();
                if (m.network == null) m.network = new NetworkLabState();
                if (m.network.history == null) m.network.history = new System.Collections.Generic.List<string>();
                if (m.osState == null) m.osState = new OsRuntimeState();
                if (m.osState.history == null) m.osState.history = new System.Collections.Generic.List<string>();
                if (m.benchmarkState == null) m.benchmarkState = new BenchmarkRunState();
                if (m.benchmarkState.history == null) m.benchmarkState.history = new System.Collections.Generic.List<string>();
                if (m.thermalState == null) m.thermalState = new ThermalRuntimeState();
                if (m.thermalState.history == null) m.thermalState.history = new System.Collections.Generic.List<string>();
                if (m.powerState == null) m.powerState = new PowerRuntimeState();
                if (m.powerState.history == null) m.powerState.history = new System.Collections.Generic.List<string>();
                if (m.maintenance == null) m.maintenance = new MaintenanceState();
                if (m.maintenance.history == null) m.maintenance.history = new System.Collections.Generic.List<string>();
                if (m.history == null) m.history = new System.Collections.Generic.List<string>();
            }
            foreach (JobState j in s.jobs)
            {
                if (j.requiredPartCategories == null) j.requiredPartCategories = new System.Collections.Generic.List<string>();
                if (j.optionalObjectives == null) j.optionalObjectives = new System.Collections.Generic.List<string>();
            }
            s.schemaVersion = CurrentSchema;
            return s;
        }

        private static int DefaultRamSlot(int index, int total)
        {
            if (total <= 1) return 1;
            if (total == 2) return index == 0 ? 1 : 3;
            return Mathf.Clamp(index, 0, 3);
        }

        public bool HasSave(int slot) => File.Exists(PathFor(slot)) || File.Exists(BackupFor(slot));
    }
}
