using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    public sealed class ToolCalibrationPanel : MonoBehaviour
    {
        private static ToolCalibrationPanel instance;
        private GameObject root;
        private RectTransform content;
        private Text summary;
        private Font font;

        public static void Open()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ForgeBench_ToolCalibrationPanel");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<ToolCalibrationPanel>();
            }
            instance.Show();
        }

        public static void RefreshIfOpen(){if(instance!=null&&instance.root!=null&&instance.root.activeSelf)instance.Refresh();}

        private void Awake()
        {
            if(instance!=null&&instance!=this){Destroy(gameObject);return;}
            instance=this;DontDestroyOnLoad(gameObject);font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");EnsureEventSystem();Build();root.SetActive(false);
        }

        private void Update(){if(root!=null&&root.activeSelf&&Input.GetKeyDown(KeyCode.Escape)&&!Application.isMobilePlatform)Close();}
        private void Show(){root.SetActive(true);Cursor.lockState=CursorLockMode.None;MobileInputState.Move=Vector2.zero;Refresh();}
        private void Close(){if(root!=null)root.SetActive(false);Cursor.lockState=CursorLockMode.Locked;}

        private void Build()
        {
            root=UI("ToolCalibrationRoot",transform);Canvas canvas=root.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=25450;CanvasScaler scaler=root.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=.5f;root.AddComponent<GraphicRaycaster>();
            GameObject bg=UI("BG",root.transform);Stretch(bg.GetComponent<RectTransform>());bg.AddComponent<Image>().color=new Color(.009f,.015f,.019f,.995f);
            GameObject panel=UI("Panel",bg.transform);RectTransform pr=panel.GetComponent<RectTransform>();pr.anchorMin=new Vector2(.07f,.06f);pr.anchorMax=new Vector2(.93f,.94f);pr.offsetMin=pr.offsetMax=Vector2.zero;panel.AddComponent<Image>().color=new Color(.045f,.056f,.061f,1f);
            Text title=Text("TOOL METROLOGY / MAINTENANCE",panel.transform,30,TextAnchor.MiddleLeft,Color.white);Set(title.rectTransform,.03f,.91f,.62f,.985f);
            summary=Text("",panel.transform,16,TextAnchor.MiddleRight,new Color(.64f,.75f,.78f,1f));Set(summary.rectTransform,.52f,.91f,.86f,.985f);
            Button close=Button("✕ CLOSE",panel.transform,new Color(.45f,.12f,.13f,1f),Close);Set(close.GetComponent<RectTransform>(),.875f,.925f,.965f,.975f);
            GameObject scroll=UI("Scroll",panel.transform);Set(scroll.GetComponent<RectTransform>(),.025f,.035f,.975f,.895f);ScrollRect sc=scroll.AddComponent<ScrollRect>();sc.horizontal=false;sc.scrollSensitivity=46f;
            GameObject viewport=UI("Viewport",scroll.transform);Stretch(viewport.GetComponent<RectTransform>());Image vi=viewport.AddComponent<Image>();vi.color=new Color(.018f,.027f,.030f,.8f);Mask mask=viewport.AddComponent<Mask>();mask.showMaskGraphic=false;sc.viewport=viewport.GetComponent<RectTransform>();
            GameObject body=UI("Content",viewport.transform);content=body.GetComponent<RectTransform>();content.anchorMin=new Vector2(0,1);content.anchorMax=new Vector2(1,1);content.pivot=new Vector2(.5f,1);VerticalLayoutGroup vg=body.AddComponent<VerticalLayoutGroup>();vg.padding=new RectOffset(18,18,16,22);vg.spacing=10;vg.childControlHeight=true;vg.childForceExpandHeight=false;vg.childControlWidth=true;vg.childForceExpandWidth=true;ContentSizeFitter fit=body.AddComponent<ContentSizeFitter>();fit.verticalFit=ContentSizeFitter.FitMode.PreferredSize;sc.content=content;
        }

        private void Refresh()
        {
            for(int i=content.childCount-1;i>=0;i--)Destroy(content.GetChild(i).gameObject);
            ToolCalibrationService service=ToolCalibrationDirector.Instance?.Service;
            GameRuntime game=GameRuntime.Instance;
            if(service==null||game==null){Card("INITIALIZING","Metrology register is starting.",new Color(.55f,.60f,.62f,1f),null);return;}
            float bench=ToolCalibrationRules.BenchReadiness(service.Tools,game.State.day);int unsafeCount=service.Tools.Count(x=>ToolCalibrationRules.Health(x,game.State.day)==ToolHealthState.Unsafe);int due=service.Tools.Count(x=>ToolCalibrationRules.Health(x,game.State.day)==ToolHealthState.CalibrationDue||ToolCalibrationRules.Health(x,game.State.day)==ToolHealthState.ServiceDue);
            summary.text="BENCH "+Mathf.RoundToInt(bench*100f)+"%   ·   DUE "+due+"   ·   LOCKOUT "+unsafeCount;
            Text intro=Text("Tools are tracked per physical inventory instance. Completed jobs add wear/drift; elapsed days add calibration drift. Unsafe tools lock out until serviced.",content,16,TextAnchor.UpperLeft,new Color(.65f,.74f,.76f,1f));intro.gameObject.AddComponent<LayoutElement>().preferredHeight=52;
            JobState job=game.ActiveJob;if(job!=null){float ready=service.Readiness(job.type);Card("ACTIVE JOB READINESS · "+job.type,"Relevant-tool confidence: "+Mathf.RoundToInt(ready*100f)+"% · "+job.jobId,ready>=.78f?new Color(.08f,.58f,.36f,1f):ready>=.62f?new Color(.72f,.52f,.08f,1f):new Color(.72f,.14f,.12f,1f),()=>ToolCalibrationDirector.Instance?.ActiveJobReadiness());}
            if(service.Tools.Count==0)Card("NO REGISTERED TOOLS","Buy or receive Tool-category inventory to register it automatically.",new Color(.58f,.36f,.08f,1f),null);
            foreach(ToolCalibrationRecord r in service.Tools.OrderBy(x=>ToolCalibrationRules.Health(x,game.State.day)).ThenBy(x=>x.definitionId))ToolCard(r,game);
        }

        private void ToolCard(ToolCalibrationRecord r,GameRuntime game)
        {
            HardwareDefinition d=game.Catalog.Get(r.definitionId);ToolHealthState health=ToolCalibrationRules.Health(r,game.State.day);float accuracy=ToolCalibrationRules.Accuracy(r,game.State.day);
            GameObject g=UI("Tool_"+r.instanceId,content);g.AddComponent<Image>().color=new Color(.072f,.087f,.092f,1f);LayoutElement le=g.AddComponent<LayoutElement>();le.preferredHeight=172;
            GameObject bar=UI("Accent",g.transform);Set(bar.GetComponent<RectTransform>(),0,0,.007f,1);bar.AddComponent<Image>().color=HealthColor(health);
            Text title=Text((d?.model??r.definitionId)+"  ·  "+health,g.transform,20,TextAnchor.UpperLeft,Color.white);title.fontStyle=FontStyle.Bold;Set(title.rectTransform,.025f,.69f,.66f,.92f);
            string detail="ID "+Short(r.instanceId)+" · accuracy "+Mathf.RoundToInt(accuracy*100f)+"% · condition "+Mathf.RoundToInt(r.condition*100f)+"% · calibration "+Mathf.RoundToInt(r.calibration*100f)+"%\n"+"drift "+Mathf.RoundToInt(r.drift*100f)+"% · contamination "+Mathf.RoundToInt(r.contamination*100f)+"% · cycles "+r.usageCycles+" · next calibration day "+r.calibrationDueDay;
            Text dt=Text(detail,g.transform,14,TextAnchor.UpperLeft,new Color(.64f,.73f,.76f,1f));Set(dt.rectTransform,.025f,.15f,.69f,.69f);
            Button service=Button("SERVICE",g.transform,new Color(.16f,.38f,.34f,1f),()=>{ToolCalibrationDirector.Instance?.ServiceTool(r.instanceId);Refresh();});Set(service.GetComponent<RectTransform>(),.72f,.18f,.84f,.48f);
            Button cal=Button(ToolCalibrationRules.RequiresCalibration(r.definitionId)?"CALIBRATE":"NO CAL",g.transform,new Color(.10f,.40f,.57f,1f),()=>{ToolCalibrationDirector.Instance?.Calibrate(r.instanceId);Refresh();});cal.interactable=ToolCalibrationRules.RequiresCalibration(r.definitionId);Set(cal.GetComponent<RectTransform>(),.85f,.18f,.975f,.48f);
        }

        private void Card(string title,string detail,Color accent,Action action)
        {
            GameObject g=UI("Card",content);g.AddComponent<Image>().color=new Color(.073f,.088f,.093f,1f);LayoutElement le=g.AddComponent<LayoutElement>();le.preferredHeight=action==null?112:130;GameObject bar=UI("Accent",g.transform);Set(bar.GetComponent<RectTransform>(),0,0,.007f,1);bar.AddComponent<Image>().color=accent;Text a=Text(title,g.transform,19,TextAnchor.UpperLeft,Color.white);a.fontStyle=FontStyle.Bold;Set(a.rectTransform,.025f,.58f,.77f,.90f);Text b=Text(detail,g.transform,14,TextAnchor.UpperLeft,new Color(.64f,.72f,.74f,1f));Set(b.rectTransform,.025f,.12f,.78f,.60f);if(action!=null){Button run=Button("CHECK",g.transform,new Color(.10f,.40f,.55f,1f),action);Set(run.GetComponent<RectTransform>(),.82f,.24f,.965f,.70f);}
        }

        private static Color HealthColor(ToolHealthState h){switch(h){case ToolHealthState.Ready:return new Color(.08f,.58f,.35f,1f);case ToolHealthState.ServiceDue:return new Color(.72f,.50f,.08f,1f);case ToolHealthState.CalibrationDue:return new Color(.82f,.35f,.07f,1f);default:return new Color(.75f,.10f,.12f,1f);}}
        private static string Short(string id){if(string.IsNullOrEmpty(id))return "?";return id.Length<=10?id:id.Substring(0,10);}
        private Button Button(string label,Transform parent,Color color,Action action){GameObject g=UI("Button_"+label,parent);Image im=g.AddComponent<Image>();im.color=color;Button b=g.AddComponent<Button>();b.targetGraphic=im;if(action!=null)b.onClick.AddListener(()=>action());Text t=Text(label,g.transform,14,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,3);t.raycastTarget=false;return b;}
        private Text Text(string value,Transform parent,int size,TextAnchor align,Color color){GameObject g=UI("Text",parent);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=color;t.raycastTarget=false;return t;}
        private static GameObject UI(string name,Transform parent){GameObject g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(parent,false);return g;}
        private static void Stretch(RectTransform r,float inset=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(inset,inset);r.offsetMax=new Vector2(-inset,-inset);}
        private static void Set(RectTransform r,float x0,float y0,float x1,float y1){r.anchorMin=new Vector2(x0,y0);r.anchorMax=new Vector2(x1,y1);r.offsetMin=r.offsetMax=Vector2.zero;}
        private static void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(e);}
    }
}