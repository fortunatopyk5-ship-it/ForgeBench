using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Context-aware feedback beyond button beeps: POST, benchmark, delivery, money and
    /// job-stage changes receive distinct procedural audio/haptic/visual responses.
    /// No external copyrighted audio assets are required.
    /// </summary>
    public sealed class SensoryFeedbackDirector : MonoBehaviour
    {
        private GameRuntime game;private AudioSource source;private AudioClip postOk,postFail,delivery,achievement,benchmarkOk,benchmarkFail;private float nextPoll;private int money,rep,delivered,completed;private string post="";private float benchmark;private bool initialized;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<SensoryFeedbackDirector>()!=null)return;GameObject go=new GameObject("ForgeBench_SensoryFeedback");DontDestroyOnLoad(go);go.AddComponent<SensoryFeedbackDirector>();}
        private IEnumerator Start(){while(GameRuntime.Instance==null||GameRuntime.Instance.State==null)yield return null;game=GameRuntime.Instance;source=game.gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0f;postOk=Sequence("post_ok",new[]{620f,820f},.06f,.10f);postFail=Sequence("post_fail",new[]{260f,190f},.085f,.12f);delivery=Sequence("delivery",new[]{420f,560f,720f},.055f,.08f);achievement=Sequence("achievement",new[]{520f,660f,880f,1040f},.05f,.07f);benchmarkOk=Sequence("bench_ok",new[]{480f,620f,760f},.06f,.09f);benchmarkFail=Sequence("bench_fail",new[]{330f,250f,210f},.07f,.10f);Snapshot();initialized=true;}
        private void Snapshot(){if(game?.State==null)return;money=Mathf.RoundToInt(game.State.money);rep=game.State.reputation;delivered=game.State.shipments.FindAll(x=>x.status==ShipmentStatus.Delivered).Count;completed=game.State.jobs.FindAll(x=>x.stage==JobStage.Completed).Count;MachineState m=game.ActiveMachine;post=m?.postCode??"";benchmark=m?.benchmarkScore??0f;}
        private void Update(){if(!initialized||Time.unscaledTime<nextPoll)return;nextPoll=Time.unscaledTime+.28f;game=GameRuntime.Instance;if(game==null||game.State==null)return;MachineState m=game.ActiveMachine;int nowDelivered=game.State.shipments.FindAll(x=>x.status==ShipmentStatus.Delivered).Count;int nowComplete=game.State.jobs.FindAll(x=>x.stage==JobStage.Completed).Count;string nowPost=m?.postCode??"";float nowBench=m?.benchmarkScore??0f;int nowRep=game.State.reputation;int nowMoney=Mathf.RoundToInt(game.State.money);
            if(nowDelivered>delivered){Play(delivery,false,new Color(.12f,.60f,.95f),"Delivery arrived");}
            if(nowComplete>completed){Play(achievement,true,new Color(.22f,.90f,.48f),"Contract completed");}
            if(!string.IsNullOrEmpty(nowPost)&&nowPost!=post&&nowPost!="OFF"&&nowPost!="SPECIAL"){bool ok=nowPost=="A0";Play(ok?postOk:postFail,!ok,ok?new Color(.20f,.85f,.45f):new Color(.95f,.25f,.18f),ok?"POST passed":"POST failed: "+nowPost);}
            if(nowBench>0f&&!Mathf.Approximately(nowBench,benchmark)){bool ok=m!=null&&m.stressStable;Play(ok?benchmarkOk:benchmarkFail,!ok,ok?new Color(.18f,.82f,.48f):new Color(.95f,.45f,.16f),ok?"Benchmark stable":"Benchmark unstable");}
            if(nowRep>=rep+20)Play(achievement,false,new Color(.72f,.52f,.12f),"Reputation increased");
            if(nowMoney>=money+1000)Play(achievement,false,new Color(.20f,.72f,.42f),"Cash milestone reached");
            delivered=nowDelivered;completed=nowComplete;post=nowPost;benchmark=nowBench;rep=nowRep;money=nowMoney;}
        private void Play(AudioClip clip,bool strongHaptic,Color pulse,string caption){if(game?.State?.settings==null||source==null)return;source.volume=Mathf.Clamp01(game.State.settings.sfxVolume*.75f);source.PlayOneShot(clip);if(game.State.settings.haptics&&Application.isMobilePlatform&&strongHaptic)Handheld.Vibrate();if(game.State.settings.captions&&game.UI!=null)game.UI.ShowToast(caption,!strongHaptic);if(!game.State.settings.reducedMotion)StartCoroutine(Pulse(pulse));}
        private IEnumerator Pulse(Color c){Camera cam=Camera.main;if(cam==null)yield break;GameObject go=new GameObject("FeedbackPulse");go.transform.position=cam.transform.position+cam.transform.forward*.8f;Light l=go.AddComponent<Light>();l.type=LightType.Point;l.range=4f;l.intensity=.7f;l.color=c;float t=0f;while(t<.22f){t+=Time.unscaledDeltaTime;l.intensity=Mathf.Lerp(.7f,0f,t/.22f);yield return null;}Destroy(go);}
        private static AudioClip Sequence(string name,float[] frequencies,float segment,float amplitude){int rate=22050;int count=Mathf.CeilToInt(rate*segment*frequencies.Length);float[] s=new float[count];int per=Mathf.Max(1,Mathf.FloorToInt(rate*segment));for(int i=0;i<count;i++){int idx=Mathf.Clamp(i/per,0,frequencies.Length-1);float local=(i%per)/(float)per;float env=Mathf.Sin(Mathf.PI*Mathf.Clamp01(local));s[i]=Mathf.Sin(2f*Mathf.PI*frequencies[idx]*i/rate)*amplitude*env;}AudioClip clip=AudioClip.Create(name,count,1,rate,false);clip.SetData(s,0);return clip;}
    }
}
