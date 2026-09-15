#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ForgeBench.EditorTools
{
    /// <summary>Prevents a production build from silently losing or mis-sizing the visual fallback stack.</summary>
    public sealed class CoolingVisualBuildGate : IPreprocessBuildWithReport
    {
        public int callbackOrder => -8990;

        public void OnPreprocessBuild(BuildReport report)
        {
            string[] required =
            {
                "Assets/Scripts/Production/ProceduralCoolingVisuals.cs",
                "Assets/Scripts/Production/CoolingVisualUpgradeDirector.cs",
                "Assets/Scripts/Production/ProceduralGeneratedMeshOwner.cs",
                "Assets/Scripts/Production/HardwarePresentationLayout.cs",
                "Assets/Scripts/Production/ProceduralSurfaceLibrary.cs",
                "Assets/Scripts/Production/HardwareSurfaceUpgradeDirector.cs",
                "Assets/Tests/EditMode/ProceduralCoolingVisualTests.cs",
                "Assets/Tests/EditMode/ProceduralSurfaceLibraryTests.cs",
                "Docs/PROCEDURAL_COOLING_MODELS.md",
                "Docs/hardware_visual_profiles.json"
            };
            foreach (string path in required)
                if (!File.Exists(path)) throw new BuildFailedException("ForgeBench visual build gate is missing: " + path);

            HardwareDefinition rad240 = new HardwareDefinition
            {
                id = "cooler_rad240",
                model = "AIO 240",
                radiatorSupportMm = 360,
                tags = new List<string> { "aio" }
            };
            HardwareDefinition rad280 = new HardwareDefinition
            {
                id = "cooler_rad280",
                model = "AIO 280",
                radiatorSupportMm = 360,
                tags = new List<string> { "aio" }
            };
            HardwareDefinition fan140 = new HardwareDefinition
            {
                id = "fan_large",
                model = "Flow 140",
                fanSizeMm = 120,
                tags = new List<string> { "140mm" }
            };

            bool radiatorIdentityOk =
                ProceduralCoolingVisuals.ResolveRadiatorMm(rad240) == 240 &&
                ProceduralCoolingVisuals.ResolveRadiatorMm(rad280) == 280 &&
                HardwarePresentationLayout.ResolveRadiatorMm(rad240) == 240 &&
                HardwarePresentationLayout.ResolveRadiatorMm(rad280) == 280 &&
                HardwarePresentationLayout.AioFanCount(rad240) == 2 &&
                HardwarePresentationLayout.AioFanCount(rad280) == 2;
            if (!radiatorIdentityOk)
                throw new BuildFailedException("Cooling visual radiator identity/layout resolver regressed.");

            if (HardwarePresentationLayout.ResolveFanMm(fan140) != 140 || HardwarePresentationLayout.FanVisualDiameter(fan140) <= .22f)
                throw new BuildFailedException("Cooling visual 140 mm fan identity/layout resolver regressed.");

            Color32 surfaceA = ProceduralSurfaceLibrary.Sample(ProceduralSurfaceKind.Graphite, 7, 11);
            Color32 surfaceB = ProceduralSurfaceLibrary.Sample(ProceduralSurfaceKind.Graphite, 31, 43);
            if (surfaceA.a != 255 || surfaceB.a != 255 || surfaceA.r == surfaceB.r)
                throw new BuildFailedException("Procedural surface fallback lost deterministic material variation.");
            if (HardwareSurfaceUpgradeDirector.ResolveKindForName("Motherboard") != ProceduralSurfaceKind.Pcb ||
                HardwareSurfaceUpgradeDirector.ResolveKindForName("Trace_0") != ProceduralSurfaceKind.Copper)
                throw new BuildFailedException("Procedural surface material mapping regressed.");

            Debug.Log("ForgeBench visual gate passed: cooling geometry, surface fallback, mesh lifetime guard, layout identity, tests and art manifest are present.");
        }
    }
}
#endif