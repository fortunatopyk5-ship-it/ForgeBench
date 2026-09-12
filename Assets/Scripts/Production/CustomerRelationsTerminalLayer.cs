using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical CRM terminal added to the workshop so customer systems are reachable without hidden debug menus.</summary>
    public sealed class CustomerRelationsTerminalLayer : MonoBehaviour
    {
        private GameObject root;
        private Material body, screen, trim;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<CustomerRelationsTerminalLayer>() != null) return;
            GameObject go = new GameObject("ForgeBench_CustomerTerminalLayer");
            DontDestroyOnLoad(go);
            go.AddComponent<CustomerRelationsTerminalLayer>();
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
            root = new GameObject("CustomerRelationsTerminal");
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            body = new Material(shader); body.color = new Color(.055f,.065f,.078f);
            screen = new Material(shader); screen.color = new Color(.03f,.22f,.32f);
            trim = new Material(shader); trim.color = new Color(.22f,.24f,.27f);

            Vector3 p = new Vector3(5.45f,.90f,-2.10f);
            Box("CRMDesk",p,new Vector3(1.55f,.10f,.72f),trim);
            Box("CRMStand",p+new Vector3(0,.35f,.19f),new Vector3(.10f,.65f,.10f),body);
            GameObject display=Box("CRMDisplay",p+new Vector3(0,.78f,.15f),new Vector3(1.15f,.62f,.08f),screen);
            display.transform.rotation=Quaternion.Euler(-7f,0f,0f);
            WorldInteractable i=display.AddComponent<WorldInteractable>();i.label="Open customer relations / CRM";i.priority=28;i.maxDistance=3.3f;i.action=()=>CustomerRelationsPanel.Open();
            CreateLabel("CUSTOMER RELATIONS",p+new Vector3(0,.80f,.095f));
        }

        private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}
        private void CreateLabel(string text,Vector3 p){GameObject go=new GameObject("Label_"+text,typeof(RectTransform),typeof(Canvas));go.transform.SetParent(root.transform,false);go.transform.position=p;go.transform.rotation=Quaternion.Euler(0,180,0);go.transform.localScale=Vector3.one*.006f;Canvas c=go.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;RectTransform rt=go.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(1.7f,.36f);GameObject tg=new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));tg.transform.SetParent(go.transform,false);RectTransform tr=tg.GetComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;UnityEngine.UI.Text t=tg.GetComponent<UnityEngine.UI.Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=text;t.fontSize=24;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;}
        private void OnDestroy(){if(body!=null)Destroy(body);if(screen!=null)Destroy(screen);if(trim!=null)Destroy(trim);}
    }
}
