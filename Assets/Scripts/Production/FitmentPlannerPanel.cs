using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    public sealed class FitmentPlannerPanel : MonoBehaviour
    {
        private static FitmentPlannerPanel instance;
        private GameRuntime game;
        private GameObject root;
        private RectTransform content;
        private Font font;
        private int page;
        private string comparisonId;
        private const int PageSize = 8;

        public static void Open()
        {
            if (instance == null)
            {
                GameObject g = new GameObject("ForgeBench_FitmentPlannerPanel");
                DontDestroyOnLoad(g);
                instance = g.AddComponent<FitmentPlannerPanel>();
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
            game = GameRuntime.Instance;
            page = 0;
            comparisonId = string.Empty;
            root.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            MobileInputState.Move = Vector2.zero;
            Refresh();
        }

        private void Close()
        {
            root.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Build()
        {
            root = new GameObject("FitmentPlannerRoot", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            Canvas c = root.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay; c.sortingOrder = 26320;
            CanvasScaler sc = root.AddComponent<CanvasScaler>(); sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; sc.referenceResolution = new Vector2(1920, 1080); sc.matchWidthOrHeight = .5f;
            root.AddComponent<GraphicRaycaster>();

            GameObject bg = UI("BG", root.transform); Stretch(bg.GetComponent<RectTransform>()); bg.AddComponent<Image>().color = new Color(.012f, .019f, .027f, .995f);
            GameObject card = UI("Card", bg.transform); RectTransform cr = card.GetComponent<RectTransform>(); cr.anchorMin = new Vector2(.05f, .04f); cr.anchorMax = new Vector2(.95f, .96f); cr.offsetMin = cr.offsetMax = Vector2.zero; card.AddComponent<Image>().color = new Color(.052f, .064f, .08f, 1f);
            Text title = Text("COMPONENT FITMENT / A-B ENGINEERING", card.transform, 30, TextAnchor.MiddleLeft, Color.white); Set(title.rectTransform, .025f, .91f, .72f, .985f);
            Button close = Button("✕ CLOSE", card.transform, new Color(.52f, .12f, .13f, 1f), Close); Set(close.GetComponent<RectTransform>(), .83f, .92f, .972f, .975f);

            GameObject scroll = UI("Scroll", card.transform); Set(scroll.GetComponent<RectTransform>(), .025f, .025f, .975f, .90f);
            ScrollRect s = scroll.AddComponent<ScrollRect>(); s.horizontal = false; s.scrollSensitivity = 40f;
            GameObject viewport = UI("Viewport", scroll.transform); Stretch(viewport.GetComponent<RectTransform>()); viewport.AddComponent<Image>().color = new Color(.023f, .031f, .041f, .72f); Mask mask = viewport.AddComponent<Mask>(); mask.showMaskGraphic = false; s.viewport = viewport.GetComponent<RectTransform>();
            GameObject body = UI("Content", viewport.transform); content = body.GetComponent<RectTransform>(); content.anchorMin = new Vector2(0, 1); content.anchorMax = new Vector2(1, 1); content.pivot = new Vector2(.5f, 1); content.sizeDelta = new Vector2(0, 600);
            VerticalLayoutGroup v = body.AddComponent<VerticalLayoutGroup>(); v.padding = new RectOffset(18, 18, 15, 22); v.spacing = 9; v.childControlHeight = true; v.childForceExpandHeight = false; v.childControlWidth = true; v.childForceExpandWidth = true;
            ContentSizeFitter fit = body.AddComponent<ContentSizeFitter>(); fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize; s.content = content;
        }

        private FitmentPlanningService Service()
        {
            return new FitmentPlanningService(game.Inventory, game.Compatibility, game.PowerThermal);
        }

        private HardwareComparisonService ComparisonService()
        {
            return new HardwareComparisonService(game.Inventory, Service());
        }

        private void Refresh()
        {
            game = GameRuntime.Instance;
            if (game == null || content == null) return;
            Clear();
            MachineState m = game.ActiveMachine;
            if (m == null)
            {
                Card("NO DEVICE", "Accept a customer job before planning component fitment.", FitmentSeverity.Blocking);
                return;
            }

            FitmentReport current = Service().InspectCurrent(m);
            Heading(game.ActiveJob == null ? m.displayName : game.ActiveJob.jobId + " · " + game.ActiveJob.title,
                "Current system: " + current.Summary + " · " + current.warnings + " warning(s) · comparisons are non-destructive until INSTALL is pressed.");
            foreach (FitmentFinding f in current.findings.Where(x => (int)x.severity >= (int)FitmentSeverity.Advisory).Take(6))
                Card(f.area.ToUpperInvariant(), f.message, f.severity);

            if (!string.IsNullOrEmpty(comparisonId)) RenderComparison(m);

            Section("AVAILABLE INVENTORY CANDIDATES");
            List<ItemInstance> candidates = game.State.inventory
                .Where(i => !i.reserved && !i.customerOwned && game.Inventory.Def(i) != null && game.Inventory.Def(i).category != PartCategory.Tool && game.Inventory.Def(i).category != PartCategory.Consumable)
                .OrderBy(i => game.Inventory.Def(i).category)
                .ThenByDescending(i => game.Inventory.Def(i).quality)
                .ThenBy(i => game.Inventory.Def(i).model)
                .ToList();
            int maxPage = Mathf.Max(0, (candidates.Count - 1) / PageSize);
            page = Mathf.Clamp(page, 0, maxPage);
            int start = page * PageSize;
            for (int n = start; n < Mathf.Min(candidates.Count, start + PageSize); n++) Candidate(candidates[n]);
            if (candidates.Count == 0) Card("NO AVAILABLE HARDWARE", "Order and receive components before using the fitment planner.", FitmentSeverity.Advisory);
            Pager(maxPage);
        }

        private void RenderComparison(MachineState machine)
        {
            ItemInstance item = game.Inventory.Get(comparisonId);
            if (item == null || item.reserved)
            {
                comparisonId = string.Empty;
                return;
            }

            HardwareComparisonReport r = ComparisonService().Compare(machine, item);
            Section("A/B ENGINEERING COMPARISON");
            GameObject box = UI("Comparison", content);
            box.AddComponent<Image>().color = new Color(.047f, .078f, .096f, 1f);
            VerticalLayoutGroup vg = box.AddComponent<VerticalLayoutGroup>(); vg.padding = new RectOffset(16, 16, 13, 14); vg.spacing = 6; vg.childControlHeight = true; vg.childForceExpandHeight = false;
            box.AddComponent<LayoutElement>().minHeight = 170;

            GameObject header = UI("ComparisonHeader", box.transform);
            HorizontalLayoutGroup hg = header.AddComponent<HorizontalLayoutGroup>(); hg.spacing = 10; hg.childControlHeight = true; hg.childForceExpandHeight = true; hg.childForceExpandWidth = false;
            header.AddComponent<LayoutElement>().preferredHeight = 48;
            Text title = Text(r.baselineName + "  →  " + r.candidateName, header.transform, 21, TextAnchor.MiddleLeft, VerdictColor(r.verdict));
            title.fontStyle = FontStyle.Bold; title.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            Button clear = Button("CLEAR", header.transform, new Color(.19f, .25f, .30f, 1f), () => { comparisonId = string.Empty; Refresh(); }); clear.gameObject.AddComponent<LayoutElement>().preferredWidth = 110;

            Text summary = Text(r.Summary + " · fitment " + (r.fitment?.score ?? 0f).ToString("0") + "/100 · suitability " + r.suitabilityScore.ToString("0") + "/100",
                box.transform, 16, TextAnchor.MiddleLeft, new Color(.69f, .79f, .86f, 1f)); summary.gameObject.AddComponent<LayoutElement>().preferredHeight = 34;

            foreach (HardwareComparisonMetric metric in r.metrics.Take(11))
            {
                GameObject row = UI("Metric_" + metric.label, box.transform);
                HorizontalLayoutGroup mh = row.AddComponent<HorizontalLayoutGroup>(); mh.spacing = 8; mh.childControlHeight = true; mh.childForceExpandHeight = true; mh.childForceExpandWidth = false;
                row.AddComponent<LayoutElement>().preferredHeight = 30;
                Text label = Text(metric.label, row.transform, 15, TextAnchor.MiddleLeft, new Color(.62f, .72f, .78f, 1f)); label.gameObject.AddComponent<LayoutElement>().preferredWidth = 210;
                Text values = Text(metric.baselineText + "  →  " + metric.candidateText, row.transform, 15, TextAnchor.MiddleLeft, Color.white); values.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
                string d = Mathf.Abs(metric.changePercent) < .05f ? "≈" : (metric.changePercent > 0f ? "+" : string.Empty) + metric.changePercent.ToString("0") + "%";
                bool favorable = metric.higherIsBetter ? metric.changePercent >= 0f : metric.changePercent <= 0f;
                Text delta = Text(d, row.transform, 15, TextAnchor.MiddleRight, favorable ? new Color(.29f, .91f, .55f, 1f) : new Color(1f, .47f, .32f, 1f)); delta.gameObject.AddComponent<LayoutElement>().preferredWidth = 92;
            }

            if (r.fitment != null)
            {
                foreach (FitmentFinding f in r.fitment.findings.Where(x => (int)x.severity >= (int)FitmentSeverity.Advisory).Take(4))
                {
                    Text note = Text(f.severity.ToString().ToUpperInvariant() + " · " + f.area + ": " + f.message, box.transform, 14, TextAnchor.UpperLeft, SevColor(f.severity));
                    note.horizontalOverflow = HorizontalWrapMode.Wrap; note.verticalOverflow = VerticalWrapMode.Overflow; note.gameObject.AddComponent<LayoutElement>().preferredHeight = 32;
                }
            }
        }

        private void Candidate(ItemInstance item)
        {
            HardwareDefinition d = game.Inventory.Def(item);
            FitmentReport fit = Service().EvaluateCandidate(game.ActiveMachine, item);
            HardwareComparisonReport cmp = ComparisonService().Compare(game.ActiveMachine, item);
            FitmentSeverity sev = fit.blockers > 0 ? FitmentSeverity.Blocking : fit.warnings > 0 ? FitmentSeverity.Warning : FitmentSeverity.Info;

            GameObject g = UI("Candidate", content); g.AddComponent<Image>().color = new Color(.085f, .102f, .13f, 1f);
            HorizontalLayoutGroup h = g.AddComponent<HorizontalLayoutGroup>(); h.padding = new RectOffset(15, 11, 9, 9); h.spacing = 10; h.childControlHeight = true; h.childForceExpandHeight = true; h.childForceExpandWidth = false;
            g.AddComponent<LayoutElement>().minHeight = 102;

            GameObject info = UI("Info", g.transform); VerticalLayoutGroup v = info.AddComponent<VerticalLayoutGroup>(); v.childForceExpandHeight = false; info.AddComponent<LayoutElement>().flexibleWidth = 1;
            Text title = Text(d.category + " · " + d.brand + " " + d.model + " · " + cmp.verdict.ToString().ToUpperInvariant(), info.transform, 19, TextAnchor.MiddleLeft, VerdictColor(cmp.verdict)); title.fontStyle = FontStyle.Bold;
            FitmentFinding first = fit.findings.FirstOrDefault(x => (int)x.severity >= (int)FitmentSeverity.Advisory);
            string detail = "$" + d.price.ToString("0") + " · condition " + Mathf.RoundToInt(item.condition * 100f) + "% · fitment " + fit.score.ToString("0") + "/100 · suitability " + cmp.suitabilityScore.ToString("0") + "/100\n" + (first == null ? "No fitment conflict detected." : first.area + ": " + first.message);
            Text t = Text(detail, info.transform, 15, TextAnchor.UpperLeft, new Color(.65f, .72f, .79f, 1f)); t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow;

            Button compare = Button(comparisonId == item.instanceId ? "VIEWING" : "COMPARE", g.transform, new Color(.13f, .34f, .52f, 1f), () => { comparisonId = item.instanceId; Refresh(); }); compare.gameObject.AddComponent<LayoutElement>().preferredWidth = 132;
            Action install = fit.pass ? (Action)(() => { game.Install(item.instanceId); if (comparisonId == item.instanceId) comparisonId = string.Empty; Refresh(); }) : null;
            Button installButton = Button(fit.pass ? "INSTALL" : "BLOCKED", g.transform, fit.pass ? new Color(.10f, .48f, .32f, 1f) : new Color(.42f, .16f, .16f, 1f), install); installButton.interactable = fit.pass; installButton.gameObject.AddComponent<LayoutElement>().preferredWidth = 132;
        }

        private void Pager(int maxPage)
        {
            GameObject g = UI("Pager", content); HorizontalLayoutGroup h = g.AddComponent<HorizontalLayoutGroup>(); h.spacing = 10; h.childControlHeight = true; h.childForceExpandHeight = true; h.childForceExpandWidth = true; g.AddComponent<LayoutElement>().preferredHeight = 56;
            Flex(g.transform, "◀ PREVIOUS", () => { page = Mathf.Max(0, page - 1); Refresh(); }, page > 0);
            Button mid = Button("PAGE " + (page + 1) + " / " + (maxPage + 1), g.transform, new Color(.10f, .18f, .25f, 1f), null); mid.interactable = false; mid.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            Flex(g.transform, "NEXT ▶", () => { page = Mathf.Min(maxPage, page + 1); Refresh(); }, page < maxPage);
        }

        private void Flex(Transform p, string label, Action a, bool enabled)
        {
            Button b = Button(label, p, new Color(.13f, .31f, .46f, 1f), a); b.interactable = enabled; b.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
        }

        private void Heading(string a, string b)
        {
            Text x = Text(a, content, 25, TextAnchor.MiddleLeft, Color.white); x.fontStyle = FontStyle.Bold; x.gameObject.AddComponent<LayoutElement>().preferredHeight = 38;
            Text y = Text(b, content, 16, TextAnchor.UpperLeft, new Color(.63f, .70f, .78f, 1f)); y.horizontalOverflow = HorizontalWrapMode.Wrap; y.verticalOverflow = VerticalWrapMode.Overflow; y.gameObject.AddComponent<LayoutElement>().preferredHeight = 46;
        }

        private void Section(string a)
        {
            Text t = Text(a, content, 21, TextAnchor.MiddleLeft, new Color(.12f, .68f, .94f, 1f)); t.fontStyle = FontStyle.Bold; t.gameObject.AddComponent<LayoutElement>().preferredHeight = 38;
        }

        private void Card(string a, string b, FitmentSeverity s)
        {
            GameObject g = UI("Card", content); g.AddComponent<Image>().color = new Color(.095f, .115f, .145f, 1f); VerticalLayoutGroup v = g.AddComponent<VerticalLayoutGroup>(); v.padding = new RectOffset(15, 15, 9, 9); v.childForceExpandHeight = false; g.AddComponent<LayoutElement>().minHeight = 65;
            Text x = Text(a, g.transform, 19, TextAnchor.MiddleLeft, SevColor(s)); x.fontStyle = FontStyle.Bold;
            Text y = Text(b, g.transform, 15, TextAnchor.UpperLeft, new Color(.68f, .75f, .82f, 1f)); y.horizontalOverflow = HorizontalWrapMode.Wrap; y.verticalOverflow = VerticalWrapMode.Overflow;
        }

        private static Color VerdictColor(HardwareComparisonVerdict v)
        {
            if (v == HardwareComparisonVerdict.Blocked || v == HardwareComparisonVerdict.Downgrade) return new Color(1f, .37f, .31f, 1f);
            if (v == HardwareComparisonVerdict.Sidegrade) return new Color(1f, .72f, .27f, 1f);
            if (v == HardwareComparisonVerdict.Expansion || v == HardwareComparisonVerdict.NewCapability) return new Color(.42f, .78f, 1f, 1f);
            return new Color(.31f, .91f, .57f, 1f);
        }

        private static Color SevColor(FitmentSeverity s)
        {
            if (s == FitmentSeverity.Blocking) return new Color(1f, .34f, .30f, 1f);
            if (s == FitmentSeverity.Warning) return new Color(1f, .69f, .22f, 1f);
            if (s == FitmentSeverity.Advisory) return new Color(.48f, .76f, 1f, 1f);
            return new Color(.31f, .91f, .57f, 1f);
        }

        private Button Button(string label, Transform p, Color c, Action a)
        {
            GameObject g = UI("Button_" + label, p); Image i = g.AddComponent<Image>(); i.color = c; Button b = g.AddComponent<Button>(); b.targetGraphic = i; if (a != null) b.onClick.AddListener(() => a()); Text t = Text(label, g.transform, 16, TextAnchor.MiddleCenter, Color.white); Stretch(t.rectTransform, 5); t.raycastTarget = false; return b;
        }

        private Text Text(string value, Transform p, int size, TextAnchor align, Color c)
        {
            GameObject g = UI("Text", p); Text t = g.AddComponent<Text>(); t.font = font; t.text = value; t.fontSize = size; t.alignment = align; t.color = c; t.raycastTarget = false; return t;
        }

        private GameObject UI(string n, Transform p)
        {
            GameObject g = new GameObject(n, typeof(RectTransform)); g.transform.SetParent(p, false); return g;
        }

        private void Clear()
        {
            for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject);
        }

        private static void Stretch(RectTransform r, float i = 0)
        {
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = new Vector2(i, i); r.offsetMax = new Vector2(-i, -i);
        }

        private static void Set(RectTransform r, float x0, float y0, float x1, float y1)
        {
            r.anchorMin = new Vector2(x0, y0); r.anchorMax = new Vector2(x1, y1); r.offsetMin = r.offsetMax = Vector2.zero;
        }

        private static void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null) return; GameObject e = new GameObject("EventSystem"); e.AddComponent<EventSystem>(); e.AddComponent<StandaloneInputModule>(); DontDestroyOnLoad(e);
        }
    }
}
