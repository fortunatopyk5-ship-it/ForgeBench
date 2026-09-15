using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>
    /// Physical pickup / inspect / align / snap workflow for inventory components.
    /// Authoritative installation still goes through GameRuntime and CompatibilityService.
    /// </summary>
    public sealed class ObjectHandlingController : MonoBehaviour
    {
        private GameRuntime game;
        private FirstPersonController player;
        private Camera cam;
        private GameObject trayRoot;
        private PickupPartProxy held;
        private Vector3 heldEuler;
        private float holdDistance = .85f;
        private bool dirty = true;
        private string traySignature = "";
        private Canvas ui;
        private Text heldLabel;
        private Text snapStatus;
        private Image controlsImage;
        private GameObject controls;
        private Shader shader;
        private readonly List<Material> trayMaterials = new List<Material>();
        private AssemblySnapPoint previewPoint;
        private SnapPreviewResult previewResult;
        private string previewReason = "";
        private SnapPreviewState lastPreviewState = SnapPreviewState.None;

        private static readonly Color PanelNeutral = new Color(.035f, .045f, .058f, .94f);
        private static readonly Color PanelValid = new Color(.035f, .115f, .105f, .96f);
        private static readonly Color PanelWarn = new Color(.13f, .09f, .035f, .96f);
        private static readonly Color PanelError = new Color(.14f, .04f, .045f, .96f);

        private IEnumerator Start()
        {
            game = GameRuntime.Instance;
            shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            while ((player = FindAnyObjectByType<FirstPersonController>()) == null) yield return null;
            cam = player.viewCamera;
            BuildUI();
            game.Events?.Subscribe("state.changed", OnStateChanged);
            dirty = true;
        }

        private void OnDestroy()
        {
            game?.Events?.Unsubscribe("state.changed", OnStateChanged);
            ClearPreview();
            if (trayRoot != null) Destroy(trayRoot);
            ClearTrayMaterials();
            if (ui != null) Destroy(ui.gameObject);
        }

        private void OnStateChanged(object payload) { dirty = true; }

        private void Update()
        {
            if (game == null || cam == null) return;
            string sig = TraySignature();
            if (dirty || sig != traySignature)
            {
                dirty = false;
                traySignature = sig;
                RebuildTray();
            }
            if (held == null) return;

            float dt = Time.unscaledDeltaTime;
            if (Input.GetKey(KeyCode.Q)) heldEuler.y -= 95f * dt;
            if (Input.GetKey(KeyCode.R)) heldEuler.y += 95f * dt;
            if (Input.GetKey(KeyCode.Z)) heldEuler.z -= 95f * dt;
            if (Input.GetKey(KeyCode.X)) heldEuler.z += 95f * dt;
            if (Input.GetKey(KeyCode.T)) heldEuler.x -= 95f * dt;
            if (Input.GetKey(KeyCode.G)) heldEuler.x += 95f * dt;
            if (Input.mouseScrollDelta.y != 0)
                holdDistance = Mathf.Clamp(holdDistance + Input.mouseScrollDelta.y * .08f, .42f, 1.25f);

            Vector3 target = cam.transform.position + cam.transform.forward * holdDistance + cam.transform.right * .08f - cam.transform.up * .05f;
            held.transform.position = Vector3.Lerp(held.transform.position, target, 1f - Mathf.Exp(-16f * dt));
            held.transform.rotation = Quaternion.Slerp(held.transform.rotation, cam.transform.rotation * Quaternion.Euler(heldEuler), 1f - Mathf.Exp(-14f * dt));
            UpdateSnapPreview();

            if (Input.GetKeyDown(KeyCode.F)) Drop();
        }

        public void BeginPickup(PickupPartProxy proxy)
        {
            if (proxy == null || held != null) return;
            ItemInstance item = game.Inventory.Get(proxy.instanceId);
            if (item == null || item.reserved)
            {
                game.Notify("That component is no longer available.", false);
                dirty = true;
                return;
            }

            held = proxy;
            proxy.SetHeld(true);
            heldEuler = Vector3.zero;
            holdDistance = .82f;
            controls.SetActive(true);
            RefreshHeldLabel();
            ClearPreview();
            UpdateSnapStatus(SnapPreviewState.None, "Move the component near a compatible installation point.");
        }

        public void RotateLeft() { if (held != null) heldEuler.y -= 22.5f; }
        public void RotateRight() { if (held != null) heldEuler.y += 22.5f; }
        public void RotateRoll() { if (held != null) heldEuler.z += 22.5f; }
        public void RotatePitch() { if (held != null) heldEuler.x += 22.5f; }
        public void Inspect() { if (held != null) holdDistance = holdDistance < .62f ? 1.0f : .50f; }

        public void Drop()
        {
            if (held == null) return;
            PickupPartProxy proxy = held;
            SocketCandidate candidate = FindBestSocket(proxy);

            if (candidate.point != null && candidate.result.state == SnapPreviewState.Incompatible)
            {
                game.Notify(string.IsNullOrEmpty(candidate.reason) ? "This component cannot be installed here." : candidate.reason, false);
                UpdateSnapStatus(candidate.result.state, candidate.reason);
                return;
            }
            if (candidate.point != null && candidate.result.state == SnapPreviewState.WrongOrientation)
            {
                game.Notify("Rotate the component until it aligns with the installation point.", false);
                UpdateSnapStatus(candidate.result.state, "Orientation error " + Mathf.RoundToInt(candidate.result.orientationError) + "°");
                return;
            }
            if (candidate.point != null && candidate.result.CanSnap)
            {
                CommitSnap(proxy, candidate.point);
                return;
            }

            held = null;
            controls.SetActive(false);
            ClearPreview();
            proxy.Recover();
            game.Notify("Component returned to the parts tray.");
        }

        private void CommitSnap(PickupPartProxy proxy, AssemblySnapPoint point)
        {
            ItemInstance item = game.Inventory.Get(proxy.instanceId);
            if (item == null)
            {
                held = null;
                controls.SetActive(false);
                ClearPreview();
                proxy.Recover();
                return;
            }

            held = null;
            controls.SetActive(false);
            ClearPreview();
            proxy.transform.position = point.transform.position;
            proxy.transform.rotation = point.transform.rotation;
            game.Install(proxy.instanceId);
            if (item.reserved) Destroy(proxy.gameObject);
            else proxy.Recover();
        }

        private void UpdateSnapPreview()
        {
            if (held == null) { ClearPreview(); return; }
            SocketCandidate candidate = FindBestSocket(held);
            if (candidate.point != previewPoint)
            {
                if (previewPoint != null) previewPoint.SetPreview(SnapPreviewState.None);
                previewPoint = candidate.point;
            }

            previewResult = candidate.result;
            previewReason = candidate.reason ?? "";
            if (previewPoint != null) previewPoint.SetPreview(previewResult.state);

            string details;
            if (previewPoint == null)
            {
                details = "Move near a compatible installation point";
            }
            else
            {
                string socket = string.IsNullOrEmpty(previewPoint.socketLabel) ? previewPoint.gameObject.name : previewPoint.socketLabel;
                details = socket + "  •  " + Mathf.RoundToInt(previewResult.distance * 100f) + " cm";
                if (previewPoint.requireOrientation)
                    details += "  •  " + Mathf.RoundToInt(previewResult.orientationError) + "°";
                if (previewResult.state == SnapPreviewState.Incompatible && !string.IsNullOrEmpty(previewReason))
                    details = previewReason;
            }
            UpdateSnapStatus(previewResult.state, details);
        }

        private SocketCandidate FindBestSocket(PickupPartProxy proxy)
        {
            SocketCandidate none = new SocketCandidate
            {
                point = null,
                result = new SnapPreviewResult(SnapPreviewState.None, 0f, 0f, float.MaxValue),
                reason = ""
            };
            ItemInstance item = game.Inventory.Get(proxy.instanceId);
            MachineState machine = game.ActiveMachine;
            HardwareDefinition def = item == null ? null : game.Inventory.Def(item);
            if (item == null || machine == null || def == null) return none;

            ActionResult compatible = PhysicalCompatibility(machine, item, def);
            AssemblySnapPoint[] points = FindObjectsByType<AssemblySnapPoint>(FindObjectsSortMode.None);
            AssemblySnapPoint best = null;
            SnapPreviewResult bestResult = none.result;
            float selectionScore = float.MaxValue;

            foreach (AssemblySnapPoint p in points)
            {
                if (p == null || !p.accepts.Contains(def.category)) continue;
                float distance = Vector3.Distance(proxy.transform.position, p.transform.position);
                if (!AssemblyInteractionRules.IsWorthPreviewing(distance, p.snapRadius)) continue;
                float orientation = p.requireOrientation
                    ? AssemblyInteractionRules.OrientationError(proxy.transform.rotation, p.transform.rotation)
                    : 0f;
                SnapPreviewResult result = AssemblyInteractionRules.Evaluate(
                    distance, p.snapRadius, orientation, p.orientationTolerance, compatible.ok, p.requireOrientation);
                float choose = distance + orientation / 180f * .18f;
                if (choose < selectionScore)
                {
                    selectionScore = choose;
                    best = p;
                    bestResult = result;
                }
            }

            return new SocketCandidate { point = best, result = bestResult, reason = compatible.ok ? "" : compatible.message };
        }

        private ActionResult PhysicalCompatibility(MachineState machine, ItemInstance item, HardwareDefinition def)
        {
            ActionResult compatibility = game.Compatibility.CanInstall(machine, item);
            if (!compatibility.ok) return compatibility;
            if (def.category != PartCategory.Case && !game.Assembly.InternalsAccessible(machine))
                return ActionResult.Fail("Open the side panel before installing internal components.");
            if (def.category == PartCategory.Case && (!string.IsNullOrEmpty(machine.motherboardItemId) ||
                                                     !string.IsNullOrEmpty(machine.cpuItemId) ||
                                                     machine.ramItemIds.Count > 0 ||
                                                     !string.IsNullOrEmpty(machine.gpuItemId)))
                return ActionResult.Fail("Remove internal components before replacing the case.");
            return ActionResult.Success("Compatible.");
        }

        private void ClearPreview()
        {
            if (previewPoint != null) previewPoint.SetPreview(SnapPreviewState.None);
            previewPoint = null;
            previewResult = new SnapPreviewResult(SnapPreviewState.None, 0f, 0f, float.MaxValue);
            previewReason = "";
            lastPreviewState = SnapPreviewState.None;
            if (controlsImage != null) controlsImage.color = PanelNeutral;
        }

        private void UpdateSnapStatus(SnapPreviewState state, string details)
        {
            if (snapStatus != null)
                snapStatus.text = AssemblyInteractionRules.StatusText(state) + "\n" + details;
            if (controlsImage != null)
            {
                controlsImage.color = state == SnapPreviewState.Valid ? PanelValid :
                    state == SnapPreviewState.WrongOrientation || state == SnapPreviewState.TooFar ? PanelWarn :
                    state == SnapPreviewState.Incompatible ? PanelError : PanelNeutral;
            }
            lastPreviewState = state;
        }

        private string TraySignature()
        {
            if (game?.State == null) return "none";
            MachineState m = game.ActiveMachine;
            string mid = m?.machineId ?? "none";
            IEnumerable<ItemInstance> items = game.State.inventory.Where(i => !i.reserved && !i.customerOwned)
                .OrderBy(i => i.instanceId).Take(18);
            return mid + "|" + string.Join("|", items.Select(i => i.instanceId));
        }

        private void RebuildTray()
        {
            if (held != null) return;
            if (trayRoot != null) Destroy(trayRoot);
            ClearTrayMaterials();

            trayRoot = new GameObject("PhysicalPartsTray");
            trayRoot.transform.position = new Vector3(-1.55f, 1.16f, 3.55f);
            Material tray = Mat(new Color(.055f, .065f, .075f), .35f, .72f);
            GameObject basePlate = Primitive(PrimitiveType.Cube, "PartsTray", trayRoot.transform, Vector3.zero, new Vector3(1.55f, .055f, .82f), tray);
            WorldInteractable wi = basePlate.AddComponent<WorldInteractable>();
            wi.label = "Physical parts tray";
            wi.priority = 2;
            wi.action = () => game.UI?.OpenTab("INVENTORY");

            List<ItemInstance> items = game.State.inventory.Where(i => !i.reserved && !i.customerOwned)
                .Where(i => game.Inventory.Def(i)?.category != PartCategory.Tool && game.Inventory.Def(i)?.category != PartCategory.Consumable)
                .OrderBy(i => i.instanceId).Take(12).ToList();
            for (int i = 0; i < items.Count; i++)
            {
                ItemInstance item = items[i];
                HardwareDefinition def = game.Inventory.Def(item);
                if (def == null) continue;
                int col = i % 4, row = i / 4;
                Vector3 p = new Vector3(-.56f + col * .38f, .10f, -.24f + row * .25f);
                GameObject visual = BuildProxyVisual(def, trayRoot.transform, p, i);
                PickupPartProxy proxy = visual.AddComponent<PickupPartProxy>();
                proxy.instanceId = item.instanceId;
                proxy.category = def.category;
                proxy.controller = this;
                proxy.CaptureHome();
                WorldInteractable it = visual.AddComponent<WorldInteractable>();
                it.label = "Pick up " + def.brand + " " + def.model;
                it.priority = 32;
                it.maxDistance = 3.2f;
                it.action = () => BeginPickup(proxy);
            }
        }

        private GameObject BuildProxyVisual(HardwareDefinition d, Transform parent, Vector3 p, int seed)
        {
            Material main = Mat(CategoryColor(d.category), .38f, .45f);
            Material metal = Mat(new Color(.45f, .48f, .52f), .62f, .8f);
            Material dark = Mat(new Color(.035f, .04f, .045f), .28f, .65f);
            Vector3 size = ProxySize(d);
            GameObject root = new GameObject("TrayPart_" + d.id);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = p;
            GameObject core = Primitive(PrimitiveType.Cube, "Body", root.transform, Vector3.zero, size, main);
            DisableCollider(core);
            BoxCollider hit = root.AddComponent<BoxCollider>();
            hit.size = size * 1.28f;

            if (d.category == PartCategory.GPU)
            {
                int fans = HardwarePresentationLayout.GpuFanCount(d);
                for (int i = 0; i < fans; i++)
                {
                    float x = fans == 1 ? 0f : Mathf.Lerp(-size.x * .34f, size.x * .34f, i / (float)(fans - 1));
                    GameObject fan = Primitive(PrimitiveType.Cylinder, "Fan_" + i, root.transform,
                        new Vector3(x, 0, -size.z * .56f), new Vector3(size.y * .32f, .010f, size.y * .32f), metal);
                    fan.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    DisableCollider(fan);
                }
            }
            else if (d.category == PartCategory.Fan)
            {
                GameObject fan = Primitive(PrimitiveType.Cylinder, "Rotor", root.transform, new Vector3(0, 0, -size.z * .56f),
                    new Vector3(size.x * .38f, .010f, size.x * .38f), metal);
                fan.transform.localRotation = Quaternion.Euler(90, 0, 0);
                DisableCollider(fan);
            }
            else if (d.category == PartCategory.Cooler && HardwarePresentationLayout.IsAio(d))
            {
                int fans = HardwarePresentationLayout.AioFanCount(d);
                GameObject radiator = Primitive(PrimitiveType.Cube, "Radiator", root.transform, new Vector3(0, .02f, .06f),
                    new Vector3(.08f + fans * .055f, .045f, .025f), dark);
                DisableCollider(radiator);
                GameObject pump = Primitive(PrimitiveType.Cylinder, "Pump", root.transform, new Vector3(0, -.035f, -.055f),
                    new Vector3(.035f, .014f, .035f), metal);
                pump.transform.localRotation = Quaternion.Euler(90, 0, 0);
                DisableCollider(pump);
            }
            else if (d.category == PartCategory.Motherboard)
            {
                int dimms = Mathf.Min(4, HardwarePresentationLayout.DimmSlotCount(d));
                for (int i = 0; i < dimms; i++)
                {
                    GameObject slot = Primitive(PrimitiveType.Cube, "DIMM_" + i, root.transform,
                        new Vector3(.04f + i * .018f, .015f, -size.z * .6f), new Vector3(.009f, .11f, .009f), dark);
                    DisableCollider(slot);
                }
                for (int i = 0; i < 3; i++)
                {
                    GameObject chip = Primitive(PrimitiveType.Cube, "Chip_" + i, root.transform,
                        new Vector3(-.07f + i * .07f, .04f, -size.z * .75f), new Vector3(.04f, .04f, .012f), dark);
                    DisableCollider(chip);
                }
            }
            else if (d.category == PartCategory.PSU)
            {
                GameObject fan = Primitive(PrimitiveType.Cylinder, "PSUFan", root.transform, new Vector3(0, size.y * .53f, 0),
                    new Vector3(size.x * .26f, .008f, size.x * .26f), metal);
                DisableCollider(fan);
            }
            else if (d.category == PartCategory.RAM)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject chip = Primitive(PrimitiveType.Cube, "MemoryChip_" + i, root.transform,
                        new Vector3(0, -.07f + i * .035f, -size.z * .55f), new Vector3(.038f, .022f, .008f), dark);
                    DisableCollider(chip);
                }
            }
            return root;
        }

        private static Vector3 ProxySize(HardwareDefinition d)
        {
            switch (d.category)
            {
                case PartCategory.Case:
                    return d.formFactor == "MiniITX" ? new Vector3(.19f, .17f, .15f) :
                           d.formFactor == "mATX" ? new Vector3(.22f, .19f, .16f) : new Vector3(.25f, .22f, .18f);
                case PartCategory.Motherboard:
                    return d.formFactor == "MiniITX" ? new Vector3(.18f, .18f, .025f) :
                           d.formFactor == "mATX" ? new Vector3(.22f, .18f, .025f) : new Vector3(.25f, .20f, .025f);
                case PartCategory.CPU: return new Vector3(.10f, .10f, .025f);
                case PartCategory.RAM: return new Vector3(.035f, .20f, .055f);
                case PartCategory.GPU:
                    return new Vector3(Mathf.Lerp(.20f, .31f, Mathf.InverseLerp(165f, 360f, Mathf.Max(165f, d.lengthMm))), .08f,
                        Mathf.Lerp(.09f, .14f, Mathf.InverseLerp(1.8f, 4f, d.gpuSlotWidth > 0 ? d.gpuSlotWidth : 2f)));
                case PartCategory.Storage:
                    return HardwarePresentationLayout.IsNvme(d) ? new Vector3(.18f, .045f, .025f) : new Vector3(.16f, .045f, .10f);
                case PartCategory.PSU: return new Vector3(.17f, .12f, .15f);
                case PartCategory.Cooler:
                    return HardwarePresentationLayout.IsAio(d) ? new Vector3(.18f, .12f, .12f) : new Vector3(.15f, .16f, .13f);
                case PartCategory.Fan:
                    float fd = HardwarePresentationLayout.FanVisualDiameter(d) * .62f;
                    return new Vector3(fd, fd, .035f);
                default: return new Vector3(.13f, .09f, .07f);
            }
        }

        private void BuildUI()
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            GameObject cg = new GameObject("ObjectHandlingUI");
            ui = cg.AddComponent<Canvas>();
            ui.renderMode = RenderMode.ScreenSpaceOverlay;
            ui.sortingOrder = 140;
            CanvasScaler sc = cg.AddComponent<CanvasScaler>();
            sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            sc.referenceResolution = new Vector2(1920, 1080);
            sc.matchWidthOrHeight = .5f;
            cg.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(cg);

            controls = new GameObject("HeldControls", typeof(RectTransform), typeof(Image));
            controls.transform.SetParent(cg.transform, false);
            RectTransform r = controls.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(.20f, .022f);
            r.anchorMax = new Vector2(.80f, .165f);
            r.offsetMin = r.offsetMax = Vector2.zero;
            controlsImage = controls.GetComponent<Image>();
            controlsImage.color = PanelNeutral;

            heldLabel = MakeText(controls.transform, font, "HOLDING COMPONENT", 17);
            RectTransform lr = heldLabel.rectTransform;
            lr.anchorMin = new Vector2(.018f, .48f);
            lr.anchorMax = new Vector2(.365f, .98f);
            lr.offsetMin = lr.offsetMax = Vector2.zero;
            heldLabel.verticalOverflow = VerticalWrapMode.Overflow;

            snapStatus = MakeText(controls.transform, font, "NO INSTALLATION POINT", 14);
            RectTransform sr = snapStatus.rectTransform;
            sr.anchorMin = new Vector2(.018f, .04f);
            sr.anchorMax = new Vector2(.365f, .46f);
            sr.offsetMin = sr.offsetMax = Vector2.zero;
            snapStatus.color = new Color(.72f, .82f, .90f, 1f);

            AddButton(controls.transform, font, "YAW −", .38f, .08f, .47f, .92f, RotateLeft);
            AddButton(controls.transform, font, "YAW +", .475f, .08f, .565f, .92f, RotateRight);
            AddButton(controls.transform, font, "ROLL", .57f, .08f, .66f, .92f, RotateRoll);
            AddButton(controls.transform, font, "PITCH", .665f, .08f, .755f, .92f, RotatePitch);
            AddButton(controls.transform, font, "INSPECT", .76f, .08f, .855f, .92f, Inspect);
            AddButton(controls.transform, font, "DROP / SNAP", .86f, .08f, .985f, .92f, Drop);
            controls.SetActive(false);
        }

        private void RefreshHeldLabel()
        {
            if (heldLabel == null || held == null) return;
            HardwareDefinition d = game.Inventory.Def(game.Inventory.Get(held.instanceId));
            heldLabel.text = d == null ? held.instanceId : d.brand + " " + d.model + "\n" + Describe(d);
        }

        private static string Describe(HardwareDefinition d)
        {
            switch (d.category)
            {
                case PartCategory.CPU:
                    return d.category + "  •  " + d.socket + "  •  " + d.coreCount + "C/" + d.threadCount + "T  •  " + Mathf.RoundToInt(d.powerWatts) + " W";
                case PartCategory.GPU:
                    return d.category + "  •  " + d.vramGB + " GB  •  " + Mathf.RoundToInt(d.lengthMm) + " mm  •  " + Mathf.RoundToInt(d.powerWatts) + " W";
                case PartCategory.RAM:
                    return d.memoryType + "  •  " + d.capacityGB + " GB  •  " + d.speed + " MT/s";
                case PartCategory.Storage:
                    return d.storageInterface + "  •  " + d.storageGB + " GB  •  " + d.readMBs + " MB/s";
                case PartCategory.PSU:
                    return d.category + "  •  " + d.psuWattage + " W  •  " + (d.modularPsu ? "Modular" : "Fixed cable");
                case PartCategory.Motherboard:
                    return d.formFactor + "  •  " + d.socket + "  •  " + d.memoryType + "  •  " + d.dimmSlots + " DIMM";
                case PartCategory.Cooler:
                    return HardwarePresentationLayout.IsAio(d) ? "AIO  •  " + d.radiatorSupportMm + " mm radiator" : "Air cooler  •  " + Mathf.RoundToInt(d.airflowCfm) + " CFM";
                case PartCategory.Fan:
                    return d.fanSizeMm + " mm fan  •  " + d.maxRpm + " RPM  •  " + Mathf.RoundToInt(d.airflowCfm) + " CFM";
                default:
                    return d.category + "  •  quality " + d.quality;
            }
        }

        private static Text MakeText(Transform p, Font f, string value, int size)
        {
            GameObject g = new GameObject("Text", typeof(RectTransform), typeof(Text));
            g.transform.SetParent(p, false);
            Text t = g.GetComponent<Text>();
            t.font = f;
            t.text = value;
            t.fontSize = size;
            t.alignment = TextAnchor.MiddleLeft;
            t.color = Color.white;
            return t;
        }

        private static void AddButton(Transform p, Font f, string label, float x0, float y0, float x1, float y1, UnityEngine.Events.UnityAction action)
        {
            GameObject g = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            g.transform.SetParent(p, false);
            RectTransform r = g.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(x0, y0);
            r.anchorMax = new Vector2(x1, y1);
            r.offsetMin = r.offsetMax = Vector2.zero;
            g.GetComponent<Image>().color = new Color(.10f, .14f, .18f, 1f);
            Button b = g.GetComponent<Button>();
            b.onClick.AddListener(action);
            Text t = MakeText(g.transform, f, label, 15);
            RectTransform tr = t.rectTransform;
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = tr.offsetMax = Vector2.zero;
            t.alignment = TextAnchor.MiddleCenter;
        }

        private GameObject Primitive(PrimitiveType type, string name, Transform parent, Vector3 localPos, Vector3 scale, Material mat)
        {
            GameObject g = GameObject.CreatePrimitive(type);
            g.name = name;
            g.transform.SetParent(parent, false);
            g.transform.localPosition = localPos;
            g.transform.localScale = scale;
            Renderer rr = g.GetComponent<Renderer>();
            if (rr != null) rr.sharedMaterial = mat;
            return g;
        }

        private Material Mat(Color c, float smooth, float metallic)
        {
            Material m = new Material(shader);
            m.color = c;
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smooth);
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
            trayMaterials.Add(m);
            return m;
        }

        private void ClearTrayMaterials()
        {
            foreach (Material m in trayMaterials) if (m != null) Destroy(m);
            trayMaterials.Clear();
        }

        private static void DisableCollider(GameObject g)
        {
            Collider c = g == null ? null : g.GetComponent<Collider>();
            if (c != null) Destroy(c);
        }

        private static Color CategoryColor(PartCategory c)
        {
            switch (c)
            {
                case PartCategory.CPU: return new Color(.66f, .42f, .16f);
                case PartCategory.Motherboard: return new Color(.04f, .32f, .17f);
                case PartCategory.RAM: return new Color(.09f, .38f, .68f);
                case PartCategory.GPU: return new Color(.17f, .18f, .21f);
                case PartCategory.Storage: return new Color(.32f, .35f, .38f);
                case PartCategory.PSU: return new Color(.08f, .09f, .10f);
                case PartCategory.Cooler: return new Color(.40f, .44f, .48f);
                case PartCategory.Fan: return new Color(.12f, .14f, .17f);
                default: return new Color(.16f, .22f, .28f);
            }
        }

        private struct SocketCandidate
        {
            public AssemblySnapPoint point;
            public SnapPreviewResult result;
            public string reason;

            public SocketCandidate(AssemblySnapPoint point, SnapPreviewResult result, string reason)
            {
                this.point = point;
                this.result = result;
                this.reason = reason;
            }

            public static implicit operator SocketCandidate((AssemblySnapPoint point, SnapPreviewResult result, string reason) value)
            {
                return new SocketCandidate(value.point, value.result, value.reason);
            }
        }
    }

    public sealed class PickupPartProxy : MonoBehaviour
    {
        public string instanceId;
        public PartCategory category;
        public ObjectHandlingController controller;
        private Vector3 homePos;
        private Quaternion homeRot;
        private Transform homeParent;
        private Collider hit;
        private WorldInteractable interaction;

        public void CaptureHome()
        {
            homePos = transform.localPosition;
            homeRot = transform.localRotation;
            homeParent = transform.parent;
            hit = GetComponent<Collider>();
            interaction = GetComponent<WorldInteractable>();
        }

        public void SetHeld(bool value)
        {
            if (value)
            {
                transform.SetParent(null, true);
                if (hit != null) hit.enabled = false;
                if (interaction != null) interaction.enabled = false;
            }
            else
            {
                if (hit != null) hit.enabled = true;
                if (interaction != null) interaction.enabled = true;
            }
        }

        public void Recover()
        {
            transform.SetParent(homeParent, false);
            transform.localPosition = homePos;
            transform.localRotation = homeRot;
            SetHeld(false);
        }
    }

    public sealed class AssemblySnapPoint : MonoBehaviour
    {
        public List<PartCategory> accepts = new List<PartCategory>();
        public float snapRadius = .55f;
        public float orientationTolerance = 85f;
        public bool requireOrientation = true;
        public string socketLabel = "";

        private Renderer[] renderers;
        private MaterialPropertyBlock block;
        private SnapPreviewState shown = SnapPreviewState.None;

        public void SetPreview(SnapPreviewState state)
        {
            if (shown == state) return;
            shown = state;
            if (renderers == null) renderers = GetComponentsInChildren<Renderer>(true);
            if (block == null) block = new MaterialPropertyBlock();

            Color color = state == SnapPreviewState.Valid ? new Color(.10f, .92f, .70f, 1f) :
                state == SnapPreviewState.WrongOrientation ? new Color(1f, .58f, .12f, 1f) :
                state == SnapPreviewState.Incompatible ? new Color(.95f, .12f, .15f, 1f) :
                state == SnapPreviewState.TooFar ? new Color(.12f, .55f, .95f, 1f) : Color.white;

            foreach (Renderer r in renderers)
            {
                if (r == null) continue;
                if (state == SnapPreviewState.None)
                {
                    r.SetPropertyBlock(null);
                    continue;
                }
                r.GetPropertyBlock(block);
                block.SetColor("_BaseColor", color);
                block.SetColor("_Color", color);
                block.SetColor("_EmissionColor", color * .65f);
                r.SetPropertyBlock(block);
            }
        }
    }
}
