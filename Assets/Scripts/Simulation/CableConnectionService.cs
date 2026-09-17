using System;
using System.Linq;

namespace ForgeBench
{
    public enum CableCircuit { Atx24, CpuEps, GpuPower, SataPower, SataData, FrontPanel, CpuFan, Pump, Rgb }

    /// <summary>Physical cable actions update the existing authoritative CableState.</summary>
    public sealed class CableConnectionService
    {
        private readonly Func<string, HardwareDefinition> definition;
        public CableConnectionService(Func<string, HardwareDefinition> definition) { this.definition = definition; }
        private HardwareDefinition D(string id) => string.IsNullOrEmpty(id) ? null : definition(id);
        private static bool Has(HardwareDefinition part, string connector) => part?.connectors != null && part.connectors.Contains(connector);
        private static bool Tagged(HardwareDefinition part, string tag) => part?.tags != null && part.tags.Contains(tag);
        private bool HasSata(MachineState m) => m.storageItemIds.Any(id => D(id)?.storageInterface == "SATA");

        public static string Label(CableCircuit circuit)
        {
            switch (circuit)
            {
                case CableCircuit.Atx24: return "ATX 24-pin";
                case CableCircuit.CpuEps: return "CPU EPS";
                case CableCircuit.GpuPower: return "GPU auxiliary power";
                case CableCircuit.SataPower: return "SATA power";
                case CableCircuit.SataData: return "SATA data";
                case CableCircuit.FrontPanel: return "Front panel";
                case CableCircuit.CpuFan: return "CPU_FAN";
                case CableCircuit.Pump: return "AIO pump";
                default: return "RGB";
            }
        }

        public bool Present(MachineState m, CableCircuit circuit)
        {
            if (m == null) return false;
            switch (circuit)
            {
                case CableCircuit.Atx24: return D(m.motherboardItemId) != null;
                case CableCircuit.CpuEps: return D(m.motherboardItemId) != null && D(m.cpuItemId) != null;
                case CableCircuit.GpuPower: return Has(D(m.gpuItemId), "PCIE8") || Has(D(m.gpuItemId), "12V2x6");
                case CableCircuit.SataPower: case CableCircuit.SataData: return HasSata(m);
                case CableCircuit.FrontPanel: return D(m.caseItemId) != null && D(m.motherboardItemId) != null;
                case CableCircuit.CpuFan: return D(m.coolerItemId) != null;
                case CableCircuit.Pump: return Tagged(D(m.coolerItemId), "aio");
                case CableCircuit.Rgb: return Tagged(D(m.caseItemId), "rgb") || m.fanItemIds.Any(id => Tagged(D(id), "rgb"));
                default: return false;
            }
        }

        public static bool Connected(MachineState m, CableCircuit circuit)
        {
            if (m?.cables == null) return false;
            switch (circuit)
            {
                case CableCircuit.Atx24: return m.cables.atx24;
                case CableCircuit.CpuEps: return m.cables.cpuEps;
                case CableCircuit.GpuPower: return m.cables.gpuPower;
                case CableCircuit.SataPower: return m.cables.sataPower;
                case CableCircuit.SataData: return m.cables.sataData;
                case CableCircuit.FrontPanel: return m.cables.frontPanel;
                case CableCircuit.CpuFan: return m.cables.cpuFan;
                case CableCircuit.Pump: return m.cables.pump;
                case CableCircuit.Rgb: return m.cables.rgb;
                default: return false;
            }
        }

        public ActionResult SetConnection(MachineState m, CableCircuit circuit, bool connected)
        {
            if (m == null) return ActionResult.Fail("No device on bench.");
            if (!Enum.IsDefined(typeof(CableCircuit), circuit)) return ActionResult.Fail("Unknown cable circuit.");
            if (m.bootState != BootState.Off) return ActionResult.Fail("Power off the PC before changing cables.");
            if (m.sidePanelInstalled && !string.IsNullOrEmpty(m.caseItemId)) return ActionResult.Fail("Remove the side panel to reach the cable connectors.");
            if (connected)
            {
                if (!Present(m, circuit)) return ActionResult.Fail(Label(circuit) + " has no installed destination.");
                if (!Compatible(m, circuit)) return ActionResult.Fail(Label(circuit) + " requires compatible source and destination connectors.");
            }
            if (m.cables == null) m.cables = new CableState();
            if (Connected(m, circuit) == connected) return ActionResult.Success(Label(circuit) + " is already " + (connected ? "connected." : "disconnected."));
            switch (circuit)
            {
                case CableCircuit.Atx24: m.cables.atx24 = connected; break;
                case CableCircuit.CpuEps: m.cables.cpuEps = connected; break;
                case CableCircuit.GpuPower: m.cables.gpuPower = connected; break;
                case CableCircuit.SataPower: m.cables.sataPower = connected; break;
                case CableCircuit.SataData: m.cables.sataData = connected; break;
                case CableCircuit.FrontPanel: m.cables.frontPanel = connected; break;
                case CableCircuit.CpuFan: m.cables.cpuFan = connected; break;
                case CableCircuit.Pump: m.cables.pump = connected; break;
                case CableCircuit.Rgb: m.cables.rgb = connected; break;
            }
            m.stressStable = false; m.benchmarkScore = 0;
            string message = Label(circuit) + (connected ? " connected." : " disconnected.");
            m.history.Add(message);
            return ActionResult.Success(message);
        }

        private bool Compatible(MachineState m, CableCircuit circuit)
        {
            var board = D(m.motherboardItemId); var psu = D(m.psuItemId); var gpu = D(m.gpuItemId);
            switch (circuit)
            {
                case CableCircuit.Atx24: return Has(board, "ATX24") && Has(psu, "ATX24");
                case CableCircuit.CpuEps: return Has(board, "EPS8") && Has(psu, "EPS8");
                case CableCircuit.GpuPower: return gpu.connectors.Where(c => c == "PCIE8" || c == "12V2x6").All(c => Has(psu, c));
                case CableCircuit.SataPower: return Has(psu, "SATA_POWER");
                case CableCircuit.SataData: return Has(board, "SATA") && board.sataPorts > 0;
                case CableCircuit.FrontPanel: return Has(board, "FRONT_PANEL");
                case CableCircuit.CpuFan: return Has(board, "CPU_FAN");
                case CableCircuit.Pump: return board != null && board.fanHeaders >= 2;
                case CableCircuit.Rgb: return board != null && (Has(board, "RGB") || Has(board, "ARGB"));
                default: return false;
            }
        }

        public ActionResult CanRemove(MachineState m, HardwareDefinition part)
        {
            if (m == null || part == null) return ActionResult.Fail("Component data unavailable.");
            if (m.bootState != BootState.Off) return ActionResult.Fail("Power off the PC before removing components.");
            foreach (CableCircuit circuit in Enum.GetValues(typeof(CableCircuit)))
            {
                if (!Present(m, circuit) || !Connected(m, circuit)) continue;
                bool blocked = part.category == PartCategory.Motherboard && circuit != CableCircuit.SataPower && circuit != CableCircuit.GpuPower;
                blocked |= part.category == PartCategory.PSU && (circuit == CableCircuit.Atx24 || circuit == CableCircuit.CpuEps || circuit == CableCircuit.GpuPower || circuit == CableCircuit.SataPower);
                blocked |= part.category == PartCategory.GPU && circuit == CableCircuit.GpuPower;
                blocked |= part.category == PartCategory.Cooler && (circuit == CableCircuit.CpuFan || circuit == CableCircuit.Pump);
                blocked |= part.category == PartCategory.Storage && part.storageInterface == "SATA" && (circuit == CableCircuit.SataData || circuit == CableCircuit.SataPower);
                blocked |= part.category == PartCategory.Case && (circuit == CableCircuit.FrontPanel || circuit == CableCircuit.Rgb);
                blocked |= part.category == PartCategory.Fan && Tagged(part, "rgb") && circuit == CableCircuit.Rgb;
                if (blocked) return ActionResult.Fail("Disconnect " + Label(circuit) + " before removing this component.");
            }
            return ActionResult.Success("Cable connections released.");
        }

        public static void InvalidateConnections(MachineState m, PartCategory category)
        {
            if (m.cables == null) m.cables = new CableState();
            var c = m.cables;
            if (category == PartCategory.Motherboard) { c.atx24 = c.cpuEps = c.frontPanel = c.cpuFan = c.pump = c.sataData = c.rgb = false; }
            if (category == PartCategory.PSU) { c.atx24 = c.cpuEps = c.gpuPower = c.sataPower = false; }
            if (category == PartCategory.GPU) c.gpuPower = false;
            if (category == PartCategory.Cooler) c.cpuFan = c.pump = false;
            if (category == PartCategory.Storage) c.sataPower = c.sataData = false;
            if (category == PartCategory.Case) c.frontPanel = c.rgb = false;
            if (category == PartCategory.Fan) c.rgb = false;
            if (category == PartCategory.CPU) c.cpuEps = false;
        }
    }
}
