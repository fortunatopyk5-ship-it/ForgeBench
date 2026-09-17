using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class RamSlotRulesTests
    {
        private static HardwareDefinition Board(int slots) => new HardwareDefinition { dimmSlots = slots };

        [TestCase(2, 0, 1)]
        [TestCase(4, 1, 3)]
        [TestCase(8, 1, 3)]
        public void RecommendedPairUsesBoardTopology(int count, int first, int second)
        {
            var machine = new MachineState(); var board = Board(count);
            Assert.IsTrue(RamSlotRules.Install(machine, board, "a").ok);
            Assert.IsTrue(RamSlotRules.Install(machine, board, "b").ok);
            Assert.AreEqual(first, machine.ramSlotIndices[0]);
            Assert.AreEqual(second, machine.ramSlotIndices[1]);
            Assert.IsTrue(RamSlotRules.IsRecommendedPair(machine, board));
        }

        [Test]
        public void PlayerCanChooseNonRecommendedSlotWithoutSilentRepositioning()
        {
            var machine = new MachineState(); var board = Board(4);
            Assert.IsTrue(RamSlotRules.Install(machine, board, "a", 0).ok);
            Assert.IsTrue(RamSlotRules.Install(machine, board, "b", 1).ok);
            RamSlotRules.Normalize(machine, board);
            Assert.AreEqual(0, machine.ramSlotIndices[0]);
            Assert.IsTrue(RamSlotRules.IsValid(machine, board));
            Assert.IsFalse(RamSlotRules.IsRecommendedPair(machine, board));
        }

        [Test]
        public void OccupiedOutOfRangeAndDuplicateInstallationsDoNotAddItems()
        {
            var machine = new MachineState(); var board = Board(2);
            Assert.IsTrue(RamSlotRules.Install(machine, board, "a", 1).ok);
            Assert.IsFalse(RamSlotRules.Install(machine, board, "b", 1).ok);
            Assert.IsFalse(RamSlotRules.Install(machine, board, "b", 3).ok);
            Assert.IsFalse(RamSlotRules.Install(machine, board, "a", 0).ok);
            Assert.AreEqual(1, machine.ramItemIds.Count);
        }

        [Test]
        public void RepairReservesValidLaterSlotsBeforeFillingMissingMappings()
        {
            var machine = new MachineState();
            machine.ramItemIds.AddRange(new[] { "a", "b" });
            machine.ramSlotIndices.AddRange(new[] { -1, 1 });
            RamSlotRules.Normalize(machine, Board(4));
            Assert.AreEqual(3, machine.ramSlotIndices[0]);
            Assert.AreEqual(1, machine.ramSlotIndices[1]);
        }

        [Test]
        public void OverCapacityRecoveryPreservesItemsAndRejectsLayout()
        {
            var machine = new MachineState();
            machine.ramItemIds.AddRange(new[] { "a", "b", "c" });
            machine.ramSlotIndices.AddRange(new[] { 1, 1, 7 });
            RamSlotRules.Normalize(machine, Board(2));
            Assert.AreEqual(3, machine.ramItemIds.Count);
            Assert.AreEqual(-1, machine.ramSlotIndices[2]);
            Assert.IsFalse(RamSlotRules.IsValid(machine, Board(2)));
            Assert.IsFalse(RamSlotRules.Install(machine, Board(2), "d").ok);
        }

        [Test]
        public void MissingMotherboardDoesNotInstallRam()
        {
            var machine = new MachineState();
            Assert.IsFalse(RamSlotRules.Install(machine, null, "a").ok);
            Assert.AreEqual(0, machine.ramItemIds.Count);
        }

        [Test]
        public void InstalledRamRequiresBothClipsClosedAndBothOpenForRemoval()
        {
            var machine = new MachineState(); var board = Board(2);
            Assert.IsTrue(RamSlotRules.Install(machine, board, "a", 0).ok);
            Assert.IsFalse(RamSlotRules.IsSecured(machine, board));
            Assert.IsTrue(RamSlotRules.ToggleLatch(machine, board, 0, true).ok);
            Assert.IsFalse(RamSlotRules.IsSecured(machine, board));
            Assert.IsFalse(RamSlotRules.CanRemove(machine, board, 0).ok);
            Assert.IsTrue(RamSlotRules.ToggleLatch(machine, board, 0, false).ok);
            Assert.IsTrue(RamSlotRules.IsSecured(machine, board));
            Assert.IsFalse(RamSlotRules.CanRemove(machine, board, 0).ok);
            RamSlotRules.ToggleLatch(machine, board, 0, true);
            RamSlotRules.ToggleLatch(machine, board, 0, false);
            Assert.IsTrue(RamSlotRules.CanRemove(machine, board, 0).ok);
        }

        [Test]
        public void ClosedEmptySocketRejectsInsertionAndPoweredMachineRejectsLatchChanges()
        {
            var machine = new MachineState(); var board = Board(2);
            RamSlotRules.ToggleLatch(machine, board, 0, true);
            Assert.IsFalse(RamSlotRules.Install(machine, board, "a", 0).ok);
            machine.bootState = BootState.OperatingSystem;
            Assert.IsFalse(RamSlotRules.ToggleLatch(machine, board, 0, true).ok);
            Assert.IsFalse(RamSlotRules.Install(machine, board, "a", 1).ok);
            Assert.IsFalse(machine.ramLatches[0].topOpen);
        }

        [Test]
        public void LatchesBelongToSocketAfterModuleRemoval()
        {
            var machine = new MachineState(); var board = Board(2);
            RamSlotRules.Install(machine, board, "a", 0);
            var hardware = machine.ramLatches[0];
            machine.ramItemIds.Clear(); machine.ramSlotIndices.Clear();
            RamSlotRules.Normalize(machine, board);
            Assert.AreEqual(hardware, machine.ramLatches[0]);
            Assert.IsTrue(RamSlotRules.Install(machine, board, "b", 0).ok);
        }
    }
}
