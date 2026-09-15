using System;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Shared presentation budget used by the desktop/mobile governors and visual systems.
    /// It deliberately owns only presentation cost, never gameplay state.
    /// </summary>
    public static class RuntimeRenderBudget
    {
        private static int currentTier=2;
        public static int CurrentTier => currentTier;
        public static event Action<int> Changed;

        public static int RealtimeLightBudget => currentTier<=0?2:currentTier==1?3:currentTier==2?5:8;
        public static int ReflectionProbeResolution => currentTier<=0?0:currentTier==1?32:currentTier==2?64:128;
        public static float DetailDistanceFactor => currentTier<=0?.62f:currentTier==1?.78f:currentTier==2?1f:1.18f;
        public static float ShadowDistance => currentTier<=0?10f:currentTier==1?18f:currentTier==2?28f:40f;
        public static float LodBias => currentTier<=0?.55f:currentTier==1?.75f:currentTier==2?1f:1.25f;
        public static int TextureMipmapLimit => currentTier<=1?1:0;

        public static void SetTier(int value)
        {
            value=Mathf.Clamp(value,0,3);
            bool changed=value!=currentTier;
            currentTier=value;
            ApplyCommonQuality();
            if(changed)Changed?.Invoke(currentTier);
        }

        public static void ApplyCommonQuality()
        {
            QualitySettings.shadowDistance=ShadowDistance;
            QualitySettings.lodBias=LodBias;
            QualitySettings.globalTextureMipmapLimit=TextureMipmapLimit;
            QualitySettings.realtimeReflectionProbes=ReflectionProbeResolution>0;
        }
    }
}
