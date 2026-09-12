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

            Debug.Log("ForgeBench production gate passed: "+data.parts.Count+" hardware definitions, critical source/content present, Android settings enforced.");
        }
    }
}
#endif
