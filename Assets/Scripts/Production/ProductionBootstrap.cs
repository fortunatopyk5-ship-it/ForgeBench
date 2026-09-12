using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>
    /// Production boot guard. It is intentionally created before the first scene,
    /// so an Android player can never end up with only a black framebuffer because
    /// a scene bootstrap component was stripped or failed to deserialize.
    /// </summary>
    [DefaultExecutionOrder(-32000)]
    public sealed class ProductionBootstrap : MonoBehaviour
    {
        private static ProductionBootstrap instance;
        private Canvas bootCanvas;
        private Text bootText;
        private float bootStarted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            if (instance != null) return;
            GameObject root = new GameObject("ForgeBench_ProductionBootstrap");
            DontDestroyOnLoad(root);
            instance = root.AddComponent<ProductionBootstrap>();
            if (FindAnyObjectByType<GameRuntime>() == null)
                root.AddComponent<GameRuntime>();
        }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            bootStarted = Time.realtimeSinceStartup;
            BuildBootOverlay();
            EnsureEmergencyCamera();
        }

        private IEnumerator Start()
        {
            SetBoot("INITIALIZING WORKSHOP…");
            float deadline = Time.realtimeSinceStartup + 15f;
            while (GameRuntime.Instance == null && Time.realtimeSinceStartup < deadline)
                yield return null;

            if (GameRuntime.Instance == null)
            {
                SetBoot("STARTUP ERROR\nGame runtime did not initialize.\nRestart the application.");
                yield break;
            }

            SetBoot("LOADING HARDWARE CATALOG…");
            while ((GameRuntime.Instance.Catalog == null || GameRuntime.Instance.State == null) && Time.realtimeSinceStartup < deadline)
                yield return null;

            SetBoot("BUILDING WORKSHOP…");
            while ((GameRuntime.Instance.World == null || GameRuntime.Instance.UI == null) && Time.realtimeSinceStartup < deadline)
                yield return null;

            if (GameRuntime.Instance.World == null || GameRuntime.Instance.UI == null)
            {
                SetBoot("STARTUP ERROR\nWorkshop systems did not finish loading.\nA diagnostic overlay was kept visible.");
                yield break;
            }

            AttachProductionSystems(GameRuntime.Instance.gameObject);
            SetBoot("READY");
            yield return new WaitForSecondsRealtime(.35f);
            if (bootCanvas != null) Destroy(bootCanvas.gameObject);
        }

        private static void AttachProductionSystems(GameObject host)
        {
            if (host.GetComponent<MainMenuController>() == null) host.AddComponent<MainMenuController>();
            if (host.GetComponent<WorkshopProductionLayer>() == null) host.AddComponent<WorkshopProductionLayer>();
            if (host.GetComponent<PhysicalAssemblyController>() == null) host.AddComponent<PhysicalAssemblyController>();
            if (host.GetComponent<ObjectHandlingController>() == null) host.AddComponent<ObjectHandlingController>();
            if (host.GetComponent<RuntimeQualityController>() == null) host.AddComponent<RuntimeQualityController>();
            if (host.GetComponent<TutorialDirector>() == null) host.AddComponent<TutorialDirector>();
            if (host.GetComponent<ProductionAudio>() == null) host.AddComponent<ProductionAudio>();
        }

        private void EnsureEmergencyCamera()
        {
            if (Camera.main != null || FindAnyObjectByType<Camera>() != null) return;
            GameObject cameraGo = new GameObject("ForgeBench_EmergencyCamera");
            DontDestroyOnLoad(cameraGo);
            Camera cam = cameraGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(.018f, .026f, .038f, 1f);
            cam.depth = -1000f;
        }

        private void BuildBootOverlay()
        {
            GameObject canvasGo = new GameObject("ForgeBench_BootOverlay");
            DontDestroyOnLoad(canvasGo);
            bootCanvas = canvasGo.AddComponent<Canvas>();
            bootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            bootCanvas.sortingOrder = 32000;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = .5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(canvasGo.transform, false);
            RectTransform br = (RectTransform)bg.transform;
            br.anchorMin = Vector2.zero; br.anchorMax = Vector2.one; br.offsetMin = br.offsetMax = Vector2.zero;
            bg.GetComponent<Image>().color = new Color(.018f, .026f, .038f, 1f);

            GameObject title = new GameObject("Title", typeof(RectTransform), typeof(Text));
            title.transform.SetParent(bg.transform, false);
            RectTransform tr = (RectTransform)title.transform;
            tr.anchorMin = new Vector2(.15f, .50f); tr.anchorMax = new Vector2(.85f, .65f); tr.offsetMin = tr.offsetMax = Vector2.zero;
            Text tt = title.GetComponent<Text>();
            tt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            tt.text = "FORGEBENCH"; tt.alignment = TextAnchor.MiddleCenter; tt.fontSize = 58; tt.color = Color.white;

            GameObject status = new GameObject("Status", typeof(RectTransform), typeof(Text));
            status.transform.SetParent(bg.transform, false);
            RectTransform sr = (RectTransform)status.transform;
            sr.anchorMin = new Vector2(.15f, .39f); sr.anchorMax = new Vector2(.85f, .50f); sr.offsetMin = sr.offsetMax = Vector2.zero;
            bootText = status.GetComponent<Text>();
            bootText.font = tt.font; bootText.alignment = TextAnchor.UpperCenter; bootText.fontSize = 24;
            bootText.color = new Color(.66f, .76f, .86f, 1f);
            bootText.text = "STARTING…";

            GameObject line = new GameObject("Accent", typeof(RectTransform), typeof(Image));
            line.transform.SetParent(bg.transform, false);
            RectTransform lr = (RectTransform)line.transform;
            lr.anchorMin = new Vector2(.37f, .49f); lr.anchorMax = new Vector2(.63f, .495f); lr.offsetMin = lr.offsetMax = Vector2.zero;
            line.GetComponent<Image>().color = new Color(.08f, .60f, .92f, 1f);
        }

        private void SetBoot(string text)
        {
            if (bootText == null) return;
            float elapsed = Mathf.Max(0f, Time.realtimeSinceStartup - bootStarted);
            bootText.text = text + "\n" + elapsed.ToString("0.0") + " s";
        }
    }
}
