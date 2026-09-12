using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    public sealed class ProgressionTerminalLayer : MonoBehaviour
    {
        private GameObject root;private Material body,screen,trim;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<ProgressionTerminalLayer>()!=null)return;GameObject go=new GameObject("ForgeBench_ProgressionTerminalLayer");DontDestroyOnLoad(go);go.AddComponent<ProgressionTerminalLayer>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;yield return null;Build();}
        private void Build(){if(root!=null)return;root=new GameObject("CareerCertificationTerminal");Shader sh=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");body=new Material(sh);body.color=new Color(.05f,.06f,.075f);screen=new Material(sh);screen.color=new Color(.20f,.12f,.34f);trim=new Material(sh);trim.color=new Color(.23f,.24f,.27f);Vector3 p=new Vector3(3.75f,.90f,-2.10f);Box("CareerDesk",p,new Vector3(1.40f,.10f,.72f),trim);Box("CareerStand",p+new Vector3(0,.34f,.18f),new Vector3(.10f,.62f,.10f),body);GameObject d=Box("CareerDisplay",p+new Vector3(0,.76f,.14f),new Vector3(1.05f,.58f,.08f),screen);d.transform.rotation=Quaternion.Euler(-7f,0f,0f);WorldInteractable i=d.AddComponent<WorldInteractable>();i.label="Open career / certifications";i.priority=28;i.maxDistance=3.3f;i.action=()=>ProgressionPanel.Open();CreateLabel("CERTIFICATIONS",p+new Vector3(0,.78f,.085f));}
        private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}
        private void CreateLabel(string text,Vector3 p){GameObject go=new GameObject("Label_"+text,typeof(RectTransform),typeof(Canvas));go.transform.SetParent(root.transform,false);go.transform.position=p;go.transform.rotation=Quaternion.Euler(0,180,0);go.transform.localScale=Vector3.one*.006f;Canvas c=go.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;RectTransform rt=go.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(1.5f,.34f);GameObject tg=new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));tg.transform.SetParent(go.transform,false);RectTransform tr=tg.GetComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;UnityEngine.UI.Text t=tg.GetComponent<UnityEngine.UI.Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=text;t.fontSize=24;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;}
        private void OnDestroy(){if(body!=null)Destroy(body);if(screen!=null)Destroy(screen);if(trim!=null)Destroy(trim);}
    }
}
