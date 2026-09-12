using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public enum WarrantyCaseStatus { Monitoring, ClaimAvailable, CallbackOffered, InService, Resolved, Failed }

    [Serializable]
    public sealed class WarrantySnapshot
    {
        public string jobId;
        public string customerName;
        public int capturedDay;
        public float qualityScore = 100f;
        public float averageCondition = 1f;
        public float maximumWear;
        public float dust;
        public float cableQuality;
        public float benchmarkScore;
        public float cpuPeakC;
        public float gpuPeakC;
        public float rippleMv;
        public bool stable;
        public bool postPassed;
        public bool osHealthy = true;
        public bool deadlineMet;
        public int unresolvedFaults;
        public int unresolvedDamage;
        public List<string> warnings = new List<string>();
    }

    [Serializable]
    public sealed class WarrantyCaseState
    {
        public string caseId;
        public string sourceJobId;
        public string callbackJobId;
        public string customerName;
        public DeviceCategory deviceCategory;
        public int openedDay;
        public int claimDay;
        public int resolvedDay;
        public float risk;
        public float estimatedLiability;
        public WarrantyCaseStatus status;
        public string reason;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public sealed class WarrantyDatabase
    {
        public int schemaVersion = 1;
        public int nextCaseSerial = 1;
        public int lastObservedDay;
        public List<string> processedSourceJobs = new List<string>();
        public List<string> processedCallbackJobs = new List<string>();
        public List<WarrantySnapshot> snapshots = new List<WarrantySnapshot>();
        public List<WarrantyCaseState> cases = new List<WarrantyCaseState>();
        public List<string> history = new List<string>();
    }

    public static class WarrantyMath
    {
        public static float CalculateRisk(WarrantySnapshot s)
        {
            if (s == null) return .05f;
            float risk = .018f;
            risk += Mathf.Clamp01((86f - s.qualityScore) / 86f) * .28f;
            risk += Mathf.Clamp01((.72f - s.averageCondition) / .72f) * .24f;
            risk += Mathf.Clamp01((s.maximumWear - .55f) / .45f) * .18f;
            risk += Mathf.Clamp01((s.dust - .12f) / .70f) * .10f;
            risk += Mathf.Clamp01((.58f - s.cableQuality) / .58f) * .08f;
            risk += Mathf.Clamp01((s.cpuPeakC - 82f) / 20f) * .12f;
            risk += Mathf.Clamp01((s.gpuPeakC - 82f) / 18f) * .10f;
            risk += Mathf.Clamp01((s.rippleMv - 70f) / 90f) * .08f;
            if (!s.stable) risk += .16f;
            if (!s.postPassed) risk += .30f;
            if (!s.osHealthy) risk += .08f;
            if (!s.deadlineMet) risk += .05f;
            risk += Mathf.Min(.30f, s.unresolvedFaults * .18f + s.unresolvedDamage * .22f);
            return Mathf.Clamp(risk, .01f, .92f);
        }

        public static float DeterministicRoll(string key)
        {
            unchecked
            {
                uint h = 2166136261u;
                string s = key ?? string.Empty;
                for (int i = 0; i < s.Length; i++) { h ^= s[i]; h *= 16777619u; }
                return (h & 0x00FFFFFFu) / 16777215f;
            }
        }

        public static string PrimaryReason(WarrantySnapshot s)
        {
            if (s == null) return "Post-service reliability concern";
            if (s.unresolvedDamage > 0) return "Latent physical damage";
            if (s.unresolvedFaults > 0) return "Recurring component fault";
            if (!s.stable) return "Intermittent stability failure";
            if (!s.postPassed) return "Boot / POST recurrence";
            if (s.cpuPeakC > 92f || s.gpuPeakC > 90f) return "Thermal regression";
            if (s.rippleMv > 105f) return "Power-quality complaint";
            if (s.averageCondition < .58f || s.maximumWear > .78f) return "Wear-related comeback";
            if (!s.osHealthy) return "Software integrity regression";
            return "Customer-reported intermittent issue";
        }
    }

    /// <summary>
    /// Tracks quality snapshots while a device is still physically present, then turns
    /// deterministic post-service risk into warranty callbacks. State is local/offline
    /// and intentionally independent from save slots so reloading cannot reroll claims.
    /// </summary>
    public sealed class WarrantyService
    {
        private readonly GameRuntime game;
        private readonly string path;
        public WarrantyDatabase State { get; private set; }

        public WarrantyService(GameRuntime runtime)
        {
            game = runtime;
            path = Path.Combine(Application.persistentDataPath, "ForgeBenchWarranty.json");
            Load();
        }

        public IReadOnlyList<WarrantyCaseState> Cases => State.cases;
        public int OpenCount => State.cases.Count(x => x.status != WarrantyCaseStatus.Resolved && x.status != WarrantyCaseStatus.Failed);
        public float Exposure => State.cases.Where(x => x.status != WarrantyCaseStatus.Resolved && x.status != WarrantyCaseStatus.Failed).Sum(x => x.estimatedLiability);
        public int ResolvedCount => State.cases.Count(x => x.status == WarrantyCaseStatus.Resolved);

        private void Load()
        {
            State = new WarrantyDatabase();
            try
            {
                if (File.Exists(path))
                {
                    WarrantyDatabase loaded = JsonUtility.FromJson<WarrantyDatabase>(File.ReadAllText(path));
                    if (loaded != null) State = loaded;
                }
            }
            catch (Exception ex) { Debug.LogWarning("[ForgeBench] Warranty database recovery: " + ex.Message); }
            if (State.processedSourceJobs == null) State.processedSourceJobs = new List<string>();
            if (State.processedCallbackJobs == null) State.processedCallbackJobs = new List<string>();
            if (State.snapshots == null) State.snapshots = new List<WarrantySnapshot>();
            if (State.cases == null) State.cases = new List<WarrantyCaseState>();
            if (State.history == null) State.history = new List<string>();
            foreach (WarrantySnapshot s in State.snapshots) if (s.warnings == null) s.warnings = new List<string>();
            foreach (WarrantyCaseState c in State.cases) if (c.history == null) c.history = new List<string>();
        }

        public void Save()
        {
            try
            {
                string tmp = path + ".tmp";
                File.WriteAllText(tmp, JsonUtility.ToJson(State, true));
                if (File.Exists(path)) File.Delete(path);
                File.Move(tmp, path);
            }
            catch (Exception ex) { Debug.LogWarning("[ForgeBench] Warranty save failed: " + ex.Message); }
        }

        public bool Poll()
        {
            bool changed = CaptureActiveMachines();
            changed |= FinalizeCompletedSourceJobs();
            changed |= ObserveCallbackJobs();
            if (State.lastObservedDay != game.State.day)
            {
                State.lastObservedDay = game.State.day;
                changed |= AdvanceCases();
            }
            if (changed) Save();
            return changed;
        }

        private bool CaptureActiveMachines()
        {
            bool changed = false;
            foreach (JobState j in game.State.jobs.Where(x => x.stage == JobStage.Accepted || x.stage == JobStage.InProgress || x.stage == JobStage.ReadyToSubmit))
            {
                if (IsWarrantyCallback(j)) continue;
                MachineState m = game.State.machines.FirstOrDefault(x => x.machineId == j.machineId);
                if (m == null) continue;
                WarrantySnapshot snap = BuildSnapshot(j, m);
                WarrantySnapshot old = State.snapshots.FirstOrDefault(x => x.jobId == j.jobId);
                if (old == null) { State.snapshots.Add(snap); changed = true; }
                else if (SnapshotSignature(old) != SnapshotSignature(snap)) { CopySnapshot(snap, old); changed = true; }
            }
            return changed;
        }

        private WarrantySnapshot BuildSnapshot(JobState j, MachineState m)
        {
            List<string> ids = Installed(m).Where(x => !string.IsNullOrEmpty(x)).ToList();
            List<ItemInstance> items = ids.Select(game.Inventory.Get).Where(x => x != null).ToList();
            WarrantySnapshot s = new WarrantySnapshot
            {
                jobId = j.jobId,
                customerName = j.customerName,
                capturedDay = game.State.day,
                averageCondition = items.Count == 0 ? 1f : items.Average(x => x.condition),
                maximumWear = items.Count == 0 ? 0f : items.Max(x => x.wear),
                unresolvedFaults = items.Count(x => x.fault != FaultType.None),
                unresolvedDamage = items.Count(x => x.damage != DamageType.None),
                dust = m.dust,
                cableQuality = m.cableManagementScore,
                benchmarkScore = m.benchmarkState != null && m.benchmarkState.totalScore > 0 ? m.benchmarkState.totalScore : m.benchmarkScore,
                cpuPeakC = m.benchmarkState != null && m.benchmarkState.peakCpuC > 0 ? m.benchmarkState.peakCpuC : m.cpuTempC,
                gpuPeakC = m.benchmarkState != null && m.benchmarkState.peakGpuC > 0 ? m.benchmarkState.peakGpuC : m.gpuTempC,
                rippleMv = m.powerState == null ? 0f : m.powerState.rippleMv,
                stable = m.stressStable && (m.benchmarkState == null || m.benchmarkState.status != BenchmarkStatus.Failed),
                postPassed = m.postCode == "A0",
                osHealthy = !m.osInstalled || m.osState == null || m.osState.systemFilesHealthy,
                deadlineMet = game.State.day <= j.dueDay
            };
            float q = 100f;
            q -= Mathf.Clamp01(s.dust) * 10f;
            q -= Mathf.Clamp01(.70f - s.cableQuality) * 16f;
            q -= Mathf.Clamp01(1f - s.averageCondition) * 24f;
            q -= Mathf.Clamp01(s.maximumWear) * 10f;
            if (!s.stable) { q -= 16f; s.warnings.Add("Stability not fully certified"); }
            if (!s.postPassed) { q -= 30f; s.warnings.Add("POST not confirmed"); }
            if (!s.osHealthy) { q -= 10f; s.warnings.Add("OS integrity warning"); }
            if (s.unresolvedFaults > 0) { q -= s.unresolvedFaults * 20f; s.warnings.Add(s.unresolvedFaults + " unresolved fault(s)"); }
            if (s.unresolvedDamage > 0) { q -= s.unresolvedDamage * 24f; s.warnings.Add(s.unresolvedDamage + " unresolved damage item(s)"); }
            if (s.cpuPeakC > 90f) s.warnings.Add("High CPU thermal peak");
            if (s.gpuPeakC > 88f) s.warnings.Add("High GPU thermal peak");
            if (s.rippleMv > 90f) s.warnings.Add("Elevated PSU ripple");
            s.qualityScore = Mathf.Clamp(q, 0f, 100f);
            return s;
        }

        private bool FinalizeCompletedSourceJobs()
        {
            bool changed = false;
            foreach (JobState j in game.State.jobs.Where(x => x.stage == JobStage.Completed || x.stage == JobStage.Failed).ToList())
            {
                if (IsWarrantyCallback(j) || string.IsNullOrEmpty(j.jobId) || State.processedSourceJobs.Contains(j.jobId)) continue;
                State.processedSourceJobs.Add(j.jobId);
                WarrantySnapshot s = State.snapshots.FirstOrDefault(x => x.jobId == j.jobId) ?? new WarrantySnapshot { jobId = j.jobId, customerName = j.customerName, capturedDay = game.State.day, deadlineMet = game.State.day <= j.dueDay, stable = j.stage == JobStage.Completed, postPassed = j.stage == JobStage.Completed };
                if (j.stage == JobStage.Completed)
                {
                    float risk = WarrantyMath.CalculateRisk(s);
                    float roll = WarrantyMath.DeterministicRoll((game.State.saveId ?? "save") + "|" + j.jobId + "|warranty-v1");
                    if (roll < risk)
                    {
                        int delay = 2 + Mathf.FloorToInt(WarrantyMath.DeterministicRoll(j.jobId + "|delay") * 6f);
                        WarrantyCaseState c = new WarrantyCaseState
                        {
                            caseId = "WC-" + State.nextCaseSerial++.ToString("D5"),
                            sourceJobId = j.jobId,
                            customerName = j.customerName,
                            deviceCategory = j.deviceCategory,
                            openedDay = game.State.day,
                            claimDay = game.State.day + delay,
                            risk = risk,
                            estimatedLiability = Mathf.Round(45f + risk * 330f),
                            status = WarrantyCaseStatus.Monitoring,
                            reason = WarrantyMath.PrimaryReason(s)
                        };
                        c.history.Add("Day " + game.State.day + ": warranty monitoring opened at " + Mathf.RoundToInt(risk * 100f) + "% modeled risk");
                        State.cases.Add(c);
                        State.history.Add(c.caseId + " · " + j.customerName + " · monitoring");
                    }
                    else State.history.Add(j.jobId + " · post-service monitoring passed without modeled comeback");
                }
                changed = true;
            }
            Trim(State.history, 160);
            return changed;
        }

        private bool AdvanceCases()
        {
            bool changed = false;
            foreach (WarrantyCaseState c in State.cases)
            {
                if (c.status == WarrantyCaseStatus.Monitoring && game.State.day >= c.claimDay)
                {
                    c.status = WarrantyCaseStatus.ClaimAvailable;
                    c.history.Add("Day " + game.State.day + ": customer reported " + c.reason);
                    changed = true;
                }
                if (c.status == WarrantyCaseStatus.CallbackOffered && !string.IsNullOrEmpty(c.callbackJobId))
                {
                    JobState j = game.State.jobs.FirstOrDefault(x => x.jobId == c.callbackJobId);
                    if (j != null && (j.stage == JobStage.Accepted || j.stage == JobStage.InProgress || j.stage == JobStage.ReadyToSubmit)) { c.status = WarrantyCaseStatus.InService; changed = true; }
                }
            }
            return changed;
        }

        public ActionResult ScheduleCallback(string caseId)
        {
            WarrantyCaseState c = State.cases.FirstOrDefault(x => x.caseId == caseId);
            if (c == null) return ActionResult.Fail("Warranty case not found.");
            if (c.status != WarrantyCaseStatus.ClaimAvailable) return ActionResult.Fail("Warranty callback is not ready to schedule.");
            int serial = game.State.nextJobSerial++;
            JobState j = new JobState
            {
                jobId = "W" + serial.ToString("D5"),
                customerName = c.customerName,
                type = JobType.Repair,
                stage = JobStage.Offered,
                deviceCategory = c.deviceCategory == DeviceCategory.Desktop ? DeviceCategory.Desktop : c.deviceCategory,
                title = "WARRANTY CALLBACK · " + c.reason,
                description = "No-charge comeback from " + c.sourceJobId + ". Diagnose the recurrence, restore reliability and document the result.",
                reward = 0f,
                budget = Mathf.Max(120f, c.estimatedLiability),
                dueDay = game.State.day + 3,
                targetBenchmark = c.deviceCategory == DeviceCategory.Desktop ? 260 : 0,
                maxNoiseDb = 50f,
                requireOs = c.deviceCategory == DeviceCategory.Desktop,
                requireDrivers = c.deviceCategory == DeviceCategory.Desktop,
                requireClean = true,
                requireStable = c.deviceCategory == DeviceCategory.Desktop,
                requireNoFaults = true
            };
            j.requiredPartCategories.Add("WARRANTY:" + c.caseId);
            j.optionalObjectives.Add("No-charge warranty service · preserve customer trust");
            j.optionalObjectives.Add("Document root cause before return");
            game.State.jobs.Add(j);
            c.callbackJobId = j.jobId;
            c.status = WarrantyCaseStatus.CallbackOffered;
            c.history.Add("Day " + game.State.day + ": callback job " + j.jobId + " created");
            State.history.Add(c.caseId + " · callback " + j.jobId + " offered");
            game.Saves?.Save(game.State, 1);
            Save();
            return ActionResult.Success("Warranty callback " + j.jobId + " created for " + c.customerName + ".");
        }

        private bool ObserveCallbackJobs()
        {
            bool changed = false;
            foreach (WarrantyCaseState c in State.cases.Where(x => !string.IsNullOrEmpty(x.callbackJobId)).ToList())
            {
                JobState j = game.State.jobs.FirstOrDefault(x => x.jobId == c.callbackJobId);
                if (j == null || State.processedCallbackJobs.Contains(j.jobId)) continue;
                if (j.stage != JobStage.Completed && j.stage != JobStage.Failed) continue;
                State.processedCallbackJobs.Add(j.jobId);
                if (j.stage == JobStage.Completed)
                {
                    c.status = WarrantyCaseStatus.Resolved;
                    c.resolvedDay = game.State.day;
                    float charge = Mathf.Min(game.State.money, c.estimatedLiability);
                    if (charge > 0f) game.Economy.Spend(charge, "Warranty callback: " + c.caseId);
                    if (charge + .01f < c.estimatedLiability) game.State.reputation = Mathf.Max(0, game.State.reputation - 3);
                    else game.State.reputation += 2;
                    c.history.Add("Day " + game.State.day + ": comeback resolved; workshop absorbed $" + charge.ToString("0"));
                }
                else
                {
                    c.status = WarrantyCaseStatus.Failed;
                    c.resolvedDay = game.State.day;
                    game.State.reputation = Mathf.Max(0, game.State.reputation - 18);
                    c.history.Add("Day " + game.State.day + ": warranty callback failed; reputation penalty applied");
                }
                game.Saves?.Save(game.State, 1);
                changed = true;
            }
            return changed;
        }

        private static bool IsWarrantyCallback(JobState j) => j?.requiredPartCategories != null && j.requiredPartCategories.Any(x => x.StartsWith("WARRANTY:", StringComparison.Ordinal));

        private static IEnumerable<string> Installed(MachineState m)
        {
            if (m == null) yield break;
            yield return m.caseItemId; yield return m.motherboardItemId; yield return m.cpuItemId; yield return m.gpuItemId; yield return m.psuItemId; yield return m.coolerItemId;
            if (m.ramItemIds != null) foreach (string x in m.ramItemIds) yield return x;
            if (m.storageItemIds != null) foreach (string x in m.storageItemIds) yield return x;
            if (m.fanItemIds != null) foreach (string x in m.fanItemIds) yield return x;
        }

        private static string SnapshotSignature(WarrantySnapshot s)
        {
            return s.capturedDay + "|" + s.qualityScore.ToString("0.0") + "|" + s.averageCondition.ToString("0.000") + "|" + s.maximumWear.ToString("0.000") + "|" + s.dust.ToString("0.000") + "|" + s.cableQuality.ToString("0.000") + "|" + s.benchmarkScore.ToString("0.0") + "|" + s.stable + "|" + s.postPassed + "|" + s.osHealthy + "|" + s.unresolvedFaults + "|" + s.unresolvedDamage;
        }

        private static void CopySnapshot(WarrantySnapshot from, WarrantySnapshot to)
        {
            to.customerName = from.customerName; to.capturedDay = from.capturedDay; to.qualityScore = from.qualityScore; to.averageCondition = from.averageCondition; to.maximumWear = from.maximumWear; to.dust = from.dust; to.cableQuality = from.cableQuality; to.benchmarkScore = from.benchmarkScore; to.cpuPeakC = from.cpuPeakC; to.gpuPeakC = from.gpuPeakC; to.rippleMv = from.rippleMv; to.stable = from.stable; to.postPassed = from.postPassed; to.osHealthy = from.osHealthy; to.deadlineMet = from.deadlineMet; to.unresolvedFaults = from.unresolvedFaults; to.unresolvedDamage = from.unresolvedDamage; to.warnings = from.warnings ?? new List<string>();
        }

        private static void Trim(List<string> list, int max) { if (list != null && list.Count > max) list.RemoveRange(0, list.Count - max); }
    }
}
