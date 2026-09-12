using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public sealed class ProgressionNode
    {
        public string id;
        public string title;
        public string description;
        public int requiredLevel;
        public int requiredReputation;
        public ProgressionNode(string i,string t,string d,int level,int rep){id=i;title=t;description=d;requiredLevel=level;requiredReputation=rep;}
    }

    /// <summary>
    /// Progression is stored through existing durable XP/reputation/milestone/unlocked-part
    /// fields. Skill unlocks are encoded as SKILL:* milestones to remain forward compatible.
    /// </summary>
    public sealed class ProgressionService
    {
        private readonly GameState state;
        private readonly HardwareCatalog catalog;
        public ProgressionService(GameState s,HardwareCatalog c){state=s;catalog=c;}

        public int Level
        {
            get
            {
                int xp=Mathf.Max(0,state.experience);int level=1;int needed=180;
                while(xp>=needed&&level<30){xp-=needed;level++;needed=180+level*90;}
                return level;
            }
        }

        public int EarnedSkillPoints => Mathf.Max(0,(Level-1)/2);
        public int SpentSkillPoints => Nodes.Count(n=>IsUnlocked(n.id));
        public int AvailableSkillPoints => Mathf.Max(0,EarnedSkillPoints-SpentSkillPoints);
        public string Certification => Level>=20&&state.reputation>=900?"MASTER SYSTEMS ENGINEER":Level>=14&&state.reputation>=550?"SENIOR WORKSHOP ENGINEER":Level>=9&&state.reputation>=300?"CERTIFIED DIAGNOSTICS TECH":Level>=5&&state.reputation>=120?"WORKSHOP TECHNICIAN":"APPRENTICE TECHNICIAN";

        public static readonly ProgressionNode[] Nodes=
        {
            new ProgressionNode("procurement","Efficient Procurement","Supplier trust reduces ordinary hardware acquisition overhead.",3,50),
            new ProgressionNode("thermal","Thermal Specialist","Raises bench thermal-work quality and unlocks advanced cooling milestones.",5,100),
            new ProgressionNode("diagnostics","Diagnostic Method","Raises diagnostics station capability and engineering acceptance quality.",5,120),
            new ProgressionNode("automation","Receiving Automation","Unlocks permanent delivery automation once workshop prerequisites are met.",7,180),
            new ProgressionNode("microsolder","Microsoldering Certification","Unlocks board repair capability and safer rework progression.",9,260),
            new ProgressionNode("network","Network Systems","Unlocks high-tier NAS/server contracts and network diagnostics certification.",9,280),
            new ProgressionNode("premium","Premium Workshop Brand","Improves customer relationship ceiling and premium contract access.",11,380),
            new ProgressionNode("liquid","Custom Loop Engineering","Recognizes advanced liquid-cooling certification and specialist access.",11,400),
            new ProgressionNode("enterprise","Enterprise Service Desk","Unlocks corporate account service milestones and premium workflow status.",15,650),
            new ProgressionNode("master","Master Workshop","Maximum workshop capability floor and elite certification status.",20,900)
        };

        public bool IsUnlocked(string id)=>state.milestones.Any(x=>string.Equals(x,"SKILL:"+id,StringComparison.Ordinal));
        public ProgressionNode Get(string id)=>Nodes.FirstOrDefault(n=>n.id==id);

        public ActionResult Unlock(string id)
        {
            ProgressionNode n=Get(id);if(n==null)return ActionResult.Fail("Unknown progression node.");
            if(IsUnlocked(id))return ActionResult.Fail(n.title+" is already unlocked.");
            if(Level<n.requiredLevel)return ActionResult.Fail("Requires technician level "+n.requiredLevel+".");
            if(state.reputation<n.requiredReputation)return ActionResult.Fail("Requires reputation "+n.requiredReputation+".");
            if(AvailableSkillPoints<=0)return ActionResult.Fail("No unspent certification point available. Gain more experience.");
            state.milestones.Add("SKILL:"+id);ApplyPermanentEffects();
            return ActionResult.Success(n.title+" unlocked.");
        }

        public void ApplyPermanentEffects()
        {
            if(IsUnlocked("thermal"))state.workshop.benchLevel=Mathf.Max(state.workshop.benchLevel,2);
            if(IsUnlocked("diagnostics"))state.workshop.diagnosticsLevel=Mathf.Max(state.workshop.diagnosticsLevel,2);
            if(IsUnlocked("microsolder"))state.workshop.boardRepairLevel=Mathf.Max(state.workshop.boardRepairLevel,2);
            if(IsUnlocked("automation")&&state.workshop.level>=2&&state.workshop.helperStaff>0)state.workshop.deliveryAutomation=true;
            if(IsUnlocked("master"))
            {
                state.workshop.benchLevel=Mathf.Max(state.workshop.benchLevel,5);state.workshop.storageLevel=Mathf.Max(state.workshop.storageLevel,5);
                state.workshop.diagnosticsLevel=Mathf.Max(state.workshop.diagnosticsLevel,4);state.workshop.boardRepairLevel=Mathf.Max(state.workshop.boardRepairLevel,3);
            }
            RefreshPartUnlocks();
        }

        public void RefreshPartUnlocks()
        {
            int level=Level;
            foreach(HardwareDefinition p in catalog.All)
            {
                int gate=1;
                if(p.quality>=90||p.performance>=800)gate=12;
                else if(p.quality>=82||p.performance>=600)gate=9;
                else if(p.quality>=72||p.performance>=420)gate=6;
                else if(p.quality>=62||p.performance>=260)gate=3;
                if(level>=gate&&!state.unlockedPartIds.Contains(p.id))state.unlockedPartIds.Add(p.id);
            }
        }

        public float ProcurementMultiplier => IsUnlocked("procurement")?.94f:1f;
        public float CustomerTrustBonus => IsUnlocked("premium")?5f:0f;
        public float SpecialistQualityBonus => IsUnlocked("microsolder")?.08f:0f;
        public bool AdvancedNetworkUnlocked => IsUnlocked("network")||state.reputation>=420;
        public bool AdvancedLiquidUnlocked => IsUnlocked("liquid")||state.reputation>=420;
        public bool EnterpriseUnlocked => IsUnlocked("enterprise")&&state.reputation>=650;

        public List<string> AchievementSnapshot()
        {
            List<string> a=new List<string>();
            int complete=state.jobs.Count(j=>j.stage==JobStage.Completed);int failed=state.jobs.Count(j=>j.stage==JobStage.Failed);
            if(complete>=1)a.Add("FIRST REPAIR · Complete one contract");if(complete>=10)a.Add("ESTABLISHED SHOP · Complete 10 contracts");if(complete>=50)a.Add("SERVICE VETERAN · Complete 50 contracts");
            if(state.money>=5000)a.Add("POSITIVE CASHFLOW · Hold $5,000");if(state.money>=25000)a.Add("CAPITAL RESERVE · Hold $25,000");
            if(state.reputation>=250)a.Add("LOCAL FAVORITE · Reach 250 reputation");if(state.reputation>=750)a.Add("REGIONAL AUTHORITY · Reach 750 reputation");
            if(failed==0&&complete>=8)a.Add("PERFECT RECORD · First 8+ completed without failure");
            if(state.milestones.Any(x=>x.StartsWith("Specialist:",StringComparison.Ordinal)))a.Add("SPECIALIST BENCH · Complete a specialist contract");
            if(state.workshop.level>=5)a.Add("FULL WORKSHOP · Reach workshop level 5");
            if(Level>=20)a.Add("MASTER TECHNICIAN · Reach technician level 20");
            return a;
        }

        public float LevelProgress01()
        {
            int xp=Mathf.Max(0,state.experience),level=1,needed=180;
            while(xp>=needed&&level<30){xp-=needed;level++;needed=180+level*90;}
            return level>=30?1f:Mathf.Clamp01(xp/(float)needed);
        }
    }
}
