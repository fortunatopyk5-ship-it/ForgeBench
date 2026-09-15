using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public enum IntakeStatus { AwaitingInspection, AwaitingApproval, ReadyForBench, OnBench, ReleaseReview, Closed, Rejected }

    [Serializable]
    public sealed class IntakeAccessoryState
    {
        public string key;
        public string label;
        public bool expected = true;
        public bool received;
        public bool returned;
    }

    [Serializable]
    public sealed class ServiceIntakeRecord
    {
        public string recordId;
        public string jobId;
        public string machineId;
        public string customerName;
        public DeviceCategory deviceCategory;
        public JobType jobType;
        public IntakeStatus status;
        public int openedDay;
        public int closedDay;
        public bool exteriorInspected;
        public bool serialVerified;
        public bool powerStateRecorded;
        public bool accessoriesConfirmed;
        public bool dataConsent;
        public bool estimateApproved;
        public bool benchCheckedIn;
        public bool releaseAuthorized;
        public bool dataConsentRequired;
        public float exteriorCondition = 1f;
        public int visibleDamageCount;
        public bool moistureWarning;
        public string powerState;
        public string reportedIssue;
        public string intakeNotes;
        public List<IntakeAccessoryState> accessories = new List<IntakeAccessoryState>();
        public List<string> evidence = new List<string>();
        public List<string> history = new List<string>();
    }

    [Serializable]
    public sealed class ServiceIntakeDatabase
    {
        public int schemaVersion = 1;
        public int nextSerial = 1;
        public int lastObservedDay;
        public List<ServiceIntakeRecord> records = new List<ServiceIntakeRecord>();
        public List<string> history = new List<string>();
    }

    public static class ServiceIntakeRules
    {
        public static bool RequiresDataConsent(JobType type, DeviceCategory device, bool osInstalled)
        {
            if (type == JobType.Software || type == JobType.Diagnostics || type == JobType.Repair || type == JobType.Upgrade) return true;
            if (osInstalled) return true;
            return device == DeviceCategory.Phone || device == DeviceCategory.Tablet || device == DeviceCategory.Laptop || device == DeviceCategory.NAS || device == DeviceCategory.Server;
        }

        public static float ExteriorCondition(IEnumerable<ItemInstance> items)
        {
            if (items == null) return 1f;
            List<ItemInstance> list = items.Where(x => x != null).ToList();
            if (list.Count == 0) return 1f;
            float average = list.Average(x => Mathf.Clamp01(x.condition));
            float wear = list.Max(x => Mathf.Clamp01(x.wear));
            float damage = list.Count(x => x.damage != DamageType.None) / (float)list.Count;
            return Mathf.Clamp01(average * .72f + (1f - wear) * .18f + (1f - damage) * .10f);
        }

        public static int VisibleDamage(IEnumerable<ItemInstance> items)
        {
            if (items == null) return 0;
            return items.Count(x => x != null && x.damage != DamageType.None);
        }

        public static bool HasMoistureRisk(IEnumerable<ItemInstance> items)
        {
            if (items == null) return false;
            return items.Any(x => x != null && (x.damage == DamageType.LiquidContamination || x.damage == DamageType.Corrosion));
        }

        public static bool CanCheckIn(ServiceIntakeRecord r, out string reason)
        {
            if (r == null) { reason = "Intake record is missing."; return false; }
            if (!r.exteriorInspected || !r.serialVerified) { reason = "Complete the exterior / identity inspection first."; return false; }
            if (!r.powerStateRecorded) { reason = "Record the received power state first."; return false; }
            if (!r.accessoriesConfirmed) { reason = "Confirm customer accessories first."; return false; }
            if (r.dataConsentRequired && !r.dataConsent) { reason = "Customer data-access consent is required for this service."; return false; }
            if (!r.estimateApproved) { reason = "Customer estimate approval is required before bench work."; return false; }
            reason = string.Empty;
            return true;
        }

        public static List<IntakeAccessoryState> DefaultAccessories(DeviceCategory device)
        {
            List<IntakeAccessoryState> result = new List<IntakeAccessoryState>();
            Action<string,string> add = (key,label) => result.Add(new IntakeAccessoryState { key = key, label = label, expected = true });
            switch (device)
            {
                case DeviceCategory.Laptop: add("charger","AC charger"); break;
                case DeviceCategory.Phone: add("sim-tray","SIM tray"); add("usb-cable","USB cable"); break;
                case DeviceCategory.Tablet: add("usb-cable","USB cable"); break;
                case DeviceCategory.Console: add("power-cable","Power cable"); add("controller","Controller"); break;
                case DeviceCategory.Handheld: add("charger","Charger"); break;
                case DeviceCategory.NAS: add("power-brick","Power adapter"); break;
                case DeviceCategory.Server: add("rails","Rack rails / hardware"); break;
                case DeviceCategory.Router: add("power-brick","Power adapter"); break;
                case DeviceCategory.Controller: add("receiver","Wireless receiver / cable"); break;
                default: add("power-cable","Power cable"); break;
            }
            return result;
        }
    }

    /// <summary>
    /// Persistent front-desk intake and chain-of-custody state. It deliberately lives in its
    /// own local file so save-slot reloads cannot silently erase customer evidence or approvals.
    /// </summary>
    public sealed class ServiceIntakeService
    {
        private readonly GameRuntime game;
        private readonly string path;
        private readonly string backupPath;
        public ServiceIntakeDatabase State { get; private set; }

        public ServiceIntakeService(GameRuntime runtime)
        {
            game = runtime;
            path = Path.Combine(Application.persistentDataPath, "ForgeBenchIntake.json");
            backupPath = path + ".bak";
            Load();
        }

        public IReadOnlyList<ServiceIntakeRecord> Records => State.records;
        public ServiceIntakeRecord ActiveRecord
        {
            get
            {
                string id = game?.ActiveJob?.jobId;
                return string.IsNullOrEmpty(id) ? null : State.records.LastOrDefault(x => x.jobId == id && x.status != IntakeStatus.Closed && x.status != IntakeStatus.Rejected);
            }
        }

        private void Load()
        {
            State = TryLoad(path) ?? TryLoad(backupPath) ?? new ServiceIntakeDatabase();
            if (State.records == null) State.records = new List<ServiceIntakeRecord>();
            if (State.history == null) State.history = new List<string>();
            foreach (ServiceIntakeRecord r in State.records) Normalize(r);
        }

        private static ServiceIntakeDatabase TryLoad(string file)
        {
            try
            {
                if (!File.Exists(file)) return null;
                return JsonUtility.FromJson<ServiceIntakeDatabase>(File.ReadAllText(file));
            }
            catch (Exception ex) { Debug.LogWarning("[ForgeBench] Intake recovery failed for " + file + ": " + ex.Message); return null; }
        }

        private static void Normalize(ServiceIntakeRecord r)
        {
            if (r.accessories == null) r.accessories = new List<IntakeAccessoryState>();
            if (r.evidence == null) r.evidence = new List<string>();
            if (r.history == null) r.history = new List<string>();
            if (string.IsNullOrEmpty(r.powerState)) r.powerState = "NOT RECORDED";
        }

        public void Save()
        {
            try
            {
                string temp = path + ".tmp";
                File.WriteAllText(temp, JsonUtility.ToJson(State, true));
                if (File.Exists(path)) File.Copy(path, backupPath, true);
                if (File.Exists(path)) File.Delete(path);
                File.Move(temp, path);
            }
            catch (Exception ex) { Debug.LogWarning("[ForgeBench] Intake save failed: " + ex.Message); }
        }

        public bool Poll()
        {
            if (game?.State == null) return false;
            bool changed = false;
            foreach (JobState j in game.State.jobs.Where(x => x.stage == JobStage.Accepted || x.stage == JobStage.InProgress || x.stage == JobStage.ReadyToSubmit))
            {
                if (State.records.Any(x => x.jobId == j.jobId && x.status != IntakeStatus.Closed && x.status != IntakeStatus.Rejected)) continue;
                CreateRecord(j);
                changed = true;
            }
            foreach (ServiceIntakeRecord r in State.records.Where(x => x.status != IntakeStatus.Closed && x.status != IntakeStatus.Rejected).ToList())
            {
                JobState job = game.State.jobs.FirstOrDefault(x => x.jobId == r.jobId);
                if (job == null) continue;
                if (job.stage == JobStage.Completed || job.stage == JobStage.Failed)
                {
                    r.status = job.stage == JobStage.Completed ? IntakeStatus.Closed : IntakeStatus.Rejected;
                    r.closedDay = game.State.day;
                    foreach (IntakeAccessoryState a in r.accessories) if (a.received) a.returned = true;
                    r.history.Add("Day " + game.State.day + ": record closed after job " + job.stage);
                    State.history.Add(r.recordId + " · closed " + job.stage);
                    changed = true;
                }
            }
            if (State.lastObservedDay != game.State.day) { State.lastObservedDay = game.State.day; Trim(State.history, 160); changed = true; }
            if (changed) Save();
            return changed;
        }

        private ServiceIntakeRecord CreateRecord(JobState job)
        {
            MachineState m = game.State.machines.FirstOrDefault(x => x.machineId == job.machineId);
            ServiceIntakeRecord r = new ServiceIntakeRecord
            {
                recordId = "IN-" + State.nextSerial++.ToString("D5"),
                jobId = job.jobId,
                machineId = job.machineId,
                customerName = job.customerName,
                deviceCategory = job.deviceCategory,
                jobType = job.type,
                status = IntakeStatus.AwaitingInspection,
                openedDay = game.State.day,
                dataConsentRequired = ServiceIntakeRules.RequiresDataConsent(job.type, job.deviceCategory, m != null && m.osInstalled),
                reportedIssue = string.IsNullOrEmpty(job.description) ? job.title : job.description,
                accessories = ServiceIntakeRules.DefaultAccessories(job.deviceCategory)
            };
            r.history.Add("Day " + game.State.day + ": device received for " + job.type);
            State.records.Add(r);
            State.history.Add(r.recordId + " · " + job.customerName + " · opened");
            return r;
        }

        private ServiceIntakeRecord Get(string jobId)
        {
            if (string.IsNullOrEmpty(jobId)) return null;
            ServiceIntakeRecord r = State.records.LastOrDefault(x => x.jobId == jobId && x.status != IntakeStatus.Closed && x.status != IntakeStatus.Rejected);
            if (r != null) return r;
            JobState job = game.State.jobs.FirstOrDefault(x => x.jobId == jobId);
            return job == null ? null : CreateRecord(job);
        }

        public ActionResult InspectExterior(string jobId)
        {
            ServiceIntakeRecord r = Get(jobId); if (r == null) return ActionResult.Fail("No active intake record.");
            MachineState m = game.State.machines.FirstOrDefault(x => x.machineId == r.machineId);
            List<ItemInstance> installed = InstalledItems(m).ToList();
            r.exteriorCondition = ServiceIntakeRules.ExteriorCondition(installed);
            r.visibleDamageCount = ServiceIntakeRules.VisibleDamage(installed);
            r.moistureWarning = ServiceIntakeRules.HasMoistureRisk(installed);
            r.exteriorInspected = true;
            r.serialVerified = true;
            r.status = IntakeStatus.AwaitingApproval;
            string evidence = "VISUAL day " + game.State.day + " · condition " + Mathf.RoundToInt(r.exteriorCondition * 100f) + "% · damage " + r.visibleDamageCount + (r.moistureWarning ? " · MOISTURE/CORROSION FLAG" : "");
            if (!r.evidence.Contains(evidence)) r.evidence.Add(evidence);
            r.history.Add("Day " + game.State.day + ": exterior and identity inspected"); Save();
            return ActionResult.Success("Intake inspection recorded: condition " + Mathf.RoundToInt(r.exteriorCondition * 100f) + "%.");
        }

        public ActionResult RecordPowerState(string jobId)
        {
            ServiceIntakeRecord r = Get(jobId); if (r == null) return ActionResult.Fail("No active intake record.");
            MachineState m = game.State.machines.FirstOrDefault(x => x.machineId == r.machineId);
            if (m == null) r.powerState = "NO MACHINE STATE";
            else if (m.bootState == BootState.PostFailed || (!string.IsNullOrEmpty(m.postCode) && m.postCode != "OFF" && m.postCode != "A0")) r.powerState = "POWERS / POST FAULT";
            else if (m.bootState == BootState.OperatingSystem || m.postCode == "A0") r.powerState = "POWERS / BOOTS";
            else r.powerState = "RECEIVED OFF / NOT BOOTED";
            r.powerStateRecorded = true;
            r.evidence.Add("POWER day " + game.State.day + " · " + r.powerState);
            r.history.Add("Day " + game.State.day + ": received power state recorded"); Save();
            return ActionResult.Success("Power state: " + r.powerState + ".");
        }

        public ActionResult ConfirmAccessories(string jobId)
        {
            ServiceIntakeRecord r = Get(jobId); if (r == null) return ActionResult.Fail("No active intake record.");
            foreach (IntakeAccessoryState a in r.accessories) if (a.expected) a.received = true;
            r.accessoriesConfirmed = true;
            r.history.Add("Day " + game.State.day + ": accessory manifest confirmed (" + r.accessories.Count(x => x.received) + " received)"); Save();
            return ActionResult.Success("Customer accessory manifest confirmed.");
        }

        public ActionResult GrantDataConsent(string jobId)
        {
            ServiceIntakeRecord r = Get(jobId); if (r == null) return ActionResult.Fail("No active intake record.");
            r.dataConsent = true;
            r.history.Add("Day " + game.State.day + ": customer authorized required data access"); Save();
            return ActionResult.Success("Data-access consent recorded.");
        }

        public ActionResult ApproveEstimate(string jobId)
        {
            ServiceIntakeRecord r = Get(jobId); if (r == null) return ActionResult.Fail("No active intake record.");
            r.estimateApproved = true;
            r.status = IntakeStatus.ReadyForBench;
            r.history.Add("Day " + game.State.day + ": customer approved service estimate"); Save();
            return ActionResult.Success("Service estimate approved.");
        }

        public ActionResult CheckIn(string jobId)
        {
            ServiceIntakeRecord r = Get(jobId); if (r == null) return ActionResult.Fail("No active intake record.");
            string reason; if (!ServiceIntakeRules.CanCheckIn(r, out reason)) return ActionResult.Fail(reason);
            r.benchCheckedIn = true; r.status = IntakeStatus.OnBench;
            r.history.Add("Day " + game.State.day + ": chain of custody transferred to workshop bench"); Save();
            return ActionResult.Success("Device checked in to bench custody.");
        }

        public ActionResult AuthorizeRelease(string jobId)
        {
            ServiceIntakeRecord r = Get(jobId); if (r == null) return ActionResult.Fail("No active intake record.");
            if (!r.benchCheckedIn || r.status != IntakeStatus.OnBench) return ActionResult.Fail("Device must be checked in before release review.");
            JobState job = game.State.jobs.FirstOrDefault(x => x.jobId == jobId);
            if (job == null) return ActionResult.Fail("Job is missing.");
            r.releaseAuthorized = true; r.status = IntakeStatus.ReleaseReview;
            r.history.Add("Day " + game.State.day + ": accessory/evidence release review authorized"); Save();
            return ActionResult.Success("Release review authorized. Final job validation may proceed.");
        }

        public ActionResult CanBeginWork(string jobId)
        {
            ServiceIntakeRecord r = Get(jobId);
            if (r == null) return ActionResult.Success("No intake record required.");
            return r.benchCheckedIn ? ActionResult.Success("Bench custody confirmed.") : ActionResult.Fail("Complete SERVICE INTAKE and check the device in before bench work.");
        }

        public ActionResult CanRelease(string jobId)
        {
            ServiceIntakeRecord r = Get(jobId);
            if (r == null) return ActionResult.Success("No intake record required.");
            if (!r.benchCheckedIn) return ActionResult.Fail("Device was never checked into bench custody.");
            if (!r.releaseAuthorized) return ActionResult.Fail("Complete the intake release review before returning the device.");
            return ActionResult.Success("Release review complete.");
        }

        private IEnumerable<ItemInstance> InstalledItems(MachineState m)
        {
            if (m == null) yield break;
            string[] singles = { m.caseItemId, m.motherboardItemId, m.cpuItemId, m.gpuItemId, m.psuItemId, m.coolerItemId };
            foreach (string id in singles) { ItemInstance x = game.Inventory.Get(id); if (x != null) yield return x; }
            foreach (string id in m.ramItemIds ?? new List<string>()) { ItemInstance x = game.Inventory.Get(id); if (x != null) yield return x; }
            foreach (string id in m.storageItemIds ?? new List<string>()) { ItemInstance x = game.Inventory.Get(id); if (x != null) yield return x; }
            foreach (string id in m.fanItemIds ?? new List<string>()) { ItemInstance x = game.Inventory.Get(id); if (x != null) yield return x; }
        }

        private static void Trim(List<string> list, int max) { if (list != null && list.Count > max) list.RemoveRange(0, list.Count - max); }
    }
}