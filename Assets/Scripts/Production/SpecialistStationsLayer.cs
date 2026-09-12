using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical entry points for specialist workflows placed in the workshop.</summary>
    public sealed class SpecialistStationsLayer : MonoBehaviour
    {
        private GameObject root;
        private readonly List<Material> materials = new List<Material>();
        private Shader shader;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<SpecialistStationsLayer>() != null) return;
            GameObject go = new GameObject("ForgeBench_SpecialistStations");
            DontDestroyOnLoad(go);
            go.AddComponent<SpecialistStationsLayer>();
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
            root = new GameObject("SpecialistStationsWorld");
            shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            BuildStation("LiquidCoolingStation", new Vector3(-5.6f,1.15f,1.55f), new Color(.05f,.48f,.75f), "LIQUID\nCOOLING", SpecialistPanelMode.Liquid);
            BuildStation("BoardRepairStationPro", new Vector3(5.6f,1.15f,1.55f), new Color(.75f,.28f,.08f), "BOARD\nREPAIR", SpecialistPanelMode.Board);
            BuildStation("PortableRepairStation", new Vector3(-5.6f,1.15f,-2.0f), new Color(.42f,.25f,.76f), "PORTABLE\nREPAIR", SpecialistPanelMode.Portable);
            BuildStation("NetworkLabStation", new Vector3(5.6f,1.15f,-2.0f), new Color(.10f,.62f,.38f), "NETWORK\nNAS", SpecialistPanelMode.Network);
        }

        private void BuildStation(string name, Vector3 pos, Color accent, string label, SpecialistPanelMode mode)
        {
            Material frame=Mat(new Color(.055f,.065f,.075f),.42f,.72f);
            Material top=Mat(new Color(.16f,.13f,.10f),.48f,.15f);
            Material screen=Mat(accent,.65f,.15f);
            Box(name+"_Bench",pos+new Vector3(0,-.58f,.15f),new Vector3(2.1f,.12f,1.0f),top);
            for(int sx=-1;sx<=1;sx+=2) for(int sz=-1;sz<=1;sz+=2)
                Box(name+"_Leg",pos+new Vector3(sx*.82f,-1.02f,sz*.33f+.15f),new Vector3(.09f,.85f,.09f),frame);
            Box(name+"_Console",pos+new Vector3(0,.0f,.33f),new Vector3(.86f,.48f,.10f),frame);
            GameObject display=Box(name+"_Display",pos+new Vector3(0,.0f,.265f),new Vector3(.74f,.36f,.025f),screen);
            AddInteractable(display,"Open "+label.Replace("\n"," ").ToLowerInvariant(),30,()=>SpecialistRepairPanel.Open(mode));
            CreateLabel(label,pos+new Vector3(0,.03f,.245f),24);

            if(mode==SpecialistPanelMode.Liquid)
            {
                Cylinder(name+"_Reservoir",pos+new Vector3(-.62f,-.25f,.15f),new Vector3(.11f,.38f,.11f),Quaternion.identity,Mat(new Color(.08f,.36f,.55f),.75f,.15f));
                Box(name+"_Radiator",pos+new Vector3(.62f,-.20f,.15f),new Vector3(.38f,.55f,.08f),frame);
            }
            else if(mode==SpecialistPanelMode.Board)
            {
                Box(name+"_MicroscopeBase",pos+new Vector3(-.55f,-.28f,.12f),new Vector3(.36f,.08f,.30f),frame);
                Cylinder(name+"_MicroscopePost",pos+new Vector3(-.55f,.05f,.12f),new Vector3(.04f,.28f,.04f),Quaternion.identity,frame);
                Box(name+"_HotAir",pos+new Vector3(.58f,-.27f,.12f),new Vector3(.22f,.18f,.28f),frame);
            }
            else if(mode==SpecialistPanelMode.Portable)
            {
                Box(name+"_HeatMat",pos+new Vector3(-.45f,-.48f,.08f),new Vector3(.65f,.025f,.55f),Mat(new Color(.17f,.07f,.20f),.25f,.1f));
                Box(name+"_PartsTray",pos+new Vector3(.48f,-.45f,.12f),new Vector3(.48f,.08f,.42f),frame);
            }
            else
            {
                for(int i=0;i<4;i++) Box(name+"_DriveBay",pos+new Vector3(-.54f+i*.36f,-.35f,.12f),new Vector3(.28f,.18f,.45f),frame);
                Box(name+"_Switch",pos+new Vector3(0,-.12f,.12f),new Vector3(1.4f,.16f,.36f),frame);
            }
        }

        private void CreateLabel(string text,Vector3 pos,int size)
        {
            GameObject go=new GameObject("StationLabel",typeof(RectTransform),typeof(Canvas));go.transform.SetParent(root.transform,false);go.transform.position=pos;go.transform.rotation=Quaternion.Euler(0,180,0);go.transform.localScale=Vector3.one*.0065f;
            Canvas c=go.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;c.sortingOrder=4;RectTransform rt=go.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(120,70);
            GameObject tgo=new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));tgo.transform.SetParent(go.transform,false);RectTransform tr=tgo.GetComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;
            UnityEngine.UI.Text t=tgo.GetComponent<UnityEngine.UI.Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=text;t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;
        }

        private GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(root.transform,true);g.transform.position=pos;g.transform.localScale=scale;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;return g;}
        private GameObject Cylinder(string name,Vector3 pos,Vector3 scale,Quaternion rot,Material mat){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=name;g.transform.SetParent(root.transform,true);g.transform.position=pos;g.transform.localScale=scale;g.transform.rotation=rot;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;return g;}
        private Material Mat(Color c,float smooth,float metallic=0f){Material m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);materials.Add(m);return m;}
        private static void AddInteractable(GameObject go,string label,int priority,Action action){WorldInteractable i=go.GetComponent<WorldInteractable>()??go.AddComponent<WorldInteractable>();i.label=label;i.priority=priority;i.action=action;i.maxDistance=3.3f;}
        private void OnDestroy(){foreach(Material m in materials)if(m!=null)Destroy(m);}
    }
}
