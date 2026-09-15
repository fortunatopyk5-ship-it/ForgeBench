#if UNITY_EDITOR
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ForgeBench.EditorTools
{
    public sealed class VisualPolishBuildGate : IPreprocessBuildWithReport
    {
        public int callbackOrder => -8985;

        public void OnPreprocessBuild(BuildReport report)
        {
            string[] required =
            {
                "Assets/Scripts/Production/ProceduralCoolingVisuals.cs",
                "Assets/Scripts/Production/CoolingVisualUpgradeDirector.cs",
                "Assets/Scripts/Production/ProceduralSurfaceLibrary.cs",
                "Assets/Scripts/Production/VisualMaterialUpgradeDirector.cs",
                "Assets/Tests/EditMode/ProceduralCoolingVisualTests.cs",
                "Assets/Tests/EditMode/ProceduralSurfaceLibraryTests.cs",
                "Docs/hardware_visual_profiles.json",
                "Docs/VERIFIED_CC0_TEXTURE_SET.md"
            };
            foreach (string path in required)
                if (!File.Exists(path)) throw new BuildFailedException("ForgeBench visual polish build gate is missing: " + path);

            HardwareDefinition rad240 = new HardwareDefinition { id = "cooler_rad240", model = "AIO 240", radiatorSupportMm = 360 };
            HardwareDefinition rad280 = new HardwareDefinition { id = "cooler_rad280", model = "AIO 280", radiatorSupportMm = 360 };
            if (ProceduralCoolingVisuals.ResolveRadiatorMm(rad240) != 240 || ProceduralCoolingVisuals.ResolveRadiatorMm(rad280) != 280)
                throw new BuildFailedException("Cooling visual radiator identity resolver regressed.");

            if (ProceduralSurfaceLibrary.Classify("Main Assembly Bench/Top") != ProceduralSurfaceProfile.EsdTeal)
                throw new BuildFailedException("Procedural surface semantic mapping regressed.");
            float n = ProceduralSurfaceLibrary.Noise01(17, 29, 73);
            if (n < 0f || n > 1f) throw new BuildFailedException("Procedural surface noise escaped [0,1].");

            Debug.Log("ForgeBench visual polish gate passed: cooling geometry, runtime PBR fallback, tests and art manifests are present.");
        }
    }
}
#endif
