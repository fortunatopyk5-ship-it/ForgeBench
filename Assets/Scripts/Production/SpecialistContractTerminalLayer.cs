using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    public sealed class SpecialistContractTerminalLayer : MonoBehaviour
    {
        private GameObject root;
        private readonly List<Material> materials=new List<Material>();
        private Shader shader;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if(FindAnyObjectByType<SpecialistContractTerminalLayer>()!=null)return;
            GameObject go=new GameObject("ForgeBench_SpecialistContractTerminal");DontDestroyOnLoad(go);go.AddComponent<SpecialistContractTerminalLayer>();
        }

        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;yield return null;Build();}
        private void Build()
        {
            if(root!=null)return;root=new GameObject("SpecialistContractTerminalWorld");shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");Material dark=Mat(new Color(.045f,.055f,.068f),.42f,.75f),accent=Mat(new Color(.78f,.38f,.06f),.58f,.18f),glass=Mat(new Color(.04f,.20f,.27f),.72f,.1f);
            Vector3 p=new Vector3(0,1.28f,-4.72f);Box("SpecialistBoardStand",p+new Vector3(0,-.70f,0),new Vector3(.16f,1.35f,.16f),dark);Box("SpecialistBoardBase",p+new Vector3(0,-1.33f,0),new Vector3(.95f,.08f,.55f),dark);GameObject panel=Box("SpecialistContractDisplay",p,new Vector3(1.72f,.86f,.09f),glass);Box("SpecialistAccent",p+new Vector3(0,-.47f,-.02f),new Vector3(1.72f,.06f,.12f),accent);AddInteractable(panel,"Open specialist contracts",35,SpecialistContractBoard.Open);CreateLabel("SPECIALIST\nCONTRACTS",p+new Vector3(0,.03f,-.065f));
        }
        private void CreateLabel(string text,Vector3 pos){GameObject g=new GameObject("SpecialistLabel",typeof(RectTransform),typeof(Canvas));g.transform.SetParent(root.transform,false);g.transform.position=pos;g.transform.rotation=Quaternion.Euler(0,180,0);g.transform.localScale=Vector3.one*.007f;Canvas c=g.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;c.sortingOrder=5;RectTransform r=g.GetComponent<RectTransform>();r.sizeDelta=new Vector2(130,70);GameObject tg=new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));tg.transform.SetParent(g.transform,false);RectTransform tr=tg.GetComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;UnityEngine.UI.Text t=tg.GetComponent<UnityEngine.UI.Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=text;t.fontSize=25;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;}
        private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}
        private Material Mat(Color c,float sm,float me=0){Material m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",sm);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",me);materials.Add(m);return m;}
        private static void AddInteractable(GameObject g,string l,int p,Action a){WorldInteractable i=g.GetComponent<WorldInteractable>()??g.AddComponent<WorldInteractable>();i.label=l;i.priority=p;i.action=a;i.maxDistance=3.4f;}
        private void OnDestroy(){foreach(Material m in materials)if(m!=null)Destroy(m);}
    }
}
