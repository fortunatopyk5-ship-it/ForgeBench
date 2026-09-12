using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    public sealed class WarrantyPanel : MonoBehaviour
    {
        private static WarrantyPanel instance;
        private GameObject root;
        private RectTransform content;
        private Text summary;
        private Font font;

        public static void Open()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ForgeBench_WarrantyPanel");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<WarrantyPanel>();
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
            root = new GameObject("WarrantyRoot", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            Canvas canvas = root.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 25420;
            CanvasScaler scaler = root.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920,1080); scaler.matchWidthOrHeight = .5f;
            root.AddComponent<GraphicRaycaster>();

            GameObject bg = UI("BG", root.transform); Stretch(bg.GetComponent<RectTransform>()); bg.AddComponent<Image>().color = new Color(.012f,.018f,.026f,.995f);
            GameObject panel = UI("Panel", bg.transform); RectTransform pr = panel.GetComponent<RectTransform>(); pr.anchorMin = new Vector2(.08f,.06f); pr.anchorMax = new Vector2(.92f,.94f); pr.offsetMin = pr.offsetMax = Vector2.zero; panel.AddComponent<Image>().color = new Color(.052f,.064f,.080f,1f);

            Text title = Text("WARRANTY / COMEBACK DESK", panel.transform, 32, TextAnchor.MiddleLeft, Color.white); RectTransform tr = title.rectTransform; tr.anchorMin = new Vector2(.035f,.90f); tr.anchorMax = new Vector2(.63f,.985f); tr.offsetMin = tr.offsetMax = Vector2.zero;
            summary = Text("", panel.transform, 17, TextAnchor.MiddleRight, new Color(.68f,.76f,.83f,1f)); RectTransform sm = summary.rectTransform; sm.anchorMin = new Vector2(.55f,.90f); sm.anchorMax = new Vector2(.86f,.985f); sm.offsetMin = sm.offsetMax = Vector2.zero;
            Button close = Button("✕ CLOSE", panel.transform, new Color(.54f,.11f,.13f,1f), Close); RectTransform xr = close.GetComponent<RectTransform>(); xr.anchorMin = new Vector2(.875f,.915f); xr.anchorMax = new Vector2(.965f,.975f); xr.offsetMin = xr.offsetMax = Vector2.zero;

            GameObject scroll = UI("Scroll", panel.transform); RectTransform sr = scroll.GetComponent<RectTransform>(); sr.anchorMin = new Vector2(.03f,.04f); sr.anchorMax = new Vector2(.97f,.89f); sr.offsetMin = sr.offsetMax = Vector2.zero;
            ScrollRect sc = scroll.AddComponent<ScrollRect>(); sc.horizontal = false; sc.scrollSensitivity = 45f;
            GameObject viewport = UI("Viewport", scroll.transform); Stretch(viewport.GetComponent<RectTransform>()); Image vi = viewport.AddComponent<Image>(); vi.color = new Color(.025f,.033f,.043f,.70f); Mask mask = viewport.AddComponent<Mask>(); mask.showMaskGraphic = false; sc.viewport = viewport.GetComponent<RectTransform>();
            GameObject body = UI("Content", viewport.transform); content = body.GetComponent<RectTransform>(); content.anchorMin = new Vector2(0,1); content.anchorMax = new Vector2(1,1); content.pivot = new Vector2(.5f,1); content.sizeDelta = new Vector2(0,600);
            VerticalLayoutGroup vg = body.AddComponent<VerticalLayoutGroup>(); vg.padding = new RectOffset(18,18,16,20); vg.spacing = 10; vg.childControlHeight = true; vg.childForceExpandHeight = false; vg.childControlWidth = true; vg.childForceExpandWidth = true;
            ContentSizeFitter fit = body.AddComponent<ContentSizeFitter>(); fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize; sc.content = content;
        }

        private void Refresh()
        {
            if (content == null) return;
            for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject);
            WarrantyService service = WarrantyDirector.Instance?.Service;
            if (service == null)
            {
                Card("INITIALIZING", "Warranty database is starting. Reopen this terminal in a moment.", new Color(.65f,.70f,.76f,1f), null, null);
                return;
            }

            summary.text = "OPEN " + service.OpenCount + "   ·   EXPOSURE $" + service.Exposure.ToString("0") + "   ·   RESOLVED " + service.ResolvedCount;
            Text intro = Text("A comeback is deterministic after the original service quality snapshot is saved. Reloading cannot reroll a claim. Warranty work pays $0 and the workshop absorbs modeled liability after successful resolution.", content, 16, TextAnchor.UpperLeft, new Color(.67f,.74f,.81f,1f)); LayoutElement il = intro.gameObject.AddComponent<LayoutElement>(); il.preferredHeight = 58;

            var cases = service.Cases.OrderBy(c => c.status == WarrantyCaseStatus.ClaimAvailable ? 0 : c.status == WarrantyCaseStatus.InService ? 1 : c.status == WarrantyCaseStatus.CallbackOffered ? 2 : c.status == WarrantyCaseStatus.Monitoring ? 3 : 4).ThenByDescending(c => c.openedDay).ToList();
            if (cases.Count == 0)
            {
                Card("NO WARRANTY CASES", "Completed jobs are being monitored. Strong assembly, thermals, stability and component condition reduce comeback risk.", new Color(.12f,.48f,.32f,1f), null, null);
                return;
            }

            foreach (WarrantyCaseState c in cases)
            {
                Color accent = StatusColor(c.status);
                string detail = c.caseId + " · source " + c.sourceJobId + " · " + c.customerName + "\n" +
                    c.status + " · modeled risk " + Mathf.RoundToInt(c.risk * 100f) + "% · liability $" + c.estimatedLiability.ToString("0") + "\n" +
                    c.reason + " · opened day " + c.openedDay + " · claim day " + c.claimDay + (string.IsNullOrEmpty(c.callbackJobId) ? "" : " · callback " + c.callbackJobId);
                Action action = null; string actionLabel = null;
                if (c.status == WarrantyCaseStatus.ClaimAvailable)
                {
                    string id = c.caseId;
                    actionLabel = "CREATE NO-CHARGE CALLBACK";
                    action = () => { WarrantyDirector.Instance?.Schedule(id); Refresh(); };
                }
                Card(c.status == WarrantyCaseStatus.Resolved ? "RESOLVED · " + c.reason : c.reason, detail, accent, actionLabel, action);
            }
        }

        private void Card(string title, string detail, Color accent, string actionLabel, Action action)
        {
            GameObject g = UI("WarrantyCard", content); g.AddComponent<Image>().color = new Color(.083f,.101f,.127f,1f); LayoutElement le = g.AddComponent<LayoutElement>(); le.preferredHeight = action == null ? 116 : 146;
            GameObject bar = UI("Accent", g.transform); RectTransform br = bar.GetComponent<RectTransform>(); br.anchorMin = new Vector2(0,0); br.anchorMax = new Vector2(.008f,1); br.offsetMin = br.offsetMax = Vector2.zero; bar.AddComponent<Image>().color = accent;
            Text a = Text(title, g.transform, 21, TextAnchor.UpperLeft, Color.white); a.fontStyle = FontStyle.Bold; RectTransform ar = a.rectTransform; ar.anchorMin = new Vector2(.025f,.58f); ar.anchorMax = new Vector2(.73f,.92f); ar.offsetMin = ar.offsetMax = Vector2.zero;
            Text b = Text(detail, g.transform, 15, TextAnchor.UpperLeft, new Color(.64f,.71f,.78f,1f)); RectTransform dr = b.rectTransform; dr.anchorMin = new Vector2(.025f, action == null ? .08f : .27f); dr.anchorMax = new Vector2(.97f,.62f); dr.offsetMin = dr.offsetMax = Vector2.zero;
            if (action != null)
            {
                Button btn = Button(actionLabel, g.transform, new Color(.10f,.46f,.68f,1f), action); RectTransform rr = btn.GetComponent<RectTransform>(); rr.anchorMin = new Vector2(.71f,.04f); rr.anchorMax = new Vector2(.97f,.24f); rr.offsetMin = rr.offsetMax = Vector2.zero;
            }
        }

        private static Color StatusColor(WarrantyCaseStatus s)
        {
            switch (s)
            {
                case WarrantyCaseStatus.ClaimAvailable: return new Color(.94f,.48f,.10f,1f);
                case WarrantyCaseStatus.CallbackOffered: return new Color(.10f,.52f,.82f,1f);
                case WarrantyCaseStatus.InService: return new Color(.72f,.62f,.08f,1f);
                case WarrantyCaseStatus.Resolved: return new Color(.10f,.62f,.34f,1f);
                case WarrantyCaseStatus.Failed: return new Color(.74f,.10f,.12f,1f);
                default: return new Color(.50f,.58f,.66f,1f);
            }
        }

        private Button Button(string label, Transform parent, Color color, Action action)
        {
            GameObject g = UI("Button_" + label, parent); Image im = g.AddComponent<Image>(); im.color = color; Button b = g.AddComponent<Button>(); b.targetGraphic = im; if (action != null) b.onClick.AddListener(() => action()); Text t = Text(label, g.transform, 15, TextAnchor.MiddleCenter, Color.white); Stretch(t.rectTransform, 4); t.raycastTarget = false; return b;
        }
        private Text Text(string value, Transform parent, int size, TextAnchor align, Color color) { GameObject g = UI("Text", parent); Text t = g.AddComponent<Text>(); t.font = font; t.text = value; t.fontSize = size; t.alignment = align; t.color = color; t.raycastTarget = false; return t; }
        private static GameObject UI(string name, Transform parent) { GameObject g = new GameObject(name, typeof(RectTransform)); g.transform.SetParent(parent, false); return g; }
        private static void Stretch(RectTransform r, float inset = 0) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = new Vector2(inset,inset); r.offsetMax = new Vector2(-inset,-inset); }
        private static void EnsureEventSystem() { if (FindAnyObjectByType<EventSystem>() != null) return; GameObject e = new GameObject("EventSystem"); e.AddComponent<EventSystem>(); e.AddComponent<StandaloneInputModule>(); DontDestroyOnLoad(e); }
    }
}
