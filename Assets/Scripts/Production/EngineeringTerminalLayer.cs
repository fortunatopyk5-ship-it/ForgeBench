using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical engineering consoles for POST/BIOS, thermal, OS/storage and benchmark labs.</summary>
    public sealed class EngineeringTerminalLayer : MonoBehaviour
    {
        private GameObject root;
        private readonly List<Material> materials=new List<Material>();
        private Shader shader;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if(FindAnyObjectByType<EngineeringTerminalLayer>()!=null)return;
            GameObject go=new GameObject("ForgeBench_EngineeringTerminals");DontDestroyOnLoad(go);go.AddComponent<EngineeringTerminalLayer>();
        }

        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;yield return null;Build();}

        private void Build()
        {
            if(root!=null)return;root=new GameObject("EngineeringTerminalsWorld");shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");
            Console("POST_UEFI_Console",new Vector3(-3.05f,1.42f,4.72f),new Color(.42f,.26f,.78f),"POST / UEFI",0);
            Console("ThermalPowerConsole",new Vector3(-1.05f,1.42f,4.72f),new Color(.82f,.26f,.08f),"THERMAL / POWER",1);
            Console("OSStorageConsole",new Vector3(1.05f,1.42f,4.72f),new Color(.06f,.48f,.76f),"OS / STORAGE",2);
            Console("BenchmarkConsole",new Vector3(3.05f,1.42f,4.72f),new Color(.10f,.64f,.38f),"BENCHMARK",3);
        }

        private void Console(string name,Vector3 p,Color accent,string label,int page)
        {
            Material dark=Mat(new Color(.045f,.055f,.068f),.42f,.74f),screen=Mat(accent,.65f,.14f),metal=Mat(new Color(.20f,.22f,.24f),.58f,.82f);
            Box(name+"_Base",p+new Vector3(0,-.72f,.12f),new Vector3(1.34f,.10f,.72f),dark);
            Box(name+"_Stand",p+new Vector3(0,-.40f,.27f),new Vector3(.10f,.58f,.10f),metal);
            GameObject display=Box(name,p,new Vector3(1.12f,.58f,.08f),screen);display.transform.rotation=Quaternion.Euler(-7,0,0);AddInteractable(display,"Open "+label.ToLowerInvariant()+" engineering console",32,()=>EngineeringDiagnosticsPanel.Open(page));
            Box(name+"_Keyboard",p+new Vector3(0,-.45f,-.13f),new Vector3(.92f,.035f,.30f),dark);CreateLabel(label,p+new Vector3(0,.01f,-.055f),page==1?18:21);
        }

        private void CreateLabel(string value,Vector3 p,int size)
        {
            GameObject g=new GameObject("EngineeringLabel",typeof(RectTransform),typeof(Canvas));g.transform.SetParent(root.transform,false);g.transform.position=p;g.transform.rotation=Quaternion.Euler(0,180,0);g.transform.localScale=Vector3.one*.006f;Canvas c=g.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;c.sortingOrder=5;RectTransform r=g.GetComponent<RectTransform>();r.sizeDelta=new Vector2(150,70);GameObject tg=new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));tg.transform.SetParent(g.transform,false);RectTransform tr=tg.GetComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;UnityEngine.UI.Text t=tg.GetComponent<UnityEngine.UI.Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=value;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;
        }

        private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}
        private Material Mat(Color c,float smooth,float metallic=0){Material m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);materials.Add(m);return m;}
        private static void AddInteractable(GameObject g,string label,int priority,Action action){WorldInteractable i=g.GetComponent<WorldInteractable>()??g.AddComponent<WorldInteractable>();i.label=label;i.priority=priority;i.action=action;i.maxDistance=3.3f;}
        private void OnDestroy(){foreach(Material m in materials)if(m!=null)Destroy(m);}
    }
}
