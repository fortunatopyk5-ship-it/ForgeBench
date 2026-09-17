using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Physical first-person assembly presentation. Gameplay services own all state;
    /// this renderer rebuilds deterministic visual children from that state.
    /// </summary>
    public sealed class PhysicalAssemblyController : MonoBehaviour
    {
        private GameRuntime game;
        private GameObject root;
        private Shader shader;
        private readonly List<Material> materials = new List<Material>();
        private string signature = "";
        private bool dirty = true;
        private float nextPoll;

        private void Start()
        {
            game = GameRuntime.Instance;
            shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            game?.Events?.Subscribe("state.changed", OnStateChanged);
            dirty = true;
        }

        private void OnDestroy()
        {
            game?.Events?.Unsubscribe("state.changed", OnStateChanged);
            DestroyVisual();
        }

        private void OnStateChanged(object payload) { dirty = true; }

        private void LateUpdate()
        {
            if (game == null) { game = GameRuntime.Instance; if (game == null) return; }
            GameObject old = GameObject.Find("ActiveMachine3D");
            if (old != null && old != root && old.activeSelf) old.SetActive(false);

            if (Time.unscaledTime < nextPoll && !dirty) return;
            nextPoll = Time.unscaledTime + .25f;
            string now = Signature(game.ActiveMachine);
            if (dirty || now != signature) { signature = now; dirty = false; Rebuild(); }
        }

        private static string Signature(MachineState m)
        {
            if (m == null) return "none";
            StringBuilder s = new StringBuilder(384);
            s.Append(m.machineId).Append('|').Append(m.caseItemId).Append('|').Append(m.motherboardItemId).Append('|')
                .Append(m.cpuItemId).Append('|').Append(m.gpuItemId).Append('|').Append(m.psuItemId).Append('|').Append(m.coolerItemId);
            for (int i = 0; i < m.ramItemIds.Count; i++)
            {
                s.Append("|r:").Append(m.ramItemIds[i]).Append('@');
                s.Append(m.ramSlotIndices != null && i < m.ramSlotIndices.Count ? m.ramSlotIndices[i] : i);
            }
            foreach (string x in m.storageItemIds) s.Append("|s:").Append(x);
            if (m.ramLatches != null) foreach (RamLatchState latch in m.ramLatches)
                s.Append("|l:").Append(latch?.topOpen).Append(':').Append(latch?.bottomOpen);
            s.Append("|power:").Append(m.bootState);
            s.Append("|cpu-lock:").Append(m.cpuRetentionOpen);
            foreach (string x in m.fanItemIds) s.Append("|f:").Append(x);
            s.Append('|').Append(m.sidePanelInstalled).Append('|').Append(m.thermalPasteApplied)
                .Append('|').Append(m.cables.atx24).Append('|').Append(m.cables.cpuEps)
                .Append('|').Append(m.cables.gpuPower).Append('|').Append(m.cables.sataData).Append('|').Append(m.cables.sataPower).Append('|').Append(m.cables.rgb)
                .Append('|').Append(m.cables.frontPanel).Append('|').Append(m.cables.cpuFan).Append('|').Append(m.cables.pump)
                .Append('|').Append(m.customization?.rgbEffect ?? 0).Append('|').Append(m.customization?.cableColorIndex ?? 0);
            if (m.sidePanel?.fasteners != null)
                foreach (FastenerState f in m.sidePanel.fasteners)
                    s.Append('|').Append(f.fastenerId).Append(':').Append(Mathf.RoundToInt(f.tightness * 100));
            return s.ToString();
        }

        private HardwareDefinition Def(string itemId)
        {
            return game?.Inventory?.Def(game.Inventory.Get(itemId));
        }

        private void Rebuild()
        {
            DestroyVisual();
            MachineState m = game.ActiveMachine;
            if (m == null) return;

            root = new GameObject("ProductionMachine3D");
            root.transform.position = new Vector3(0, 1.43f, 3.72f);

            Material frame = Mat(new Color(.075f, .085f, .095f), .30f, .85f);
            Material dark = Mat(new Color(.035f, .04f, .05f), .28f, .60f);
            Material board = Mat(new Color(.035f, .20f, .12f), .40f, .25f);
            Material copper = Mat(new Color(.72f, .34f, .12f), .42f, .72f);
            Material silver = Mat(new Color(.52f, .55f, .58f), .70f, .90f);
            Material black = Mat(new Color(.045f, .05f, .06f), .32f, .72f);
            Material blue = Mat(new Color(.055f, .25f, .48f), .42f, .35f);
            Material ghost = Mat(new Color(.16f, .22f, .28f), .18f, .1f);

            if (string.IsNullOrEmpty(m.caseItemId))
            {
                GameObject installCase = CreateGhostFrame("EMPTY CASE POSITION", Vector3.zero, new Vector3(.95f, .78f, .62f), ghost);
                AddSnap(installCase, PartCategory.Case);
                Interact(installCase, "Install best available case", 40, () => game.InstallBestAvailable(PartCategory.Case));
                return;
            }

            BuildCaseShell(m, frame, dark, silver);
            BuildMotherboard(m, board, black, silver, copper, ghost);
            BuildCpuAndCooler(m, copper, silver, black, ghost);
            BuildRam(m, blue, copper, black, ghost);
            BuildGpu(m, black, silver, ghost);
            BuildPsu(m, black, silver, ghost);
            BuildStorage(m, dark, board, silver, ghost);
            BuildFans(m, black, silver, ghost);
            BuildCables(m);
            BuildPanelAndFasteners(m, frame, silver);
            BuildPowerButton(m);

            BoxCollider col = root.AddComponent<BoxCollider>();
            col.size = new Vector3(1.05f, .92f, .72f);
            WorldInteractable rootInteraction = root.AddComponent<WorldInteractable>();
            rootInteraction.label = "Open assembly bench";
            rootInteraction.priority = 4;
            rootInteraction.maxDistance = 3.6f;
            rootInteraction.action = () => game.UI?.OpenTab("BENCH");
        }

        private void BuildCaseShell(MachineState m, Material frame, Material dark, Material silver)
        {
            HardwareDefinition pcCase = Def(m.caseItemId);
            bool compact = pcCase != null && (pcCase.formFactor == "mATX" || pcCase.formFactor == "MiniITX");
            float height = compact ? .66f : .72f;
            float yTop = height * .5f;
            Box("CaseBottom", new Vector3(0, -yTop, 0), new Vector3(.92f, .055f, .58f), frame);
            Box("CaseTop", new Vector3(0, yTop, 0), new Vector3(.92f, .055f, .58f), frame);
            Box("CaseRear", new Vector3(0, 0, .275f), new Vector3(.92f, height, .04f), frame);
            for (int sx = -1; sx <= 1; sx += 2)
                for (int sz = -1; sz <= 1; sz += 2)
                    Box("CaseRail", new Vector3(sx * .445f, 0, sz * .275f), new Vector3(.035f, height, .035f), frame);
            Box("FrontBezel", new Vector3(0, 0, -.30f), new Vector3(.92f, height, .035f), dark);
            for (int y = -2; y <= 2; y++)
                Box("FrontVent", new Vector3(0, y * .10f, -.323f), new Vector3(.65f, .018f, .010f), silver);
        }

        private void BuildMotherboard(MachineState m, Material board, Material chip, Material metal, Material copper, Material ghost)
        {
            Vector3 p = new Vector3(.06f, .02f, .235f);
            if (string.IsNullOrEmpty(m.motherboardItemId))
            {
                GameObject g = Box("MotherboardGhost", p, new Vector3(.62f, .57f, .022f), ghost);
                AddSnap(g, PartCategory.Motherboard);
                Interact(g, "Install motherboard", 32, () => game.InstallBestAvailable(PartCategory.Motherboard));
                return;
            }

            HardwareDefinition def = Def(m.motherboardItemId);
            float boardHeight = def != null && def.formFactor == "mATX" ? .49f : .57f;
            GameObject mb = Box("Motherboard", p, new Vector3(.62f, boardHeight, .026f), board);
            InteractPart(mb, m.motherboardItemId, "Remove motherboard");
            Box("CpuSocket", p + new Vector3(-.08f, .11f, -.022f), new Vector3(.16f, .16f, .025f), metal);
            Box("Chipset", p + new Vector3(.18f, -.13f, -.025f), new Vector3(.10f, .10f, .030f), chip);

            int vrmCount = Mathf.Clamp(4 + (def?.quality ?? 50) / 20, 4, 8);
            for (int i = 0; i < vrmCount; i++)
                Box("VRM_" + i, p + new Vector3(-.27f + i * (.38f / Mathf.Max(1, vrmCount - 1)), .24f, -.026f), new Vector3(.045f, .050f, .035f), chip);

            int dimmSlots = HardwarePresentationLayout.DimmSlotCount(def);
            for (int i = 0; i < dimmSlots; i++)
            {
                Vector3 rp = RamSlotPosition(i, dimmSlots);
                Box("DIMMSlot_" + i, new Vector3(rp.x, rp.y, p.z - .028f), new Vector3(.025f, .35f, .026f), chip);
            }

            int pcieSlots = HardwarePresentationLayout.PcieSlotCount(def);
            for (int i = 0; i < pcieSlots; i++)
                Box("PCIeSlot_" + i, p + new Vector3(.03f, -.16f - i * .065f, -.029f), new Vector3(.43f, .020f, .025f), chip);

            int m2Slots = HardwarePresentationLayout.M2SlotCount(def);
            for (int i = 0; i < m2Slots; i++)
                Box("M2Shield_" + i, p + new Vector3(-.14f + (i % 2) * .26f, -.08f - (i / 2) * .075f, -.032f), new Vector3(.20f, .042f, .020f), metal);

            for (int i = 0; i < 7; i++)
                Box("Trace_" + i, p + new Vector3(-.22f + i * .07f, -.02f, -.043f), new Vector3(.004f, boardHeight * .70f, .004f), copper);
        }

        private void BuildCpuAndCooler(MachineState m, Material copper, Material silver, Material black, Material ghost)
        {
            if (string.IsNullOrEmpty(m.motherboardItemId)) return;
            Vector3 cpuP = new Vector3(-.02f, .13f, .185f);
            if (string.IsNullOrEmpty(m.coolerItemId))
            {
                GameObject lever = Box("CpuRetentionLever", cpuP + new Vector3(.095f, 0, m.cpuRetentionOpen ? -.055f : -.018f),
                    new Vector3(.025f, .17f, .024f), silver);
                lever.transform.localRotation = Quaternion.Euler(m.cpuRetentionOpen ? 65f : 0f, 0, 0);
                Interact(lever, m.cpuRetentionOpen ? "Lock CPU retention lever" : "Open CPU retention lever", 49, () => game.ToggleCpuRetention());
            }
            if (string.IsNullOrEmpty(m.cpuItemId))
            {
                GameObject g = Box("CpuGhost", cpuP, new Vector3(.135f, .135f, .035f), ghost);
                AddSnap(g, PartCategory.CPU);
                Interact(g, "Install CPU", 36, () => game.InstallBestAvailable(PartCategory.CPU));
            }
            else
            {
                GameObject cpu = Box("CPU", cpuP, new Vector3(.135f, .135f, .035f), silver);
                InteractPart(cpu, m.cpuItemId, "Remove CPU");
                if (m.thermalPasteApplied)
                    Cylinder("ThermalPaste", cpuP + new Vector3(0, 0, -.025f), new Vector3(.045f, .006f, .045f), Quaternion.Euler(90, 0, 0), Mat(new Color(.62f, .64f, .66f), .18f, .05f));
                else if (string.IsNullOrEmpty(m.coolerItemId) && !m.cpuRetentionOpen)
                {
                    GameObject paste = Box("PastePrompt", cpuP + new Vector3(0, 0, -.045f), new Vector3(.10f, .10f, .012f), ghost);
                    Interact(paste, "Apply thermal paste", 38, () => game.ApplyThermalPaste());
                }
            }

            Vector3 coolP = new Vector3(-.02f, .13f, .045f);
            if (string.IsNullOrEmpty(m.coolerItemId))
            {
                if (string.IsNullOrEmpty(m.cpuItemId) || !m.thermalPasteApplied || m.cpuRetentionOpen) return;
                GameObject g = Box("CoolerGhost", coolP, new Vector3(.26f, .28f, .20f), ghost);
                AddSnap(g, PartCategory.Cooler);
                Interact(g, "Install CPU cooler", 31, () => game.InstallBestAvailable(PartCategory.Cooler));
                return;
            }

            HardwareDefinition cooler = Def(m.coolerItemId);
            if (HardwarePresentationLayout.IsAio(cooler))
            {
                GameObject pump = Cylinder("AIOPumpBlock", new Vector3(-.02f, .13f, .12f), new Vector3(.085f, .025f, .085f), Quaternion.Euler(90, 0, 0), black);
                InteractPart(pump, m.coolerItemId, "Remove liquid cooler");

                int fanCount = HardwarePresentationLayout.AioFanCount(cooler);
                float radWidth = Mathf.Clamp(.18f * fanCount + .06f, .28f, .62f);
                Vector3 rad = new Vector3(0, .305f, -.11f);
                Box("AIORadiator", rad, new Vector3(radWidth, .065f, .09f), black);
                for (int i = 0; i < fanCount; i++)
                {
                    float x = fanCount == 1 ? 0f : Mathf.Lerp(-radWidth * .34f, radWidth * .34f, i / (float)(fanCount - 1));
                    GameObject fan = Cylinder("AIOFan_" + i, rad + new Vector3(x, -.055f, 0), new Vector3(.07f, .012f, .07f), Quaternion.identity, silver);
                    fan.AddComponent<SpinVisual>().speed = 360f + i * 15f;
                }
                Cable("AIOTubeA", new Vector3(-.07f, .14f, .10f), rad + new Vector3(-radWidth * .30f, -.03f, 0), black, .016f);
                Cable("AIOTubeB", new Vector3(.03f, .11f, .10f), rad + new Vector3(radWidth * .30f, -.03f, 0), black, .016f);
            }
            else
            {
                GameObject heatsink = Box("CoolerHeatsink", coolP, new Vector3(.25f, .29f, .18f), silver);
                InteractPart(heatsink, m.coolerItemId, "Remove CPU cooler");
                for (int i = -3; i <= 3; i++)
                    Box("CoolerFin_" + (i + 3), coolP + new Vector3(0, i * .035f, -.105f), new Vector3(.27f, .009f, .055f), silver);
                GameObject fan = Cylinder("CoolerFan", coolP + new Vector3(0, 0, -.12f), new Vector3(.12f, .018f, .12f), Quaternion.Euler(90, 0, 0), black);
                fan.AddComponent<SpinVisual>().speed = 420f;
            }
        }

        private void BuildRam(MachineState m, Material ramMat, Material copper, Material chip, Material ghost)
        {
            HardwareDefinition board = Def(m.motherboardItemId);
            int slots = HardwarePresentationLayout.DimmSlotCount(board);
            int nextRecommended = HardwarePresentationLayout.NextRecommendedRamSlot(m, board);
            RamSlotRules.EnsureLatches(m, board);

            for (int slot = 0; slot < slots; slot++)
            {
                Vector3 p = RamSlotPosition(slot, slots);
                if (board != null)
                {
                    int latchSlot = slot;
                    RamLatchState latch = m.ramLatches[slot];
                    for (int end = 0; end < 2; end++)
                    {
                        bool top = end == 0;
                        bool open = top ? latch.topOpen : latch.bottomOpen;
                        float direction = top ? 1f : -1f;
                        GameObject clip = Box("DIMMLatch_" + slot + "_" + end,
                            p + new Vector3(0, direction * (open ? .206f : .181f), open ? .014f : -.012f),
                            new Vector3(.04f, .035f, .065f), open ? copper : chip);
                        clip.transform.localRotation = Quaternion.Euler(open ? direction * 35f : 0, 0, 0);
                        Interact(clip, (open ? "Close" : "Open") + " DIMM " + (slot + 1) + (top ? " top latch" : " bottom latch"), 48,
                            () => game.ToggleRamLatch(latchSlot, top));
                    }
                }
                int itemIndex = HardwarePresentationLayout.RamItemIndexAtSlot(m, slot, slots);
                if (itemIndex >= 0 && itemIndex < m.ramItemIds.Count)
                {
                    string id = m.ramItemIds[itemIndex];
                    GameObject r = Box("RAM_Slot" + slot, p, new Vector3(.032f, .33f, .085f), ramMat);
                    InteractPart(r, id, "Remove RAM module from DIMM " + (slot + 1));
                    for (int c = 0; c < 8; c++)
                        Box("RAMChip_" + slot + "_" + c, p + new Vector3(0, -.12f + c * .034f, -.048f), new Vector3(.036f, .022f, .012f), chip);
                    Box("RAMContacts_" + slot, p + new Vector3(0, -.17f, -.01f), new Vector3(.035f, .018f, .07f), copper);
                }
                else if (board != null)
                {
                    int selectedSlot = slot;
                    GameObject g = Box("RAMGhost_Slot" + slot, p, new Vector3(.028f, .33f, .045f), ghost);
                    AddSnap(g, PartCategory.RAM, slot);
                    string label = "Install RAM — DIMM " + (slot + 1) + (slot == nextRecommended ? " (recommended)" : "");
                    Interact(g, label, 28, () => game.InstallBestAvailable(PartCategory.RAM, selectedSlot));
                }
            }
        }

        private static Vector3 RamSlotPosition(int slot, int count)
        {
            float startX = .205f - (count - 1) * .0275f;
            return new Vector3(startX + slot * .055f, .07f, .145f);
        }

        private void BuildGpu(MachineState m, Material black, Material silver, Material ghost)
        {
            Vector3 p = new Vector3(-.02f, -.13f, .05f);
            if (string.IsNullOrEmpty(m.gpuItemId))
            {
                GameObject g = Box("GPUGhost", p, new Vector3(.58f, .13f, .17f), ghost);
                AddSnap(g, PartCategory.GPU);
                Interact(g, "Install graphics card", 30, () => game.InstallBestAvailable(PartCategory.GPU));
                return;
            }

            HardwareDefinition def = Def(m.gpuItemId);
            float length = HardwarePresentationLayout.GpuVisualLength(def);
            float thickness = Mathf.Lerp(.14f, .22f, Mathf.InverseLerp(1.8f, 4f, def?.gpuSlotWidth ?? 2f));
            GameObject gpu = Box("GPU", p, new Vector3(length, .13f, thickness), black);
            InteractPart(gpu, m.gpuItemId, "Remove graphics card");

            int fanCount = HardwarePresentationLayout.GpuFanCount(def);
            for (int i = 0; i < fanCount; i++)
            {
                float t = fanCount == 1 ? .5f : i / (float)(fanCount - 1);
                float x = Mathf.Lerp(-length * .34f, length * .34f, t);
                GameObject fan = Cylinder("GPUFan_" + i, p + new Vector3(x, 0, -thickness * .56f), new Vector3(.065f, .015f, .065f), Quaternion.Euler(90, 0, 0), silver);
                fan.AddComponent<SpinVisual>().speed = 300f + i * 25f;
            }
            Box("GPUBackplate", p + new Vector3(0, .075f, .01f), new Vector3(length * .96f, .025f, thickness * .92f), silver);
        }

        private void BuildPsu(MachineState m, Material black, Material silver, Material ghost)
        {
            Vector3 p = new Vector3(.25f, -.245f, -.13f);
            if (string.IsNullOrEmpty(m.psuItemId))
            {
                GameObject g = Box("PSUGhost", p, new Vector3(.32f, .20f, .30f), ghost);
                AddSnap(g, PartCategory.PSU);
                Interact(g, "Install power supply", 30, () => game.InstallBestAvailable(PartCategory.PSU));
                return;
            }

            HardwareDefinition def = Def(m.psuItemId);
            GameObject psu = Box("PSU", p, new Vector3(.32f, .20f, .30f), black);
            InteractPart(psu, m.psuItemId, "Remove power supply");
            Cylinder("PSUGrille", p + new Vector3(0, .11f, 0), new Vector3(.11f, .008f, .11f), Quaternion.identity, silver);
            GameObject rotor = Cylinder("PSUFan", p + new Vector3(0, .105f, 0), new Vector3(.085f, .006f, .085f), Quaternion.identity, black);
            rotor.AddComponent<SpinVisual>().speed = 220f;

            int ports = def != null && def.modularPsu ? 5 : 2;
            for (int i = 0; i < ports; i++)
            {
                int row = i / 3;
                int col = i % 3;
                Box("PSUPort_" + i, p + new Vector3(-.10f + col * .10f, .035f - row * .07f, -.16f), new Vector3(.065f, .045f, .012f), silver);
            }
        }

        private void BuildStorage(MachineState m, Material dark, Material pcb, Material silver, Material ghost)
        {
            int sataIndex = 0;
            int nvmeIndex = 0;
            for (int i = 0; i < m.storageItemIds.Count; i++)
            {
                string id = m.storageItemIds[i];
                HardwareDefinition def = Def(id);
                if (HardwarePresentationLayout.IsNvme(def))
                {
                    Vector3 p = new Vector3(-.10f + (nvmeIndex % 2) * .23f, -.06f - (nvmeIndex / 2) * .075f, .185f);
                    GameObject drive = Box("NVMe_" + nvmeIndex, p, new Vector3(.18f, .036f, .012f), pcb);
                    InteractPart(drive, id, "Remove NVMe drive");
                    Box("NVMeController_" + nvmeIndex, p + new Vector3(.035f, 0, -.010f), new Vector3(.035f, .026f, .010f), dark);
                    Box("NVMeLabel_" + nvmeIndex, p + new Vector3(-.035f, 0, -.011f), new Vector3(.055f, .028f, .008f), silver);
                    nvmeIndex++;
                }
                else
                {
                    Vector3 p = new Vector3(-.29f, -.25f + sataIndex * .085f, -.12f);
                    GameObject drive = Box("SATADrive_" + sataIndex, p, new Vector3(.23f, .060f, .16f), dark);
                    InteractPart(drive, id, "Remove SATA drive");
                    Box("DriveLabel_" + sataIndex, p + new Vector3(0, .034f, 0), new Vector3(.17f, .006f, .10f), silver);
                    sataIndex++;
                }
            }

            if (m.storageItemIds.Count < 4)
            {
                Vector3 p = new Vector3(-.29f, -.25f + Mathf.Min(sataIndex, 2) * .085f, -.12f);
                GameObject g = Box("StorageGhost", p, new Vector3(.23f, .050f, .16f), ghost);
                AddSnap(g, PartCategory.Storage);
                Interact(g, "Install compatible storage", 24, () => game.InstallBestAvailable(PartCategory.Storage));
            }
        }

        private void BuildFans(MachineState m, Material black, Material silver, Material ghost)
        {
            Vector3[] spots =
            {
                new Vector3(-.29f,.20f,-.27f), new Vector3(0,.20f,-.27f), new Vector3(.29f,.20f,-.27f),
                new Vector3(.33f,.16f,.255f), new Vector3(-.20f,.31f,.02f), new Vector3(.10f,.31f,.02f)
            };
            int visualSlots = Mathf.Min(spots.Length, HardwarePresentationLayout.CaseFanVisualSlots(m));
            for (int i = 0; i < visualSlots; i++)
            {
                bool top = i >= 4;
                if (i < m.fanItemIds.Count)
                {
                    string id = m.fanItemIds[i];
                    HardwareDefinition fanDef = Def(id);
                    float d = HardwarePresentationLayout.FanVisualDiameter(fanDef);
                    Vector3 frameScale = top ? new Vector3(d, .025f, d) : new Vector3(d, d, .025f);
                    GameObject frame = Box("CaseFanFrame_" + i, spots[i], frameScale, black);
                    InteractPart(frame, id, "Remove case fan");
                    Quaternion rot = top ? Quaternion.identity : Quaternion.Euler(90, 0, 0);
                    Vector3 rotorOffset = top ? new Vector3(0, -.02f, 0) : new Vector3(0, 0, -.02f);
                    GameObject rotor = Cylinder("CaseFanRotor_" + i, spots[i] + rotorOffset, new Vector3(d * .40f, .012f, d * .40f), rot, silver);
                    rotor.AddComponent<SpinVisual>().speed = 250f + i * 18f;
                }
                else if (i == 0)
                {
                    GameObject g = Box("FanGhost", spots[i], new Vector3(.20f, .20f, .018f), ghost);
                    AddSnap(g, PartCategory.Fan);
                    Interact(g, "Install case fan", 20, () => game.InstallBestAvailable(PartCategory.Fan));
                }
            }
        }

        private void BuildCables(MachineState m)
        {
            Color[] palette =
            {
                new Color(.025f,.025f,.03f), new Color(.70f,.05f,.045f), new Color(.05f,.28f,.72f),
                new Color(.82f,.78f,.66f), new Color(.38f,.10f,.58f)
            };
            int idx = Mathf.Clamp(m.customization?.cableColorIndex ?? 0, 0, palette.Length - 1);
            Material cable = Mat(palette[idx], .25f, .08f);
            Material port = Mat(new Color(.18f, .20f, .22f), .40f, .45f);

            CableSocket(m, CableCircuit.Atx24, new Vector3(.28f,-.15f,-.08f), new Vector3(.30f,.05f,.17f), cable, port);
            CableSocket(m, CableCircuit.CpuEps, new Vector3(.25f,-.08f,-.02f), new Vector3(-.18f,.24f,.18f), cable, port);
            CableSocket(m, CableCircuit.GpuPower, new Vector3(.25f,-.13f,-.11f), new Vector3(.15f,-.13f,.02f), cable, port);
            CableSocket(m, CableCircuit.SataData, new Vector3(-.22f,-.25f,-.12f), new Vector3(.05f,-.08f,.19f), cable, port);
            CableSocket(m, CableCircuit.SataPower, new Vector3(.22f,-.27f,-.08f), new Vector3(-.25f,-.25f,-.12f), cable, port);
            CableSocket(m, CableCircuit.FrontPanel, new Vector3(.37f,.22f,-.28f), new Vector3(.23f,-.23f,.17f), cable, port);
            CableSocket(m, CableCircuit.CpuFan, new Vector3(-.02f,.12f,.06f), new Vector3(-.07f,.27f,.18f), cable, port);
            CableSocket(m, CableCircuit.Pump, new Vector3(-.05f,.10f,.09f), new Vector3(.02f,.27f,.18f), cable, port);
            CableSocket(m, CableCircuit.Rgb, new Vector3(.25f,.28f,-.20f), new Vector3(.27f,-.16f,.18f), cable, port);
        }

        private void CableSocket(MachineState machine, CableCircuit circuit, Vector3 source, Vector3 destination, Material cable, Material port)
        {
            if (!game.Cabling.Present(machine, circuit)) return;
            bool connected = CableConnectionService.Connected(machine, circuit);
            if (connected) Cable(circuit.ToString(), source, destination, cable, .018f);
            GameObject connector = Box(circuit + "Connector", destination, new Vector3(.055f,.045f,.055f), port);
            Interact(connector, (connected ? "Disconnect " : "Connect ") + CableConnectionService.Label(circuit), 47, () => game.ToggleCable(circuit));
        }

        private void BuildPanelAndFasteners(MachineState m, Material frame, Material silver)
        {
            game.Assembly.EnsureCaseHardware(m);
            if (m.sidePanelInstalled)
            {
                HardwareDefinition pcCase = Def(m.caseItemId);
                bool glassCase = pcCase?.tags?.Any(t => t == "tempered-glass" || t == "glass") == true;
                Material panelMat = glassCase
                    ? GlassMat(Color.Lerp(new Color(.10f, .14f, .16f, .30f), game.Customization.CurrentColor(m), .10f))
                    : frame;
                GameObject panel = Box("SidePanel", new Vector3(.472f, 0, 0), new Vector3(.025f, .69f, .55f), panelMat);
                Renderer pr = panel.GetComponent<Renderer>();
                if (glassCase && pr != null) pr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                Interact(panel, "Side panel — loosen fasteners before removal", 8, () => game.ToggleSidePanel());

                if (m.sidePanel.fasteners != null)
                {
                    for (int i = 0; i < m.sidePanel.fasteners.Count; i++)
                    {
                        FastenerState f = m.sidePanel.fasteners[i];
                        float y = i < 2 ? .29f : -.29f;
                        float z = i % 2 == 0 ? .235f : -.235f;
                        Material stateMat = f.tightness > .05f ? silver : Mat(new Color(.68f, .18f, .08f), .45f, .65f);
                        GameObject screw = Cylinder("PanelScrew_" + f.fastenerId, new Vector3(.495f, y, z), new Vector3(.025f, .012f, .025f), Quaternion.Euler(0, 0, 90), stateMat);
                        Interact(screw, "Loosen panel fastener " + f.fastenerId, 55, () => game.LoosenFastener());
                        WorldInteractable screwI = screw.GetComponent<WorldInteractable>();
                        screwI.holdSeconds = .32f;
                        screwI.requiredToolId = "tool_driver";
                    }
                }
            }
            else
            {
                GameObject edge = CreateGhostFrame("PanelMount", new Vector3(.472f, 0, 0), new Vector3(.02f, .69f, .55f), frame);
                Interact(edge, "Fit side panel", 18, () => game.ToggleSidePanel());
            }
        }

        private void BuildPowerButton(MachineState m)
        {
            Material led = Mat(m.bootState == BootState.Off ? new Color(.16f, .18f, .20f) : new Color(.06f, .75f, .38f), .65f, .3f);
            if (led.HasProperty("_EmissionColor"))
            {
                led.EnableKeyword("_EMISSION");
                led.SetColor("_EmissionColor", led.color * 2f);
            }
            GameObject p = Cylinder("PowerButton", new Vector3(.32f, .27f, -.325f), new Vector3(.035f, .012f, .035f), Quaternion.Euler(90, 0, 0), led);
            bool powered = m.bootState != BootState.Off;
            Interact(p, powered ? "Power off for service" : "Power on / run POST", 50, () => { if (powered) game.PowerOff(); else game.PowerOn(); });
        }

        private void AddSnap(GameObject go, PartCategory category, int ramSlot = -1)
        {
            AssemblySnapPoint p = go.GetComponent<AssemblySnapPoint>() ?? go.AddComponent<AssemblySnapPoint>();
            if (!p.accepts.Contains(category)) p.accepts.Add(category);
            p.machineId = game.ActiveMachine?.machineId;
            p.ramSlot = ramSlot;
        }

        private void InteractPart(GameObject go, string itemId, string label)
        {
            Interact(go, label, 44, () => game.Remove(itemId));
        }

        private static void Interact(GameObject go, string label, int priority, Action action)
        {
            WorldInteractable i = go.GetComponent<WorldInteractable>() ?? go.AddComponent<WorldInteractable>();
            i.label = label;
            i.priority = priority;
            i.maxDistance = 3.5f;
            i.action = action;
        }

        private GameObject CreateGhostFrame(string name, Vector3 p, Vector3 s, Material m)
        {
            GameObject holder = new GameObject(name);
            holder.transform.SetParent(root.transform, false);
            holder.transform.localPosition = p;
            float t = .025f;
            Vector3 hs = s * .5f;
            BoxLocal(holder.transform, "GTop", new Vector3(0, hs.y, 0), new Vector3(s.x, t, t), m);
            BoxLocal(holder.transform, "GBottom", new Vector3(0, -hs.y, 0), new Vector3(s.x, t, t), m);
            BoxLocal(holder.transform, "GL", new Vector3(-hs.x, 0, 0), new Vector3(t, s.y, t), m);
            BoxLocal(holder.transform, "GR", new Vector3(hs.x, 0, 0), new Vector3(t, s.y, t), m);
            BoxCollider c = holder.AddComponent<BoxCollider>();
            c.size = s;
            return holder;
        }

        private GameObject Box(string name, Vector3 localPos, Vector3 scale, Material mat)
        {
            return BoxLocal(root.transform, name, localPos, scale, mat);
        }

        private GameObject BoxLocal(Transform parent, string name, Vector3 localPos, Vector3 scale, Material mat)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.name = name;
            g.transform.SetParent(parent, false);
            g.transform.localPosition = localPos;
            g.transform.localScale = scale;
            Renderer r = g.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = mat;
            return g;
        }

        private GameObject Cylinder(string name, Vector3 localPos, Vector3 scale, Quaternion rot, Material mat)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            g.name = name;
            g.transform.SetParent(root.transform, false);
            g.transform.localPosition = localPos;
            g.transform.localScale = scale;
            g.transform.localRotation = rot;
            Renderer r = g.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = mat;
            return g;
        }

        private void Cable(string name, Vector3 a, Vector3 b, Material mat, float thickness)
        {
            const int segments = 6;
            Vector3 control = (a + b) * .5f + Vector3.down * .055f + Vector3.forward * .018f;
            Vector3 previous = a;
            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                float u = 1f - t;
                Vector3 point = u * u * a + 2f * u * t * control + t * t * b;
                Vector3 d = point - previous;
                GameObject g = Box(name + "_Seg" + i, (previous + point) * .5f, new Vector3(thickness, thickness, Mathf.Max(.001f, d.magnitude)), mat);
                if (d.sqrMagnitude > .000001f) g.transform.localRotation = Quaternion.LookRotation(d.normalized, Vector3.up);
                Collider c = g.GetComponent<Collider>();
                if (c != null) c.enabled = false;
                previous = point;
            }
        }

        private void CablePort(string name, Vector3 p, Material mat, string label)
        {
            GameObject g = Box(name, p, new Vector3(.07f, .07f, .04f), mat);
            Interact(g, label, 25, () => game.ConnectCables());
        }

        private Material Mat(Color c, float smooth, float metallic = 0f)
        {
            Material m = new Material(shader);
            m.color = c;
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smooth);
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
            materials.Add(m);
            return m;
        }

        private Material GlassMat(Color tint)
        {
            tint.a = Mathf.Clamp(tint.a <= 0f ? .30f : tint.a, .20f, .42f);
            Material m = Mat(tint, .92f, .08f);
            if (m.HasProperty("_Surface")) m.SetFloat("_Surface", 1f);
            if (m.HasProperty("_ZWrite")) m.SetFloat("_ZWrite", 0f);
            if (m.HasProperty("_SrcBlend")) m.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            if (m.HasProperty("_DstBlend")) m.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            return m;
        }

        private void DestroyVisual()
        {
            if (root != null) Destroy(root);
            root = null;
            foreach (Material m in materials) if (m != null) Destroy(m);
            materials.Clear();
        }
    }

    public sealed class SpinVisual : MonoBehaviour
    {
        public float speed = 240f;

        private void Update()
        {
            GameRuntime g = GameRuntime.Instance;
            if (g?.State?.settings?.reducedMotion == true) return;
            MachineState m = g?.ActiveMachine;
            if (m == null || m.bootState == BootState.Off) return;

            float factor;
            if (m.bootState == BootState.Posting) factor = .42f;
            else
            {
                float hottest = Mathf.Max(m.cpuTempC, m.gpuTempC);
                factor = Mathf.Lerp(.28f, 1.15f, Mathf.InverseLerp(32f, 95f, hottest));
                if (m.bios != null)
                {
                    if (m.bios.fanProfile == 0) factor *= .72f;
                    else if (m.bios.fanProfile == 2) factor *= 1.18f;
                }
            }
            transform.Rotate(0, 0, speed * Mathf.Clamp(factor, .15f, 1.35f) * Time.unscaledDeltaTime, Space.Self);
        }
    }
}
