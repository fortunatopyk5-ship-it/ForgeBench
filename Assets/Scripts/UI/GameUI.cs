using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    public sealed class GameUI : MonoBehaviour
    {
        private GameRuntime game;
        private Canvas canvas;
        private RectTransform safeRoot;
        private Font font;
        private LocalizationService loc = new LocalizationService();
        private RectTransform tablet;
        private RectTransform body;
        private Text hudText;
        private Text promptText;
        private Text toastText;
        private GameObject toastPanel;
        private float toastUntil;
        private string currentTab = "JOBS";
        private PartCategory storeCategory = PartCategory.Case;
        private readonly Dictionary<string, Button> tabButtons = new Dictionary<string, Button>();
        private bool built;

        private readonly Color bg = new Color(.055f,.065f,.08f,.96f);
        private readonly Color panel = new Color(.09f,.105f,.13f,.98f);
        private readonly Color card = new Color(.13f,.15f,.18f,.96f);
        private readonly Color accent = new Color(.08f,.58f,.82f,1f);
        private readonly Color good = new Color(.14f,.72f,.42f,1f);
        private readonly Color bad = new Color(.9f,.25f,.24f,1f);
        private readonly Color muted = new Color(.55f,.62f,.68f,1f);

        public void Build()
        {
            if (built) return;
            built = true; game = GameRuntime.Instance;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureEventSystem();
            GameObject c = new GameObject("ForgeBenchCanvas"); canvas = c.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 20;
            CanvasScaler scaler = c.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920,1080); scaler.matchWidthOrHeight=.5f;
            c.AddComponent<GraphicRaycaster>(); DontDestroyOnLoad(c);
            GameObject safe=UIObject("SafeArea",canvas.transform);safeRoot=safe.GetComponent<RectTransform>();Stretch(safeRoot,0);safe.AddComponent<SafeAreaFitter>();
            BuildLookPad(); BuildHud(); BuildTablet(); BuildMobileControls();
            Refresh();
        }

        private void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null) return;
            GameObject es = new GameObject("EventSystem"); es.AddComponent<EventSystem>(); es.AddComponent<StandaloneInputModule>(); DontDestroyOnLoad(es);
        }

        private void BuildLookPad()
        {
            GameObject go = UIObject("LookPad", safeRoot); RectTransform rt=go.GetComponent<RectTransform>();
            rt.anchorMin=new Vector2(.46f,0); rt.anchorMax=new Vector2(1,1); rt.offsetMin=rt.offsetMax=Vector2.zero;
            Image img=go.AddComponent<Image>(); img.color=new Color(0,0,0,0); img.raycastTarget=true;
            go.AddComponent<TouchLookPad>();
        }

        private void BuildHud()
        {
            GameObject top=UIObject("HUDTop",safeRoot); RectTransform r=top.GetComponent<RectTransform>(); r.anchorMin=new Vector2(0,1);r.anchorMax=new Vector2(1,1);r.pivot=new Vector2(.5f,1);r.sizeDelta=new Vector2(0,74);r.anchoredPosition=Vector2.zero;
            Image i=top.AddComponent<Image>();i.color=new Color(.03f,.04f,.05f,.90f);
            hudText=AddText(top.transform,"HUD",26,TextAnchor.MiddleLeft,Color.white); RectTransform hr=hudText.rectTransform;hr.anchorMin=new Vector2(0,0);hr.anchorMax=new Vector2(.7f,1);hr.offsetMin=new Vector2(26,0);hr.offsetMax=Vector2.zero;
            Button tab=AddButton(top.transform,"Tablet",()=>ToggleTablet(true),accent,24); RectTransform tr=tab.GetComponent<RectTransform>();tr.anchorMin=new Vector2(.82f,.12f);tr.anchorMax=new Vector2(.93f,.88f);tr.offsetMin=tr.offsetMax=Vector2.zero;
            Button action=AddButton(top.transform,"Interact",()=>game.World?.InteractFocused(),new Color(.25f,.3f,.35f,1),22); RectTransform ar=action.GetComponent<RectTransform>();ar.anchorMin=new Vector2(.94f,.12f);ar.anchorMax=new Vector2(.995f,.88f);ar.offsetMin=ar.offsetMax=Vector2.zero;

            promptText=AddText(safeRoot,"",24,TextAnchor.MiddleCenter,Color.white);RectTransform pr=promptText.rectTransform;pr.anchorMin=new Vector2(.28f,.12f);pr.anchorMax=new Vector2(.72f,.20f);pr.offsetMin=pr.offsetMax=Vector2.zero;
            Outline pbg=promptText.gameObject.AddComponent<Outline>(); pbg.effectColor=new Color(0,0,0,.85f);pbg.effectDistance=new Vector2(2,-2);

            toastPanel=UIObject("Toast",safeRoot);RectTransform tor=toastPanel.GetComponent<RectTransform>();tor.anchorMin=new Vector2(.30f,.76f);tor.anchorMax=new Vector2(.70f,.85f);tor.offsetMin=tor.offsetMax=Vector2.zero;Image ti=toastPanel.AddComponent<Image>();ti.color=panel;
            toastText=AddText(toastPanel.transform,"",24,TextAnchor.MiddleCenter,Color.white);Stretch(toastText.rectTransform,14);toastPanel.SetActive(false);
        }

        private void BuildMobileControls()
        {
            GameObject joy=UIObject("MoveJoystick",safeRoot);RectTransform jr=joy.GetComponent<RectTransform>();jr.anchorMin=new Vector2(.025f,.045f);jr.anchorMax=new Vector2(.17f,.30f);jr.offsetMin=jr.offsetMax=Vector2.zero;Image ji=joy.AddComponent<Image>();ji.color=new Color(.1f,.12f,.14f,.55f);ji.raycastTarget=true;
            GameObject knob=UIObject("Knob",joy.transform);RectTransform kr=knob.GetComponent<RectTransform>();kr.anchorMin=new Vector2(.28f,.28f);kr.anchorMax=new Vector2(.72f,.72f);kr.offsetMin=kr.offsetMax=Vector2.zero;Image ki=knob.AddComponent<Image>();ki.color=new Color(.65f,.75f,.82f,.65f);ki.raycastTarget=false;
            TouchJoystick tj=joy.AddComponent<TouchJoystick>();tj.knob=kr;
            Button precision=AddButton(safeRoot,"PRECISION",null,new Color(.16f,.18f,.21f,.65f),19);RectTransform rr=precision.GetComponent<RectTransform>();rr.anchorMin=new Vector2(.80f,.05f);rr.anchorMax=new Vector2(.90f,.115f);rr.offsetMin=rr.offsetMax=Vector2.zero;HoldPrecision hp=precision.gameObject.AddComponent<HoldPrecision>();
            if(!Application.isMobilePlatform){joy.SetActive(false);precision.gameObject.SetActive(false);}
        }

        private void BuildTablet()
        {
            tablet=UIObject("Tablet",safeRoot).GetComponent<RectTransform>();tablet.anchorMin=new Vector2(.035f,.06f);tablet.anchorMax=new Vector2(.965f,.94f);tablet.offsetMin=tablet.offsetMax=Vector2.zero;Image bgImg=tablet.gameObject.AddComponent<Image>();bgImg.color=bg;
            GameObject header=UIObject("Header",tablet);RectTransform h=header.GetComponent<RectTransform>();h.anchorMin=new Vector2(0,1);h.anchorMax=new Vector2(1,1);h.pivot=new Vector2(.5f,1);h.sizeDelta=new Vector2(0,72);h.anchoredPosition=Vector2.zero;Image hi=header.AddComponent<Image>();hi.color=new Color(.07f,.085f,.10f,1);
            Text title=AddText(header.transform,"FORGEBENCH",28,TextAnchor.MiddleLeft,Color.white);RectTransform tir=title.rectTransform;tir.anchorMin=new Vector2(.015f,0);tir.anchorMax=new Vector2(.18f,1);tir.offsetMin=tir.offsetMax=Vector2.zero;
            Button close=AddButton(header.transform,"✕",()=>ToggleTablet(false),bad,28);RectTransform cr=close.GetComponent<RectTransform>();cr.anchorMin=new Vector2(.955f,.15f);cr.anchorMax=new Vector2(.992f,.85f);cr.offsetMin=cr.offsetMax=Vector2.zero;
            string[] tabs={"JOBS","STORE","INVENTORY","BENCH","BIOS","OS","DIAG","PROGRESS","SETTINGS"};
            for(int x=0;x<tabs.Length;x++)
            {
                string id=tabs[x];float a=.18f+x*.084f;
                Button b=AddButton(header.transform,id,()=>OpenTab(id),new Color(.11f,.13f,.16f,1),18);RectTransform br=b.GetComponent<RectTransform>();br.anchorMin=new Vector2(a,.15f);br.anchorMax=new Vector2(a+.078f,.85f);br.offsetMin=br.offsetMax=Vector2.zero;tabButtons[id]=b;
            }
            GameObject scrollGo=UIObject("Scroll",tablet);RectTransform sr=scrollGo.GetComponent<RectTransform>();sr.anchorMin=new Vector2(.02f,.025f);sr.anchorMax=new Vector2(.98f,.895f);sr.offsetMin=sr.offsetMax=Vector2.zero;ScrollRect scroll=scrollGo.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.scrollSensitivity=38f;
            GameObject viewport=UIObject("Viewport",scrollGo.transform);RectTransform vr=viewport.GetComponent<RectTransform>();Stretch(vr,0);Image vi=viewport.AddComponent<Image>();vi.color=new Color(.03f,.04f,.05f,.35f);Mask mask=viewport.AddComponent<Mask>();mask.showMaskGraphic=false;scroll.viewport=vr;
            GameObject content=UIObject("Content",viewport.transform);body=content.GetComponent<RectTransform>();body.anchorMin=new Vector2(0,1);body.anchorMax=new Vector2(1,1);body.pivot=new Vector2(.5f,1);body.anchoredPosition=Vector2.zero;body.sizeDelta=new Vector2(0,500);
            VerticalLayoutGroup v=content.AddComponent<VerticalLayoutGroup>();v.padding=new RectOffset(18,18,18,18);v.spacing=12;v.childControlHeight=true;v.childForceExpandHeight=false;v.childControlWidth=true;v.childForceExpandWidth=true;
            ContentSizeFitter fit=content.AddComponent<ContentSizeFitter>();fit.verticalFit=ContentSizeFitter.FitMode.PreferredSize;scroll.content=body;
            tablet.gameObject.SetActive(false);
        }

        public void ToggleTablet(bool show)
        {
            if(tablet==null)return;tablet.gameObject.SetActive(show);Cursor.lockState=show?CursorLockMode.None:CursorLockMode.Locked;MobileInputState.Move=Vector2.zero;Refresh();
        }
        public void OpenTab(string id){currentTab=id;ToggleTablet(true);RenderTab();}
        public void SetWorldPrompt(string text){if(promptText!=null)promptText.text=string.IsNullOrEmpty(text)?"":("[E] "+text);}

        public void ShowToast(string text,bool success=true)
        {
            if(toastPanel==null)return;toastPanel.SetActive(true);toastText.text=text;toastText.color=success?Color.white:new Color(1f,.72f,.72f);toastUntil=Time.unscaledTime+4.5f;
        }

        private void Update(){if(toastPanel!=null&&toastPanel.activeSelf&&Time.unscaledTime>toastUntil)toastPanel.SetActive(false);}

        public void Refresh()
        {
            if(!built||game==null||game.State==null)return;
            loc.Load(game.State.settings.language);
            JobState j=game.ActiveJob;MachineState m=game.ActiveMachine;
            hudText.text="FORGEBENCH   DAY "+game.State.day+"   $"+game.State.money.ToString("0")+"   REP "+game.State.reputation+(j==null?"   NO ACTIVE JOB":"   "+j.jobId+" · "+j.title+(m==null?"":" · POST "+m.postCode));
            if(tablet.gameObject.activeSelf)RenderTab();
        }

        private void RenderTab()
        {
            if(body==null)return;ClearBody();
            foreach(var kv in tabButtons)SetButtonColor(kv.Value,kv.Key==currentTab?accent:new Color(.11f,.13f,.16f,1));
            switch(currentTab)
            {
                case "JOBS":RenderJobs();break;case "STORE":RenderStore();break;case "INVENTORY":RenderInventory();break;case "BENCH":RenderBench();break;case "BIOS":RenderBios();break;case "OS":RenderOS();break;case "DIAG":RenderDiag();break;case "PROGRESS":RenderProgress();break;default:RenderSettings();break;
            }
        }

        private void RenderJobs()
        {
            Heading("JOB BOARD", "Contracts are data-driven and validated centrally before submission.");
            JobState active=game.ActiveJob;
            if(active!=null)
            {
                CardText(active.jobId+" · "+active.customerName+" · "+active.title,"Stage: "+active.stage+"\n"+active.description+"\nReward: $"+active.reward.ToString("0")+" · Required benchmark: "+active.targetBenchmark+" · Max noise optional: "+active.maxNoiseDb.ToString("0")+" dB");
                WideButton(loc.T("jobs.submit","VALIDATE & SUBMIT"),()=>game.ValidateAndSubmit(),good);
            }
            else CardText(loc.T("jobs.none","No active job."),"Accept one contract. Only one customer device may occupy the main workflow at a time.");
            Section("AVAILABLE CONTRACTS");
            foreach(JobState j in game.State.jobs.Where(x=>x.stage==JobStage.Offered).Take(8))
            {
                JobState captured=j;
                Row(j.jobId+"  "+j.title+"  ·  "+j.customerName,"$"+j.reward.ToString("0")+" · due day "+j.dueDay+" · target "+j.targetBenchmark,"ACCEPT",()=>game.AcceptJob(captured.jobId));
            }
        }

        private void RenderStore()
        {
            Heading("HARDWARE STORE","Orders create persistent shipment entities. Standard delivery arrives next day; receive it in the workshop.");
            GameObject cats=Horizontal("Categories",58);
            PartCategory[] essentials={PartCategory.Case,PartCategory.Motherboard,PartCategory.CPU,PartCategory.RAM,PartCategory.GPU,PartCategory.Storage,PartCategory.PSU,PartCategory.Cooler,PartCategory.Fan,PartCategory.Consumable,PartCategory.Tool};
            foreach(PartCategory cat in essentials){PartCategory captured=cat;Button b=AddButton(cats.transform,cat.ToString(),()=>{storeCategory=captured;RenderTab();},cat==storeCategory?accent:new Color(.16f,.18f,.21f,1),16);LayoutElement le=b.gameObject.AddComponent<LayoutElement>();le.preferredWidth=130;le.preferredHeight=48;}
            GameObject actions=Horizontal("ShipActions",62);AddFlexButton(actions.transform,loc.T("store.receive","RECEIVE DELIVERIES"),()=>game.ReceiveAll(),good);AddFlexButton(actions.transform,loc.T("store.nextday","ADVANCE DAY"),()=>game.AdvanceDay(),new Color(.28f,.32f,.38f,1));
            foreach(ShipmentState s in game.State.shipments.Where(s=>s.status==ShipmentStatus.InTransit||s.status==ShipmentStatus.Delivered).Take(6))
                CardText("Shipment "+s.shipmentId+" · "+s.status,"Delivery day "+s.deliveryDay+(s.delayed?" · supplier delay":"")+" · "+string.Join(", ",s.lines.Select(l=>game.Catalog.Get(l.definitionId)?.model??l.definitionId)));
            Section(storeCategory.ToString().ToUpperInvariant());
            foreach(HardwareDefinition p in game.Catalog.ByCategory(storeCategory).Take(18))
            {
                HardwareDefinition captured=p;string spec=Spec(p);
                Row(p.brand+" "+p.model,spec+" · $"+p.price.ToString("0"),"BUY",()=>game.Buy(captured.id));
            }
        }

        private void RenderInventory()
        {
            Heading("INVENTORY & WAREHOUSE","Every component is a unique instance with condition, reservation, customer ownership, fault and persistence state.");
            if(game.State.inventory.Count==0)CardText("Warehouse empty","Buy hardware from the store and receive the shipment.");
            foreach(ItemInstance item in game.State.inventory.Take(80))
            {
                HardwareDefinition d=game.Inventory.Def(item);if(d==null)continue;ItemInstance captured=item;
                string status=item.reserved?"INSTALLED / RESERVED":"AVAILABLE";
                Row(item.instanceId+" · "+d.brand+" "+d.model,status+" · condition "+(item.condition*100).ToString("0")+"% · "+d.category,item.reserved?"REMOVE":"INSTALL",()=>{if(captured.reserved)game.Remove(captured.instanceId);else game.Install(captured.instanceId);});
            }
        }

        private void RenderBench()
        {
            MachineState m=game.ActiveMachine;
            Heading("MAIN ASSEMBLY BENCH","Physical machine state drives compatibility, power, thermals, POST and job validation.");
            if(m==null){CardText("Bench is empty","Accept a job first.");return;}
            CardText(m.displayName,"POST: "+m.postCode+" · Boot: "+m.bootState+" · Benchmark: "+m.benchmarkScore+" · Dust: "+(m.dust*100).ToString("0")+"%\nCPU "+m.cpuTempC.ToString("0")+"°C · GPU "+m.gpuTempC.ToString("0")+"°C · Power "+m.systemPowerW.ToString("0")+"W · Noise "+m.noiseDb.ToString("0")+" dB");
            List<string> issues=game.Compatibility.ExplainSystem(m);if(issues.Count>0)CardText("Compatibility blockers",string.Join("\n",issues),bad);
            SlotRow("CASE",m.caseItemId,PartCategory.Case);SlotRow("MOTHERBOARD",m.motherboardItemId,PartCategory.Motherboard);SlotRow("CPU",m.cpuItemId,PartCategory.CPU);SlotRow("RAM",string.Join(",",m.ramItemIds),PartCategory.RAM);SlotRow("GPU",m.gpuItemId,PartCategory.GPU);SlotRow("STORAGE",string.Join(",",m.storageItemIds),PartCategory.Storage);SlotRow("PSU",m.psuItemId,PartCategory.PSU);SlotRow("COOLER",m.coolerItemId,PartCategory.Cooler);SlotRow("FANS",string.Join(",",m.fanItemIds),PartCategory.Fan);
            GameObject actions=Horizontal("BenchActions1",64);AddFlexButton(actions.transform,loc.T("bench.paste","APPLY THERMAL PASTE"),()=>game.ApplyThermalPaste(),new Color(.32f,.28f,.18f,1));AddFlexButton(actions.transform,loc.T("bench.connect","CONNECT CABLES"),()=>game.ConnectCables(),accent);AddFlexButton(actions.transform,loc.T("bench.power","POWER ON / POST"),()=>game.PowerOn(),good);
            GameObject actions2=Horizontal("BenchActions2",64);AddFlexButton(actions2.transform,loc.T("bench.clean","CLEAN DEVICE"),()=>game.CleanDevice(),new Color(.30f,.35f,.38f,1));AddFlexButton(actions2.transform,"DIAGNOSE",()=>game.Diagnose(),new Color(.30f,.35f,.38f,1));AddFlexButton(actions2.transform,"BENCHMARK",()=>game.RunBenchmark(),new Color(.30f,.35f,.38f,1));
            CardText("CABLE GRAPH","ATX24 "+OnOff(m.cables.atx24)+" · EPS "+OnOff(m.cables.cpuEps)+" · GPU "+OnOff(m.cables.gpuPower)+" · SATA DATA "+OnOff(m.cables.sataData)+" · SATA POWER "+OnOff(m.cables.sataPower)+" · CPU FAN "+OnOff(m.cables.cpuFan)+" · FRONT PANEL "+OnOff(m.cables.frontPanel));
            game.Assembly.EnsureCaseHardware(m);
            string screwState=string.Join(" · ",m.sidePanel.fasteners.Select(f=>f.fastenerId+":"+Mathf.RoundToInt(f.tightness*100)+"%"));
            CardText("SIDE PANEL / FASTENERS",(m.sidePanelInstalled?"PANEL INSTALLED":"PANEL REMOVED")+" · tightening quality "+Mathf.RoundToInt(game.Assembly.TighteningQuality(m)*100)+"%\n"+screwState);
            GameObject fasteners=Horizontal("FastenerActions",64);AddFlexButton(fasteners.transform,"LOOSEN NEXT",()=>game.LoosenFastener(),new Color(.35f,.31f,.22f,1));AddFlexButton(fasteners.transform,m.sidePanelInstalled?"REMOVE PANEL":"FIT PANEL",()=>game.ToggleSidePanel(),accent);AddFlexButton(fasteners.transform,"TIGHTEN NEXT",()=>game.TightenFastener(),good);
            RenderComponentMounts(m);
            Section("CUSTOMIZATION / RGB");
            string[] fx={"Static","Breathing","Spectrum"};
            CardText("Aesthetic state","RGB hardware: "+OnOff(game.Customization.HasRgbDevice(m))+" · effect "+fx[Mathf.Clamp(m.customization.rgbEffect,0,2)]+" · cable theme #"+(m.customization.cableColorIndex+1)+" · aesthetic "+Mathf.RoundToInt(m.aestheticScore*100)+"%");
            GameObject rgb=Horizontal("RgbActions",64);AddFlexButton(rgb.transform,"NEXT COLOR",()=>game.CycleRgbPreset(),accent);AddFlexButton(rgb.transform,"RGB EFFECT",()=>game.CycleRgbEffect(),new Color(.30f,.32f,.40f,1));AddFlexButton(rgb.transform,"CABLE COLOR",()=>game.CycleCableColor(),new Color(.30f,.32f,.40f,1));
            CreateRgbPicker();
        }

        private void RenderComponentMounts(MachineState machine)
        {
            Section(loc.T("mount.title", "COMPONENT MOUNTING"));
            CardText(loc.T("mount.control", "Screwdriver"), loc.T("mount.help", "Power off and remove the panel. Each action turns one screw by 25%. Tighten every screw before POST; release all screws before removal."));
            WideButton(loc.T("mount.poweroff", "POWER OFF FOR SERVICE"), () => game.PowerOff(), accent);
            foreach (PartCategory category in new[] { PartCategory.Motherboard, PartCategory.PSU, PartCategory.GPU, PartCategory.Cooler })
            {
                if (string.IsNullOrEmpty(ComponentMountRules.InstalledItemId(machine, category))) continue;
                var mount = ComponentMountRules.Find(machine, category);
                if (mount?.fasteners == null) continue;
                string status = string.Join(" · ", mount.fasteners.Select((screw, index) => (index + 1) + ": " +
                    (screw == null || screw.damaged ? loc.T("mount.damaged", "DAMAGED") : Mathf.RoundToInt(screw.tightness * 100) + "%")));
                CardText(loc.T("mount." + category.ToString().ToLowerInvariant(), category.ToString()), status);
                PartCategory captured = category;
                GameObject row = Horizontal("MountActions_" + category, 64);
                AddFlexButton(row.transform, loc.T("mount.loosen", "LOOSEN NEXT SCREW"), () => game.TurnNextMountFastener(captured, false), accent);
                AddFlexButton(row.transform, loc.T("mount.tighten", "TIGHTEN NEXT SCREW"), () => game.TurnNextMountFastener(captured, true), good);
            }
        }

        private void RenderBios()
        {
            MachineState m=game.ActiveMachine;Heading("BIOS / UEFI","Firmware settings affect memory performance, thermals, acoustics and stability.");if(m==null){CardText("No device","Accept a job first.");return;}
            if(m.postCode!="A0")CardText("Firmware locked","A successful POST is required before normal BIOS configuration.",bad);
            CardText("Current firmware state","Memory profile: "+OnOff(m.bios.memoryProfileEnabled)+" · Memory target: "+m.bios.memorySpeedOverride+" MT/s\nCPU power limit: "+m.bios.cpuPowerLimitWatts+" W · Fan profile: "+(m.bios.fanProfile==0?"Silent":m.bios.fanProfile==1?"Balanced":"Performance")+" · Secure Boot: "+OnOff(m.bios.secureBoot));
            WideButton(m.bios.memoryProfileEnabled?"DISABLE MEMORY PROFILE":"ENABLE MEMORY PROFILE",()=>game.SetMemoryProfile(!m.bios.memoryProfileEnabled),accent);
            WideButton("CYCLE FAN PROFILE",()=>game.CycleFanProfile(),new Color(.28f,.32f,.38f,1));
        }

        private void RenderOS()
        {
            MachineState m=game.ActiveMachine;Heading("FORGEOS & SOFTWARE","The operating-system path is gated by POST and storage state; benchmark is gated by drivers.");if(m==null){CardText("No device","Accept a job first.");return;}
            CardText("Software state","Partitioned: "+OnOff(m.partitioned)+" · OS installed: "+OnOff(m.osInstalled)+" · activated: "+OnOff(m.activated)+" · drivers: "+OnOff(m.driversInstalled));
            GameObject row=Horizontal("OSActions",64);AddFlexButton(row.transform,loc.T("os.install","INSTALL FORGEOS"),()=>game.InstallOS(),accent);AddFlexButton(row.transform,loc.T("os.drivers","INSTALL DRIVERS"),()=>game.InstallDrivers(),new Color(.30f,.35f,.38f,1));AddFlexButton(row.transform,loc.T("os.benchmark","RUN BENCHMARK"),()=>game.RunBenchmark(),good);
            if(m.benchmarkScore>0)CardText("Benchmark report",m.benchmarkScore+" points · CPU "+m.cpuTempC.ToString("0")+"°C · GPU "+m.gpuTempC.ToString("0")+"°C · "+(m.stressStable?"PASS stable":"FAIL unstable"),m.stressStable?good:bad);
        }

        private void RenderDiag()
        {
            MachineState m=game.ActiveMachine;Heading("DIAGNOSTICS","Symptoms, POST codes, thermals, condition and fault flags are inspected without directly revealing hidden faults until the scan path is used.");if(m==null){CardText("No device","Accept a job first.");return;}
            WideButton(loc.T("diag.scan","RUN DIAGNOSTICS"),()=>game.Diagnose(),accent);CardText("Latest report",string.IsNullOrEmpty(m.lastDiagnostic)?"No scan yet.":m.lastDiagnostic);
            if(m.history.Count>0){Section("DEVICE HISTORY");foreach(string h in m.history.AsEnumerable().Reverse().Take(12))CardText(h,"");}
        }

        private void RenderProgress()
        {
            Heading("PROGRESSION & WORKSHOP","Upgrades change bench quality and unlock diagnostics/board-repair tiers while adding recurring costs.");
            WorkshopState w=game.State.workshop;CardText("Workshop level "+w.level,"Bench "+w.benchLevel+" · Storage "+w.storageLevel+" · Diagnostics "+w.diagnosticsLevel+" · Board repair "+w.boardRepairLevel+"\nReputation "+game.State.reputation+" · XP "+game.State.experience+" · Next upgrade $"+(350*w.level));
            WideButton(loc.T("progress.upgrade","UPGRADE WORKSHOP"),()=>game.UpgradeWorkshop(),good);
            if(game.State.milestones.Count>0)CardText("Milestones",string.Join(" · ",game.State.milestones));
            if(game.State.ledger.Count>0){Section("RECENT LEDGER");foreach(LedgerEntry e in game.State.ledger.AsEnumerable().Reverse().Take(10))CardText("Day "+e.day+" · "+e.reason,(e.amount>=0?"+":"")+e.amount.ToString("0.00")+" → balance $"+e.balanceAfter.ToString("0.00"),e.amount>=0?good:Color.white);}
        }

        private void RenderSettings()
        {
            Heading("SETTINGS / SAVE / ACCESSIBILITY","Three-slot atomic saves with backup recovery. Settings persist separately inside the schema and are applied on resume.");
            CardText("Current","Language: "+game.State.settings.language+" · FPS cap: "+Application.targetFrameRate+" · Battery saver: "+OnOff(game.State.settings.batterySaver)+" · Haptics: "+OnOff(game.State.settings.haptics)+"\nText "+Mathf.RoundToInt(game.State.settings.textScale*100)+"% · Look "+game.State.settings.lookSensitivity.ToString("0.00")+" · Reduced motion "+OnOff(game.State.settings.reducedMotion)+" · Captions "+OnOff(game.State.settings.captions)+" · Color assist "+game.State.settings.colorblindMode);
            GameObject r1=Horizontal("SaveRow",64);AddFlexButton(r1.transform,loc.T("settings.save","SAVE SLOT 1"),()=>game.Save(1),good);AddFlexButton(r1.transform,loc.T("settings.load","LOAD SLOT 1"),()=>game.Load(1),new Color(.28f,.32f,.38f,1));AddFlexButton(r1.transform,"NEW GAME",()=>game.NewGame(),bad);
            GameObject r2=Horizontal("SettingsRow",64);AddFlexButton(r2.transform,loc.T("settings.language","SWITCH LANGUAGE"),()=>game.SetLanguage(game.State.settings.language=="uk"?"en":"uk"),accent);AddFlexButton(r2.transform,loc.T("settings.battery","BATTERY SAVER"),()=>game.ToggleBatterySaver(),new Color(.28f,.32f,.38f,1));AddFlexButton(r2.transform,"HAPTICS",()=>game.ToggleHaptics(),new Color(.28f,.32f,.38f,1));
            GameObject r3=Horizontal("AccessRow",64);AddFlexButton(r3.transform,"TEXT −",()=>game.AdjustTextScale(-.1f),new Color(.28f,.32f,.38f,1));AddFlexButton(r3.transform,"TEXT +",()=>game.AdjustTextScale(.1f),new Color(.28f,.32f,.38f,1));AddFlexButton(r3.transform,"REDUCED MOTION",()=>game.ToggleReducedMotion(),new Color(.28f,.32f,.38f,1));AddFlexButton(r3.transform,"COLOR ASSIST",()=>game.CycleColorblindMode(),new Color(.28f,.32f,.38f,1));
            GameObject r4=Horizontal("ControlRow",64);AddFlexButton(r4.transform,"LOOK −",()=>game.AdjustLookSensitivity(-.1f),new Color(.28f,.32f,.38f,1));AddFlexButton(r4.transform,"LOOK +",()=>game.AdjustLookSensitivity(.1f),new Color(.28f,.32f,.38f,1));AddFlexButton(r4.transform,"CAPTIONS",()=>game.ToggleCaptions(),new Color(.28f,.32f,.38f,1));
            CardText("Touch controls","Left joystick moves. Drag the right half to look. Use INTERACT or tap workshop stations. PRECISION reduces walking speed. Keyboard/mouse fallback: WASD, mouse, E, Ctrl, Shift.");
        }

        private void CreateRgbPicker()
        {
            GameObject go=UIObject("TouchColorPicker",body);RawImage image=go.AddComponent<RawImage>();LayoutElement le=go.AddComponent<LayoutElement>();le.preferredHeight=100;le.minHeight=86;
            RgbColorPicker picker=go.AddComponent<RgbColorPicker>();picker.target=image;picker.onColor=c=>game.SetRgbColor(c);picker.onCommit=()=>game.CommitCustomization();picker.BuildTexture(192,64);
        }

        private void SlotRow(string label,string ids,PartCategory cat)
        {
            string display="EMPTY";if(!string.IsNullOrEmpty(ids)){var names=new List<string>();foreach(string id in ids.Split(',')){ItemInstance it=game.Inventory.Get(id);HardwareDefinition d=game.Inventory.Def(it);if(d!=null)names.Add(d.model);}if(names.Count>0)display=string.Join(" + ",names);}
            PartCategory captured=cat;Row(label,display,"INSTALL BEST",()=>game.InstallBestAvailable(captured));
        }

        private static string Spec(HardwareDefinition p)
        {
            var s=new List<string>();if(!string.IsNullOrEmpty(p.socket))s.Add(p.socket);if(!string.IsNullOrEmpty(p.formFactor))s.Add(p.formFactor);if(!string.IsNullOrEmpty(p.memoryType))s.Add(p.memoryType);if(p.capacityGB>0)s.Add(p.capacityGB+"GB");if(p.speed>0)s.Add(p.speed+"MT/s");if(p.storageGB>0)s.Add(p.storageGB+"GB "+p.storageInterface);if(p.psuWattage>0)s.Add(p.psuWattage+"W");if(p.vramGB>0)s.Add(p.vramGB+"GB VRAM");if(p.performance>0)s.Add("perf "+p.performance);return string.Join(" · ",s);
        }
        private static string OnOff(bool x)=>x?"ON":"OFF";

        private void Heading(string title,string subtitle){Text t=BlockText(title,34,Color.white,62);t.fontStyle=FontStyle.Bold;BlockText(subtitle,19,muted,54);}
        private void Section(string title){Text t=BlockText(title,23,accent,46);t.fontStyle=FontStyle.Bold;}
        private void CardText(string title,string sub){CardText(title,sub,Color.white);}
        private void CardText(string title,string sub,Color titleColor)
        {
            GameObject go=UIObject("Card",body);Image im=go.AddComponent<Image>();im.color=card;VerticalLayoutGroup v=go.AddComponent<VerticalLayoutGroup>();v.padding=new RectOffset(18,18,12,12);v.spacing=5;v.childForceExpandHeight=false;v.childControlHeight=true;LayoutElement le=go.AddComponent<LayoutElement>();le.minHeight=62;
            Text a=AddText(go.transform,title,22,TextAnchor.MiddleLeft,titleColor);a.fontStyle=FontStyle.Bold;LayoutElement la=a.gameObject.AddComponent<LayoutElement>();la.minHeight=30;
            if(!string.IsNullOrEmpty(sub)){Text b=AddText(go.transform,sub,18,TextAnchor.UpperLeft,muted);b.horizontalOverflow=HorizontalWrapMode.Wrap;b.verticalOverflow=VerticalWrapMode.Overflow;LayoutElement lb=b.gameObject.AddComponent<LayoutElement>();lb.minHeight=28;}
        }
        private void Row(string title,string sub,string buttonText,Action action)
        {
            GameObject row=UIObject("Row",body);Image im=row.AddComponent<Image>();im.color=card;HorizontalLayoutGroup h=row.AddComponent<HorizontalLayoutGroup>();h.padding=new RectOffset(16,12,8,8);h.spacing=12;h.childControlHeight=true;h.childForceExpandHeight=true;LayoutElement rl=row.AddComponent<LayoutElement>();rl.preferredHeight=70;
            GameObject textBox=UIObject("TextBox",row.transform);VerticalLayoutGroup tv=textBox.AddComponent<VerticalLayoutGroup>();tv.spacing=1;tv.childForceExpandHeight=false;LayoutElement tl=textBox.AddComponent<LayoutElement>();tl.flexibleWidth=1;
            Text a=AddText(textBox.transform,title,20,TextAnchor.MiddleLeft,Color.white);a.fontStyle=FontStyle.Bold;Text b=AddText(textBox.transform,sub,16,TextAnchor.MiddleLeft,muted);
            Button btn=AddButton(row.transform,buttonText,action,accent,17);LayoutElement bl=btn.gameObject.AddComponent<LayoutElement>();bl.preferredWidth=180;bl.minWidth=155;
        }
        private GameObject Horizontal(string name,float height){GameObject row=UIObject(name,body);HorizontalLayoutGroup h=row.AddComponent<HorizontalLayoutGroup>();h.spacing=10;h.childControlHeight=true;h.childForceExpandHeight=true;h.childForceExpandWidth=false;LayoutElement le=row.AddComponent<LayoutElement>();le.preferredHeight=height;return row;}
        private void AddFlexButton(Transform parent,string text,Action action,Color color){Button b=AddButton(parent,text,action,color,18);LayoutElement le=b.gameObject.AddComponent<LayoutElement>();le.flexibleWidth=1;le.preferredHeight=54;}
        private void WideButton(string text,Action action,Color color){Button b=AddButton(body,text,action,color,20);LayoutElement le=b.gameObject.AddComponent<LayoutElement>();le.preferredHeight=58;}
        private Text BlockText(string text,int size,Color color,float height){Text t=AddText(body,text,size,TextAnchor.MiddleLeft,color);LayoutElement le=t.gameObject.AddComponent<LayoutElement>();le.preferredHeight=height;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;return t;}

        private Button AddButton(Transform parent,string text,Action action,Color color,int size)
        {
            GameObject go=UIObject("Button_"+text,parent);Image im=go.AddComponent<Image>();im.color=color;Button b=go.AddComponent<Button>();ColorBlock cb=b.colors;cb.normalColor=Color.white;cb.highlightedColor=new Color(1.08f,1.08f,1.08f,1);cb.pressedColor=new Color(.82f,.82f,.82f,1);b.colors=cb;if(action!=null)b.onClick.AddListener(()=>action());
            Text t=AddText(go.transform,text,size,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,6);t.raycastTarget=false;return b;
        }
        private Text AddText(Transform parent,string text,int size,TextAnchor anchor,Color color)
        {
            GameObject go=UIObject("Text",parent);Text t=go.AddComponent<Text>();t.font=font;t.text=text;float scale=game?.State?.settings?.textScale??1f;t.fontSize=Mathf.Clamp(Mathf.RoundToInt(size*scale),12,52);t.alignment=anchor;t.color=color;t.supportRichText=true;t.raycastTarget=false;return t;
        }
        private GameObject UIObject(string name,Transform parent){GameObject go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);return go;}
        private static void Stretch(RectTransform r,float inset){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(inset,inset);r.offsetMax=new Vector2(-inset,-inset);}
        private void ClearBody(){for(int i=body.childCount-1;i>=0;i--)Destroy(body.GetChild(i).gameObject);}
        private static void SetButtonColor(Button b,Color color){Image i=b.GetComponent<Image>();if(i!=null)i.color=color;}
    }

    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform rect; private Rect last; private Vector2Int lastScreen;
        private void Awake(){rect=transform as RectTransform;Apply();}
        private void Update(){if(lastScreen.x!=Screen.width||lastScreen.y!=Screen.height||Screen.safeArea!=last)Apply();}
        private void Apply()
        {
            if(rect==null||Screen.width<=0||Screen.height<=0)return;Rect a=Screen.safeArea;last=a;lastScreen=new Vector2Int(Screen.width,Screen.height);
            rect.anchorMin=new Vector2(a.xMin/Screen.width,a.yMin/Screen.height);rect.anchorMax=new Vector2(a.xMax/Screen.width,a.yMax/Screen.height);rect.offsetMin=rect.offsetMax=Vector2.zero;
        }
    }

    public sealed class RgbColorPicker : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RawImage target; public Action<Color> onColor; public Action onCommit; private Texture2D texture; private RectTransform rect;
        private void Awake(){rect=transform as RectTransform;}
        public void BuildTexture(int width,int height)
        {
            texture=new Texture2D(width,height,TextureFormat.RGB24,false);texture.wrapMode=TextureWrapMode.Clamp;texture.filterMode=FilterMode.Bilinear;
            Color[] pixels=new Color[width*height];for(int y=0;y<height;y++){float sat=.2f+.8f*y/Mathf.Max(1f,height-1f);for(int x=0;x<width;x++){float hue=x/Mathf.Max(1f,width-1f);pixels[y*width+x]=Color.HSVToRGB(hue,sat,1f);}}
            texture.SetPixels(pixels);texture.Apply(false,false);if(target!=null)target.texture=texture;
        }
        public void OnPointerDown(PointerEventData e){Pick(e);}
        public void OnDrag(PointerEventData e){Pick(e);}
        public void OnPointerUp(PointerEventData e){Pick(e);onCommit?.Invoke();}
        private void Pick(PointerEventData e)
        {
            if(rect==null||!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,e.position,e.pressEventCamera,out Vector2 local))return;
            Rect r=rect.rect;float u=Mathf.Clamp01((local.x-r.xMin)/Mathf.Max(1f,r.width));float v=Mathf.Clamp01((local.y-r.yMin)/Mathf.Max(1f,r.height));onColor?.Invoke(Color.HSVToRGB(u,.2f+.8f*v,1f));
        }
        private void OnDestroy(){if(texture!=null)Destroy(texture);}
    }

    public sealed class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform knob; private RectTransform rect;
        private void Awake(){rect=transform as RectTransform;}
        public void OnPointerDown(PointerEventData e){UpdateValue(e);}
        public void OnDrag(PointerEventData e){UpdateValue(e);}
        public void OnPointerUp(PointerEventData e){MobileInputState.Move=Vector2.zero;if(knob!=null)knob.anchoredPosition=Vector2.zero;}
        private void UpdateValue(PointerEventData e)
        {
            Vector2 local;if(rect==null||!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,e.position,e.pressEventCamera,out local))return;Vector2 half=rect.rect.size*.5f;Vector2 n=new Vector2(local.x/Mathf.Max(1,half.x),local.y/Mathf.Max(1,half.y));n=Vector2.ClampMagnitude(n,1f);MobileInputState.Move=n;if(knob!=null)knob.anchoredPosition=new Vector2(n.x*half.x*.48f,n.y*half.y*.48f);
        }
    }

    public sealed class TouchLookPad : MonoBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData e){if(GameRuntime.Instance?.UI==null)return;MobileInputState.Look+=e.delta*.085f;}
    }

    public sealed class HoldPrecision : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData e){MobileInputState.Precision=true;}
        public void OnPointerUp(PointerEventData e){MobileInputState.Precision=false;}
        private void OnDisable(){MobileInputState.Precision=false;}
    }
}
