using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace ForgeBench.Tests
{
    public sealed class SaveServiceTests
    {
        private string directory;
        private SaveService saves;
        private string Primary => Path.Combine(directory, "save_1.json");
        private string Backup => Primary + ".bak";

        [SetUp]
        public void SetUp()
        {
            directory = Path.Combine(Path.GetTempPath(), "ForgeBenchSaveTests", Guid.NewGuid().ToString("N"));
            saves = new SaveService(directory);
        }

        [TearDown]
        public void TearDown() { if (Directory.Exists(directory)) Directory.Delete(directory, true); }

        [Test]
        public void RepeatedSave_RoundTripsAndKeepsPreviousGeneration()
        {
            Assert.IsTrue(saves.Save(new GameState { money = 111 }, 1).ok);
            Assert.IsTrue(saves.Save(new GameState { money = 222 }, 1).ok);
            Assert.AreEqual(222, saves.Load(1, out _).money);
            Assert.AreEqual(111, JsonUtility.FromJson<GameState>(File.ReadAllText(Backup)).money);
            Assert.IsFalse(File.Exists(Primary + ".tmp"));
        }

        [Test]
        public void CorruptPrimary_SavePreservesUsableBackup()
        {
            Assert.IsTrue(saves.Save(new GameState { money = 111 }, 1).ok);
            Assert.IsTrue(saves.Save(new GameState { money = 222 }, 1).ok);
            string backup = File.ReadAllText(Backup);
            File.WriteAllText(Primary, "broken JSON");
            Assert.AreEqual(111, saves.Load(1, out _).money);
            Assert.IsTrue(saves.Save(new GameState { money = 333 }, 1).ok);
            Assert.AreEqual(backup, File.ReadAllText(Backup));
            Assert.AreEqual(333, saves.Load(1, out _).money);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void FuturePrimary_DoesNotLoadOlderBackupOrAllowOverwrite(bool hasBackup)
        {
            string future = "{\"schemaVersion\":" + (SaveService.CurrentSchema + 1) + ",\"unknownData\":42}";
            File.WriteAllText(Primary, future);
            if (hasBackup) File.WriteAllText(Backup, JsonUtility.ToJson(new GameState()));
            Assert.IsNull(saves.Load(1, out string message));
            StringAssert.Contains("newer", message);
            Assert.IsFalse(saves.Save(new GameState(), 1).ok);
            Assert.AreEqual(future, File.ReadAllText(Primary));
            Assert.IsTrue(saves.Save(new GameState(), 2).ok);
        }

        [Test]
        public void FutureBackup_IsNeverOverwritten()
        {
            Assert.IsTrue(saves.Save(new GameState(), 1).ok);
            string future = "{\"schemaVersion\":" + (SaveService.CurrentSchema + 1) + "}";
            File.WriteAllText(Backup, future);
            string primary = File.ReadAllText(Primary);
            Assert.IsFalse(saves.Save(new GameState(), 1).ok);
            Assert.AreEqual(primary, File.ReadAllText(Primary));
            Assert.AreEqual(future, File.ReadAllText(Backup));
        }

        [TestCase("{}")]
        [TestCase("{\"schemaVersion\":0}")]
        [TestCase("{\"schemaVersion\":-1}")]
        public void MissingOrInvalidSchema_RecoversBackup(string invalid)
        {
            File.WriteAllText(Backup, JsonUtility.ToJson(new GameState { money = 456 }));
            File.WriteAllText(Primary, invalid);
            Assert.AreEqual(456, saves.Load(1, out string message).money);
            StringAssert.Contains("backup recovered", message);
        }

        [Test]
        public void MigrationFailure_RecoversBackup()
        {
            File.WriteAllText(Backup, JsonUtility.ToJson(new GameState { money = 456 }));
            File.WriteAllText(Primary, "{\"schemaVersion\":7,\"machines\":[null]}");
            Assert.AreEqual(456, saves.Load(1, out _).money);
        }

        [Test]
        public void OlderSchema_MigratesWithoutChangingSourceFile()
        {
            const string legacy = "{\"schemaVersion\":1,\"money\":123,\"machines\":null}";
            File.WriteAllText(Primary, legacy);
            GameState loaded = saves.Load(1, out _);
            Assert.AreEqual(SaveService.CurrentSchema, loaded.schemaVersion);
            Assert.AreEqual(123, loaded.money);
            Assert.IsNotNull(loaded.machines);
            Assert.AreEqual(legacy, File.ReadAllText(Primary));
        }

        [Test]
        public void FailedWrite_DoesNotDeletePrimaryOrBackup()
        {
            Assert.IsTrue(saves.Save(new GameState { money = 111 }, 1).ok);
            Assert.IsTrue(saves.Save(new GameState { money = 222 }, 1).ok);
            string primary = File.ReadAllText(Primary), backup = File.ReadAllText(Backup);
            Directory.CreateDirectory(Primary + ".tmp");
            var oldState = new GameState { schemaVersion = 1 };
            Assert.IsFalse(saves.Save(oldState, 1).ok);
            Assert.AreEqual(1, oldState.schemaVersion);
            Assert.AreEqual(primary, File.ReadAllText(Primary));
            Assert.AreEqual(backup, File.ReadAllText(Backup));
        }

        [Test]
        public void FutureState_IsNotDowngraded()
        {
            var future = new GameState { schemaVersion = SaveService.CurrentSchema + 1 };
            Assert.IsFalse(saves.Save(future, 1).ok);
            Assert.AreEqual(SaveService.CurrentSchema + 1, future.schemaVersion);
            Assert.IsFalse(saves.HasSave(1));
        }

        [TestCase(2, 0, 1)]
        [TestCase(4, 1, 3)]
        public void LegacyRamPositionsUseSavedMotherboardDefinition(int slots, int first, int second)
        {
            var state = new GameState { schemaVersion = 6 };
            state.inventory.Add(new ItemInstance { instanceId = "board-item", definitionId = "test-board" });
            var machine = new MachineState { motherboardItemId = "board-item" };
            machine.ramItemIds.AddRange(new[] { "ram-a", "ram-b" });
            state.machines.Add(machine);
            File.WriteAllText(Primary, JsonUtility.ToJson(state));
            var service = new SaveService(directory, id => id == "test-board" ? new HardwareDefinition { dimmSlots = slots } : null);
            var loaded = service.Load(1, out _).machines[0];
            Assert.AreEqual(first, loaded.ramSlotIndices[0]);
            Assert.AreEqual(second, loaded.ramSlotIndices[1]);
            Assert.IsTrue(service.Save(service.Load(1, out _), 1).ok);
            Assert.AreEqual(second, service.Load(1, out _).machines[0].ramSlotIndices[1]);
        }

        [Test]
        public void LegacyLatchMigrationSecuresInstalledRamButOpensEmptySockets()
        {
            const string legacy = "{\"schemaVersion\":7,\"machines\":[{\"ramItemIds\":[\"a\"],\"ramSlotIndices\":[1]}]}";
            File.WriteAllText(Primary, legacy);
            var loaded = saves.Load(1, out _);
            Assert.AreEqual(8, loaded.schemaVersion);
            Assert.IsFalse(loaded.machines[0].ramLatches[1].topOpen);
            Assert.IsFalse(loaded.machines[0].ramLatches[1].bottomOpen);
            Assert.IsTrue(loaded.machines[0].ramLatches[0].topOpen);
            Assert.AreEqual(legacy, File.ReadAllText(Primary));
        }

        [Test]
        public void PartiallyOpenedLatchSurvivesSaveLoad()
        {
            var state = new GameState(); var machine = new MachineState();
            state.machines.Add(machine);
            machine.ramItemIds.Add("a"); machine.ramSlotIndices.Add(1);
            RamSlotRules.Normalize(machine, null);
            machine.ramLatches[1].topOpen = true;
            Assert.IsTrue(saves.Save(state, 1).ok);
            var latch = saves.Load(1, out _).machines[0].ramLatches[1];
            Assert.IsTrue(latch.topOpen);
            Assert.IsFalse(latch.bottomOpen);
        }
    }
}
