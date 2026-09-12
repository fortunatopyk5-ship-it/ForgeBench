using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Local-only release diagnostics. Captures exceptions/asserts and lifecycle markers
    /// into a bounded text log under persistentDataPath. No network transmission, user
    /// identifiers, tokens or arbitrary save contents are recorded.
    /// </summary>
    public sealed class CrashTelemetryLogger : MonoBehaviour
    {
        private const long MaxBytes = 256 * 1024;
        private static CrashTelemetryLogger instance;
        private string path;
        private bool quitting;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            if(instance!=null||FindAnyObjectByType<CrashTelemetryLogger>()!=null)return;
            GameObject go=new GameObject("ForgeBench_CrashTelemetry");DontDestroyOnLoad(go);instance=go.AddComponent<CrashTelemetryLogger>();
        }

        private void Awake()
        {
            if(instance!=null&&instance!=this){Destroy(gameObject);return;}instance=this;DontDestroyOnLoad(gameObject);
            path=Path.Combine(Application.persistentDataPath,"ForgeBenchCrash.log");RotateIfNeeded();Application.logMessageReceived+=OnLog;Write("SESSION START · "+Application.version+" · "+SystemInfo.operatingSystem+" · "+SystemInfo.deviceModel+" · RAM "+SystemInfo.systemMemorySize+" MB · GPU "+SystemInfo.graphicsDeviceName);
        }

        private void OnDestroy(){Application.logMessageReceived-=OnLog;}
        private void OnApplicationPause(bool paused){if(paused)Write("APP PAUSE");else Write("APP RESUME");}
        private void OnApplicationFocus(bool focused){if(!focused)Write("FOCUS LOST");}
        private void OnApplicationQuit(){quitting=true;Write("SESSION END · clean quit");}

        private void OnLog(string condition,string stackTrace,LogType type)
        {
            if(type!=LogType.Exception&&type!=LogType.Error&&type!=LogType.Assert)return;
            string cleanCondition=Sanitize(condition,1400);string cleanStack=Sanitize(stackTrace,5000);
            Write(type+" · "+cleanCondition+(string.IsNullOrWhiteSpace(cleanStack)?string.Empty:"\n"+cleanStack));
            if(!quitting&&GameRuntime.Instance?.State!=null)GameRuntime.Instance.Saves?.Save(GameRuntime.Instance.State,1);
        }

        private void Write(string text)
        {
            try
            {
                RotateIfNeeded();
                string line="["+DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")+"] "+Sanitize(text,7000)+Environment.NewLine;
                File.AppendAllText(path,line,Encoding.UTF8);
            }
            catch { }
        }

        private void RotateIfNeeded()
        {
            try
            {
                if(string.IsNullOrEmpty(path))path=Path.Combine(Application.persistentDataPath,"ForgeBenchCrash.log");
                FileInfo info=new FileInfo(path);if(!info.Exists||info.Length<MaxBytes)return;
                string old=path+".previous";if(File.Exists(old))File.Delete(old);File.Move(path,old);
            }
            catch { }
        }

        public static string Sanitize(string input,int maxLength)
        {
            if(string.IsNullOrEmpty(input))return string.Empty;
            string s=input.Replace("\0",string.Empty).Replace("\r\n","\n").Replace("\r","\n");
            // Do not persist obvious authorization/token lines even if a third-party package logs one by mistake.
            string[] lines=s.Split('\n');StringBuilder b=new StringBuilder();
            foreach(string raw in lines)
            {
                string lower=raw.ToLowerInvariant();
                if(lower.Contains("authorization:")||lower.Contains("bearer ")||lower.Contains("password=")||lower.Contains("token="))continue;
                if(b.Length>0)b.Append('\n');b.Append(raw);
                if(b.Length>=maxLength)break;
            }
            if(b.Length>maxLength)b.Length=maxLength;
            return b.ToString();
        }
    }
}
