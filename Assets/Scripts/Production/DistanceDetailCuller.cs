using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Lightweight runtime culling for procedural details that cannot use pre-baked
    /// occlusion data. Expensive micro-detail renderers are disabled beyond a safe
    /// distance while colliders/interactions remain intact.
    /// </summary>
    public sealed class DistanceDetailCuller : MonoBehaviour
    {
        private sealed class Entry{public Renderer renderer;public float sqrDistance;}
        private readonly List<Entry> entries=new List<Entry>();private Camera cameraRef;private float nextScan;private float nextRefresh;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<DistanceDetailCuller>()!=null)return;GameObject g=new GameObject("ForgeBench_DetailCuller");DontDestroyOnLoad(g);g.AddComponent<DistanceDetailCuller>();}
        private void Update(){if(Time.unscaledTime>=nextRefresh){nextRefresh=Time.unscaledTime+5f;RefreshList();}if(Time.unscaledTime<nextScan)return;nextScan=Time.unscaledTime+.30f;if(cameraRef==null)cameraRef=Camera.main;if(cameraRef==null)return;Vector3 p=cameraRef.transform.position;bool saver=GameRuntime.Instance?.State?.settings?.batterySaver??false;float factor=saver ? .72f : 1f;foreach(Entry e in entries){if(e.renderer==null)continue;float d=(e.renderer.transform.position-p).sqrMagnitude;bool visible=d<=e.sqrDistance*factor*factor;if(e.renderer.enabled!=visible)e.renderer.enabled=visible;}}
        private void RefreshList(){entries.Clear();Renderer[] renderers=FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None);foreach(Renderer r in renderers){if(r==null)continue;string n=r.gameObject.name;float dist=0f;if(ContainsAny(n,"Peg","Screw","DIMMChip","M2Screw","Probe","Fitting","ServerLED","VentRotor","CableClip","Capacitor"))dist=7.5f;else if(ContainsAny(n,"PartsBin","ShippingCarton","DriverHandle","DriverShaft","RadFan","Scope","Tool"))dist=13f;else if(ContainsAny(n,"Label_"))dist=18f;if(dist>0f)entries.Add(new Entry{renderer=r,sqrDistance=dist*dist});}}
        private static bool ContainsAny(string n,params string[] needles){foreach(string x in needles)if(n.IndexOf(x,System.StringComparison.OrdinalIgnoreCase)>=0)return true;return false;}
    }
}
