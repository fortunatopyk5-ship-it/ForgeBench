using System;
using System.Collections;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Android/mobile lifecycle guard: autosaves around focus transitions, responds to
    /// low-memory callbacks, caps render cost under sustained frame pressure and battery
    /// saver conditions, and restores quality gradually instead of oscillating every frame.
    /// </summary>
    public sealed class MobilePlatformController : MonoBehaviour
    {
        private GameRuntime game;
        private float sampleTimer;
        private float frameAccumulator;
        private int frameCount;
        private int adaptiveTier = 2;
        private float lastTierChange;
        private bool lowMemoryRecovery;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<MobilePlatformController>() != null) return;
            GameObject go = new GameObject("ForgeBench_MobilePlatformController");
            DontDestroyOnLoad(go);
            go.AddComponent<MobilePlatformController>();
        }

        private IEnumerator Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.lowMemory += OnLowMemory;
            while (GameRuntime.Instance == null || GameRuntime.Instance.State == null) yield return null;
            game = GameRuntime.Instance;
            ApplyPlatformDefaults();
        }

        private void OnDestroy() { Application.lowMemory -= OnLowMemory; }

        private void Update()
        {
            if (game == null) game = GameRuntime.Instance;
            if (game?.State?.settings == null) return;
            frameAccumulator += Time.unscaledDeltaTime;
            frameCount++;
            sampleTimer += Time.unscaledDeltaTime;
            if (sampleTimer < 3.0f) return;
            float fps = frameAccumulator > .001f ? frameCount / frameAccumulator : 60f;
            sampleTimer = 0f; frameAccumulator = 0f; frameCount = 0;
            Adapt(fps);
        }

        private void ApplyPlatformDefaults()
        {
            if (game?.State?.settings == null) return;
            Application.targetFrameRate = game.State.settings.batterySaver ? 30 : Mathf.Clamp(game.State.settings.fpsLimit, 30, 120);
            QualitySettings.vSyncCount = 0;
            if (Application.isMobilePlatform)
            {
                QualitySettings.shadowDistance = Mathf.Min(QualitySettings.shadowDistance, 32f);
                QualitySettings.lodBias = Mathf.Clamp(QualitySettings.lodBias, .65f, 1.3f);
            }
        }

        private void Adapt(float fps)
        {
            if (!Application.isMobilePlatform || Time.unscaledTime - lastTierChange < 8f) return;
            bool saver = game.State.settings.batterySaver;
            float battery = SystemInfo.batteryLevel;
            bool lowBattery = battery >= 0f && battery < .18f && SystemInfo.batteryStatus != BatteryStatus.Charging;
            float target = Mathf.Clamp(game.State.settings.fpsLimit, 30, 120);
            int next = adaptiveTier;
            if (lowMemoryRecovery || saver || lowBattery || fps < Mathf.Min(36f, target * .68f)) next = Mathf.Max(0, adaptiveTier - 1);
            else if (fps > Mathf.Min(58f, target * .92f) && !saver && !lowBattery) next = Mathf.Min(3, adaptiveTier + 1);
            if (next != adaptiveTier) { adaptiveTier = next; lastTierChange = Time.unscaledTime; ApplyTier(); }
            lowMemoryRecovery = false;
        }

        private void ApplyTier()
        {
            switch (adaptiveTier)
            {
                case 0:
                    QualitySettings.shadowDistance = 10f; QualitySettings.lodBias = .55f; QualitySettings.globalTextureMipmapLimit = 1;
                    if (game != null) Application.targetFrameRate = 30;
                    break;
                case 1:
                    QualitySettings.shadowDistance = 18f; QualitySettings.lodBias = .72f; QualitySettings.globalTextureMipmapLimit = 1;
                    if (game != null) Application.targetFrameRate = game.State.settings.batterySaver ? 30 : Mathf.Min(45, game.State.settings.fpsLimit);
                    break;
                case 2:
                    QualitySettings.shadowDistance = 28f; QualitySettings.lodBias = .95f; QualitySettings.globalTextureMipmapLimit = 0;
                    if (game != null) Application.targetFrameRate = game.State.settings.batterySaver ? 30 : Mathf.Min(60, game.State.settings.fpsLimit);
                    break;
                default:
                    QualitySettings.shadowDistance = 38f; QualitySettings.lodBias = 1.18f; QualitySettings.globalTextureMipmapLimit = 0;
                    if (game != null) Application.targetFrameRate = game.State.settings.batterySaver ? 30 : Mathf.Clamp(game.State.settings.fpsLimit, 30, 120);
                    break;
            }
            Debug.Log("[ForgeBench] Adaptive mobile tier " + adaptiveTier + " · target " + Application.targetFrameRate + " FPS");
        }

        private void OnLowMemory()
        {
            lowMemoryRecovery = true;
            adaptiveTier = 0;
            lastTierChange = Time.unscaledTime;
            ApplyTier();
            if (game?.State != null) game.Saves?.Save(game.State, 1);
            StartCoroutine(RecoverMemory());
        }

        private IEnumerator RecoverMemory()
        {
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
            Debug.Log("[ForgeBench] Low-memory recovery completed.");
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused && game?.State != null) game.Saves?.Save(game.State, 1);
            if (!paused) { ApplyPlatformDefaults(); StartCoroutine(ResumeStabilization()); }
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused && game?.State != null) game.Saves?.Save(game.State, 1);
            if (focused) ApplyPlatformDefaults();
        }

        private IEnumerator ResumeStabilization()
        {
            Time.timeScale = 1f;
            yield return null;
            yield return null;
            if (AudioListener.pause) AudioListener.pause = false;
            MobileInputState.Move = Vector2.zero;
        }
    }
}
