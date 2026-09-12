using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>
    /// Responsive pooled inventory browser. Only a fixed row pool is alive regardless
    /// of warehouse size, preventing large inventories from creating hundreds of uGUI objects.
    /// </summary>
    public sealed class VirtualizedInventoryPanel : MonoBehaviour
    {
        private static VirtualizedInventoryPanel instance;
        private GameObject root;
        private RectTransform listRoot;
        private Text summary;
        private InputField search;
        private Dropdown category;
        private Font font;
        private readonly List<RowView> rows = new List<RowView>();
        private List<ItemInstance> filtered = new List<ItemInstance>();
        private int page;
        private const int RowsPerPage = 12;

        private sealed class RowView
        {
            public GameObject root; public Text title; public Text detail; public Button action; public Text actionText; public string itemId;
        }

        public static void Open()
        {
            if(instance==null){GameObject go=new GameObject("ForgeBench_VirtualizedInventoryPanel");DontDestroyOnLoad(go);instance=go.AddComponent<VirtualizedInventoryPanel>();}
            instance.Show();
        }

        private void Awake(){if(instance!=null&&instance!=this){Destroy(gameObject);return;}instance=this;DontDestroyOnLoad(gameObject);font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");EnsureEventSystem();Build();root.SetActive(false);}
        private void Update(){if(root!=null&&root.activeSelf&&Input.GetKeyDown(KeyCode.Escape)&&!Application.isMobilePlatform)Close();}
        private void Show(){root.SetActive(true);Cursor.lockState=CursorLockMode.None;MobileInputState.Move=Vector2.zero;page=0;Refresh();}
        private void Close(){root.SetActive(false);Cursor.lockState=CursorLockMode.Locked;}

        private void Build()
        {
            root=new GameObject("VirtualInventoryRoot",typeof(RectTransform));root.transform.SetParent(transform,false);Canvas c=root.AddComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;c.sortingOrder=25370;CanvasScaler sc=root.AddComponent<CanvasScaler>();sc.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;sc.referenceResolution=new Vector2(1920,1080);sc.matchWidthOrHeight=.5f;root.AddComponent<GraphicRaycaster>();
            GameObject bg=UI("BG",root.transform);Stretch(bg.GetComponent<RectTransform>());bg.AddComponent<Image>().color=new Color(.014f,.020f,.029f,.995f);GameObject panel=UI("Panel",bg.transform);RectTransform pr=panel.GetComponent<RectTransform>();pr.anchorMin=new Vector2(.04f,.04f);pr.anchorMax=new Vector2(.96f,.96f);pr.offsetMin=pr.offsetMax=Vector2.zero;panel.AddComponent<Image>().color=new Color(.052f,.064f,.080f,1f);
            GameObject header=UI("Header",panel.transform);RectTransform hr=header.GetComponent<RectTransform>();hr.anchorMin=new Vector2(0,1);hr.anchorMax=new Vector2(1,1);hr.pivot=new Vector2(.5f,1);hr.sizeDelta=new Vector2(0,88);hr.anchoredPosition=Vector2.zero;header.AddComponent<Image>().color=new Color(.072f,.087f,.105f,1f);Text title=Text("WAREHOUSE INVENTORY",header.transform,30,TextAnchor.MiddleLeft,Color.white);RectTransform tr=title.rectTransform;tr.anchorMin=new Vector2(.018f,0);tr.anchorMax=new Vector2(.30f,1);tr.offsetMin=tr.offsetMax=Vector2.zero;
            search=Input(header.transform,"Search model / brand / ID");RectTransform sr=search.GetComponent<RectTransform>();sr.anchorMin=new Vector2(.31f,.18f);sr.anchorMax=new Vector2(.55f,.82f);sr.offsetMin=sr.offsetMax=Vector2.zero;search.onValueChanged.AddListener(_=>{page=0;Refresh();});
            category=DropdownField(header.transform);RectTransform dr=category.GetComponent<RectTransform>();dr.anchorMin=new Vector2(.57f,.18f);dr.anchorMax=new Vector2(.72f,.82f);dr.offsetMin=dr.offsetMax=Vector2.zero;category.options.Add(new Dropdown.OptionData("ALL"));foreach(string n in Enum.GetNames(typeof(PartCategory)))category.options.Add(new Dropdown.OptionData(n));category.value=0;category.onValueChanged.AddListener(_=>{page=0;Refresh();});
            summary=Text("",header.transform,17,TextAnchor.MiddleCenter,new Color(.65f,.73f,.80f,1f));RectTransform sm=summary.rectTransform;sm.anchorMin=new Vector2(.73f,0);sm.anchorMax=new Vector2(.91f,1);sm.offsetMin=sm.offsetMax=Vector2.zero;Button close=Button("✕",header.transform,new Color(.55f,.11f,.12f,1f),Close);RectTransform xr=close.GetComponent<RectTransform>();xr.anchorMin=new Vector2(.925f,.18f);xr.anchorMax=new Vector2(.982f,.82f);xr.offsetMin=xr.offsetMax=Vector2.zero;
            listRoot=UI("Rows",panel.transform).GetComponent<RectTransform>();listRoot.anchorMin=new Vector2(.02f,.09f);listRoot.anchorMax=new Vector2(.98f,.89f);listRoot.offsetMin=listRoot.offsetMax=Vector2.zero;VerticalLayoutGroup vg=listRoot.gameObject.AddComponent<VerticalLayoutGroup>();vg.spacing=7;vg.childControlHeight=true;vg.childForceExpandHeight=true;vg.childControlWidth=true;vg.childForceExpandWidth=true;
            for(int i=0;i<RowsPerPage;i++)rows.Add(CreateRow(listRoot));
            GameObject footer=UI("Footer",panel.transform);RectTransform fr=footer.GetComponent<RectTransform>();fr.anchorMin=new Vector2(.02f,.018f);fr.anchorMax=new Vector2(.98f,.082f);fr.offsetMin=fr.offsetMax=Vector2.zero;HorizontalLayoutGroup fh=footer.AddComponent<HorizontalLayoutGroup>();fh.spacing=12;fh.childControlHeight=true;fh.childForceExpandHeight=true;fh.childForceExpandWidth=true;Button prev=Button("◀ PREVIOUS",footer.transform,new Color(.13f,.28f,.40f,1f),()=>{page=Mathf.Max(0,page-1);RenderRows();});prev.gameObject.AddComponent<LayoutElement>().flexibleWidth=1;Button refresh=Button("REFRESH",footer.transform,new Color(.12f,.38f,.58f,1f),Refresh);refresh.gameObject.AddComponent<LayoutElement>().flexibleWidth=1;Button next=Button("NEXT ▶",footer.transform,new Color(.13f,.28f,.40f,1f),()=>{int max=Mathf.Max(0,(filtered.Count-1)/RowsPerPage);page=Mathf.Min(max,page+1);RenderRows();});next.gameObject.AddComponent<LayoutElement>().flexibleWidth=1;
        }

        private RowView CreateRow(Transform parent)
        {
            RowView r=new RowView();r.root=UI("PooledInventoryRow",parent);r.root.AddComponent<Image>().color=new Color(.09f,.11f,.14f,1f);HorizontalLayoutGroup h=r.root.AddComponent<HorizontalLayoutGroup>();h.padding=new RectOffset(14,10,7,7);h.spacing=10;h.childControlHeight=true;h.childForceExpandHeight=true;LayoutElement le=r.root.AddComponent<LayoutElement>();le.preferredHeight=65;
            GameObject box=UI("Info",r.root.transform);VerticalLayoutGroup v=box.AddComponent<VerticalLayoutGroup>();v.childForceExpandHeight=false;v.spacing=1;LayoutElement bl=box.AddComponent<LayoutElement>();bl.flexibleWidth=1;r.title=Text("",box.transform,19,TextAnchor.MiddleLeft,Color.white);r.title.fontStyle=FontStyle.Bold;r.detail=Text("",box.transform,15,TextAnchor.MiddleLeft,new Color(.62f,.70f,.78f,1f));r.action=Button("ACTION",r.root.transform,new Color(.10f,.45f,.66f,1f),()=>Act(r));LayoutElement al=r.action.gameObject.AddComponent<LayoutElement>();al.preferredWidth=155;r.actionText=r.action.GetComponentInChildren<Text>();return r;
        }

        private void Refresh()
        {
            GameRuntime g=GameRuntime.Instance;if(g==null||g.State==null)return;string q=(search?.text??"").Trim().ToLowerInvariant();int cat=category==null?0:category.value;
            filtered=g.State.inventory.Where(i=>{HardwareDefinition d=g.Inventory.Def(i);if(d==null)return false;if(cat>0&&(int)d.category!=cat-1)return false;if(q.Length==0)return true;string hay=(i.instanceId+" "+d.brand+" "+d.model+" "+d.category).ToLowerInvariant();return hay.Contains(q);}).OrderBy(i=>i.reserved).ThenBy(i=>g.Inventory.Def(i)?.category).ThenBy(i=>g.Inventory.Def(i)?.brand).ThenBy(i=>g.Inventory.Def(i)?.model).ToList();int max=Mathf.Max(0,(filtered.Count-1)/RowsPerPage);page=Mathf.Clamp(page,0,max);RenderRows();
        }

        private void RenderRows()
        {
            GameRuntime g=GameRuntime.Instance;if(g==null)return;int start=page*RowsPerPage;for(int n=0;n<rows.Count;n++){RowView r=rows[n];int idx=start+n;if(idx>=filtered.Count){r.root.SetActive(false);continue;}r.root.SetActive(true);ItemInstance item=filtered[idx];HardwareDefinition d=g.Inventory.Def(item);r.itemId=item.instanceId;r.title.text=item.instanceId+" · "+d.brand+" "+d.model;r.detail.text=d.category+" · "+(item.reserved?"INSTALLED/RESERVED":"AVAILABLE")+" · condition "+Mathf.RoundToInt(item.condition*100)+"% · fault "+item.fault+" · damage "+item.damage;r.actionText.text=item.reserved?"REMOVE":"INSTALL";Image im=r.action.GetComponent<Image>();if(im!=null)im.color=item.reserved?new Color(.45f,.24f,.12f,1f):new Color(.10f,.45f,.66f,1f);}int pages=Mathf.Max(1,(filtered.Count+RowsPerPage-1)/RowsPerPage);if(summary!=null)summary.text=filtered.Count+" MATCHES\nPAGE "+(page+1)+" / "+pages;
        }

        private void Act(RowView row){GameRuntime g=GameRuntime.Instance;if(g==null||string.IsNullOrEmpty(row.itemId))return;ItemInstance item=g.Inventory.Get(row.itemId);if(item==null)return;if(item.reserved)g.Remove(item.instanceId);else g.Install(item.instanceId);Refresh();}
        private InputField Input(Transform p,string placeholder){GameObject g=UI("Search",p);Image i=g.AddComponent<Image>();i.color=new Color(.11f,.13f,.16f,1f);InputField f=g.AddComponent<InputField>();GameObject textGo=UI("Text",g.transform);Text t=Text("",textGo.transform,17,TextAnchor.MiddleLeft,Color.white);Stretch(t.rectTransform,10);GameObject phGo=UI("Placeholder",g.transform);Text ph=Text(placeholder,phGo.transform,17,TextAnchor.MiddleLeft,new Color(.48f,.55f,.62f,1f));Stretch(ph.rectTransform,10);f.textComponent=t;f.placeholder=ph;return f;}
        private Dropdown DropdownField(Transform p){GameObject g=UI("Category",p);Image i=g.AddComponent<Image>();i.color=new Color(.11f,.13f,.16f,1f);Dropdown d=g.AddComponent<Dropdown>();Text label=Text("ALL",g.transform,16,TextAnchor.MiddleLeft,Color.white);Stretch(label.rectTransform,10);d.captionText=label;GameObject template=UI("Template",g.transform);RectTransform rt=template.GetComponent<RectTransform>();rt.anchorMin=new Vector2(0,0);rt.anchorMax=new Vector2(1,0);rt.pivot=new Vector2(.5f,1);rt.sizeDelta=new Vector2(0,280);template.AddComponent<Image>().color=new Color(.06f,.08f,.10f,1f);ScrollRect scroll=template.AddComponent<ScrollRect>();GameObject viewport=UI("Viewport",template.transform);Stretch(viewport.GetComponent<RectTransform>());viewport.AddComponent<Mask>().showMaskGraphic=false;viewport.AddComponent<Image>().color=new Color(0,0,0,.01f);GameObject content=UI("Content",viewport.transform);RectTransform cr=content.GetComponent<RectTransform>();cr.anchorMin=new Vector2(0,1);cr.anchorMax=new Vector2(1,1);cr.pivot=new Vector2(.5f,1);cr.sizeDelta=new Vector2(0,35);Toggle toggle=content.AddComponent<Toggle>();Text item=Text("Option",content.transform,15,TextAnchor.MiddleLeft,Color.white);Stretch(item.rectTransform,8);scroll.viewport=viewport.GetComponent<RectTransform>();scroll.content=cr;d.template=rt;d.itemText=item;template.SetActive(false);return d;}
        private Button Button(string label,Transform p,Color c,Action a){GameObject g=UI("Button_"+label,p);Image i=g.AddComponent<Image>();i.color=c;Button b=g.AddComponent<Button>();b.targetGraphic=i;if(a!=null)b.onClick.AddListener(()=>a());Text t=Text(label,g.transform,16,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,5);t.raycastTarget=false;return b;}private Text Text(string value,Transform p,int size,TextAnchor align,Color c){GameObject g=UI("Text",p);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=c;t.raycastTarget=false;return t;}private GameObject UI(string n,Transform p){GameObject g=new GameObject(n,typeof(RectTransform));g.transform.SetParent(p,false);return g;}private static void Stretch(RectTransform r,float i=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(i,i);r.offsetMax=new Vector2(-i,-i);}private static void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(e);}
    }
}
