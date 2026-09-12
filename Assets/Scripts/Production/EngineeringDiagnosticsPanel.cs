using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>
    /// Mobile-first engineering console exposing persistent POST/firmware, airflow,
    /// power, thermal, storage, OS and benchmark simulations.
    /// </summary>
    public sealed class EngineeringDiagnosticsPanel : MonoBehaviour
    {
        private static EngineeringDiagnosticsPanel instance;
        private GameRuntime game;
        private GameObject root;
        private RectTransform content;
        private Font font;
        private int page;

        public static void Open(int requestedPage = 0)
        {
            if (instance == null)
            {
                GameObject host = new GameObject("ForgeBench_EngineeringDiagnosticsPanel");
                DontDestroyOnLoad(host);
                instance = host.AddComponent<EngineeringDiagnosticsPanel>();
            }
            instance.page = Mathf.Clamp(requestedPage, 0, 3);
            instance.Show();
        }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this; DontDestroyOnLoad(gameObject);
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureEventSystem(); Build(); root.SetActive(false);
        }

        private void Update()
        {
            if (root != null && root.activeSelf && Input.GetKeyDown(KeyCode.Escape) && !Application.isMobilePlatform) Close();
        }

        private void Show()
        {
            game = GameRuntime.Instance; if (root == null) Build(); root.SetActive(true);
            Cursor.lockState = CursorLockMode.None; MobileInputState.Move = Vector2.zero; Refresh();
        }

        private void Close(){if(root!=null)root.SetActive(false);Cursor.lockState=CursorLockMode.Locked;}

        private void Build()
        {
            if(root!=null)return;
            root=new GameObject("EngineeringPanelRoot",typeof(RectTransform));root.transform.SetParent(transform,false);
            Canvas canvas=root.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=25500;
            CanvasScaler scaler=root.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=.5f;root.AddComponent<GraphicRaycaster>();
            GameObject bg=UI("Background",root.transform);Stretch(bg.GetComponent<RectTransform>());bg.AddComponent<Image>().color=new Color(.014f,.020f,.029f,.99f);
            GameObject panel=UI("Panel",bg.transform);RectTransform pr=panel.GetComponent<RectTransform>();pr.anchorMin=new Vector2(.045f,.045f);pr.anchorMax=new Vector2(.955f,.955f);pr.offsetMin=pr.offsetMax=Vector2.zero;panel.AddComponent<Image>().color=new Color(.047f,.058f,.073f,1f);
            GameObject header=UI("Header",panel.transform);RectTransform hr=header.GetComponent<RectTransform>();hr.anchorMin=new Vector2(0,1);hr.anchorMax=new Vector2(1,1);hr.pivot=new Vector2(.5f,1);hr.sizeDelta=new Vector2(0,86);hr.anchoredPosition=Vector2.zero;header.AddComponent<Image>().color=new Color(.065f,.080f,.100f,1f);
            Text title=Text("ENGINEERING CONSOLE",header.transform,30,TextAnchor.MiddleLeft,Color.white);RectTransform tr=title.rectTransform;tr.anchorMin=new Vector2(.025f,0);tr.anchorMax=new Vector2(.35f,1);tr.offsetMin=tr.offsetMax=Vector2.zero;
            string[] pages={"POST / BIOS","THERMAL / POWER","OS / STORAGE","BENCHMARK"};
            for(int i=0;i<pages.Length;i++){int capture=i;Button b=Button(pages[i],header.transform,new Color(.11f,.14f,.18f,1f),()=>{page=capture;Refresh();});RectTransform r=b.GetComponent<RectTransform>();float x=.36f+i*.135f;r.anchorMin=new Vector2(x,.16f);r.anchorMax=new Vector2(x+.125f,.84f);r.offsetMin=r.offsetMax=Vector2.zero;}
            Button close=Button("✕",header.transform,new Color(.55f,.10f,.12f,1f),Close);RectTransform xr=close.GetComponent<RectTransform>();xr.anchorMin=new Vector2(.93f,.16f);xr.anchorMax=new Vector2(.982f,.84f);xr.offsetMin=xr.offsetMax=Vector2.zero;
            GameObject scrollGo=UI("Scroll",panel.transform);RectTransform sr=scrollGo.GetComponent<RectTransform>();sr.anchorMin=new Vector2(.018f,.022f);sr.anchorMax=new Vector2(.982f,.895f);sr.offsetMin=sr.offsetMax=Vector2.zero;ScrollRect scroll=scrollGo.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.scrollSensitivity=44f;
            GameObject viewport=UI("Viewport",scrollGo.transform);Stretch(viewport.GetComponent<RectTransform>());Image vi=viewport.AddComponent<Image>();vi.color=new Color(.022f,.030f,.040f,.78f);Mask mask=viewport.AddComponent<Mask>();mask.showMaskGraphic=false;scroll.viewport=viewport.GetComponent<RectTransform>();
            GameObject body=UI("Content",viewport.transform);content=body.GetComponent<RectTransform>();content.anchorMin=new Vector2(0,1);content.anchorMax=new Vector2(1,1);content.pivot=new Vector2(.5f,1);content.sizeDelta=new Vector2(0,500);VerticalLayoutGroup vg=body.AddComponent<VerticalLayoutGroup>();vg.padding=new RectOffset(20,20,18,22);vg.spacing=11;vg.childControlHeight=true;vg.childForceExpandHeight=false;vg.childControlWidth=true;vg.childForceExpandWidth=true;ContentSizeFitter fitter=body.AddComponent<ContentSizeFitter>();fitter.verticalFit=ContentSizeFitter.FitMode.PreferredSize;scroll.content=content;
        }

        public void Refresh()
        {
            if(content==null)return;for(int i=content.childCount-1;i>=0;i--)Destroy(content.GetChild(i).gameObject);
            game=GameRuntime.Instance;MachineState m=game?.ActiveMachine;
            Heading(page==0?"POST / UEFI ENGINEERING":page==1?"THERMAL / POWER LAB":page==2?"OS / STORAGE LAB":"BENCHMARK / STABILITY LAB",m==null?"NO ACTIVE MACHINE":m.displayName+" · "+m.category+" · POST "+m.postCode);
            if(game==null||m==null){Card("No active device","Accept a desktop/upgrade/diagnostics contract first.",new Color(.95f,.35f,.30f,1f));return;}
            EnsureState(m);
            switch(page){case 0:RenderFirmware(m);break;case 1:RenderThermal(m);break;case 2:RenderOs(m);break;default:RenderBenchmark(m);break;}
        }

        private static void EnsureState(MachineState m)
        {
            if(m.osState==null)m.osState=new OsRuntimeState();if(m.benchmarkState==null)m.benchmarkState=new BenchmarkRunState();if(m.thermalState==null)m.thermalState=new ThermalRuntimeState();if(m.powerState==null)m.powerState=new PowerRuntimeState();if(m.maintenance==null)m.maintenance=new MaintenanceState();
        }

        private void RenderFirmware(MachineState m)
        {
            BiosState b=m.bios;
            Card("FIRMWARE STATE","Memory profile "+On(b.memoryProfileEnabled)+" · target "+b.memorySpeedOverride+" MT/s · DRAM "+b.memoryVoltage.ToString("0.00")+" V\nCPU limit "+b.cpuPowerLimitWatts+" W · multiplier "+Signed(b.cpuMultiplierOffset)+" · CPU V "+b.cpuVoltage.ToString("0.00")+" V\nSecure Boot "+On(b.secureBoot)+" · ReBAR "+On(b.resizableBar)+" · Above 4G "+On(b.above4G)+" · CSM "+On(b.csm)+"\nTraining: "+b.lastTrainingMessage,b.lastTrainingPassed?new Color(.20f,.82f,.48f,1f):Color.white);
            Row("POST / MEMORY",new[]{A("ENGINEERING POST",()=>game.EngineeringPost()),A("TRAIN MEMORY",()=>game.EngineeringTrainMemory()),A(b.memoryProfileEnabled?"DISABLE XMP/EXPO":"ENABLE XMP/EXPO",()=>{game.SetMemoryProfile(!b.memoryProfileEnabled);}),A("RESET BIOS",()=>game.EngineeringResetBios())});
            Row("DRAM VOLTAGE",new[]{A("−0.05 V",()=>game.EngineeringAdjustMemoryVoltage(-.05f)),A("−0.01 V",()=>game.EngineeringAdjustMemoryVoltage(-.01f)),A("+0.01 V",()=>game.EngineeringAdjustMemoryVoltage(.01f)),A("+0.05 V",()=>game.EngineeringAdjustMemoryVoltage(.05f))});
            Row("CPU TUNING",new[]{A("PL −10 W",()=>game.EngineeringAdjustCpuPower(-10)),A("PL +10 W",()=>game.EngineeringAdjustCpuPower(10)),A("MULT −1",()=>game.EngineeringAdjustMultiplier(-1)),A("MULT +1",()=>game.EngineeringAdjustMultiplier(1))});
            Row("BOOT / PCIe",new[]{A("SECURE BOOT",()=>game.EngineeringToggleSecureBoot()),A("RESIZABLE BAR",()=>game.EngineeringToggleRebar()),A("CYCLE FAN PROFILE",()=>game.CycleFanProfile())});
            History(m.history,"RECENT FIRMWARE / DEVICE LOG",10);
        }

        private void RenderThermal(MachineState m)
        {
            ThermalRuntimeState t=m.thermalState;PowerRuntimeState p=m.powerState;
            Card("THERMAL TELEMETRY","Ambient "+t.ambientC.ToString("0")+"°C · Case air "+t.caseAirC.ToString("0")+"°C · CPU "+m.cpuTempC.ToString("0")+"°C · GPU "+m.gpuTempC.ToString("0")+"°C\nVRM "+t.vrmC.ToString("0")+"°C · Storage "+t.storageC.ToString("0")+"°C · Coolant "+t.coolantC.ToString("0")+"°C\nAirflow "+t.airflowCfm.ToString("0")+" CFM · intake/exhaust "+t.intakeFans+"/"+t.exhaustFans+" · pressure "+Signed(t.pressureBalance)+"\nCPU throttle "+On(t.cpuThrottling)+" · GPU throttle "+On(t.gpuThrottling),(!t.cpuThrottling&&!t.gpuThrottling)?new Color(.20f,.82f,.48f,1f):new Color(.95f,.32f,.25f,1f));
            Card("POWER TELEMETRY","12V "+p.rail12V.ToString("0.00")+" V · 5V "+p.rail5V.ToString("0.00")+" V · 3.3V "+p.rail33V.ToString("0.00")+" V\nRipple "+p.rippleMv.ToString("0")+" mV · efficiency "+(p.efficiency*100).ToString("0")+"% · headroom "+p.headroomW.ToString("0")+" W\nTransient "+p.transientPeakW.ToString("0")+" W · OCP "+On(p.ocpTriggered)+" · stable "+On(p.stable),p.stable?new Color(.20f,.82f,.48f,1f):new Color(.95f,.32f,.25f,1f));
            Row("ANALYSIS",new[]{A("AIRFLOW",()=>game.EngineeringAirflow()),A("POWER RAILS",()=>game.EngineeringPower()),A("10 MIN SOAK",()=>game.EngineeringThermal(10)),A("30 MIN SOAK",()=>game.EngineeringThermal(30))});
            Card("MAINTENANCE","Service age "+m.maintenance.serviceAgeDays+" days · paste age "+m.maintenance.thermalPasteAgeDays+" days · filter dust "+Mathf.RoundToInt(m.maintenance.filterDust*100)+"% · corrosion "+Mathf.RoundToInt(m.maintenance.corrosion*100)+"%",Color.white);
            History(t.history,"THERMAL HISTORY",8);History(p.history,"POWER HISTORY",8);
        }

        private void RenderOs(MachineState m)
        {
            OsRuntimeState o=m.osState;
            Card("FORGEOS STATE","Installed "+On(m.osInstalled)+" · partitioned "+On(m.partitioned)+" · activated "+On(m.activated)+" · drivers "+On(m.driversInstalled)+"\nFilesystem "+o.filesystem+" · partitions "+o.partitionCount+" · free "+o.freeStorageGB+" GB · system files "+(o.systemFilesHealthy?"HEALTHY":"DAMAGED")+"\nDriver revision "+o.driverRevision+" · pending updates "+o.pendingUpdates+" · firewall "+On(o.firewallEnabled)+" · network stack "+On(o.networkStackReady)+"\nCrashes "+o.crashCount+(string.IsNullOrEmpty(o.lastCrashCode)?"":" · last "+o.lastCrashCode),o.systemFilesHealthy?new Color(.20f,.82f,.48f,1f):new Color(.95f,.32f,.25f,1f));
            Row("SOFTWARE",new[]{A("INSTALL FORGEOS",()=>game.InstallOS()),A("INTEGRITY SCAN",()=>game.EngineeringOsScan()),A("REPAIR SYSTEM",()=>game.EngineeringOsRepair()),A("UPDATE + DRIVERS",()=>game.EngineeringUpdateSoftware())});
            Row("STORAGE",new[]{A("SMART / ENDURANCE",()=>game.EngineeringSmart()),A("DIAGNOSTICS",()=>game.Diagnose())});
            History(o.history,"OS SERVICE LOG",10);History(m.history,"DEVICE HISTORY",8);
        }

        private void RenderBenchmark(MachineState m)
        {
            BenchmarkRunState b=m.benchmarkState;
            Color c=b.status==BenchmarkStatus.Passed?new Color(.20f,.82f,.48f,1f):b.status==BenchmarkStatus.Failed?new Color(.95f,.32f,.25f,1f):new Color(.95f,.68f,.22f,1f);
            Card("LATEST ENGINEERING RUN","Status "+b.status+" · total "+b.totalScore.ToString("0")+"\nCPU "+b.cpuScore.ToString("0")+" · GPU "+b.gpuScore.ToString("0")+" · Memory "+b.memoryScore.ToString("0")+" · Storage "+b.storageScore.ToString("0")+"\nStress "+b.stressMinutes+" min · memory errors "+b.memoryErrors+" · CPU peak "+b.peakCpuC.ToString("0")+"°C · GPU peak "+b.peakGpuC.ToString("0")+"°C\nPeak power "+b.peakPowerW.ToString("0")+" W · min 12V "+b.minimum12V.ToString("0.00")+" V · ripple "+b.maxRippleMv.ToString("0")+" mV\n"+(string.IsNullOrEmpty(b.lastResult)?"No run yet.":b.lastResult),c);
            Row("SUITE",new[]{A("15 MIN VALIDATION",()=>game.EngineeringBenchmark(15)),A("30 MIN STRESS",()=>game.EngineeringBenchmark(30)),A("MEMORY TRAIN",()=>game.EngineeringTrainMemory()),A("SMART",()=>game.EngineeringSmart())});
            History(b.history,"BENCHMARK HISTORY",10);
        }

        private struct Act{public string label;public Action action;public Act(string l,Action a){label=l;action=a;}}
        private static Act A(string l,Action a)=>new Act(l,a);
        private static string On(bool v)=>v?"ON":"OFF";
        private static string Signed(int v)=>(v>=0?"+":"")+v;
        private static string Signed(float v)=>v.ToString("+0.00;-0.00;0.00");

        private void History(List<string> lines,string title,int max){if(lines==null||lines.Count==0)return;Section(title);for(int i=lines.Count-1;i>=0&&i>=lines.Count-max;i--)Card(lines[i],"",new Color(.82f,.88f,.94f,1f));}
        private void Heading(string a,string b){Text t=Block(a,34,Color.white,58);t.fontStyle=FontStyle.Bold;Block(b,20,new Color(.55f,.65f,.73f,1f),42);}
        private void Section(string a){Text t=Block(a,22,new Color(.10f,.64f,.92f,1f),42);t.fontStyle=FontStyle.Bold;}
        private void Card(string title,string sub,Color c){GameObject g=UI("Card",content);g.AddComponent<Image>().color=new Color(.095f,.115f,.145f,1f);VerticalLayoutGroup v=g.AddComponent<VerticalLayoutGroup>();v.padding=new RectOffset(18,18,11,12);v.spacing=4;v.childControlHeight=true;v.childForceExpandHeight=false;LayoutElement le=g.AddComponent<LayoutElement>();le.minHeight=72;Text a=Text(title,g.transform,22,TextAnchor.MiddleLeft,c);a.fontStyle=FontStyle.Bold;LayoutElement al=a.gameObject.AddComponent<LayoutElement>();al.minHeight=30;if(!string.IsNullOrEmpty(sub)){Text s=Text(sub,g.transform,17,TextAnchor.UpperLeft,new Color(.66f,.73f,.80f,1f));s.horizontalOverflow=HorizontalWrapMode.Wrap;s.verticalOverflow=VerticalWrapMode.Overflow;LayoutElement sl=s.gameObject.AddComponent<LayoutElement>();sl.minHeight=34;}}
        private void Row(string title,Act[] actions){Section(title);GameObject g=UI("Actions",content);HorizontalLayoutGroup h=g.AddComponent<HorizontalLayoutGroup>();h.spacing=9;h.childControlHeight=true;h.childForceExpandHeight=true;h.childForceExpandWidth=true;LayoutElement le=g.AddComponent<LayoutElement>();le.preferredHeight=60;foreach(Act a in actions){Button b=Button(a.label,g.transform,new Color(.11f,.38f,.59f,1f),()=>{a.action?.Invoke();Refresh();});LayoutElement bl=b.gameObject.AddComponent<LayoutElement>();bl.flexibleWidth=1;}}
        private Text Block(string value,int size,Color c,float height){Text t=Text(value,content,size,TextAnchor.MiddleLeft,c);LayoutElement l=t.gameObject.AddComponent<LayoutElement>();l.preferredHeight=height;return t;}
        private Button Button(string label,Transform p,Color c,Action a){GameObject g=UI("Button_"+label,p);Image i=g.AddComponent<Image>();i.color=c;Button b=g.AddComponent<Button>();b.targetGraphic=i;if(a!=null)b.onClick.AddListener(()=>a());Text t=Text(label,g.transform,17,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,5);t.raycastTarget=false;return b;}
        private Text Text(string value,Transform p,int size,TextAnchor align,Color c){GameObject g=UI("Text",p);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=c;t.raycastTarget=false;return t;}
        private GameObject UI(string n,Transform p){GameObject g=new GameObject(n,typeof(RectTransform));g.transform.SetParent(p,false);return g;}
        private static void Stretch(RectTransform r,float i=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(i,i);r.offsetMax=new Vector2(-i,-i);}
        private static void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(e);}
    }
}
