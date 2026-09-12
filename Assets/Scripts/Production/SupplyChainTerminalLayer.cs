using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    public sealed class SupplyChainTerminalLayer : MonoBehaviour
    {
        private GameObject root;private readonly List<Material> materials=new List<Material>();private Shader shader;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<SupplyChainTerminalLayer>()!=null)return;GameObject g=new GameObject("ForgeBench_SupplyChainTerminal");DontDestroyOnLoad(g);g.AddComponent<SupplyChainTerminalLayer>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;yield return null;Build();}
        private void Build(){if(root!=null)return;root=new GameObject("SupplyChainTerminalWorld");shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");Vector3 p=new Vector3(-5.85f,1.50f,-.80f);Material dark=Mat(new Color(.045f,.055f,.065f),.42f,.75f),screen=Mat(new Color(.04f,.48f,.76f),.68f,.12f),green=Mat(new Color(.08f,.65f,.37f),.58f,.12f);GameObject display=Box("SupplierNetworkDisplay",p,new Vector3(.92f,.62f,.08f),screen);AddInteractable(display,"Open supplier network",38,SupplyChainPanel.Open);Box("SupplierStatusLed",p+new Vector3(.36f,-.25f,-.05f),new Vector3(.08f,.035f,.016f),green);}
        private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}private Material Mat(Color c,float sm,float me=0){Material m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",sm);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",me);materials.Add(m);return m;}private static void AddInteractable(GameObject g,string l,int p,Action a){WorldInteractable i=g.GetComponent<WorldInteractable>()??g.AddComponent<WorldInteractable>();i.label=l;i.priority=p;i.action=a;i.maxDistance=3.2f;}private void OnDestroy(){foreach(Material m in materials)if(m!=null)Destroy(m);}
    }
}
