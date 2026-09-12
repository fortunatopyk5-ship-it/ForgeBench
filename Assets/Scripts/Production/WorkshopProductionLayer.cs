using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Adds a materially richer workshop around the core gameplay world. Everything
    /// here is generated from deterministic source code, so the repository stays
    /// source-first while the Android player receives a complete visible space.
    /// </summary>
    public sealed class WorkshopProductionLayer : MonoBehaviour
    {
        private GameRuntime game;
        private GameObject root;
        private readonly List<Material> materials = new List<Material>();
        private Shader shader;

        private IEnumerator Start()
        {
            game = GameRuntime.Instance;
            while (game == null || game.World == null) { game = GameRuntime.Instance; yield return null; }
            yield return null;
            Build();
        }

        private void Build()
        {
            if (root != null) return;
            root = new GameObject("ProductionWorkshopLayer");
            shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            BuildArchitecture();
            BuildToolWall();
            BuildStorageDetail();
            BuildOfficeCorner();
            BuildServiceStations();
            BuildCeilingLights();
            BuildSignage();
        }

        private void BuildArchitecture()
        {
            Material wall=Mat(new Color(.115f,.13f,.15f),.55f);
            Material trim=Mat(new Color(.035f,.045f,.055f),.35f,.65f);
            Material floor=Mat(new Color(.07f,.075f,.08f),.28f,.15f);
            Material glass=Mat(new Color(.12f,.28f,.36f),.75f,.1f);
            Material accent=Mat(new Color(.03f,.48f,.76f),.48f,.25f);

            Box("ProductionFloor",new Vector3(0,-.155f,0),new Vector3(14.2f,.14f,12.2f),floor);
            Box("FrontWallL",new Vector3(-4.3f,2.5f,-5.95f),new Vector3(5.4f,5f,.18f),wall);
            Box("FrontWallR",new Vector3(4.3f,2.5f,-5.95f),new Vector3(5.4f,5f,.18f),wall);
            Box("FrontHeader",new Vector3(0,4.45f,-5.95f),new Vector3(3.2f,1.1f,.18f),wall);
            Box("DoorFrameL",new Vector3(-1.65f,1.7f,-5.82f),new Vector3(.12f,3.4f,.18f),trim);
            Box("DoorFrameR",new Vector3(1.65f,1.7f,-5.82f),new Vector3(.12f,3.4f,.18f),trim);
            Box("DoorHeader",new Vector3(0,3.35f,-5.82f),new Vector3(3.4f,.12f,.18f),trim);
            Box("GlassDoorL",new Vector3(-.82f,1.65f,-5.84f),new Vector3(1.5f,3.15f,.045f),glass);
            Box("GlassDoorR",new Vector3(.82f,1.65f,-5.84f),new Vector3(1.5f,3.15f,.045f),glass);
            Box("BlueThreshold",new Vector3(0,.015f,-5.72f),new Vector3(3.35f,.035f,.35f),accent);
            Box("Ceiling",new Vector3(0,4.85f,0),new Vector3(14,.16f,12),trim);

            // Anti-slip work zones.
            Material mat=Mat(new Color(.055f,.065f,.075f),.18f,.05f);
            Box("AssemblyMat",new Vector3(0,.012f,2.9f),new Vector3(3.7f,.02f,2.7f),mat);
            Box("DiagnosticsMat",new Vector3(-4,.012f,2.9f),new Vector3(2.8f,.02f,2.7f),mat);
            Box("RepairMat",new Vector3(4,.012f,2.9f),new Vector3(2.8f,.02f,2.7f),mat);
        }

        private void BuildToolWall()
        {
            Material panel=Mat(new Color(.16f,.18f,.19f),.42f,.35f);
            Material metal=Mat(new Color(.30f,.32f,.34f),.65f,.75f);
            Material red=Mat(new Color(.56f,.08f,.07f),.42f,.4f);
            Material yellow=Mat(new Color(.75f,.48f,.04f),.42f,.3f);
            Box("PegBoard",new Vector3(0,2.55f,5.74f),new Vector3(5.3f,2.6f,.08f),panel);
            for(int y=0;y<8;y++) for(int x=0;x<18;x++)
                Cylinder("Peg",new Vector3(-2.35f+x*.28f,1.55f+y*.26f,5.63f),new Vector3(.018f,.025f,.018f),Quaternion.Euler(90,0,0),metal);

            // Recognisable source-generated tools.
            for(int i=0;i<5;i++)
            {
                float x=-1.9f+i*.55f;
                Cylinder("DriverHandle",new Vector3(x,2.85f,5.52f),new Vector3(.065f,.23f,.065f),Quaternion.Euler(0,0,0),i%2==0?red:yellow);
                Cylinder("DriverShaft",new Vector3(x,2.45f,5.52f),new Vector3(.018f,.24f,.018f),Quaternion.identity,metal);
            }
            Box("ESDMatWall",new Vector3(1.65f,2.55f,5.58f),new Vector3(1.6f,.75f,.035f),Mat(new Color(.07f,.23f,.21f),.25f,.1f));
            Box("ToolShelf",new Vector3(0,1.25f,5.45f),new Vector3(5.6f,.10f,.55f),metal);
            for(int i=0;i<8;i++) Box("PartsBin",new Vector3(-2.25f+i*.64f,1.48f,5.32f),new Vector3(.50f,.38f,.42f),Mat(new Color(.10f+.02f*(i%3),.18f,.23f),.35f,.1f));
        }

        private void BuildStorageDetail()
        {
            Material rack=Mat(new Color(.17f,.19f,.21f),.45f,.78f);
            Material box=Mat(new Color(.32f,.24f,.15f),.30f,.05f);
            for(int bay=0;bay<2;bay++)
            {
                float x=-5.35f+bay*2.05f;
                for(int sy=0;sy<4;sy++) Box("RackShelf",new Vector3(x,.55f+sy*.82f,-3.85f),new Vector3(1.8f,.07f,.85f),rack);
                for(int sx=-1;sx<=1;sx+=2) Box("RackPost",new Vector3(x+sx*.82f,1.75f,-3.85f),new Vector3(.07f,3.25f,.07f),rack);
                for(int n=0;n<6;n++)
                {
                    float px=x-.55f+(n%3)*.55f, py=.80f+(n/3)*.82f;
                    Box("ShippingCarton",new Vector3(px,py,-3.84f),new Vector3(.46f,.42f,.64f),box);
                }
            }
            GameObject scanner=Box("BarcodeScanner",new Vector3(-2.82f,.95f,-4.15f),new Vector3(.18f,.28f,.12f),Mat(new Color(.08f,.09f,.10f),.5f,.65f));
            AddInteractable(scanner,"Receive and scan delivered packages",15,()=>game.ReceiveAll());
        }

        private void BuildOfficeCorner()
        {
            Material wood=Mat(new Color(.19f,.135f,.095f),.5f,.08f);
            Material metal=Mat(new Color(.12f,.14f,.16f),.35f,.65f);
            Material screen=Mat(new Color(.025f,.12f,.18f),.72f,.1f);
            Box("OfficeDesk",new Vector3(4.85f,.75f,-3.7f),new Vector3(2.5f,.12f,1.1f),wood);
            for(int sx=-1;sx<=1;sx+=2) for(int sz=-1;sz<=1;sz+=2) Box("DeskLeg",new Vector3(4.85f+sx*1.05f,.35f,-3.7f+sz*.43f),new Vector3(.08f,.75f,.08f),metal);
            Box("Monitor",new Vector3(4.85f,1.55f,-3.34f),new Vector3(1.35f,.78f,.08f),screen);
            Box("MonitorStand",new Vector3(4.85f,1.10f,-3.37f),new Vector3(.10f,.38f,.10f),metal);
            Box("Keyboard",new Vector3(4.85f,.87f,-3.90f),new Vector3(1.1f,.045f,.36f),metal);
            GameObject tablet=Box("OfficeTablet",new Vector3(5.65f,.92f,-3.82f),new Vector3(.38f,.035f,.52f),screen);
            AddInteractable(tablet,"Open workshop management",12,()=>game.UI?.OpenTab("PROGRESS"));
        }

        private void BuildServiceStations()
        {
            Station("StoreTerminal",new Vector3(-5.85f,1.45f,-.8f),new Color(.08f,.43f,.72f),"Open hardware store",()=>game.UI?.OpenTab("STORE"));
            Station("DiagnosticsTerminal",new Vector3(-4.0f,1.45f,3.25f),new Color(.14f,.64f,.42f),"Open diagnostics",()=>game.UI?.OpenTab("DIAG"));
            Station("FirmwareTerminal",new Vector3(2.9f,1.45f,3.30f),new Color(.45f,.30f,.75f),"Open BIOS / UEFI",()=>game.UI?.OpenTab("BIOS"));
            Station("SoftwareTerminal",new Vector3(4.25f,1.45f,3.30f),new Color(.20f,.48f,.82f),"Open operating system tools",()=>game.UI?.OpenTab("OS"));
            Station("JobBoardTerminal",new Vector3(5.85f,1.45f,-.8f),new Color(.78f,.42f,.10f),"Open job board",()=>game.UI?.OpenTab("JOBS"));
        }

        private void Station(string name,Vector3 pos,Color color,string label,Action action)
        {
            Material dark=Mat(new Color(.06f,.07f,.08f),.42f,.72f), screen=Mat(color,.65f,.12f);
            Box(name+"Base",pos-new Vector3(0,.65f,0),new Vector3(.72f,1.05f,.55f),dark);
            GameObject display=Box(name,pos,new Vector3(.84f,.55f,.10f),screen);
            display.transform.rotation=Quaternion.Euler(-8,0,0);
            AddInteractable(display,label,20,action);
        }

        private void BuildCeilingLights()
        {
            Material fixture=Mat(new Color(.72f,.75f,.78f),.75f,.5f);
            Material emissive=Mat(new Color(.85f,.94f,1f),.85f,.05f);
            if(emissive.HasProperty("_EmissionColor")){emissive.EnableKeyword("_EMISSION");emissive.SetColor("_EmissionColor",new Color(.75f,.90f,1f)*2.2f);}
            for(int z=-4;z<=4;z+=4) for(int x=-4;x<=4;x+=4)
            {
                Box("CeilingFixture",new Vector3(x,4.72f,z),new Vector3(1.65f,.08f,.42f),fixture);
                Box("CeilingEmitter",new Vector3(x,4.66f,z),new Vector3(1.45f,.025f,.30f),emissive);
            }
        }

        private void BuildSignage()
        {
            Material blue=Mat(new Color(.035f,.48f,.78f),.6f,.2f);
            Box("ForgeBenchLogoPlate",new Vector3(0,3.93f,-5.78f),new Vector3(3.2f,.56f,.08f),blue);
            CreateWorldLabel("FORGEBENCH",new Vector3(0,3.91f,-5.68f),38,new Vector2(3.0f,.5f));
            CreateWorldLabel("ASSEMBLY",new Vector3(0,4.15f,5.62f),24,new Vector2(2.2f,.35f));
            CreateWorldLabel("DIAGNOSTICS",new Vector3(-4,4.15f,5.62f),24,new Vector2(2.2f,.35f));
            CreateWorldLabel("BOARD REPAIR",new Vector3(4,4.15f,5.62f),24,new Vector2(2.2f,.35f));
        }

        private void CreateWorldLabel(string text,Vector3 position,int size,Vector2 rect)
        {
            GameObject go=new GameObject("Label_"+text,typeof(RectTransform),typeof(Canvas));go.transform.SetParent(root.transform,false);go.transform.position=position;go.transform.rotation=Quaternion.Euler(0,180,0);
            Canvas c=go.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;c.sortingOrder=2;RectTransform rt=go.GetComponent<RectTransform>();rt.sizeDelta=rect;go.transform.localScale=Vector3.one*.01f;
            GameObject tg=new GameObject("Text",typeof(RectTransform),typeof(UnityEngine.UI.Text));tg.transform.SetParent(go.transform,false);RectTransform tr=tg.GetComponent<RectTransform>();tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;
            UnityEngine.UI.Text t=tg.GetComponent<UnityEngine.UI.Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=text;t.alignment=TextAnchor.MiddleCenter;t.fontSize=size;t.color=Color.white;
        }

        private GameObject Box(string name,Vector3 pos,Vector3 scale,Material mat)
        {
            GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(root.transform,true);g.transform.position=pos;g.transform.localScale=scale;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;return g;
        }
        private GameObject Cylinder(string name,Vector3 pos,Vector3 scale,Quaternion rot,Material mat)
        {
            GameObject g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=name;g.transform.SetParent(root.transform,true);g.transform.position=pos;g.transform.localScale=scale;g.transform.rotation=rot;Renderer r=g.GetComponent<Renderer>();if(r!=null)r.sharedMaterial=mat;return g;
        }
        private Material Mat(Color c,float smooth,float metallic=0f){Material m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth);if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic);materials.Add(m);return m;}
        private static void AddInteractable(GameObject go,string label,int priority,Action action){WorldInteractable i=go.GetComponent<WorldInteractable>()??go.AddComponent<WorldInteractable>();i.label=label;i.priority=priority;i.action=action;}
        private void OnDestroy(){foreach(Material m in materials)if(m!=null)Destroy(m);}
    }
}
