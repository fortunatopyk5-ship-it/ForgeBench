using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Lightweight runtime culling for procedural details that cannot use pre-baked
    /// occlusion data. It never takes ownership of LODGroup renderers or interactive
    /// gameplay targets, and uses forceRenderingOff so authored renderer.enabled state
    /// remains authoritative.
    /// </summary>
    public sealed class DistanceDetailCuller : MonoBehaviour
    {
        private sealed class Entry
        {
            public Renderer renderer;
            public float sqrDistance;
            public bool inheritedForceOff;
        }

        private readonly List<Entry> entries=new List<Entry>();
        private Camera cameraRef;
        private float nextScan;
        private float nextRefresh;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if(FindAnyObjectByType<DistanceDetailCuller>()!=null)return;
            GameObject g=new GameObject("ForgeBench_DetailCuller");
            DontDestroyOnLoad(g);
            g.AddComponent<DistanceDetailCuller>();
        }

        private void Update()
        {
            if(Time.unscaledTime>=nextRefresh){nextRefresh=Time.unscaledTime+5f;RefreshList();}
            if(Time.unscaledTime<nextScan)return;
            nextScan=Time.unscaledTime+.30f;
            if(cameraRef==null)cameraRef=Camera.main;
            if(cameraRef==null)return;

            Vector3 p=cameraRef.transform.position;
            float factor=RuntimeRenderBudget.DetailDistanceFactor;
            foreach(Entry e in entries)
            {
                if(e.renderer==null)continue;
                float d=(e.renderer.transform.position-p).sqrMagnitude;
                bool visible=d<=e.sqrDistance*factor*factor;
                e.renderer.forceRenderingOff=e.inheritedForceOff||!visible;
            }
        }

        private void OnDestroy(){RestoreEntries();}

        private void RefreshList()
        {
            RestoreEntries();
            entries.Clear();
            Renderer[] renderers=FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None);
            foreach(Renderer r in renderers)
            {
                if(!ShouldManage(r))continue;
                float dist=ResolveDistance(r);
                if(dist<=0f)continue;
                entries.Add(new Entry{renderer=r,sqrDistance=dist*dist,inheritedForceOff=r.forceRenderingOff});
            }
        }

        private void RestoreEntries()
        {
            foreach(Entry e in entries)
                if(e.renderer!=null)e.renderer.forceRenderingOff=e.inheritedForceOff;
        }

        /// <summary>Returns false when another system owns visibility or the renderer is a gameplay target.</summary>
        public static bool ShouldManage(Renderer r)
        {
            if(r==null)return false;
            if(r.GetComponentInParent<LODGroup>()!=null)return false;
            if(r.GetComponentInParent<WorldInteractable>()!=null)return false;
            return true;
        }

        /// <summary>Returns the culling distance, or zero when the renderer has no culling policy.</summary>
        public static float ResolveDistance(Renderer r)
        {
            if(r==null)return 0f;
            DistanceCullHint hint=r.GetComponentInParent<DistanceCullHint>();
            if(hint!=null)return Mathf.Max(0f,hint.maxDistance);
            string n=r.gameObject.name;
            if(ContainsAny(n,"Peg","Screw","DIMMChip","M2Screw","Probe","Fitting","ServerLED","VentRotor","CableClip","Capacitor"))return 7.5f;
            if(ContainsAny(n,"PartsBin","ShippingCarton","DriverHandle","DriverShaft","RadFan","Scope","Tool"))return 13f;
            if(ContainsAny(n,"Label_"))return 18f;
            return 0f;
        }

        private static bool ContainsAny(string n,params string[] needles)
        {
            foreach(string x in needles)if(n.IndexOf(x,System.StringComparison.OrdinalIgnoreCase)>=0)return true;
            return false;
        }
    }

    /// <summary>
    /// Explicit opt-in distance for authored decorative details. Put this on a parent
    /// visual root rather than relying on object names. LODGroup and WorldInteractable
    /// hierarchies remain excluded even when a hint is present.
    /// </summary>
    public sealed class DistanceCullHint : MonoBehaviour
    {
        [Min(0f)] public float maxDistance=12f;
    }
}
