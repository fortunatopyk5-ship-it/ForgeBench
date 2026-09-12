using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    public sealed class StaffRosterDirector : MonoBehaviour
    {
        public static StaffRosterDirector Instance{get;private set;}public StaffRosterService Service{get;private set;}private GameRuntime game;private int helpers=-1,specialists=-1,day=-1;private float nextPoll;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<StaffRosterDirector>()!=null)return;GameObject g=new GameObject("ForgeBench_StaffRosterDirector");DontDestroyOnLoad(g);g.AddComponent<StaffRosterDirector>();}
        private IEnumerator Start(){if(Instance!=null&&Instance!=this){Destroy(gameObject);yield break;}Instance=this;DontDestroyOnLoad(gameObject);while(GameRuntime.Instance==null||GameRuntime.Instance.State==null)yield return null;Bind();}
        private void Bind(){game=GameRuntime.Instance;Service=new StaffRosterService(game);helpers=game.State.workshop.helperStaff;specialists=game.State.workshop.specialistStaff;day=game.State.day;Service.DayTick();}
        private void Update(){if(Time.unscaledTime<nextPoll)return;nextPoll=Time.unscaledTime+.7f;if(GameRuntime.Instance==null||GameRuntime.Instance.State==null)return;if(game!=GameRuntime.Instance||Service==null){Bind();return;}if(helpers!=game.State.workshop.helperStaff||specialists!=game.State.workshop.specialistStaff){helpers=game.State.workshop.helperStaff;specialists=game.State.workshop.specialistStaff;Service.SyncCounts();StaffRosterPanel.RefreshIfOpen();}if(day!=game.State.day){day=game.State.day;Service.DayTick();StaffRosterPanel.RefreshIfOpen();}}
        public ActionResult Assign(string id,StaffRole role){ActionResult r=Service==null?ActionResult.Fail("Staff roster not ready."):Service.Assign(id,role);game?.Notify(r.message,r.ok);StaffRosterPanel.RefreshIfOpen();return r;}
        public ActionResult Train(string id){ActionResult r=Service==null?ActionResult.Fail("Staff roster not ready."):Service.Train(id);game?.Notify(r.message,r.ok);StaffRosterPanel.RefreshIfOpen();return r;}
        private void OnApplicationPause(bool p){if(p)Service?.Save();}private void OnApplicationQuit(){Service?.Save();}
    }
}
