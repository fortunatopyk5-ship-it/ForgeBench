using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>
    /// Read-mostly command center joining jobs, QA, inventory, staffing, CRM, logistics
    /// and warranty exposure without duplicating their underlying simulation state.
    /// </summary>
    public sealed class OperationsDashboardPanel : MonoBehaviour
    {
        private static OperationsDashboardPanel instance;
        private GameObject root;
        private RectTransform content;
        private Text headerStats;
        private Font font;
        private float nextRefresh;

        public static void Open()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ForgeBench_OperationsDashboard");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<OperationsDashboardPanel>();
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
            if (root == null || !root.activeSelf) return;
            if (Input.GetKeyDown(KeyCode.Escape) && !Application.isMobilePlatform) { Close(); return; }
            if (Time.unscaledTime >= nextRefresh) { nextRefresh = Time.unscaledTime + 1.0f; Refresh(); }
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
            root = new GameObject("OperationsDashboardRoot", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            Canvas canvas = root.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 25500;
            CanvasScaler scaler = root.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920,1080); scaler.matchWidthOrHeight = .5f;
            root.AddComponent<GraphicRaycaster>();

            GameObject bg = UI("BG", root.transform); Stretch(bg.GetComponent<RectTransform>()); bg.AddComponent<Image>().color = new Color(.010f,.016f,.024f,.997f);
            GameObject panel = UI("Panel", bg.transform); RectTransform pr = panel.GetComponent<RectTransform>(); pr.anchorMin = new Vector2(.035f,.035f); pr.anchorMax = new Vector2(.965f,.965f); pr.offsetMin = pr.offsetMax = Vector2.zero; panel.AddComponent<Image>().color = new Color(.047f,.058f,.074f,1f);

            GameObject header = UI("Header", panel.transform); RectTransform hr = header.GetComponent<RectTransform>(); hr.anchorMin = new Vector2(0,1); hr.anchorMax = new Vector2(1,1); hr.pivot = new Vector2(.5f,1); hr.sizeDelta = new Vector2(0,92); hr.anchoredPosition = Vector2.zero; header.AddComponent<Image>().color = new Color(.068f,.082f,.101f,1f);
            Text title = Text("WORKSHOP OPERATIONS / QA COMMAND", header.transform, 30, TextAnchor.MiddleLeft, Color.white); RectTransform tr = title.rectTransform; tr.anchorMin = new Vector2(.018f,0); tr.anchorMax = new Vector2(.46f,1); tr.offsetMin = tr.offsetMax = Vector2.zero;
            headerStats = Text("", header.transform, 17, TextAnchor.MiddleRight, new Color(.66f,.74f,.81f,1f)); RectTransform hs = headerStats.rectTransform; hs.anchorMin = new Vector2(.44f,0); hs.anchorMax = new Vector2(.91f,1); hs.offsetMin = hs.offsetMax = Vector2.zero;
            Button close = Button("✕", header.transform, new Color(.55f,.11f,.13f,1f), Close); RectTransform xr = close.GetComponent<RectTransform>(); xr.anchorMin = new Vector2(.925f,.18f); xr.anchorMax = new Vector2(.982f,.82f); xr.offsetMin = xr.offsetMax = Vector2.zero;

            GameObject scrollGo = UI("Scroll", panel.transform); RectTransform sr = scrollGo.GetComponent<RectTransform>(); sr.anchorMin = new Vector2(.018f,.02f); sr.anchorMax = new Vector2(.982f,.90f); sr.offsetMin = sr.offsetMax = Vector2.zero;
            ScrollRect scroll = scrollGo.AddComponent<ScrollRect>(); scroll.horizontal = false; scroll.scrollSensitivity = 44f;
            GameObject viewport = UI("Viewport", scrollGo.transform); Stretch(viewport.GetComponent<RectTransform>()); viewport.AddComponent<Image>().color = new Color(.021f,.028f,.037f,.70f); Mask mask = viewport.AddComponent<Mask>(); mask.showMaskGraphic = false; scroll.viewport = viewport.GetComponent<RectTransform>();
            GameObject body = UI("Content", viewport.transform); content = body.GetComponent<RectTransform>(); content.anchorMin = new Vector2(0,1); content.anchorMax = new Vector2(1,1); content.pivot = new Vector2(.5f,1); content.sizeDelta = new Vector2(0,900);
            VerticalLayoutGroup vg = body.AddComponent<VerticalLayoutGroup>(); vg.padding = new RectOffset(16,16,14,22); vg.spacing = 10; vg.childControlHeight = true; vg.childForceExpandHeight = false; vg.childControlWidth = true; vg.childForceExpandWidth = true;
            ContentSizeFitter fit = body.AddComponent<ContentSizeFitter>(); fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize; scroll.content = content;
        }

        private void Refresh()
        {
            if (content == null) return;
            for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject);
            GameRuntime g = GameRuntime.Instance;
            if (g == null || g.State == null)
            {
                Card("RUNTIME OFFLINE", "Workshop runtime has not initialized.", new Color(.90f,.25f,.20f,1f));
                return;
            }

            int offered = g.State.jobs.Count(x => x.stage == JobStage.Offered);
            int active = g.State.jobs.Count(x => x.stage == JobStage.Accepted || x.stage == JobStage.InProgress || x.stage == JobStage.ReadyToSubmit);
            int overdue = g.State.jobs.Count(x => (x.stage == JobStage.Accepted || x.stage == JobStage.InProgress || x.stage == JobStage.ReadyToSubmit) && g.State.day > x.dueDay);
            int completed = g.State.jobs.Count(x => x.stage == JobStage.Completed);
            int failed = g.State.jobs.Count(x => x.stage == JobStage.Failed);
            int availableInventory = g.State.inventory.Count(x => !x.reserved);
            int reservedInventory = g.State.inventory.Count(x => x.reserved);
            int delivered = g.State.shipments.Count(x => x.status == ShipmentStatus.Delivered);
            int transit = g.State.shipments.Count(x => x.status == ShipmentStatus.InTransit);

            WarrantyService warranty = WarrantyDirector.Instance?.Service;
            CustomerRelationsService crm = CustomerRelationsDirector.Instance?.Service;
            StaffRosterService staff = StaffRosterDirector.Instance?.Service;
            headerStats.text = "DAY " + g.State.day + "   ·   $" + g.State.money.ToString("0") + "   ·   REP " + g.State.reputation + "   ·   XP " + g.State.experience + "   ·   WORKSHOP L" + g.State.workshop.level;

            Section("LIVE OPERATIONS");
            MetricRow(new[]
            {
                Metric("ACTIVE JOBS", active.ToString(), overdue > 0 ? new Color(.94f,.38f,.18f,1f) : new Color(.20f,.78f,.48f,1f)),
                Metric("OPEN OFFERS", offered.ToString(), new Color(.16f,.57f,.88f,1f)),
                Metric("DELIVERIES READY", delivered.ToString(), delivered > 0 ? new Color(.94f,.64f,.16f,1f) : new Color(.58f,.65f,.72f,1f)),
                Metric("IN TRANSIT", transit.ToString(), new Color(.50f,.63f,.82f,1f))
            });
            MetricRow(new[]
            {
                Metric("INVENTORY FREE", availableInventory.ToString(), new Color(.18f,.71f,.48f,1f)),
                Metric("INSTALLED/RESERVED", reservedInventory.ToString(), new Color(.55f,.64f,.74f,1f)),
                Metric("COMPLETED", completed.ToString(), new Color(.18f,.74f,.42f,1f)),
                Metric("FAILED", failed.ToString(), failed > 0 ? new Color(.85f,.22f,.20f,1f) : new Color(.55f,.64f,.72f,1f))
            });

            JobState job = g.ActiveJob; MachineState machine = g.ActiveMachine;
            Section("ACTIVE CONTRACT / FINAL QUALITY");
            if (job == null || machine == null)
            {
                Card("NO DEVICE ON ACTIVE BENCH", "Accept a contract to populate live QA, deadline and reliability telemetry.", new Color(.58f,.66f,.74f,1f));
            }
            else
            {
                int remaining = job.dueDay - g.State.day;
                Color dueColor = remaining < 0 ? new Color(.92f,.24f,.18f,1f) : remaining <= 1 ? new Color(.95f,.62f,.15f,1f) : new Color(.21f,.76f,.46f,1f);
                Card(job.jobId + " · " + job.customerName + " · " + job.title,
                    "Stage " + job.stage + " · deadline day " + job.dueDay + " · " + (remaining >= 0 ? remaining + " day(s) remaining" : (-remaining) + " day(s) overdue") + "\n" +
                    machine.displayName + " · POST " + machine.postCode + " · benchmark " + machine.benchmarkScore.ToString("0") + " · dust " + Mathf.RoundToInt(machine.dust * 100f) + "% · noise " + machine.noiseDb.ToString("0") + " dB", dueColor);
                PreflightReport report = new PreflightInspectionService(g).Inspect(job, machine);
                Card(report.Summary,
                    "Acceptance quality " + report.qualityScore.ToString("0") + "/100 · blocking " + report.blocking + " · warnings " + report.warnings + "\n" +
                    (report.blocking > 0 ? string.Join(" · ", report.findings.Where(x => x.severity == InspectionSeverity.Blocking).Take(3).Select(x => x.system + ": " + x.message)) : "No blocking preflight finding in the current ruleset."),
                    report.pass ? new Color(.18f,.78f,.43f,1f) : new Color(.90f,.28f,.19f,1f));
            }

            Section("CUSTOMER / WARRANTY RISK");
            float satisfaction = crm == null ? 0f : crm.AverageSatisfaction;
            float trust = crm == null ? 0f : crm.AverageTrust;
            int review = crm == null ? 0 : crm.State.reviewScore;
            int openWarranty = warranty == null ? 0 : warranty.OpenCount;
            float exposure = warranty == null ? 0f : warranty.Exposure;
            MetricRow(new[]
            {
                Metric("REVIEW SCORE", crm == null ? "—" : review + "/100", review < 55 ? new Color(.88f,.25f,.18f,1f) : new Color(.16f,.68f,.44f,1f)),
                Metric("SATISFACTION", crm == null ? "—" : satisfaction.ToString("0") + "%", new Color(.16f,.62f,.82f,1f)),
                Metric("TRUST", crm == null ? "—" : trust.ToString("0") + "%", new Color(.42f,.62f,.88f,1f)),
                Metric("WARRANTY EXPOSURE", warranty == null ? "—" : openWarranty + " / $" + exposure.ToString("0"), openWarranty > 0 ? new Color(.95f,.50f,.12f,1f) : new Color(.18f,.72f,.44f,1f))
            });

            Section("STAFF CAPACITY");
            if (staff == null)
            {
                Card("STAFF DATABASE STARTING", "Roster service has not finished initializing.", new Color(.57f,.64f,.72f,1f));
            }
            else
            {
                int count = staff.State.members.Count;
                float fatigue = count == 0 ? 0f : staff.State.members.Average(x => x.fatigue);
                float morale = count == 0 ? 0f : staff.State.members.Average(x => x.morale);
                MetricRow(new[]
                {
                    Metric("TEAM", count.ToString(), new Color(.16f,.58f,.83f,1f)),
                    Metric("AVG FATIGUE", Mathf.RoundToInt(fatigue * 100f) + "%", fatigue > .72f ? new Color(.92f,.35f,.18f,1f) : new Color(.19f,.72f,.43f,1f)),
                    Metric("AVG MORALE", Mathf.RoundToInt(morale * 100f) + "%", morale < .45f ? new Color(.91f,.31f,.19f,1f) : new Color(.18f,.72f,.43f,1f)),
                    Metric("DIAG / BOARD", staff.Assigned(StaffRole.Diagnostics) + " / " + staff.Assigned(StaffRole.BoardRepair), new Color(.48f,.61f,.84f,1f))
                });
                Card("ROLE QUALITY",
                    "Receiving " + Pct(staff.RoleQuality(StaffRole.Receiving)) + " · Assembly " + Pct(staff.RoleQuality(StaffRole.Assembly)) + " · Diagnostics " + Pct(staff.RoleQuality(StaffRole.Diagnostics)) + "\nBoard repair " + Pct(staff.RoleQuality(StaffRole.BoardRepair)) + " · Network " + Pct(staff.RoleQuality(StaffRole.Network)) + " · Customer desk " + Pct(staff.RoleQuality(StaffRole.CustomerDesk)),
                    new Color(.53f,.66f,.83f,1f));
            }

            Section("WORKSHOP HEALTH");
            Card("FACILITY",
                "Level " + g.State.workshop.level + " · bench " + g.State.workshop.benchLevel + " · storage " + g.State.workshop.storageLevel + " · diagnostics " + g.State.workshop.diagnosticsLevel + " · board lab " + g.State.workshop.boardRepairLevel + "\nCleanliness " + Mathf.RoundToInt(g.State.workshop.cleanliness * 100f) + "% · helpers " + g.State.workshop.helperStaff + " · specialists " + g.State.workshop.specialistStaff + " · delivery automation " + (g.State.workshop.deliveryAutomation ? "ON" : "OFF"),
                g.State.workshop.cleanliness < .55f ? new Color(.91f,.42f,.17f,1f) : new Color(.20f,.72f,.45f,1f));

            Section("CONTROL SURFACES");
            ButtonRow(new[]
            {
                Btn("FINAL PREFLIGHT", PreflightInspectionPanel.Open, new Color(.10f,.52f,.76f,1f)),
                Btn("WARRANTY", WarrantyPanel.Open, new Color(.70f,.36f,.10f,1f)),
                Btn("CUSTOMERS", () => CustomerRelationsPanel.Open(), new Color(.15f,.54f,.72f,1f)),
                Btn("STAFF", StaffRosterPanel.Open, new Color(.20f,.48f,.68f,1f))
            });
            ButtonRow(new[]
            {
                Btn("INVENTORY", VirtualizedInventoryPanel.Open, new Color(.12f,.48f,.63f,1f)),
                Btn("MAINTENANCE", MaintenancePanel.Open, new Color(.54f,.46f,.08f,1f)),
                Btn("RECEIVE READY", () => { g.ReceiveAll(); Refresh(); }, delivered > 0 ? new Color(.16f,.58f,.38f,1f) : new Color(.30f,.34f,.39f,1f)),
                Btn("JOB BOARD", () => { Close(); g.UI?.OpenTab("JOBS"); }, new Color(.12f,.45f,.68f,1f))
            });
            Text caution = Text("Advance Day remains outside this dashboard on purpose: day advancement processes costs, deadlines, shipping, reliability and warranty timelines and is therefore never a hidden quick action.", content, 14, TextAnchor.UpperLeft, new Color(.55f,.62f,.69f,1f)); caution.gameObject.AddComponent<LayoutElement>().preferredHeight = 42;
        }

        private struct MetricData { public string name; public string value; public Color color; }
        private struct ButtonData { public string label; public Action action; public Color color; }
        private static MetricData Metric(string n, string v, Color c) { return new MetricData { name = n, value = v, color = c }; }
        private static ButtonData Btn(string l, Action a, Color c) { return new ButtonData { label = l, action = a, color = c }; }
        private static string Pct(float value) { return Mathf.RoundToInt(Mathf.Clamp01(value) * 100f) + "%"; }

        private void MetricRow(MetricData[] data)
        {
            GameObject row = UI("MetricRow", content); HorizontalLayoutGroup h = row.AddComponent<HorizontalLayoutGroup>(); h.spacing = 8; h.childControlHeight = true; h.childForceExpandHeight = true; h.childControlWidth = true; h.childForceExpandWidth = true; row.AddComponent<LayoutElement>().preferredHeight = 82;
            foreach (MetricData d in data)
            {
                GameObject card = UI("Metric", row.transform); card.AddComponent<Image>().color = new Color(.082f,.101f,.128f,1f); VerticalLayoutGroup v = card.AddComponent<VerticalLayoutGroup>(); v.padding = new RectOffset(10,10,8,8); v.spacing = 2; v.childControlHeight = true; v.childForceExpandHeight = true;
                Text a = Text(d.name, card.transform, 14, TextAnchor.MiddleCenter, new Color(.61f,.69f,.77f,1f)); Text b = Text(d.value, card.transform, 23, TextAnchor.MiddleCenter, d.color); b.fontStyle = FontStyle.Bold;
            }
        }

        private void ButtonRow(ButtonData[] data)
        {
            GameObject row = UI("ButtonRow", content); HorizontalLayoutGroup h = row.AddComponent<HorizontalLayoutGroup>(); h.spacing = 8; h.childControlHeight = true; h.childForceExpandHeight = true; h.childControlWidth = true; h.childForceExpandWidth = true; row.AddComponent<LayoutElement>().preferredHeight = 58;
            foreach (ButtonData d in data) { Button b = Button(d.label, row.transform, d.color, d.action); b.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1; }
        }

        private void Section(string text)
        {
            Text t = Text(text, content, 20, TextAnchor.MiddleLeft, new Color(.10f,.64f,.92f,1f)); t.fontStyle = FontStyle.Bold; t.gameObject.AddComponent<LayoutElement>().preferredHeight = 34;
        }

        private void Card(string title, string detail, Color accent)
        {
            GameObject card = UI("Card", content); card.AddComponent<Image>().color = new Color(.082f,.100f,.126f,1f); VerticalLayoutGroup v = card.AddComponent<VerticalLayoutGroup>(); v.padding = new RectOffset(14,14,9,11); v.spacing = 3; v.childControlHeight = true; v.childForceExpandHeight = false;
            Text a = Text(title, card.transform, 19, TextAnchor.MiddleLeft, accent); a.fontStyle = FontStyle.Bold; Text b = Text(detail, card.transform, 15, TextAnchor.UpperLeft, new Color(.64f,.71f,.78f,1f)); b.horizontalOverflow = HorizontalWrapMode.Wrap; b.verticalOverflow = VerticalWrapMode.Overflow;
        }

        private Button Button(string label, Transform parent, Color color, Action action)
        {
            GameObject go = UI("Button_" + label, parent); Image image = go.AddComponent<Image>(); image.color = color; Button b = go.AddComponent<Button>(); b.targetGraphic = image; if (action != null) b.onClick.AddListener(() => action()); Text t = Text(label, go.transform, 15, TextAnchor.MiddleCenter, Color.white); Stretch(t.rectTransform, 4); t.raycastTarget = false; return b;
        }
        private Text Text(string value, Transform parent, int size, TextAnchor align, Color color) { GameObject go = UI("Text", parent); Text t = go.AddComponent<Text>(); t.font = font; t.text = value; t.fontSize = size; t.alignment = align; t.color = color; t.raycastTarget = false; return t; }
        private static GameObject UI(string name, Transform parent) { GameObject go = new GameObject(name, typeof(RectTransform)); go.transform.SetParent(parent, false); return go; }
        private static void Stretch(RectTransform rect, float inset = 0f) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = new Vector2(inset,inset); rect.offsetMax = new Vector2(-inset,-inset); }
        private static void EnsureEventSystem() { if (FindAnyObjectByType<EventSystem>() != null) return; GameObject e = new GameObject("EventSystem"); e.AddComponent<EventSystem>(); e.AddComponent<StandaloneInputModule>(); DontDestroyOnLoad(e); }
    }
}
