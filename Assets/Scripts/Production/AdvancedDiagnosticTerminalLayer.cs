using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical workshop console that opens the evidence-driven diagnostics lab.</summary>
    public sealed class AdvancedDiagnosticTerminalLayer : MonoBehaviour
    {
        private GameObject root; private Material body, screen, lamp;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<AdvancedDiagnosticTerminalLayer>() != null) return;
            GameObject g = new GameObject("ForgeBench_AdvancedDiagnosticTerminalLayer"); DontDestroyOnLoad(g); g.AddComponent<AdvancedDiagnosticTerminalLayer>();
        }
        private IEnumerator Start() { while (GameRuntime.Instance == null || GameRuntime.Instance.World == null) yield return null; yield return null; Build(); }
        private void Build()
        {
            if (root != null) return; root = new GameObject("AdvancedDiagnosticTerminal"); Shader sh = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            body = Mat(sh, new Color(.055f, .065f, .075f)); screen = Mat(sh, new Color(.025f, .24f, .38f)); lamp = Mat(sh, new Color(.08f, .75f, .95f));
            Vector3 p = new Vector3(4.55f, .94f, -2.05f); Box("DiagnosticCabinet", p - new Vector3(0, .43f, 0), new Vector3(.92f, .96f, .62f), body);
            GameObject display = Box("DiagnosticDisplay", p + new Vector3(0, .27f, -.08f), new Vector3(1.12f, .64f, .075f), screen); display.transform.rotation = Quaternion.Euler(-7f, 0f, 0f);
            for (int i = -2; i <= 2; i++) Box("ProbeJack_" + i, p + new Vector3(i * .15f, -.10f, -.37f), new Vector3(.07f, .07f, .045f), lamp);
            GameObject tray = Box("ProbeTray", p + new Vector3(0, -.28f, -.38f), new Vector3(.76f, .08f, .22f), body); tray.transform.rotation = Quaternion.Euler(4f, 0f, 0f);
            WorldInteractable wi = display.AddComponent<WorldInteractable>(); wi.label = "Open advanced diagnostic lab"; wi.priority = 34; wi.maxDistance = 3.2f; wi.action = AdvancedDiagnosticWorkflowPanel.Open;
        }
        private Material Mat(Shader sh, Color c) { Material m = new Material(sh); m.color = c; if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", .48f); return m; }
        private GameObject Box(string n, Vector3 p, Vector3 s, Material m) { GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube); g.name = n; g.transform.SetParent(root.transform, true); g.transform.position = p; g.transform.localScale = s; Renderer r = g.GetComponent<Renderer>(); if (r != null) r.sharedMaterial = m; return g; }
        private void OnDestroy() { if (body != null) Destroy(body); if (screen != null) Destroy(screen); if (lamp != null) Destroy(lamp); }
    }
}
