using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    public sealed class WorkshopManagementTerminalLayer : MonoBehaviour
    {
        private GameObject root;private readonly List<Material> materials=new List<Material>();private Shader shader;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<WorkshopManagementTerminalLayer>()!=null)return;GameObject g=new GameObject("ForgeBench_WorkshopManagementTerminal");DontDestroyOnLoad(g);g.AddComponent<WorkshopManagementTerminalLayer>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;yield return null;Build();}
        private void Build(){if(root!=null)return;root=new GameObject("WorkshopManagementWorld");shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");Vector3 p=new Vector3(4.86f,1.46f,-3.28f);Material dark=Mat(new Color(.045f,.055f,.065f),.42f,.75f),screen=Mat(new Color(.04f,.44f,.68f),.68f,.12f),accent=Mat(new Color(.08f,.66f,.40f),.58f,.15f);GameObject display=Box("ManagementConsole",p,new Vector3(1.26f,.70f,.07f),screen);AddInteractable(display,"Open workshop management",35,()=>WorkshopBusinessPanel.Open(0));Box("ManagementStatus",p+new Vector3(.52f,-.27f,-.045f),new Vector3(.14f,.05f,.018f),accent);}
        private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}private Material Mat(Color c,float sm,float me=0){Material m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",sm);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",me);materials.Add(m);return m;}private static void AddInteractable(GameObject g,string l,int p,Action a){WorldInteractable i=g.GetComponent<WorldInteractable>()??g.AddComponent<WorldInteractable>();i.label=l;i.priority=p;i.action=a;i.maxDistance=3.2f;}private void OnDestroy(){foreach(Material m in materials)if(m!=null)Destroy(m);}
    }
}
