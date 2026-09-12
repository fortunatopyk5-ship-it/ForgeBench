using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>Executes staffed workshop operations once per persisted game-day transition.</summary>
    public sealed class WorkshopBusinessLifecycle : MonoBehaviour
    {
        private int observedDay=-1;private float nextCheck;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<WorkshopBusinessLifecycle>()!=null)return;GameObject g=new GameObject("ForgeBench_WorkshopBusinessLifecycle");DontDestroyOnLoad(g);g.AddComponent<WorkshopBusinessLifecycle>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.State==null)yield return null;observedDay=GameRuntime.Instance.State.day;}
        private void Update()
        {
            if(Time.unscaledTime<nextCheck)return;nextCheck=Time.unscaledTime+.5f;GameRuntime g=GameRuntime.Instance;if(g==null||g.State==null)return;if(observedDay<0){observedDay=g.State.day;return;}if(observedDay==g.State.day)return;int delta=Mathf.Clamp(g.State.day-observedDay,0,30);observedDay=g.State.day;if(delta<=0)return;
            WorkshopBusinessService s=new WorkshopBusinessService(g.State,g.Catalog,g.Inventory,g.Economy,g.Shipping);bool changed=false;
            for(int d=0;d<delta;d++)
            {
                if(g.State.workshop.helperStaff>0){float before=g.State.workshop.cleanliness;g.State.workshop.cleanliness=Mathf.Clamp01(before+.022f*g.State.workshop.helperStaff);changed|=g.State.workshop.cleanliness!=before;}
                if(g.State.workshop.deliveryAutomation&&g.State.workshop.helperStaff>0){ActionResult r=s.ProcessAutomatedReceiving();changed|=r.ok;}
                if(g.State.workshop.cleanliness<.35f)g.State.reputation=Mathf.Max(0,g.State.reputation-1);
            }
            if(changed)g.Saves?.Save(g.State,1);
        }
    }
}
