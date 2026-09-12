using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Observes the normal job loop and keeps customer relationship state synchronized without duplicating job processing.</summary>
    public sealed class CustomerRelationsDirector : MonoBehaviour
    {
        public static CustomerRelationsDirector Instance { get; private set; }
        public CustomerRelationsService Service { get; private set; }
        private GameRuntime game;
        private float nextPoll;
        private int lastDay = -1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<CustomerRelationsDirector>() != null) return;
            GameObject go = new GameObject("ForgeBench_CustomerRelations");
            DontDestroyOnLoad(go);
            go.AddComponent<CustomerRelationsDirector>();
        }

        private IEnumerator Start()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); yield break; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            while (GameRuntime.Instance == null || GameRuntime.Instance.State == null) yield return null;
            Bind();
        }

        private void Bind()
        {
            game = GameRuntime.Instance;
            Service = new CustomerRelationsService(game.State);
            lastDay = game.State.day;
            Service.ObserveJobs();
            Service.DailyTick(game);
        }

        private void Update()
        {
            if (Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + .65f;
            if (GameRuntime.Instance == null || GameRuntime.Instance.State == null) return;
            if (game != GameRuntime.Instance || Service == null) Bind();
            if (Service.ObserveJobs()) CustomerRelationsPanel.RefreshIfOpen();
            if (lastDay != game.State.day)
            {
                lastDay = game.State.day;
                Service.DailyTick(game);
                CustomerRelationsPanel.RefreshIfOpen();
            }
        }

        public ActionResult Invite(string id)
        {
            if (Service == null) return ActionResult.Fail("Customer database not ready.");
            CustomerProfile p = null;
            foreach (CustomerProfile candidate in Service.Profiles) if (candidate.customerId == id) { p = candidate; break; }
            ActionResult r = Service.InviteRepeatCustomer(p);
            if (r.ok) game.Saves?.Save(game.State, 1);
            game.Notify(r.message, r.ok);
            return r;
        }

        public ActionResult ResolveComplaint(string id)
        {
            if (Service == null) return ActionResult.Fail("Customer database not ready.");
            ActionResult r = Service.ResolveComplaint(game, id);
            if (r.ok) game.Saves?.Save(game.State, 1);
            game.Notify(r.message, r.ok);
            return r;
        }

        private void OnApplicationPause(bool paused) { if (paused) Service?.Save(); }
        private void OnApplicationQuit() { Service?.Save(); }
    }
}
