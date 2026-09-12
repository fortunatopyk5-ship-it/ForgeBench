using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Stateful custom-loop workflow. Every action changes persisted MachineState data;
    /// the loop cannot be marked ready until hardware, fittings, coolant, bleed and leak
    /// validation all pass in order.
    /// </summary>
    public sealed class LiquidCoolingService
    {
        private readonly InventoryService inventory;
        private readonly WorkshopState workshop;

        public LiquidCoolingService(InventoryService i, WorkshopState w)
        {
            inventory = i;
            workshop = w;
        }

        private static LiquidLoopState S(MachineState m)
        {
            if (m.liquidLoop == null) m.liquidLoop = new LiquidLoopState();
            if (m.liquidLoop.history == null) m.liquidLoop.history = new List<string>();
            return m.liquidLoop;
        }

        public ActionResult InstallPump(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No machine is on the liquid-cooling bench.");
            LiquidLoopState s = S(m);
            if (s.pumpInstalled) return ActionResult.Fail("Pump is already installed.");
            s.pumpInstalled = true;
            s.leakTestPassed = false;
            s.history.Add("Pump mounted and electrically isolated for loop assembly");
            return ActionResult.Success("Pump installed. Add a reservoir and radiator before filling.");
        }

        public ActionResult InstallReservoir(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No machine is on the liquid-cooling bench.");
            LiquidLoopState s = S(m);
            if (s.reservoirInstalled) return ActionResult.Fail("Reservoir is already installed.");
            s.reservoirInstalled = true;
            s.leakTestPassed = false;
            s.history.Add("Reservoir mounted above pump inlet");
            return ActionResult.Success("Reservoir installed.");
        }

        public ActionResult InstallRadiator(MachineState m, int sizeMm)
        {
            if (m == null) return ActionResult.Fail("No machine is on the liquid-cooling bench.");
            if (sizeMm != 120 && sizeMm != 240 && sizeMm != 280 && sizeMm != 360 && sizeMm != 420)
                return ActionResult.Fail("Unsupported radiator size.");
            HardwareDefinition pcCase = inventory.Def(inventory.Get(m.caseItemId));
            if (pcCase != null && pcCase.radiatorSupportMm > 0 && sizeMm > pcCase.radiatorSupportMm)
                return ActionResult.Fail("The case supports radiators only up to " + pcCase.radiatorSupportMm + " mm.");
            LiquidLoopState s = S(m);
            s.radiatorMm = sizeMm;
            s.leakTestPassed = false;
            s.history.Add(sizeMm + " mm radiator installed");
            return ActionResult.Success(sizeMm + " mm radiator installed.");
        }

        public ActionResult AddFittings(MachineState m, int count = 2)
        {
            if (m == null) return ActionResult.Fail("No machine is on the liquid-cooling bench.");
            LiquidLoopState s = S(m);
            if (count <= 0 || count > 8) return ActionResult.Fail("Invalid fitting quantity.");
            s.fittingCount = Mathf.Clamp(s.fittingCount + count, 0, 24);
            s.leakTestPassed = false;
            s.history.Add(count + " fitting(s) installed; total " + s.fittingCount);
            return ActionResult.Success("Installed fittings: " + s.fittingCount + ".");
        }

        public ActionResult TightenNextFitting(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No machine is on the liquid-cooling bench.");
            LiquidLoopState s = S(m);
            if (s.fittingCount == 0) return ActionResult.Fail("Install fittings first.");
            if (s.tightFittings >= s.fittingCount) return ActionResult.Success("All fittings are already tightened.");
            s.tightFittings++;
            s.leakTestPassed = false;
            s.history.Add("Fitting " + s.tightFittings + "/" + s.fittingCount + " tightened");
            return ActionResult.Success("Fitting tightened: " + s.tightFittings + "/" + s.fittingCount + ".");
        }

        public ActionResult Fill(MachineState m, float litres = .25f)
        {
            if (m == null) return ActionResult.Fail("No machine is on the liquid-cooling bench.");
            LiquidLoopState s = S(m);
            if (!s.pumpInstalled || !s.reservoirInstalled || s.radiatorMm <= 0)
                return ActionResult.Fail("Install pump, reservoir and radiator before filling.");
            if (s.fittingCount < 6) return ActionResult.Fail("The loop is incomplete: at least six fittings are required.");
            if (litres <= 0f) return ActionResult.Fail("Invalid coolant amount.");
            s.coolantLitres = Mathf.Clamp(s.coolantLitres + litres, 0f, 1.5f);
            s.airFraction = Mathf.Clamp01(1f - s.coolantLitres / .75f);
            s.leakTestPassed = false;
            s.history.Add("Added " + litres.ToString("0.00") + " L coolant; total " + s.coolantLitres.ToString("0.00") + " L");
            return ActionResult.Success("Coolant level: " + s.coolantLitres.ToString("0.00") + " L. Bleed trapped air before final validation.");
        }

        public ActionResult Bleed(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No machine is on the liquid-cooling bench.");
            LiquidLoopState s = S(m);
            if (s.coolantLitres < .45f) return ActionResult.Fail("Coolant level is too low to bleed the loop safely.");
            if (!s.pumpInstalled) return ActionResult.Fail("Pump is not installed.");
            if (s.tightFittings < s.fittingCount) return ActionResult.Fail("Tighten all fittings before running the pump.");
            s.airFraction = Mathf.Max(0f, s.airFraction - (.30f + workshop.benchLevel * .08f));
            s.flowLpm = Mathf.Clamp(1.25f + s.radiatorMm / 500f - s.airFraction * .8f, .2f, 3.2f);
            s.pressureKpa = Mathf.Clamp(22f + s.flowLpm * 11f, 15f, 70f);
            s.history.Add("Loop bled to " + Mathf.RoundToInt(s.airFraction * 100f) + "% trapped air; flow " + s.flowLpm.ToString("0.00") + " L/min");
            return ActionResult.Success("Bleeding pass complete. Trapped air " + Mathf.RoundToInt(s.airFraction * 100f) + "%.");
        }

        public ActionResult LeakTest(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No machine is on the liquid-cooling bench.");
            LiquidLoopState s = S(m);
            List<string> blockers = new List<string>();
            if (!s.pumpInstalled) blockers.Add("pump missing");
            if (!s.reservoirInstalled) blockers.Add("reservoir missing");
            if (s.radiatorMm <= 0) blockers.Add("radiator missing");
            if (s.fittingCount < 6) blockers.Add("insufficient fittings");
            if (s.tightFittings < s.fittingCount) blockers.Add("loose fitting(s)");
            if (s.coolantLitres < .45f) blockers.Add("coolant level low");
            if (blockers.Count > 0)
            {
                s.leakDetected = s.fittingCount > 0 && s.tightFittings < s.fittingCount && s.coolantLitres > 0f;
                s.leakTestPassed = false;
                return ActionResult.Fail("Leak test blocked: " + string.Join(", ", blockers) + ".");
            }

            s.leakDetected = s.airFraction > .32f || s.pressureKpa > 65f;
            s.leakTestPassed = !s.leakDetected && s.airFraction <= .18f && s.flowLpm >= .8f;
            if (s.leakDetected)
            {
                s.history.Add("Leak test FAILED; drain/bleed inspection required");
                return ActionResult.Fail("Leak test failed. Check trapped air, pressure and all fittings before powering PC hardware.");
            }
            if (!s.leakTestPassed)
            {
                s.history.Add("Leak test incomplete because flow/air targets were not met");
                return ActionResult.Fail("No active leak detected, but the loop is not fully bled. Bleed again before approval.");
            }
            s.history.Add("15-minute isolated pump leak test PASSED");
            return ActionResult.Success("Liquid loop leak test passed: " + s.flowLpm.ToString("0.00") + " L/min, " + s.pressureKpa.ToString("0") + " kPa.");
        }

        public void AgeOneDay(MachineState m)
        {
            if (m == null || m.liquidLoop == null || m.liquidLoop.coolantLitres <= 0f) return;
            m.liquidLoop.coolantAgeDays++;
            if (m.liquidLoop.coolantAgeDays % 30 == 0)
                m.liquidLoop.flowLpm = Mathf.Max(.2f, m.liquidLoop.flowLpm * .985f);
        }
    }

    /// <summary>Deterministic microsoldering/board-repair workflow with ESD and pad-damage failure states.</summary>
    public sealed class BoardRepairService
    {
        private readonly WorkshopState workshop;
        public BoardRepairService(WorkshopState w) { workshop = w; }

        private static BoardRepairState S(MachineState m)
        {
            if (m.boardRepair == null) m.boardRepair = new BoardRepairState();
            if (m.boardRepair.history == null) m.boardRepair.history = new List<string>();
            return m.boardRepair;
        }

        private ActionResult Unlocked()
        {
            return workshop.boardRepairLevel > 0 ? ActionResult.Success("Board station ready.") : ActionResult.Fail("Board-repair station unlocks at workshop level 3.");
        }

        public ActionResult GroundEsd(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No board on the repair station.");
            ActionResult u = Unlocked(); if (!u.ok) return u;
            BoardRepairState s = S(m); s.esdGrounded = true; s.history.Add("ESD mat and wrist strap continuity verified");
            return ActionResult.Success("ESD grounding verified.");
        }

        public ActionResult Inspect(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No board on the repair station.");
            ActionResult u = Unlocked(); if (!u.ok) return u;
            BoardRepairState s = S(m);
            if (!s.esdGrounded) return ActionResult.Fail("Connect ESD protection before microscope inspection.");
            s.microscopeInspected = true;
            s.lastMeasurement = "Microscope: connector/power-stage inspection complete";
            s.history.Add(s.lastMeasurement);
            return ActionResult.Success("Microscope inspection complete. Measure the suspect power rail next.");
        }

        public ActionResult MeasureRail(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No board on the repair station.");
            ActionResult u = Unlocked(); if (!u.ok) return u;
            BoardRepairState s = S(m);
            if (!s.esdGrounded || !s.microscopeInspected) return ActionResult.Fail("Ground ESD and inspect the board before probing powered rails.");
            bool boardFault = m.history.Any(x => x.IndexOf("short", StringComparison.OrdinalIgnoreCase) >= 0) || m.lastDiagnostic != null && m.lastDiagnostic.IndexOf("ConnectorDamage", StringComparison.OrdinalIgnoreCase) >= 0;
            s.powerRailMeasured = true;
            s.measuredRailV = boardFault ? .18f : .92f;
            s.lastMeasurement = "Core rail: " + s.measuredRailV.ToString("0.00") + " V";
            s.history.Add(s.lastMeasurement);
            return ActionResult.Success(s.lastMeasurement + (s.measuredRailV < .5f ? " — abnormal low rail / probable short." : " — rail is present."));
        }

        public ActionResult LocateShort(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No board on the repair station.");
            BoardRepairState s = S(m);
            if (!s.powerRailMeasured) return ActionResult.Fail("Measure the suspect rail first.");
            s.shortLocated = s.measuredRailV < .5f;
            if (!s.shortLocated) return ActionResult.Fail("Measurements do not indicate a short on this rail. Continue diagnostics rather than reworking blindly.");
            s.history.Add("Short localized to power-stage component by resistance/thermal correlation");
            return ActionResult.Success("Short localized. Apply flux before hot-air rework.");
        }

        public ActionResult ApplyFlux(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No board on the repair station.");
            BoardRepairState s = S(m);
            if (!s.shortLocated) return ActionResult.Fail("Do not apply rework materials until the faulty area is localized.");
            s.fluxApplied = true; s.history.Add("No-clean flux applied to isolated rework zone");
            return ActionResult.Success("Flux applied. Rework can begin.");
        }

        public ActionResult Rework(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No board on the repair station.");
            BoardRepairState s = S(m);
            if (!s.esdGrounded) return ActionResult.Fail("ESD protection is disconnected.");
            if (!s.shortLocated || !s.fluxApplied) return ActionResult.Fail("Locate the fault and apply flux before rework.");
            s.reworkCycles++;
            float skill = Mathf.Clamp01(.58f + workshop.boardRepairLevel * .10f + workshop.benchLevel * .025f);
            s.solderQuality = Mathf.Clamp01(skill - Mathf.Max(0, s.reworkCycles - 1) * .13f);
            if (s.reworkCycles >= 4 || s.solderQuality < .38f) s.padsIntact = false;
            s.fluxApplied = false;
            s.history.Add("Hot-air/reflow cycle " + s.reworkCycles + "; solder quality " + Mathf.RoundToInt(s.solderQuality * 100f) + "%");
            if (!s.padsIntact) return ActionResult.Fail("Pad damage detected from excessive rework. Board now requires advanced trace/pad reconstruction.");
            return ActionResult.Success("Rework cycle complete. Clean and verify the rail before declaring repair.");
        }

        public ActionResult Verify(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No board on the repair station.");
            BoardRepairState s = S(m);
            if (!s.padsIntact) return ActionResult.Fail("Verification failed: damaged pads remain.");
            if (s.reworkCycles == 0 || s.solderQuality < .62f) return ActionResult.Fail("Repair joint quality is below acceptance threshold.");
            s.measuredRailV = 1.00f;
            s.powerRailMeasured = true;
            s.shortLocated = false;
            s.repaired = true;
            s.lastMeasurement = "Post-repair core rail: 1.00 V; no short detected";
            s.history.Add(s.lastMeasurement);
            m.history.Add("Board-level repair verified electrically");
            return ActionResult.Success("Board repair verified. Rail is stable and the short is cleared.");
        }
    }

    /// <summary>Disassembly-first portable electronics workflow for laptop/phone/tablet/console devices.</summary>
    public sealed class PortableRepairService
    {
        private readonly InventoryService inventory;
        private readonly WorkshopState workshop;
        public PortableRepairService(InventoryService i, WorkshopState w) { inventory = i; workshop = w; }

        private static PortableDeviceState S(MachineState m)
        {
            if (m.portable == null) m.portable = new PortableDeviceState();
            if (m.portable.history == null) m.portable.history = new List<string>();
            return m.portable;
        }

        private static bool Portable(DeviceCategory c)
        {
            return c == DeviceCategory.Laptop || c == DeviceCategory.Phone || c == DeviceCategory.Tablet || c == DeviceCategory.Console || c == DeviceCategory.Handheld || c == DeviceCategory.Controller;
        }

        public ActionResult RemoveScrew(MachineState m)
        {
            if (m == null || !Portable(m.category)) return ActionResult.Fail("This workflow is for portable electronics.");
            PortableDeviceState s = S(m);
            if (s.backCoverRemoved) return ActionResult.Fail("Back cover is already removed.");
            if (s.screwsRemaining <= 0) return ActionResult.Success("All perimeter screws are already removed.");
            s.screwsRemaining--;
            s.history.Add("Removed chassis screw; " + s.screwsRemaining + " remaining");
            return ActionResult.Success("Screw removed. " + s.screwsRemaining + " remaining.");
        }

        public ActionResult RemoveBackCover(MachineState m)
        {
            if (m == null || !Portable(m.category)) return ActionResult.Fail("This workflow is for portable electronics.");
            PortableDeviceState s = S(m);
            if (s.screwsRemaining > 0) return ActionResult.Fail("Remove all chassis screws before lifting the cover.");
            if (s.backCoverRemoved) return ActionResult.Success("Back cover is already removed.");
            s.backCoverRemoved = true;
            s.sealed = false;
            s.adhesiveIntegrity = Mathf.Max(0f, s.adhesiveIntegrity - (m.category == DeviceCategory.Phone || m.category == DeviceCategory.Tablet ? .35f : .05f));
            s.history.Add("Back cover removed without connector damage");
            return ActionResult.Success("Back cover removed. Disconnect the battery before touching internal connectors.");
        }

        public ActionResult DisconnectBattery(MachineState m)
        {
            if (m == null || !Portable(m.category)) return ActionResult.Fail("This workflow is for portable electronics.");
            PortableDeviceState s = S(m);
            if (!s.backCoverRemoved) return ActionResult.Fail("Open the device first.");
            if (s.batteryDisconnected) return ActionResult.Success("Battery is already disconnected.");
            s.batteryDisconnected = true; s.history.Add("Battery isolated from main board");
            return ActionResult.Success("Battery disconnected. Internal service is now electrically safe.");
        }

        public ActionResult SeparateDisplay(MachineState m)
        {
            if (m == null || !Portable(m.category)) return ActionResult.Fail("This workflow is for portable electronics.");
            PortableDeviceState s = S(m);
            if (!s.batteryDisconnected) return ActionResult.Fail("Disconnect the battery before separating the display.");
            if (s.displaySeparated) return ActionResult.Success("Display is already separated.");
            s.displaySeparated = true;
            s.adhesiveIntegrity = Mathf.Max(0f, s.adhesiveIntegrity - .35f);
            s.history.Add("Display adhesive cut and panel separated");
            return ActionResult.Success("Display separated. Screen/port service can proceed.");
        }

        public ActionResult ReplaceBattery(MachineState m)
        {
            if (m == null || !Portable(m.category)) return ActionResult.Fail("This workflow is for portable electronics.");
            PortableDeviceState s = S(m);
            if (!s.batteryDisconnected) return ActionResult.Fail("Disconnect the old battery before replacement.");
            ItemInstance replacement = inventory.Available(PartCategory.Battery).OrderByDescending(i => inventory.Def(i)?.quality ?? 0).FirstOrDefault();
            if (replacement == null) return ActionResult.Fail("No replacement battery is available in inventory.");
            inventory.Consume(replacement.instanceId);
            s.batteryHealth = Mathf.Clamp01(.94f + (inventory.Def(replacement)?.quality ?? 60) / 2000f);
            s.history.Add("Battery replaced; health " + Mathf.RoundToInt(s.batteryHealth * 100f) + "%");
            return ActionResult.Success("Battery replacement completed. Health " + Mathf.RoundToInt(s.batteryHealth * 100f) + "%.");
        }

        public ActionResult ReplaceDisplay(MachineState m)
        {
            if (m == null || !Portable(m.category)) return ActionResult.Fail("This workflow is for portable electronics.");
            PortableDeviceState s = S(m);
            if (!s.displaySeparated) return ActionResult.Fail("Separate the display before replacement.");
            ItemInstance replacement = inventory.Available(PartCategory.Display).OrderByDescending(i => inventory.Def(i)?.quality ?? 0).FirstOrDefault();
            if (replacement == null) return ActionResult.Fail("No compatible display module is available in inventory.");
            inventory.Consume(replacement.instanceId);
            s.displayHealth = 1f;
            s.history.Add("Display module replaced and flex connector seated");
            return ActionResult.Success("Display module replaced.");
        }

        public ActionResult ServiceChargingPort(MachineState m)
        {
            if (m == null || !Portable(m.category)) return ActionResult.Fail("This workflow is for portable electronics.");
            PortableDeviceState s = S(m);
            if (!s.batteryDisconnected) return ActionResult.Fail("Disconnect battery power before port service.");
            float q = Mathf.Clamp01(.75f + workshop.diagnosticsLevel * .06f + workshop.boardRepairLevel * .05f);
            s.chargingPortHealth = Mathf.Max(s.chargingPortHealth, q);
            s.history.Add("Charging connector cleaned/reworked; health " + Mathf.RoundToInt(s.chargingPortHealth * 100f) + "%");
            return ActionResult.Success("Charging port service complete: " + Mathf.RoundToInt(s.chargingPortHealth * 100f) + "% health.");
        }

        public ActionResult CalibrateController(MachineState m)
        {
            if (m == null || (m.category != DeviceCategory.Controller && m.category != DeviceCategory.Console && m.category != DeviceCategory.Handheld))
                return ActionResult.Fail("No controller-class device is active.");
            PortableDeviceState s = S(m);
            float before = s.controllerDrift;
            s.controllerDrift = Mathf.Max(0f, s.controllerDrift - (.12f + workshop.diagnosticsLevel * .035f));
            s.history.Add("Analog calibration: drift " + before.ToString("0.00") + " → " + s.controllerDrift.ToString("0.00"));
            return ActionResult.Success("Controller calibration complete. Residual drift " + s.controllerDrift.ToString("0.00") + ".");
        }

        public ActionResult Reseal(MachineState m)
        {
            if (m == null || !Portable(m.category)) return ActionResult.Fail("This workflow is for portable electronics.");
            PortableDeviceState s = S(m);
            if (!s.backCoverRemoved) return ActionResult.Fail("Device is not open.");
            if (!s.batteryDisconnected) return ActionResult.Fail("Battery state is inconsistent; reconnect/verify before sealing.");
            s.batteryDisconnected = false;
            s.displaySeparated = false;
            s.backCoverRemoved = false;
            s.screwsRemaining = m.category == DeviceCategory.Phone || m.category == DeviceCategory.Tablet ? 2 : 8;
            float baseSeal = (m.category == DeviceCategory.Phone || m.category == DeviceCategory.Tablet) ? .82f : .95f;
            s.sealQuality = Mathf.Clamp01(baseSeal + workshop.benchLevel * .025f - s.waterDamage * .20f);
            s.adhesiveIntegrity = s.sealQuality;
            s.sealed = true;
            s.history.Add("Device reassembled; seal quality " + Mathf.RoundToInt(s.sealQuality * 100f) + "%");
            return ActionResult.Success("Device resealed. Seal quality " + Mathf.RoundToInt(s.sealQuality * 100f) + "%.");
        }
    }

    /// <summary>Networking/NAS/server diagnostics with persistent link, addressing and RAID state.</summary>
    public sealed class NetworkLabService
    {
        private readonly InventoryService inventory;
        private readonly WorkshopState workshop;
        public NetworkLabService(InventoryService i, WorkshopState w) { inventory = i; workshop = w; }

        private static NetworkLabState S(MachineState m)
        {
            if (m.network == null) m.network = new NetworkLabState();
            if (m.network.history == null) m.network.history = new List<string>();
            return m.network;
        }

        public ActionResult ConnectLink(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No network device is active.");
            NetworkLabState s = S(m);
            bool networkClass = m.category == DeviceCategory.NAS || m.category == DeviceCategory.Server || m.category == DeviceCategory.Router || m.category == DeviceCategory.Desktop;
            if (!networkClass) return ActionResult.Fail("This device is not assigned to the network lab.");
            s.linkUp = true;
            s.packetLoss = Mathf.Max(0f, s.packetLoss);
            s.history.Add("Ethernet link negotiated successfully");
            return ActionResult.Success("Network link is up.");
        }

        public ActionResult ConfigureDhcp(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No network device is active.");
            NetworkLabState s = S(m);
            if (!s.linkUp) return ActionResult.Fail("Bring the physical link up first.");
            s.dhcp = true; s.ipAddress = "192.168.1." + (10 + Mathf.Abs(m.machineId == null ? 1 : m.machineId.GetHashCode()) % 180);
            s.subnetMask = "255.255.255.0";
            s.history.Add("DHCP lease acquired: " + s.ipAddress);
            return ActionResult.Success("DHCP configured: " + s.ipAddress + ".");
        }

        public ActionResult ConfigureStatic(MachineState m, string ip = "192.168.1.50")
        {
            if (m == null) return ActionResult.Fail("No network device is active.");
            NetworkLabState s = S(m);
            if (!s.linkUp) return ActionResult.Fail("Bring the physical link up first.");
            if (string.IsNullOrWhiteSpace(ip) || ip.Count(c => c == '.') != 3) return ActionResult.Fail("Invalid IPv4 address.");
            s.dhcp = false; s.ipAddress = ip; s.subnetMask = "255.255.255.0";
            s.history.Add("Static IPv4 configured: " + ip);
            return ActionResult.Success("Static network configuration applied.");
        }

        public ActionResult ThroughputTest(MachineState m)
        {
            if (m == null) return ActionResult.Fail("No network device is active.");
            NetworkLabState s = S(m);
            if (!s.linkUp) return ActionResult.Fail("Network link is down.");
            float tier = Mathf.Max(1, workshop.diagnosticsLevel);
            float health = s.arrayDegraded ? .62f : 1f;
            s.packetLoss = Mathf.Clamp(s.packetLoss, 0f, .25f);
            s.latencyMs = Mathf.RoundToInt(2f + s.packetLoss * 180f + (s.arrayDegraded ? 4f : 0f));
            s.throughputMbps = Mathf.Clamp(940f * health * (1f - s.packetLoss) * Mathf.Lerp(.88f, 1.02f, Mathf.Clamp01(tier / 4f)), 10f, 2500f);
            s.history.Add("iperf-style test " + s.throughputMbps.ToString("0") + " Mbps, loss " + (s.packetLoss * 100f).ToString("0.0") + "%, latency " + s.latencyMs + " ms");
            return ActionResult.Success("Throughput " + s.throughputMbps.ToString("0") + " Mbps · loss " + (s.packetLoss * 100f).ToString("0.0") + "% · " + s.latencyMs + " ms.");
        }

        public ActionResult ConfigureRaid(MachineState m, int level)
        {
            if (m == null || (m.category != DeviceCategory.NAS && m.category != DeviceCategory.Server))
                return ActionResult.Fail("RAID configuration is available only for NAS/server jobs.");
            if (level != 0 && level != 1 && level != 5 && level != 10) return ActionResult.Fail("Supported RAID levels: 0, 1, 5, 10.");
            NetworkLabState s = S(m);
            int disks = Mathf.Max(s.disksTotal, m.storageItemIds.Count);
            int required = level == 0 ? 2 : level == 1 ? 2 : level == 5 ? 3 : 4;
            if (disks < required) return ActionResult.Fail("RAID " + level + " requires at least " + required + " disks; only " + disks + " available.");
            s.disksTotal = disks; s.raidLevel = level; s.disksHealthy = Mathf.Clamp(s.disksHealthy, 0, disks); s.arrayDegraded = s.disksHealthy < disks;
            s.scrubComplete = false; s.history.Add("RAID " + level + " configured across " + disks + " disks");
            return ActionResult.Success("RAID " + level + " configured across " + disks + " disks.");
        }

        public ActionResult ReplaceFailedDisk(MachineState m)
        {
            if (m == null || (m.category != DeviceCategory.NAS && m.category != DeviceCategory.Server)) return ActionResult.Fail("No NAS/server array is active.");
            NetworkLabState s = S(m);
            if (s.disksHealthy >= s.disksTotal) return ActionResult.Success("All array disks are already healthy.");
            ItemInstance disk = inventory.Available(PartCategory.Storage).OrderByDescending(i => inventory.Def(i)?.storageGB ?? 0).FirstOrDefault();
            if (disk == null) return ActionResult.Fail("No replacement storage device is available in inventory.");
            inventory.Consume(disk.instanceId);
            s.disksHealthy++;
            s.arrayDegraded = s.disksHealthy < s.disksTotal;
            s.scrubComplete = false;
            s.history.Add("Failed array disk replaced; healthy " + s.disksHealthy + "/" + s.disksTotal);
            return ActionResult.Success("Replacement disk installed. Rebuild/scrub is required.");
        }

        public ActionResult Scrub(MachineState m)
        {
            if (m == null || (m.category != DeviceCategory.NAS && m.category != DeviceCategory.Server)) return ActionResult.Fail("No NAS/server array is active.");
            NetworkLabState s = S(m);
            if (s.disksHealthy < s.disksTotal) { s.arrayDegraded = true; return ActionResult.Fail("Array remains degraded; replace failed disk(s) before scrub completion."); }
            s.arrayDegraded = false; s.scrubComplete = true; s.packetLoss = Mathf.Max(0f, s.packetLoss - .01f);
            s.history.Add("Array rebuild and checksum scrub completed without error");
            return ActionResult.Success("RAID rebuild/scrub completed. Array is healthy.");
        }
    }
}
