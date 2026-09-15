using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    public sealed class ServiceIntakePanel : MonoBehaviour
    {
        private static ServiceIntakePanel instance;
        private GameObject root;
        private RectTransform content;
        private Text summary;
        private Font font;

        public static void Open()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ForgeBench_ServiceIntakePanel");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<ServiceIntakePanel>();
            }
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
            if (root == null) Build();
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
            root = new GameObject("ServiceIntakeRoot", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            Canvas canvas = root.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 25440;
            CanvasScaler scaler = root.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920,1080); scaler.matchWidthOrHeight = .5f;
            root.AddComponent<GraphicRaycaster>();

            GameObject bg = UI("BG", root.transform); Stretch(bg.GetComponent<RectTransform>()); bg.AddComponent<Image>().color = new Color(.010f,.017f,.023f,.995f);
            GameObject panel = UI("Panel", bg.transform); RectTransform pr = panel.GetComponent<RectTransform>(); pr.anchorMin = new Vector2(.065f,.055f); pr.anchorMax = new Vector2(.935f,.945f); pr.offsetMin = pr.offsetMax = Vector2.zero; panel.AddComponent<Image>().color = new Color(.047f,.061f,.071f,1f);

            Text title = Text("SERVICE INTAKE / CHAIN OF CUSTODY", panel.transform, 31, TextAnchor.MiddleLeft, Color.white); RectTransform tr = title.rectTransform; tr.anchorMin = new Vector2(.03f,.905f); tr.anchorMax = new Vector2(.64f,.985f); tr.offsetMin = tr.offsetMax = Vector2.zero;
            summary = Text("", panel.transform, 16, TextAnchor.MiddleRight, new Color(.66f,.76f,.80f,1f)); RectTransform sm = summary.rectTransform; sm.anchorMin = new Vector2(.55f,.905f); sm.anchorMax = new Vector2(.86f,.985f); sm.offsetMin = sm.offsetMax = Vector2.zero;
            Button close = Button("✕ CLOSE", panel.transform, new Color(.46f,.12f,.13f,1f), Close); RectTransform xr = close.GetComponent<RectTransform>(); xr.anchorMin = new Vector2(.875f,.92f); xr.anchorMax = new Vector2(.965f,.975f); xr.offsetMin = xr.offsetMax = Vector2.zero;

            GameObject scroll = UI("Scroll", panel.transform); RectTransform sr = scroll.GetComponent<RectTransform>(); sr.anchorMin = new Vector2(.025f,.035f); sr.anchorMax = new Vector2(.975f,.89f); sr.offsetMin = sr.offsetMax = Vector2.zero;
            ScrollRect sc = scroll.AddComponent<ScrollRect>(); sc.horizontal = false; sc.scrollSensitivity = 45f;
            GameObject viewport = UI("Viewport", scroll.transform); Stretch(viewport.GetComponent<RectTransform>()); Image vi = viewport.AddComponent<Image>(); vi.color = new Color(.022f,.032f,.037f,.75f); Mask mask = viewport.AddComponent<Mask>(); mask.showMaskGraphic = false; sc.viewport = viewport.GetComponent<RectTransform>();
            GameObject body = UI("Content", viewport.transform); content = body.GetComponent<RectTransform>(); content.anchorMin = new Vector2(0,1); content.anchorMax = new Vector2(1,1); content.pivot = new Vector2(.5f,1); content.sizeDelta = new Vector2(0,900);
            VerticalLayoutGroup vg = body.AddComponent<VerticalLayoutGroup>(); vg.padding = new RectOffset(18,18,16,22); vg.spacing = 10; vg.childControlHeight = true; vg.childForceExpandHeight = false; vg.childControlWidth = true; vg.childForceExpandWidth = true;
            ContentSizeFitter fit = body.AddComponent<ContentSizeFitter>(); fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize; sc.content = content;
        }

        private void Refresh()
        {
            if (content == null) return;
            for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject);
            ServiceIntakeService service = ServiceIntakeDirector.Instance?.Service;
            if (service == null)
            {
                Card("INITIALIZING", "Front-desk custody database is starting.", new Color(.60f,.67f,.72f,1f), null, null);
                return;
            }

            ServiceIntakeRecord active = service.ActiveRecord;
            int open = service.Records.Count(x => x.status != IntakeStatus.Closed && x.status != IntakeStatus.Rejected);
            int closed = service.Records.Count(x => x.status == IntakeStatus.Closed);
            summary.text = "OPEN " + open + "   ·   CLOSED " + closed;

            if (active == null)
            {
                Card("NO ACTIVE DEVICE", "Accept a job to create an intake record. Completed records remain below as chain-of-custody history.", new Color(.12f,.46f,.34f,1f), null, null);
            }
            else
            {
                Text intro = Text("Front desk evidence is persistent outside save slots. Bench work is locked until inspection, power state, accessories, required data consent and estimate approval are recorded.", content, 16, TextAnchor.UpperLeft, new Color(.65f,.74f,.78f,1f));
                intro.gameObject.AddComponent<LayoutElement>().preferredHeight = 54;
                ActiveCard(active);
                ActionGrid(active);
            }

            foreach (ServiceIntakeRecord r in service.Records.OrderByDescending(x => x.openedDay).Take(12))
            {
                if (active != null && r.recordId == active.recordId) continue;
                string detail = r.recordId + " · " + r.jobId + " · " + r.customerName + " · " + r.deviceCategory + "\n" +
                    r.status + " · opened day " + r.openedDay + (r.closedDay > 0 ? " · closed day " + r.closedDay : "") + " · condition " + Mathf.RoundToInt(r.exteriorCondition * 100f) + "%";
                Card("HISTORY · " + r.status, detail, StatusColor(r.status), null, null);
            }
        }

        private void ActiveCard(ServiceIntakeRecord r)
        {
            string accessories = r.accessories.Count == 0 ? "none" : string.Join(", ", r.accessories.Select(a => a.label + (a.received ? " ✓" : " ?")));
            string detail = r.recordId + " · " + r.jobId + " · " + r.customerName + " · " + r.deviceCategory + " / " + r.jobType + "\n" +
                "STATUS " + r.status + " · condition " + Mathf.RoundToInt(r.exteriorCondition * 100f) + "% · visible damage " + r.visibleDamageCount + (r.moistureWarning ? " · ⚠ MOISTURE/CORROSION" : "") + "\n" +
                "Power: " + (r.powerStateRecorded ? r.powerState : "not recorded") + " · data consent " + (r.dataConsentRequired ? (r.dataConsent ? "YES" : "REQUIRED") : "N/A") + "\n" +
                "Accessories: " + accessories;
            Card("ACTIVE INTAKE · " + r.status, detail, StatusColor(r.status), null, null);
        }

        private void ActionGrid(ServiceIntakeRecord r)
        {
            GameObject g = UI("Actions", content); g.AddComponent<Image>().color = new Color(.067f,.083f,.092f,1f); LayoutElement le = g.AddComponent<LayoutElement>(); le.preferredHeight = 210;
            GridLayoutGroup grid = g.AddComponent<GridLayoutGroup>(); grid.padding = new RectOffset(14,14,14,14); grid.spacing = new Vector2(10,10); grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount; grid.constraintCount = 4; grid.cellSize = new Vector2(310,76);
            AddAction(g.transform,"1 · VISUAL / ID",r.exteriorInspected,()=>ServiceIntakeDirector.Instance?.Inspect());
            AddAction(g.transform,"2 · POWER STATE",r.powerStateRecorded,()=>ServiceIntakeDirector.Instance?.RecordPower());
            AddAction(g.transform,"3 · ACCESSORIES",r.accessoriesConfirmed,()=>ServiceIntakeDirector.Instance?.ConfirmAccessories());
            AddAction(g.transform,"4 · DATA CONSENT",!r.dataConsentRequired||r.dataConsent,()=>ServiceIntakeDirector.Instance?.GrantDataConsent());
            AddAction(g.transform,"5 · ESTIMATE APPROVAL",r.estimateApproved,()=>ServiceIntakeDirector.Instance?.ApproveEstimate());
            AddAction(g.transform,"6 · BENCH CHECK-IN",r.benchCheckedIn,()=>ServiceIntakeDirector.Instance?.CheckIn());
            AddAction(g.transform,"7 · RELEASE REVIEW",r.releaseAuthorized,()=>ServiceIntakeDirector.Instance?.AuthorizeRelease());
            Button close = Button("CLOSE TERMINAL",g.transform,new Color(.18f,.24f,.27f,1f),Close); close.GetComponentInChildren<Text>().fontSize = 14;
        }

        private void AddAction(Transform parent,string label,bool complete,Action action)
        {
            Color c = complete ? new Color(.10f,.44f,.28f,1f) : new Color(.08f,.40f,.55f,1f);
            Button b = Button((complete ? "✓ " : "") + label,parent,c,()=>{action?.Invoke();Refresh();});
            b.GetComponentInChildren<Text>().fontSize = 14;
        }

        private void Card(string title,string detail,Color accent,string actionLabel,Action action)
        {
            GameObject g=UI("IntakeCard",content);g.AddComponent<Image>().color=new Color(.077f,.096f,.105f,1f);LayoutElement le=g.AddComponent<LayoutElement>();le.preferredHeight=action==null?132:160;
            GameObject bar=UI("Accent",g.transform);RectTransform br=bar.GetComponent<RectTransform>();br.anchorMin=new Vector2(0,0);br.anchorMax=new Vector2(.008f,1);br.offsetMin=br.offsetMax=Vector2.zero;bar.AddComponent<Image>().color=accent;
            Text a=Text(title,g.transform,20,TextAnchor.UpperLeft,Color.white);a.fontStyle=FontStyle.Bold;RectTransform ar=a.rectTransform;ar.anchorMin=new Vector2(.025f,.66f);ar.anchorMax=new Vector2(.95f,.93f);ar.offsetMin=ar.offsetMax=Vector2.zero;
            Text b=Text(detail,g.transform,15,TextAnchor.UpperLeft,new Color(.64f,.72f,.75f,1f));RectTransform dr=b.rectTransform;dr.anchorMin=new Vector2(.025f,.08f);dr.anchorMax=new Vector2(.97f,.68f);dr.offsetMin=dr.offsetMax=Vector2.zero;
        }

        private static Color StatusColor(IntakeStatus s)
        {
            switch(s)
            {
                case IntakeStatus.OnBench:return new Color(.08f,.58f,.38f,1f);
                case IntakeStatus.ReleaseReview:return new Color(.10f,.55f,.72f,1f);
                case IntakeStatus.ReadyForBench:return new Color(.72f,.57f,.08f,1f);
                case IntakeStatus.Closed:return new Color(.12f,.48f,.32f,1f);
                case IntakeStatus.Rejected:return new Color(.70f,.12f,.12f,1f);
                case IntakeStatus.AwaitingApproval:return new Color(.82f,.42f,.08f,1f);
                default:return new Color(.50f,.58f,.61f,1f);
            }
        }

        private Button Button(string label,Transform parent,Color color,Action action){GameObject g=UI("Button_"+label,parent);Image im=g.AddComponent<Image>();im.color=color;Button b=g.AddComponent<Button>();b.targetGraphic=im;if(action!=null)b.onClick.AddListener(()=>action());Text t=Text(label,g.transform,15,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,4);t.raycastTarget=false;return b;}
        private Text Text(string value,Transform parent,int size,TextAnchor align,Color color){GameObject g=UI("Text",parent);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=color;t.raycastTarget=false;return t;}
        private static GameObject UI(string name,Transform parent){GameObject g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(parent,false);return g;}
        private static void Stretch(RectTransform r,float inset=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(inset,inset);r.offsetMax=new Vector2(-inset,-inset);}
        private static void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(e);}
    }
}