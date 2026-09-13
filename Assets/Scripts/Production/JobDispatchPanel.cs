using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>Concurrent contract queue, focus switching, due-date triage and specialist intake.</summary>
    public sealed class JobDispatchPanel : MonoBehaviour
    {
        private static JobDispatchPanel instance;
        private GameRuntime game;
        private GameObject root;
        private RectTransform content;
        private Font font;

        public static void Open()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ForgeBench_JobDispatchPanel");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<JobDispatchPanel>();
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

        private JobDispatchService Service()
        {
            return new JobDispatchService(game.State, game.Jobs, new SpecialistJobService(game.State, game.Catalog, game.Inventory, game.Economy));
        }

        private void Show()
        {
            game = GameRuntime.Instance;
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
            root = new GameObject("DispatchRoot", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            Canvas canvas = root.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 26500;
            CanvasScaler scaler = root.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
            root.AddComponent<GraphicRaycaster>();

            GameObject bg = UI("BG", root.transform); Stretch(bg.GetComponent<RectTransform>()); bg.AddComponent<Image>().color = new Color(.012f, .018f, .027f, .995f);
            GameObject card = UI("Card", bg.transform); RectTransform cr = card.GetComponent<RectTransform>(); cr.anchorMin = new Vector2(.04f, .035f); cr.anchorMax = new Vector2(.96f, .965f); cr.offsetMin = cr.offsetMax = Vector2.zero; card.AddComponent<Image>().color = new Color(.05f, .063f, .081f, 1f);

            Text title = Text("WORKSHOP DISPATCH", card.transform, 32, TextAnchor.MiddleLeft, Color.white); RectTransform tr = title.rectTransform; tr.anchorMin = new Vector2(.025f, .91f); tr.anchorMax = new Vector2(.72f, .985f); tr.offsetMin = tr.offsetMax = Vector2.zero;
            Button close = Button("✕ CLOSE", card.transform, new Color(.52f, .12f, .13f, 1f), Close); RectTransform xr = close.GetComponent<RectTransform>(); xr.anchorMin = new Vector2(.83f, .92f); xr.anchorMax = new Vector2(.972f, .975f); xr.offsetMin = xr.offsetMax = Vector2.zero;

            GameObject scroll = UI("Scroll", card.transform); RectTransform sr = scroll.GetComponent<RectTransform>(); sr.anchorMin = new Vector2(.025f, .025f); sr.anchorMax = new Vector2(.975f, .90f); sr.offsetMin = sr.offsetMax = Vector2.zero;
            ScrollRect s = scroll.AddComponent<ScrollRect>(); s.horizontal = false; s.scrollSensitivity = 42f;
            GameObject viewport = UI("Viewport", scroll.transform); Stretch(viewport.GetComponent<RectTransform>()); viewport.AddComponent<Image>().color = new Color(.022f, .030f, .041f, .72f); Mask mask = viewport.AddComponent<Mask>(); mask.showMaskGraphic = false; s.viewport = viewport.GetComponent<RectTransform>();
            GameObject body = UI("Content", viewport.transform); content = body.GetComponent<RectTransform>(); content.anchorMin = new Vector2(0, 1); content.anchorMax = new Vector2(1, 1); content.pivot = new Vector2(.5f, 1); content.sizeDelta = new Vector2(0, 700);
            VerticalLayoutGroup layout = body.AddComponent<VerticalLayoutGroup>(); layout.padding = new RectOffset(18, 18, 16, 24); layout.spacing = 10; layout.childControlHeight = true; layout.childForceExpandHeight = false; layout.childControlWidth = true; layout.childForceExpandWidth = true;
            ContentSizeFitter fit = body.AddComponent<ContentSizeFitter>(); fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize; s.content = content;
        }

        private void Refresh()
        {
            game = GameRuntime.Instance;
            if (game == null || game.State == null || content == null) return;
            Clear();
            JobDispatchService dispatch = Service();
            DispatchSnapshot snap = dispatch.Snapshot();
            string focus = game.ActiveJob == null ? "none" : game.ActiveJob.jobId + " · " + game.ActiveJob.title;
            SummaryCard("CAPACITY " + snap.usedUnits + " / " + snap.capacityUnits + " UNITS", snap.activeJobs + " active contract(s) · " + snap.freeUnits + " free unit(s) · " + snap.overdueJobs + " overdue\nFocused bench: " + focus, snap.overdueJobs > 0 ? new Color(1f, .48f, .25f, 1f) : new Color(.30f, .91f, .57f, 1f));

            ActionRow(new[]
            {
                new NamedAction("FOCUS HIGHEST PRIORITY", () => Apply(dispatch.FocusRecommended())),
                new NamedAction("NEXT ACTIVE JOB", () => Apply(dispatch.FocusNext())),
                new NamedAction("SAVE QUEUE", () => { game.Saves?.Save(game.State, 1); game.Notify("Dispatch queue saved."); })
            });

            Section("ACTIVE CONTRACTS");
            List<JobState> active = dispatch.ActiveJobs.OrderByDescending(dispatch.PriorityScore).ToList();
            if (active.Count == 0) SummaryCard("NO ACTIVE CONTRACTS", "Accept a standard or specialist contract below.", new Color(.55f, .69f, .82f, 1f));
            foreach (JobState j in active) ActiveCard(dispatch, j);

            Section("STANDARD OFFERS");
            List<JobState> offers = game.State.jobs.Where(j => j.stage == JobStage.Offered && !SpecialistJobService.IsSpecialist(j)).OrderBy(j => j.dueDay).ThenByDescending(j => j.reward).Take(6).ToList();
            if (offers.Count == 0) SummaryCard("NO STANDARD OFFERS", "Advance the day to refresh incoming contracts.", new Color(.55f, .69f, .82f, 1f));
            foreach (JobState j in offers) OfferCard(dispatch, j);

            Section("SPECIALIST INTAKE");
            SpecialistRows(dispatch);
        }

        private void ActiveCard(JobDispatchService dispatch, JobState j)
        {
            DispatchRisk risk = dispatch.Risk(j);
            int left = dispatch.DaysRemaining(j);
            bool focused = game.ActiveJob == j;
            string deadline = left < 0 ? Mathf.Abs(left) + " day(s) overdue" : left == 0 ? "due today" : left + " day(s) remaining";
            string specialist = SpecialistJobService.IsSpecialist(j) ? "SPECIALIST · " : string.Empty;
            GameObject row = UI("Active_" + j.jobId, content); row.AddComponent<Image>().color = focused ? new Color(.10f, .18f, .22f, 1f) : new Color(.082f, .100f, .13f, 1f);
            HorizontalLayoutGroup h = row.AddComponent<HorizontalLayoutGroup>(); h.padding = new RectOffset(15, 11, 10, 10); h.spacing = 10; h.childControlHeight = true; h.childForceExpandHeight = true; row.AddComponent<LayoutElement>().minHeight = 92;
            GameObject info = UI("Info", row.transform); VerticalLayoutGroup v = info.AddComponent<VerticalLayoutGroup>(); v.childForceExpandHeight = false; v.spacing = 2; info.AddComponent<LayoutElement>().flexibleWidth = 1;
            Text head = Text((focused ? "▶ " : "") + specialist + j.jobId + " · " + j.title, info.transform, 20, TextAnchor.MiddleLeft, RiskColor(risk)); head.fontStyle = FontStyle.Bold;
            Text detail = Text(j.customerName + " · " + j.deviceCategory + " · " + j.stage + " · load " + dispatch.WorkloadUnits(j) + "\n" + deadline + " · reward $" + j.reward.ToString("0") + " · priority " + dispatch.PriorityScore(j).ToString("0"), info.transform, 15, TextAnchor.UpperLeft, new Color(.66f, .73f, .80f, 1f)); detail.horizontalOverflow = HorizontalWrapMode.Wrap; detail.verticalOverflow = VerticalWrapMode.Overflow;
            Button focus = Button(focused ? "FOCUSED" : "FOCUS", row.transform, focused ? new Color(.12f, .38f, .28f, 1f) : new Color(.10f, .43f, .66f, 1f), focused ? null : (Action)(() => Apply(dispatch.Focus(j)))); focus.interactable = !focused; focus.gameObject.AddComponent<LayoutElement>().preferredWidth = 155;
        }

        private void OfferCard(JobDispatchService dispatch, JobState j)
        {
            string reason; bool can = dispatch.CanAcceptStandard(out reason);
            GameObject row = UI("Offer_" + j.jobId, content); row.AddComponent<Image>().color = new Color(.085f, .103f, .132f, 1f); HorizontalLayoutGroup h = row.AddComponent<HorizontalLayoutGroup>(); h.padding = new RectOffset(15, 11, 9, 9); h.spacing = 10; h.childControlHeight = true; h.childForceExpandHeight = true; row.AddComponent<LayoutElement>().minHeight = 88;
            GameObject info = UI("Info", row.transform); VerticalLayoutGroup v = info.AddComponent<VerticalLayoutGroup>(); v.childForceExpandHeight = false; info.AddComponent<LayoutElement>().flexibleWidth = 1;
            Text head = Text(j.jobId + " · " + j.title, info.transform, 19, TextAnchor.MiddleLeft, Color.white); head.fontStyle = FontStyle.Bold;
            string sub = j.customerName + " · " + j.type + " · due day " + j.dueDay + " · reward $" + j.reward.ToString("0") + " · budget $" + j.budget.ToString("0"); if (!can) sub += "\n" + reason;
            Text detail = Text(sub, info.transform, 15, TextAnchor.UpperLeft, new Color(.65f, .72f, .79f, 1f)); detail.horizontalOverflow = HorizontalWrapMode.Wrap; detail.verticalOverflow = VerticalWrapMode.Overflow;
            Button accept = Button(can ? "ACCEPT" : "CAPACITY FULL", row.transform, can ? new Color(.09f, .49f, .32f, 1f) : new Color(.39f, .16f, .16f, 1f), can ? (Action)(() => Apply(dispatch.AcceptStandard(j))) : null); accept.interactable = can; accept.gameObject.AddComponent<LayoutElement>().preferredWidth = 175;
        }

        private void SpecialistRows(JobDispatchService dispatch)
        {
            SpecialistButtonRow(dispatch, new[] { SpecialistContractKind.LiquidBuild, SpecialistContractKind.BoardRepair, SpecialistContractKind.LaptopBattery, SpecialistContractKind.PhoneDisplay });
            SpecialistButtonRow(dispatch, new[] { SpecialistContractKind.ConsoleController, SpecialistContractKind.NasRecovery, SpecialistContractKind.ServerNetwork });
        }

        private void SpecialistButtonRow(JobDispatchService dispatch, SpecialistContractKind[] kinds)
        {
            GameObject row = UI("SpecialistRow", content); HorizontalLayoutGroup h = row.AddComponent<HorizontalLayoutGroup>(); h.spacing = 9; h.childControlHeight = true; h.childForceExpandHeight = true; h.childForceExpandWidth = true; row.AddComponent<LayoutElement>().preferredHeight = 62;
            foreach (SpecialistContractKind kind in kinds)
            {
                string reason; bool can = dispatch.CanAcceptSpecialist(kind, out reason); int load = JobDispatchService.SpecialistWorkload(kind);
                Button b = Button(ShortKind(kind) + " [" + load + "]", row.transform, can ? new Color(.20f, .32f, .54f, 1f) : new Color(.25f, .20f, .23f, 1f), can ? (Action)(() => Apply(dispatch.AcceptSpecialist(kind))) : null); b.interactable = can; b.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            }
        }

        private static string ShortKind(SpecialistContractKind k)
        {
            switch (k)
            {
                case SpecialistContractKind.LiquidBuild: return "LIQUID LOOP";
                case SpecialistContractKind.BoardRepair: return "BOARD REPAIR";
                case SpecialistContractKind.LaptopBattery: return "LAPTOP";
                case SpecialistContractKind.PhoneDisplay: return "PHONE";
                case SpecialistContractKind.ConsoleController: return "CONTROLLER";
                case SpecialistContractKind.NasRecovery: return "NAS";
                default: return "SERVER";
            }
        }

        private void Apply(ActionResult result)
        {
            if (game == null) return;
            if (result.ok) game.Saves?.Save(game.State, 1);
            game.World?.RefreshMachine();
            game.UI?.Refresh();
            game.Notify(result.message, result.ok);
            Refresh();
        }

        private sealed class NamedAction
        {
            public string label; public Action action;
            public NamedAction(string l, Action a) { label = l; action = a; }
        }

        private void ActionRow(NamedAction[] actions)
        {
            GameObject row = UI("Actions", content); HorizontalLayoutGroup h = row.AddComponent<HorizontalLayoutGroup>(); h.spacing = 10; h.childControlHeight = true; h.childForceExpandHeight = true; h.childForceExpandWidth = true; row.AddComponent<LayoutElement>().preferredHeight = 58;
            foreach (NamedAction a in actions) { Button b = Button(a.label, row.transform, new Color(.12f, .37f, .56f, 1f), a.action); b.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1; }
        }

        private void Section(string title)
        {
            Text t = Text(title, content, 21, TextAnchor.MiddleLeft, new Color(.12f, .69f, .95f, 1f)); t.fontStyle = FontStyle.Bold; t.gameObject.AddComponent<LayoutElement>().preferredHeight = 40;
        }

        private void SummaryCard(string title, string text, Color accent)
        {
            GameObject card = UI("Summary", content); card.AddComponent<Image>().color = new Color(.092f, .112f, .143f, 1f); VerticalLayoutGroup v = card.AddComponent<VerticalLayoutGroup>(); v.padding = new RectOffset(15, 15, 10, 10); v.spacing = 3; v.childForceExpandHeight = false; card.AddComponent<LayoutElement>().minHeight = 72;
            Text a = Text(title, card.transform, 20, TextAnchor.MiddleLeft, accent); a.fontStyle = FontStyle.Bold;
            Text b = Text(text, card.transform, 15, TextAnchor.UpperLeft, new Color(.68f, .75f, .82f, 1f)); b.horizontalOverflow = HorizontalWrapMode.Wrap; b.verticalOverflow = VerticalWrapMode.Overflow;
        }

        private static Color RiskColor(DispatchRisk risk)
        {
            if (risk == DispatchRisk.Failed || risk == DispatchRisk.Overdue) return new Color(1f, .32f, .28f, 1f);
            if (risk == DispatchRisk.High) return new Color(1f, .56f, .20f, 1f);
            if (risk == DispatchRisk.Watch) return new Color(1f, .78f, .28f, 1f);
            if (risk == DispatchRisk.Ready) return new Color(.30f, .92f, .55f, 1f);
            return Color.white;
        }

        private Button Button(string label, Transform parent, Color color, Action action)
        {
            GameObject go = UI("Button_" + label, parent); Image image = go.AddComponent<Image>(); image.color = color; Button b = go.AddComponent<Button>(); b.targetGraphic = image; if (action != null) b.onClick.AddListener(() => action()); Text t = Text(label, go.transform, 15, TextAnchor.MiddleCenter, Color.white); Stretch(t.rectTransform, 5); t.raycastTarget = false; return b;
        }

        private Text Text(string value, Transform parent, int size, TextAnchor align, Color color)
        {
            GameObject go = UI("Text", parent); Text t = go.AddComponent<Text>(); t.font = font; t.text = value; t.fontSize = size; t.alignment = align; t.color = color; t.raycastTarget = false; return t;
        }

        private GameObject UI(string name, Transform parent) { GameObject go = new GameObject(name, typeof(RectTransform)); go.transform.SetParent(parent, false); return go; }
        private void Clear() { for (int i = content.childCount - 1; i >= 0; i--) Destroy(content.GetChild(i).gameObject); }
        private static void Stretch(RectTransform r, float inset = 0) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = new Vector2(inset, inset); r.offsetMax = new Vector2(-inset, -inset); }
        private static void EnsureEventSystem() { if (FindAnyObjectByType<EventSystem>() != null) return; GameObject e = new GameObject("EventSystem"); e.AddComponent<EventSystem>(); e.AddComponent<StandaloneInputModule>(); DontDestroyOnLoad(e); }
    }
}
