using UnityEngine;

namespace ForgeBench
{
    /// <summary>Low-cost procedural workshop ambience; no external copyrighted audio assets.</summary>
    public sealed class ProductionAudio : MonoBehaviour
    {
        private AudioSource ambience;
        private AudioClip clip;

        private void Start()
        {
            ambience=gameObject.AddComponent<AudioSource>();
            ambience.loop=true;ambience.playOnAwake=false;ambience.spatialBlend=0f;
            clip=CreateWorkshopHum();ambience.clip=clip;
            ApplyVolume();ambience.Play();
        }

        private void Update()
        {
            if(ambience==null)return;
            if(Time.frameCount%120==0)ApplyVolume();
        }

        private void ApplyVolume()
        {
            GameRuntime g=GameRuntime.Instance;
            float master=g?.State?.settings?.musicVolume??.5f;
            ambience.volume=Mathf.Clamp01(master)*.10f;
        }

        private static AudioClip CreateWorkshopHum()
        {
            const int rate=22050;const int seconds=4;int count=rate*seconds;float[] data=new float[count];
            uint noise=0x1234ABCDu;
            for(int i=0;i<count;i++)
            {
                float t=i/(float)rate;
                noise=noise*1664525u+1013904223u;
                float n=((noise>>9)&0x7FFF)/16384f-1f;
                float baseHum=Mathf.Sin(t*Mathf.PI*2f*50f)*.020f+Mathf.Sin(t*Mathf.PI*2f*100f)*.006f;
                float fan=n*.004f*(.65f+.35f*Mathf.Sin(t*.7f));
                data[i]=baseHum+fan;
            }
            AudioClip c=AudioClip.Create("ForgeBench_WorkshopAmbience",count,1,rate,false);c.SetData(data,0);return c;
        }
    }
}
