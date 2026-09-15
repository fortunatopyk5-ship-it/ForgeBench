using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Presentation-only material pass for the runtime-built workshop. At quality tier 2+ it gives
    /// walls, epoxy floor, ESD bench tops and metal furniture distinct tactile surface breakup while
    /// preserving all colliders/interactions. Every shared material is restored when the pass exits.
    /// </summary>
    public sealed class WorkshopSurfaceUpgradeDirector : MonoBehaviour
    {
        private readonly Dictionary<Renderer, Material> originals = new Dictionary<Renderer, Material>();
        private readonly Dictionary<string, Material> cache = new Dictionary<string, Material>();
        private readonly List<Material> owned = new List<Material>();
        private bool dirty = true;
        private int appliedTier = -1;
        private float nextRefresh;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<WorkshopSurfaceUpgradeDirector>() != null) return;
            GameObject host = new GameObject("ForgeBench_WorkshopSurfaceUpgrade");
            DontDestroyOnLoad(host);
            host.AddComponent<WorkshopSurfaceUpgradeDirector>();
        }

        private void OnEnable() { RuntimeRenderBudget.Changed += OnBudgetChanged; }
        private void OnDisable()
        {
            RuntimeRenderBudget.Changed -= OnBudgetChanged;
            Restore();
        }

        private void OnBudgetChanged(int tier) { dirty = true; }

        private void LateUpdate()
        {
            if (!dirty && Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + 1.5f;
            int tier = RuntimeRenderBudget.CurrentTier;

            // Keep tier 0/1 extremely cheap for battery-saver / constrained Android devices.
            if (tier < 2)
            {
                if (originals.Count > 0) Restore();
                appliedTier = tier;
                dirty = false;
                return;
            }

            if (!dirty && appliedTier == tier) return;
            Restore();
            Apply(tier);
            appliedTier = tier;
            dirty = false;
        }

        private void Apply(int tier)
        {
            Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || renderer.sharedMaterial == null) continue;
                if (renderer.sharedMaterials != null && renderer.sharedMaterials.Length != 1) continue;
                if (IsMachineOrUiHierarchy(renderer.transform)) continue;

                string parentName = renderer.transform.parent != null ? renderer.transform.parent.name : string.Empty;
                WorkshopSurfaceProfile profile = ResolveProfile(renderer.gameObject.name, parentName);
                if (!profile.valid) continue;

                Material original = renderer.sharedMaterial;
                originals[renderer] = original;
                string key = original.GetInstanceID() + ":" + (int)profile.kind + ":" + profile.tintId + ":" + tier;
                if (!cache.TryGetValue(key, out Material upgraded) || upgraded == null)
                {
                    upgraded = new Material(original)
                    {
                        name = original.name + "_FB_Workshop_" + profile.tintId,
                        hideFlags = HideFlags.DontSave
                    };
                    upgraded.color = Color.Lerp(original.color, profile.targetTint, profile.tintStrength);
                    if (upgraded.HasProperty("_Smoothness")) upgraded.SetFloat("_Smoothness", profile.smoothness);
                    if (upgraded.HasProperty("_Metallic")) upgraded.SetFloat("_Metallic", profile.metallic);
                    ProceduralSurfaceLibrary.ApplyToMaterial(upgraded, profile.kind, tier >= 3 ? profile.tiling * 1.15f : profile.tiling);
                    cache[key] = upgraded;
                    owned.Add(upgraded);
                }
                renderer.sharedMaterial = upgraded;
            }
        }

        public static WorkshopSurfaceProfile ResolveProfile(string objectName, string parentName)
        {
            string n = objectName ?? string.Empty;
            string p = parentName ?? string.Empty;
            if (n == "Floor")
                return WorkshopSurfaceProfile.Make(ProceduralSurfaceKind.DarkMetal, "epoxy", new Color(.095f, .112f, .122f), .58f, .48f, .22f, 4.2f);
            if (n == "BackWall" || n == "LeftWall" || n == "RightWall")
                return WorkshopSurfaceProfile.Make(ProceduralSurfaceKind.Polymer, "warm-wall", new Color(.74f, .72f, .67f), .72f, .28f, .02f, 2.4f);

            bool serviceBench = p == "Main Assembly Bench" || p == "Diagnostic Bench" || p == "Board Repair Station";
            if (n == "Top" && serviceBench)
                return WorkshopSurfaceProfile.Make(ProceduralSurfaceKind.Rubber, "esd-teal", new Color(.055f, .245f, .225f), .76f, .24f, .03f, 5.0f);
            if (n == "Top" && p == "Packaging Station")
                return WorkshopSurfaceProfile.Make(ProceduralSurfaceKind.Polymer, "packing", new Color(.31f, .29f, .26f), .48f, .34f, .04f, 3.4f);

            if ((n == "Leg" && p.EndsWith("Bench")) || (n == "Leg" && p.EndsWith("Station")) ||
                (p == "WarehouseShelf" && (n == "Post" || n == "Shelf")))
                return WorkshopSurfaceProfile.Make(ProceduralSurfaceKind.DarkMetal, "graphite-frame", new Color(.095f, .105f, .115f), .60f, .42f, .78f, 4.8f);

            if (p == "WarehouseShelf" && n == "Box")
                return WorkshopSurfaceProfile.Make(ProceduralSurfaceKind.Polymer, "storage-bin", new Color(.16f, .34f, .36f), .38f, .26f, .05f, 3.2f);
            if (n == "ReceivingArea")
                return WorkshopSurfaceProfile.Make(ProceduralSurfaceKind.Rubber, "receiving-mat", new Color(.075f, .30f, .29f), .55f, .22f, .02f, 4.5f);
            if (n == "TabletDock")
                return WorkshopSurfaceProfile.Make(ProceduralSurfaceKind.Polymer, "tablet-dock", new Color(.08f, .14f, .17f), .48f, .52f, .18f, 5.2f);

            return default;
        }

        private static bool IsMachineOrUiHierarchy(Transform t)
        {
            while (t != null)
            {
                string n = t.name;
                if (n == "ProductionMachine3D" || n == "ActiveMachine3D" || n == "ProceduralCoolingUpgrade") return true;
                if (t.GetComponent<Canvas>() != null) return true;
                t = t.parent;
            }
            return false;
        }

        private void Restore()
        {
            foreach (KeyValuePair<Renderer, Material> pair in originals)
                if (pair.Key != null && pair.Value != null) pair.Key.sharedMaterial = pair.Value;
            originals.Clear();
            foreach (Material material in owned)
                if (material != null) Destroy(material);
            owned.Clear();
            cache.Clear();
        }
    }

    public struct WorkshopSurfaceProfile
    {
        public bool valid;
        public ProceduralSurfaceKind kind;
        public string tintId;
        public Color targetTint;
        public float tintStrength;
        public float smoothness;
        public float metallic;
        public float tiling;

        public static WorkshopSurfaceProfile Make(ProceduralSurfaceKind kind, string tintId, Color tint,
            float tintStrength, float smoothness, float metallic, float tiling)
        {
            return new WorkshopSurfaceProfile
            {
                valid = true,
                kind = kind,
                tintId = tintId,
                targetTint = tint,
                tintStrength = Mathf.Clamp01(tintStrength),
                smoothness = Mathf.Clamp01(smoothness),
                metallic = Mathf.Clamp01(metallic),
                tiling = Mathf.Max(.25f, tiling)
            };
        }
    }
}
