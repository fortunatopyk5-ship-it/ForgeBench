using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    public sealed class ManualArticle
    {
        public string id,title,category,body;
        public string[] tags;
    }

    /// <summary>
    /// Searchable offline workshop manual. Content explains the actual simulation rules
    /// and derives a context article from the live contract instead of displaying generic hints.
    /// </summary>
    public sealed class InGameManualPanel : MonoBehaviour
    {
        private static InGameManualPanel instance;
        private GameObject root;
        private RectTransform listRoot;
        private Text articleTitle,articleBody,contextText;
        private InputField search;
        private Font font;
        private readonly List<ManualArticle> articles=new List<ManualArticle>();

        public static void Open(){OpenArticle(null);}
        public static void OpenContext(){OpenArticle("context");}
        public static void OpenArticle(string id)
        {
            if(instance==null){GameObject go=new GameObject("ForgeBench_InGameManual");DontDestroyOnLoad(go);instance=go.AddComponent<InGameManualPanel>();}
            instance.Show(id);
        }

        private void Awake()
        {
            if(instance!=null&&instance!=this){Destroy(gameObject);return;}instance=this;DontDestroyOnLoad(gameObject);font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");EnsureEventSystem();BuildArticles();Build();root.SetActive(false);
        }
        private void Update(){if(root!=null&&root.activeSelf&&Input.GetKeyDown(KeyCode.Escape)&&!Application.isMobilePlatform)Close();}

        private void Show(string id)
        {
            root.SetActive(true);Cursor.lockState=CursorLockMode.None;MobileInputState.Move=Vector2.zero;RefreshList();
            if(id=="context")ShowContext();else Select(articles.FirstOrDefault(x=>x.id==id)??articles.First());
        }
        private void Close(){root.SetActive(false);Cursor.lockState=CursorLockMode.Locked;}

        private void BuildArticles()
        {
            articles.Add(A("start","First repair","Getting started","Accept one contract, inspect the device, diagnose before buying replacements, complete every mandatory criterion, run final preflight, then submit. The simulation never auto-fixes a failed requirement. Use the Operations / QA terminal to see blockers without changing device state.","job","repair","preflight"));
            articles.Add(A("assembly","PC assembly order","Assembly","Open the case before touching internals. Mount the board and CPU, respect socket/form-factor and RAM-generation compatibility, apply thermal interface material, install cooling and remaining parts, route cables, then restore the side panel and fastener torque before final acceptance.","case","cpu","ram","gpu","screws"));
            articles.Add(A("cables","Power and cable graph","Assembly","POST checks the persistent cable graph: 24-pin ATX, CPU EPS, front-panel and CPU fan are fundamental. High-power GPUs need auxiliary GPU power. SATA devices require power and data. Cable-management quality affects airflow and benchmark scoring.","atx","eps","sata","cables"));
            articles.Add(A("post","POST and boot codes","Diagnostics","Power-on is a diagnostic step. Missing board/CPU/RAM/PSU/cooling/cables, unstable memory, failed hardware, video-path issues and insufficient PSU headroom can stop POST. A0 is the normal successful code; resolve the actual cause rather than bypassing it.","post","boot","a0"));
            articles.Add(A("bios","BIOS / UEFI","Firmware","Use firmware for memory profiles, CPU power limits, fan behavior, virtualization/security and engineering tuning. Aggressive voltage/power settings can increase heat and long-term wear. Memory training must pass before delivery.","bios","uefi","memory","voltage"));
            articles.Add(A("os","ForgeOS and drivers","Software","A successful POST is required before OS work. Partition storage, install ForgeOS, then install the required drivers. Engineering diagnostics track OS integrity, updates, crashes and storage health; software jobs can fail final acceptance even when the hardware itself is healthy.","os","drivers","storage"));
            articles.Add(A("benchmark","Benchmark and stability","Diagnostics","Benchmark results are derived from the actual CPU/GPU/RAM/storage configuration and are modified by thermals, power quality, BIOS settings and cable management. Stress stability is a separate acceptance condition. Review memory errors, peak temperatures, PSU ripple and throttling, not only the final score.","benchmark","stress","thermal","power"));
            articles.Add(A("thermal","Thermals and airflow","Engineering","Dust, thermal paste, cooler capability, fan count/profile, case airflow and ambient conditions determine temperatures. Intake/exhaust balance and filters matter. High sustained temperatures can throttle performance and accelerate deterministic component wear.","thermal","airflow","fan","dust"));
            articles.Add(A("power","Power engineering","Engineering","PSU capacity is only one part of power quality. Engineering tools track rail voltage, headroom, transient load, efficiency, over-current protection and ripple. A system can have enough nominal wattage and still fail reliability acceptance if its simulated power quality is poor.","psu","power","ripple"));
            articles.Add(A("storage","Storage and SMART","Engineering","Storage has persistent health, read/write workload and fault state. Use SMART-like diagnostics before replacing a drive. Preserve customer data during upgrades; avoid treating a software or cable symptom as a failed disk without evidence.","ssd","nvme","sata","smart"));
            articles.Add(A("liquid","Liquid cooling","Specialist","Custom-loop work is isolated from ordinary desktop validation. Install pump/reservoir/radiator/fittings, tighten every fitting, fill coolant, bleed air and perform the isolated leak test before powered validation. Final flow and air fraction must remain within acceptance limits.","liquid","pump","radiator","leak"));
            articles.Add(A("board","Board-level repair","Specialist","Board repair uses a safe abstracted workflow: establish ESD protection, inspect under the microscope, measure rails, localize the short, apply flux/rework and verify the result. Excess rework and pad damage reduce repair quality and can block delivery.","board","solder","esd","meter"));
            articles.Add(A("portable","Laptop / phone / controller","Specialist","Portable devices have screw count, enclosure state, battery isolation, display/charging health, adhesive/seal quality and controller-drift state. Disconnect battery power before sensitive work. Reassembly and seal quality are explicit delivery requirements.","laptop","phone","controller","battery"));
            articles.Add(A("network","NAS / server networking","Specialist","Network contracts track physical link, DHCP/static addressing, throughput, packet loss, latency and RAID health. NAS recovery requires the array rebuilt/scrubbed; server commissioning requires suitable static addressing and validated network quality.","nas","server","raid","network"));
            articles.Add(A("inventory","Inventory and logistics","Business","Every hardware item is an individual instance with condition, ownership, reservation and wear. Customer-owned parts are not ordinary stock. Orders consume cash before arrival, warehouse capacity includes inbound stock, and supplier mismatch shipments must be returned rather than silently received.","inventory","shipping","warehouse"));
            articles.Add(A("customers","Customers and warranty","Business","Customer trust, satisfaction and loyalty persist across jobs. Quality, deadlines and complaints influence future work. Completed repairs are monitored for deterministic comeback risk; warranty callbacks pay no revenue and the workshop absorbs modeled liability when resolving them.","customer","crm","warranty"));
            articles.Add(A("staff","Staff and progression","Business","Employees have role, skill, fatigue, morale, training and certifications. Role quality influences workshop operations. Technician XP, reputation, certifications and workshop upgrades gate progressively harder work and equipment.","staff","career","training"));
            articles.Add(A("mobile","Android controls and performance","Controls","ForgeBench is landscape/touch-first. Use the movement joystick, drag-look and interact button; keyboard/mouse and gamepad remain fallback inputs. Battery saver lowers target frame rate. The mobile quality governor responds to frame pressure, memory warnings and low battery without changing simulation rules.","android","touch","fps","battery"));
            articles.Add(A("save","Saving and recovery","System","Gameplay uses versioned save slots with temporary-file atomic writes and backup recovery. Important transitions autosave. Customer, staff and warranty databases are local/offline persistent layers. Never force-close during platform-level storage operations even though save writes are designed to recover safely.","save","backup","offline"));
        }
        private static ManualArticle A(string id,string title,string category,string body,params string[] tags)=>new ManualArticle{id=id,title=title,category=category,body=body,tags=tags};

        private void Build()
        {
            root=new GameObject("ManualRoot",typeof(RectTransform));root.transform.SetParent(transform,false);Canvas canvas=root.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=25600;CanvasScaler scaler=root.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=.5f;root.AddComponent<GraphicRaycaster>();
            GameObject bg=UI("BG",root.transform);Stretch(bg.GetComponent<RectTransform>());bg.AddComponent<Image>().color=new Color(.010f,.016f,.023f,.998f);
            GameObject panel=UI("Panel",bg.transform);RectTransform pr=panel.GetComponent<RectTransform>();pr.anchorMin=new Vector2(.035f,.035f);pr.anchorMax=new Vector2(.965f,.965f);pr.offsetMin=pr.offsetMax=Vector2.zero;panel.AddComponent<Image>().color=new Color(.048f,.060f,.077f,1f);
            Text title=Text("WORKSHOP MANUAL / HELP",panel.transform,30,TextAnchor.MiddleLeft,Color.white);RectTransform tr=title.rectTransform;tr.anchorMin=new Vector2(.02f,.91f);tr.anchorMax=new Vector2(.42f,.985f);tr.offsetMin=tr.offsetMax=Vector2.zero;
            search=Input(panel.transform,"Search manual");RectTransform sr=search.GetComponent<RectTransform>();sr.anchorMin=new Vector2(.43f,.922f);sr.anchorMax=new Vector2(.70f,.975f);sr.offsetMin=sr.offsetMax=Vector2.zero;search.onValueChanged.AddListener(_=>RefreshList());
            Button context=Button("CONTEXT HELP",panel.transform,new Color(.11f,.48f,.70f,1f),ShowContext);RectTransform cr=context.GetComponent<RectTransform>();cr.anchorMin=new Vector2(.715f,.922f);cr.anchorMax=new Vector2(.86f,.975f);cr.offsetMin=cr.offsetMax=Vector2.zero;
            Button close=Button("✕",panel.transform,new Color(.54f,.11f,.13f,1f),Close);RectTransform xr=close.GetComponent<RectTransform>();xr.anchorMin=new Vector2(.90f,.922f);xr.anchorMax=new Vector2(.965f,.975f);xr.offsetMin=xr.offsetMax=Vector2.zero;
            GameObject left=UI("ArticleList",panel.transform);RectTransform lr=left.GetComponent<RectTransform>();lr.anchorMin=new Vector2(.02f,.045f);lr.anchorMax=new Vector2(.32f,.90f);lr.offsetMin=lr.offsetMax=Vector2.zero;left.AddComponent<Image>().color=new Color(.025f,.033f,.043f,.80f);ScrollRect ls=left.AddComponent<ScrollRect>();ls.horizontal=false;GameObject lv=UI("Viewport",left.transform);Stretch(lv.GetComponent<RectTransform>());lv.AddComponent<Image>().color=new Color(0,0,0,.01f);Mask lm=lv.AddComponent<Mask>();lm.showMaskGraphic=false;GameObject list=UI("Content",lv.transform);listRoot=list.GetComponent<RectTransform>();listRoot.anchorMin=new Vector2(0,1);listRoot.anchorMax=new Vector2(1,1);listRoot.pivot=new Vector2(.5f,1);listRoot.sizeDelta=new Vector2(0,800);VerticalLayoutGroup lvg=list.AddComponent<VerticalLayoutGroup>();lvg.padding=new RectOffset(10,10,10,12);lvg.spacing=6;lvg.childControlHeight=true;lvg.childForceExpandHeight=false;lvg.childControlWidth=true;lvg.childForceExpandWidth=true;ContentSizeFitter lf=list.AddComponent<ContentSizeFitter>();lf.verticalFit=ContentSizeFitter.FitMode.PreferredSize;ls.viewport=lv.GetComponent<RectTransform>();ls.content=listRoot;
            GameObject right=UI("Article",panel.transform);RectTransform rr=right.GetComponent<RectTransform>();rr.anchorMin=new Vector2(.335f,.045f);rr.anchorMax=new Vector2(.98f,.90f);rr.offsetMin=rr.offsetMax=Vector2.zero;right.AddComponent<Image>().color=new Color(.026f,.035f,.046f,.82f);articleTitle=Text("",right.transform,29,TextAnchor.UpperLeft,Color.white);RectTransform ar=articleTitle.rectTransform;ar.anchorMin=new Vector2(.04f,.82f);ar.anchorMax=new Vector2(.96f,.96f);ar.offsetMin=ar.offsetMax=Vector2.zero;articleTitle.fontStyle=FontStyle.Bold;articleBody=Text("",right.transform,19,TextAnchor.UpperLeft,new Color(.74f,.81f,.87f,1f));RectTransform br=articleBody.rectTransform;br.anchorMin=new Vector2(.04f,.20f);br.anchorMax=new Vector2(.96f,.81f);br.offsetMin=br.offsetMax=Vector2.zero;articleBody.horizontalOverflow=HorizontalWrapMode.Wrap;articleBody.verticalOverflow=VerticalWrapMode.Overflow;contextText=Text("",right.transform,16,TextAnchor.UpperLeft,new Color(.37f,.70f,.92f,1f));RectTransform kr=contextText.rectTransform;kr.anchorMin=new Vector2(.04f,.04f);kr.anchorMax=new Vector2(.96f,.19f);kr.offsetMin=kr.offsetMax=Vector2.zero;contextText.horizontalOverflow=HorizontalWrapMode.Wrap;
        }

        private void RefreshList()
        {
            if(listRoot==null)return;for(int i=listRoot.childCount-1;i>=0;i--)Destroy(listRoot.GetChild(i).gameObject);string q=(search?.text??"").Trim().ToLowerInvariant();IEnumerable<ManualArticle> rows=articles;if(q.Length>0)rows=rows.Where(a=>(a.title+" "+a.category+" "+a.body+" "+string.Join(" ",a.tags??Array.Empty<string>())).ToLowerInvariant().Contains(q));
            foreach(ManualArticle a in rows){ManualArticle pick=a;Button b=Button(a.category.ToUpperInvariant()+"\n"+a.title,listRoot,new Color(.075f,.095f,.122f,1f),()=>Select(pick));LayoutElement le=b.gameObject.AddComponent<LayoutElement>();le.preferredHeight=62;Text t=b.GetComponentInChildren<Text>();t.alignment=TextAnchor.MiddleLeft;t.fontSize=15;}
        }

        private void Select(ManualArticle a)
        {
            if(a==null)return;articleTitle.text=a.category.ToUpperInvariant()+" · "+a.title;articleBody.text=a.body;contextText.text=ContextFooter(a);
        }

        private void ShowContext()
        {
            GameRuntime g=GameRuntime.Instance;JobState j=g?.ActiveJob;MachineState m=g?.ActiveMachine;
            if(g==null||j==null){Select(articles.First(x=>x.id=="start"));contextText.text="Context: no active contract. Start from the Job Board.";return;}
            string id="start";
            if(SpecialistJobService.IsSpecialist(j))
            {
                string marker=j.requiredPartCategories.FirstOrDefault(x=>x.StartsWith("SPECIALIST:",StringComparison.Ordinal))??string.Empty;
                if(marker.Contains("Liquid"))id="liquid";else if(marker.Contains("Board"))id="board";else if(marker.Contains("Nas")||marker.Contains("Server"))id="network";else id="portable";
            }
            else if(m!=null)
            {
                if(!g.Assembly.InternalsAccessible(m))id="assembly";
                else if(!m.cables.atx24||!m.cables.cpuEps||!m.cables.frontPanel)id="cables";
                else if(m.postCode!="A0")id="post";
                else if(j.requireOs&&(!m.osInstalled||!m.driversInstalled))id="os";
                else if(j.requireStable&&(m.benchmarkScore<=0||!m.stressStable))id="benchmark";
                else id="start";
            }
            Select(articles.First(x=>x.id==id));contextText.text="Context: "+j.jobId+" · "+j.title+" · stage "+j.stage+" · due day "+j.dueDay+". This help does not modify job/device state.";
        }

        private static string ContextFooter(ManualArticle a)=>"Tags: "+string.Join(" · ",a.tags??Array.Empty<string>())+". Use CONTEXT HELP to match the manual to the live contract.";
        private InputField Input(Transform parent,string placeholder){GameObject g=UI("Search",parent);g.AddComponent<Image>().color=new Color(.10f,.12f,.15f,1f);InputField f=g.AddComponent<InputField>();Text t=Text("",g.transform,16,TextAnchor.MiddleLeft,Color.white);Stretch(t.rectTransform,10);Text ph=Text(placeholder,g.transform,16,TextAnchor.MiddleLeft,new Color(.48f,.55f,.62f,1f));Stretch(ph.rectTransform,10);f.textComponent=t;f.placeholder=ph;return f;}
        private Button Button(string label,Transform parent,Color color,Action action){GameObject g=UI("Button_"+label,parent);Image im=g.AddComponent<Image>();im.color=color;Button b=g.AddComponent<Button>();b.targetGraphic=im;if(action!=null)b.onClick.AddListener(()=>action());Text t=Text(label,g.transform,16,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,6);t.raycastTarget=false;return b;}
        private Text Text(string value,Transform parent,int size,TextAnchor align,Color color){GameObject g=UI("Text",parent);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=color;t.raycastTarget=false;return t;}
        private static GameObject UI(string n,Transform p){GameObject g=new GameObject(n,typeof(RectTransform));g.transform.SetParent(p,false);return g;}private static void Stretch(RectTransform r,float i=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(i,i);r.offsetMax=new Vector2(-i,-i);}private static void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(e);}
    }
}
