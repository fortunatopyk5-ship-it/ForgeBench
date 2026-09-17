using System;
using System.IO;
using UnityEngine;

namespace ForgeBench
{
    public sealed class SaveService
    {
        public const int CurrentSchema = 9;
        private readonly string root;
        private readonly Func<string, HardwareDefinition> definition;
        [Serializable]
        private sealed class SaveHeader { public int schemaVersion; }

        public SaveService(Func<string, HardwareDefinition> definition = null)
            : this(Path.Combine(Application.persistentDataPath, "ForgeBenchSaves"), definition) { }

        public SaveService(string saveDirectory, Func<string, HardwareDefinition> definition = null)
        {
            this.definition = definition;
            root = saveDirectory ?? throw new ArgumentNullException(nameof(saveDirectory));
            Directory.CreateDirectory(root);
        }

        private string PathFor(int slot) => Path.Combine(root, "save_" + Mathf.Clamp(slot, 1, 3) + ".json");
        private string BackupFor(int slot) => PathFor(slot) + ".bak";

        public ActionResult Save(GameState state, int slot)
        {
            if (state == null) return ActionResult.Fail("No game state to save.");
            if (state.schemaVersion > CurrentSchema) return ActionResult.Fail("Cannot save state from a newer game version.");
            int previousSchema = state.schemaVersion;
            try
            {
                string target = PathFor(slot);
                string backup = BackupFor(slot);
                // Check both files on every write, including startup/autosave after
                // a failed load. Never destroy data created by a newer game.
                string ignored;
                bool futurePrimary, futureBackup;
                GameState primary = TryLoad(target, out ignored, out futurePrimary);
                TryLoad(backup, out ignored, out futureBackup);
                if (futurePrimary || futureBackup)
                    return ActionResult.Fail("This slot contains a save from a newer game version. Use another slot.");
                state.schemaVersion = CurrentSchema;
                string json = JsonUtility.ToJson(state, true);
                string temp = target + ".tmp";
                File.WriteAllText(temp, json);
                if (File.Exists(target))
                    // Replacing the target avoids a delete/move gap. A corrupt
                    // primary must not overwrite the last usable backup.
                    File.Replace(temp, target, primary != null ? backup : null);
                else File.Move(temp, target);
                return ActionResult.Success("Saved to slot " + slot + ".");
            }
            catch (Exception ex)
            {
                state.schemaVersion = previousSchema;
                return ActionResult.Fail("Save failed: " + ex.Message);
            }
        }

        public GameState Load(int slot, out string message)
        {
            string path = PathFor(slot);
            bool futureSchema;
            GameState result = TryLoad(path, out message, out futureSchema);
            if (result != null || futureSchema) return result;
            string backup = BackupFor(slot);
            if (File.Exists(backup))
            {
                result = TryLoad(backup, out message, out futureSchema);
                if (result != null) { message = "Primary save was invalid; backup recovered."; return result; }
            }
            return null;
        }

        private GameState TryLoad(string path, out string message, out bool futureSchema)
        {
            futureSchema = false;
            message = "Save not found.";
            if (!File.Exists(path)) return null;
            try
            {
                string json = File.ReadAllText(path);
                SaveHeader header = JsonUtility.FromJson<SaveHeader>(json);
                if (header == null || header.schemaVersion <= 0) { message = "Invalid save schema."; return null; }
                if (header.schemaVersion > CurrentSchema)
                {
                    futureSchema = true;
                    message = "This save requires a newer game version. Its files were left unchanged.";
                    return null;
                }
                GameState state = JsonUtility.FromJson<GameState>(json);
                if (state == null || state.schemaVersion <= 0) { message = "Invalid save schema."; return null; }
                message = "Loaded save.";
                // Migration failures are load failures too, so backup recovery
                // remains available rather than throwing out of Load().
                return Migrate(state);
            }
            catch (Exception ex) { message = "Load failed: " + ex.Message; return null; }
        }

        private GameState Migrate(GameState s)
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
                if (s.schemaVersion < 9) m.cpuRetentionOpen = string.IsNullOrEmpty(m.cpuItemId);
                if (m.ramItemIds == null) m.ramItemIds = new System.Collections.Generic.List<string>();
                ItemInstance boardItem = s.inventory.Find(item => item != null && item.instanceId == m.motherboardItemId);
                RamSlotRules.Normalize(m, boardItem != null && definition != null ? definition(boardItem.definitionId) : null);
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

        public bool HasSave(int slot) => File.Exists(PathFor(slot)) || File.Exists(BackupFor(slot));
    }
}
