using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>Dedicated contract board for non-desktop repair disciplines.</summary>
    public sealed class SpecialistContractBoard : MonoBehaviour
    {
        private static SpecialistContractBoard instance;
        private GameRuntime game;
        private GameObject root;
        private RectTransform content;
        private Font font;

        public static void Open()
        {
            if(instance==null)
            {
                GameObject go=new GameObject("ForgeBench_SpecialistContractBoard");
                DontDestroyOnLoad(go);instance=go.AddComponent<SpecialistContractBoard>();
            }
            instance.Show();
        }

        private void Awake()
        {
            if(instance!=null&&instance!=this){Destroy(gameObject);return;}instance=this;DontDestroyOnLoad(gameObject);font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");EnsureEventSystem();Build();root.SetActive(false);
        }

        private void Update(){if(root!=null&&root.activeSelf&&Input.GetKeyDown(KeyCode.Escape)&&!Application.isMobilePlatform)Close();}
        private void Show(){game=GameRuntime.Instance;if(root==null)Build();root.SetActive(true);Cursor.lockState=CursorLockMode.None;MobileInputState.Move=Vector2.zero;Refresh();}
        private void Close(){if(root!=null)root.SetActive(false);Cursor.lockState=CursorLockMode.Locked;}

        private void Build()
        {
            if(root!=null)return;root=new GameObject("SpecialistContractsRoot",typeof(RectTransform));root.transform.SetParent(transform,false);Canvas c=root.AddComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;c.sortingOrder=26000;CanvasScaler sc=root.AddComponent<CanvasScaler>();sc.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;sc.referenceResolution=new Vector2(1920,1080);sc.matchWidthOrHeight=.5f;root.AddComponent<GraphicRaycaster>();
            GameObject bg=UI("BG",root.transform);Stretch(bg.GetComponent<RectTransform>());bg.AddComponent<Image>().color=new Color(.015f,.022f,.031f,.99f);
            GameObject card=UI("Card",bg.transform);RectTransform cr=card.GetComponent<RectTransform>();cr.anchorMin=new Vector2(.08f,.06f);cr.anchorMax=new Vector2(.92f,.94f);cr.offsetMin=cr.offsetMax=Vector2.zero;card.AddComponent<Image>().color=new Color(.055f,.068f,.085f,1f);
            Text title=Text("SPECIALIST CONTRACTS",card.transform,34,TextAnchor.MiddleLeft,Color.white);RectTransform tr=title.rectTransform;tr.anchorMin=new Vector2(.035f,.90f);tr.anchorMax=new Vector2(.70f,.985f);tr.offsetMin=tr.offsetMax=Vector2.zero;
            Button close=Button("✕ CLOSE",card.transform,new Color(.52f,.12f,.13f,1f),Close);RectTransform xr=close.GetComponent<RectTransform>();xr.anchorMin=new Vector2(.80f,.91f);xr.anchorMax=new Vector2(.965f,.975f);xr.offsetMin=xr.offsetMax=Vector2.zero;
            GameObject scroll=UI("Scroll",card.transform);RectTransform sr=scroll.GetComponent<RectTransform>();sr.anchorMin=new Vector2(.03f,.03f);sr.anchorMax=new Vector2(.97f,.89f);sr.offsetMin=sr.offsetMax=Vector2.zero;ScrollRect s=scroll.AddComponent<ScrollRect>();s.horizontal=false;s.scrollSensitivity=42f;GameObject viewport=UI("Viewport",scroll.transform);Stretch(viewport.GetComponent<RectTransform>());Image vi=viewport.AddComponent<Image>();vi.color=new Color(.025f,.033f,.043f,.7f);Mask mask=viewport.AddComponent<Mask>();mask.showMaskGraphic=false;s.viewport=viewport.GetComponent<RectTransform>();GameObject body=UI("Content",viewport.transform);content=body.GetComponent<RectTransform>();content.anchorMin=new Vector2(0,1);content.anchorMax=new Vector2(1,1);content.pivot=new Vector2(.5f,1);content.sizeDelta=new Vector2(0,500);VerticalLayoutGroup vg=body.AddComponent<VerticalLayoutGroup>();vg.padding=new RectOffset(20,20,18,20);vg.spacing=11;vg.childControlHeight=true;vg.childForceExpandHeight=false;vg.childControlWidth=true;vg.childForceExpandWidth=true;ContentSizeFitter fit=body.AddComponent<ContentSizeFitter>();fit.verticalFit=ContentSizeFitter.FitMode.PreferredSize;s.content=content;
        }

        private void Refresh()
        {
            game=GameRuntime.Instance;if(content==null||game==null)return;for(int i=content.childCount-1;i>=0;i--)Destroy(content.GetChild(i).gameObject);
            JobState active=game.ActiveJob;
            if(active!=null)
            {
                bool specialist=SpecialistJobService.IsSpecialist(active);
                Card("ACTIVE CONTRACT",active.jobId+" · "+active.title+"\n"+active.description+"\nDay "+game.State.day+" / due "+active.dueDay+" · reward $"+active.reward.ToString("0"),specialist?new Color(.18f,.82f,.48f,1f):new Color(.95f,.62f,.20f,1f));
                if(specialist)
                {
                    Wide("VALIDATE & DELIVER SPECIALIST JOB",()=>{game.SubmitSpecialistContract();Refresh();},new Color(.12f,.58f,.38f,1f));
                    Section("SERVICE PARTS");
                    Row(new[]{A("ORDER BATTERY",()=>game.OrderSpecialistPart(PartCategory.Battery)),A("ORDER DISPLAY",()=>game.OrderSpecialistPart(PartCategory.Display)),A("ORDER STORAGE",()=>game.OrderSpecialistPart(PartCategory.Storage)),A("RECEIVE DELIVERIES",()=>game.ReceiveAll())});
                }
                Card("CONTRACT BOARD LOCKED","Finish the active contract before accepting another.",new Color(.72f,.76f,.82f,1f));return;
            }

            Heading("AVAILABLE SPECIALIST WORK");
            Heading("These contracts use dedicated persistent repair state and cannot be completed through the normal desktop validation path.");
            Contract("CUSTOM LIQUID-LOOP WORKSTATION","Build, fill, bleed and leak-test a custom loop.",SpecialistContractKind.LiquidBuild,SpecialistPanelMode.Liquid);
            Contract("BOARD-LEVEL POWER SHORT","ESD, microscope, rail measurement, fault localization and controlled rework.",SpecialistContractKind.BoardRepair,SpecialistPanelMode.Board);
            Contract("LAPTOP BATTERY / CHARGE PATH","Safe disassembly, battery replacement, charging service and reassembly.",SpecialistContractKind.LaptopBattery,SpecialistPanelMode.Portable);
            Contract("PHONE DISPLAY / SEAL","Display separation/replacement with battery isolation and enclosure reseal.",SpecialistContractKind.PhoneDisplay,SpecialistPanelMode.Portable);
            Contract("CONTROLLER DRIFT SERVICE","Disassembly, isolation, analog calibration and reassembly.",SpecialistContractKind.ConsoleController,SpecialistPanelMode.Portable);
            Contract("DEGRADED NAS RECOVERY","Replace failed storage, rebuild RAID and validate network throughput.",SpecialistContractKind.NasRecovery,SpecialistPanelMode.Network);
            Contract("SERVER NETWORK COMMISSIONING","Static addressing, loss/latency checks and storage-array validation.",SpecialistContractKind.ServerNetwork,SpecialistPanelMode.Network);
        }

        private void Contract(string title,string sub,SpecialistContractKind kind,SpecialistPanelMode station)
        {
            GameObject g=UI("Contract",content);g.AddComponent<Image>().color=new Color(.105f,.125f,.155f,1f);HorizontalLayoutGroup h=g.AddComponent<HorizontalLayoutGroup>();h.padding=new RectOffset(18,12,10,10);h.spacing=12;h.childControlHeight=true;h.childForceExpandHeight=true;LayoutElement le=g.AddComponent<LayoutElement>();le.preferredHeight=88;GameObject text=UI("TextBox",g.transform);VerticalLayoutGroup v=text.AddComponent<VerticalLayoutGroup>();v.spacing=3;v.childForceExpandHeight=false;LayoutElement tl=text.AddComponent<LayoutElement>();tl.flexibleWidth=1;Text a=Text(title,text.transform,21,TextAnchor.MiddleLeft,Color.white);a.fontStyle=FontStyle.Bold;Text b=Text(sub,text.transform,16,TextAnchor.UpperLeft,new Color(.63f,.70f,.78f,1f));Button btn=Button("ACCEPT",g.transform,new Color(.08f,.50f,.78f,1f),()=>{game.AcceptSpecialistContract(kind);if(game.ActiveJob!=null){SpecialistRepairPanel.Open(station);Close();}else Refresh();});LayoutElement bl=btn.gameObject.AddComponent<LayoutElement>();bl.preferredWidth=180;
        }

        private struct Act{public string t;public Action a;public Act(string x,Action y){t=x;a=y;}}
        private static Act A(string t,Action a)=>new Act(t,a);
        private void Heading(string a){Text t=Text(a,content,20,TextAnchor.MiddleLeft,new Color(.61f,.70f,.78f,1f));LayoutElement l=t.gameObject.AddComponent<LayoutElement>();l.preferredHeight=55;}
        private void Section(string a){Text t=Text(a,content,22,TextAnchor.MiddleLeft,new Color(.10f,.64f,.92f,1f));t.fontStyle=FontStyle.Bold;LayoutElement l=t.gameObject.AddComponent<LayoutElement>();l.preferredHeight=42;}
        private void Card(string title,string sub,Color c){GameObject g=UI("Card",content);g.AddComponent<Image>().color=new Color(.10f,.12f,.15f,1f);VerticalLayoutGroup v=g.AddComponent<VerticalLayoutGroup>();v.padding=new RectOffset(18,18,12,12);v.spacing=4;v.childControlHeight=true;v.childForceExpandHeight=false;LayoutElement le=g.AddComponent<LayoutElement>();le.minHeight=78;Text a=Text(title,g.transform,22,TextAnchor.MiddleLeft,c);a.fontStyle=FontStyle.Bold;Text b=Text(sub,g.transform,17,TextAnchor.UpperLeft,new Color(.66f,.73f,.80f,1f));b.horizontalOverflow=HorizontalWrapMode.Wrap;b.verticalOverflow=VerticalWrapMode.Overflow;}
        private void Row(Act[] actions){GameObject g=UI("Row",content);HorizontalLayoutGroup h=g.AddComponent<HorizontalLayoutGroup>();h.spacing=10;h.childControlHeight=true;h.childForceExpandHeight=true;h.childForceExpandWidth=true;LayoutElement le=g.AddComponent<LayoutElement>();le.preferredHeight=60;foreach(Act a in actions){Button b=Button(a.t,g.transform,new Color(.13f,.37f,.56f,1f),()=>{a.a?.Invoke();Refresh();});LayoutElement l=b.gameObject.AddComponent<LayoutElement>();l.flexibleWidth=1;}}
        private void Wide(string title,Action a,Color c){Button b=Button(title,content,c,a);LayoutElement l=b.gameObject.AddComponent<LayoutElement>();l.preferredHeight=62;}
        private Button Button(string label,Transform p,Color c,Action a){GameObject g=UI("Button_"+label,p);Image i=g.AddComponent<Image>();i.color=c;Button b=g.AddComponent<Button>();b.targetGraphic=i;if(a!=null)b.onClick.AddListener(()=>a());Text t=Text(label,g.transform,18,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,6);t.raycastTarget=false;return b;}
        private Text Text(string value,Transform p,int size,TextAnchor align,Color c){GameObject g=UI("Text",p);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=c;t.raycastTarget=false;return t;}
        private GameObject UI(string n,Transform p){GameObject g=new GameObject(n,typeof(RectTransform));g.transform.SetParent(p,false);return g;}
        private static void Stretch(RectTransform r,float i=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(i,i);r.offsetMax=new Vector2(-i,-i);}
        private static void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(e);}
    }
}
