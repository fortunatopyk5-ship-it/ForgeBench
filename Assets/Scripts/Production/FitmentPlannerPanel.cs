using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>Bench-side non-destructive component fitment planner.</summary>
    public sealed class FitmentPlannerPanel : MonoBehaviour
    {
        private static FitmentPlannerPanel instance;
        private GameRuntime game; private GameObject root; private RectTransform content; private Font font; private int page; private const int PageSize = 8;
        public static void Open() { if (instance == null) { GameObject g = new GameObject("ForgeBench_FitmentPlannerPanel"); DontDestroyOnLoad(g); instance = g.AddComponent<FitmentPlannerPanel>(); } instance.Show(); }
        public static void RefreshIfOpen() { if (instance != null && instance.root != null && instance.root.activeSelf) instance.Refresh(); }
        private void Awake() { if (instance != null && instance != this) { Destroy(gameObject); return; } instance = this; DontDestroyOnLoad(gameObject); font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); EnsureEventSystem(); Build(); root.SetActive(false); }
        private void Update() { if (root != null && root.activeSelf && Input.GetKeyDown(KeyCode.Escape) && !Application.isMobilePlatform) Close(); }
        private void Show() { game = GameRuntime.Instance; page = 0; root.SetActive(true); Cursor.lockState = CursorLockMode.None; MobileInputState.Move = Vector2.zero; Refresh(); }
        private void Close() { if (root != null) root.SetActive(false); Cursor.lockState = CursorLockMode.Locked; }

        private void Build()
        {
            root = new GameObject("FitmentPlannerRoot", typeof(RectTransform)); root.transform.SetParent(transform, false); Canvas c = root.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay; c.sortingOrder = 26320; CanvasScaler sc = root.AddComponent<CanvasScaler>(); sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; sc.referenceResolution = new Vector2(1920, 1080); sc.matchWidthOrHeight = .5f; root.AddComponent<GraphicRaycaster>();
            GameObject bg = UI("BG", root.transform); Stretch(bg.GetComponent<RectTransform>()); bg.AddComponent<Image>().color = new Color(.012f, .019f, .027f, .995f); GameObject card = UI("Card", bg.transform); RectTransform cr = card.GetComponent<RectTransform>(); cr.anchorMin = new Vector2(.05f, .04f); cr.anchorMax = new Vector2(.95f, .96f); cr.offsetMin = cr.offsetMax = Vector2.zero; card.AddComponent<Image>().color = new Color(.052f, .064f, .08f, 1f);
            Text title = Text("COMPONENT FITMENT PLANNER", card.transform, 30, TextAnchor.MiddleLeft, Color.white); RectTransform tr = title.rectTransform; tr.anchorMin = new Vector2(.025f, .91f); tr.anchorMax = new Vector2(.72f, .985f); tr.offsetMin = tr.offsetMax = Vector2.zero; Button close = Button("✕ CLOSE", card.transform, new Color(.52f, .12f, .13f, 1f), Close); RectTransform xr = close.GetComponent<RectTransform>(); xr.anchorMin = new Vector2(.83f, .92f); xr.anchorMax = new Vector2(.972f, .975f); xr.offsetMin = xr.offsetMax = Vector2.zero;
            GameObject scroll = UI("Scroll", card.transform); RectTransform sr = scroll.GetComponent<RectTransform>(); sr.anchorMin = new Vector2(.025f, .025f); sr.anchorMax = new Vector2(.975f, .90f); sr.offsetMin = sr.offsetMax = Vector2.zero; ScrollRect s = scroll.AddComponent<ScrollRect>(); s.horizontal = false; s.scrollSensitivity = 40f; GameObject viewport = UI("Viewport", scroll.transform); Stretch(viewport.GetComponent<RectTransform>()); viewport.AddComponent<Image>().color = new Color(.023f, .031f, .041f, .72f); Mask mask = viewport.AddComponent<Mask>(); mask.showMaskGraphic = false; s.viewport = viewport.GetComponent<RectTransform>(); GameObject body = UI("Content", viewport.transform); content = body.GetComponent<RectTransform>(); content.anchorMin = new Vector2(0, 1); content.anchorMax = new Vector2(1, 1); content.pivot = new Vector2(.5f, 1); content.sizeDelta = new Vector2(0, 600); VerticalLayoutGroup v = body.AddComponent<VerticalLayoutGroup>(); v.padding = new RectOffset(18, 18, 15, 22); v.spacing = 9; v.childControlHeight = true; v.childForceExpandHeight = false; v.childControlWidth = true; v.childForceExpandWidth = true; ContentSizeFitter fit = body.AddComponent<ContentSizeFitter>(); fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize; s.content = content;
        }

        private FitmentPlanningService Service() => new FitmentPlanningService(game.Inventory, game.Compatibility, game.PowerThermal);
        private void Refresh()
        {
            game = GameRuntime.Instance; if (game == null || content == null) return; Clear(); MachineState m = game.ActiveMachine; if (m == null) { Card("NO DEVICE", "Accept a customer job before planning component fitment.", FitmentSeverity.Blocking); return; }
            FitmentReport current = Service().InspectCurrent(m); Heading((game.ActiveJob == null ? m.displayName : game.ActiveJob.jobId + " · " + game.ActiveJob.title), "Current system: " + current.Summary + " · " + current.warnings + " warning(s)");
            foreach (FitmentFinding f in current.findings.Where(x => x.severity >= FitmentSeverity.Advisory).Take(6)) Card(f.area.ToUpperInvariant(), f.message, f.severity);
            Section("AVAILABLE INVENTORY CANDIDATES");
            List<ItemInstance> candidates = game.State.inventory.Where(i => !i.reserved && !i.customerOwned && game.Inventory.Def(i) != null && game.Inventory.Def(i).category != PartCategory.Tool && game.Inventory.Def(i).category != PartCategory.Consumable).OrderBy(i => game.Inventory.Def(i).category).ThenByDescending(i => game.Inventory.Def(i).quality).ThenBy(i => game.Inventory.Def(i).model).ToList();
            int maxPage = Mathf.Max(0, (candidates.Count - 1) / PageSize); page = Mathf.Clamp(page, 0, maxPage); int start = page * PageSize;
            for (int n = start; n < Mathf.Min(candidates.Count, start + PageSize); n++) Candidate(candidates[n]);
            if (candidates.Count == 0) Card("NO AVAILABLE HARDWARE", "Order and receive components before using the fitment planner.", FitmentSeverity.Advisory);
            Row(new[] { A("◀ PREVIOUS", () => { page = Mathf.Max(0, page - 1); Refresh(); }), A("PAGE " + (page + 1) + " / " + (maxPage + 1), null), A("NEXT ▶", () => { page = Mathf.Min(maxPage, page + 1); Refresh(); }) });
        }

        private void Candidate(ItemInstance item)
        {
            HardwareDefinition d = game.Inventory.Def(item); FitmentReport r = Service().EvaluateCandidate(game.ActiveMachine, item); FitmentSeverity sev = r.blockers > 0 ? FitmentSeverity.Blocking : r.warnings > 0 ? FitmentSeverity.Warning : FitmentSeverity.Info;
            GameObject g = UI("Candidate", content); g.AddComponent<Image>().color = new Color(.085f, .102f, .13f, 1f); HorizontalLayoutGroup h = g.AddComponent<HorizontalLayoutGroup>(); h.padding = new RectOffset(15, 11, 9, 9); h.spacing = 10; h.childControlHeight = true; h.childForceExpandHeight = true; g.AddComponent<LayoutElement>().minHeight = 86;
            GameObject info = UI("Info", g.transform); VerticalLayoutGroup v = info.AddComponent<VerticalLayoutGroup>(); v.childForceExpandHeight = false; v.spacing = 2; info.AddComponent<LayoutElement>().flexibleWidth = 1; Text title = Text(d.category + " · " + d.brand + " " + d.model + " · " + r.Summary, info.transform, 19, TextAnchor.MiddleLeft, SevColor(sev)); title.fontStyle = FontStyle.Bold;
            string detail = "$" + d.price.ToString("0") + " · condition " + Mathf.RoundToInt(item.condition * 100f) + "%"; FitmentFinding first = r.findings.FirstOrDefault(x => x.severity >= FitmentSeverity.Advisory); if (first != null) detail += "\n" + first.area + ": " + first.message; else detail += "\nNo fitment conflict detected."; Text t = Text(detail, info.transform, 15, TextAnchor.UpperLeft, new Color(.65f, .72f, .79f, 1f)); t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow;
            Button b = Button(r.pass ? "INSTALL" : "BLOCKED", g.transform, r.pass ? new Color(.10f, .48f, .32f, 1f) : new Color(.42f, .16f, .16f, 1f), r.pass ? (Action)(() => { game.Install(item.instanceId); Refresh(); }) : null); b.interactable = r.pass; b.gameObject.AddComponent<LayoutElement>().preferredWidth = 150;
        }

        private struct Act { public string t; public Action a; public Act(string x, Action y) { t = x; a = y; } }
        private static Act A(string t, Action a) => new Act(t, a);
        private void Heading(string a, string b) { Text t = Text(a, content, 25, TextAnchor.MiddleLeft, Color.white); t.fontStyle = FontStyle.Bold; t.gameObject.AddComponent<LayoutElement>().preferredHeight = 38; Text s = Text(b, content, 16, TextAnchor.UpperLeft, new Color(.63f, .70f, .78f, 1f)); s.gameObject.AddComponent<LayoutElement>().preferredHeight = 42; }
        private void Section(string a) { Text t = Text(a, content, 21, TextAnchor.MiddleLeft, new Color(.12f, .68f, .94f, 1f)); t.fontStyle = FontStyle.Bold; t.gameObject.AddComponent<LayoutElement>().preferredHeight = 38; }
        private void Card(string a, string b, FitmentSeverity sev) { GameObject g = UI("Card", content); g.AddComponent<Image>().color = new Color(.095f, .115f, .145f, 1f); VerticalLayoutGroup v = g.AddComponent<VerticalLayoutGroup>(); v.padding = new RectOffset(15, 15, 9, 9); v.childForceExpandHeight = false; g.AddComponent<LayoutElement>().minHeight = 65; Text x = Text(a, g.transform, 19, TextAnchor.MiddleLeft, SevColor(sev)); x.fontStyle = FontStyle.Bold; Text y = Text(b, g.transform, 15, TextAnchor.UpperLeft, new Color(.68f, .75f, .82f, 1f)); y.horizontalOverflow = HorizontalWrapMode.Wrap; y.verticalOverflow = VerticalWrapMode.Overflow; }
        private void Row(Act[] aa) { GameObject g = UI("Pager", content); HorizontalLayoutGroup h = g.AddComponent<HorizontalLayoutGroup>(); h.spacing = 10; h.childControlHeight = true; h.childForceExpandHeight = true; h.childForceExpandWidth = true; g.AddComponent<LayoutElement>().preferredHeight = 56; foreach (Act a in aa) { Button b = Button(a.t, g.transform, new Color(.13f, .31f, .46f, 1f), a.a); b.interactable = a.a != null; b.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1; } }
        private static Color SevColor(FitmentSeverity s) { if (s == FitmentSeverity.Blocking) return new Color(1f, .34f, .30f, 1f); if (s == FitmentSeverity.Warning) return new Color(1f, .69f, .22f, 1f); if (s == FitmentSeverity.Advisory) return new Color(.48f, .76f, 1f, 1f); return new Color(.31f, .91f, .57f, 1f); }
        private Button Button(string label, Transform p, Color c, Action a) { GameObject g = UI("Button_" + label, p); Image i = g.AddComponent<Image>(); i.color = c; Button b = g.AddComponent<Button>(); b.targetGraphic = i; if (a != null) b.onClick.AddListener(() => a()); Text t = Text(label, g.transform, 16, TextAnchor.MiddleCenter, Color.white); Stretch(t.rectTransform, 5); t.raycastTarget = false; return b; }
        private Text Text(string value, Transform p, int size, TextAnchor align, Color c) { GameObject g = UI("Text", p); Text t = g.AddComponent<Text>(); t.font = font; t.text = value; t.fontSize = size; t.alignment = align; t.color = c; t.raycastTarget = false; return t; }
        private GameObject UI(string n, Transform p) { GameObject g = new GameObject(n, typeof(RectTransform)); g.transform.SetParent(p, false); return g; }
        private void Clear() { for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject); }
        private static void Stretch(RectTransform r, float i = 0) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = new Vector2(i, i); r.offsetMax = new Vector2(-i, -i); }
        private static void EnsureEventSystem() { if (FindAnyObjectByType<EventSystem>() != null) return; GameObject e = new GameObject("EventSystem"); e.AddComponent<EventSystem>(); e.AddComponent<StandaloneInputModule>(); DontDestroyOnLoad(e); }
    }
}
