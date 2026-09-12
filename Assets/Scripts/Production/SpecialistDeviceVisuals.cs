using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Procedural physical representations for laptop/phone/controller, board-repair,
    /// NAS/server and liquid-loop work. Visual state is derived entirely from persisted
    /// MachineState so save/load and workstation UI always agree.
    /// </summary>
    public sealed class SpecialistDeviceVisuals : MonoBehaviour
    {
        private GameRuntime game;
        private GameObject root;
        private readonly List<Material> materials=new List<Material>();
        private Shader shader;
        private string lastSignature=string.Empty;
        private float nextRefresh;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if(FindAnyObjectByType<SpecialistDeviceVisuals>()!=null)return;
            GameObject go=new GameObject("ForgeBench_SpecialistDeviceVisuals");DontDestroyOnLoad(go);go.AddComponent<SpecialistDeviceVisuals>();
        }

        private IEnumerator Start()
        {
            while(GameRuntime.Instance==null||GameRuntime.Instance.World==null)yield return null;
            game=GameRuntime.Instance;shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");Rebuild();
        }

        private void Update()
        {
            if(Time.unscaledTime<nextRefresh)return;nextRefresh=Time.unscaledTime+.25f;
            game=GameRuntime.Instance;if(game==null)return;string sig=Signature(game.ActiveMachine);if(sig!=lastSignature)Rebuild();
        }

        private string Signature(MachineState m)
        {
            if(m==null)return "none";
            PortableDeviceState p=m.portable;BoardRepairState b=m.boardRepair;NetworkLabState n=m.network;LiquidLoopState l=m.liquidLoop;
            return (m.machineId??"")+"|"+m.category+"|"+
                (p==null?"":p.screwsRemaining+":"+p.backCoverRemoved+":"+p.batteryDisconnected+":"+p.displaySeparated+":"+p.batteryHealth.ToString("0.00")+":"+p.displayHealth.ToString("0.00")+":"+p.controllerDrift.ToString("0.00"))+"|"+
                (b==null?"":b.esdGrounded+":"+b.microscopeInspected+":"+b.powerRailMeasured+":"+b.shortLocated+":"+b.reworkCycles+":"+b.repaired)+"|"+
                (n==null?"":n.linkUp+":"+n.disksHealthy+":"+n.disksTotal+":"+n.arrayDegraded+":"+n.scrubComplete)+"|"+
                (l==null?"":l.pumpInstalled+":"+l.reservoirInstalled+":"+l.radiatorMm+":"+l.fittingCount+":"+l.tightFittings+":"+l.coolantLitres.ToString("0.00")+":"+l.leakTestPassed);
        }

        private void Rebuild()
        {
            game=GameRuntime.Instance;if(shader==null)shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");
            if(root!=null)Destroy(root);foreach(Material m in materials)if(m!=null)Destroy(m);materials.Clear();root=new GameObject("SpecialistActiveDevice");MachineState machine=game?.ActiveMachine;lastSignature=Signature(machine);if(machine==null)return;
            if(machine.category==DeviceCategory.Phone||machine.category==DeviceCategory.Tablet||machine.category==DeviceCategory.Laptop||machine.category==DeviceCategory.Controller||machine.category==DeviceCategory.Handheld||machine.category==DeviceCategory.Console)BuildPortable(machine);
            if(machine.category==DeviceCategory.NAS||machine.category==DeviceCategory.Server||machine.category==DeviceCategory.Router)BuildNetwork(machine);
            if(machine.boardRepair!=null&&(SpecialistJobService.IsSpecialist(game.ActiveJob)&&game.ActiveJob.type==JobType.BoardRepair))BuildBoard(machine);
            if(machine.liquidLoop!=null&&SpecialistJobService.IsSpecialist(game.ActiveJob)&&game.ActiveJob.requiredPartCategories.Exists(x=>x.Contains("LiquidBuild")))BuildLiquid(machine);
        }

        private void BuildPortable(MachineState m)
        {
            Vector3 basePos=new Vector3(-5.55f,.78f,-2.0f);PortableDeviceState s=m.portable??new PortableDeviceState();
            Material body=Mat(new Color(.075f,.085f,.095f),.55f,.75f),metal=Mat(new Color(.22f,.24f,.26f),.62f,.82f),pcb=Mat(new Color(.045f,.25f,.14f),.38f,.25f),battery=Mat(new Color(.11f,.12f,.13f),.42f,.35f),screen=Mat(s.displayHealth<.5f?new Color(.22f,.035f,.04f):new Color(.025f,.12f,.17f),.82f,.10f),accent=Mat(new Color(.09f,.54f,.83f),.55f,.18f);
            bool phone=m.category==DeviceCategory.Phone||m.category==DeviceCategory.Tablet;
            float w=1.65f;
            float d=1.08f;
            if(phone)
            {
                if(m.category==DeviceCategory.Tablet){w=1.05f;d=.72f;}
                else {w=.62f;d=.34f;}
            }
            GameObject chassis=Box("PortableChassis",basePos,new Vector3(w,.09f,d),body);
            AddInteractable(chassis,"Inspect "+m.category,8,()=>SpecialistRepairPanel.Open(SpecialistPanelMode.Portable));
            if(!s.backCoverRemoved)
            {
                GameObject cover=Box("BackCover",basePos+new Vector3(0,.075f,0),new Vector3(w*.96f,.045f,d*.94f),metal);AddInteractable(cover,"Remove back cover",18,()=>{game.PortableRemoveCover();Rebuild();});
                int screws=Mathf.Clamp(s.screwsRemaining,0,10);for(int i=0;i<screws;i++){float t=screws<=1?.5f:i/(float)(screws-1);float x=Mathf.Lerp(-w*.40f,w*.40f,t);float z=(i%2==0?-1:1)*d*.39f;GameObject screw=Cylinder("Screw",basePos+new Vector3(x,.115f,z),new Vector3(.025f,.012f,.025f),Quaternion.identity,metal);AddInteractable(screw,"Remove chassis screw",24,()=>{game.PortableRemoveScrew();Rebuild();});}
            }
            else
            {
                Box("MainBoard",basePos+new Vector3(-w*.18f,.10f,0),new Vector3(w*.48f,.025f,d*.75f),pcb);
                GameObject batt=Box("Battery",basePos+new Vector3(w*.23f,.11f,0),new Vector3(w*.34f,.05f,d*.70f),battery);AddInteractable(batt,s.batteryDisconnected?"Replace battery":"Disconnect battery",25,()=>{if(!s.batteryDisconnected)game.PortableDisconnectBattery();else game.PortableReplaceBattery();Rebuild();});
                GameObject port=Box("ChargePort",basePos+new Vector3(0,.115f,-d*.48f),new Vector3(.18f,.045f,.08f),metal);AddInteractable(port,"Service charging port",24,()=>{game.PortableServicePort();Rebuild();});
                if(phone)
                {
                    GameObject display=Box("DisplayAssembly",basePos+new Vector3(0,s.displaySeparated?.36f:.18f,0),new Vector3(w*.96f,.035f,d*.96f),screen);if(s.displaySeparated)display.transform.rotation=Quaternion.Euler(0,0,12);AddInteractable(display,s.displaySeparated?"Replace display":"Separate display",26,()=>{if(!s.displaySeparated)game.PortableSeparateDisplay();else game.PortableReplaceDisplay();Rebuild();});
                }
                if(m.category==DeviceCategory.Controller||m.category==DeviceCategory.Handheld||m.category==DeviceCategory.Console)
                {
                    for(int side=-1;side<=1;side+=2){GameObject stick=Cylinder("AnalogStick",basePos+new Vector3(side*w*.23f,.19f,.08f),new Vector3(.10f,.08f,.10f),Quaternion.identity,accent);AddInteractable(stick,"Calibrate analog control",25,()=>{game.PortableCalibrate();Rebuild();});}
                }
                GameObject seal=Box("ReassemblyZone",basePos+new Vector3(0,.06f,d*.58f),new Vector3(w*.55f,.035f,.12f),accent);AddInteractable(seal,"Reassemble and reseal device",20,()=>{game.PortableReseal();Rebuild();});
            }
        }

        private void BuildBoard(MachineState m)
        {
            Vector3 p=new Vector3(5.55f,.83f,1.55f);BoardRepairState s=m.boardRepair??new BoardRepairState();Material pcb=Mat(new Color(.035f,.28f,.16f),.38f,.28f),chip=Mat(new Color(.045f,.05f,.055f),.55f,.65f),copper=Mat(new Color(.58f,.25f,.08f),.48f,.75f),danger=Mat(new Color(.75f,.08f,.045f),.45f,.25f),good=Mat(new Color(.08f,.65f,.36f),.55f,.18f),metal=Mat(new Color(.30f,.32f,.34f),.62f,.82f);
            GameObject board=Box("RepairPCB",p,new Vector3(1.45f,.035f,.86f),pcb);AddInteractable(board,"Microscope inspect board",20,()=>{game.BoardInspect();Rebuild();});
            for(int x=-2;x<=2;x++)for(int z=-1;z<=1;z++){Vector3 cp=p+new Vector3(x*.23f,.055f,z*.20f);Box("IC",cp,new Vector3(.13f,.035f,.11f),chip);}
            for(int i=0;i<6;i++)Cylinder("Capacitor",p+new Vector3(-.52f+i*.19f,.08f,.30f),new Vector3(.035f,.05f,.035f),Quaternion.identity,copper);
            GameObject esd=Box("ESDClip",p+new Vector3(-.82f,.09f,-.36f),new Vector3(.16f,.08f,.12f),s.esdGrounded?good:danger);AddInteractable(esd,"Ground ESD strap",30,()=>{game.BoardGroundEsd();Rebuild();});
            GameObject probe=Cylinder("MeterProbe",p+new Vector3(.45f,.22f,-.22f),new Vector3(.025f,.20f,.025f),Quaternion.Euler(65,0,18),metal);AddInteractable(probe,"Measure power rail",28,()=>{game.BoardMeasure();Rebuild();});
            GameObject hotspot=Box("SuspectPowerStage",p+new Vector3(.30f,.075f,.18f),new Vector3(.18f,.05f,.14f),s.repaired?good:danger);AddInteractable(hotspot,s.shortLocated?"Apply flux / rework short":"Locate electrical short",32,()=>{if(!s.shortLocated)game.BoardLocateShort();else if(!s.fluxApplied)game.BoardApplyFlux();else game.BoardRework();Rebuild();});
            GameObject verify=Box("VerificationPad",p+new Vector3(.68f,.075f,.32f),new Vector3(.18f,.045f,.12f),good);AddInteractable(verify,"Verify board repair",24,()=>{game.BoardVerify();Rebuild();});
        }

        private void BuildNetwork(MachineState m)
        {
            Vector3 p=new Vector3(5.55f,.78f,-2.0f);NetworkLabState s=m.network??new NetworkLabState();Material chassis=Mat(new Color(.07f,.08f,.09f),.42f,.78f),metal=Mat(new Color(.22f,.24f,.26f),.55f,.82f),green=Mat(new Color(.04f,.70f,.28f),.55f,.18f),red=Mat(new Color(.80f,.07f,.04f),.50f,.18f),blue=Mat(new Color(.05f,.42f,.80f),.58f,.18f);
            GameObject box=Box("NASChassis",p,new Vector3(1.75f,.66f,.98f),chassis);AddInteractable(box,"Open network/NAS workstation",12,()=>SpecialistRepairPanel.Open(SpecialistPanelMode.Network));
            int total=Mathf.Clamp(s.disksTotal,2,8);for(int i=0;i<total;i++){float x=-.66f+i*(1.32f/Mathf.Max(1,total-1));bool healthy=i<s.disksHealthy;GameObject bay=Box("DriveBay"+i,p+new Vector3(x,.02f,-.51f),new Vector3(.22f,.48f,.07f),healthy?metal:red);if(!healthy)AddInteractable(bay,"Replace failed array disk",30,()=>{game.NetworkReplaceDisk();Rebuild();});GameObject led=Box("DriveLed",p+new Vector3(x,-.20f,-.555f),new Vector3(.08f,.025f,.015f),healthy?green:red);}
            GameObject port=Box("EthernetPort",p+new Vector3(.62f,.12f,.51f),new Vector3(.22f,.12f,.05f),s.linkUp?green:red);AddInteractable(port,s.linkUp?"Run network throughput test":"Connect network link",28,()=>{if(!s.linkUp)game.NetworkConnect();else game.NetworkTest();Rebuild();});
            GameObject scrub=Box("RaidStatus",p+new Vector3(-.48f,.12f,.51f),new Vector3(.28f,.12f,.05f),!s.arrayDegraded&&s.scrubComplete?green:blue);AddInteractable(scrub,"Rebuild / scrub storage array",24,()=>{game.NetworkScrub();Rebuild();});
        }

        private void BuildLiquid(MachineState m)
        {
            Vector3 p=new Vector3(-5.55f,.78f,1.55f);LiquidLoopState s=m.liquidLoop??new LiquidLoopState();Material frame=Mat(new Color(.08f,.09f,.10f),.38f,.72f),blue=Mat(new Color(.04f,.36f,.70f),.72f,.12f),metal=Mat(new Color(.25f,.27f,.29f),.62f,.82f),green=Mat(new Color(.05f,.67f,.32f),.60f,.14f),red=Mat(new Color(.72f,.06f,.04f),.55f,.14f);
            Box("LoopFrame",p,new Vector3(1.45f,.86f,.78f),frame);
            if(s.reservoirInstalled){GameObject reservoir=Cylinder("Reservoir",p+new Vector3(-.46f,.12f,-.18f),new Vector3(.12f,.30f,.12f),Quaternion.identity,blue);AddInteractable(reservoir,"Add coolant",24,()=>{game.LiquidFill();Rebuild();});}else{GameObject spot=Box("ReservoirMount",p+new Vector3(-.46f,.12f,-.18f),new Vector3(.25f,.60f,.25f),metal);AddInteractable(spot,"Install reservoir",25,()=>{game.LiquidInstallReservoir();Rebuild();});}
            if(s.pumpInstalled){GameObject pump=Cylinder("Pump",p+new Vector3(-.42f,-.27f,.10f),new Vector3(.15f,.12f,.15f),Quaternion.identity,metal);AddInteractable(pump,"Bleed custom loop",24,()=>{game.LiquidBleed();Rebuild();});}else{GameObject mount=Box("PumpMount",p+new Vector3(-.42f,-.27f,.10f),new Vector3(.30f,.18f,.30f),metal);AddInteractable(mount,"Install pump",25,()=>{game.LiquidInstallPump();Rebuild();});}
            if(s.radiatorMm>0){GameObject rad=Box("Radiator",p+new Vector3(.43f,.15f,.15f),new Vector3(.26f,.62f,.50f),metal);AddInteractable(rad,"Inspect radiator / fittings",18,()=>SpecialistRepairPanel.Open(SpecialistPanelMode.Liquid));}else{GameObject rm=Box("RadiatorMount",p+new Vector3(.43f,.15f,.15f),new Vector3(.30f,.65f,.52f),frame);AddInteractable(rm,"Install 360 mm radiator",25,()=>{game.LiquidInstallRadiator(360);Rebuild();});}
            int fits=Mathf.Clamp(s.fittingCount,0,12);for(int i=0;i<fits;i++){bool tight=i<s.tightFittings;GameObject f=Cylinder("Fitting"+i,p+new Vector3(-.15f+(i%4)*.14f,-.30f+(i/4)*.13f,-.32f),new Vector3(.035f,.035f,.035f),Quaternion.Euler(90,0,0),tight?green:red);if(!tight)AddInteractable(f,"Tighten fitting",30,()=>{game.LiquidTightenFitting();Rebuild();});}
            GameObject test=Box("LeakTestPanel",p+new Vector3(.48f,-.30f,-.35f),new Vector3(.34f,.16f,.08f),s.leakTestPassed?green:(s.leakDetected?red:blue));AddInteractable(test,"Run isolated leak test",28,()=>{game.LiquidLeakTest();Rebuild();});
        }

        private GameObject Box(string n,Vector3 p,Vector3 s,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}
        private GameObject Cylinder(string n,Vector3 p,Vector3 s,Quaternion q,Material m){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=n;g.transform.SetParent(root.transform,true);g.transform.position=p;g.transform.localScale=s;g.transform.rotation=q;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=m;return g;}
        private Material Mat(Color c,float smooth,float metallic=0){Material m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);materials.Add(m);return m;}
        private static void AddInteractable(GameObject g,string label,int priority,Action action){WorldInteractable i=g.GetComponent<WorldInteractable>()??g.AddComponent<WorldInteractable>();i.label=label;i.priority=priority;i.action=action;i.maxDistance=3.2f;}
        private void OnDestroy(){if(root!=null)Destroy(root);foreach(Material m in materials)if(m!=null)Destroy(m);}
    }
}
