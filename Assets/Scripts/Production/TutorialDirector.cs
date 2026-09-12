using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>Persistent contextual onboarding tied to real gameplay state.</summary>
    public sealed class TutorialDirector : MonoBehaviour
    {
        private GameRuntime game;
        private Canvas canvas;
        private GameObject panel;
        private Text title;
        private Text body;
        private Button action;
        private float nextEvaluate;

        private IEnumerator Start()
        {
            game=GameRuntime.Instance;
            while(game==null||game.UI==null){game=GameRuntime.Instance;yield return null;}
            Build();
            game.Events?.Subscribe("state.changed",OnStateChanged);
            Evaluate(true);
        }

        private void OnDestroy(){game?.Events?.Unsubscribe("state.changed",OnStateChanged);}
        private void OnStateChanged(object p){Evaluate(false);}
        private void Update(){if(Time.unscaledTime>nextEvaluate){nextEvaluate=Time.unscaledTime+1f;Evaluate(false);}}

        private void Evaluate(bool force)
        {
            if(game?.State==null||panel==null)return;
            if(!game.State.tutorialEnabled||game.State.tutorialStep>=9){panel.SetActive(false);return;}
            int desired=InferStep();
            if(desired>game.State.tutorialStep)
            {
                game.State.tutorialStep=desired;
                game.Saves?.Save(game.State,1);
            }
            panel.SetActive(true);
            Render(game.State.tutorialStep);
        }

        private int InferStep()
        {
            JobState j=game.ActiveJob;MachineState m=game.ActiveMachine;
            if(j==null)return 0;
            if(game.State.shipments.Exists(s=>s.status==ShipmentStatus.InTransit||s.status==ShipmentStatus.Delivered))return 1;
            if(m==null)return 1;
            if(!game.Assembly.InternalsAccessible(m))return 2;
            if(string.IsNullOrEmpty(m.motherboardItemId)||string.IsNullOrEmpty(m.cpuItemId)||m.ramItemIds.Count==0||string.IsNullOrEmpty(m.psuItemId))return 3;
            if(!m.cables.atx24||!m.cables.cpuEps||!m.cables.frontPanel)return 4;
            if(m.postCode!="A0")return 5;
            if(j.requireOs&&(!m.osInstalled||!m.driversInstalled))return 6;
            if(j.requireStable&&(m.benchmarkScore<=0||!m.stressStable))return 7;
            if(j.stage!=JobStage.Completed)return 8;
            return 9;
        }

        private void Render(int step)
        {
            string h="WORKSHOP TRAINING",b="",button="OPEN";UnityEngine.Events.UnityAction fn=null;
            switch(step)
            {
                case 0:b="Choose a contract from the Job Board. Every device enters the workshop through a real persistent job state.";button="JOB BOARD";fn=()=>game.UI.OpenTab("JOBS");break;
                case 1:b="Order needed parts, advance delivery if required, then receive and scan the shipment into inventory.";button="STORE";fn=()=>game.UI.OpenTab("STORE");break;
                case 2:b="Internal components are physically blocked by the side panel. Loosen all fasteners, then remove the panel.";button="ASSEMBLY";fn=()=>game.UI.OpenTab("BENCH");break;
                case 3:b="Install compatible hardware. Empty 3D slots on the main bench are interactable; incompatible choices explain why they are blocked.";button="BENCH";fn=()=>game.UI.OpenTab("BENCH");break;
                case 4:b="Route ATX, EPS, GPU, storage, fan and front-panel connections. POST checks the cable graph.";button="CABLES";fn=()=>game.ConnectCables();break;
                case 5:b="Power the machine on. Resolve the exact POST code instead of bypassing it.";button="POWER / POST";fn=()=>game.PowerOn();break;
                case 6:b="After a clean POST, configure firmware as needed, then install ForgeOS and the correct drivers.";button="OS TOOLS";fn=()=>game.UI.OpenTab("OS");break;
                case 7:b="Run benchmark and stress validation. Temperature, PSU headroom, dust, paste and fan profile affect stability.";button="DIAGNOSTICS";fn=()=>game.UI.OpenTab("DIAG");break;
                default:b="Finish optional objectives, reinstall and tighten the side panel, then validate and submit the contract.";button="JOB BOARD";fn=()=>game.UI.OpenTab("JOBS");break;
            }
            title.text=h+"  "+(step+1)+"/9";body.text=b;
            action.GetComponentInChildren<Text>().text=button;action.onClick.RemoveAllListeners();if(fn!=null)action.onClick.AddListener(fn);
        }

        private void Build()
        {
            Font font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            GameObject cg=new GameObject("ForgeBench_Tutorial");canvas=cg.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=120;CanvasScaler s=cg.AddComponent<CanvasScaler>();s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1920,1080);s.matchWidthOrHeight=.5f;cg.AddComponent<GraphicRaycaster>();DontDestroyOnLoad(cg);
            panel=new GameObject("TutorialCard",typeof(RectTransform),typeof(Image));panel.transform.SetParent(cg.transform,false);RectTransform pr=panel.GetComponent<RectTransform>();pr.anchorMin=new Vector2(.57f,.70f);pr.anchorMax=new Vector2(.965f,.94f);pr.offsetMin=pr.offsetMax=Vector2.zero;panel.GetComponent<Image>().color=new Color(.045f,.058f,.075f,.96f);
            title=MakeText(panel.transform,font,24,TextAnchor.MiddleLeft,Color.white);RectTransform tr=title.rectTransform;tr.anchorMin=new Vector2(.05f,.72f);tr.anchorMax=new Vector2(.95f,.95f);tr.offsetMin=tr.offsetMax=Vector2.zero;
            body=MakeText(panel.transform,font,19,TextAnchor.UpperLeft,new Color(.78f,.84f,.90f,1f));RectTransform br=body.rectTransform;br.anchorMin=new Vector2(.05f,.28f);br.anchorMax=new Vector2(.95f,.72f);br.offsetMin=br.offsetMax=Vector2.zero;
            action=MakeButton(panel.transform,font,"OPEN",new Color(.06f,.52f,.82f,1f));RectTransform ar=action.GetComponent<RectTransform>();ar.anchorMin=new Vector2(.05f,.06f);ar.anchorMax=new Vector2(.72f,.25f);ar.offsetMin=ar.offsetMax=Vector2.zero;
            Button skip=MakeButton(panel.transform,font,"SKIP",new Color(.18f,.20f,.23f,1f));RectTransform sr=skip.GetComponent<RectTransform>();sr.anchorMin=new Vector2(.75f,.06f);sr.anchorMax=new Vector2(.95f,.25f);sr.offsetMin=sr.offsetMax=Vector2.zero;skip.onClick.AddListener(()=>{game.State.tutorialEnabled=false;game.Saves.Save(game.State,1);panel.SetActive(false);});
        }

        private static Text MakeText(Transform p,Font f,int size,TextAnchor align,Color c){GameObject g=new GameObject("Text",typeof(RectTransform),typeof(Text));g.transform.SetParent(p,false);Text t=g.GetComponent<Text>();t.font=f;t.fontSize=size;t.alignment=align;t.color=c;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;}
        private static Button MakeButton(Transform p,Font f,string label,Color c){GameObject g=new GameObject(label,typeof(RectTransform),typeof(Image),typeof(Button));g.transform.SetParent(p,false);Image i=g.GetComponent<Image>();i.color=c;Button b=g.GetComponent<Button>();b.targetGraphic=i;Text t=MakeText(g.transform,f,18,TextAnchor.MiddleCenter,Color.white);RectTransform r=t.rectTransform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;return b;}
    }
}
