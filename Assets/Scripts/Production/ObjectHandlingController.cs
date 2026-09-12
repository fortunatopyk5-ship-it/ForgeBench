using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>Physical pickup/rotate/drop/snap workflow for inventory components.</summary>
    public sealed class ObjectHandlingController : MonoBehaviour
    {
        private GameRuntime game;
        private FirstPersonController player;
        private Camera cam;
        private GameObject trayRoot;
        private PickupPartProxy held;
        private Vector3 heldEuler;
        private float holdDistance=.85f;
        private bool dirty=true;
        private string traySignature="";
        private Canvas ui;
        private Text heldLabel;
        private GameObject controls;
        private Shader shader;
        private readonly List<Material> mats=new List<Material>();

        private IEnumerator Start()
        {
            game=GameRuntime.Instance;
            shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");
            while((player=FindAnyObjectByType<FirstPersonController>())==null)yield return null;
            cam=player.viewCamera;
            BuildUI();
            game.Events?.Subscribe("state.changed",OnStateChanged);
            dirty=true;
        }

        private void OnDestroy()
        {
            game?.Events?.Unsubscribe("state.changed",OnStateChanged);
            if(trayRoot!=null)Destroy(trayRoot);
            foreach(Material m in mats)if(m!=null)Destroy(m);
        }
        private void OnStateChanged(object p){dirty=true;}

        private void Update()
        {
            if(game==null||cam==null)return;
            string sig=TraySignature();
            if(dirty||sig!=traySignature){dirty=false;traySignature=sig;RebuildTray();}
            if(held==null)return;

            float dt=Time.unscaledDeltaTime;
            if(Input.GetKey(KeyCode.Q))heldEuler.y-=90f*dt;
            if(Input.GetKey(KeyCode.R))heldEuler.y+=90f*dt;
            if(Input.GetKey(KeyCode.Z))heldEuler.z-=90f*dt;
            if(Input.GetKey(KeyCode.X))heldEuler.z+=90f*dt;
            if(Input.mouseScrollDelta.y!=0)holdDistance=Mathf.Clamp(holdDistance+Input.mouseScrollDelta.y*.08f,.45f,1.25f);

            Vector3 target=cam.transform.position+cam.transform.forward*holdDistance+cam.transform.right*.08f-cam.transform.up*.05f;
            held.transform.position=Vector3.Lerp(held.transform.position,target,1f-Mathf.Exp(-16f*dt));
            held.transform.rotation=Quaternion.Slerp(held.transform.rotation,cam.transform.rotation*Quaternion.Euler(heldEuler),1f-Mathf.Exp(-14f*dt));
            if(Input.GetKeyDown(KeyCode.F))Drop();
        }

        public void BeginPickup(PickupPartProxy proxy)
        {
            if(proxy==null||held!=null)return;
            ItemInstance item=game.Inventory.Get(proxy.instanceId);
            if(item==null||item.reserved){game.Notify("That component is no longer available.",false);dirty=true;return;}
            held=proxy;proxy.SetHeld(true);heldEuler=Vector3.zero;holdDistance=.82f;
            controls.SetActive(true);RefreshHeldLabel();
        }

        public void RotateLeft(){if(held!=null)heldEuler.y-=22.5f;}
        public void RotateRight(){if(held!=null)heldEuler.y+=22.5f;}
        public void RotateRoll(){if(held!=null)heldEuler.z+=22.5f;}
        public void Inspect(){if(held!=null){holdDistance=holdDistance<.62f?1.0f:.52f;}}

        public void Drop()
        {
            if(held==null)return;
            PickupPartProxy proxy=held;held=null;controls.SetActive(false);
            if(TrySnap(proxy))return;
            proxy.Recover();
            game.Notify("Component returned to the parts tray.");
        }

        private bool TrySnap(PickupPartProxy proxy)
        {
            ItemInstance item=game.Inventory.Get(proxy.instanceId);MachineState machine=game.ActiveMachine;
            if(item==null||machine==null){proxy.Recover();return false;}
            HardwareDefinition def=game.Inventory.Def(item);if(def==null){proxy.Recover();return false;}
            AssemblySnapPoint[] points=FindObjectsByType<AssemblySnapPoint>(FindObjectsSortMode.None);
            AssemblySnapPoint best=null;float bestD=float.MaxValue;
            foreach(AssemblySnapPoint p in points)
            {
                if(!p.accepts.Contains(def.category))continue;float d=Vector3.Distance(proxy.transform.position,p.transform.position);if(d<p.snapRadius&&d<bestD){bestD=d;best=p;}
            }
            if(best==null)return false;
            ActionResult compatible=game.Compatibility.CanInstall(machine,item);
            if(!compatible.ok){game.Notify(compatible.message,false);proxy.Recover();return true;}
            proxy.transform.position=best.transform.position;proxy.transform.rotation=best.transform.rotation;
            game.Install(proxy.instanceId);
            if(item.reserved)Destroy(proxy.gameObject);else proxy.Recover();
            return true;
        }

        private string TraySignature()
        {
            if(game?.State==null)return "none";
            MachineState m=game.ActiveMachine;string mid=m?.machineId??"none";
            IEnumerable<ItemInstance> items=game.State.inventory.Where(i=>!i.reserved&&!i.customerOwned).OrderBy(i=>i.instanceId).Take(18);
            return mid+"|"+string.Join("|",items.Select(i=>i.instanceId));
        }

        private void RebuildTray()
        {
            if(held!=null)return;
            if(trayRoot!=null)Destroy(trayRoot);
            trayRoot=new GameObject("PhysicalPartsTray");
            trayRoot.transform.position=new Vector3(-1.55f,1.16f,3.55f);
            Material tray=Mat(new Color(.055f,.065f,.075f),.35f,.72f);
            GameObject basePlate=Primitive(PrimitiveType.Cube,"PartsTray",trayRoot.transform,Vector3.zero,new Vector3(1.55f,.055f,.82f),tray);
            WorldInteractable wi=basePlate.AddComponent<WorldInteractable>();wi.label="Physical parts tray";wi.priority=2;wi.action=()=>game.UI?.OpenTab("INVENTORY");

            List<ItemInstance> items=game.State.inventory.Where(i=>!i.reserved&&!i.customerOwned).Where(i=>game.Inventory.Def(i)?.category!=PartCategory.Tool&&game.Inventory.Def(i)?.category!=PartCategory.Consumable).OrderBy(i=>i.instanceId).Take(12).ToList();
            for(int i=0;i<items.Count;i++)
            {
                ItemInstance item=items[i];HardwareDefinition def=game.Inventory.Def(item);if(def==null)continue;
                int col=i%4,row=i/4;Vector3 p=new Vector3(-.56f+col*.38f,.10f,-.24f+row*.25f);
                GameObject visual=BuildProxyVisual(def,trayRoot.transform,p,i);
                PickupPartProxy proxy=visual.AddComponent<PickupPartProxy>();proxy.instanceId=item.instanceId;proxy.category=def.category;proxy.controller=this;proxy.CaptureHome();
                WorldInteractable it=visual.AddComponent<WorldInteractable>();it.label="Pick up "+def.brand+" "+def.model;it.priority=32;it.maxDistance=3.2f;it.action=()=>BeginPickup(proxy);
            }
        }

        private GameObject BuildProxyVisual(HardwareDefinition d,Transform parent,Vector3 p,int seed)
        {
            Color color=CategoryColor(d.category);Material main=Mat(color,.38f,.45f),metal=Mat(new Color(.45f,.48f,.52f),.62f,.8f),dark=Mat(new Color(.035f,.04f,.045f),.28f,.65f);
            Vector3 size;
            switch(d.category)
            {
                case PartCategory.Case:size=new Vector3(.25f,.22f,.18f);break;
                case PartCategory.Motherboard:size=new Vector3(.25f,.18f,.025f);break;
                case PartCategory.CPU:size=new Vector3(.10f,.10f,.025f);break;
                case PartCategory.RAM:size=new Vector3(.035f,.20f,.055f);break;
                case PartCategory.GPU:size=new Vector3(.28f,.08f,.12f);break;
                case PartCategory.Storage:size=new Vector3(.16f,.045f,.10f);break;
                case PartCategory.PSU:size=new Vector3(.17f,.12f,.15f);break;
                case PartCategory.Cooler:size=new Vector3(.15f,.16f,.13f);break;
                case PartCategory.Fan:size=new Vector3(.14f,.14f,.035f);break;
                default:size=new Vector3(.13f,.09f,.07f);break;
            }
            GameObject root=new GameObject("TrayPart_"+d.id);root.transform.SetParent(parent,false);root.transform.localPosition=p;
            GameObject core=Primitive(PrimitiveType.Cube,"Body",root.transform,Vector3.zero,size,main);
            Collider cc=core.GetComponent<Collider>();if(cc!=null)Destroy(cc);
            BoxCollider hit=root.AddComponent<BoxCollider>();hit.size=size*1.25f;
            if(d.category==PartCategory.GPU||d.category==PartCategory.Fan||d.category==PartCategory.Cooler)
            {
                GameObject fan=Primitive(PrimitiveType.Cylinder,"Fan",root.transform,new Vector3(0,0,-size.z*.55f),new Vector3(size.y*.34f,.012f,size.y*.34f),metal);fan.transform.localRotation=Quaternion.Euler(90,0,0);Collider c=fan.GetComponent<Collider>();if(c!=null)Destroy(c);fan.AddComponent<SpinVisual>().speed=120f+seed*7f;
            }
            if(d.category==PartCategory.Motherboard)
            {
                for(int i=0;i<3;i++){GameObject chip=Primitive(PrimitiveType.Cube,"Chip",root.transform,new Vector3(-.07f+i*.07f,.04f,-.02f),new Vector3(.04f,.04f,.018f),dark);Collider c=chip.GetComponent<Collider>();if(c!=null)Destroy(c);}
            }
            return root;
        }

        private void BuildUI()
        {
            Font font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");GameObject cg=new GameObject("ObjectHandlingUI");ui=cg.AddComponent<Canvas>();ui.renderMode=RenderMode.ScreenSpaceOverlay;ui.sortingOrder=140;CanvasScaler sc=cg.AddComponent<CanvasScaler>();sc.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;sc.referenceResolution=new Vector2(1920,1080);sc.matchWidthOrHeight=.5f;cg.AddComponent<GraphicRaycaster>();DontDestroyOnLoad(cg);
            controls=new GameObject("HeldControls",typeof(RectTransform),typeof(Image));controls.transform.SetParent(cg.transform,false);RectTransform r=controls.GetComponent<RectTransform>();r.anchorMin=new Vector2(.24f,.025f);r.anchorMax=new Vector2(.76f,.13f);r.offsetMin=r.offsetMax=Vector2.zero;controls.GetComponent<Image>().color=new Color(.035f,.045f,.058f,.94f);
            heldLabel=MakeText(controls.transform,font,"HOLDING COMPONENT",18);RectTransform lr=heldLabel.rectTransform;lr.anchorMin=new Vector2(.02f,.52f);lr.anchorMax=new Vector2(.36f,.98f);lr.offsetMin=lr.offsetMax=Vector2.zero;
            AddButton(controls.transform,font,"⟲",.38f,.03f,.48f,.48f,RotateLeft);AddButton(controls.transform,font,"⟳",.49f,.03f,.59f,.48f,RotateRight);AddButton(controls.transform,font,"ROLL",.60f,.03f,.70f,.48f,RotateRoll);AddButton(controls.transform,font,"INSPECT",.71f,.03f,.83f,.48f,Inspect);AddButton(controls.transform,font,"DROP / SNAP",.84f,.03f,.985f,.95f,Drop);
            controls.SetActive(false);
        }
        private void RefreshHeldLabel(){if(heldLabel==null||held==null)return;HardwareDefinition d=game.Inventory.Def(game.Inventory.Get(held.instanceId));heldLabel.text=d==null?held.instanceId:d.brand+" "+d.model+"\n"+d.category;}
        private static Text MakeText(Transform p,Font f,string value,int size){GameObject g=new GameObject("Text",typeof(RectTransform),typeof(Text));g.transform.SetParent(p,false);Text t=g.GetComponent<Text>();t.font=f;t.text=value;t.fontSize=size;t.alignment=TextAnchor.MiddleLeft;t.color=Color.white;return t;}
        private static void AddButton(Transform p,Font f,string label,float x0,float y0,float x1,float y1,UnityEngine.Events.UnityAction action){GameObject g=new GameObject(label,typeof(RectTransform),typeof(Image),typeof(Button));g.transform.SetParent(p,false);RectTransform r=g.GetComponent<RectTransform>();r.anchorMin=new Vector2(x0,y0);r.anchorMax=new Vector2(x1,y1);r.offsetMin=r.offsetMax=Vector2.zero;g.GetComponent<Image>().color=new Color(.10f,.14f,.18f,1f);Button b=g.GetComponent<Button>();b.onClick.AddListener(action);Text t=MakeText(g.transform,f,label,17);RectTransform tr=t.rectTransform;tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;t.alignment=TextAnchor.MiddleCenter;}
        private GameObject Primitive(PrimitiveType type,string name,Transform parent,Vector3 localPos,Vector3 scale,Material mat){GameObject g=GameObject.CreatePrimitive(type);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=localPos;g.transform.localScale=scale;Renderer rr=g.GetComponent<Renderer>();if(rr!=null)rr.sharedMaterial=mat;return g;}
        private Material Mat(Color c,float smooth,float metallic){Material m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);mats.Add(m);return m;}
        private static Color CategoryColor(PartCategory c){switch(c){case PartCategory.CPU:return new Color(.66f,.42f,.16f);case PartCategory.Motherboard:return new Color(.04f,.32f,.17f);case PartCategory.RAM:return new Color(.09f,.38f,.68f);case PartCategory.GPU:return new Color(.17f,.18f,.21f);case PartCategory.Storage:return new Color(.32f,.35f,.38f);case PartCategory.PSU:return new Color(.08f,.09f,.10f);case PartCategory.Cooler:return new Color(.40f,.44f,.48f);case PartCategory.Fan:return new Color(.12f,.14f,.17f);default:return new Color(.16f,.22f,.28f);}}
    }

    public sealed class PickupPartProxy : MonoBehaviour
    {
        public string instanceId;public PartCategory category;public ObjectHandlingController controller;
        private Vector3 homePos;private Quaternion homeRot;private Transform homeParent;private Collider hit;private WorldInteractable interaction;
        public void CaptureHome(){homePos=transform.localPosition;homeRot=transform.localRotation;homeParent=transform.parent;hit=GetComponent<Collider>();interaction=GetComponent<WorldInteractable>();}
        public void SetHeld(bool value){if(value){transform.SetParent(null,true);if(hit!=null)hit.enabled=false;if(interaction!=null)interaction.enabled=false;}else{if(hit!=null)hit.enabled=true;if(interaction!=null)interaction.enabled=true;}}
        public void Recover(){transform.SetParent(homeParent,false);transform.localPosition=homePos;transform.localRotation=homeRot;SetHeld(false);}
    }

    public sealed class AssemblySnapPoint : MonoBehaviour
    {
        public List<PartCategory> accepts=new List<PartCategory>();
        public float snapRadius=.55f;
    }
}
