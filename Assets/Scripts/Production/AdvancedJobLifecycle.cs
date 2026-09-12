using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Ensures advanced procedural offers exist after startup and each day.</summary>
    public sealed class AdvancedJobLifecycle : MonoBehaviour
    {
        private int observedDay=-1;private float nextCheck;private bool seeded;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<AdvancedJobLifecycle>()!=null)return;GameObject g=new GameObject("ForgeBench_AdvancedJobLifecycle");DontDestroyOnLoad(g);g.AddComponent<AdvancedJobLifecycle>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.State==null)yield return null;Seed();observedDay=GameRuntime.Instance.State.day;}
        private void Update(){if(Time.unscaledTime<nextCheck)return;nextCheck=Time.unscaledTime+.65f;GameRuntime g=GameRuntime.Instance;if(g==null||g.State==null)return;if(!seeded)Seed();if(observedDay<0){observedDay=g.State.day;return;}if(g.State.day!=observedDay){observedDay=g.State.day;Seed();}}
        private void Seed(){GameRuntime g=GameRuntime.Instance;if(g==null||g.State==null)return;new AdvancedJobGeneratorService(g.State).EnsureOffers(3);g.Saves?.Save(g.State,1);g.UI?.Refresh();seeded=true;}
    }
}
