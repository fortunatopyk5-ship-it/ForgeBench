using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Replaces only the visible primitive cooling meshes with higher-detail procedural geometry.
    /// Existing gameplay colliders, interactables, machine state and save data remain authoritative.
    /// </summary>
    public sealed class CoolingVisualUpgradeDirector : MonoBehaviour
    {
        private static bool installing;
        private GameObject observedMachineRoot;
        private GameObject generatedRoot;
        private string lastSignature = string.Empty;
        private float nextPoll;
        private readonly List<Material> ownedMaterials = new List<Material>();
        private Shader shader;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (installing) return;
            installing = true;
            GameObject host = new GameObject("ForgeBench_CoolingVisualUpgrade");
            DontDestroyOnLoad(host);
            host.AddComponent<CoolingVisualUpgradeDirector>();
        }

        private void Awake()
        {
            shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        }

        private void OnDestroy()
        {
            ClearGenerated();
        }

        private void LateUpdate()
        {
            if (Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + .20f;

            GameRuntime game = GameRuntime.Instance;
            if (game == null || game.ActiveMachine == null) return;
            GameObject machineRoot = GameObject.Find("ProductionMachine3D");
            if (machineRoot == null) return;

            string signature = BuildSignature(game.ActiveMachine);
            if (machineRoot != observedMachineRoot || signature != lastSignature)
            {
                observedMachineRoot = machineRoot;
                lastSignature = signature;
                Rebuild(game, machineRoot);
            }
        }

        private static string BuildSignature(MachineState m)
        {
            if (m == null) return "none";
            string s = (m.coolerItemId ?? string.Empty) + "|" + (m.gpuItemId ?? string.Empty);
            if (m.fanItemIds != null) s += "|" + string.Join(",", m.fanItemIds);
            s += "|" + (m.customization?.themeR ?? 0f).ToString("0.00") + ":" + (m.customization?.themeG ?? 0f).ToString("0.00") + ":" + (m.customization?.themeB ?? 0f).ToString("0.00");
            s += "|" + (m.customization?.rgbEffect ?? 0);
            return s;
        }

        private void Rebuild(GameRuntime game, GameObject machineRoot)
        {
            ClearGenerated();
            if (machineRoot == null || game.ActiveMachine == null) return;

            generatedRoot = new GameObject("ProceduralCoolingUpgrade");
            generatedRoot.transform.SetParent(machineRoot.transform, false);

            Material graphite = MakeMaterial(new Color(.035f, .042f, .050f), .30f, .70f);
            Material darkMetal = MakeMaterial(new Color(.080f, .090f, .100f), .42f, .84f);
            Material aluminium = MakeMaterial(new Color(.48f, .51f, .54f), .62f, .92f);
            Material copper = MakeMaterial(new Color(.54f, .20f, .075f), .48f, .86f);
            Material rubber = MakeMaterial(new Color(.018f, .021f, .024f), .18f, .02f);
            Color rgbColor = game.Customization != null ? game.Customization.CurrentColor(game.ActiveMachine) : new Color(.08f, .62f, .92f);
            Material accent = MakeMaterial(rgbColor, .72f, .18f, true);

            UpgradeCpuCooler(game, machineRoot.transform, graphite, darkMetal, aluminium, copper, rubber, accent);
            UpgradeCaseFans(game, machineRoot.transform, graphite, darkMetal, aluminium, accent);
            UpgradeGpuFans(game, machineRoot.transform, graphite, darkMetal, aluminium, accent);
        }

        private void UpgradeCpuCooler(GameRuntime game, Transform machineRoot, Material graphite, Material darkMetal,
            Material aluminium, Material copper, Material rubber, Material accent)
        {
            MachineState m = game.ActiveMachine;
            if (string.IsNullOrEmpty(m.coolerItemId)) return;
            HardwareDefinition def = game.Inventory.Def(game.Inventory.Get(m.coolerItemId));
            if (def == null) return;

            bool rgb = ProceduralCoolingVisuals.HasRgb(def);
            if (ProceduralCoolingVisuals.IsAio(def))
            {
                Transform pump = FindChild(machineRoot, "AIOPumpBlock");
                Transform radiator = FindChild(machineRoot, "AIORadiator");
                HideRendererOnly(pump != null ? pump.gameObject : null);
                HideRendererOnly(radiator != null ? radiator.gameObject : null);
                HideNamedPrefix(machineRoot, "AIOFan_");
                HideNamedPrefix(machineRoot, "AIOTube");

                Vector3 pumpPosition = pump != null ? pump.localPosition : new Vector3(-.02f, .13f, .12f);
                Vector3 radiatorPosition = radiator != null ? radiator.localPosition : new Vector3(0f, .305f, -.11f);
                int radiatorMm = ProceduralCoolingVisuals.ResolveRadiatorMm(def);
                ProceduralCoolingVisuals.BuildAio(generatedRoot.transform, "FB_AIO_" + radiatorMm,
                    pumpPosition, radiatorPosition, Quaternion.Euler(90f, 0f, 0f), radiatorMm,
                    graphite, aluminium, darkMetal, rubber, accent, rgb, false);
            }
            else
            {
                Transform coarse = FindChild(machineRoot, "CoolerHeatsink");
                Transform coarseFan = FindChild(machineRoot, "CoolerFan");
                HideRendererOnly(coarse != null ? coarse.gameObject : null);
                HideRendererOnly(coarseFan != null ? coarseFan.gameObject : null);
                HideNamedPrefix(machineRoot, "CoolerFin_");

                Vector3 position = coarse != null ? coarse.localPosition : new Vector3(-.02f, .13f, .045f);
                ProceduralCoolingVisuals.BuildAirCooler(generatedRoot.transform, "FB_AirCooler", position,
                    aluminium, graphite, darkMetal, copper, accent, rgb, false);
            }
        }

        private void UpgradeCaseFans(GameRuntime game, Transform machineRoot, Material graphite, Material darkMetal,
            Material aluminium, Material accent)
        {
            MachineState m = game.ActiveMachine;
            if (m.fanItemIds == null) return;
            for (int i = 0; i < m.fanItemIds.Count; i++)
            {
                Transform frame = FindChild(machineRoot, "CaseFanFrame_" + i);
                if (frame == null) continue;
                Transform oldRotor = FindChild(machineRoot, "CaseFanRotor_" + i);
                HideRendererOnly(frame.gameObject);
                HideRendererOnly(oldRotor != null ? oldRotor.gameObject : null);

                HardwareDefinition def = game.Inventory.Def(game.Inventory.Get(m.fanItemIds[i]));
                int mm = ProceduralCoolingVisuals.ResolveFanMm(def);
                float size = mm >= 140 ? .242f : .216f;
                bool rgb = ProceduralCoolingVisuals.HasRgb(def);
                Quaternion rotation = i >= 4 ? Quaternion.Euler(90f, 0f, 0f) : Quaternion.identity;
                ProceduralCoolingVisuals.BuildFan(generatedRoot.transform, "FB_CaseFan_" + i,
                    frame.localPosition, rotation, size,
                    graphite, darkMetal, aluminium, accent, rgb, 265f + i * 18f, false);
            }
        }

        private void UpgradeGpuFans(GameRuntime game, Transform machineRoot, Material graphite, Material darkMetal,
            Material aluminium, Material accent)
        {
            if (string.IsNullOrEmpty(game.ActiveMachine.gpuItemId)) return;
            HardwareDefinition gpu = game.Inventory.Def(game.Inventory.Get(game.ActiveMachine.gpuItemId));
            bool rgb = ProceduralCoolingVisuals.HasRgb(gpu);
            for (int i = 0; i < 4; i++)
            {
                Transform fan = FindChild(machineRoot, "GPUFan_" + i);
                if (fan == null) break;
                HideRendererOnly(fan.gameObject);
                ProceduralCoolingVisuals.BuildFan(generatedRoot.transform, "FB_GPUFan_" + i,
                    fan.localPosition, Quaternion.identity, .145f,
                    graphite, darkMetal, aluminium, accent, rgb, 315f + i * 17f, false);
            }
        }

        private Material MakeMaterial(Color color, float smoothness, float metallic, bool emissive = false)
        {
            Material m = new Material(shader);
            m.color = color;
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smoothness);
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
            if (emissive && m.HasProperty("_EmissionColor"))
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", color * 1.75f);
            }
            ownedMaterials.Add(m);
            return m;
        }

        private void ClearGenerated()
        {
            if (generatedRoot != null) Destroy(generatedRoot);
            generatedRoot = null;
            foreach (Material m in ownedMaterials) if (m != null) Destroy(m);
            ownedMaterials.Clear();
        }

        private static void HideRendererOnly(GameObject go)
        {
            Renderer r = go != null ? go.GetComponent<Renderer>() : null;
            if (r != null) r.enabled = false;
        }

        private static void HideNamedPrefix(Transform root, string prefix)
        {
            if (root == null) return;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
                if (r != null && r.gameObject.name.StartsWith(prefix)) r.enabled = false;
        }

        private static Transform FindChild(Transform root, string exactName)
        {
            if (root == null) return null;
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in all) if (t.name == exactName) return t;
            return null;
        }
    }
}
