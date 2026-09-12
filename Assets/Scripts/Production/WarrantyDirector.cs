using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Continuously snapshots active service quality and advances deterministic warranty callbacks.</summary>
    public sealed class WarrantyDirector : MonoBehaviour
    {
        public static WarrantyDirector Instance { get; private set; }
        public WarrantyService Service { get; private set; }
        private GameRuntime game;
        private float nextPoll;
        private int lastOpenCount;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<WarrantyDirector>() != null) return;
            GameObject go = new GameObject("ForgeBench_WarrantyDirector");
            DontDestroyOnLoad(go);
            go.AddComponent<WarrantyDirector>();
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
            Service = new WarrantyService(game);
            Service.Poll();
            lastOpenCount = Service.OpenCount;
        }

        private void Update()
        {
            if (Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + .8f;
            if (GameRuntime.Instance == null || GameRuntime.Instance.State == null) return;
            if (game != GameRuntime.Instance || Service == null) { Bind(); return; }
            int before = Service.OpenCount;
            if (Service.Poll())
            {
                WarrantyPanel.RefreshIfOpen();
                OperationsDashboardPanel.RefreshIfOpen();
            }
            int after = Service.OpenCount;
            if (after > before || after > lastOpenCount)
                game.Notify("Warranty desk: a post-service case needs attention.", false);
            lastOpenCount = after;
        }

        public ActionResult Schedule(string caseId)
        {
            if (Service == null) return ActionResult.Fail("Warranty database not ready.");
            ActionResult result = Service.ScheduleCallback(caseId);
            game?.Notify(result.message, result.ok);
            WarrantyPanel.RefreshIfOpen();
            OperationsDashboardPanel.RefreshIfOpen();
            return result;
        }

        private void OnApplicationPause(bool paused) { if (paused) Service?.Save(); }
        private void OnApplicationQuit() { Service?.Save(); }
    }
}
