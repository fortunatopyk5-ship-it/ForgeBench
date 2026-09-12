using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    public sealed class ReliabilityLifecycle : MonoBehaviour
    {
        private GameRuntime game;private int observedDay=-1;private float nextPoll;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<ReliabilityLifecycle>()!=null)return;GameObject go=new GameObject("ForgeBench_ReliabilityLifecycle");DontDestroyOnLoad(go);go.AddComponent<ReliabilityLifecycle>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.State==null)yield return null;game=GameRuntime.Instance;observedDay=game.State.day;}
        private void Update(){if(Time.unscaledTime<nextPoll)return;nextPoll=Time.unscaledTime+.5f;game=GameRuntime.Instance;if(game==null||game.State==null)return;if(observedDay<0){observedDay=game.State.day;return;}if(game.State.day<=observedDay)return;int delta=Mathf.Clamp(game.State.day-observedDay,1,30);ReliabilitySimulationService r=new ReliabilitySimulationService(game.State,game.Catalog,game.Inventory);for(int d=0;d<delta;d++)foreach(MachineState m in game.State.machines)r.AdvanceMachineOneDay(m);observedDay=game.State.day;game.Saves?.Save(game.State,1);game.UI?.Refresh();}
    }
}
