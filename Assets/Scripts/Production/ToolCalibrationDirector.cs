using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Runs tool-registry wear, calibration drift and maintenance persistence.</summary>
    public sealed class ToolCalibrationDirector : MonoBehaviour
    {
        public static ToolCalibrationDirector Instance { get; private set; }
        public ToolCalibrationService Service { get; private set; }
        private GameRuntime game;
        private float nextPoll;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<ToolCalibrationDirector>() != null) return;
            GameObject go = new GameObject("ForgeBench_ToolCalibrationDirector");
            DontDestroyOnLoad(go);
            go.AddComponent<ToolCalibrationDirector>();
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
            Service = new ToolCalibrationService(game);
            Service.Poll();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + .8f;
            if (GameRuntime.Instance == null || GameRuntime.Instance.State == null) return;
            if (game != GameRuntime.Instance || Service == null) { Bind(); return; }
            if (Service.Poll()) ToolCalibrationPanel.RefreshIfOpen();
        }

        public void Calibrate(string instanceId)
        {
            if (Service == null) return;
            ActionResult result = Service.Calibrate(instanceId);
            game?.Notify(result.message,result.ok);
            ToolCalibrationPanel.RefreshIfOpen();
        }

        public void ServiceTool(string instanceId)
        {
            if (Service == null) return;
            ActionResult result = Service.Service(instanceId);
            game?.Notify(result.message,result.ok);
            ToolCalibrationPanel.RefreshIfOpen();
        }

        public ActionResult ActiveJobReadiness()
        {
            if (Service == null) return ActionResult.Fail("Tool register is still initializing.");
            JobState job = game?.ActiveJob;
            if (job == null) return ActionResult.Fail("No active job.");
            ActionResult result = Service.ReadinessCheck(job.type);
            game.Notify(result.message,result.ok);
            return result;
        }

        private void OnApplicationPause(bool paused) { if (paused) Service?.Save(); }
        private void OnApplicationQuit() { Service?.Save(); }
    }
}