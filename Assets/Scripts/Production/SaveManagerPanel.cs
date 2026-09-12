using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>Explicit UI for all three atomic save slots and backup-aware loading.</summary>
    public sealed class SaveManagerPanel : MonoBehaviour
    {
        private static SaveManagerPanel instance;
        private GameObject root;
        private RectTransform content;
        private Font font;

        public static void Open()
        {
            if(instance==null){GameObject go=new GameObject("ForgeBench_SaveManager");DontDestroyOnLoad(go);instance=go.AddComponent<SaveManagerPanel>();}
            instance.Show();
        }
        public static void RefreshIfOpen(){if(instance!=null&&instance.root!=null&&instance.root.activeSelf)instance.Refresh();}

        private void Awake(){if(instance!=null&&instance!=this){Destroy(gameObject);return;}instance=this;DontDestroyOnLoad(gameObject);font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");EnsureEventSystem();Build();root.SetActive(false);}
        private void Update(){if(root!=null&&root.activeSelf&&Input.GetKeyDown(KeyCode.Escape)&&!Application.isMobilePlatform)Close();}
        private void Show(){root.SetActive(true);Cursor.lockState=CursorLockMode.None;MobileInputState.Move=Vector2.zero;Refresh();}
        private void Close(){root.SetActive(false);Cursor.lockState=CursorLockMode.Locked;}

        private void Build()
        {
            root=new GameObject("SaveManagerRoot",typeof(RectTransform));root.transform.SetParent(transform,false);Canvas c=root.AddComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;c.sortingOrder=25580;CanvasScaler sc=root.AddComponent<CanvasScaler>();sc.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;sc.referenceResolution=new Vector2(1920,1080);sc.matchWidthOrHeight=.5f;root.AddComponent<GraphicRaycaster>();
            GameObject bg=UI("BG",root.transform);Stretch(bg.GetComponent<RectTransform>());bg.AddComponent<Image>().color=new Color(.011f,.017f,.024f,.997f);GameObject panel=UI("Panel",bg.transform);RectTransform pr=panel.GetComponent<RectTransform>();pr.anchorMin=new Vector2(.15f,.12f);pr.anchorMax=new Vector2(.85f,.88f);pr.offsetMin=pr.offsetMax=Vector2.zero;panel.AddComponent<Image>().color=new Color(.052f,.065f,.083f,1f);
            Text title=Text("SAVE / RECOVERY MANAGER",panel.transform,31,TextAnchor.MiddleLeft,Color.white);RectTransform tr=title.rectTransform;tr.anchorMin=new Vector2(.04f,.87f);tr.anchorMax=new Vector2(.72f,.97f);tr.offsetMin=tr.offsetMax=Vector2.zero;Button close=Button("✕",panel.transform,new Color(.55f,.11f,.13f,1f),Close);RectTransform xr=close.GetComponent<RectTransform>();xr.anchorMin=new Vector2(.90f,.89f);xr.anchorMax=new Vector2(.96f,.96f);xr.offsetMin=xr.offsetMax=Vector2.zero;
            Text info=Text("Three independent slots use versioned JSON, temporary-file atomic writes and .bak recovery. Loading replaces the current workshop state; save important changes first.",panel.transform,16,TextAnchor.UpperLeft,new Color(.65f,.73f,.80f,1f));RectTransform ir=info.rectTransform;ir.anchorMin=new Vector2(.04f,.75f);ir.anchorMax=new Vector2(.96f,.86f);ir.offsetMin=ir.offsetMax=Vector2.zero;
            content=UI("Slots",panel.transform).GetComponent<RectTransform>();content.anchorMin=new Vector2(.04f,.08f);content.anchorMax=new Vector2(.96f,.73f);content.offsetMin=content.offsetMax=Vector2.zero;VerticalLayoutGroup vg=content.gameObject.AddComponent<VerticalLayoutGroup>();vg.spacing=12;vg.childControlHeight=true;vg.childForceExpandHeight=true;vg.childControlWidth=true;vg.childForceExpandWidth=true;
        }

        private void Refresh()
        {
            GameRuntime g=GameRuntime.Instance;if(g==null||g.State==null)return;for(int i=content.childCount-1;i>=0;i--)Destroy(content.GetChild(i).gameObject);
            for(int slot=1;slot<=3;slot++)CreateSlot(slot,g.Saves.HasSave(slot));
        }

        private void CreateSlot(int slot,bool exists)
        {
            GameRuntime g=GameRuntime.Instance;GameObject card=UI("Slot"+slot,content);card.AddComponent<Image>().color=new Color(.082f,.101f,.129f,1f);HorizontalLayoutGroup h=card.AddComponent<HorizontalLayoutGroup>();h.padding=new RectOffset(18,14,14,14);h.spacing=12;h.childControlHeight=true;h.childForceExpandHeight=true;LayoutElement le=card.AddComponent<LayoutElement>();le.preferredHeight=120;
            GameObject text=UI("Info",card.transform);VerticalLayoutGroup v=text.AddComponent<VerticalLayoutGroup>();v.spacing=4;v.childForceExpandHeight=false;text.AddComponent<LayoutElement>().flexibleWidth=1;Text a=Text("SLOT "+slot,text.transform,23,TextAnchor.MiddleLeft,exists?new Color(.20f,.76f,.47f,1f):new Color(.59f,.66f,.73f,1f));a.fontStyle=FontStyle.Bold;Text b=Text(exists?"Save or backup present · backup recovery is automatic if primary JSON is invalid":"Empty slot",text.transform,15,TextAnchor.MiddleLeft,new Color(.64f,.71f,.78f,1f));
            int captured=slot;Button save=Button("SAVE",card.transform,new Color(.10f,.56f,.36f,1f),()=>{g.Save(captured);Refresh();});save.gameObject.AddComponent<LayoutElement>().preferredWidth=155;Button load=Button(exists?"LOAD":"NO SAVE",card.transform,exists?new Color(.10f,.44f,.68f,1f):new Color(.28f,.31f,.35f,1f),()=>{if(g.Saves.HasSave(captured)){g.Load(captured);Refresh();}});load.interactable=exists;load.gameObject.AddComponent<LayoutElement>().preferredWidth=155;
        }

        private Button Button(string label,Transform p,Color color,Action action){GameObject g=UI("Button_"+label,p);Image im=g.AddComponent<Image>();im.color=color;Button b=g.AddComponent<Button>();b.targetGraphic=im;if(action!=null)b.onClick.AddListener(()=>action());Text t=Text(label,g.transform,16,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,5);t.raycastTarget=false;return b;}private Text Text(string value,Transform p,int size,TextAnchor align,Color color){GameObject g=UI("Text",p);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=color;t.raycastTarget=false;return t;}private static GameObject UI(string n,Transform p){GameObject g=new GameObject(n,typeof(RectTransform));g.transform.SetParent(p,false);return g;}private static void Stretch(RectTransform r,float i=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(i,i);r.offsetMax=new Vector2(-i,-i);}private static void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(e);}
    }
}
