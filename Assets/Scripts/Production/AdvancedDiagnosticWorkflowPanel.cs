using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>Interactive evidence console for the advanced diagnostic workflow.</summary>
    public sealed class AdvancedDiagnosticWorkflowPanel : MonoBehaviour
    {
        private static AdvancedDiagnosticWorkflowPanel instance;
        private GameObject root; private RectTransform content; private Font font; private GameRuntime game;
        private readonly Dictionary<DiagnosticProbeKind, DiagnosticEvidence> latest = new Dictionary<DiagnosticProbeKind, DiagnosticEvidence>();

        public static void Open()
        {
            if (instance == null) { GameObject g = new GameObject("ForgeBench_AdvancedDiagnosticWorkflowPanel"); DontDestroyOnLoad(g); instance = g.AddComponent<AdvancedDiagnosticWorkflowPanel>(); }
            instance.Show();
        }
        public static void RefreshIfOpen() { if (instance != null && instance.root != null && instance.root.activeSelf) instance.Refresh(); }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this; DontDestroyOnLoad(gameObject); font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); EnsureEventSystem(); Build(); root.SetActive(false);
        }
        private void Update() { if (root != null && root.activeSelf && Input.GetKeyDown(KeyCode.Escape) && !Application.isMobilePlatform) Close(); }
        private void Show() { game = GameRuntime.Instance; if (root == null) Build(); root.SetActive(true); Cursor.lockState = CursorLockMode.None; MobileInputState.Move = Vector2.zero; Refresh(); }
        private void Close() { if (root != null) root.SetActive(false); Cursor.lockState = CursorLockMode.Locked; }

        private void Build()
        {
            root = new GameObject("AdvancedDiagnosticsRoot", typeof(RectTransform)); root.transform.SetParent(transform, false);
            Canvas c = root.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay; c.sortingOrder = 26350;
            CanvasScaler sc = root.AddComponent<CanvasScaler>(); sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; sc.referenceResolution = new Vector2(1920, 1080); sc.matchWidthOrHeight = .5f; root.AddComponent<GraphicRaycaster>();
            GameObject bg = UI("BG", root.transform); Stretch(bg.GetComponent<RectTransform>()); bg.AddComponent<Image>().color = new Color(.012f, .018f, .026f, .995f);
            GameObject card = UI("Card", bg.transform); RectTransform cr = card.GetComponent<RectTransform>(); cr.anchorMin = new Vector2(.045f, .035f); cr.anchorMax = new Vector2(.955f, .965f); cr.offsetMin = cr.offsetMax = Vector2.zero; card.AddComponent<Image>().color = new Color(.047f, .059f, .074f, 1f);
            Text title = Text("ADVANCED DIAGNOSTIC LAB", card.transform, 31, TextAnchor.MiddleLeft, Color.white); RectTransform tr = title.rectTransform; tr.anchorMin = new Vector2(.025f, .91f); tr.anchorMax = new Vector2(.68f, .985f); tr.offsetMin = tr.offsetMax = Vector2.zero;
            Button close = Button("✕ CLOSE", card.transform, new Color(.52f, .12f, .13f, 1f), Close); RectTransform xr = close.GetComponent<RectTransform>(); xr.anchorMin = new Vector2(.82f, .92f); xr.anchorMax = new Vector2(.972f, .975f); xr.offsetMin = xr.offsetMax = Vector2.zero;
            GameObject scroll = UI("Scroll", card.transform); RectTransform sr = scroll.GetComponent<RectTransform>(); sr.anchorMin = new Vector2(.025f, .025f); sr.anchorMax = new Vector2(.975f, .90f); sr.offsetMin = sr.offsetMax = Vector2.zero; ScrollRect s = scroll.AddComponent<ScrollRect>(); s.horizontal = false; s.scrollSensitivity = 40f;
            GameObject viewport = UI("Viewport", scroll.transform); Stretch(viewport.GetComponent<RectTransform>()); viewport.AddComponent<Image>().color = new Color(.022f, .030f, .040f, .75f); Mask mask = viewport.AddComponent<Mask>(); mask.showMaskGraphic = false; s.viewport = viewport.GetComponent<RectTransform>();
            GameObject body = UI("Content", viewport.transform); content = body.GetComponent<RectTransform>(); content.anchorMin = new Vector2(0, 1); content.anchorMax = new Vector2(1, 1); content.pivot = new Vector2(.5f, 1); content.sizeDelta = new Vector2(0, 600); VerticalLayoutGroup v = body.AddComponent<VerticalLayoutGroup>(); v.padding = new RectOffset(18, 18, 16, 22); v.spacing = 10; v.childControlHeight = true; v.childForceExpandHeight = false; v.childControlWidth = true; v.childForceExpandWidth = true; ContentSizeFitter fit = body.AddComponent<ContentSizeFitter>(); fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize; s.content = content;
        }

        private void Refresh()
        {
            game = GameRuntime.Instance; if (content == null || game == null) return; Clear(); MachineState m = game.ActiveMachine; JobState j = game.ActiveJob;
            if (m == null)
            {
                Card("NO DEVICE ON ACTIVE BENCH", "Accept or focus a customer job before running diagnostics.", DiagnosticSeverity.Warning); return;
            }
            Heading((j == null ? "UNASSIGNED DEVICE" : j.jobId + " · " + j.title), m.displayName + " · " + m.category + " · POST " + m.postCode + " · last diagnostic: " + (string.IsNullOrEmpty(m.lastDiagnostic) ? "none" : m.lastDiagnostic));
            Row(new[]
            {
                A("RUN NON-DESTRUCTIVE SUITE", RunSuite),
                A("RUN POST / BOOT TRACE", () => Run(DiagnosticProbeKind.BootTrace)),
                A("CLEAR SESSION EVIDENCE", () => { latest.Clear(); Refresh(); })
            });
            if (latest.Count > 0)
            {
                AdvancedDiagnosticWorkflowService service = Service(); DiagnosticEvidence summary = service.Summarize(latest.Values);
                Card(summary.headline, summary.summary + "\nConfidence " + Mathf.RoundToInt(summary.confidence * 100f) + "%" + (summary.likelyCauses.Count == 0 ? "" : "\nLikely causes: " + string.Join(" · ", summary.likelyCauses)), summary.severity);
            }
            Section("PROBES");
            Probe(DiagnosticProbeKind.VisualInspection, "VISUAL / DAMAGE", "Dust, corrosion, physical damage and component condition.");
            Probe(DiagnosticProbeKind.CableContinuity, "CABLE CONTINUITY", "ATX/EPS/GPU/SATA/front-panel/fan signal-path checks.");
            Probe(DiagnosticProbeKind.PowerBudget, "POWER BUDGET & RAILS", "PSU sizing, headroom, OCP, voltage regulation and ripple evidence.");
            Probe(DiagnosticProbeKind.MemoryIntegrity, "MEMORY INTEGRITY", "DIMM state, training result and stress-test memory errors.");
            Probe(DiagnosticProbeKind.StorageHealth, "STORAGE HEALTH", "Fault state, condition, wear and flash endurance usage.");
            Probe(DiagnosticProbeKind.ThermalLoad, "THERMAL LOAD", "Runs the real power/thermal model under stress and records temperatures.");
            Probe(DiagnosticProbeKind.CoolingAndFans, "COOLING / FANS", "CPU_FAN path, fan failures, bearing condition, dust and airflow.");
            Probe(DiagnosticProbeKind.NetworkHealth, "NETWORK / RAID", "Link, throughput, packet loss, latency and NAS array state where applicable.");
            Probe(DiagnosticProbeKind.BootTrace, "POST / BOOT TRACE", "Runs actual POST logic and reports the persistent POST code and boot state.");
        }

        private AdvancedDiagnosticWorkflowService Service() => new AdvancedDiagnosticWorkflowService(game.Inventory, game.PowerThermal, game.Boot);
        private void Run(DiagnosticProbeKind p)
        {
            MachineState m = game?.ActiveMachine; if (m == null) { game?.Notify("No device on active bench.", false); return; }
            DiagnosticEvidence e = Service().Run(m, p); latest[p] = e; game.Saves?.Save(game.State, 1); game.World?.RefreshMachine(); game.UI?.Refresh(); game.Notify(e.headline + ": " + e.summary, e.severity < DiagnosticSeverity.Critical); Refresh();
        }
        private void RunSuite()
        {
            MachineState m = game?.ActiveMachine; if (m == null) { game?.Notify("No device on active bench.", false); return; }
            foreach (DiagnosticEvidence e in Service().RunNonDestructiveSuite(m)) latest[e.probe] = e;
            DiagnosticEvidence summary = Service().Summarize(latest.Values); game.Saves?.Save(game.State, 1); game.UI?.Refresh(); game.Notify(summary.headline + ": " + summary.summary, summary.severity < DiagnosticSeverity.Critical); Refresh();
        }

        private void Probe(DiagnosticProbeKind p, string title, string description)
        {
            GameObject g = UI("Probe_" + p, content); g.AddComponent<Image>().color = new Color(.085f, .102f, .13f, 1f); HorizontalLayoutGroup h = g.AddComponent<HorizontalLayoutGroup>(); h.padding = new RectOffset(16, 12, 10, 10); h.spacing = 12; h.childControlHeight = true; h.childForceExpandHeight = true; LayoutElement le = g.AddComponent<LayoutElement>(); le.minHeight = 86;
            GameObject info = UI("Info", g.transform); VerticalLayoutGroup iv = info.AddComponent<VerticalLayoutGroup>(); iv.spacing = 2; iv.childForceExpandHeight = false; info.AddComponent<LayoutElement>().flexibleWidth = 1;
            DiagnosticEvidence e; bool has = latest.TryGetValue(p, out e); Text t = Text(title + (has ? " · " + e.severity : ""), info.transform, 20, TextAnchor.MiddleLeft, has ? SevColor(e.severity) : Color.white); t.fontStyle = FontStyle.Bold;
            Text d = Text(description + (has ? "\n" + e.summary + (e.likelyCauses.Count > 0 ? "\nLikely: " + string.Join(" · ", e.likelyCauses) : "") : ""), info.transform, 15, TextAnchor.UpperLeft, new Color(.65f, .72f, .79f, 1f)); d.horizontalOverflow = HorizontalWrapMode.Wrap; d.verticalOverflow = VerticalWrapMode.Overflow;
            Button b = Button(has ? "RERUN" : "RUN", g.transform, new Color(.09f, .45f, .68f, 1f), () => Run(p)); LayoutElement bl = b.gameObject.AddComponent<LayoutElement>(); bl.preferredWidth = 150;
        }

        private struct Act { public string t; public Action a; public Act(string text, Action action) { t = text; a = action; } }
        private static Act A(string t, Action a) => new Act(t, a);
        private void Heading(string a, string b) { Text t = Text(a, content, 25, TextAnchor.MiddleLeft, Color.white); t.fontStyle = FontStyle.Bold; t.gameObject.AddComponent<LayoutElement>().preferredHeight = 38; Text s = Text(b, content, 16, TextAnchor.UpperLeft, new Color(.63f, .70f, .78f, 1f)); s.gameObject.AddComponent<LayoutElement>().preferredHeight = 48; }
        private void Section(string a) { Text t = Text(a, content, 21, TextAnchor.MiddleLeft, new Color(.12f, .68f, .94f, 1f)); t.fontStyle = FontStyle.Bold; t.gameObject.AddComponent<LayoutElement>().preferredHeight = 38; }
        private void Card(string a, string b, DiagnosticSeverity sev) { GameObject g = UI("Card", content); g.AddComponent<Image>().color = new Color(.095f, .115f, .145f, 1f); VerticalLayoutGroup v = g.AddComponent<VerticalLayoutGroup>(); v.padding = new RectOffset(16, 16, 10, 10); v.spacing = 3; v.childForceExpandHeight = false; g.AddComponent<LayoutElement>().minHeight = 78; Text t = Text(a, g.transform, 21, TextAnchor.MiddleLeft, SevColor(sev)); t.fontStyle = FontStyle.Bold; Text s = Text(b, g.transform, 16, TextAnchor.UpperLeft, new Color(.69f, .75f, .82f, 1f)); s.horizontalOverflow = HorizontalWrapMode.Wrap; s.verticalOverflow = VerticalWrapMode.Overflow; }
        private void Row(Act[] actions) { GameObject g = UI("Actions", content); HorizontalLayoutGroup h = g.AddComponent<HorizontalLayoutGroup>(); h.spacing = 10; h.childControlHeight = true; h.childForceExpandHeight = true; h.childForceExpandWidth = true; g.AddComponent<LayoutElement>().preferredHeight = 58; foreach (Act a in actions) { Button b = Button(a.t, g.transform, new Color(.12f, .38f, .58f, 1f), a.a); b.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1; } }
        private static Color SevColor(DiagnosticSeverity s) { if (s == DiagnosticSeverity.Critical) return new Color(1f, .34f, .30f, 1f); if (s == DiagnosticSeverity.Warning) return new Color(1f, .68f, .22f, 1f); if (s == DiagnosticSeverity.Advisory) return new Color(.45f, .76f, 1f, 1f); return new Color(.30f, .92f, .56f, 1f); }
        private Button Button(string label, Transform p, Color c, Action a) { GameObject g = UI("Button_" + label, p); Image i = g.AddComponent<Image>(); i.color = c; Button b = g.AddComponent<Button>(); b.targetGraphic = i; if (a != null) b.onClick.AddListener(() => a()); Text t = Text(label, g.transform, 16, TextAnchor.MiddleCenter, Color.white); Stretch(t.rectTransform, 5); t.raycastTarget = false; return b; }
        private Text Text(string value, Transform p, int size, TextAnchor align, Color c) { GameObject g = UI("Text", p); Text t = g.AddComponent<Text>(); t.font = font; t.text = value; t.fontSize = size; t.alignment = align; t.color = c; t.raycastTarget = false; return t; }
        private GameObject UI(string n, Transform p) { GameObject g = new GameObject(n, typeof(RectTransform)); g.transform.SetParent(p, false); return g; }
        private void Clear() { for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject); }
        private static void Stretch(RectTransform r, float i = 0) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = new Vector2(i, i); r.offsetMax = new Vector2(-i, -i); }
        private static void EnsureEventSystem() { if (FindAnyObjectByType<EventSystem>() != null) return; GameObject e = new GameObject("EventSystem"); e.AddComponent<EventSystem>(); e.AddComponent<StandaloneInputModule>(); DontDestroyOnLoad(e); }
    }
}
