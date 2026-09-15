using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Environmental polish for the procedural workshop. Realtime lights and probes
    /// consume the shared RuntimeRenderBudget so desktop/mobile governors cannot fight
    /// a third independent presentation policy.
    /// </summary>
    public sealed class WorkshopAtmosphereDirector : MonoBehaviour
    {
        private GameRuntime game;
        private GameObject root;
        private readonly List<Material> mats=new List<Material>();
        private readonly List<Transform> fanRotors=new List<Transform>();
        private readonly List<Light> taskLights=new List<Light>();
        private readonly List<ReflectionProbe> reflectionProbes=new List<ReflectionProbe>();
        private Shader shader;
        private int shownDay=-1;
        private UnityEngine.UI.Text clockText;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if(FindAnyObjectByType<WorkshopAtmosphereDirector>()!=null)return;
            GameObject g=new GameObject("ForgeBench_WorkshopAtmosphere");
            DontDestroyOnLoad(g);
            g.AddComponent<WorkshopAtmosphereDirector>();
        }

        private IEnumerator Start()
        {
            while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;
            game=GameRuntime.Instance;
            shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");
            Build();
            RuntimeRenderBudget.Changed+=OnRenderBudgetChanged;
            ApplyRenderBudget();
        }

        private void Build()
        {
            if(root!=null)return;
            root=new GameObject("WorkshopAtmosphere");
            root.transform.SetParent(transform,false);
            Material duct=Mat(new Color(.20f,.22f,.24f),.48f,.75f);
            Material dark=Mat(new Color(.04f,.05f,.06f),.35f,.60f);
            Material safe=Mat(new Color(.08f,.42f,.17f),.45f,.20f);
            Material red=Mat(new Color(.60f,.05f,.035f),.42f,.25f);
            Material em=Mat(new Color(.75f,.90f,1f),.80f,.05f);
            if(em.HasProperty("_EmissionColor")){em.EnableKeyword("_EMISSION");em.SetColor("_EmissionColor",new Color(.65f,.85f,1f)*1.6f);}
            for(int x=-5;x<=5;x+=5)
            {
                Box("CeilingDuct",new Vector3(x,4.48f,0),new Vector3(.62f,.44f,10.5f),duct);
                for(int z=-4;z<=4;z+=4)
                {
                    GameObject rotor=Cylinder("VentRotor",new Vector3(x,4.21f,z),new Vector3(.24f,.025f,.24f),Quaternion.Euler(90,0,0),dark);
                    fanRotors.Add(rotor.transform);
                }
            }
            Box("ExitSign",new Vector3(0,3.55f,-5.70f),new Vector3(1.0f,.30f,.06f),safe);
            Label("EXIT",new Vector3(0,3.55f,-5.62f),new Vector2(.9f,.25f),24);
            Cylinder("Extinguisher",new Vector3(-6.35f,.82f,-4.8f),new Vector3(.14f,.55f,.14f),Quaternion.identity,red);
            Box("FirstAid",new Vector3(-6.35f,1.65f,-4.78f),new Vector3(.48f,.52f,.12f),safe);
            Label("+",new Vector3(-6.35f,1.65f,-4.64f),new Vector2(.30f,.30f),30);
            BuildClock();
            BuildTaskLights(em);
            BuildReflectionProbes();
        }

        private void BuildTaskLights(Material em)
        {
            Vector3[] p={new Vector3(0,2.75f,2.6f),new Vector3(-4,2.8f,2.6f),new Vector3(4,2.8f,2.6f),new Vector3(0,3.7f,-2.5f)};
            int count=Application.isMobilePlatform?3:p.Length;
            for(int i=0;i<count;i++)
            {
                Box("TaskLightFixture"+i,p[i],new Vector3(1.15f,.07f,.18f),em);
                GameObject l=new GameObject("TaskLight"+i);
                l.transform.SetParent(root.transform);
                l.transform.position=p[i]-new Vector3(0,.10f,0);
                Light light=l.AddComponent<Light>();
                light.type=LightType.Point;
                light.color=new Color(.80f,.90f,1f);
                light.range=Application.isMobilePlatform?3.9f:4.8f;
                light.intensity=i==0?1.15f:.72f;
                light.shadows=LightShadows.None;
                taskLights.Add(light);
            }
        }

        private void BuildReflectionProbes()
        {
            bool mobile=Application.isMobilePlatform;
            Vector3[] p=mobile
                ?new[]{new Vector3(0,2.1f,1.6f)}
                :new[]{new Vector3(0,2.1f,2.7f),new Vector3(-4.6f,2f,-2.2f),new Vector3(4.6f,2f,-2.2f)};
            for(int i=0;i<p.Length;i++)
            {
                GameObject g=new GameObject("WorkshopReflectionProbe"+i);
                g.transform.SetParent(root.transform);
                g.transform.position=p[i];
                ReflectionProbe r=g.AddComponent<ReflectionProbe>();
                r.mode=UnityEngine.Rendering.ReflectionProbeMode.Realtime;
                r.refreshMode=UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
                r.size=mobile?new Vector3(8f,4f,8f):new Vector3(5.5f,4f,5.5f);
                r.intensity=mobile ? .38f : .55f;
                r.cullingMask=~0;
                r.enabled=false;
                reflectionProbes.Add(r);
            }
        }

        private void OnRenderBudgetChanged(int tier){ApplyRenderBudget();}

        private void ApplyRenderBudget()
        {
            int lightCount=Mathf.Min(taskLights.Count,RuntimeRenderBudget.RealtimeLightBudget);
            for(int i=0;i<taskLights.Count;i++)if(taskLights[i]!=null)taskLights[i].enabled=i<lightCount;

            int resolution=RuntimeRenderBudget.ReflectionProbeResolution;
            int probeCount=resolution<=0?0:(Application.isMobilePlatform?1:(RuntimeRenderBudget.CurrentTier<=1?1:RuntimeRenderBudget.CurrentTier==2?2:reflectionProbes.Count));
            for(int i=0;i<reflectionProbes.Count;i++)
            {
                ReflectionProbe probe=reflectionProbes[i];
                if(probe==null)continue;
                bool active=i<probeCount;
                probe.enabled=active;
                if(!active)continue;
                bool changed=probe.resolution!=resolution;
                probe.resolution=resolution;
                if(changed||probe.texture==null)probe.RenderProbe();
            }
        }

        private void BuildClock()
        {
            GameObject go=new GameObject("WorkshopClock",typeof(RectTransform),typeof(Canvas));
            go.transform.SetParent(root.transform,false);go.transform.position=new Vector3(0,3.45f,5.60f);go.transform.rotation=Quaternion.Euler(0,180,0);go.transform.localScale=Vector3.one*.01f;
            Canvas c=go.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;
            RectTransform rt=go.GetComponent<RectTransform>();rt.sizeDelta=new Vector2(2.2f,.65f);
            GameObject tg=new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));tg.transform.SetParent(go.transform,false);
            RectTransform tr=tg.GetComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;
            clockText=tg.GetComponent<UnityEngine.UI.Text>();clockText.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");clockText.fontSize=28;clockText.alignment=TextAnchor.MiddleCenter;clockText.color=Color.white;
        }

        private void Update()
        {
            game=GameRuntime.Instance;
            float speed=game?.State?.settings?.reducedMotion==true?0f:85f;
            foreach(Transform t in fanRotors)if(t!=null&&speed>0f)t.Rotate(Vector3.forward,speed*Time.unscaledDeltaTime,Space.Self);
            if(game?.State==null||clockText==null)return;
            if(shownDay!=game.State.day)
            {
                shownDay=game.State.day;
                clockText.text="DAY "+shownDay+"  ·  WORKSHOP OPEN";
                RenderSettings.ambientIntensity=Mathf.Lerp(.72f,1.05f,Mathf.Clamp01(game.State.workshop.cleanliness));
            }
        }

        private GameObject Box(string n,Vector3 p,Vector3 s,Material m)
        {
            GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;
        }
        private GameObject Cylinder(string n,Vector3 p,Vector3 s,Quaternion q,Material m)
        {
            GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;g.transform.rotation=q;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;
        }
        private Material Mat(Color c,float smooth,float metal=0f)
        {
            Material m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metal);mats.Add(m);return m;
        }
        private void Label(string text,Vector3 p,Vector2 size,int fs)
        {
            GameObject go=new GameObject("Label_"+text,typeof(RectTransform),typeof(Canvas));go.transform.SetParent(root.transform,false);go.transform.position=p;go.transform.rotation=Quaternion.Euler(0,180,0);go.transform.localScale=Vector3.one*.01f;
            Canvas c=go.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;RectTransform rt=go.GetComponent<RectTransform>();rt.sizeDelta=size;
            GameObject tg=new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));tg.transform.SetParent(go.transform,false);RectTransform tr=tg.GetComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;
            UnityEngine.UI.Text t=tg.GetComponent<UnityEngine.UI.Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=text;t.fontSize=fs;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;
        }

        private void OnDestroy()
        {
            RuntimeRenderBudget.Changed-=OnRenderBudgetChanged;
            foreach(Material m in mats)if(m!=null)Destroy(m);
        }
    }
}
