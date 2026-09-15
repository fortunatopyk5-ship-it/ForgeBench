using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Presentation adapter that upgrades the current primitive cooling visuals without replacing
    /// gameplay colliders, interactables, inventory, sockets or save state. It is safe to remove
    /// once authored prefabs are available.
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

            string signature = BuildSignature(game);
            if (machineRoot != observedMachineRoot || signature != lastSignature)
            {
                observedMachineRoot = machineRoot;
                lastSignature = signature;
                Rebuild(game, machineRoot);
            }
        }

        private static string BuildSignature(GameRuntime game)
        {
            MachineState m = game.ActiveMachine;
            if (m == null) return "none";
            string s = (m.coolerItemId ?? string.Empty) + "|" + string.Join(",", m.fanItemIds ?? new List<string>());
            s += "|" + (m.gpuItemId ?? string.Empty) + "|" + (m.customization?.themeR ?? 0f).ToString("0.00");
            s += ":" + (m.customization?.themeG ?? 0f).ToString("0.00") + ":" + (m.customization?.themeB ?? 0f).ToString("0.00");
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
            Color rgbColor = game.Customization != null && game.ActiveMachine != null
                ? game.Customization.CurrentColor(game.ActiveMachine)
                : new Color(.08f, .62f, .92f);
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
            ItemInstance item = game.Inventory.Get(m.coolerItemId);
            HardwareDefinition def = game.Inventory.Def(item);
            if (def == null) return;

            Transform coarse = FindChild(machineRoot, "CoolerHeatsink");
            Transform coarseFan = FindChild(machineRoot, "CoolerFan");
            if (coarse != null) HideRendererOnly(coarse.gameObject);
            if (coarseFan != null) HideRendererOnly(coarseFan.gameObject);
            HideNamedPrefix(machineRoot, "CoolerFin");

            Vector3 pumpPosition = coarse != null ? coarse.localPosition : new Vector3(-.02f, .13f, .045f);
            bool rgb = ProceduralCoolingVisuals.HasRgb(def);
            if (ProceduralCoolingVisuals.IsAio(def))
            {
                int radiatorMm = ProceduralCoolingVisuals.ResolveRadiatorMm(def);
                // Top-mounted within the existing oversized gameplay case. The scale follows the
                // established interaction scene rather than pretending case.lengthMm is an exterior size.
                Vector3 radiatorPosition = new Vector3(0f, .300f, .035f);
                Quaternion radiatorRotation = Quaternion.Euler(90f, 0f, 0f);
                ProceduralCoolingVisuals.BuildAio(generatedRoot.transform, "FB_AIO_" + radiatorMm,
                    pumpPosition, radiatorPosition, radiatorRotation, radiatorMm,
                    graphite, aluminium, darkMetal, rubber, accent, rgb, false);
            }
            else
            {
                ProceduralCoolingVisuals.BuildAirCooler(generatedRoot.transform, "FB_AirCooler", pumpPosition,
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
                if (oldRotor != null) HideRendererOnly(oldRotor.gameObject);

                HardwareDefinition def = game.Inventory.Def(game.Inventory.Get(m.fanItemIds[i]));
                int mm = ProceduralCoolingVisuals.ResolveFanMm(def);
                float size = mm == 140 ? .242f : .216f;
                bool rgb = ProceduralCoolingVisuals.HasRgb(def);
                ProceduralCoolingVisuals.BuildFan(generatedRoot.transform, "FB_CaseFan_" + i,
                    frame.localPosition, frame.localRotation, size,
                    graphite, darkMetal, aluminium, accent, rgb, 265f + i * 18f, false);
            }
        }

        private void UpgradeGpuFans(GameRuntime game, Transform machineRoot, Material graphite, Material darkMetal,
            Material aluminium, Material accent)
        {
            if (string.IsNullOrEmpty(game.ActiveMachine.gpuItemId)) return;
            for (int i = -1; i <= 1; i++)
            {
                // The current renderer creates three generic GPU fan cylinders. Replace only their
                // visible mesh; the GPU body/interactable remains untouched.
                Transform fan = FindNthByName(machineRoot, "GPUFan", i + 1);
                if (fan == null) continue;
                HideRendererOnly(fan.gameObject);
                ProceduralCoolingVisuals.BuildFan(generatedRoot.transform, "FB_GPUFan_" + (i + 1),
                    fan.localPosition, fan.localRotation, .145f,
                    graphite, darkMetal, aluminium, accent, false, 315f + (i + 1) * 17f, false);
            }
        }

        private Material MakeMaterial(Color color, float smoothness, float metallic, bool emissive = false)
        {
            Material m = new Material(shader);
            m.color = color;
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
            foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true))
                if (r != null && r.gameObject.name.StartsWith(prefix)) r.enabled = false;
        }

        private static Transform FindChild(Transform root, string exactName)
        {
            if (root == null) return null;
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in all) if (t.name == exactName) return t;
            return null;
        }

        private static Transform FindNthByName(Transform root, string exactName, int index)
        {
            if (root == null) return null;
            int seen = 0;
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in all)
            {
                if (t.name != exactName) continue;
                if (seen == index) return t;
                seen++;
            }
            return null;
        }
    }
}
