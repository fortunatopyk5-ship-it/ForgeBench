using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>Actual in-player title screen and pause/settings entry point.</summary>
    public sealed class MainMenuController : MonoBehaviour
    {
        private GameRuntime game;
        private Canvas canvas;
        private GameObject panel;
        private Text subtitle;
        private Font font;
        private bool built;

        private void Start()
        {
            game = GameRuntime.Instance;
            Build();
            Show(true);
        }

        private void Update()
        {
            if (!built) return;
            if (Input.GetKeyDown(KeyCode.Escape) && !Application.isMobilePlatform)
                Show(panel == null || !panel.activeSelf);
        }

        public void Show(bool visible)
        {
            if (!built) Build();
            if (panel == null) return;
            panel.SetActive(visible);
            FirstPersonController fp=FindAnyObjectByType<FirstPersonController>();
            if(fp!=null)fp.enabled=!visible;
            MobileInputState.Move=Vector2.zero;
            MobileInputState.Look=Vector2.zero;
            MobileInputState.InteractHeld=false;
            MobileInputState.InteractPressed=false;
            Cursor.lockState=visible?CursorLockMode.None:CursorLockMode.Locked;
            RefreshSubtitle();
        }

        private void Build()
        {
            if (built) return;
            built = true;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureEventSystem();

            GameObject cg = new GameObject("ForgeBench_MainMenu");
            canvas = cg.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 20000;
            CanvasScaler scaler = cg.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920,1080); scaler.matchWidthOrHeight=.5f;
            cg.AddComponent<GraphicRaycaster>(); DontDestroyOnLoad(cg);

            panel = UI("Panel", cg.transform); Stretch(panel.GetComponent<RectTransform>());
            Image bg = panel.AddComponent<Image>(); bg.color = new Color(.018f,.026f,.038f,.985f);

            GameObject card = UI("MenuCard", panel.transform); RectTransform cr = card.GetComponent<RectTransform>(); cr.anchorMin=new Vector2(.08f,.10f); cr.anchorMax=new Vector2(.48f,.90f); cr.offsetMin=cr.offsetMax=Vector2.zero;
            Image ci=card.AddComponent<Image>(); ci.color=new Color(.055f,.069f,.088f,.98f);

            Text title=Text("FORGEBENCH",card.transform,54,TextAnchor.MiddleLeft,Color.white); RectTransform tr=title.rectTransform;tr.anchorMin=new Vector2(.08f,.78f);tr.anchorMax=new Vector2(.92f,.94f);tr.offsetMin=tr.offsetMax=Vector2.zero;
            Text kicker=Text("PC & ELECTRONICS WORKSHOP",card.transform,19,TextAnchor.UpperLeft,new Color(.20f,.72f,1f,1f));RectTransform kr=kicker.rectTransform;kr.anchorMin=new Vector2(.08f,.72f);kr.anchorMax=new Vector2(.92f,.79f);kr.offsetMin=kr.offsetMax=Vector2.zero;
            subtitle=Text("",card.transform,20,TextAnchor.UpperLeft,new Color(.72f,.78f,.84f,1f));RectTransform sr=subtitle.rectTransform;sr.anchorMin=new Vector2(.08f,.61f);sr.anchorMax=new Vector2(.92f,.72f);sr.offsetMin=sr.offsetMax=Vector2.zero;

            Button cont=Button("CONTINUE",card.transform,new Color(.07f,.55f,.86f,1f),()=>Show(false)); Place(cont, .08f,.49f,.92f,.58f);
            Button jobs=Button("CONTINUE · OPEN JOB BOARD",card.transform,new Color(.12f,.16f,.21f,1f),()=>{Show(false);game?.UI?.OpenTab("JOBS");});Place(jobs,.08f,.385f,.92f,.475f);
            Button fresh=Button("NEW WORKSHOP",card.transform,new Color(.12f,.16f,.21f,1f),()=>{game?.NewGame();Show(false);game?.UI?.OpenTab("JOBS");});Place(fresh,.08f,.28f,.92f,.37f);
            Button settings=Button("SETTINGS / SAVE",card.transform,new Color(.12f,.16f,.21f,1f),()=>{Show(false);game?.UI?.OpenTab("SETTINGS");});Place(settings,.08f,.175f,.92f,.265f);
            Button quit=Button("QUIT",card.transform,new Color(.18f,.10f,.11f,1f),Application.Quit);Place(quit,.08f,.07f,.92f,.16f);

            GameObject visual=UI("Visual",panel.transform);RectTransform vr=visual.GetComponent<RectTransform>();vr.anchorMin=new Vector2(.53f,.10f);vr.anchorMax=new Vector2(.92f,.90f);vr.offsetMin=vr.offsetMax=Vector2.zero;Image vi=visual.AddComponent<Image>();vi.color=new Color(.028f,.038f,.052f,1f);
            Text big=Text("BUILD\nDIAGNOSE\nREPAIR\nTUNE",visual.transform,56,TextAnchor.MiddleCenter,new Color(.82f,.90f,.96f,1f));Stretch(big.rectTransform);

            RefreshSubtitle();
        }

        private void RefreshSubtitle()
        {
            if (subtitle == null || game == null || game.State == null) return;
            JobState j=game.ActiveJob;
            subtitle.text="DAY "+game.State.day+"   ·   $"+game.State.money.ToString("0")+"   ·   REP "+game.State.reputation+"\n"+(j==null?"No active contract":j.jobId+" · "+j.title);
        }

        private void EnsureEventSystem(){if(FindAnyObjectByType<EventSystem>()!=null)return;GameObject go=new GameObject("EventSystem");go.AddComponent<EventSystem>();go.AddComponent<StandaloneInputModule>();DontDestroyOnLoad(go);}
        private GameObject UI(string name,Transform p){GameObject g=new GameObject(name,typeof(RectTransform));g.transform.SetParent(p,false);return g;}
        private Text Text(string value,Transform p,int size,TextAnchor align,Color c){GameObject g=UI("Text",p);Text t=g.AddComponent<Text>();t.font=font;t.text=value;t.fontSize=size;t.alignment=align;t.color=c;t.resizeTextForBestFit=false;return t;}
        private Button Button(string label,Transform p,Color c,UnityEngine.Events.UnityAction action){GameObject g=UI(label,p);Image i=g.AddComponent<Image>();i.color=c;Button b=g.AddComponent<Button>();b.targetGraphic=i;b.onClick.AddListener(action);Text t=Text(label,g.transform,22,TextAnchor.MiddleCenter,Color.white);Stretch(t.rectTransform,8);return b;}
        private static void Place(Component c,float x0,float y0,float x1,float y1){RectTransform r=c.GetComponent<RectTransform>();r.anchorMin=new Vector2(x0,y0);r.anchorMax=new Vector2(x1,y1);r.offsetMin=r.offsetMax=Vector2.zero;}
        private static void Stretch(RectTransform r,float inset=0){r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(inset,inset);r.offsetMax=new Vector2(-inset,-inset);}
    }
}
