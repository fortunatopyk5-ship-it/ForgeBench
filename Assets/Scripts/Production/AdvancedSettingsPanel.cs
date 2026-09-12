using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>
    /// Full settings surface for the persistent AccessibilitySettings model. It exposes
    /// FPS, touch sizing, look speed, text scale, audio, language, haptics, captions,
    /// reduced motion, color assist and battery saver instead of hiding those values in code.
    /// </summary>
    public sealed class AdvancedSettingsPanel : MonoBehaviour
    {
        private static AdvancedSettingsPanel instance;
        private GameObject root;
        private RectTransform content;
        private Text summary;
        private Font font;

        public static void Open()
        {
            if(instance==null){GameObject go=new GameObject("ForgeBench_AdvancedSettings");DontDestroyOnLoad(go);instance=go.AddComponent<AdvancedSettingsPanel>();}
            instance.Show();
        }
        public static void RefreshIfOpen(){if(instance!=null&&instance.root!=null&&instance.root.activeSelf)instance.Refresh();}

        private void Awake(){if(instance!=null&&instance!=this){Destroy(gameObject);return;}instance=this;DontDestroyOnLoad(gameObject);font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");EnsureEventSystem();Build();root.SetActive(false);}
        private void Update(){if(root!=null&&root.activeSelf&&Input.GetKeyDown(KeyCode.Escape)&&!Application.isMobilePlatform)Close();}
        private void Show(){root.SetActive(true);Cursor.lockState=CursorLockMode.None;MobileInputState.Move=Vector2.zero;Refresh();}
        private void Close(){root.SetActive(false);Cursor.lockState=CursorLockMode.Locked;}

        private void Build()
        {
            root=new GameObject("AdvancedSettingsRoot",typeof(RectTransform));root.transform.SetParent(transform,false);Canvas c=root.AddComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;c.sortingOrder=25620;CanvasScaler cs=root.AddComponent<CanvasScaler>();cs.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;cs.referenceResolution=new Vector2(1920,1080);cs.matchWidthOrHeight=.5f;root.AddComponent<GraphicRaycaster>();
            GameObject bg=UI("BG",root.transform);Stretch(bg.GetComponent<RectTransform>());bg.AddComponent<Image>().color=new Color(.010f,.016f,.024f,.998f);GameObject panel=UI("Panel",bg.transform);RectTransform pr=panel.GetComponent<RectTransform>();pr.anchorMin=new Vector2(.08f,.055f);pr.anchorMax=new Vector2(.92f,.945f);pr.offsetMin=pr.offsetMax=Vector2.zero;panel.AddComponent<Image>().color=new Color(.050f,.062f,.079f,1f);
            Text title=Text("SETTINGS / ANDROID / ACCESSIBILITY",panel.transform,31,TextAnchor.MiddleLeft,Color.white);RectTransform tr=title.rectTransform;tr.anchorMin=new Vector2(.035f,.90f);tr.anchorMax=new Vector2(.70f,.985f);tr.offsetMin=tr.offsetMax=Vector2.zero;summary=Text("",panel.transform,15,TextAnchor.MiddleRight,new Color(.63f,.72f,.80f,1f));RectTransform sr=summary.rectTransform;sr.anchorMin=new Vector2(.54f,.90f);sr.anchorMax=new Vector2(.88f,.985f);sr.offsetMin=sr.offsetMax=Vector2.zero;Button close=Button("✕",panel.transform,new Color(.55f,.11f,.13f,1f),Close);RectTransform xr=close.GetComponent<RectTransform>();xr.anchorMin=new Vector2(.91f,.915f);xr.anchorMax=new Vector2(.965f,.975f);xr.offsetMin=xr.offsetMax=Vector2.zero;
            GameObject scroll=UI("Scroll",panel.transform);RectTransform rr=scroll.GetComponent<RectTransform>();rr.anchorMin=new Vector2(.03f,.035f);rr.anchorMax=new Vector2(.97f,.89f);rr.offsetMin=rr.offsetMax=Vector2.zero;ScrollRect sc=scroll.AddComponent<ScrollRect>();sc.horizontal=false;sc.scrollSensitivity=44f;GameObject viewport=UI("Viewport",scroll.transform);Stretch(viewport.GetComponent<RectTransform>());viewport.AddComponent<Image>().color=new Color(.024f,.032f,.042f,.72f);Mask mask=viewport.AddComponent<Mask>();mask.showMaskGraphic=false;sc.viewport=viewport.GetComponent<RectTransform>();GameObject body=UI("Content",viewport.transform);content=body.GetComponent<RectTransform>();content.anchorMin=new Vector2(0,1);content.anchorMax=new Vector2(1,1);content.pivot=new Vector2(.5f,1);content.sizeDelta=new Vector2(0,800);VerticalLayoutGroup vg=body.AddComponent<VerticalLayoutGroup>();vg.padding=new RectOffset(18,18,16,22);vg.spacing=10;vg.childControlHeight=true;vg.childForceExpandHeight=false;vg.childControlWidth=true;vg.childForceExpandWidth=true;ContentSizeFitter fit=body.AddComponent<ContentSizeFitter>();fit.verticalFit=ContentSizeFitter.FitMode.PreferredSize;sc.content=content;
        }

        private void Refresh()
        {
            GameRuntime g=GameRuntime.Instance;if(g?.State?.settings==null)return;AccessibilitySettings s=g.State.settings;for(int i=content.childCount-1;i>=0;i--)Destroy(content.GetChild(i).gameObject);
            float battery=SystemInfo.batteryLevel;string batteryText=battery<0f?"unknown":Mathf.RoundToInt(battery*100f)+"%";summary.text="TARGET "+Application.targetFrameRate+" FPS · BATTERY "+batteryText+" · RAM "+SystemInfo.systemMemorySize+" MB";
            Section("PERFORMANCE / POWER");
            Setting("FRAME RATE LIMIT",s.fpsLimit+" FPS",()=>SetFps(PreviousFps(s.fpsLimit)),()=>SetFps(NextFps(s.fpsLimit)),"30 / 45 / 60 / 90 / 120. Mobile adaptive quality may temporarily lower the effective cap under thermal, low-memory or battery pressure.");
            Toggle("BATTERY SAVER",s.batterySaver,()=>{g.ToggleBatterySaver();Refresh();},"Forces a low-power target and allows the mobile governor to lower render cost earlier.");
            Card("ADAPTIVE RENDER OWNER",Application.isMobilePlatform?"MobilePlatformController exclusively owns mobile render adaptation; the desktop quality governor disables itself on mobile.":"Desktop quality governor active. Mobile governor remains dormant on this platform.",new Color(.20f,.66f,.86f,1f));

            Section("TOUCH / CAMERA");
            Setting("JOYSTICK SIZE",Mathf.RoundToInt(s.joystickSize*100f)+"%",()=>AdjustJoystick(-.10f),()=>AdjustJoystick(.10f),"Persistent touch-control scale. Range 75–145%.");
            Setting("LOOK SENSITIVITY",s.lookSensitivity.ToString("0.00")+"×",()=>{g.AdjustLookSensitivity(-.10f);Refresh();},()=>{g.AdjustLookSensitivity(.10f);Refresh();},"Applies to touch-look, mouse and gamepad camera paths.");

            Section("READABILITY / ACCESSIBILITY");
            Setting("TEXT SCALE",Mathf.RoundToInt(s.textScale*100f)+"%",()=>{g.AdjustTextScale(-.10f);Refresh();},()=>{g.AdjustTextScale(.10f);Refresh();},"Persistent UI scale target. Range 80–140%.");
            Toggle("REDUCED MOTION",s.reducedMotion,()=>{g.ToggleReducedMotion();Refresh();},"Disables decorative workshop motion and animated focus scaling where supported.");
            Toggle("HAPTICS",s.haptics,()=>{g.ToggleHaptics();Refresh();},"Controls success/error vibration feedback on supported mobile devices.");
            Toggle("CAPTIONS",s.captions,()=>{g.ToggleCaptions();Refresh();},"Keeps sound-related information available as on-screen feedback.");
            RowButton("COLOR ASSIST MODE "+s.colorblindMode,"CYCLE",()=>{g.CycleColorblindMode();Refresh();},new Color(.16f,.48f,.68f,1f));
            RowButton("LANGUAGE · "+(s.language=="uk"?"УКРАЇНСЬКА":"ENGLISH"),"SWITCH",()=>{g.SetLanguage(s.language=="uk"?"en":"uk");Refresh();},new Color(.16f,.48f,.68f,1f));

            Section("AUDIO");
            Setting("AMBIENCE / MUSIC",Mathf.RoundToInt(s.musicVolume*100f)+"%",()=>AdjustAudio(true,-.10f),()=>AdjustAudio(true,.10f),"Controls the procedural workshop ambience layer.");
            Setting("SFX / FEEDBACK",Mathf.RoundToInt(s.sfxVolume*100f)+"%",()=>AdjustAudio(false,-.10f),()=>AdjustAudio(false,.10f),"Controls tool, UI and feedback effects where the effect system supports volume scaling.");

            Section("SAVE / HELP");
            ButtonRow("OPEN SAVE / RECOVERY MANAGER",SaveManagerPanel.Open,new Color(.12f,.52f,.36f,1f));
            ButtonRow("OPEN WORKSHOP MANUAL",InGameManualPanel.Open,new Color(.10f,.45f,.66f,1f));
            ButtonRow("CONTEXT HELP",InGameManualPanel.OpenContext,new Color(.18f,.42f,.62f,1f));
        }

        private void SetFps(int fps)
        {
            GameRuntime g=GameRuntime.Instance;if(g?.State?.settings==null)return;g.State.settings.fpsLimit=fps;Application.targetFrameRate=g.State.settings.batterySaver?30:fps;QualitySettings.vSyncCount=0;Persist("FPS limit set to "+fps+".");
        }
        private void AdjustJoystick(float delta)
        {
            GameRuntime g=GameRuntime.Instance;if(g?.State?.settings==null)return;g.State.settings.joystickSize=Mathf.Clamp(g.State.settings.joystickSize+delta,.75f,1.45f);Persist("Joystick size "+Mathf.RoundToInt(g.State.settings.joystickSize*100f)+"%.");
        }
        private void AdjustAudio(bool music,float delta)
        {
            GameRuntime g=GameRuntime.Instance;if(g?.State?.settings==null)return;if(music)g.State.settings.musicVolume=Mathf.Clamp01(g.State.settings.musicVolume+delta);else g.State.settings.sfxVolume=Mathf.Clamp01(g.State.settings.sfxVolume+delta);Persist((music?"Ambience":"SFX")+" volume updated.");
        }
        private void Persist(string message)
        {
            GameRuntime g=GameRuntime.Instance;if(g==null)return;g.Saves?.Save(g.State,1);g.UI?.Refresh();g.Notify(message);Refresh();
        }
        private static int PreviousFps(int current){int[] v={30,45,60,90,120};for(int i=v.Length-1;i>=0;i--)if(v[i]<current)return v[i];return 30;}
        private static int NextFps(int current){int[] v={30,45,60,90,120};foreach(int x in v)if(x>current)return x;return 120;}

        private void Section(string label){Text t=Text(label,content,20,TextAnchor.MiddleLeft,new Color(.10f,.65f,.92f,1f));t.fontStyle=FontStyle.Bold;t.gameObject.AddComponent<LayoutElement>().preferredHeight=34;}
        private void Setting(string label,string value,Action minus,Action plus,string help)
        {
            GameObject card=UI("Setting",content);card.AddComponent<Image>().color=new Color(.082f,.101f,.129f,1f);HorizontalLayoutGroup h=card.AddComponent<HorizontalLayoutGroup>();h.padding=new RectOffset(14,10,8,8);h.spacing=10;h.childControlHeight=true;h.childForceExpandHeight=true;card.AddComponent<LayoutElement>().preferredHeight=78;GameObject info=UI("Info",card.transform);VerticalLayoutGroup v=info.AddComponent<VerticalLayoutGroup>();v.spacing=2;v.childForceExpandHeight=false;info.AddComponent<LayoutElement>().flexibleWidth=1;Text a=Text(label+"  ·  "+value,info.transform,18,TextAnchor.MiddleLeft,Color.white);a.fontStyle=FontStyle.Bold;Text b=Text(help,info.transform,13,TextAnchor.UpperLeft,new Color(.61f,.69f,.77f,1f));Button m=Button("−",card.transform,new Color(.25f,.29f,.34f,1f),minus);m.gameObject.AddComponent<LayoutElement>().preferredWidth=84;Button p=Button("+",card.transform,new Color(.12f,.48f,.68f,1f),plus);p.gameObject.AddComponent<LayoutElement>().preferredWidth=84;
        }
        private void Toggle(string label,bool value,Action action,string help)
        {
            GameObject card=UI("Toggle",content);card.AddComponent<Image>().color=new Color(.082f,.101f,.129f,1f);HorizontalLayoutGroup h=card.AddComponent<HorizontalLayoutGroup>();h.padding=new RectOffset(14,10,8,8);h.spacing=10;h.childControlHeight=true;h.childForceExpandHeight=true;card.AddComponent<LayoutElement>().preferredHeight=74;GameObject info=UI("Info",card.transform);VerticalLayoutGroup v=info.AddComponent<VerticalLayoutGroup>();v.spacing=2;v.childForceExpandHeight=false;info.AddComponent<LayoutElement>().flexibleWidth=1;Text a=Text(label,info.transform,18,TextAnchor.MiddleLeft,Color.white);a.fontStyle=FontStyle.Bold;Text b=Text(help,info.transform,13,TextAnchor.UpperLeft,new Color(.61f,.69f,.77f,1f));Button bt=Button(value?"ON":"OFF",card.transform,value?new Color(.12f,.58f,.36f,1f):new Color(.34f,.37f,.42f,1f),action);bt.gameObject.AddComponent<LayoutElement>().preferredWidth=130;
        }
        private void RowButton(string label,string actionLabel,Action action,Color color){GameObject row=UI("Row",content);row.AddComponent<Image>().color=new Color(.082f,.101f,.129f,1f);HorizontalLayoutGroup h=row.AddComponent<HorizontalLayoutGroup>();h.padding=new RectOffset(14,10,8,8);h.spacing=10;h.childControlHeight=true;h.childForceExpandHeight=true;row.AddComponent<LayoutElement>().preferredHeight=58;Text t=Text(label,row.transform,17,TextAnchor.MiddleLeft,Color.white);t.gameObject.AddComponent<LayoutElement>().flexibleWidth=1;Button b=Button(actionLabel,row.transform,color,action);b.gameObject.AddComponent<LayoutElement>().preferredWidth=150;}
        private void ButtonRow(string label,Action action,Color color){Button b=Button(label,content,color,action);b.gameObject.AddComponent<LayoutElement>().preferredHeight=56;}
        private void Card(string title,string body,Color accent){GameObject card=UI("Card",content);card.AddComponent<Image>().color=new Color(.082f,.101f,.129f,1f);VerticalLayoutGroup v=card.AddComponent<VerticalLayoutGroup>();v.padding=new RectOffset(14,14,8,10);v.spacing=3;v.childForceExpandHeight=false;Text a=Text(title,card.transform,18,TextAnchor.MiddleLeft,accent);a.fontStyle=FontStyle.Bold;Text b=Text(body,card.transform,14,TextAnchor.UpperLeft,new Color(.63f,.71f,.79f,1f));}
        private Button Button(string label,Transform parent,Color color,Action action){GameObject g=UI("Button_"+label,parent);Image im=g.AddComponent<Image>();im.color=color;Button b=g.AddComponent<Button>();b.targetGraphic=im;if(action!=null)b.onClick.AddListener(()=>action());Text t=Text(label,g.transform,16,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,5);t.raycastTarget=false;return b;}private Text Text(string value,Transform parent,int size,TextAnchor align,Color color){GameObject g=UI("Text",parent);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=color;t.raycastTarget=false;return t;}private static GameObject UI(string name,Transform parent){GameObject g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(parent,false);return g;}private static void Stretch(RectTransform r,float i=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(i,i);r.offsetMax=new Vector2(-i,-i);}private static void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject e=new GameObject("EventSystem");e.AddComponent<EventSystem>();e.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(e);}
    }
}
