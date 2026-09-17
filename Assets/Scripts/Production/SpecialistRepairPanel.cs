using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    public enum SpecialistPanelMode { Liquid, Board, Portable, Network }

    /// <summary>
    /// Mobile-first workstation UI for the non-desktop specialist workflows. It is
    /// intentionally separate from the main tablet so each physical station can open
    /// a focused process with explicit steps and live persisted measurements.
    /// </summary>
    public sealed class SpecialistRepairPanel : MonoBehaviour
    {
        private static SpecialistRepairPanel instance;
        private GameRuntime game;
        private Canvas canvas;
        private RectTransform content;
        private GameObject root;
        private Font font;
        private SpecialistPanelMode mode;

        public static void Open(SpecialistPanelMode requested)
        {
            if (instance == null)
            {
                GameObject host = new GameObject("ForgeBench_SpecialistRepairPanel");
                DontDestroyOnLoad(host);
                instance = host.AddComponent<SpecialistRepairPanel>();
            }
            instance.Show(requested);
        }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            game = GameRuntime.Instance;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureEventSystem();
            Build();
            if (root != null) root.SetActive(false);
        }

        private void Update()
        {
            if (root != null && root.activeSelf && Input.GetKeyDown(KeyCode.Escape) && !Application.isMobilePlatform) Close();
        }

        private void Show(SpecialistPanelMode requested)
        {
            game = GameRuntime.Instance;
            if (root == null) Build();
            mode = requested;
            if (root != null) root.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            MobileInputState.Move = Vector2.zero;
            Refresh();
        }

        public void Close()
        {
            if (root != null) root.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Build()
        {
            if (root != null) return;
            root = new GameObject("SpecialistPanelRoot", typeof(RectTransform));
            root.transform.SetParent(transform, false);
            canvas = root.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 25000;
            CanvasScaler scaler = root.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920,1080); scaler.matchWidthOrHeight = .5f;
            root.AddComponent<GraphicRaycaster>();

            GameObject bg = UI("Background", root.transform); Stretch(bg.GetComponent<RectTransform>()); bg.AddComponent<Image>().color = new Color(.018f,.025f,.035f,.985f);
            GameObject card = UI("Workstation", bg.transform); RectTransform cr=card.GetComponent<RectTransform>();cr.anchorMin=new Vector2(.06f,.06f);cr.anchorMax=new Vector2(.94f,.94f);cr.offsetMin=cr.offsetMax=Vector2.zero;card.AddComponent<Image>().color=new Color(.055f,.066f,.082f,1f);

            GameObject header=UI("Header",card.transform);RectTransform hr=header.GetComponent<RectTransform>();hr.anchorMin=new Vector2(0,1);hr.anchorMax=new Vector2(1,1);hr.pivot=new Vector2(.5f,1);hr.sizeDelta=new Vector2(0,84);hr.anchoredPosition=Vector2.zero;header.AddComponent<Image>().color=new Color(.075f,.09f,.11f,1f);
            Text title=Text("SPECIALIST WORKSTATION",header.transform,30,TextAnchor.MiddleLeft,Color.white);RectTransform tr=title.rectTransform;tr.anchorMin=new Vector2(.025f,0);tr.anchorMax=new Vector2(.45f,1);tr.offsetMin=tr.offsetMax=Vector2.zero;
            string[] names={"LIQUID LOOP","BOARD REPAIR","PORTABLE","NETWORK / NAS"};
            for(int i=0;i<4;i++)
            {
                int capture=i;Button b=Button(names[i],header.transform,new Color(.12f,.15f,.19f,1f),()=>{mode=(SpecialistPanelMode)capture;Refresh();});RectTransform r=b.GetComponent<RectTransform>();float x=.45f+i*.115f;r.anchorMin=new Vector2(x,.16f);r.anchorMax=new Vector2(x+.108f,.84f);r.offsetMin=r.offsetMax=Vector2.zero;
            }
            Button close=Button("✕",header.transform,new Color(.55f,.12f,.12f,1f),Close);RectTransform xr=close.GetComponent<RectTransform>();xr.anchorMin=new Vector2(.925f,.16f);xr.anchorMax=new Vector2(.982f,.84f);xr.offsetMin=xr.offsetMax=Vector2.zero;

            GameObject scrollGo=UI("Scroll",card.transform);RectTransform sr=scrollGo.GetComponent<RectTransform>();sr.anchorMin=new Vector2(.025f,.03f);sr.anchorMax=new Vector2(.975f,.89f);sr.offsetMin=sr.offsetMax=Vector2.zero;ScrollRect scroll=scrollGo.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.scrollSensitivity=42f;
            GameObject viewport=UI("Viewport",scrollGo.transform);Stretch(viewport.GetComponent<RectTransform>());Image vi=viewport.AddComponent<Image>();vi.color=new Color(.025f,.032f,.043f,.8f);Mask mask=viewport.AddComponent<Mask>();mask.showMaskGraphic=false;scroll.viewport=viewport.GetComponent<RectTransform>();
            GameObject body=UI("Content",viewport.transform);content=body.GetComponent<RectTransform>();content.anchorMin=new Vector2(0,1);content.anchorMax=new Vector2(1,1);content.pivot=new Vector2(.5f,1);content.sizeDelta=new Vector2(0,500);VerticalLayoutGroup vg=body.AddComponent<VerticalLayoutGroup>();vg.padding=new RectOffset(22,22,20,22);vg.spacing=12;vg.childControlHeight=true;vg.childForceExpandHeight=false;vg.childControlWidth=true;vg.childForceExpandWidth=true;ContentSizeFitter fit=body.AddComponent<ContentSizeFitter>();fit.verticalFit=ContentSizeFitter.FitMode.PreferredSize;scroll.content=content;
        }

        public void Refresh()
        {
            if (content == null) return;
            for(int i=content.childCount-1;i>=0;i--) Destroy(content.GetChild(i).gameObject);
            game=GameRuntime.Instance;
            MachineState m=game?.ActiveMachine;
            string device=m==null?"NO ACTIVE DEVICE":m.displayName+" · "+m.category;
            Heading(mode==SpecialistPanelMode.Liquid?"CUSTOM LIQUID COOLING":mode==SpecialistPanelMode.Board?"MICROSOLDERING / BOARD REPAIR":mode==SpecialistPanelMode.Portable?"PORTABLE ELECTRONICS":"NETWORK / NAS / SERVER",device);
            if(game==null||m==null){Card("No active job","Accept a contract before using this workstation.",new Color(.92f,.35f,.30f,1f));return;}
            switch(mode){case SpecialistPanelMode.Liquid:RenderLiquid(m);break;case SpecialistPanelMode.Board:RenderBoard(m);break;case SpecialistPanelMode.Portable:RenderPortable(m);break;default:RenderNetwork(m);break;}
        }

        private void RenderLiquid(MachineState m)
        {
            LiquidLoopState s=m.liquidLoop??new LiquidLoopState();
            string status="Pump "+On(s.pumpInstalled)+" · Reservoir "+On(s.reservoirInstalled)+" · Radiator "+s.radiatorMm+" mm\nFittings "+s.tightFittings+"/"+s.fittingCount+" tight · Coolant "+s.coolantLitres.ToString("0.00")+" L · Air "+Mathf.RoundToInt(s.airFraction*100)+"%\nFlow "+s.flowLpm.ToString("0.00")+" L/min · Pressure "+s.pressureKpa.ToString("0")+" kPa · Leak test "+(s.leakTestPassed?"PASS":s.leakDetected?"FAIL":"NOT PASSED");
            Card("LOOP STATE",status,s.leakTestPassed?new Color(.20f,.80f,.48f,1f):Color.white);
            Row("HARDWARE",new[]{A("INSTALL PUMP",()=>game.LiquidInstallPump()),A("INSTALL RESERVOIR",()=>game.LiquidInstallReservoir()),A("240 mm RAD",()=>game.LiquidInstallRadiator(240)),A("360 mm RAD",()=>game.LiquidInstallRadiator(360))});
            Row("TUBING / FITTINGS",new[]{A("ADD 2 FITTINGS",()=>game.LiquidAddFittings()),A("TIGHTEN NEXT",()=>game.LiquidTightenFitting()),A("ADD 0.25 L",()=>game.LiquidFill()),A("BLEED LOOP",()=>game.LiquidBleed())});
            Wide("RUN ISOLATED LEAK TEST",()=>game.LiquidLeakTest(),new Color(.08f,.55f,.78f,1f));
            History(s.history);
        }

        private void RenderBoard(MachineState m)
        {
            BoardRepairState s=m.boardRepair??new BoardRepairState();
            string status="ESD "+On(s.esdGrounded)+" · Microscope "+On(s.microscopeInspected)+" · Rail measured "+On(s.powerRailMeasured)+"\nShort localized "+On(s.shortLocated)+" · Flux "+On(s.fluxApplied)+" · Rework cycles "+s.reworkCycles+"\nPads "+(s.padsIntact?"INTACT":"DAMAGED")+" · Joint quality "+Mathf.RoundToInt(s.solderQuality*100)+"% · Verified "+On(s.repaired)+"\n"+(string.IsNullOrEmpty(s.lastMeasurement)?"No measurement":s.lastMeasurement);
            Card("BOARD STATE",status,s.repaired?new Color(.20f,.80f,.48f,1f):s.padsIntact?Color.white:new Color(.95f,.28f,.25f,1f));
            if(game.State.workshop.boardRepairLevel<=0)Card("STATION LOCKED","Upgrade workshop to level 3 before powered board repair.",new Color(.95f,.58f,.20f,1f));
            Row("SAFE DIAGNOSTICS",new[]{A("GROUND ESD",()=>game.BoardGroundEsd()),A("MICROSCOPE",()=>game.BoardInspect()),A("MEASURE RAIL",()=>game.BoardMeasure()),A("LOCATE SHORT",()=>game.BoardLocateShort())});
            Row("REWORK",new[]{A("APPLY FLUX",()=>game.BoardApplyFlux()),A("HOT-AIR REWORK",()=>game.BoardRework()),A("VERIFY REPAIR",()=>game.BoardVerify())});
            History(s.history);
        }

        private void RenderPortable(MachineState m)
        {
            PortableDeviceState s=m.portable??new PortableDeviceState();
            string status="Device "+m.category+" · Screws remaining "+s.screwsRemaining+" · Cover removed "+On(s.backCoverRemoved)+"\nBattery isolated "+On(s.batteryDisconnected)+" · Display separated "+On(s.displaySeparated)+" · Sealed "+On(s.@sealed)+"\nBattery health "+Mathf.RoundToInt(s.batteryHealth*100)+"% · Display "+Mathf.RoundToInt(s.displayHealth*100)+"% · Port "+Mathf.RoundToInt(s.chargingPortHealth*100)+"%\nSeal quality "+Mathf.RoundToInt(s.sealQuality*100)+"% · Water damage "+Mathf.RoundToInt(s.waterDamage*100)+"% · Controller drift "+s.controllerDrift.ToString("0.00");
            Card("PORTABLE DEVICE STATE",status,Color.white);
            Row("DISASSEMBLY",new[]{A("REMOVE SCREW",()=>game.PortableRemoveScrew()),A("REMOVE COVER",()=>game.PortableRemoveCover()),A("DISCONNECT BATTERY",()=>game.PortableDisconnectBattery()),A("SEPARATE DISPLAY",()=>game.PortableSeparateDisplay())});
            Row("SERVICE",new[]{A("REPLACE BATTERY",()=>game.PortableReplaceBattery()),A("REPLACE DISPLAY",()=>game.PortableReplaceDisplay()),A("SERVICE PORT",()=>game.PortableServicePort()),A("CALIBRATE STICKS",()=>game.PortableCalibrate())});
            Wide("REASSEMBLE & RESEAL",()=>game.PortableReseal(),new Color(.10f,.57f,.38f,1f));
            History(s.history);
        }

        private void RenderNetwork(MachineState m)
        {
            NetworkLabState s=m.network??new NetworkLabState();
            string status="Link "+On(s.linkUp)+" · "+(s.dhcp?"DHCP":"STATIC")+" · "+s.ipAddress+" / "+s.subnetMask+"\nThroughput "+s.throughputMbps.ToString("0")+" Mbps · Loss "+(s.packetLoss*100).ToString("0.0")+"% · Latency "+s.latencyMs+" ms\nRAID "+s.raidLevel+" · Disks "+s.disksHealthy+"/"+s.disksTotal+" healthy · Degraded "+On(s.arrayDegraded)+" · Scrub "+On(s.scrubComplete);
            Card("NETWORK LAB STATE",status,!s.arrayDegraded&&s.scrubComplete?new Color(.20f,.80f,.48f,1f):Color.white);
            Row("LINK / IP",new[]{A("CONNECT LINK",()=>game.NetworkConnect()),A("DHCP",()=>game.NetworkDhcp()),A("STATIC .50",()=>game.NetworkStatic()),A("THROUGHPUT TEST",()=>game.NetworkTest())});
            Row("STORAGE ARRAY",new[]{A("RAID 1",()=>game.NetworkRaid(1)),A("RAID 5",()=>game.NetworkRaid(5)),A("REPLACE DISK",()=>game.NetworkReplaceDisk()),A("REBUILD / SCRUB",()=>game.NetworkScrub())});
            History(s.history);
        }

        private struct WorkAction{public string label;public Action action;public WorkAction(string l,Action a){label=l;action=a;}}
        private static WorkAction A(string text,Action action)=>new WorkAction(text,action);
        private static string On(bool v)=>v?"YES":"NO";

        private void History(List<string> lines)
        {
            if(lines==null||lines.Count==0)return;Section("WORK LOG");for(int i=lines.Count-1;i>=0&&i>=lines.Count-10;i--)Card(lines[i],"",new Color(.82f,.88f,.94f,1f));
        }
        private void Heading(string a,string b){Text t=Block(a,34,Color.white,58);t.fontStyle=FontStyle.Bold;Block(b,20,new Color(.55f,.65f,.73f,1f),42);}
        private void Section(string s){Text t=Block(s,22,new Color(.10f,.64f,.92f,1f),42);t.fontStyle=FontStyle.Bold;}
        private void Card(string title,string sub,Color c){GameObject g=UI("Card",content);g.AddComponent<Image>().color=new Color(.10f,.12f,.15f,1f);VerticalLayoutGroup v=g.AddComponent<VerticalLayoutGroup>();v.padding=new RectOffset(18,18,12,12);v.spacing=5;v.childControlHeight=true;v.childForceExpandHeight=false;LayoutElement le=g.AddComponent<LayoutElement>();le.minHeight=70;Text a=Text(title,g.transform,22,TextAnchor.MiddleLeft,c);a.fontStyle=FontStyle.Bold;LayoutElement la=a.gameObject.AddComponent<LayoutElement>();la.minHeight=30;if(!string.IsNullOrEmpty(sub)){Text b=Text(sub,g.transform,18,TextAnchor.UpperLeft,new Color(.66f,.72f,.78f,1f));b.horizontalOverflow=HorizontalWrapMode.Wrap;b.verticalOverflow=VerticalWrapMode.Overflow;LayoutElement lb=b.gameObject.AddComponent<LayoutElement>();lb.minHeight=36;}}
        private void Row(string name,WorkAction[] actions){Section(name);GameObject g=UI("ActionRow",content);HorizontalLayoutGroup h=g.AddComponent<HorizontalLayoutGroup>();h.spacing=10;h.childControlHeight=true;h.childForceExpandHeight=true;h.childForceExpandWidth=true;LayoutElement gl=g.AddComponent<LayoutElement>();gl.preferredHeight=62;foreach(WorkAction a in actions){Button b=Button(a.label,g.transform,new Color(.12f,.38f,.57f,1f),()=>{a.action?.Invoke();Refresh();});LayoutElement bl=b.gameObject.AddComponent<LayoutElement>();bl.flexibleWidth=1;}}
        private void Wide(string label,Action action,Color color){Button b=Button(label,content,color,()=>{action?.Invoke();Refresh();});LayoutElement le=b.gameObject.AddComponent<LayoutElement>();le.preferredHeight=60;}
        private Text Block(string text,int size,Color c,float height){Text t=Text(text,content,size,TextAnchor.MiddleLeft,c);LayoutElement l=t.gameObject.AddComponent<LayoutElement>();l.preferredHeight=height;return t;}
        private Button Button(string label,Transform p,Color color,Action action){GameObject g=UI("Button_"+label,p);Image i=g.AddComponent<Image>();i.color=color;Button b=g.AddComponent<Button>();b.targetGraphic=i;if(action!=null)b.onClick.AddListener(()=>action());Text t=Text(label,g.transform,18,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,6);t.raycastTarget=false;return b;}
        private Text Text(string value,Transform p,int size,TextAnchor align,Color color){GameObject g=UI("Text",p);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=color;t.raycastTarget=false;return t;}
        private GameObject UI(string name,Transform p){GameObject g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(p,false);return g;}
        private static void Stretch(RectTransform r,float inset=0f){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(inset,inset);r.offsetMax=new Vector2(-inset,-inset);}
        private static void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(e);}
    }
}
