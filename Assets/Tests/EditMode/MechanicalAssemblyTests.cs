using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class MechanicalAssemblyTests
    {
        private static MachineState OpenPc() => new MachineState { motherboardItemId = "board", cpuItemId = "cpu", caseItemId = "case", sidePanelInstalled = false };

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
