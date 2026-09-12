using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForgeBench
{
    public static class MobileInputState
    {
        public static Vector2 Move;
        public static Vector2 Look;
        public static bool Precision;
        public static bool InteractPressed;
    }

    public sealed class WorldInteractable : MonoBehaviour
    {
        public string label;
        public int priority;
        public float maxDistance = 3.2f;
        public Action action;
        public void Invoke() { action?.Invoke(); }
    }

    public sealed class FirstPersonController : MonoBehaviour
    {
        public Camera viewCamera;
        public Action<string> FocusChanged;
        private CharacterController controller;
        private float pitch;
        private float rayTimer;
        private WorldInteractable focused;

        public void Initialize(Camera cam)
        {
            viewCamera = cam;
            controller = GetComponent<CharacterController>();
            if (controller == null) controller = gameObject.AddComponent<CharacterController>();
            controller.height = 1.72f; controller.radius = .28f; controller.center = new Vector3(0, .86f, 0);
        }

        private void Update()
        {
            if (viewCamera == null || controller == null) return;
            float dt = Time.unscaledDeltaTime;
            Vector2 move = MobileInputState.Move;
            if (move.sqrMagnitude < .01f)
            {
                move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                move = Vector2.ClampMagnitude(move, 1f);
            }
            bool precision = MobileInputState.Precision || Input.GetKey(KeyCode.LeftControl);
            float speed = precision ? 1.55f : (Input.GetKey(KeyCode.LeftShift) ? 5.2f : 3.3f);
            Vector3 desired = transform.right * move.x + transform.forward * move.y;
            controller.Move(desired * speed * dt + Physics.gravity * dt * .25f);

            Vector2 look = MobileInputState.Look;
            MobileInputState.Look = Vector2.zero;
            if (look.sqrMagnitude < .0001f && Cursor.lockState == CursorLockMode.Locked)
                look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 4f;
            if (look.sqrMagnitude < .0001f)
                look = new Vector2(Input.GetAxis("LookHorizontal"), Input.GetAxis("LookVertical")) * 2.4f;
            float sensitivity = GameRuntime.Instance?.State?.settings?.lookSensitivity ?? 1f;
            transform.Rotate(0, look.x * sensitivity, 0);
            pitch = Mathf.Clamp(pitch - look.y * sensitivity, -82f, 82f);
            viewCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);

            if (Input.GetMouseButtonDown(0) && !Application.isMobilePlatform) Cursor.lockState = CursorLockMode.Locked;
            if (Input.GetKeyDown(KeyCode.Escape)) Cursor.lockState = CursorLockMode.None;

            rayTimer -= dt;
            if (rayTimer <= 0f) { rayTimer = .1f; ScanFocus(); }
            if (Input.GetKeyDown(KeyCode.E) || MobileInputState.InteractPressed)
            {
                MobileInputState.InteractPressed = false;
                if (focused != null) focused.Invoke();
            }
        }

        private void ScanFocus()
        {
            WorldInteractable next = null;
            RaycastHit[] hits = Physics.RaycastAll(viewCamera.transform.position, viewCamera.transform.forward, 3.5f);
            float bestScore = float.MinValue;
            foreach (RaycastHit hit in hits)
            {
                WorldInteractable i = hit.collider.GetComponentInParent<WorldInteractable>();
                if (i == null || hit.distance > i.maxDistance) continue;
                float score = i.priority * 10f - hit.distance;
                if (score > bestScore) { bestScore = score; next = i; }
            }
            if (next != focused)
            {
                focused = next;
                FocusChanged?.Invoke(focused == null ? string.Empty : focused.label);
            }
        }

        public void InteractFocused() { focused?.Invoke(); }
    }

    public sealed class MachineRgbVisual : MonoBehaviour
    {
        private Material target; private MachineState machine; private Color baseColor; private float nextTick;
        public void Initialize(Material material, MachineState state, Color color){target=material;machine=state;baseColor=color;Apply(color);}
        private void Update()
        {
            if(target==null||machine==null||Time.unscaledTime<nextTick)return;nextTick=Time.unscaledTime+.05f;Color c=baseColor;int fx=GameRuntime.Instance?.State?.settings?.reducedMotion==true?0:(machine.customization?.rgbEffect??0);
            if(fx==1){float k=.55f+.45f*(Mathf.Sin(Time.unscaledTime*2.6f)*.5f+.5f);c=baseColor*k;}
            else if(fx==2){Color.RGBToHSV(baseColor,out float h,out float sat,out float val);c=Color.HSVToRGB(Mathf.Repeat(h+Time.unscaledTime*.08f,1f),Mathf.Max(.7f,sat),Mathf.Max(.8f,val));}
            Apply(c);
        }
        private void Apply(Color c){target.color=c;if(target.HasProperty("_EmissionColor")){target.EnableKeyword("_EMISSION");target.SetColor("_EmissionColor",c*2.1f);}}
    }

    public sealed class WorkshopWorld : MonoBehaviour
    {
        private GameRuntime game;
        private Transform machineRoot;
        private FirstPersonController player;
        private readonly List<Material> materials = new List<Material>();
        private Shader litShader;
        private bool built;

        public void Build()
        {
            if (built) return;
            built = true;
            game = GameRuntime.Instance;
            litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");
            BuildRoom();
            BuildPlayer();
            RefreshMachine();
        }

        private void BuildRoom()
        {
            Material floor = Mat(new Color(.10f, .115f, .13f), .65f);
            Material wall = Mat(new Color(.18f, .20f, .22f), .7f);
            Material bench = Mat(new Color(.24f, .19f, .15f), .55f);
            Material metal = Mat(new Color(.19f, .22f, .25f), .35f, .65f);
            Material accent = Mat(new Color(.12f, .55f, .78f), .35f, .25f);

            Cube("Floor", new Vector3(0,-.08f,0), new Vector3(14,.15f,12), floor, null);
            Cube("BackWall", new Vector3(0,2.5f,5.9f), new Vector3(14,5,.2f), wall, null);
            Cube("LeftWall", new Vector3(-6.9f,2.5f,0), new Vector3(.2f,5,12), wall, null);
            Cube("RightWall", new Vector3(6.9f,2.5f,0), new Vector3(.2f,5,12), wall, null);

            CreateBench("Main Assembly Bench", new Vector3(0,.45f,3.9f), bench, metal, () => game.UI?.OpenTab("BENCH"));
            CreateBench("Diagnostic Bench", new Vector3(-4,.45f,3.9f), bench, metal, () => game.UI?.OpenTab("DIAG"));
            CreateBench("Board Repair Station", new Vector3(4,.45f,3.9f), bench, metal, () => game.Notify(game.State.workshop.boardRepairLevel > 0 ? "Board-repair station ready." : "Unlock at workshop level 3.", game.State.workshop.boardRepairLevel > 0));
            CreateBench("Packaging Station", new Vector3(4,.45f,-3.9f), bench, metal, () => game.UI?.OpenTab("JOBS"));
            CreateShelf(new Vector3(-5.4f,1.1f,-3.8f), metal, accent);

            GameObject receiving = Cube("ReceivingArea", new Vector3(-2.8f,.06f,-4.5f), new Vector3(2.2f,.1f,1.7f), accent, null);
            AddInteractable(receiving, "Receive delivered packages", 5, () => game.ReceiveAll());
            GameObject tabletDock = Cube("TabletDock", new Vector3(2.2f,1.25f,4.15f), new Vector3(.45f,.3f,.08f), accent, null);
            AddInteractable(tabletDock, "Open workshop tablet", 10, () => game.UI?.ToggleTablet(true));

            GameObject lightGo = new GameObject("Sun");
            Light sun = lightGo.AddComponent<Light>(); sun.type = LightType.Directional; sun.intensity = 1.15f; sun.shadows = LightShadows.Soft;
            lightGo.transform.rotation = Quaternion.Euler(48,-35,0);
            for (int x=-4; x<=4; x+=4)
            {
                GameObject lamp = new GameObject("CeilingLamp"); lamp.transform.position = new Vector3(x,4.2f,0);
                Light l = lamp.AddComponent<Light>(); l.type = LightType.Point; l.range = 8; l.intensity = 8f; l.shadows = LightShadows.None;
            }
        }

        private void CreateBench(string name, Vector3 pos, Material wood, Material metal, Action action)
        {
            GameObject root = new GameObject(name); root.transform.position = pos;
            Cube("Top", pos + new Vector3(0,.55f,0), new Vector3(2.6f,.16f,1.25f), wood, root.transform);
            foreach (float x in new[]{-1.08f,1.08f}) foreach (float z in new[]{-.46f,.46f})
                Cube("Leg", pos + new Vector3(x,-.05f,z), new Vector3(.12f,1.1f,.12f), metal, root.transform);
            BoxCollider col = root.AddComponent<BoxCollider>(); col.center = new Vector3(0,.5f,0); col.size = new Vector3(2.8f,1.4f,1.5f);
            AddInteractable(root, name + " — interact", 4, action);
        }

        private void CreateShelf(Vector3 pos, Material metal, Material accent)
        {
            GameObject root = new GameObject("WarehouseShelf"); root.transform.position = pos;
            foreach (float x in new[]{-1.1f,1.1f}) foreach (float z in new[]{-.35f,.35f}) Cube("Post", pos+new Vector3(x,.3f,z), new Vector3(.08f,2.7f,.08f), metal, root.transform);
            for (int i=0;i<4;i++) Cube("Shelf", pos+new Vector3(0,-.8f+i*.72f,0), new Vector3(2.3f,.08f,.82f), metal, root.transform);
            for (int i=0;i<5;i++) Cube("Box", pos+new Vector3(-.8f+i*.4f,-.45f+(i%3)*.72f,0), new Vector3(.32f,.28f,.55f), accent, root.transform);
            BoxCollider col = root.AddComponent<BoxCollider>(); col.size = new Vector3(2.5f,2.9f,1f);
            AddInteractable(root, "Open inventory", 3, () => game.UI?.OpenTab("INVENTORY"));
        }

        private void BuildPlayer()
        {
            GameObject p = new GameObject("Player"); p.transform.position = new Vector3(0,.15f,-1.8f);
            Camera cam = new GameObject("PlayerCamera").AddComponent<Camera>(); cam.transform.SetParent(p.transform); cam.transform.localPosition = new Vector3(0,1.55f,0); cam.fieldOfView = 68f; cam.nearClipPlane=.04f;
            cam.gameObject.AddComponent<AudioListener>();
            player = p.AddComponent<FirstPersonController>(); player.Initialize(cam);
            player.FocusChanged += label => game.UI?.SetWorldPrompt(label);
        }

        public void InteractFocused() => player?.InteractFocused();

        public void RefreshMachine()
        {
            if (!built) return;
            if (machineRoot != null) Destroy(machineRoot.gameObject);
            MachineState m = game.ActiveMachine;
            if (m == null) return;
            GameObject root = new GameObject("ActiveMachine3D"); machineRoot = root.transform; root.transform.position = new Vector3(0,1.32f,3.8f);
            Color theme=game.Customization.CurrentColor(m);
            Material caseMat=Mat(Color.Lerp(new Color(.08f,.09f,.105f),theme,.10f),.28f,.75f), boardMat=Mat(new Color(.06f,.25f,.15f),.5f,.3f), gpuMat=Mat(new Color(.16f,.17f,.20f),.4f,.65f), ramMat=Mat(new Color(.12f,.42f,.62f),.38f,.4f), copper=Mat(new Color(.63f,.30f,.12f),.38f,.6f), psu=Mat(new Color(.11f,.11f,.12f),.45f,.75f);
            if (!string.IsNullOrEmpty(m.caseItemId))
            {
                // Open-frame case geometry keeps internal components visible.
                Cube("CaseBase", root.transform.position + new Vector3(0,-.33f,0), new Vector3(.75f,.05f,.46f), caseMat, root.transform);
                Cube("CaseTop", root.transform.position + new Vector3(0,.34f,0), new Vector3(.75f,.05f,.46f), caseMat, root.transform);
                foreach(float x in new[]{-.36f,.36f}) foreach(float z in new[]{-.21f,.21f}) Cube("CasePost",root.transform.position+new Vector3(x,0,z),new Vector3(.04f,.68f,.04f),caseMat,root.transform);
                game.Assembly.EnsureCaseHardware(m);
                if(m.sidePanelInstalled)
                {
                    Material panelMat=Mat(Color.Lerp(new Color(.10f,.11f,.13f),theme,.18f),.42f,.7f);
                    Cube("SidePanel",root.transform.position+new Vector3(.385f,0,0),new Vector3(.025f,.64f,.43f),panelMat,root.transform);
                    for(int q=0;q<m.sidePanel.fasteners.Count;q++)
                    {
                        FastenerState f=m.sidePanel.fasteners[q];float sy = q < 2 ? .25f : -.25f; float sz = (q % 2 == 0) ? .18f : -.18f;
                        Material sm=Mat(Color.Lerp(new Color(.78f,.18f,.10f),new Color(.72f,.75f,.78f),Mathf.Clamp01(f.tightness)),.68f,.75f);
                        Cube("Fastener_"+f.fastenerId,root.transform.position+new Vector3(.405f,sy,sz),new Vector3(.022f,.035f,.035f),sm,root.transform);
                    }
                }
            }
            if (!string.IsNullOrEmpty(m.motherboardItemId)) Cube("Motherboard",root.transform.position+new Vector3(0,.02f,.19f),new Vector3(.56f,.54f,.025f),boardMat,root.transform);
            if (!string.IsNullOrEmpty(m.cpuItemId)) Cube("CPU",root.transform.position+new Vector3(0,.12f,.16f),new Vector3(.12f,.12f,.035f),copper,root.transform);
            if (!string.IsNullOrEmpty(m.coolerItemId)) Cube("Cooler",root.transform.position+new Vector3(0,.12f,.02f),new Vector3(.25f,.27f,.22f),caseMat,root.transform);
            for (int i=0;i<m.ramItemIds.Count;i++) Cube("RAM",root.transform.position+new Vector3(.16f+i*.045f,.08f,.13f),new Vector3(.028f,.29f,.06f),ramMat,root.transform);
            if (!string.IsNullOrEmpty(m.gpuItemId)) Cube("GPU",root.transform.position+new Vector3(0,-.13f,.01f),new Vector3(.55f,.10f,.16f),gpuMat,root.transform);
            if (!string.IsNullOrEmpty(m.psuItemId)) Cube("PSU",root.transform.position+new Vector3(.20f,-.22f,-.12f),new Vector3(.28f,.19f,.28f),psu,root.transform);
            for(int i=0;i<m.storageItemIds.Count;i++) Cube("Drive",root.transform.position+new Vector3(-.25f,-.23f+i*.08f,-.12f),new Vector3(.20f,.055f,.14f),gpuMat,root.transform);
            if(m.cables.atx24||m.cables.cpuEps||m.cables.gpuPower)
            {
                Color[] cablePalette={new Color(.06f,.06f,.07f),new Color(.82f,.12f,.10f),new Color(.15f,.38f,.85f),new Color(.85f,.82f,.73f),new Color(.45f,.16f,.65f)};
                Color cc=cablePalette[Mathf.Clamp(m.customization?.cableColorIndex??0,0,cablePalette.Length-1)];Material cm=Mat(cc,.28f,.1f);
                Cube("ATX_Cable",root.transform.position+new Vector3(-.23f,.03f,-.03f),new Vector3(.035f,.40f,.035f),cm,root.transform);
                Cube("GPU_Cable",root.transform.position+new Vector3(.20f,-.06f,-.06f),new Vector3(.035f,.28f,.035f),cm,root.transform);
            }
            if(game.Customization.HasRgbDevice(m))
            {
                Material rgb=Mat(theme,.45f,.15f);Cube("RgbStripTop",root.transform.position+new Vector3(0,.30f,-.215f),new Vector3(.62f,.018f,.018f),rgb,root.transform);Cube("RgbStripFront",root.transform.position+new Vector3(-.34f,0,-.215f),new Vector3(.018f,.55f,.018f),rgb,root.transform);
                root.AddComponent<MachineRgbVisual>().Initialize(rgb,m,theme);
            }
            BoxCollider col = root.AddComponent<BoxCollider>(); col.size = new Vector3(.9f,.9f,.7f);
            AddInteractable(root,"Inspect PC on main bench",9,()=>game.UI?.OpenTab("BENCH"));
        }

        private GameObject Cube(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name=name; go.transform.position=position; go.transform.localScale=scale; if(parent!=null) go.transform.SetParent(parent,true);
            Renderer r=go.GetComponent<Renderer>(); if(r!=null) r.sharedMaterial=material; return go;
        }

        private Material Mat(Color color, float smooth, float metallic=0f)
        {
            Material m = new Material(litShader); m.color=color; if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",smooth); if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",metallic); materials.Add(m); return m;
        }

        private static void AddInteractable(GameObject go,string label,int priority,Action action)
        {
            WorldInteractable i=go.AddComponent<WorldInteractable>(); i.label=label; i.priority=priority; i.action=action;
        }

        private void OnDestroy() { foreach(Material m in materials) if(m!=null) Destroy(m); }
    }
}
