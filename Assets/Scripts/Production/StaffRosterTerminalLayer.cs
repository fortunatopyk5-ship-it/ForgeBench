using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    public sealed class StaffRosterTerminalLayer : MonoBehaviour
    {
        private GameObject root;private Material body,screen,accent;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<StaffRosterTerminalLayer>()!=null)return;GameObject g=new GameObject("ForgeBench_StaffRosterTerminalLayer");DontDestroyOnLoad(g);g.AddComponent<StaffRosterTerminalLayer>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;yield return null;Build();}
        private void Build(){if(root!=null)return;root=new GameObject("StaffRosterTerminal");Shader sh=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");body=new Material(sh);body.color=new Color(.055f,.065f,.078f);screen=new Material(sh);screen.color=new Color(.19f,.13f,.31f);accent=new Material(sh);accent.color=new Color(.38f,.25f,.65f);Vector3 p=new Vector3(2.05f,.90f,-2.10f);Box("StaffDesk",p,new Vector3(1.40f,.10f,.72f),body);Box("StaffStand",p+new Vector3(0,.34f,.18f),new Vector3(.10f,.62f,.10f),accent);GameObject d=Box("StaffDisplay",p+new Vector3(0,.76f,.14f),new Vector3(1.05f,.58f,.08f),screen);d.transform.rotation=Quaternion.Euler(-7f,0f,0f);WorldInteractable i=d.AddComponent<WorldInteractable>();i.label="Open staff roster / training";i.priority=28;i.maxDistance=3.3f;i.action=StaffRosterPanel.Open;CreateLabel("STAFF / TRAINING",p+new Vector3(0,.78f,.085f));}
        private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}
        private void CreateLabel(string text,Vector3 p){GameObject go=new GameObject("Label_"+text,typeof(RectTransform),typeof(Canvas));go.transform.SetParent(root.transform,false);go.transform.position=p;go.transform.rotation=Quaternion.Euler(0,180,0);go.transform.localScale=Vector3.one*.006f;Canvas c=go.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;RectTransform rt=go.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(1.55f,.34f);GameObject tg=new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));tg.transform.SetParent(go.transform,false);RectTransform tr=tg.GetComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;UnityEngine.UI.Text t=tg.GetComponent<UnityEngine.UI.Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=text;t.fontSize=24;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;}
        private void OnDestroy(){if(body!=null)Destroy(body);if(screen!=null)Destroy(screen);if(accent!=null)Destroy(accent);}
    }
}
