using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    public sealed class InventoryTerminalLayer : MonoBehaviour
    {
        private GameObject root;private Material body,screen;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<InventoryTerminalLayer>()!=null)return;GameObject g=new GameObject("ForgeBench_InventoryTerminalLayer");DontDestroyOnLoad(g);g.AddComponent<InventoryTerminalLayer>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;yield return null;Build();}
        private void Build(){if(root!=null)return;root=new GameObject("InventoryTerminal");Shader sh=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");body=new Material(sh);body.color=new Color(.06f,.07f,.08f);screen=new Material(sh);screen.color=new Color(.05f,.31f,.24f);Vector3 p=new Vector3(-4.55f,.92f,-2.18f);Box("InventoryPedestal",p-new Vector3(0,.40f,0),new Vector3(.80f,.90f,.58f),body);GameObject d=Box("InventoryScreen",p+new Vector3(0,.25f,-.05f),new Vector3(1.02f,.62f,.08f),screen);d.transform.rotation=Quaternion.Euler(-8f,0f,0f);WorldInteractable i=d.AddComponent<WorldInteractable>();i.label="Open virtualized warehouse inventory";i.priority=27;i.maxDistance=3.2f;i.action=VirtualizedInventoryPanel.Open;}
        private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}private void OnDestroy(){if(body!=null)Destroy(body);if(screen!=null)Destroy(screen);}
    }
}
