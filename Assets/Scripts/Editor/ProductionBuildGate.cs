#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using ForgeBench;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ForgeBench.EditorTools
{
    /// <summary>Fails a release build instead of silently producing an incomplete player.</summary>
    public sealed class ProductionBuildGate : IPreprocessBuildWithReport
    {
        public int callbackOrder => -9000;
        private static readonly string[] CriticalFiles =
        {
            "Assets/Scenes/Workshop.unity",
            "Assets/Resources/Data/hardware.json",
            "Assets/Resources/Localization/en.json",
            "Assets/Resources/Localization/uk.json",
            "Assets/Scripts/Production/ProductionBootstrap.cs",
            "Assets/Scripts/Production/MainMenuController.cs",
            "Assets/Scripts/Production/WorkshopProductionLayer.cs",
            "Assets/Scripts/Production/PhysicalAssemblyController.cs",
            "Assets/Scripts/Production/ObjectHandlingController.cs",
            "Assets/Scripts/Production/SpecialistRepairPanel.cs",
            "Assets/Scripts/Production/SpecialistStationsLayer.cs",
            "Assets/Scripts/Production/SpecialistContractBoard.cs",
            "Assets/Scripts/Production/SpecialistContractTerminalLayer.cs",
            "Assets/Scripts/Production/SpecialistLifecycle.cs",
            "Assets/Scripts/Production/SpecialistDeviceVisuals.cs",
            "Assets/Scripts/Production/EngineeringDiagnosticsPanel.cs",
            "Assets/Scripts/Production/EngineeringTerminalLayer.cs",
            "Assets/Scripts/Simulation/SpecialistRepairServices.cs",
            "Assets/Scripts/Simulation/SpecialistJobService.cs",
            "Assets/Scripts/Simulation/EngineeringSimulationService.cs",
            "Assets/Scripts/Runtime/SpecialistRuntimeExtensions.cs",
            "Assets/Scripts/Runtime/EngineeringRuntimeExtensions.cs",
            "Assets/link.xml"
        };

        public void OnPreprocessBuild(BuildReport report)
        {
            foreach(string path in CriticalFiles)
                if(!File.Exists(path))throw new BuildFailedException("ForgeBench production build is missing: "+path);

            TextAsset hardware=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/Data/hardware.json");
            if(hardware==null)throw new BuildFailedException("Hardware database could not be imported.");
            HardwareCatalogData data=JsonUtility.FromJson<HardwareCatalogData>(hardware.text);
            if(data?.parts==null||data.parts.Count<70)throw new BuildFailedException("Hardware catalog is too small for the production build ("+(data?.parts?.Count??0)+").");
            if(data.parts.Any(p=>string.IsNullOrWhiteSpace(p.id)||string.IsNullOrWhiteSpace(p.model)))throw new BuildFailedException("Hardware catalog contains an unnamed definition.");
            if(data.parts.Select(p=>p.id).Distinct().Count()!=data.parts.Count)throw new BuildFailedException("Hardware catalog contains duplicate IDs.");
            if(!data.parts.Any(p=>p.category==PartCategory.Battery))throw new BuildFailedException("Portable repair requires at least one Battery definition.");
            if(!data.parts.Any(p=>p.category==PartCategory.Display))throw new BuildFailedException("Portable repair requires at least one Display definition.");
            if(!data.parts.Any(p=>p.category==PartCategory.Storage))throw new BuildFailedException("NAS/server repair requires Storage definitions.");
            if(!data.parts.Any(p=>p.category==PartCategory.PSU))throw new BuildFailedException("Power simulation requires PSU definitions.");
            if(!data.parts.Any(p=>p.category==PartCategory.Fan))throw new BuildFailedException("Airflow simulation requires fan definitions.");

            if(SaveService.CurrentSchema<7)throw new BuildFailedException("Save schema must include engineering simulation state (schema 7+).");

            if(report.summary.platform==BuildTarget.Android)
            {
                PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android,"com.originalforge.forgebench");
                PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
                PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
                PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
                PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
                PlayerSettings.allowedAutorotateToPortrait=false;
                PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
                PlayerSettings.allowedAutorotateToLandscapeLeft=true;
                PlayerSettings.allowedAutorotateToLandscapeRight=true;
            }

            Debug.Log("ForgeBench production gate passed: "+data.parts.Count+" hardware definitions; core, specialist and engineering source present; save schema "+SaveService.CurrentSchema+"; Android settings enforced.");
        }
    }
}
#endif
