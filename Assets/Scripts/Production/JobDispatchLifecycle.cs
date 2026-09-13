using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Day-boundary dispatch watchdog: surfaces deadline/capacity pressure without changing player focus automatically.</summary>
    public sealed class JobDispatchLifecycle : MonoBehaviour
    {
        private GameRuntime game;
        private int observedDay = -1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<JobDispatchLifecycle>() != null) return;
            GameObject go = new GameObject("ForgeBench_JobDispatchLifecycle");
            DontDestroyOnLoad(go);
            go.AddComponent<JobDispatchLifecycle>();
        }

        private IEnumerator Start()
        {
            while (GameRuntime.Instance == null || GameRuntime.Instance.State == null) yield return null;
            game = GameRuntime.Instance;
            observedDay = game.State.day;
        }

        private void Update()
        {
            if (GameRuntime.Instance == null || GameRuntime.Instance.State == null) return;
            game = GameRuntime.Instance;
            if (game.State.day == observedDay) return;
            observedDay = game.State.day;
            JobDispatchService dispatch = new JobDispatchService(game.State, game.Jobs, new SpecialistJobService(game.State, game.Catalog, game.Inventory, game.Economy));
            DispatchSnapshot s = dispatch.Snapshot();
            if (s.activeJobs == 0) return;
            if (s.overdueJobs > 0)
                game.Notify("Dispatch alert: " + s.overdueJobs + " contract(s) overdue. Highest priority: " + (s.recommended == null ? "none" : s.recommended.jobId) + ".", false);
            else if (s.recommended != null && dispatch.Risk(s.recommended) == DispatchRisk.High)
                game.Notify("Dispatch alert: " + s.recommended.jobId + " is due today.", false);
            else if (s.usedUnits >= s.capacityUnits)
                game.Notify("Dispatch capacity fully allocated: " + s.usedUnits + "/" + s.capacityUnits + " units.");
            JobDispatchPanel.RefreshIfOpen();
        }
    }
}
