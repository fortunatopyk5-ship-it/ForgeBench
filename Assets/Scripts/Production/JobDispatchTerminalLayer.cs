using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical dispatch desk terminal for concurrent job intake and bench focus switching.</summary>
    public sealed class JobDispatchTerminalLayer : MonoBehaviour
    {
        private GameObject root;
        private Material body, screen, lamp;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<JobDispatchTerminalLayer>() != null) return;
            GameObject go = new GameObject("ForgeBench_JobDispatchTerminalLayer");
            DontDestroyOnLoad(go);
            go.AddComponent<JobDispatchTerminalLayer>();
        }

        private IEnumerator Start()
        {
            while (GameRuntime.Instance == null || GameRuntime.Instance.World == null) yield return null;
            yield return null;
            Build();
        }

        private void Build()
        {
            if (root != null) return;
            root = new GameObject("WorkshopDispatchTerminal");
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            body = Mat(shader, new Color(.055f, .066f, .08f));
            screen = Mat(shader, new Color(.035f, .21f, .31f));
            lamp = Mat(shader, new Color(.13f, .78f, .95f));
            Vector3 p = new Vector3(-3.55f, .95f, -4.70f);
            Box("DispatchDesk", p - new Vector3(0, .48f, 0), new Vector3(1.45f, .12f, .72f), body);
            Box("DispatchPedestal", p - new Vector3(0, .92f, .05f), new Vector3(.56f, .82f, .48f), body);
            GameObject display = Box("DispatchDisplay", p + new Vector3(0, .18f, -.14f), new Vector3(1.18f, .68f, .075f), screen);
            display.transform.rotation = Quaternion.Euler(-8f, 0f, 0f);
            for (int i = -2; i <= 2; i++) Box("QueueLamp_" + i, p + new Vector3(i * .18f, -.20f, -.39f), new Vector3(.075f, .05f, .04f), lamp);
            WorldInteractable wi = display.AddComponent<WorldInteractable>();
            wi.label = "Open workshop dispatch";
            wi.priority = 40;
            wi.maxDistance = 3.3f;
            wi.action = JobDispatchPanel.Open;
        }

        private Material Mat(Shader shader, Color color)
        {
            Material m = new Material(shader); m.color = color; if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", .46f); return m;
        }
        private GameObject Box(string name, Vector3 pos, Vector3 scale, Material material)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube); g.name = name; g.transform.SetParent(root.transform, true); g.transform.position = pos; g.transform.localScale = scale; Renderer r = g.GetComponent<Renderer>(); if (r != null) r.sharedMaterial = material; return g;
        }
        private void OnDestroy() { if (body != null) Destroy(body); if (screen != null) Destroy(screen); if (lamp != null) Destroy(lamp); }
    }
}
