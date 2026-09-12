using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForgeBench
{
    /// <summary>
    /// Localizes exact static labels created by runtime-generated production panels.
    /// Dynamic measurements/customer strings remain untouched. Both EN and UK source
    /// variants are recognized so switching languages repeatedly is reversible.
    /// </summary>
    public sealed class RuntimeLocalizationDirector : MonoBehaviour
    {
        private readonly LocalizationService loc=new LocalizationService();private string language="";private float nextScan;
        private static readonly Dictionary<string,string> Keys=new Dictionary<string,string>
        {
            {"CONTINUE","menu.continue"},{"ПРОДОВЖИТИ","menu.continue"},{"CONTINUE · OPEN JOB BOARD","menu.jobs"},{"ПРОДОВЖИТИ · ВІДКРИТИ ЗАМОВЛЕННЯ","menu.jobs"},{"NEW WORKSHOP","menu.new"},{"НОВА МАЙСТЕРНЯ","menu.new"},{"SETTINGS / SAVE","menu.settings"},{"НАЛАШТУВАННЯ / ЗБЕРЕЖЕННЯ","menu.settings"},{"QUIT","menu.quit"},{"ВИЙТИ","menu.quit"},
            {"CUSTOMERS / CRM","crm.title"},{"КЛІЄНТИ / CRM","crm.title"},{"OVERVIEW","crm.overview"},{"ОГЛЯД","crm.overview"},{"CUSTOMERS","crm.customers"},{"КЛІЄНТИ","crm.customers"},{"REVIEWS","crm.reviews"},{"ВІДГУКИ","crm.reviews"},{"INVITE BACK","crm.invite"},{"ЗАПРОСИТИ ЗНОВУ","crm.invite"},{"RESOLVE","crm.resolve"},{"ВИРІШИТИ","crm.resolve"},
            {"CAREER / CERTIFICATIONS","career.title"},{"КАР'ЄРА / СЕРТИФІКАЦІЇ","career.title"},{"CAREER","career.career"},{"КАР'ЄРА","career.career"},{"SKILLS","career.skills"},{"НАВИЧКИ","career.skills"},{"ACHIEVEMENTS","career.achievements"},{"ДОСЯГНЕННЯ","career.achievements"},{"UNLOCK","career.unlock"},{"ВІДКРИТИ","career.unlock"},{"LOCKED","career.locked"},{"ЗАБЛОКОВАНО","career.locked"},
            {"WAREHOUSE INVENTORY","inventory.title"},{"СКЛАДСЬКИЙ ОБЛІК","inventory.title"},{"◀ PREVIOUS","inventory.previous"},{"◀ НАЗАД","inventory.previous"},{"NEXT ▶","inventory.next"},{"ДАЛІ ▶","inventory.next"},{"REFRESH","inventory.refresh"},{"ОНОВИТИ","inventory.refresh"},{"INSTALL","inventory.install"},{"ВСТАНОВИТИ","inventory.install"},{"REMOVE","inventory.remove"},{"ЗНЯТИ","inventory.remove"},
            {"LIQUID LOOP","specialist.liquid"},{"РІДИННЕ ОХОЛОДЖЕННЯ","specialist.liquid"},{"BOARD REPAIR","specialist.board"},{"РЕМОНТ ПЛАТ","specialist.board"},{"PORTABLE","specialist.portable"},{"ПОРТАТИВНА ТЕХНІКА","specialist.portable"},{"NETWORK / NAS","specialist.network"},{"МЕРЕЖА / NAS","specialist.network"},
            {"WORKSHOP MANAGEMENT","business.title"},{"КЕРУВАННЯ МАЙСТЕРНЕЮ","business.title"},{"BUSINESS","business.business"},{"БІЗНЕС","business.business"},{"STAFF","business.staff"},{"ПЕРСОНАЛ","business.staff"},{"WAREHOUSE","business.warehouse"},{"СКЛАД","business.warehouse"},
            {"POST / BIOS","engineering.post"},{"THERMAL / POWER","engineering.thermal"},{"ТЕМПЕРАТУРИ / ЖИВЛЕННЯ","engineering.thermal"},{"OS / STORAGE","engineering.storage"},{"ОС / НАКОПИЧУВАЧІ","engineering.storage"},{"BENCHMARK","engineering.benchmark"},{"БЕНЧМАРК","engineering.benchmark"},
            {"WORKSHOP OPERATIONS / QA COMMAND","operations.title"},{"ОПЕРАЦІЇ МАЙСТЕРНІ / КОНТРОЛЬ ЯКОСТІ","operations.title"},{"LIVE OPERATIONS","operations.live"},{"ПОТОЧНІ ОПЕРАЦІЇ","operations.live"},{"ACTIVE CONTRACT / FINAL QUALITY","operations.active"},{"АКТИВНЕ ЗАМОВЛЕННЯ / ФІНАЛЬНА ЯКІСТЬ","operations.active"},{"CUSTOMER / WARRANTY RISK","operations.customer"},{"КЛІЄНТИ / ГАРАНТІЙНИЙ РИЗИК","operations.customer"},{"STAFF CAPACITY","operations.staff"},{"ЗАВАНТАЖЕННЯ ПЕРСОНАЛУ","operations.staff"},{"WORKSHOP HEALTH","operations.health"},{"СТАН МАЙСТЕРНІ","operations.health"},{"CONTROL SURFACES","operations.controls"},{"ПАНЕЛІ КЕРУВАННЯ","operations.controls"},{"FINAL PREFLIGHT","operations.preflight"},{"ФІНАЛЬНА ПЕРЕВІРКА","operations.preflight"},{"RECEIVE READY","operations.receive"},{"ПРИЙНЯТИ ГОТОВЕ","operations.receive"},{"JOB BOARD","operations.jobs"},{"ЗАМОВЛЕННЯ","operations.jobs"},
            {"WARRANTY / COMEBACK DESK","warranty.title"},{"ГАРАНТІЯ / ПОВТОРНІ ЗВЕРНЕННЯ","warranty.title"},{"CREATE NO-CHARGE CALLBACK","warranty.action"},{"СТВОРИТИ БЕЗКОШТОВНЕ ПОВТОРНЕ ЗАМОВЛЕННЯ","warranty.action"},{"NO WARRANTY CASES","warranty.none"},{"НЕМАЄ ГАРАНТІЙНИХ ВИПАДКІВ","warranty.none"},{"INITIALIZING","warranty.initializing"},{"ІНІЦІАЛІЗАЦІЯ","warranty.initializing"},{"WARRANTY","warranty.button"},{"ГАРАНТІЯ","warranty.button"},{"MAINTENANCE","maintenance.button"},{"ОБСЛУГОВУВАННЯ","maintenance.button"},
            {"JOBS","tab.jobs"},{"ЗАМОВЛЕННЯ","tab.jobs"},{"STORE","tab.store"},{"МАГАЗИН","tab.store"},{"INVENTORY","tab.inventory"},{"BENCH","tab.bench"},{"ВЕРСТАК","tab.bench"},{"BIOS","tab.bios"},{"OS","tab.os"},{"ОС","tab.os"},{"DIAG","tab.diag"},{"ДІАГНОСТИКА","tab.diag"},{"PROGRESS","tab.progress"},{"ПРОГРЕС","tab.progress"},{"SETTINGS","tab.settings"},{"НАЛАШТУВАННЯ","tab.settings"},
            {"TABLET","hud.tablet"},{"ПЛАНШЕТ","hud.tablet"},{"INTERACT","hud.interact"},{"ДІЯ","hud.interact"},{"✕","common.close"}
        };
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]private static void Install(){if(FindAnyObjectByType<RuntimeLocalizationDirector>()!=null)return;GameObject go=new GameObject("ForgeBench_RuntimeLocalization");DontDestroyOnLoad(go);go.AddComponent<RuntimeLocalizationDirector>();}
        private void Update(){if(Time.unscaledTime<nextScan)return;nextScan=Time.unscaledTime+.8f;GameRuntime g=GameRuntime.Instance;if(g?.State?.settings==null)return;string next=g.State.settings.language=="en"?"en":"uk";if(next!=language){language=next;loc.Load(language);}Apply();}
        private void Apply(){Text[] texts=FindObjectsByType<Text>(FindObjectsInactive.Include,FindObjectsSortMode.None);foreach(Text t in texts){if(t==null||string.IsNullOrEmpty(t.text))continue;string key;if(Keys.TryGetValue(t.text,out key))t.text=loc.T(key,t.text);}}
    }
}
