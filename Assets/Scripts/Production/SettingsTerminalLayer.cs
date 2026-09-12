using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical workshop terminal for touch, performance, audio and accessibility settings.</summary>
    public sealed class SettingsTerminalLayer : MonoBehaviour
    {
        private GameObject root;private Material body,screen,accent;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<SettingsTerminalLayer>()!=null)return;GameObject go=new GameObject("ForgeBench_SettingsTerminalLayer");DontDestroyOnLoad(go);go.AddComponent<SettingsTerminalLayer>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;yield return null;Build();}
        private void Build()
        {
            if(root!=null)return;root=new GameObject("SettingsTerminal");Shader shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");body=Mat(shader,new Color(.055f,.063f,.076f),.54f,.68f);screen=Mat(shader,new Color(.11f,.07f,.24f),.67f,.16f);accent=Mat(shader,new Color(.48f,.30f,.92f),.60f,.18f);Vector3 p=new Vector3(-3.95f,.92f,-2.18f);
            Box("SettingsPedestal",p-new Vector3(0,.41f,0),new Vector3(.78f,.92f,.56f),body);GameObject display=Box("SettingsScreen",p+new Vector3(0,.25f,-.05f),new Vector3(1.00f,.62f,.08f),screen);display.transform.rotation=Quaternion.Euler(-8f,0,0);WorldInteractable wi=display.AddComponent<WorldInteractable>();wi.label="Open Android / accessibility settings";wi.priority=29;wi.maxDistance=3.2f;wi.action=AdvancedSettingsPanel.Open;Box("SettingsIndicator",p+new Vector3(.40f,.67f,-.02f),new Vector3(.08f,.08f,.08f),accent);
        }
        private Material Mat(Shader shader,Color color,float smooth,float metallic){Material m=new Material(shader);m.color=color;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);return m;}private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}private void OnDestroy(){if(body!=null)Destroy(body);if(screen!=null)Destroy(screen);if(accent!=null)Destroy(accent);}
    }
}
