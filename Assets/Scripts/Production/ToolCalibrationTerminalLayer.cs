using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical workshop station for tool calibration, maintenance and job-readiness checks.</summary>
    public sealed class ToolCalibrationTerminalLayer : MonoBehaviour
    {
        private GameObject root;
        private Material body,bench,screen,accent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if(FindAnyObjectByType<ToolCalibrationTerminalLayer>()!=null)return;
            GameObject go=new GameObject("ForgeBench_ToolCalibrationTerminalLayer");DontDestroyOnLoad(go);go.AddComponent<ToolCalibrationTerminalLayer>();
        }

        private IEnumerator Start()
        {
            while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;
            yield return null;Build();
        }

        private void Build()
        {
            if(root!=null)return;
            root=new GameObject("MetrologyBench");Shader shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");
            body=Mat(shader,new Color(.055f,.062f,.065f),.48f,.72f);bench=Mat(shader,new Color(.055f,.23f,.22f),.52f,.10f);screen=Mat(shader,new Color(.025f,.13f,.17f),.68f,.12f);accent=Mat(shader,new Color(.15f,.80f,.70f),.58f,.18f);
            Vector3 p=new Vector3(3.10f,.78f,-2.00f);
            Box("MetrologyCabinet",p-new Vector3(0,.40f,0),new Vector3(1.25f,.82f,.65f),body);
            Box("ESDTop",p+new Vector3(0,.04f,0),new Vector3(1.38f,.08f,.76f),bench);
            GameObject display=Box("CalibrationDisplay",p+new Vector3(.32f,.40f,.10f),new Vector3(.54f,.38f,.07f),screen);display.transform.rotation=Quaternion.Euler(-8f,0,0);WorldInteractable wi=display.AddComponent<WorldInteractable>();wi.label="Open tool metrology / maintenance";wi.priority=31;wi.maxDistance=3.1f;wi.action=ToolCalibrationPanel.Open;
            Box("ReferenceBlock",p+new Vector3(-.32f,.18f,.04f),new Vector3(.30f,.12f,.22f),accent);
            Box("DriverDock",p+new Vector3(-.12f,.16f,-.16f),new Vector3(.10f,.22f,.10f),body);
        }

        private Material Mat(Shader shader,Color color,float smooth,float metallic){Material m=new Material(shader);m.color=color;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);return m;}
        private GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(root.transform,true);g.transform.position=pos;g.transform.localScale=scale;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;return g;}
        private void OnDestroy(){if(body!=null)Destroy(body);if(bench!=null)Destroy(bench);if(screen!=null)Destroy(screen);if(accent!=null)Destroy(accent);}
    }
}