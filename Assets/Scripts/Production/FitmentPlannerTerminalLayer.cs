using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    public sealed class FitmentPlannerTerminalLayer : MonoBehaviour
    {
        private GameObject root; private Material body, screen, ruler;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] private static void Install() { if (FindAnyObjectByType<FitmentPlannerTerminalLayer>() != null) return; GameObject g = new GameObject("ForgeBench_FitmentPlannerTerminalLayer"); DontDestroyOnLoad(g); g.AddComponent<FitmentPlannerTerminalLayer>(); }
        private IEnumerator Start() { while (GameRuntime.Instance == null || GameRuntime.Instance.World == null) yield return null; yield return null; Build(); }
        private void Build()
        {
            if (root != null) return; root = new GameObject("FitmentPlannerTerminal"); Shader sh = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"); body = Mat(sh, new Color(.07f, .075f, .085f)); screen = Mat(sh, new Color(.08f, .28f, .14f)); ruler = Mat(sh, new Color(.72f, .58f, .10f)); Vector3 p = new Vector3(3.25f, .92f, -4.72f);
            Box("PlannerCabinet", p - new Vector3(0, .40f, 0), new Vector3(.84f, .90f, .56f), body); GameObject d = Box("PlannerDisplay", p + new Vector3(0, .25f, -.06f), new Vector3(1.02f, .60f, .07f), screen); d.transform.rotation = Quaternion.Euler(-8f, 0f, 0f); Box("MeasurementRail", p + new Vector3(0, -.20f, -.34f), new Vector3(.74f, .055f, .10f), ruler);
            WorldInteractable wi = d.AddComponent<WorldInteractable>(); wi.label = "Open component fitment planner"; wi.priority = 32; wi.maxDistance = 3.2f; wi.action = FitmentPlannerPanel.Open;
        }
        private Material Mat(Shader s, Color c) { Material m = new Material(s); m.color = c; if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", .45f); return m; }
        private GameObject Box(string n, Vector3 p, Vector3 s, Material m) { GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube); g.name = n; g.transform.SetParent(root.transform, true); g.transform.position = p; g.transform.localScale = s; Renderer r = g.GetComponent<Renderer>(); if (r != null) r.sharedMaterial = m; return g; }
        private void OnDestroy() { if (body != null) Destroy(body); if (screen != null) Destroy(screen); if (ruler != null) Destroy(ruler); }
    }
}
