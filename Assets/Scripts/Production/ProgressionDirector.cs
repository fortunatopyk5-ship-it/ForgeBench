using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    public sealed class ProgressionDirector : MonoBehaviour
    {
        public static ProgressionDirector Instance { get; private set; }
        public ProgressionService Service { get; private set; }
        private GameRuntime game;
        private int lastExperience=-1,lastReputation=-1,lastWorkshop=-1;
        private float nextPoll;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if(FindAnyObjectByType<ProgressionDirector>()!=null)return;
            GameObject go=new GameObject("ForgeBench_ProgressionDirector");DontDestroyOnLoad(go);go.AddComponent<ProgressionDirector>();
        }

        private IEnumerator Start()
        {
            if(Instance!=null&&Instance!=this){Destroy(gameObject);yield break;}Instance=this;DontDestroyOnLoad(gameObject);
            while(GameRuntime.Instance==null||GameRuntime.Instance.State==null||GameRuntime.Instance.Catalog==null)yield return null;
            Bind();
        }

        private void Bind()
        {
            game=GameRuntime.Instance;Service=new ProgressionService(game.State,game.Catalog);Service.ApplyPermanentEffects();
            lastExperience=game.State.experience;lastReputation=game.State.reputation;lastWorkshop=game.State.workshop.level;
        }

        private void Update()
        {
            if(Time.unscaledTime<nextPoll)return;nextPoll=Time.unscaledTime+.75f;
            if(GameRuntime.Instance==null||GameRuntime.Instance.State==null)return;
            if(game!=GameRuntime.Instance||Service==null){Bind();return;}
            if(lastExperience!=game.State.experience||lastReputation!=game.State.reputation||lastWorkshop!=game.State.workshop.level)
            {
                int before=Service.Level;lastExperience=game.State.experience;lastReputation=game.State.reputation;lastWorkshop=game.State.workshop.level;
                Service.ApplyPermanentEffects();game.Saves?.Save(game.State,1);ProgressionPanel.RefreshIfOpen();
                if(Service.Level>before)game.Notify("Technician level increased to "+Service.Level+".");
            }
        }

        public ActionResult Unlock(string id)
        {
            if(Service==null)return ActionResult.Fail("Progression system not ready.");ActionResult r=Service.Unlock(id);if(r.ok)game.Saves?.Save(game.State,1);game.Notify(r.message,r.ok);ProgressionPanel.RefreshIfOpen();return r;
        }
    }
}
