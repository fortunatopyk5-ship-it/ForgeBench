using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    public sealed class CustomerRelationsPanel : MonoBehaviour
    {
        private static CustomerRelationsPanel instance;
        private GameObject root;
        private RectTransform content;
        private Font font;
        private int page;

        public static void Open(int initialPage = 0)
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ForgeBench_CustomerRelationsPanel");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<CustomerRelationsPanel>();
            }
            instance.page = Mathf.Clamp(initialPage, 0, 2);
            instance.Show();
        }

        public static void RefreshIfOpen()
        {
            if (instance != null && instance.root != null && instance.root.activeSelf) instance.Refresh();
        }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureEventSystem();
            Build();
            root.SetActive(false);
        }

        private void Update()
        {
            if (root != null && root.activeSelf && Input.GetKeyDown(KeyCode.Escape) && !Application.isMobilePlatform) Close();
        }

        private void Show()
        {
            root.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            MobileInputState.Move = Vector2.zero;
            Refresh();
        }

        private void Close()
        {
            if (root != null) root.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Build()
        {
            root = new GameObject("CustomerRelationsRoot", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            Canvas canvas = root.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 25350;
            CanvasScaler scaler = root.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
            root.AddComponent<GraphicRaycaster>();

            GameObject bg = UI("BG", root.transform); Stretch(bg.GetComponent<RectTransform>()); bg.AddComponent<Image>().color = new Color(.014f, .020f, .029f, .995f);
            GameObject panel = UI("Panel", bg.transform); RectTransform pr = panel.GetComponent<RectTransform>(); pr.anchorMin = new Vector2(.055f,.05f); pr.anchorMax = new Vector2(.945f,.95f); pr.offsetMin = pr.offsetMax = Vector2.zero; panel.AddComponent<Image>().color = new Color(.052f,.064f,.08f,1f);
            GameObject header = UI("Header", panel.transform); RectTransform hr=header.GetComponent<RectTransform>();hr.anchorMin=new Vector2(0,1);hr.anchorMax=new Vector2(1,1);hr.pivot=new Vector2(.5f,1);hr.sizeDelta=new Vector2(0,84);hr.anchoredPosition=Vector2.zero;header.AddComponent<Image>().color=new Color(.072f,.087f,.105f,1f);
            Text title=Text("CUSTOMERS / CRM",header.transform,30,TextAnchor.MiddleLeft,Color.white);RectTransform tr=title.rectTransform;tr.anchorMin=new Vector2(.025f,0);tr.anchorMax=new Vector2(.42f,1);tr.offsetMin=tr.offsetMax=Vector2.zero;
            string[] tabs={"OVERVIEW","CUSTOMERS","REVIEWS"};
            for(int i=0;i<tabs.Length;i++){int capture=i;Button b=Button(tabs[i],header.transform,new Color(.12f,.15f,.19f,1f),()=>{page=capture;Refresh();});RectTransform r=b.GetComponent<RectTransform>();float x=.46f+i*.13f;r.anchorMin=new Vector2(x,.16f);r.anchorMax=new Vector2(x+.12f,.84f);r.offsetMin=r.offsetMax=Vector2.zero;}
            Button close=Button("✕",header.transform,new Color(.55f,.11f,.12f,1f),Close);RectTransform xr=close.GetComponent<RectTransform>();xr.anchorMin=new Vector2(.91f,.16f);xr.anchorMax=new Vector2(.975f,.84f);xr.offsetMin=xr.offsetMax=Vector2.zero;

            GameObject scrollGo=UI("Scroll",panel.transform);RectTransform sr=scrollGo.GetComponent<RectTransform>();sr.anchorMin=new Vector2(.02f,.025f);sr.anchorMax=new Vector2(.98f,.90f);sr.offsetMin=sr.offsetMax=Vector2.zero;ScrollRect scroll=scrollGo.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.scrollSensitivity=42f;
            GameObject viewport=UI("Viewport",scrollGo.transform);Stretch(viewport.GetComponent<RectTransform>());Image vi=viewport.AddComponent<Image>();vi.color=new Color(.024f,.032f,.042f,.78f);Mask mask=viewport.AddComponent<Mask>();mask.showMaskGraphic=false;scroll.viewport=viewport.GetComponent<RectTransform>();
            GameObject body=UI("Content",viewport.transform);content=body.GetComponent<RectTransform>();content.anchorMin=new Vector2(0,1);content.anchorMax=new Vector2(1,1);content.pivot=new Vector2(.5f,1);content.sizeDelta=new Vector2(0,500);VerticalLayoutGroup vg=body.AddComponent<VerticalLayoutGroup>();vg.padding=new RectOffset(20,20,18,22);vg.spacing=11;vg.childControlHeight=true;vg.childForceExpandHeight=false;vg.childControlWidth=true;vg.childForceExpandWidth=true;ContentSizeFitter fit=body.AddComponent<ContentSizeFitter>();fit.verticalFit=ContentSizeFitter.FitMode.PreferredSize;scroll.content=content;
        }

        private void Refresh()
        {
            if (content == null) return;
            for (int i=content.childCount-1;i>=0;i--) Destroy(content.GetChild(i).gameObject);
            CustomerRelationsDirector director = CustomerRelationsDirector.Instance;
            CustomerRelationsService s = director?.Service;
            if (s == null) { Heading("CUSTOMER DATABASE", "Initializing persistent CRM…"); return; }
            Heading(page==0?"CUSTOMER HEALTH":page==1?"CUSTOMER DIRECTORY":"REVIEWS / SERVICE HISTORY",
                "Profiles " + s.Profiles.Count + " · VIP " + s.VipCount + " · Corporate " + s.CorporateCount + " · review score " + s.State.reviewScore + "/100");
            if(page==0) RenderOverview(s); else if(page==1) RenderCustomers(s); else RenderReviews(s);
        }

        private void RenderOverview(CustomerRelationsService s)
        {
            Color sat=s.AverageSatisfaction>=82?new Color(.20f,.82f,.48f,1f):s.AverageSatisfaction>=60?new Color(.95f,.68f,.22f,1f):new Color(.94f,.30f,.25f,1f);
            Card("CUSTOMER HEALTH","Average satisfaction "+s.AverageSatisfaction.ToString("0")+"% · trust "+s.AverageTrust.ToString("0")+"%\nFive-star reviews "+s.State.fiveStarReviews+" · one-star reviews "+s.State.oneStarReviews+" · referrals "+s.State.referralCount,sat);
            int open=s.Profiles.Sum(p=>p.complaints);Card("SERVICE RECOVERY",open+" unresolved complaint(s). Complaints reduce trust every game day until resolved. Goodwill resolution costs money but restores part of the relationship.",open==0?new Color(.20f,.82f,.48f,1f):new Color(.95f,.52f,.20f,1f));
            Section("TOP RELATIONSHIPS");
            foreach(CustomerProfile p in s.Profiles.OrderByDescending(x=>x.trust+x.loyalty).Take(8)) CustomerRow(p, true);
        }

        private void RenderCustomers(CustomerRelationsService s)
        {
            if(s.Profiles.Count==0){Card("NO HISTORY","Complete jobs to create persistent customer profiles.",Color.white);return;}
            foreach(CustomerProfile p in s.Profiles.OrderByDescending(x=>x.vip).ThenByDescending(x=>x.corporate).ThenByDescending(x=>x.lifetimeValue)) CustomerRow(p,false);
        }

        private void CustomerRow(CustomerProfile p,bool compact)
        {
            string badges=(p.vip?"VIP ":"")+(p.corporate?"CORPORATE ":"");if(string.IsNullOrWhiteSpace(badges))badges="STANDARD";
            string detail=badges+" · trust "+p.trust.ToString("0")+"% · loyalty "+p.loyalty.ToString("0")+"% · satisfaction "+p.satisfaction.ToString("0")+"%\nJobs "+p.jobsCompleted+" completed / "+p.jobsFailed+" failed · repeats "+p.repeatJobs+" · referrals "+p.referrals+" · lifetime $"+p.lifetimeValue.ToString("0")+" · preference "+p.preferredDevice;
            if(compact){Card(p.name,detail,p.complaints>0?new Color(.95f,.50f,.20f,1f):p.vip?new Color(.82f,.70f,.22f,1f):Color.white);return;}
            GameObject row=UI("Customer",content);row.AddComponent<Image>().color=new Color(.095f,.115f,.145f,1f);HorizontalLayoutGroup h=row.AddComponent<HorizontalLayoutGroup>();h.padding=new RectOffset(16,10,10,10);h.spacing=10;h.childControlHeight=true;h.childForceExpandHeight=true;LayoutElement rl=row.AddComponent<LayoutElement>();rl.preferredHeight=92;
            GameObject text=UI("Text",row.transform);VerticalLayoutGroup tv=text.AddComponent<VerticalLayoutGroup>();tv.childForceExpandHeight=false;tv.spacing=2;LayoutElement tl=text.AddComponent<LayoutElement>();tl.flexibleWidth=1;Text name=Text(p.name,text.transform,20,TextAnchor.MiddleLeft,p.vip?new Color(.92f,.78f,.28f,1f):Color.white);name.fontStyle=FontStyle.Bold;Text sub=Text(detail,text.transform,15,TextAnchor.UpperLeft,new Color(.64f,.72f,.80f,1f));sub.horizontalOverflow=HorizontalWrapMode.Wrap;sub.verticalOverflow=VerticalWrapMode.Overflow;
            string id=p.customerId;Button invite=Button("INVITE BACK",row.transform,new Color(.10f,.42f,.64f,1f),()=>{CustomerRelationsDirector.Instance?.Invite(id);Refresh();});LayoutElement il=invite.gameObject.AddComponent<LayoutElement>();il.preferredWidth=150;
            if(p.complaints>0){Button resolve=Button("RESOLVE",row.transform,new Color(.56f,.30f,.10f,1f),()=>{CustomerRelationsDirector.Instance?.ResolveComplaint(id);Refresh();});LayoutElement cl=resolve.gameObject.AddComponent<LayoutElement>();cl.preferredWidth=130;}
        }

        private void RenderReviews(CustomerRelationsService s)
        {
            Card("PUBLIC REPUTATION","Review score "+s.State.reviewScore+" / 100\nPositive reviews "+s.State.fiveStarReviews+" · negative reviews "+s.State.oneStarReviews+" · referrals "+s.State.referralCount+"\nHigh review score periodically generates organic in-game reputation.",s.State.reviewScore>=80?new Color(.20f,.82f,.48f,1f):Color.white);
            Section("RECENT SERVICE EVENTS");
            foreach(string line in s.State.history.AsEnumerable().Reverse().Take(40)) Card(line,"",Color.white);
        }

        private void Heading(string a,string b){Text t=Block(a,32,Color.white,55);t.fontStyle=FontStyle.Bold;Block(b,19,new Color(.56f,.65f,.73f,1f),38);}private void Section(string a){Text t=Block(a,22,new Color(.10f,.64f,.92f,1f),42);t.fontStyle=FontStyle.Bold;}
        private void Card(string title,string sub,Color c){GameObject g=UI("Card",content);g.AddComponent<Image>().color=new Color(.095f,.115f,.145f,1f);VerticalLayoutGroup v=g.AddComponent<VerticalLayoutGroup>();v.padding=new RectOffset(18,18,11,12);v.spacing=4;v.childControlHeight=true;v.childForceExpandHeight=false;LayoutElement le=g.AddComponent<LayoutElement>();le.minHeight=68;Text a=Text(title,g.transform,21,TextAnchor.MiddleLeft,c);a.fontStyle=FontStyle.Bold;if(!string.IsNullOrEmpty(sub)){Text b=Text(sub,g.transform,17,TextAnchor.UpperLeft,new Color(.66f,.73f,.80f,1f));b.horizontalOverflow=HorizontalWrapMode.Wrap;b.verticalOverflow=VerticalWrapMode.Overflow;}}
        private Text Block(string value,int size,Color c,float height){Text t=Text(value,content,size,TextAnchor.MiddleLeft,c);LayoutElement l=t.gameObject.AddComponent<LayoutElement>();l.preferredHeight=height;return t;}private Button Button(string label,Transform p,Color c,Action a){GameObject g=UI("Button_"+label,p);Image i=g.AddComponent<Image>();i.color=c;Button b=g.AddComponent<Button>();b.targetGraphic=i;if(a!=null)b.onClick.AddListener(()=>a());Text t=Text(label,g.transform,16,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,5);t.raycastTarget=false;return b;}private Text Text(string value,Transform p,int size,TextAnchor align,Color c){GameObject g=UI("Text",p);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=c;t.raycastTarget=false;return t;}private GameObject UI(string n,Transform p){GameObject g=new GameObject(n,typeof(RectTransform));g.transform.SetParent(p,false);return g;}private static void Stretch(RectTransform r,float i=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(i,i);r.offsetMax=new Vector2(-i,-i);}private static void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(e);}
    }
}
