#if UNITY_EDITOR
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace ForgeBench.EditorTools
{
    /// <summary>Dedicated build-time contract for the non-destructive A/B hardware planner.</summary>
    public sealed class HardwareComparisonBuildGate : IPreprocessBuildWithReport
    {
        public int callbackOrder => -8990;
        public void OnPreprocessBuild(BuildReport report)
        {
            const string servicePath = "Assets/Scripts/Simulation/HardwareComparisonService.cs";
            const string panelPath = "Assets/Scripts/Production/FitmentPlannerPanel.cs";
            const string testPath = "Assets/Tests/EditMode/HardwareComparisonTests.cs";
            foreach (string path in new[] { servicePath, panelPath, testPath })
                if (!File.Exists(path)) throw new BuildFailedException("Hardware comparison production contract is missing: " + path);

            string service = File.ReadAllText(servicePath);
            string panel = File.ReadAllText(panelPath);
            string tests = File.ReadAllText(testPath);
            foreach (string token in new[] { "HardwareComparisonVerdict", "technicalDelta", "efficiencyDelta", "valueDelta", "FitmentPlanningService", "IsJobRelevant" })
                if (!service.Contains(token)) throw new BuildFailedException("Hardware comparison service is missing production token: " + token);
            if (service.Contains(".Install(") || service.Contains("InstallBestAvailable"))
                throw new BuildFailedException("HardwareComparisonService must remain non-destructive and may not install hardware.");
            foreach (string token in new[] { "COMPARE", "RELEVANT ONLY", "A/B ENGINEERING COMPARISON", "technicalDelta", "valueDelta" })
                if (!panel.Contains(token)) throw new BuildFailedException("Fitment planner comparison UI is missing: " + token);
            foreach (string token in new[] { "DamagedCandidate_IsBlocked", "StorageCandidate_IsClassifiedAsExpansion", "SeparatesTechnicalEfficiencyAndValueDimensions", "RequiredCategory_IsMarkedJobRelevant" })
                if (!tests.Contains(token)) throw new BuildFailedException("Hardware comparison tests are missing: " + token);
        }
    }
}
#endif
