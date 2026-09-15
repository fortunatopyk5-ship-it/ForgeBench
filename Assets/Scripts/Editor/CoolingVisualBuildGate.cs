#if UNITY_EDITOR
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ForgeBench.EditorTools
{
    /// <summary>Prevents a production build from silently losing the original cooling visual fallback.</summary>
    public sealed class CoolingVisualBuildGate : IPreprocessBuildWithReport
    {
        public int callbackOrder => -8990;

        public void OnPreprocessBuild(BuildReport report)
        {
            string[] required =
            {
                "Assets/Scripts/Production/ProceduralCoolingVisuals.cs",
                "Assets/Scripts/Production/CoolingVisualUpgradeDirector.cs",
                "Assets/Tests/EditMode/ProceduralCoolingVisualTests.cs",
                "Docs/PROCEDURAL_COOLING_MODELS.md",
                "Docs/hardware_visual_profiles.json"
            };
            foreach (string path in required)
                if (!File.Exists(path)) throw new BuildFailedException("ForgeBench cooling visual build gate is missing: " + path);

            HardwareDefinition rad240 = new HardwareDefinition { id = "cooler_rad240", model = "AIO 240", radiatorSupportMm = 360 };
            HardwareDefinition rad280 = new HardwareDefinition { id = "cooler_rad280", model = "AIO 280", radiatorSupportMm = 360 };
            if (ProceduralCoolingVisuals.ResolveRadiatorMm(rad240) != 240 || ProceduralCoolingVisuals.ResolveRadiatorMm(rad280) != 280)
                throw new BuildFailedException("Cooling visual radiator identity resolver regressed.");

            Debug.Log("ForgeBench cooling visual gate passed: original fan/AIO fallback source, tests and art manifest are present.");
        }
    }
}
#endif
