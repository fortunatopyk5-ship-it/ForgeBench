using System;
using System.IO;
using UnityEngine;

namespace ForgeBench
{
    public sealed class FeedbackService : MonoBehaviour
    {
        private AudioSource uiSource;
        private AudioClip okClip;
        private AudioClip errorClip;

        public void Initialize()
        {
            uiSource = gameObject.AddComponent<AudioSource>(); uiSource.playOnAwake = false; uiSource.spatialBlend = 0f;
            okClip = Tone("ui_ok", 720f, .075f, .15f);
            errorClip = Tone("ui_error", 210f, .11f, .18f);
        }

        public void Play(bool success)
        {
            GameRuntime g = GameRuntime.Instance;
            if (uiSource == null || g?.State?.settings == null) return;
            uiSource.volume = Mathf.Clamp01(g.State.settings.sfxVolume);
            uiSource.PlayOneShot(success ? okClip : errorClip);
            if (!success && g.State.settings.haptics && Application.isMobilePlatform) Handheld.Vibrate();
        }

        private static AudioClip Tone(string name, float hz, float seconds, float amplitude)
        {
            int rate = 22050, count = Mathf.CeilToInt(rate * seconds); float[] samples = new float[count];
            for (int i = 0; i < count; i++)
            {
                float env = 1f - i / (float)count;
                samples[i] = Mathf.Sin(2f * Mathf.PI * hz * i / rate) * amplitude * env;
            }
            AudioClip clip = AudioClip.Create(name, count, 1, rate, false); clip.SetData(samples, 0); return clip;
        }
    }

    public sealed class CrashLogService : MonoBehaviour
    {
        private string path;
        private void Awake()
        {
            path = Path.Combine(Application.persistentDataPath, "forgebench_runtime.log");
            Application.logMessageReceived += OnLog;
        }
        private void OnDestroy() { Application.logMessageReceived -= OnLog; }
        private void OnLog(string condition, string stackTrace, LogType type)
        {
            if (type != LogType.Exception && type != LogType.Error && type != LogType.Assert) return;
            try { File.AppendAllText(path, DateTime.UtcNow.ToString("o") + " [" + type + "] " + condition + "\n" + stackTrace + "\n"); } catch { }
        }
    }
}
