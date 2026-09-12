using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    public sealed class MaintenanceTerminalLayer : MonoBehaviour
    {
        private GameObject root;private Material body,screen,accent;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<MaintenanceTerminalLayer>()!=null)return;GameObject g=new GameObject("ForgeBench_MaintenanceTerminalLayer");DontDestroyOnLoad(g);g.AddComponent<MaintenanceTerminalLayer>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;yield return null;Build();}
        private void Build(){if(root!=null)return;root=new GameObject("MaintenanceTerminal");Shader sh=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");body=new Material(sh);body.color=new Color(.055f,.065f,.078f);screen=new Material(sh);screen.color=new Color(.22f,.21f,.08f);accent=new Material(sh);accent.color=new Color(.68f,.56f,.08f);Vector3 p=new Vector3(-2.65f,.92f,-2.18f);Box("MaintenancePedestal",p-new Vector3(0,.40f,0),new Vector3(.82f,.90f,.58f),body);GameObject d=Box("MaintenanceScreen",p+new Vector3(0,.25f,-.05f),new Vector3(1.04f,.62f,.08f),screen);d.transform.rotation=Quaternion.Euler(-8f,0f,0f);WorldInteractable i=d.AddComponent<WorldInteractable>();i.label="Open maintenance / reliability";i.priority=27;i.maxDistance=3.2f;i.action=MaintenancePanel.Open;Box("ServiceLamp",p+new Vector3(.42f,.67f,-.02f),new Vector3(.08f,.08f,.08f),accent);}
        private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}private void OnDestroy(){if(body!=null)Destroy(body);if(screen!=null)Destroy(screen);if(accent!=null)Destroy(accent);}
    }
}
