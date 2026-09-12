using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical workshop terminal for the unified operations and QA command screen.</summary>
    public sealed class OperationsTerminalLayer : MonoBehaviour
    {
        private GameObject root;
        private Material body,screen,accent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<OperationsTerminalLayer>() != null) return;
            GameObject go = new GameObject("ForgeBench_OperationsTerminalLayer");
            DontDestroyOnLoad(go);
            go.AddComponent<OperationsTerminalLayer>();
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
            root = new GameObject("OperationsCommandTerminal");
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            body = Mat(shader,new Color(.045f,.056f,.071f),.55f,.72f);
            screen = Mat(shader,new Color(.025f,.17f,.24f),.72f,.18f);
            accent = Mat(shader,new Color(.06f,.58f,.86f),.65f,.24f);
            Vector3 p = new Vector3(0f,.94f,-2.18f);
            Box("OperationsPedestal",p-new Vector3(0,.44f,0),new Vector3(.94f,.96f,.62f),body);
            Box("OperationsConsoleLip",p-new Vector3(0,.02f,.08f),new Vector3(1.16f,.10f,.74f),body);
            GameObject display = Box("OperationsScreen",p+new Vector3(0,.28f,-.04f),new Vector3(1.34f,.72f,.075f),screen);display.transform.rotation=Quaternion.Euler(-8f,0,0);
            WorldInteractable wi=display.AddComponent<WorldInteractable>();wi.label="Open workshop operations / QA";wi.priority=34;wi.maxDistance=3.4f;wi.action=OperationsDashboardPanel.Open;
            for(int i=0;i<4;i++)Box("StatusLed"+i,p+new Vector3(-.42f+i*.28f,.70f,-.015f),new Vector3(.055f,.035f,.025f),accent);
        }

        private Material Mat(Shader shader,Color color,float smooth,float metallic){Material m=new Material(shader);m.color=color;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);return m;}
        private GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(root.transform,true);g.transform.position=pos;g.transform.localScale=scale;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;return g;}
        private void OnDestroy(){if(body!=null)Destroy(body);if(screen!=null)Destroy(screen);if(accent!=null)Destroy(accent);}
    }
}
