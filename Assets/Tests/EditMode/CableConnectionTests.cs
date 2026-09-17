using System.Collections.Generic;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class CableConnectionTests
    {
        private MachineState machine;
        private Dictionary<string, HardwareDefinition> parts;
        private CableConnectionService service;

        [SetUp]
        public void SetUp()
        {
            machine = new MachineState { sidePanelInstalled = false, caseItemId = "case", motherboardItemId = "board", psuItemId = "psu", cpuItemId = "cpu", gpuItemId = "gpu", coolerItemId = "cooler" };
            parts = new Dictionary<string, HardwareDefinition>
            {
                { "case", new HardwareDefinition { category = PartCategory.Case } },
                { "board", new HardwareDefinition { category = PartCategory.Motherboard, sataPorts = 4, fanHeaders = 3, connectors = new List<string> { "ATX24", "EPS8", "CPU_FAN", "SATA", "FRONT_PANEL" } } },
                { "psu", new HardwareDefinition { category = PartCategory.PSU, connectors = new List<string> { "ATX24", "EPS8", "PCIE8", "SATA_POWER" } } },
                { "gpu", new HardwareDefinition { category = PartCategory.GPU, connectors = new List<string> { "PCIE8" } } },
                { "cpu", new HardwareDefinition { category = PartCategory.CPU } },
                { "cooler", new HardwareDefinition { category = PartCategory.Cooler, tags = new List<string> { "air" } } },
                { "sata", new HardwareDefinition { category = PartCategory.Storage, storageInterface = "SATA" } },
                { "nvme", new HardwareDefinition { category = PartCategory.Storage, storageInterface = "NVMe" } }
            };
            service = new CableConnectionService(id => parts.TryGetValue(id, out var definition) ? definition : null);
        }

        [Test]
        public void OneConnectorChangesOnlyItsCircuitAndBlocksRemovalUntilDisconnected()
        {
            Assert.IsTrue(service.SetConnection(machine, CableCircuit.GpuPower, true).ok);
            Assert.IsTrue(machine.cables.gpuPower);
            Assert.IsFalse(machine.cables.atx24);
            Assert.IsFalse(service.CanRemove(machine, parts["gpu"]).ok);
            Assert.IsTrue(service.SetConnection(machine, CableCircuit.GpuPower, false).ok);
            Assert.IsTrue(service.CanRemove(machine, parts["gpu"]).ok);
        }

        [Test]
        public void IncompatibleGpuPowerPlugCannotBeConnected()
        {
            parts["gpu"].connectors = new List<string> { "12V2x6" };
            Assert.IsFalse(service.SetConnection(machine, CableCircuit.GpuPower, true).ok);
            Assert.IsFalse(machine.cables.gpuPower);
        }

        [Test]
        public void UnneededPumpGpuAndSataCircuitsAreNotPresented()
        {
            parts["gpu"].connectors.Clear(); machine.storageItemIds.Add("nvme");
            Assert.IsFalse(service.Present(machine, CableCircuit.GpuPower));
            Assert.IsFalse(service.Present(machine, CableCircuit.Pump));
            Assert.IsFalse(service.Present(machine, CableCircuit.SataPower));
            Assert.IsFalse(service.SetConnection(machine, CableCircuit.SataData, true).ok);
        }

        [Test]
        public void SataRequiresIndependentPowerAndDataDisconnection()
        {
            machine.storageItemIds.Add("sata");
            Assert.IsTrue(service.SetConnection(machine, CableCircuit.SataPower, true).ok);
            Assert.IsTrue(service.SetConnection(machine, CableCircuit.SataData, true).ok);
            service.SetConnection(machine, CableCircuit.SataPower, false);
            Assert.IsFalse(service.CanRemove(machine, parts["sata"]).ok);
            service.SetConnection(machine, CableCircuit.SataData, false);
            Assert.IsTrue(service.CanRemove(machine, parts["sata"]).ok);
        }

        [Test]
        public void PowerAndClosedPanelGuardsDoNotChangeState()
        {
            machine.bootState = BootState.OperatingSystem;
            Assert.IsFalse(service.SetConnection(machine, CableCircuit.Atx24, true).ok);
            machine.bootState = BootState.Off; machine.sidePanelInstalled = true;
            Assert.IsFalse(service.SetConnection(machine, CableCircuit.Atx24, true).ok);
            Assert.IsFalse(machine.cables.atx24);
        }

        [Test]
        public void ReplacementDoesNotInheritPreviousComponentsConnection()
        {
            machine.cables.gpuPower = machine.cables.atx24 = true;
            CableConnectionService.InvalidateConnections(machine, PartCategory.GPU);
            Assert.IsFalse(machine.cables.gpuPower);
            Assert.IsTrue(machine.cables.atx24);
        }

        [Test]
        public void AioRequiresPumpHeaderAndCannotBeRemovedWhileAttached()
        {
            parts["cooler"].tags = new List<string> { "aio" };
            Assert.IsTrue(service.Present(machine, CableCircuit.Pump));
            Assert.IsTrue(service.SetConnection(machine, CableCircuit.Pump, true).ok);
            Assert.IsFalse(service.CanRemove(machine, parts["cooler"]).ok);
            service.SetConnection(machine, CableCircuit.Pump, false);
            parts["board"].fanHeaders = 1;
            Assert.IsFalse(service.SetConnection(machine, CableCircuit.Pump, true).ok);
        }
    }
}
