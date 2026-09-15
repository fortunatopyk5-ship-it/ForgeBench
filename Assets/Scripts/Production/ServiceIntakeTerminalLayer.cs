using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical front-desk station for intake evidence, approvals and chain-of-custody handoff.</summary>
    public sealed class ServiceIntakeTerminalLayer : MonoBehaviour
    {
        private GameObject root;
        private Material body,screen,accent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<ServiceIntakeTerminalLayer>() != null) return;
            GameObject go = new GameObject("ForgeBench_ServiceIntakeTerminalLayer");
            DontDestroyOnLoad(go);
            go.AddComponent<ServiceIntakeTerminalLayer>();
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
            root = new GameObject("ServiceIntakeTerminal");
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            body = Mat(shader,new Color(.055f,.065f,.068f),.50f,.68f);
            screen = Mat(shader,new Color(.020f,.135f,.150f),.62f,.16f);
            accent = Mat(shader,new Color(.12f,.78f,.76f),.55f,.18f);
            Vector3 p = new Vector3(-2.72f,.92f,-2.18f);
            Box("IntakePedestal",p-new Vector3(0,.41f,0),new Vector3(.82f,.92f,.58f),body);
            GameObject display = Box("IntakeScreen",p+new Vector3(0,.25f,-.05f),new Vector3(1.04f,.62f,.08f),screen); display.transform.rotation = Quaternion.Euler(-8f,0,0);
            WorldInteractable wi = display.AddComponent<WorldInteractable>(); wi.label = "Open service intake / custody desk"; wi.priority = 32; wi.maxDistance = 3.2f; wi.action = ServiceIntakePanel.Open;
            Box("IntakeLamp",p+new Vector3(.42f,.67f,-.02f),new Vector3(.08f,.08f,.08f),accent);
        }

        private Material Mat(Shader shader,Color color,float smooth,float metallic){Material m=new Material(shader);m.color=color;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);return m;}
        private GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(root.transform,true);g.transform.position=pos;g.transform.localScale=scale;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;return g;}
        private void OnDestroy(){if(body!=null)Destroy(body);if(screen!=null)Destroy(screen);if(accent!=null)Destroy(accent);}
    }
}