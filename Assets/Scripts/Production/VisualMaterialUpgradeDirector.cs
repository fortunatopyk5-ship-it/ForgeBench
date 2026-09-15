using UnityEngine;
using UnityEngine.SceneManagement;

namespace ForgeBench
{
    /// <summary>
    /// Applies deterministic PBR-like fallback materials to the runtime-authored workshop and machine.
    /// It never owns gameplay state and never replaces colliders or interactables.
    /// </summary>
    public sealed class VisualMaterialUpgradeDirector : MonoBehaviour
    {
        private static bool installing;
        private ProceduralSurfaceLibrary library;
        private GameObject observedMachineRoot;
        private int observedCoolingRevision = -1;
        private int observedScene = int.MinValue;
        private float nextPoll;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (installing) return;
            installing = true;
            GameObject host = new GameObject("ForgeBench_VisualMaterialUpgrade");
            DontDestroyOnLoad(host);
            host.AddComponent<VisualMaterialUpgradeDirector>();
        }

        private void Awake()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            int size = Application.isMobilePlatform ? 64 : 128;
            library = new ProceduralSurfaceLibrary(shader, size);
        }

        private void OnDestroy()
        {
            if (library != null) library.Dispose();
            library = null;
        }

        private void LateUpdate()
        {
            if (Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + .35f;

            int scene = SceneManager.GetActiveScene().handle;
            if (scene != observedScene)
            {
                observedScene = scene;
                ApplyWorkshopSurfaces();
                observedMachineRoot = null;
                observedCoolingRevision = -1;
            }

            GameObject machineRoot = GameObject.Find("ProductionMachine3D");
            int coolingRevision = CoolingVisualUpgradeDirector.VisualRevision;
            if (machineRoot != null && (machineRoot != observedMachineRoot || coolingRevision != observedCoolingRevision))
            {
                observedMachineRoot = machineRoot;
                observedCoolingRevision = coolingRevision;
                ApplyMachineSurfaces(machineRoot.transform);
            }
        }

        private void ApplyWorkshopSurfaces()
        {
            Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || renderer.transform == null) continue;
                if (IsInsideNamedRoot(renderer.transform, "ProductionMachine3D")) continue;
                string semantic = SemanticName(renderer.transform);
                ProceduralSurfaceProfile profile = ProceduralSurfaceLibrary.Classify(semantic);
                if (profile == ProceduralSurfaceProfile.None) continue;
                Assign(renderer, profile);
            }
        }

        private void ApplyMachineSurfaces(Transform root)
        {
            if (root == null) return;
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;
                string name = renderer.gameObject.name;
                if (ShouldPreserveDynamicMaterial(name)) continue;
                string semantic = SemanticName(renderer.transform);
                ProceduralSurfaceProfile profile = ResolveMachineProfile(semantic);
                if (profile == ProceduralSurfaceProfile.None) continue;
                Assign(renderer, profile);
            }
        }

        private void Assign(Renderer renderer, ProceduralSurfaceProfile profile)
        {
            Material material = library != null ? library.Get(profile) : null;
            if (material != null) renderer.sharedMaterial = material;
        }

        private static ProceduralSurfaceProfile ResolveMachineProfile(string semantic)
        {
            string n = (semantic ?? string.Empty).ToLowerInvariant();
            if (n.Contains("radiatorfinpack") || n.Contains("towerfinstack") || n.Contains("coolerfin_"))
                return ProceduralSurfaceProfile.BrushedAluminum;
            if (n.Contains("coldplate") || n.Contains("baseplate") || n.Contains("heatpipe_"))
                return ProceduralSurfaceProfile.Copper;
            if (n.Contains("pumphousing")) return ProceduralSurfaceProfile.GraphitePowderCoat;
            return ProceduralSurfaceLibrary.Classify(semantic);
        }

        private static bool ShouldPreserveDynamicMaterial(string name)
        {
            string n = (name ?? string.Empty).ToLowerInvariant();
            return n.Contains("ghost") || n.Contains("powerbutton") || n.Contains("thermalpaste") ||
                   n.Contains("rgbring") || n.Contains("accent") || n.Contains("pumpcap") || n.Contains("sidepanel");
        }

        private static bool IsInsideNamedRoot(Transform t, string rootName)
        {
            Transform cursor = t;
            while (cursor != null)
            {
                if (cursor.name == rootName) return true;
                cursor = cursor.parent;
            }
            return false;
        }

        private static string SemanticName(Transform t)
        {
            if (t == null) return string.Empty;
            string value = t.name;
            Transform p = t.parent;
            int depth = 0;
            while (p != null && depth < 4)
            {
                value = p.name + "/" + value;
                p = p.parent;
                depth++;
            }
            return value;
        }
    }
}
