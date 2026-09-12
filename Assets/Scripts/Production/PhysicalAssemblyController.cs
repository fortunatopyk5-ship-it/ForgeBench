using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Physical, first-person assembly view. Installed components and empty slots are
    /// real colliders/interactables rather than decorative UI-only state.
    /// </summary>
    public sealed class PhysicalAssemblyController : MonoBehaviour
    {
        private GameRuntime game;
        private GameObject root;
        private Shader shader;
        private readonly List<Material> materials=new List<Material>();
        private string signature="";
        private bool dirty=true;
        private float nextPoll;

        private void Start()
        {
            game=GameRuntime.Instance;
            shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");
            game?.Events?.Subscribe("state.changed",OnStateChanged);
            dirty=true;
        }

        private void OnDestroy()
        {
            game?.Events?.Unsubscribe("state.changed",OnStateChanged);
            DestroyVisual();
        }

        private void OnStateChanged(object payload){dirty=true;}

        private void LateUpdate()
        {
            if(game==null){game=GameRuntime.Instance;if(game==null)return;}
            GameObject old=GameObject.Find("ActiveMachine3D");
            if(old!=null && old!=root && old.activeSelf)old.SetActive(false);

            if(Time.unscaledTime<nextPoll && !dirty)return;
            nextPoll=Time.unscaledTime+.25f;
            string now=Signature(game.ActiveMachine);
            if(dirty||now!=signature){signature=now;dirty=false;Rebuild();}
        }

        private static string Signature(MachineState m)
        {
            if(m==null)return "none";
            StringBuilder s=new StringBuilder(256);
            s.Append(m.machineId).Append('|').Append(m.caseItemId).Append('|').Append(m.motherboardItemId).Append('|').Append(m.cpuItemId).Append('|').Append(m.gpuItemId).Append('|').Append(m.psuItemId).Append('|').Append(m.coolerItemId);
            foreach(string x in m.ramItemIds)s.Append("|r:").Append(x);
            foreach(string x in m.storageItemIds)s.Append("|s:").Append(x);
            foreach(string x in m.fanItemIds)s.Append("|f:").Append(x);
            s.Append('|').Append(m.sidePanelInstalled).Append('|').Append(m.thermalPasteApplied).Append('|').Append(m.cables.atx24).Append('|').Append(m.cables.cpuEps).Append('|').Append(m.cables.gpuPower).Append('|').Append(m.cables.sataData).Append('|').Append(m.cables.frontPanel).Append('|').Append(m.customization?.rgbEffect??0).Append('|').Append(m.customization?.cableColorIndex??0);
            if(m.sidePanel?.fasteners!=null)foreach(FastenerState f in m.sidePanel.fasteners)s.Append('|').Append(f.fastenerId).Append(':').Append(Mathf.RoundToInt(f.tightness*100));
            return s.ToString();
        }

        private void Rebuild()
        {
            DestroyVisual();
            MachineState m=game.ActiveMachine;
            if(m==null)return;
            root=new GameObject("ProductionMachine3D");
            root.transform.position=new Vector3(0,1.43f,3.72f);

            Material frame=Mat(new Color(.075f,.085f,.095f),.30f,.85f);
            Material dark=Mat(new Color(.035f,.04f,.05f),.28f,.60f);
            Material board=Mat(new Color(.035f,.24f,.13f),.40f,.25f);
            Material copper=Mat(new Color(.72f,.34f,.12f),.42f,.72f);
            Material silver=Mat(new Color(.52f,.55f,.58f),.70f,.90f);
            Material black=Mat(new Color(.045f,.05f,.06f),.32f,.72f);
            Material blue=Mat(new Color(.06f,.34f,.66f),.42f,.35f);
            Material ghost=Mat(new Color(.16f,.22f,.28f),.18f,.1f);

            if(string.IsNullOrEmpty(m.caseItemId))
            {
                GameObject installCase=CreateGhostFrame("EMPTY CASE POSITION",Vector3.zero,new Vector3(.95f,.78f,.62f),ghost); AddSnap(installCase,PartCategory.Case);
                Interact(installCase,"Install best available case",40,()=>game.InstallBestAvailable(PartCategory.Case));
                return;
            }

            Box("CaseBottom",new Vector3(0,-.36f,0),new Vector3(.92f,.055f,.58f),frame);
            Box("CaseTop",new Vector3(0,.36f,0),new Vector3(.92f,.055f,.58f),frame);
            Box("CaseRear",new Vector3(0,0,.275f),new Vector3(.92f,.72f,.04f),frame);
            for(int sx=-1;sx<=1;sx+=2)for(int sz=-1;sz<=1;sz+=2)Box("CaseRail",new Vector3(sx*.445f,0,sz*.275f),new Vector3(.035f,.72f,.035f),frame);
            Box("FrontBezel",new Vector3(0,0,-.30f),new Vector3(.92f,.72f,.035f),dark);
            for(int y=-2;y<=2;y++)Box("FrontVent",new Vector3(0,y*.10f,-.323f),new Vector3(.65f,.025f,.012f),silver);

            BuildMotherboard(m,board,black,silver,copper,ghost);
            BuildCpuAndCooler(m,copper,silver,black,ghost);
            BuildRam(m,blue,copper,ghost);
            BuildGpu(m,black,silver,ghost);
            BuildPsu(m,black,silver,ghost);
            BuildStorage(m,dark,silver,ghost);
            BuildFans(m,black,silver,ghost);
            BuildCables(m);
            BuildPanelAndFasteners(m,frame,silver);
            BuildPowerButton(m);

            BoxCollider col=root.AddComponent<BoxCollider>();col.size=new Vector3(1.05f,.92f,.72f);
            WorldInteractable rootInteraction=root.AddComponent<WorldInteractable>();rootInteraction.label="Open assembly bench";rootInteraction.priority=4;rootInteraction.maxDistance=3.6f;rootInteraction.action=()=>game.UI?.OpenTab("BENCH");
        }

        private void BuildMotherboard(MachineState m,Material board,Material chip,Material metal,Material copper,Material ghost)
        {
            Vector3 p=new Vector3(.06f,.02f,.235f);
            if(string.IsNullOrEmpty(m.motherboardItemId))
            {
                GameObject g=Box("MotherboardGhost",p,new Vector3(.62f,.57f,.022f),ghost);AddSnap(g,PartCategory.Motherboard);Interact(g,"Install motherboard",32,()=>game.InstallBestAvailable(PartCategory.Motherboard));return;
            }
            GameObject mb=Box("Motherboard",p,new Vector3(.62f,.57f,.026f),board);InteractPart(mb,m.motherboardItemId,"Remove motherboard");
            Box("CpuSocket",p+new Vector3(-.08f,.11f,-.022f),new Vector3(.16f,.16f,.025f),metal);
            Box("Chipset",p+new Vector3(.18f,-.13f,-.025f),new Vector3(.10f,.10f,.030f),chip);
            for(int i=0;i<5;i++)Box("VRM",p+new Vector3(-.26f+i*.075f,.25f,-.026f),new Vector3(.055f,.055f,.035f),chip);
            for(int i=0;i<4;i++)Box("DIMMSlot",p+new Vector3(.12f+i*.055f,.07f,-.028f),new Vector3(.025f,.35f,.026f),chip);
            for(int i=0;i<2;i++)Box("PCIeSlot",p+new Vector3(.03f,-.16f-i*.09f,-.029f),new Vector3(.43f,.025f,.028f),chip);
            Box("M2Shield",p+new Vector3(-.12f,-.11f,-.032f),new Vector3(.27f,.055f,.025f),metal);
            for(int i=0;i<7;i++)Box("Trace",p+new Vector3(-.22f+i*.07f,-.02f,-.043f),new Vector3(.006f,.40f,.005f),copper);
        }

        private void BuildCpuAndCooler(MachineState m,Material copper,Material silver,Material black,Material ghost)
        {
            Vector3 cpuP=new Vector3(-.02f,.13f,.185f);
            if(string.IsNullOrEmpty(m.cpuItemId))
            {
                GameObject g=Box("CpuGhost",cpuP,new Vector3(.135f,.135f,.035f),ghost);AddSnap(g,PartCategory.CPU);Interact(g,"Install CPU",36,()=>game.InstallBestAvailable(PartCategory.CPU));
            }
            else
            {
                GameObject cpu=Box("CPU",cpuP,new Vector3(.135f,.135f,.035f),silver);InteractPart(cpu,m.cpuItemId,"Remove CPU");
                if(m.thermalPasteApplied)Cylinder("ThermalPaste",cpuP+new Vector3(0,0,-.025f),new Vector3(.045f,.006f,.045f),Quaternion.Euler(90,0,0),Mat(new Color(.62f,.64f,.66f),.18f,.05f));
                else{GameObject paste=Box("PastePrompt",cpuP+new Vector3(0,0,-.045f),new Vector3(.10f,.10f,.012f),ghost);Interact(paste,"Apply thermal paste",38,()=>game.ApplyThermalPaste());}
            }

            Vector3 coolP=new Vector3(-.02f,.13f,.045f);
            if(string.IsNullOrEmpty(m.coolerItemId))
            {
                GameObject g=Box("CoolerGhost",coolP,new Vector3(.26f,.28f,.20f),ghost);AddSnap(g,PartCategory.Cooler);Interact(g,"Install CPU cooler",31,()=>game.InstallBestAvailable(PartCategory.Cooler));
            }
            else
            {
                GameObject heatsink=Box("CoolerHeatsink",coolP,new Vector3(.25f,.29f,.18f),silver);InteractPart(heatsink,m.coolerItemId,"Remove CPU cooler");
                for(int i=-3;i<=3;i++)Box("CoolerFin",coolP+new Vector3(0,i*.035f,-.105f),new Vector3(.27f,.012f,.055f),silver);
                GameObject fan=Cylinder("CoolerFan",coolP+new Vector3(0,0,-.12f),new Vector3(.12f,.018f,.12f),Quaternion.Euler(90,0,0),black);fan.AddComponent<SpinVisual>().speed=420f;
            }
        }

        private void BuildRam(MachineState m,Material blue,Material copper,Material ghost)
        {
            for(int i=0;i<4;i++)
            {
                Vector3 p=new Vector3(.18f+i*.055f,.07f,.145f);
                if(i<m.ramItemIds.Count)
                {
                    string id=m.ramItemIds[i];GameObject r=Box("RAM_"+i,p,new Vector3(.032f,.33f,.085f),blue);InteractPart(r,id,"Remove RAM module");
                    for(int c=0;c<8;c++)Box("RAMChip",p+new Vector3(0,-.12f+c*.034f,-.048f),new Vector3(.036f,.022f,.012f),Mat(new Color(.025f,.03f,.035f),.25f,.3f));
                    Box("RAMContacts",p+new Vector3(0,-.17f,-.01f),new Vector3(.035f,.018f,.07f),copper);
                }
                else
                {
                    GameObject g=Box("RAMGhost_"+i,p,new Vector3(.028f,.33f,.045f),ghost);AddSnap(g,PartCategory.RAM);Interact(g,"Install RAM in DIMM slot "+(i+1),28,()=>game.InstallBestAvailable(PartCategory.RAM));
                }
            }
        }

        private void BuildGpu(MachineState m,Material black,Material silver,Material ghost)
        {
            Vector3 p=new Vector3(-.02f,-.13f,.05f);
            if(string.IsNullOrEmpty(m.gpuItemId))
            {
                GameObject g=Box("GPUGhost",p,new Vector3(.63f,.13f,.19f),ghost);AddSnap(g,PartCategory.GPU);Interact(g,"Install graphics card",30,()=>game.InstallBestAvailable(PartCategory.GPU));return;
            }
            GameObject gpu=Box("GPU",p,new Vector3(.63f,.13f,.19f),black);InteractPart(gpu,m.gpuItemId,"Remove graphics card");
            for(int i=-1;i<=1;i++)
            {
                GameObject fan=Cylinder("GPUFan",p+new Vector3(i*.19f,0,-.105f),new Vector3(.075f,.018f,.075f),Quaternion.Euler(90,0,0),silver);fan.AddComponent<SpinVisual>().speed=300f+i*35f;
            }
            Box("GPUBackplate",p+new Vector3(0,.075f,.01f),new Vector3(.61f,.025f,.18f),silver);
        }

        private void BuildPsu(MachineState m,Material black,Material silver,Material ghost)
        {
            Vector3 p=new Vector3(.25f,-.245f,-.13f);
            if(string.IsNullOrEmpty(m.psuItemId))
            {
                GameObject g=Box("PSUGhost",p,new Vector3(.32f,.20f,.30f),ghost);AddSnap(g,PartCategory.PSU);Interact(g,"Install power supply",30,()=>game.InstallBestAvailable(PartCategory.PSU));return;
            }
            GameObject psu=Box("PSU",p,new Vector3(.32f,.20f,.30f),black);InteractPart(psu,m.psuItemId,"Remove power supply");
            GameObject grille=Cylinder("PSUGrille",p+new Vector3(0,.11f,0),new Vector3(.11f,.01f,.11f),Quaternion.identity,silver);grille.AddComponent<SpinVisual>().speed=120f;
            for(int i=0;i<3;i++)Box("PSUPort",p+new Vector3(-.10f+i*.10f,0,-.16f),new Vector3(.065f,.06f,.012f),silver);
        }

        private void BuildStorage(MachineState m,Material dark,Material silver,Material ghost)
        {
            for(int i=0;i<2;i++)
            {
                Vector3 p=new Vector3(-.29f,-.24f+i*.09f,-.12f);
                if(i<m.storageItemIds.Count)
                {
                    string id=m.storageItemIds[i];GameObject drive=Box("Storage_"+i,p,new Vector3(.23f,.065f,.16f),dark);InteractPart(drive,id,"Remove storage drive");Box("DriveLabel",p+new Vector3(0,.037f,0),new Vector3(.17f,.008f,.10f),silver);
                }
                else if(i==0)
                {
                    GameObject g=Box("StorageGhost",p,new Vector3(.23f,.055f,.16f),ghost);AddSnap(g,PartCategory.Storage);Interact(g,"Install storage",24,()=>game.InstallBestAvailable(PartCategory.Storage));
                }
            }
        }

        private void BuildFans(MachineState m,Material black,Material silver,Material ghost)
        {
            Vector3[] spots={new Vector3(-.31f,.20f,-.27f),new Vector3(0,.20f,-.27f),new Vector3(.31f,.20f,-.27f)};
            for(int i=0;i<spots.Length;i++)
            {
                if(i<m.fanItemIds.Count)
                {
                    string id=m.fanItemIds[i];GameObject frame=Box("CaseFanFrame_"+i,spots[i],new Vector3(.22f,.22f,.025f),black);InteractPart(frame,id,"Remove case fan");GameObject rotor=Cylinder("CaseFanRotor_"+i,spots[i]+new Vector3(0,0,-.02f),new Vector3(.09f,.012f,.09f),Quaternion.Euler(90,0,0),silver);rotor.AddComponent<SpinVisual>().speed=260f+i*20f;
                }
                else if(i==0)
                {
                    GameObject g=Box("FanGhost",spots[i],new Vector3(.20f,.20f,.018f),ghost);AddSnap(g,PartCategory.Fan);Interact(g,"Install case fan",20,()=>game.InstallBestAvailable(PartCategory.Fan));
                }
            }
        }

        private void BuildCables(MachineState m)
        {
            Color[] palette={new Color(.025f,.025f,.03f),new Color(.70f,.05f,.045f),new Color(.05f,.28f,.72f),new Color(.82f,.78f,.66f),new Color(.38f,.10f,.58f)};
            int idx=Mathf.Clamp(m.customization?.cableColorIndex??0,0,palette.Length-1);Material cable=Mat(palette[idx],.25f,.08f);Material port=Mat(new Color(.18f,.20f,.22f),.40f,.45f);
            if(m.cables.atx24)Cable("ATX24",new Vector3(.28f,-.15f,-.08f),new Vector3(.30f,.05f,.17f),cable);
            else CablePort("ATX24Port",new Vector3(.30f,.08f,.17f),port,"Connect PC cables");
            if(m.cables.cpuEps)Cable("EPS",new Vector3(.25f,-.08f,-.02f),new Vector3(-.18f,.24f,.18f),cable);
            if(m.cables.gpuPower)Cable("GPU_PWR",new Vector3(.25f,-.13f,-.11f),new Vector3(.15f,-.13f,.02f),cable);
            if(m.cables.sataData)Cable("SATA",new Vector3(-.22f,-.25f,-.12f),new Vector3(.05f,-.08f,.19f),cable);
            if(!m.cables.atx24||!m.cables.cpuEps||(!string.IsNullOrEmpty(m.gpuItemId)&&!m.cables.gpuPower))
            {
                GameObject harness=Box("CableHarnessPrompt",new Vector3(.40f,-.05f,-.20f),new Vector3(.08f,.12f,.08f),port);Interact(harness,"Route and connect required cables",35,()=>game.ConnectCables());
            }
        }

        private void BuildPanelAndFasteners(MachineState m,Material frame,Material silver)
        {
            game.Assembly.EnsureCaseHardware(m);
            if(m.sidePanelInstalled)
            {
                Material glass=Mat(Color.Lerp(new Color(.07f,.10f,.12f),game.Customization.CurrentColor(m),.18f),.68f,.28f);
                GameObject panel=Box("SidePanel",new Vector3(.472f,0,0),new Vector3(.025f,.69f,.55f),glass);Interact(panel,"Side panel — loosen fasteners before removal",8,()=>game.ToggleSidePanel());
                if(m.sidePanel.fasteners!=null)
                {
                    for(int i=0;i<m.sidePanel.fasteners.Count;i++)
                    {
                        FastenerState f=m.sidePanel.fasteners[i];float y=i<2?.29f:-.29f;float z=i%2==0?.235f:-.235f;
                        Material stateMat=f.tightness>.05f?silver:Mat(new Color(.68f,.18f,.08f),.45f,.65f);
                        GameObject screw=Cylinder("PanelScrew_"+f.fastenerId,new Vector3(.495f,y,z),new Vector3(.025f,.012f,.025f),Quaternion.Euler(0,0,90),stateMat);
                        Interact(screw,"Loosen panel fastener "+f.fastenerId,55,()=>game.LoosenFastener()); WorldInteractable screwI=screw.GetComponent<WorldInteractable>();screwI.holdSeconds=.32f;screwI.requiredToolId="tool_driver";
                    }
                }
            }
            else
            {
                GameObject edge=CreateGhostFrame("PanelMount",new Vector3(.472f,0,0),new Vector3(.02f,.69f,.55f),frame);Interact(edge,"Fit side panel",18,()=>game.ToggleSidePanel());
            }
        }

        private void BuildPowerButton(MachineState m)
        {
            Material led=Mat(m.bootState==BootState.Off?new Color(.16f,.18f,.20f):new Color(.06f,.75f,.38f),.65f,.3f);if(led.HasProperty("_EmissionColor")){led.EnableKeyword("_EMISSION");led.SetColor("_EmissionColor",led.color*2f);}
            GameObject p=Cylinder("PowerButton",new Vector3(.32f,.27f,-.325f),new Vector3(.035f,.012f,.035f),Quaternion.Euler(90,0,0),led);Interact(p,"Power on / run POST",50,()=>game.PowerOn());
        }

        private static void AddSnap(GameObject go,PartCategory category){AssemblySnapPoint p=go.GetComponent<AssemblySnapPoint>()??go.AddComponent<AssemblySnapPoint>();if(!p.accepts.Contains(category))p.accepts.Add(category);}
        private void InteractPart(GameObject go,string itemId,string label){Interact(go,label,44,()=>game.Remove(itemId));}
        private static void Interact(GameObject go,string label,int priority,Action action){WorldInteractable i=go.GetComponent<WorldInteractable>()??go.AddComponent<WorldInteractable>();i.label=label;i.priority=priority;i.maxDistance=3.5f;i.action=action;}

        private GameObject CreateGhostFrame(string name,Vector3 p,Vector3 s,Material m)
        {
            GameObject holder=new GameObject(name);holder.transform.SetParent(root.transform,false);holder.transform.localPosition=p;
            float t=.025f;Vector3 hs=s*.5f;
            BoxLocal(holder.transform,"GTop",new Vector3(0,hs.y,0),new Vector3(s.x,t,t),m);BoxLocal(holder.transform,"GBottom",new Vector3(0,-hs.y,0),new Vector3(s.x,t,t),m);
            BoxLocal(holder.transform,"GL",new Vector3(-hs.x,0,0),new Vector3(t,s.y,t),m);BoxLocal(holder.transform,"GR",new Vector3(hs.x,0,0),new Vector3(t,s.y,t),m);
            BoxCollider c=holder.AddComponent<BoxCollider>();c.size=s;return holder;
        }

        private GameObject Box(string name,Vector3 localPos,Vector3 scale,Material mat){return BoxLocal(root.transform,name,localPos,scale,mat);}
        private GameObject BoxLocal(Transform parent,string name,Vector3 localPos,Vector3 scale,Material mat){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=localPos;g.transform.localScale=scale;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;return g;}
        private GameObject Cylinder(string name,Vector3 localPos,Vector3 scale,Quaternion rot,Material mat){GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=name;g.transform.SetParent(root.transform,false);g.transform.localPosition=localPos;g.transform.localScale=scale;g.transform.localRotation=rot;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;return g;}
        private void Cable(string name,Vector3 a,Vector3 b,Material mat){Vector3 d=b-a;GameObject g=Box(name,(a+b)*.5f,new Vector3(.028f,.028f,d.magnitude),mat);g.transform.localRotation=Quaternion.LookRotation(d.normalized,Vector3.up);}
        private void CablePort(string name,Vector3 p,Material mat,string label){GameObject g=Box(name,p,new Vector3(.07f,.07f,.04f),mat);Interact(g,label,25,()=>game.ConnectCables());}
        private Material Mat(Color c,float smooth,float metallic=0f){Material m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);materials.Add(m);return m;}
        private void DestroyVisual(){if(root!=null)Destroy(root);root=null;foreach(Material m in materials)if(m!=null)Destroy(m);materials.Clear();}
    }

    public sealed class SpinVisual : MonoBehaviour
    {
        public float speed=240f;
        private void Update(){if(GameRuntime.Instance?.State?.settings?.reducedMotion==true)return;transform.Rotate(0,0,speed*Time.unscaledDeltaTime,Space.Self);}
    }
}
