using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class MechanicalAssemblyTests
    {
        [Test]
        public void MountRequiresEveryScrewAndReleasesBeforeRemoval()
        {
            var m = OpenPc();
            var mount = ComponentMountRules.Ensure(m, PartCategory.Motherboard, null);
            Assert.IsFalse(ComponentMountRules.Secured(m, PartCategory.Motherboard));
            for (int i = 0; i < mount.fasteners.Count; i++)
                for (int turn = 0; turn < 4; turn++) Assert.IsTrue(ComponentMountRules.Turn(m, PartCategory.Motherboard, i, true, true).ok);
            Assert.IsTrue(ComponentMountRules.Secured(m, PartCategory.Motherboard));
            Assert.IsFalse(ComponentMountRules.CanRemove(m, PartCategory.Motherboard).ok);
            for (int i = 0; i < mount.fasteners.Count; i++)
                for (int turn = 0; turn < 4; turn++) ComponentMountRules.Turn(m, PartCategory.Motherboard, i, false, true);
            Assert.IsTrue(ComponentMountRules.CanRemove(m, PartCategory.Motherboard).ok);
            Assert.AreEqual(mount, ComponentMountRules.Ensure(m, PartCategory.Motherboard, null));
        }

        [Test]
        public void MountAccessAndDamagedScrewRejectWithoutChangingTightness()
        {
            var m = OpenPc(); var mount = ComponentMountRules.Ensure(m, PartCategory.Motherboard, null);
            Assert.IsFalse(ComponentMountRules.Turn(m, PartCategory.Motherboard, 0, true, false).ok);
            m.sidePanelInstalled = true;
            Assert.IsFalse(ComponentMountRules.Turn(m, PartCategory.Motherboard, 0, true, true).ok);
            m.sidePanelInstalled = false; m.bootState = BootState.Posting;
            Assert.IsFalse(ComponentMountRules.Turn(m, PartCategory.Motherboard, 0, true, true).ok);
            m.bootState = BootState.Off; mount.fasteners[0].damaged = true;
            Assert.IsFalse(ComponentMountRules.Turn(m, PartCategory.Motherboard, 0, true, true).ok);
            Assert.AreEqual(0f, mount.fasteners[0].tightness);
        }

        private static MachineState OpenPc() => new MachineState { motherboardItemId = "board", cpuItemId = "cpu", caseItemId = "case", sidePanelInstalled = false };

        [Test]
        public void AssistedMountControlDistributesTurnsAndStopsAtLimit()
        {
            var m = OpenPc();
            var mount = ComponentMountRules.Ensure(m, PartCategory.Motherboard, new HardwareDefinition { formFactor = "MiniITX" });
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(i, ComponentMountRules.NextFastener(m, PartCategory.Motherboard, true));
                Assert.IsTrue(ComponentMountRules.Turn(m, PartCategory.Motherboard, i, true, true).ok);
            }
            foreach (var screw in mount.fasteners) Assert.AreEqual(.25f, screw.tightness);
            for (int i = 0; i < 12; i++)
                Assert.IsTrue(ComponentMountRules.Turn(m, PartCategory.Motherboard, ComponentMountRules.NextFastener(m, PartCategory.Motherboard, true), true, true).ok);
            Assert.AreEqual(-1, ComponentMountRules.NextFastener(m, PartCategory.Motherboard, true));
            m.stressStable = true; m.benchmarkScore = 123f;
            Assert.IsFalse(ComponentMountRules.Turn(m, PartCategory.Motherboard, 0, true, true).ok);
            Assert.AreEqual(1f, mount.fasteners[0].tightness);
            Assert.IsTrue(m.stressStable); Assert.AreEqual(123f, m.benchmarkScore);
        }

        [Test]
        public void EmptyMountCannotBeTightenedBeforeInstallingReplacement()
        {
            var m = OpenPc();
            var mount = ComponentMountRules.Ensure(m, PartCategory.Motherboard, null);
            m.motherboardItemId = null;
            Assert.AreEqual(-1, ComponentMountRules.NextFastener(m, PartCategory.Motherboard, true));
            Assert.IsFalse(ComponentMountRules.Turn(m, PartCategory.Motherboard, 0, true, true).ok);
            Assert.AreEqual(0f, mount.fasteners[0].tightness);
            m.motherboardItemId = "replacement";
            ComponentMountRules.Ensure(m, PartCategory.Motherboard, null);
            Assert.IsFalse(ComponentMountRules.Secured(m, PartCategory.Motherboard));
            Assert.IsTrue(ComponentMountRules.Turn(m, PartCategory.Motherboard, 0, true, true).ok);
        }

        [Test]
        public void CoolerBlocksProcessorRemovalReplacementAndPasteAccess()
        {
            var machine = OpenPc(); machine.coolerItemId = "cooler";
            Assert.IsFalse(MechanicalAssemblyRules.CanRemove(machine, PartCategory.CPU, null).ok);
            Assert.IsFalse(MechanicalAssemblyRules.CanInstall(machine, PartCategory.CPU).ok);
            Assert.IsFalse(MechanicalAssemblyRules.CanApplyPaste(machine).ok);
        }

        [Test]
        public void CoolerNeedsCpuAndFreshThermalInterface()
        {
            var machine = OpenPc();
            Assert.IsFalse(MechanicalAssemblyRules.CanInstall(machine, PartCategory.Cooler).ok);
            Assert.IsTrue(MechanicalAssemblyRules.CanApplyPaste(machine).ok);
            machine.thermalPasteApplied = true; machine.thermalPasteQuality = .9f;
            Assert.IsTrue(MechanicalAssemblyRules.CanInstall(machine, PartCategory.Cooler).ok);
            machine.cpuItemId = null;
            Assert.IsFalse(MechanicalAssemblyRules.CanInstall(machine, PartCategory.Cooler).ok);
        }

        [Test]
        public void BrokenThermalInterfaceInvalidatesPreviousAcceptance()
        {
            var machine = OpenPc(); machine.thermalPasteApplied = true; machine.thermalPasteQuality = .9f;
            machine.stressStable = true; machine.benchmarkScore = 1000;
            MechanicalAssemblyRules.BreakThermalInterface(machine);
            Assert.IsFalse(machine.thermalPasteApplied); Assert.AreEqual(0f, machine.thermalPasteQuality);
            Assert.IsFalse(machine.stressStable); Assert.AreEqual(0f, machine.benchmarkScore);
            Assert.IsFalse(MechanicalAssemblyRules.CanInstall(machine, PartCategory.Cooler).ok);
        }

        [Test]
        public void MotherboardRemovalRequiresMountedComponentsAndNvmeRemoved()
        {
            var machine = OpenPc();
            Assert.IsFalse(MechanicalAssemblyRules.CanRemove(machine, PartCategory.Motherboard, null).ok);
            machine.cpuItemId = null; machine.storageItemIds.Add("drive");
            Assert.IsFalse(MechanicalAssemblyRules.CanRemove(machine, PartCategory.Motherboard, id => new HardwareDefinition { storageInterface = "NVMe" }).ok);
            Assert.IsTrue(MechanicalAssemblyRules.CanRemove(machine, PartCategory.Motherboard, id => new HardwareDefinition { storageInterface = "SATA" }).ok);
        }

        [Test]
        public void ChassisCannotBeReplacedWhilePowerSupplyOrDrivesRemain()
        {
            var machine = new MachineState { psuItemId = "psu", sidePanelInstalled = false };
            Assert.IsFalse(MechanicalAssemblyRules.CanInstall(machine, PartCategory.Case).ok);
            machine.psuItemId = null; machine.storageItemIds.Add("drive");
            Assert.IsFalse(MechanicalAssemblyRules.CanRemove(machine, PartCategory.Case, null).ok);
            machine.storageItemIds.Clear();
            Assert.IsTrue(MechanicalAssemblyRules.CanInstall(machine, PartCategory.Case).ok);
        }

        [Test]
        public void ClosedPanelAndPoweredMachineBlockPaste()
        {
            var machine = OpenPc(); machine.sidePanelInstalled = true;
            Assert.IsFalse(MechanicalAssemblyRules.CanApplyPaste(machine).ok);
            machine.sidePanelInstalled = false; machine.bootState = BootState.OperatingSystem;
            Assert.IsFalse(MechanicalAssemblyRules.CanApplyPaste(machine).ok);
        }

        [Test]
        public void CpuRetentionControlsInsertionRemovalAndCoolerSequence()
        {
            var machine = OpenPc(); machine.cpuItemId = null;
            Assert.IsFalse(MechanicalAssemblyRules.CanInstall(machine, PartCategory.CPU).ok);
            Assert.IsTrue(MechanicalAssemblyRules.ToggleCpuRetention(machine).ok);
            Assert.IsTrue(MechanicalAssemblyRules.CanInstall(machine, PartCategory.CPU).ok);
            machine.cpuItemId = "cpu";
            Assert.IsTrue(MechanicalAssemblyRules.CanRemove(machine, PartCategory.CPU, null).ok);
            Assert.IsFalse(MechanicalAssemblyRules.CanApplyPaste(machine).ok);
            machine.thermalPasteApplied = true; machine.thermalPasteQuality = .9f;
            Assert.IsFalse(MechanicalAssemblyRules.CanInstall(machine, PartCategory.Cooler).ok);
            Assert.IsTrue(MechanicalAssemblyRules.ToggleCpuRetention(machine).ok);
            Assert.IsFalse(MechanicalAssemblyRules.CanRemove(machine, PartCategory.CPU, null).ok);
            Assert.IsTrue(MechanicalAssemblyRules.CanApplyPaste(machine).ok);
            Assert.IsTrue(MechanicalAssemblyRules.CanInstall(machine, PartCategory.Cooler).ok);
        }

        [Test]
        public void CpuRetentionCannotOperateThroughCoolerOrUnderPower()
        {
            var machine = OpenPc(); machine.coolerItemId = "cooler";
            Assert.IsFalse(MechanicalAssemblyRules.ToggleCpuRetention(machine).ok);
            machine.coolerItemId = null; machine.bootState = BootState.OperatingSystem;
            Assert.IsFalse(MechanicalAssemblyRules.ToggleCpuRetention(machine).ok);
            Assert.IsFalse(machine.cpuRetentionOpen);
        }
    }
}
