using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Non-destructive presentation upgrade for the currently rendered machine. It replaces only
    /// coarse cooling renderers while leaving colliders, WorldInteractables, sockets and save state
    /// owned by the existing physical assembly systems. When quality drops to tier 0 the original
    /// renderers are restored automatically.
    /// </summary>
    public sealed class CoolingVisualUpgradeDirector : MonoBehaviour
    {
        private static bool installing;
        private readonly List<Material> ownedMaterials = new List<Material>();
        private readonly List<Renderer> hiddenRenderers = new List<Renderer>();
        private GameRuntime game;
        private GameObject observedMachineRoot;
        private GameObject generatedRoot;
        private Shader shader;
        private string lastSignature = string.Empty;
        private bool dirty = true;
        private float nextPoll;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (installing || FindAnyObjectByType<CoolingVisualUpgradeDirector>() != null) return;
            installing = true;
            GameObject host = new GameObject("ForgeBench_CoolingVisualUpgrade");
            DontDestroyOnLoad(host);
            host.AddComponent<CoolingVisualUpgradeDirector>();
        }

        private IEnumerator Start()
        {
            shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            while (GameRuntime.Instance == null || GameRuntime.Instance.State == null) yield return null;
            game = GameRuntime.Instance;
            game.Events?.Subscribe("state.changed", OnStateChanged);
            RuntimeRenderBudget.Changed += OnRenderBudgetChanged;
            dirty = true;
        }

        private void OnStateChanged(object payload) { dirty = true; }
        private void OnRenderBudgetChanged(int tier) { dirty = true; }

        private void LateUpdate()
        {
            if (game == null) game = GameRuntime.Instance;
            if (game?.ActiveMachine == null)
            {
                if (generatedRoot != null) ClearGenerated(true);
                observedMachineRoot = null;
                lastSignature = string.Empty;
                return;
            }

            if (!dirty && Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + .30f;

            GameObject machineRoot = GameObject.Find("ProductionMachine3D");
            if (machineRoot == null) return;

            string signature = BuildSignature(game);
            if (!dirty && machineRoot == observedMachineRoot && signature == lastSignature) return;

            dirty = false;
            observedMachineRoot = machineRoot;
            lastSignature = signature;
            Rebuild(game, machineRoot);
        }

        private static string BuildSignature(GameRuntime g)
        {
            MachineState m = g.ActiveMachine;
            if (m == null) return "none";
            string signature = (m.machineId ?? string.Empty) + "|" + (m.coolerItemId ?? string.Empty) + "|";
            signature += string.Join(",", m.fanItemIds ?? new List<string>()) + "|" + (m.gpuItemId ?? string.Empty) + "|" + (m.psuItemId ?? string.Empty);
            signature += "|" + (m.customization?.themeR ?? 0f).ToString("0.000") + ":" + (m.customization?.themeG ?? 0f).ToString("0.000") + ":" + (m.customization?.themeB ?? 0f).ToString("0.000");
            signature += "|fx=" + (m.customization?.rgbEffect ?? 0) + "|tier=" + RuntimeRenderBudget.CurrentTier;
            return signature;
        }

        private void Rebuild(GameRuntime g, GameObject machineRoot)
        {
            ClearGenerated(true);
            if (machineRoot == null || g.ActiveMachine == null) return;

            // Tier 0 deliberately keeps the cheap authoritative primitive presentation.
            if (RuntimeRenderBudget.CurrentTier <= 0) return;

            generatedRoot = new GameObject("ProceduralCoolingUpgrade");
            generatedRoot.transform.SetParent(machineRoot.transform, false);
            generatedRoot.AddComponent<ProceduralGeneratedMeshOwner>();
            DistanceCullHint cull = generatedRoot.AddComponent<DistanceCullHint>();
            cull.maxDistance = RuntimeRenderBudget.CurrentTier >= 3 ? 11f : 8.5f;

            Material graphite = MakeMaterial(new Color(.032f, .038f, .046f), .30f, .72f, false, ProceduralSurfaceKind.Graphite, 4.0f);
            Material darkMetal = MakeMaterial(new Color(.070f, .078f, .088f), .42f, .86f, false, ProceduralSurfaceKind.DarkMetal, 4.5f);
            Material aluminium = MakeMaterial(new Color(.50f, .53f, .56f), .64f, .92f, false, ProceduralSurfaceKind.BrushedMetal, 6.0f);
            Material copper = MakeMaterial(new Color(.56f, .21f, .07f), .48f, .86f, false, ProceduralSurfaceKind.Copper, 5.5f);
            Material rubber = MakeMaterial(new Color(.016f, .019f, .022f), .16f, .02f, false, ProceduralSurfaceKind.Rubber, 5.0f);
            Color rgbColor = g.Customization != null ? g.Customization.CurrentColor(g.ActiveMachine) : new Color(.08f, .62f, .92f);
            Material accent = MakeMaterial(rgbColor, .72f, .18f, true, null, 1f);

            UpgradeCpuCooler(g, machineRoot.transform, graphite, darkMetal, aluminium, copper, rubber, accent);
            UpgradeCaseFans(g, machineRoot.transform, graphite, darkMetal, aluminium, accent);

            if (RuntimeRenderBudget.CurrentTier >= 2)
            {
                UpgradeGpuFans(g, machineRoot.transform, graphite, darkMetal, aluminium, accent);
                UpgradePsuFan(g, machineRoot.transform, graphite, darkMetal, aluminium, accent);
            }
        }

        private void UpgradeCpuCooler(GameRuntime g, Transform machineRoot, Material graphite, Material darkMetal,
            Material aluminium, Material copper, Material rubber, Material accent)
        {
            MachineState m = g.ActiveMachine;
            if (string.IsNullOrEmpty(m.coolerItemId)) return;
            HardwareDefinition def = g.Inventory.Def(g.Inventory.Get(m.coolerItemId));
            if (def == null) return;

            bool rgb = RuntimeRenderBudget.CurrentTier >= 2 && ProceduralCoolingVisuals.HasRgb(def);
            if (ProceduralCoolingVisuals.IsAio(def))
            {
                Transform pump = FindChild(machineRoot, "AIOPumpBlock");
                Transform radiator = FindChild(machineRoot, "AIORadiator");
                if (pump == null || radiator == null) return;

                HideRendererOnly(pump.gameObject);
                HideRendererOnly(radiator.gameObject);
                HideNamedPrefix(machineRoot, "AIOFan_");
                HideNamedPrefix(machineRoot, "AIOTube");

                int radiatorMm = HardwarePresentationLayout.ResolveRadiatorMm(def);
                Vector3 pumpPosition = pump.localPosition;
                Vector3 radiatorPosition = radiator.localPosition;
                ProceduralCoolingVisuals.BuildAio(generatedRoot.transform, "FB_AIO_" + radiatorMm,
                    pumpPosition, radiatorPosition, Quaternion.Euler(90f, 0f, 0f), radiatorMm,
                    graphite, aluminium, darkMetal, rubber, accent, rgb, false);
                return;
            }

            Transform coarse = FindChild(machineRoot, "CoolerHeatsink");
            if (coarse == null) return;
            HideRendererOnly(coarse.gameObject);
            Transform coarseFan = FindChild(machineRoot, "CoolerFan");
            if (coarseFan != null) HideRendererOnly(coarseFan.gameObject);
            HideNamedPrefix(machineRoot, "CoolerFin_");

            ProceduralCoolingVisuals.BuildAirCooler(generatedRoot.transform, "FB_AirCooler", coarse.localPosition,
                aluminium, graphite, darkMetal, copper, accent, rgb, false);
        }

        private void UpgradeCaseFans(GameRuntime g, Transform machineRoot, Material graphite, Material darkMetal,
            Material aluminium, Material accent)
        {
            MachineState m = g.ActiveMachine;
            if (m.fanItemIds == null) return;
            for (int i = 0; i < m.fanItemIds.Count; i++)
            {
                Transform frame = FindChild(machineRoot, "CaseFanFrame_" + i);
                if (frame == null) continue;
                Transform oldRotor = FindChild(machineRoot, "CaseFanRotor_" + i);
                HideRendererOnly(frame.gameObject);
                if (oldRotor != null) HideRendererOnly(oldRotor.gameObject);

                HardwareDefinition def = g.Inventory.Def(g.Inventory.Get(m.fanItemIds[i]));
                float size = HardwarePresentationLayout.FanVisualDiameter(def);
                bool rgb = RuntimeRenderBudget.CurrentTier >= 2 && ProceduralCoolingVisuals.HasRgb(def);
                bool topMounted = frame.localScale.y < frame.localScale.z * .35f;
                Quaternion rotation = topMounted ? Quaternion.Euler(90f, 0f, 0f) : Quaternion.identity;

                ProceduralCoolingVisuals.BuildFan(generatedRoot.transform, "FB_CaseFan_" + i,
                    frame.localPosition, rotation, size,
                    graphite, darkMetal, aluminium, accent, rgb, 265f + i * 18f, false);
            }
        }

        private void UpgradeGpuFans(GameRuntime g, Transform machineRoot, Material graphite, Material darkMetal,
            Material aluminium, Material accent)
        {
            MachineState m = g.ActiveMachine;
            if (string.IsNullOrEmpty(m.gpuItemId)) return;
            HardwareDefinition gpu = g.Inventory.Def(g.Inventory.Get(m.gpuItemId));
            int fanCount = HardwarePresentationLayout.GpuFanCount(gpu);
            if (fanCount <= 0) return;

            float gpuLength = HardwarePresentationLayout.GpuVisualLength(gpu);
            float fanSize = Mathf.Clamp(gpuLength / Mathf.Max(2.7f, fanCount + .45f), .125f, .165f);
            bool rgb = ProceduralCoolingVisuals.HasRgb(gpu);
            for (int i = 0; i < fanCount; i++)
            {
                Transform fan = FindChild(machineRoot, "GPUFan_" + i);
                if (fan == null) continue;
                HideRendererOnly(fan.gameObject);
                ProceduralCoolingVisuals.BuildFan(generatedRoot.transform, "FB_GPUFan_" + i,
                    fan.localPosition, Quaternion.identity, fanSize,
                    graphite, darkMetal, aluminium, accent, rgb, 315f + i * 17f, false);
            }
        }

        private void UpgradePsuFan(GameRuntime g, Transform machineRoot, Material graphite, Material darkMetal,
            Material aluminium, Material accent)
        {
            if (string.IsNullOrEmpty(g.ActiveMachine.psuItemId)) return;
            Transform fan = FindChild(machineRoot, "PSUFan");
            if (fan == null) return;
            HideRendererOnly(fan.gameObject);
            ProceduralCoolingVisuals.BuildFan(generatedRoot.transform, "FB_PSUFan", fan.localPosition,
                Quaternion.Euler(90f, 0f, 0f), .18f,
                graphite, darkMetal, aluminium, accent, false, 220f, false);
        }

        private Material MakeMaterial(Color color, float smoothness, float metallic, bool emissive,
            ProceduralSurfaceKind? surfaceKind, float textureTiling)
        {
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material material = new Material(shader);
            material.color = color;
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (surfaceKind.HasValue) ProceduralSurfaceLibrary.ApplyToMaterial(material, surfaceKind.Value, textureTiling);
            if (emissive && material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                float strength = RuntimeRenderBudget.CurrentTier >= 3 ? 2.0f : 1.35f;
                material.SetColor("_EmissionColor", color * strength);
            }
            ownedMaterials.Add(material);
            return material;
        }

        private void HideRendererOnly(GameObject go)
        {
            Renderer renderer = go != null ? go.GetComponent<Renderer>() : null;
            if (renderer == null) return;
            if (!hiddenRenderers.Contains(renderer)) hiddenRenderers.Add(renderer);
            renderer.enabled = false;
        }

        private void HideNamedPrefix(Transform root, string prefix)
        {
            if (root == null) return;
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null || !renderer.gameObject.name.StartsWith(prefix)) continue;
                if (!hiddenRenderers.Contains(renderer)) hiddenRenderers.Add(renderer);
                renderer.enabled = false;
            }
        }

        private void ClearGenerated(bool restoreOriginals)
        {
            if (generatedRoot != null)
            {
                ProceduralGeneratedMeshOwner owner = generatedRoot.GetComponent<ProceduralGeneratedMeshOwner>();
                if (owner != null) owner.Release();
                Destroy(generatedRoot);
            }
            generatedRoot = null;

            if (restoreOriginals)
            {
                foreach (Renderer renderer in hiddenRenderers)
                    if (renderer != null) renderer.enabled = true;
            }
            hiddenRenderers.Clear();

            foreach (Material material in ownedMaterials)
                if (material != null) Destroy(material);
            ownedMaterials.Clear();
        }

        private static Transform FindChild(Transform root, string exactName)
        {
            if (root == null) return null;
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                if (child.name == exactName) return child;
            return null;
        }

        private void OnDestroy()
        {
            if (game != null) game.Events?.Unsubscribe("state.changed", OnStateChanged);
            RuntimeRenderBudget.Changed -= OnRenderBudgetChanged;
            ClearGenerated(true);
            installing = false;
        }
    }
}