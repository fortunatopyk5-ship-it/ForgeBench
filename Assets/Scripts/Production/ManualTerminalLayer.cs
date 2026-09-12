using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical terminal that exposes the offline workshop manual and context help.</summary>
    public sealed class ManualTerminalLayer : MonoBehaviour
    {
        private GameObject root;private Material body,screen,accent;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<ManualTerminalLayer>()!=null)return;GameObject go=new GameObject("ForgeBench_ManualTerminalLayer");DontDestroyOnLoad(go);go.AddComponent<ManualTerminalLayer>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;yield return null;Build();}
        private void Build()
        {
            if(root!=null)return;root=new GameObject("WorkshopManualTerminal");Shader shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");body=Mat(shader,new Color(.055f,.065f,.078f),.52f,.68f);screen=Mat(shader,new Color(.035f,.18f,.12f),.65f,.15f);accent=Mat(shader,new Color(.10f,.72f,.44f),.58f,.18f);Vector3 p=new Vector3(-2.65f,.92f,-2.18f);
            Box("ManualPedestal",p-new Vector3(0,.41f,0),new Vector3(.82f,.92f,.58f),body);GameObject display=Box("ManualScreen",p+new Vector3(0,.25f,-.05f),new Vector3(1.04f,.62f,.08f),screen);display.transform.rotation=Quaternion.Euler(-8f,0,0);WorldInteractable wi=display.AddComponent<WorldInteractable>();wi.label="Open workshop manual / context help";wi.priority=30;wi.maxDistance=3.2f;wi.action=InGameManualPanel.Open;Box("HelpIndicator",p+new Vector3(.42f,.67f,-.02f),new Vector3(.08f,.08f,.08f),accent);
        }
        private Material Mat(Shader shader,Color color,float smooth,float metallic){Material m=new Material(shader);m.color=color;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);return m;}private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}private void OnDestroy(){if(body!=null)Destroy(body);if(screen!=null)Destroy(screen);if(accent!=null)Destroy(accent);}
    }
}
