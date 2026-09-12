using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>
    /// Applies persistent accessibility settings to runtime-generated interfaces.
    /// Maintains original image colors so color-assist modes are reversible, scales
    /// touch controls, and disables decorative motion when reduced-motion is enabled.
    /// </summary>
    public sealed class AccessibilityRuntimeDirector : MonoBehaviour
    {
        private GameRuntime game;private float nextScan;private int lastMode=-1;private float lastJoystick=-1f;private bool lastReduced;private readonly Dictionary<int,Color> originalColors=new Dictionary<int,Color>();
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<AccessibilityRuntimeDirector>()!=null)return;GameObject g=new GameObject("ForgeBench_AccessibilityRuntime");DontDestroyOnLoad(g);g.AddComponent<AccessibilityRuntimeDirector>();}
        private void Update(){if(Time.unscaledTime<nextScan)return;nextScan=Time.unscaledTime+.8f;game=GameRuntime.Instance;if(game?.State?.settings==null)return;AccessibilitySettings s=game.State.settings;if(s.colorblindMode!=lastMode){lastMode=s.colorblindMode;ApplyColorAssist(lastMode);}if(!Mathf.Approximately(s.joystickSize,lastJoystick)){lastJoystick=s.joystickSize;ApplyJoystickScale();}if(s.reducedMotion!=lastReduced){lastReduced=s.reducedMotion;ApplyMotion();}}
        private void ApplyJoystickScale(){GameObject joy=GameObject.Find("MoveJoystick");if(joy!=null)joy.transform.localScale=Vector3.one*Mathf.Clamp(game.State.settings.joystickSize,.75f,1.45f);GameObject precision=GameObject.Find("Button_PRECISION");if(precision!=null)precision.transform.localScale=Vector3.one*Mathf.Lerp(.95f,1.12f,Mathf.InverseLerp(.75f,1.45f,game.State.settings.joystickSize));}
        private void ApplyMotion(){WorkshopAtmosphereDirector atmosphere=FindAnyObjectByType<WorkshopAtmosphereDirector>();DistanceDetailCuller culler=FindAnyObjectByType<DistanceDetailCuller>();if(game.State.settings.reducedMotion&&atmosphere!=null)atmosphere.enabled=false;else if(atmosphere!=null)atmosphere.enabled=true;if(culler!=null)culler.enabled=true;}
        private void ApplyColorAssist(int mode){Image[] images=FindObjectsByType<Image>(FindObjectsInactive.Include,FindObjectsSortMode.None);foreach(Image img in images){if(img==null)continue;int id=img.GetInstanceID();Color baseColor;if(!originalColors.TryGetValue(id,out baseColor)){baseColor=img.color;originalColors[id]=baseColor;}img.color=Transform(baseColor,mode);}}
        private static Color Transform(Color c,int mode){if(mode<=0)return c;float r=c.r,g=c.g,b=c.b;if(mode==1){return new Color(.567f*r+.433f*g,.558f*r+.442f*g,.242f*g+.758f*b,c.a);}if(mode==2){return new Color(.625f*r+.375f*g,.70f*r+.30f*g,.30f*g+.70f*b,c.a);}return new Color(.95f*r+.05f*g,.433f*g+.567f*b,.475f*g+.525f*b,c.a);}
    }
}
