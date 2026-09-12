using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Physical front-office terminal for warranty claims and no-charge comeback work.</summary>
    public sealed class WarrantyTerminalLayer : MonoBehaviour
    {
        private GameObject root;
        private Material body,screen,accent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<WarrantyTerminalLayer>() != null) return;
            GameObject go = new GameObject("ForgeBench_WarrantyTerminalLayer");
            DontDestroyOnLoad(go);
            go.AddComponent<WarrantyTerminalLayer>();
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
            root = new GameObject("WarrantyServiceTerminal");
            Shader shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");
            body=Mat(shader,new Color(.055f,.060f,.072f),.52f,.68f);
            screen=Mat(shader,new Color(.25f,.105f,.025f),.62f,.18f);
            accent=Mat(shader,new Color(.94f,.44f,.08f),.58f,.20f);
            Vector3 p=new Vector3(2.65f,.92f,-2.18f);
            Box("WarrantyPedestal",p-new Vector3(0,.41f,0),new Vector3(.82f,.92f,.58f),body);
            GameObject display=Box("WarrantyScreen",p+new Vector3(0,.25f,-.05f),new Vector3(1.04f,.62f,.08f),screen);display.transform.rotation=Quaternion.Euler(-8f,0,0);
            WorldInteractable wi=display.AddComponent<WorldInteractable>();wi.label="Open warranty / comeback desk";wi.priority=31;wi.maxDistance=3.2f;wi.action=WarrantyPanel.Open;
            Box("WarrantyLamp",p+new Vector3(.42f,.67f,-.02f),new Vector3(.08f,.08f,.08f),accent);
        }

        private Material Mat(Shader shader,Color color,float smooth,float metallic){Material m=new Material(shader);m.color=color;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);return m;}
        private GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(root.transform,true);g.transform.position=pos;g.transform.localScale=scale;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;return g;}
        private void OnDestroy(){if(body!=null)Destroy(body);if(screen!=null)Destroy(screen);if(accent!=null)Destroy(accent);}
    }
}
