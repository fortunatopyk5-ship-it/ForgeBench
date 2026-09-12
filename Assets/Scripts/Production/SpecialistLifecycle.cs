using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Applies time-based wear to specialist systems after the ordinary day cycle.
    /// It watches persisted day state rather than using real-time timers, so save/load
    /// and pause/resume cannot duplicate wear events.
    /// </summary>
    public sealed class SpecialistLifecycle : MonoBehaviour
    {
        private GameRuntime game;
        private int observedDay=-1;
        private float nextCheck;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if(FindAnyObjectByType<SpecialistLifecycle>()!=null)return;
            GameObject go=new GameObject("ForgeBench_SpecialistLifecycle");DontDestroyOnLoad(go);go.AddComponent<SpecialistLifecycle>();
        }

        private IEnumerator Start()
        {
            while(GameRuntime.Instance==null||GameRuntime.Instance.State==null)yield return null;
            game=GameRuntime.Instance;observedDay=game.State.day;
        }

        private void Update()
        {
            if(Time.unscaledTime<nextCheck)return;nextCheck=Time.unscaledTime+.5f;
            game=GameRuntime.Instance;if(game==null||game.State==null)return;
            if(observedDay<0){observedDay=game.State.day;return;}
            if(game.State.day==observedDay)return;
            int delta=Mathf.Clamp(game.State.day-observedDay,0,365);observedDay=game.State.day;
            if(delta<=0)return;
            bool changed=false;
            foreach(MachineState m in game.State.machines)
            {
                for(int d=0;d<delta;d++)
                {
                    if(m.liquidLoop!=null&&m.liquidLoop.coolantLitres>0f)
                    {
                        new LiquidCoolingService(game.Inventory,game.State.workshop).AgeOneDay(m);changed=true;
                    }
                    if(m.portable!=null)
                    {
                        m.portable.batteryHealth=Mathf.Max(.05f,m.portable.batteryHealth-.00025f);
                        if(!m.portable.sealed)m.portable.adhesiveIntegrity=Mathf.Max(0f,m.portable.adhesiveIntegrity-.006f);
                        changed=true;
                    }
                    if(m.network!=null&&m.network.linkUp)
                    {
                        m.network.throughputMbps=0f;m.network.latencyMs=0;
                        changed=true;
                    }
                }
            }
            if(changed)game.Saves?.Save(game.State,1);
        }
    }
}
