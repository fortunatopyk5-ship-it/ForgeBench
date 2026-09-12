using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ForgeBench
{
    /// <summary>Connects the runtime-created HUD interact button to press-and-hold world actions on touch devices.</summary>
    public sealed class MobileInteractionBridge : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private static bool installing;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if(installing)return;
            installing=true;
            GameObject host=new GameObject("ForgeBench_MobileInteractionInstaller");
            DontDestroyOnLoad(host);
            host.AddComponent<MobileInteractionInstaller>();
        }

        public void OnPointerDown(PointerEventData eventData){MobileInputState.InteractHeld=true;MobileInputState.InteractPressed=true;}
        public void OnPointerUp(PointerEventData eventData){MobileInputState.InteractHeld=false;}
        public void OnPointerExit(PointerEventData eventData){MobileInputState.InteractHeld=false;}
        private void OnDisable(){MobileInputState.InteractHeld=false;}
    }

    public sealed class MobileInteractionInstaller : MonoBehaviour
    {
        private IEnumerator Start()
        {
            float deadline=Time.realtimeSinceStartup+20f;
            while(Time.realtimeSinceStartup<deadline)
            {
                GameObject target=GameObject.Find("Interact");
                if(target!=null)
                {
                    if(target.GetComponent<MobileInteractionBridge>()==null)target.AddComponent<MobileInteractionBridge>();
                    Destroy(gameObject);
                    yield break;
                }
                yield return new WaitForSecondsRealtime(.15f);
            }
            Destroy(gameObject);
        }
    }
}
