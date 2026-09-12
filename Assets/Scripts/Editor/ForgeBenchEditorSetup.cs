#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace ForgeBench.EditorTools
{
    [InitializeOnLoad]
    public static class ForgeBenchEditorSetup
    {
        private const string ScenePath = "Assets/Scenes/Workshop.unity";
        private const string PipelinePath = "Assets/Settings/ForgeBenchURP.asset";
        private const string SetupMarker = "Assets/Settings/.configured";

        static ForgeBenchEditorSetup() { EditorApplication.delayCall += ConfigureIfNeeded; }

        [MenuItem("ForgeBench/Configure Project")]
        public static void ConfigureIfNeeded()
        {
            if (File.Exists(SetupMarker)) return;
            Directory.CreateDirectory("Assets/Settings");
            ConfigurePlayer();
            EnsureUrp();
            EnsureScene();
            File.WriteAllText(SetupMarker, DateTime.UtcNow.ToString("o"));
            AssetDatabase.Refresh();
            Debug.Log("ForgeBench project configured for Android ARM64 / URP.");
        }

        private static void ConfigurePlayer()
        {
            PlayerSettings.companyName = "Original Forge Workshop";
            PlayerSettings.productName = "ForgeBench";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.originalforge.forgebench");
            PlayerSettings.bundleVersion = "0.9.0";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.Android.bundleVersionCode = 90;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        }

        private static void EnsureUrp()
        {
            UniversalRenderPipelineAsset existing = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelinePath);
            if (existing != null) { GraphicsSettings.defaultRenderPipeline = existing; QualitySettings.renderPipeline = existing; return; }
            UniversalRenderPipelineAsset asset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
            AssetDatabase.CreateAsset(asset, PipelinePath);
            MethodInfo load = typeof(UniversalRenderPipelineAsset).GetMethod("LoadBuiltinRendererData", BindingFlags.Instance | BindingFlags.Public);
            if (load != null)
            {
                ParameterInfo[] p = load.GetParameters();
                object[] args = p.Length == 0 ? null : new[] { Activator.CreateInstance(p[0].ParameterType) };
                object renderer = load.Invoke(asset, args);
                UnityEngine.Object rendererObject = renderer as UnityEngine.Object;
                if (rendererObject != null && !AssetDatabase.Contains(rendererObject)) AssetDatabase.AddObjectToAsset(rendererObject, asset);
            }
            EditorUtility.SetDirty(asset); AssetDatabase.SaveAssets();
            GraphicsSettings.defaultRenderPipeline = asset; QualitySettings.renderPipeline = asset;
        }

        private static void EnsureScene()
        {
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            if (sceneAsset == null)
            {
                if (File.Exists(ScenePath)) File.Delete(ScenePath);
                Scene s = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                new GameObject("ForgeBenchSceneAnchor");
                EditorSceneManager.SaveScene(s, ScenePath);
                AssetDatabase.ImportAsset(ScenePath, ImportAssetOptions.ForceUpdate);
            }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        [MenuItem("ForgeBench/Build Android APK")]
        public static void BuildAndroid()
        {
            ConfigurePlayer(); EnsureUrp(); EnsureScene(); ForgeBenchContentValidator.ValidateOrThrow();
            Directory.CreateDirectory("Builds/Android");
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = "Builds/Android/ForgeBench.apk",
                target = BuildTarget.Android,
                options = BuildOptions.CompressWithLz4HC
            };
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded) throw new Exception("Android build failed: " + report.summary.result);
            Debug.Log("APK built: " + Path.GetFullPath(options.locationPathName) + " (" + report.summary.totalSize + " bytes)");
        }

        [MenuItem("ForgeBench/Run EditMode Tests")]
        public static void OpenTestRunner() { EditorApplication.ExecuteMenuItem("Window/General/Test Runner"); }
    }
}
#endif
