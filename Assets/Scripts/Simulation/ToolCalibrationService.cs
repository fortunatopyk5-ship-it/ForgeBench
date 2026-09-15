using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public enum ToolHealthState { Ready, ServiceDue, CalibrationDue, Unsafe }

    [Serializable]
    public sealed class ToolCalibrationRecord
    {
        public string instanceId;
        public string definitionId;
        public float condition = 1f;
        public float calibration = 1f;
        public float contamination;
        public float drift;
        public int usageCycles;
        public int lastCalibrationDay;
        public int lastServiceDay;
        public int calibrationDueDay = 10;
        public bool lockedOut;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public sealed class ToolCalibrationDatabase
    {
        public int schemaVersion = 1;
        public int lastDay;
        public List<ToolCalibrationRecord> tools = new List<ToolCalibrationRecord>();
        public List<string> observedCompletedJobs = new List<string>();
        public List<string> history = new List<string>();
    }

    public static class ToolCalibrationRules
    {
        public static bool RequiresCalibration(string definitionId)
        {
            return definitionId == "tool_driver" || definitionId == "tool_meter" || definitionId == "tool_psu" || definitionId == "tool_solder";
        }

        public static float CalibrationIntervalDays(string definitionId)
        {
            if (definitionId == "tool_meter") return 14f;
            if (definitionId == "tool_psu") return 18f;
            if (definitionId == "tool_driver") return 12f;
            if (definitionId == "tool_solder") return 10f;
            return 30f;
        }

        public static float Workload(JobType type, string definitionId)
        {
            switch (type)
            {
                case JobType.BoardRepair:
                    if (definitionId == "tool_solder") return 1f;
                    if (definitionId == "tool_meter") return .72f;
                    if (definitionId == "tool_driver") return .32f;
                    break;
                case JobType.Diagnostics:
                    if (definitionId == "tool_meter" || definitionId == "tool_psu") return .74f;
                    if (definitionId == "tool_driver") return .18f;
                    break;
                case JobType.Cleaning:
                    if (definitionId == "tool_air") return .90f;
                    break;
                case JobType.Network:
                    if (definitionId == "tool_meter") return .44f;
                    if (definitionId == "tool_driver") return .22f;
                    break;
                case JobType.Repair:
                case JobType.Upgrade:
                    if (definitionId == "tool_driver") return .62f;
                    if (definitionId == "tool_meter") return .35f;
                    if (definitionId == "tool_air") return .30f;
                    break;
                case JobType.CustomBuild:
                    if (definitionId == "tool_driver") return .72f;
                    if (definitionId == "tool_psu") return .34f;
                    if (definitionId == "tool_air") return .18f;
                    break;
                case JobType.Software:
                    if (definitionId == "tool_meter") return .08f;
                    break;
            }
            return .05f;
        }

        public static ToolHealthState Health(ToolCalibrationRecord r, int day)
        {
            if (r == null || r.lockedOut || r.condition < .28f || r.contamination > .82f) return ToolHealthState.Unsafe;
            if (RequiresCalibration(r.definitionId) && (r.calibration < .70f || day >= r.calibrationDueDay)) return ToolHealthState.CalibrationDue;
            if (r.condition < .62f || r.contamination > .48f) return ToolHealthState.ServiceDue;
            return ToolHealthState.Ready;
        }

        public static float Accuracy(ToolCalibrationRecord r, int day)
        {
            if (r == null) return 0f;
            float overdue = RequiresCalibration(r.definitionId) && day > r.calibrationDueDay ? Mathf.Clamp01((day - r.calibrationDueDay) / 20f) : 0f;
            float accuracy = Mathf.Clamp01(r.calibration * .56f + r.condition * .28f + (1f - r.contamination) * .16f - r.drift * .32f - overdue * .22f);
            if (r.lockedOut) accuracy *= .25f;
            return accuracy;
        }

        public static float BenchReadiness(IEnumerable<ToolCalibrationRecord> tools, int day)
        {
            if (tools == null) return 0f;
            List<ToolCalibrationRecord> list = tools.Where(x => x != null).ToList();
            if (list.Count == 0) return 0f;
            float average = list.Average(x => Accuracy(x,day));
            float unsafePenalty = list.Count(x => Health(x,day) == ToolHealthState.Unsafe) / (float)list.Count;
            return Mathf.Clamp01(average - unsafePenalty * .35f);
        }
    }

    /// <summary>Persistent per-instance tool wear, calibration drift, contamination and service history.</summary>
    public sealed class ToolCalibrationService
    {
        private readonly GameRuntime game;
        private readonly string path;
        private readonly string backupPath;
        public ToolCalibrationDatabase State { get; private set; }

        public ToolCalibrationService(GameRuntime runtime)
        {
            game = runtime;
            path = Path.Combine(Application.persistentDataPath,"ForgeBenchToolCalibration.json");
            backupPath = path + ".bak";
            Load();
            SyncInventory();
        }

        public IReadOnlyList<ToolCalibrationRecord> Tools => State.tools;

        private void Load()
        {
            State = TryLoad(path) ?? TryLoad(backupPath) ?? new ToolCalibrationDatabase();
            if (State.tools == null) State.tools = new List<ToolCalibrationRecord>();
            if (State.observedCompletedJobs == null) State.observedCompletedJobs = new List<string>();
            if (State.history == null) State.history = new List<string>();
            foreach (ToolCalibrationRecord r in State.tools) if (r.history == null) r.history = new List<string>();
        }

        private static ToolCalibrationDatabase TryLoad(string file)
        {
            try
            {
                if (!File.Exists(file)) return null;
                return JsonUtility.FromJson<ToolCalibrationDatabase>(File.ReadAllText(file));
            }
            catch (Exception ex) { Debug.LogWarning("[ForgeBench] Tool calibration recovery failed: " + ex.Message); return null; }
        }

        public void Save()
        {
            try
            {
                string temp = path + ".tmp";
                File.WriteAllText(temp,JsonUtility.ToJson(State,true));
                if (File.Exists(path)) File.Copy(path,backupPath,true);
                if (File.Exists(path)) File.Delete(path);
                File.Move(temp,path);
            }
            catch (Exception ex) { Debug.LogWarning("[ForgeBench] Tool calibration save failed: " + ex.Message); }
        }

        public bool Poll()
        {
            bool changed = SyncInventory();
            if (game?.State == null) return changed;
            if (State.lastDay != game.State.day)
            {
                int delta = State.lastDay <= 0 ? 1 : Mathf.Clamp(game.State.day - State.lastDay,1,30);
                Age(delta);
                State.lastDay = game.State.day;
                changed = true;
            }
            foreach (JobState job in game.State.jobs.Where(x => x.stage == JobStage.Completed))
            {
                if (string.IsNullOrEmpty(job.jobId) || State.observedCompletedJobs.Contains(job.jobId)) continue;
                ApplyCompletedJob(job);
                State.observedCompletedJobs.Add(job.jobId);
                changed = true;
            }
            if (State.observedCompletedJobs.Count > 180) State.observedCompletedJobs.RemoveRange(0,State.observedCompletedJobs.Count-180);
            if (State.history.Count > 180) State.history.RemoveRange(0,State.history.Count-180);
            if (changed) Save();
            return changed;
        }

        public bool SyncInventory()
        {
            if (game?.State?.inventory == null || game.Catalog == null) return false;
            bool changed = false;
            HashSet<string> present = new HashSet<string>();
            foreach (ItemInstance item in game.State.inventory)
            {
                HardwareDefinition d = game.Catalog.Get(item.definitionId);
                if (d == null || d.category != PartCategory.Tool) continue;
                present.Add(item.instanceId);
                ToolCalibrationRecord r = State.tools.FirstOrDefault(x => x.instanceId == item.instanceId);
                if (r != null) continue;
                r = new ToolCalibrationRecord
                {
                    instanceId=item.instanceId,
                    definitionId=item.definitionId,
                    condition=Mathf.Clamp01(item.condition <= 0f ? 1f : item.condition),
                    calibration=1f,
                    lastCalibrationDay=game.State.day,
                    lastServiceDay=game.State.day,
                    calibrationDueDay=game.State.day+Mathf.RoundToInt(ToolCalibrationRules.CalibrationIntervalDays(item.definitionId))
                };
                r.history.Add("Day "+game.State.day+": entered calibration register");
                State.tools.Add(r); changed=true;
            }
            for (int i=State.tools.Count-1;i>=0;i--)
            {
                if (present.Contains(State.tools[i].instanceId)) continue;
                State.history.Add(State.tools[i].definitionId+" removed from tool register");
                State.tools.RemoveAt(i); changed=true;
            }
            return changed;
        }

        private void Age(int days)
        {
            foreach (ToolCalibrationRecord r in State.tools)
            {
                float ambient = r.definitionId == "tool_solder" ? .0022f : .0010f;
                r.condition = Mathf.Clamp01(r.condition - ambient * days);
                r.drift = Mathf.Clamp01(r.drift + (ToolCalibrationRules.RequiresCalibration(r.definitionId) ? .0035f : .0012f) * days);
                r.calibration = Mathf.Clamp01(r.calibration - r.drift * .0025f * days);
                if (ToolCalibrationRules.Health(r,game.State.day) == ToolHealthState.Unsafe) r.lockedOut=true;
            }
        }

        private void ApplyCompletedJob(JobState job)
        {
            foreach (ToolCalibrationRecord r in State.tools)
            {
                float load=ToolCalibrationRules.Workload(job.type,r.definitionId);
                if (load <= .05f) continue;
                r.usageCycles++;
                r.condition=Mathf.Clamp01(r.condition-.006f*load);
                r.drift=Mathf.Clamp01(r.drift+.008f*load);
                r.calibration=Mathf.Clamp01(r.calibration-.005f*load);
                if (job.type == JobType.Cleaning || job.type == JobType.BoardRepair) r.contamination=Mathf.Clamp01(r.contamination+.010f*load);
                r.history.Add("Day "+game.State.day+": workload from "+job.jobId+" / "+job.type);
                if (r.history.Count>45) r.history.RemoveRange(0,r.history.Count-45);
            }
            State.history.Add("Day "+game.State.day+": tool wear posted for completed "+job.jobId);
        }

        public ActionResult Calibrate(string instanceId)
        {
            ToolCalibrationRecord r=State.tools.FirstOrDefault(x=>x.instanceId==instanceId);
            if(r==null)return ActionResult.Fail("Tool is not in the calibration register.");
            if(!ToolCalibrationRules.RequiresCalibration(r.definitionId))return ActionResult.Fail("This tool does not require metrology calibration.");
            float cost=r.definitionId=="tool_meter"?28f:r.definitionId=="tool_solder"?22f:16f;
            ActionResult pay=game.Economy.Spend(cost,"Calibrate "+r.definitionId);if(!pay.ok)return pay;
            r.calibration=1f;r.drift=0f;r.lockedOut=r.condition<.28f||r.contamination>.82f;r.lastCalibrationDay=game.State.day;r.calibrationDueDay=game.State.day+Mathf.RoundToInt(ToolCalibrationRules.CalibrationIntervalDays(r.definitionId));
            r.history.Add("Day "+game.State.day+": calibrated against reference standard");Save();game.Saves?.Save(game.State,1);
            return ActionResult.Success("Calibration passed. Next due day "+r.calibrationDueDay+".");
        }

        public ActionResult Service(string instanceId)
        {
            ToolCalibrationRecord r=State.tools.FirstOrDefault(x=>x.instanceId==instanceId);if(r==null)return ActionResult.Fail("Tool is not in the calibration register.");
            float cost=12f+(1f-r.condition)*38f+r.contamination*18f;ActionResult pay=game.Economy.Spend(cost,"Tool service: "+r.definitionId);if(!pay.ok)return pay;
            r.condition=Mathf.Clamp01(r.condition+.34f);r.contamination=Mathf.Max(0f,r.contamination-.72f);r.lockedOut=false;r.lastServiceDay=game.State.day;r.history.Add("Day "+game.State.day+": cleaned, inspected and serviced");Save();game.Saves?.Save(game.State,1);
            return ActionResult.Success("Tool serviced and returned to bench.");
        }

        public float Readiness(JobType type)
        {
            List<ToolCalibrationRecord> relevant=State.tools.Where(x=>ToolCalibrationRules.Workload(type,x.definitionId)>.10f).ToList();
            if(relevant.Count==0)return .35f;
            float weighted=0f,total=0f;
            foreach(ToolCalibrationRecord r in relevant){float w=ToolCalibrationRules.Workload(type,r.definitionId);weighted+=ToolCalibrationRules.Accuracy(r,game.State.day)*w;total+=w;}
            return total<=0f?0f:Mathf.Clamp01(weighted/total);
        }

        public ActionResult ReadinessCheck(JobType type)
        {
            float readiness=Readiness(type);int unsafeCount=State.tools.Count(x=>ToolCalibrationRules.Workload(type,x.definitionId)>.10f&&ToolCalibrationRules.Health(x,game.State.day)==ToolHealthState.Unsafe);
            if(unsafeCount>0)return ActionResult.Fail("Tool lockout: "+unsafeCount+" unsafe tool(s) are relevant to this job.");
            if(readiness<.62f)return ActionResult.Fail("Tool readiness is only "+Mathf.RoundToInt(readiness*100f)+"%. Service/calibrate before precision work.");
            return ActionResult.Success("Tool readiness "+Mathf.RoundToInt(readiness*100f)+"% for "+type+".");
        }
    }
}