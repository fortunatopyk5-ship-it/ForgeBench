using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Owns the persistent service-intake database and keeps active jobs synchronized with front-desk custody state.</summary>
    public sealed class ServiceIntakeDirector : MonoBehaviour
    {
        public static ServiceIntakeDirector Instance { get; private set; }
        public ServiceIntakeService Service { get; private set; }
        private GameRuntime game;
        private float nextPoll;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<ServiceIntakeDirector>() != null) return;
            GameObject go = new GameObject("ForgeBench_ServiceIntakeDirector");
            DontDestroyOnLoad(go);
            go.AddComponent<ServiceIntakeDirector>();
        }

        private IEnumerator Start()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); yield break; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            while (GameRuntime.Instance == null || GameRuntime.Instance.State == null || GameRuntime.Instance.Inventory == null) yield return null;
            Bind();
        }

        private void Bind()
        {
            game = GameRuntime.Instance;
            Service = new ServiceIntakeService(game);
            Service.Poll();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + .65f;
            if (GameRuntime.Instance == null || GameRuntime.Instance.State == null) return;
            if (game != GameRuntime.Instance || Service == null) { Bind(); return; }
            if (Service.Poll()) ServiceIntakePanel.RefreshIfOpen();
        }

        public ActionResult Inspect() => Run(s => s.InspectExterior(ActiveJobId()));
        public ActionResult RecordPower() => Run(s => s.RecordPowerState(ActiveJobId()));
        public ActionResult ConfirmAccessories() => Run(s => s.ConfirmAccessories(ActiveJobId()));
        public ActionResult GrantDataConsent() => Run(s => s.GrantDataConsent(ActiveJobId()));
        public ActionResult ApproveEstimate() => Run(s => s.ApproveEstimate(ActiveJobId()));
        public ActionResult CheckIn() => Run(s => s.CheckIn(ActiveJobId()));
        public ActionResult AuthorizeRelease() => Run(s => s.AuthorizeRelease(ActiveJobId()));

        private string ActiveJobId() => game?.ActiveJob?.jobId;

        private ActionResult Run(System.Func<ServiceIntakeService, ActionResult> action)
        {
            if (Service == null) return ActionResult.Fail("Service intake is still initializing.");
            ActionResult result = action(Service);
            game?.Notify(result.message, result.ok);
            ServiceIntakePanel.RefreshIfOpen();
            return result;
        }

        private void OnApplicationPause(bool paused) { if (paused) Service?.Save(); }
        private void OnApplicationQuit() { Service?.Save(); }
    }
}