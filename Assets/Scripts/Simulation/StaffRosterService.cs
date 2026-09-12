using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public enum StaffRole { Receiving, Assembly, Diagnostics, BoardRepair, Network, CustomerDesk }

    [Serializable]
    public sealed class StaffMemberState
    {
        public string staffId;
        public string name;
        public bool specialist;
        public StaffRole role;
        public int skill = 1;
        public int trainingXp;
        public float morale = .78f;
        public float fatigue;
        public int daysEmployed;
        public int jobsAssisted;
        public List<string> certifications = new List<string>();
        public List<string> history = new List<string>();
    }

    [Serializable]
    public sealed class StaffRosterState
    {
        public int schemaVersion = 1;
        public int lastDay;
        public int nextSerial = 1;
        public List<StaffMemberState> members = new List<StaffMemberState>();
        public List<string> history = new List<string>();
    }

    public sealed class StaffRosterService
    {
        private readonly GameRuntime game;private readonly string path;public StaffRosterState State{get;private set;}
        private static readonly string[] Names={"Alex Rowan","Mila Hart","Danylo Voss","Nora Pike","Ihor Vale","Eva Chen","Leo Moss","Sofia Kern","Maks Ray","Lina Novak"};
        public StaffRosterService(GameRuntime g){game=g;path=Path.Combine(Application.persistentDataPath,"ForgeBenchStaff.json");Load();SyncCounts();}
        private void Load(){State=new StaffRosterState();try{if(File.Exists(path)){StaffRosterState x=JsonUtility.FromJson<StaffRosterState>(File.ReadAllText(path));if(x!=null)State=x;}}catch(Exception ex){Debug.LogWarning("[ForgeBench] Staff roster recovery: "+ex.Message);}if(State.members==null)State.members=new List<StaffMemberState>();if(State.history==null)State.history=new List<string>();foreach(StaffMemberState m in State.members){if(m.certifications==null)m.certifications=new List<string>();if(m.history==null)m.history=new List<string>();}}
        public void Save(){try{string tmp=path+".tmp";File.WriteAllText(tmp,JsonUtility.ToJson(State,true));if(File.Exists(path))File.Delete(path);File.Move(tmp,path);}catch(Exception ex){Debug.LogWarning("[ForgeBench] Staff roster save failed: "+ex.Message);}}
        public void SyncCounts(){if(game?.State?.workshop==null)return;int helpers=State.members.Count(x=>!x.specialist),specialists=State.members.Count(x=>x.specialist);while(helpers<game.State.workshop.helperStaff){Add(false);helpers++;}while(specialists<game.State.workshop.specialistStaff){Add(true);specialists++;}while(helpers>game.State.workshop.helperStaff){StaffMemberState x=State.members.LastOrDefault(m=>!m.specialist);if(x==null)break;State.members.Remove(x);helpers--;}while(specialists>game.State.workshop.specialistStaff){StaffMemberState x=State.members.LastOrDefault(m=>m.specialist);if(x==null)break;State.members.Remove(x);specialists--;}Save();}
        private void Add(bool specialist){int serial=State.nextSerial++;StaffMemberState m=new StaffMemberState{staffId="EMP-"+serial.ToString("D3"),name=Names[(serial-1)%Names.Length],specialist=specialist,role=specialist?StaffRole.Diagnostics:StaffRole.Receiving,skill=specialist?2:1,morale=.80f};m.history.Add("Joined workshop day "+game.State.day);State.members.Add(m);State.history.Add(m.name+" joined as "+(specialist?"specialist":"helper"));}
        public ActionResult Assign(string id,StaffRole role){StaffMemberState m=State.members.FirstOrDefault(x=>x.staffId==id);if(m==null)return ActionResult.Fail("Staff member not found.");if(role==StaffRole.BoardRepair&&!m.specialist)return ActionResult.Fail("Board repair assignment requires specialist staff.");m.role=role;m.history.Add("Day "+game.State.day+": assigned to "+role);Save();return ActionResult.Success(m.name+" assigned to "+role+".");}
        public ActionResult Train(string id){StaffMemberState m=State.members.FirstOrDefault(x=>x.staffId==id);if(m==null)return ActionResult.Fail("Staff member not found.");if(m.skill>=5)return ActionResult.Fail("Staff member already has maximum practical skill.");float cost=80f+m.skill*70f+(m.specialist?60f:0f);ActionResult pay=game.Economy.Spend(cost,"Staff training: "+m.name);if(!pay.ok)return pay;m.trainingXp+=100;m.skill++;m.morale=Mathf.Clamp01(m.morale+.08f);string cert=m.skill>=5?"Master Technician":m.skill>=4?"Senior Service":m.skill>=3?"Certified Service":null;if(cert!=null&&!m.certifications.Contains(cert))m.certifications.Add(cert);m.history.Add("Day "+game.State.day+": training completed · skill "+m.skill);ApplyCapability();game.Saves?.Save(game.State,1);Save();return ActionResult.Success(m.name+" trained to skill "+m.skill+".");}
        public void DayTick(){if(State.lastDay==game.State.day)return;int delta=State.lastDay<=0?1:Mathf.Clamp(game.State.day-State.lastDay,1,30);State.lastDay=game.State.day;foreach(StaffMemberState m in State.members){m.daysEmployed+=delta;float workload=RoleWorkload(m.role);m.fatigue=Mathf.Clamp01(m.fatigue+workload*.08f*delta-.04f*delta);m.morale=Mathf.Clamp01(m.morale-.012f*delta+(game.State.workshop.cleanliness>.8f?.006f:0f)*delta);if(m.fatigue>.82f)m.history.Add("Day "+game.State.day+": high fatigue warning");}ApplyCapability();Trim(State.history,80);Save();}
        public float RoleQuality(StaffRole role){List<StaffMemberState> staff=State.members.Where(x=>x.role==role&&x.fatigue<.95f).ToList();if(staff.Count==0)return 0f;return staff.Average(x=>(x.skill/5f)*Mathf.Lerp(.55f,1f,x.morale)*(1f-x.fatigue*.45f));}
        public int Assigned(StaffRole role)=>State.members.Count(x=>x.role==role);
        private void ApplyCapability(){float diag=RoleQuality(StaffRole.Diagnostics),board=RoleQuality(StaffRole.BoardRepair);if(diag>.45f)game.State.workshop.diagnosticsLevel=Mathf.Max(game.State.workshop.diagnosticsLevel,2);if(diag>.75f)game.State.workshop.diagnosticsLevel=Mathf.Max(game.State.workshop.diagnosticsLevel,3);if(board>.40f)game.State.workshop.boardRepairLevel=Mathf.Max(game.State.workshop.boardRepairLevel,2);if(board>.78f)game.State.workshop.boardRepairLevel=Mathf.Max(game.State.workshop.boardRepairLevel,3);}
        private static float RoleWorkload(StaffRole r){switch(r){case StaffRole.BoardRepair:return .95f;case StaffRole.Diagnostics:return .78f;case StaffRole.Assembly:return .72f;case StaffRole.Network:return .68f;case StaffRole.Receiving:return .52f;default:return .45f;}}
        private static void Trim(List<string> l,int max){if(l!=null&&l.Count>max)l.RemoveRange(0,l.Count-max);}
    }
}
