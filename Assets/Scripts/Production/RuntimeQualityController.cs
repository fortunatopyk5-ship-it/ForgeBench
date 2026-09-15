using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Desktop/non-mobile runtime quality governor. Mobile quality ownership belongs to
    /// MobilePlatformController so two independent governors cannot oscillate settings.
    /// Shared presentation systems consume RuntimeRenderBudget rather than inventing
    /// independent light/LOD/probe rules.
    /// </summary>
    public sealed class RuntimeQualityController : MonoBehaviour
    {
        private float sampleTime;
        private int frames;
        private float smoothedFps=60f;
        private int tier;
        private float nextAdjust;

        private void Start()
        {
            if(Application.isMobilePlatform)
            {
                enabled=false;
                return;
            }
            int memory=SystemInfo.systemMemorySize;
            int cores=SystemInfo.processorCount;
            tier=(memory>=8000&&cores>=8)?2:(memory>=4000&&cores>=6?1:0);
            ApplyTier(tier);
        }

        private void Update()
        {
            frames++;
            sampleTime+=Time.unscaledDeltaTime;
            if(sampleTime>=2f)
            {
                smoothedFps=Mathf.Lerp(smoothedFps,frames/Mathf.Max(.001f,sampleTime),.5f);
                frames=0;sampleTime=0f;
                if(Time.unscaledTime>=nextAdjust){nextAdjust=Time.unscaledTime+8f;AutoAdjust();}
            }
        }

        private void AutoAdjust()
        {
            GameRuntime g=GameRuntime.Instance;
            if(g?.State?.settings==null)return;
            if(g.State.settings.batterySaver){if(tier!=0){tier=0;ApplyTier(tier);}return;}
            int target=Mathf.Clamp(g.State.settings.fpsLimit,30,120);
            if(smoothedFps<target*.68f&&tier>0){tier--;ApplyTier(tier);}
            else if(smoothedFps>target*.94f&&tier<2&&SystemInfo.systemMemorySize>=6000){tier++;ApplyTier(tier);}
        }

        private static void ApplyTier(int value)
        {
            value=Mathf.Clamp(value,0,2);
            RuntimeRenderBudget.SetTier(value);
            QualitySettings.vSyncCount=0;
            QualitySettings.antiAliasing=value==2?4:value==1?2:0;
            QualitySettings.anisotropicFiltering=value==0?AnisotropicFiltering.Disable:AnisotropicFiltering.Enable;
            QualitySettings.shadowResolution=value==2?ShadowResolution.High:value==1?ShadowResolution.Medium:ShadowResolution.Low;
            QualitySettings.pixelLightCount=RuntimeRenderBudget.RealtimeLightBudget;
        }
    }
}
