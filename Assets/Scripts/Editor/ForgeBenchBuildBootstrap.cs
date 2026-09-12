#if UNITY_EDITOR
using System.IO;
using ForgeBench;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ForgeBench.EditorTools
{
    /// <summary>
    /// Makes the cloud-built player self contained even when the repository was
    /// imported from a text-only source tree. In particular it guarantees that:
    ///  - GameRuntime is serialized into the first scene (not only auto-booted);
    ///  - an actual camera exists before runtime scripts execute;
    ///  - the URP Lit shader is referenced by a Resources material so stripping
    ///    cannot remove the shader used by the procedural workshop.
    /// </summary>
    public sealed class ForgeBenchBuildBootstrap : IPreprocessBuildWithReport
    {
        private const string ScenePath = "Assets/Scenes/Workshop.unity";
        private const string ResourceDir = "Assets/Resources/Generated";
        private const string RuntimeMaterialPath = ResourceDir + "/RuntimeLit.mat";

        public int callbackOrder => -10000;

        public void OnPreprocessBuild(BuildReport report)
        {
            PrepareBuildScene();
        }

        [MenuItem("ForgeBench/Prepare Cloud Build Scene")]
        public static void PrepareBuildScene()
        {
            if (!File.Exists(ScenePath))
                throw new BuildFailedException("Missing startup scene: " + ScenePath);

            Directory.CreateDirectory(ResourceDir);
            EnsureRuntimeShaderReference();

            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject anchor = GameObject.Find("ForgeBenchSceneAnchor");
            if (anchor == null)
            {
                anchor = new GameObject("ForgeBenchSceneAnchor");
            }

            if (anchor.GetComponent<GameRuntime>() == null)
                anchor.AddComponent<GameRuntime>();

            // A real serialized camera prevents a completely black framebuffer if
            // any runtime-generated world object fails during startup. The player
            // camera created by WorkshopWorld renders later at depth 0.
            GameObject fallback = GameObject.Find("ForgeBenchFallbackCamera");
            if (fallback == null)
                fallback = new GameObject("ForgeBenchFallbackCamera");

            Camera cam = fallback.GetComponent<Camera>();
            if (cam == null) cam = fallback.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.025f, 0.035f, 0.05f, 1f);
            cam.depth = -100f;
            cam.fieldOfView = 65f;
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 150f;
            fallback.transform.position = new Vector3(0f, 2.0f, -6.5f);
            fallback.transform.rotation = Quaternion.Euler(8f, 0f, 0f);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("ForgeBench cloud-build bootstrap prepared: runtime, fallback camera and runtime shader reference are present.");
        }

        private static void EnsureRuntimeShaderReference()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null)
                throw new BuildFailedException("No runtime render shader could be resolved for ForgeBench.");

            Material material = AssetDatabase.LoadAssetAtPath<Material>(RuntimeMaterialPath);
            if (material == null)
            {
                material = new Material(shader)
                {
                    name = "ForgeBench Runtime Lit",
                    color = new Color(0.18f, 0.22f, 0.28f, 1f)
                };
                AssetDatabase.CreateAsset(material, RuntimeMaterialPath);
            }
            else if (material.shader != shader)
            {
                material.shader = shader;
                EditorUtility.SetDirty(material);
            }
        }
    }
}
#endif
