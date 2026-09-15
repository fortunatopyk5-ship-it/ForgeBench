using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Applies subtle original procedural surface breakup to the current primitive machine renderer.
    /// It never changes gameplay objects, colliders or material colors, skips stateful/transparent
    /// renderers, and restores the authoritative materials whenever quality drops or the component is
    /// removed. Authored multi-material prefabs are intentionally left alone.
    /// </summary>
    public sealed class HardwareSurfaceUpgradeDirector : MonoBehaviour
    {
        private readonly Dictionary<Renderer, Material> originals = new Dictionary<Renderer, Material>();
        private readonly Dictionary<string, Material> materialCache = new Dictionary<string, Material>();
        private readonly List<Material> ownedMaterials = new List<Material>();
        private GameObject observedRoot;
        private bool dirty = true;
        private int appliedTier = -1;
        private float nextPoll;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<HardwareSurfaceUpgradeDirector>() != null) return;
            GameObject host = new GameObject("ForgeBench_HardwareSurfaceUpgrade");
            DontDestroyOnLoad(host);
            host.AddComponent<HardwareSurfaceUpgradeDirector>();
        }

        private void OnEnable() { RuntimeRenderBudget.Changed += OnRenderBudgetChanged; }
        private void OnDisable()
        {
            RuntimeRenderBudget.Changed -= OnRenderBudgetChanged;
            Restore();
        }

        private void OnRenderBudgetChanged(int tier) { dirty = true; }

        private void LateUpdate()
        {
            if (!dirty && Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + .35f;

            GameObject root = GameObject.Find("ProductionMachine3D");
            int tier = RuntimeRenderBudget.CurrentTier;
            if (!dirty && root == observedRoot && tier == appliedTier) return;

            dirty = false;
            Restore();
            observedRoot = root;
            appliedTier = tier;
            if (root == null || tier <= 0) return;
            Apply(root, tier);
        }

        private void Apply(GameObject root, int tier)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || !renderer.enabled || renderer.sharedMaterial == null) continue;
                if (renderer.sharedMaterials != null && renderer.sharedMaterials.Length != 1) continue;
                if (IsUnderGeneratedCooling(renderer.transform)) continue;
                if (ShouldSkip(renderer)) continue;

                ProceduralSurfaceKind? kind = ResolveKindForName(renderer.gameObject.name);
                if (!kind.HasValue) continue;

                Material original = renderer.sharedMaterial;
                originals[renderer] = original;
                string key = original.GetInstanceID() + ":" + (int)kind.Value + ":" + tier;
                if (!materialCache.TryGetValue(key, out Material upgraded) || upgraded == null)
                {
                    upgraded = new Material(original)
                    {
                        name = original.name + "_FB_" + kind.Value,
                        hideFlags = HideFlags.DontSave
                    };
                    float tiling = TextureTiling(kind.Value, tier);
                    ProceduralSurfaceLibrary.ApplyToMaterial(upgraded, kind.Value, tiling);
                    ownedMaterials.Add(upgraded);
                    materialCache[key] = upgraded;
                }
                renderer.sharedMaterial = upgraded;
            }
        }

        private static bool IsUnderGeneratedCooling(Transform t)
        {
            while (t != null)
            {
                if (t.name == "ProceduralCoolingUpgrade") return true;
                t = t.parent;
            }
            return false;
        }

        private static bool ShouldSkip(Renderer renderer)
        {
            string n = renderer.gameObject.name;
            if (ContainsAny(n, "Ghost", "Prompt", "PowerButton", "ThermalPaste", "PanelScrew", "Rgb", "RGB", "Label_", "CableHarness")) return true;
            Material material = renderer.sharedMaterial;
            if (material != null && material.renderQueue >= (int)UnityEngine.Rendering.RenderQueue.Transparent) return true;
            return false;
        }

        public static ProceduralSurfaceKind? ResolveKindForName(string objectName)
        {
            string n = objectName ?? string.Empty;
            if (ContainsAny(n, "Trace", "RAMContacts", "Heatpipe", "ColdPlate")) return ProceduralSurfaceKind.Copper;
            if (ContainsAny(n, "Motherboard", "NVMe", "RAMChip", "Chipset")) return ProceduralSurfaceKind.Pcb;
            if (ContainsAny(n, "Tube", "Rubber", "Grommet")) return ProceduralSurfaceKind.Rubber;
            if (ContainsAny(n, "CPU", "M2Shield", "GPUBackplate", "PSUGrille", "DriveLabel", "VRM")) return ProceduralSurfaceKind.BrushedMetal;
            if (ContainsAny(n, "DIMMSlot", "PCIeSlot", "PSUPort", "FanFrame", "FrontVent")) return ProceduralSurfaceKind.Polymer;
            if (ContainsAny(n, "GPU", "PSU", "Case", "FrontBezel", "SATADrive", "CoolerHeatsink", "Radiator")) return ProceduralSurfaceKind.Graphite;
            return null;
        }

        private static float TextureTiling(ProceduralSurfaceKind kind, int tier)
        {
            float baseTiling;
            switch (kind)
            {
                case ProceduralSurfaceKind.BrushedMetal: baseTiling = 5.5f; break;
                case ProceduralSurfaceKind.Pcb: baseTiling = 3.2f; break;
                case ProceduralSurfaceKind.Rubber: baseTiling = 4.5f; break;
                case ProceduralSurfaceKind.Copper: baseTiling = 5f; break;
                default: baseTiling = 3.6f; break;
            }
            return tier >= 3 ? baseTiling * 1.15f : baseTiling;
        }

        private void Restore()
        {
            foreach (KeyValuePair<Renderer, Material> pair in originals)
                if (pair.Key != null && pair.Value != null) pair.Key.sharedMaterial = pair.Value;
            originals.Clear();

            foreach (Material material in ownedMaterials)
                if (material != null) Destroy(material);
            ownedMaterials.Clear();
            materialCache.Clear();
        }

        private static bool ContainsAny(string text, params string[] needles)
        {
            foreach (string needle in needles)
                if (text.IndexOf(needle, System.StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }
    }
}
