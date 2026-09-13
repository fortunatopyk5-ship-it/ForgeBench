using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    public enum DispatchRisk { Ready, Normal, Watch, High, Overdue, Failed }

    public sealed class DispatchSnapshot
    {
        public int capacityUnits;
        public int usedUnits;
        public int freeUnits;
        public int activeJobs;
        public int overdueJobs;
        public JobState recommended;
    }

    /// <summary>
    /// Multi-job workshop orchestration layered over the legacy single-bench job services.
    /// The underlying services remain the authority for creating customer machines and specialist
    /// contracts; dispatch temporarily suppresses their old single-active guard transactionally,
    /// then restores every existing job stage and focuses the newly accepted contract.
    /// </summary>
    public sealed class JobDispatchService
    {
        private readonly GameState state;
        private readonly JobService standardJobs;
        private readonly SpecialistJobService specialistJobs;

        private sealed class StageSnapshot
        {
            public JobState job;
            public JobStage stage;
        }

        public JobDispatchService(GameState gameState, JobService standard, SpecialistJobService specialist)
        {
            state = gameState;
            standardJobs = standard;
            specialistJobs = specialist;
        }

        public static bool IsActive(JobState j)
        {
            return j != null && (j.stage == JobStage.Accepted || j.stage == JobStage.InProgress || j.stage == JobStage.ReadyToSubmit);
        }

        public int CapacityUnits
        {
            get
            {
                if (state == null || state.workshop == null) return 1;
                int benches = Mathf.Max(0, state.workshop.benchLevel - 1);
                int staff = Mathf.Max(0, state.workshop.helperStaff) + Mathf.Max(0, state.workshop.specialistStaff);
                return Mathf.Clamp(2 + benches + staff, 2, 8);
            }
        }

        public int UsedUnits => ActiveJobs.Sum(WorkloadUnits);
        public int FreeUnits => Mathf.Max(0, CapacityUnits - UsedUnits);
        public List<JobState> ActiveJobs => state == null ? new List<JobState>() : state.jobs.Where(IsActive).ToList();

        public int WorkloadUnits(JobState j)
        {
            if (!SpecialistJobService.IsSpecialist(j)) return 1;
            SpecialistContractKind kind;
            return TryKind(j, out kind) ? SpecialistWorkload(kind) : 2;
        }

        public static int SpecialistWorkload(SpecialistContractKind kind)
        {
            switch (kind)
            {
                case SpecialistContractKind.LiquidBuild:
                case SpecialistContractKind.BoardRepair:
                case SpecialistContractKind.NasRecovery:
                case SpecialistContractKind.ServerNetwork:
                    return 2;
                default:
                    return 1;
            }
        }

        public bool CanAcceptStandard(out string reason)
        {
            return CanFit(1, out reason);
        }

        public bool CanAcceptSpecialist(SpecialistContractKind kind, out string reason)
        {
            return CanFit(SpecialistWorkload(kind), out reason);
        }

        private bool CanFit(int units, out string reason)
        {
            int free = FreeUnits;
            if (free < units)
            {
                reason = "Workshop dispatch capacity is full: " + UsedUnits + "/" + CapacityUnits + " units in use; this contract needs " + units + ". Upgrade benches or staff, or finish an active job.";
                return false;
            }
            reason = string.Empty;
            return true;
        }

        public ActionResult AcceptStandard(JobState offered)
        {
            if (offered == null || offered.stage != JobStage.Offered) return ActionResult.Fail("Job is no longer available.");
            string reason;
            if (!CanAcceptStandard(out reason)) return ActionResult.Fail(reason);
            if (standardJobs == null) return ActionResult.Fail("Standard job service is unavailable.");
            List<StageSnapshot> old = SuppressLegacyActiveGuard();
            ActionResult result;
            try { result = standardJobs.Accept(offered); }
            finally { RestoreStages(old); }
            if (result.ok)
            {
                Focus(offered);
                result = ActionResult.Success(result.message + " Dispatch load " + UsedUnits + "/" + CapacityUnits + ".");
            }
            return result;
        }

        public ActionResult AcceptSpecialist(SpecialistContractKind kind)
        {
            string reason;
            if (!CanAcceptSpecialist(kind, out reason)) return ActionResult.Fail(reason);
            if (specialistJobs == null) return ActionResult.Fail("Specialist job service is unavailable.");
            HashSet<string> before = new HashSet<string>(state.jobs.Where(x => x != null).Select(x => x.jobId));
            List<StageSnapshot> old = SuppressLegacyActiveGuard();
            ActionResult result;
            try { result = specialistJobs.Accept(kind); }
            finally { RestoreStages(old); }
            if (result.ok)
            {
                JobState created = state.jobs.FirstOrDefault(x => x != null && !before.Contains(x.jobId) && IsActive(x));
                if (created != null) Focus(created);
                result = ActionResult.Success(result.message + " Dispatch load " + UsedUnits + "/" + CapacityUnits + ".");
            }
            return result;
        }

        private List<StageSnapshot> SuppressLegacyActiveGuard()
        {
            List<StageSnapshot> snapshots = new List<StageSnapshot>();
            foreach (JobState j in state.jobs.Where(IsActive).ToList())
            {
                snapshots.Add(new StageSnapshot { job = j, stage = j.stage });
                j.stage = JobStage.Offered;
            }
            return snapshots;
        }

        private static void RestoreStages(List<StageSnapshot> snapshots)
        {
            if (snapshots == null) return;
            foreach (StageSnapshot s in snapshots)
                if (s != null && s.job != null) s.job.stage = s.stage;
        }

        public ActionResult Focus(JobState job)
        {
            if (state == null || job == null || !IsActive(job)) return ActionResult.Fail("Only an active contract can be focused.");
            int index = state.jobs.IndexOf(job);
            if (index < 0) return ActionResult.Fail("Contract is not part of this workshop state.");
            if (index > 0)
            {
                state.jobs.RemoveAt(index);
                state.jobs.Insert(0, job);
            }
            return ActionResult.Success("Focused " + job.jobId + " · " + job.title + ".");
        }

        public ActionResult FocusNext()
        {
            List<JobState> active = ActiveJobs;
            if (active.Count == 0) return ActionResult.Fail("No active contracts.");
            if (active.Count == 1) return Focus(active[0]);
            JobState current = state.jobs.FirstOrDefault(IsActive);
            int i = active.IndexOf(current);
            return Focus(active[(i + 1 + active.Count) % active.Count]);
        }

        public JobState RecommendedJob()
        {
            return ActiveJobs.OrderByDescending(PriorityScore).FirstOrDefault();
        }

        public ActionResult FocusRecommended()
        {
            JobState j = RecommendedJob();
            return j == null ? ActionResult.Fail("No active contracts to triage.") : Focus(j);
        }

        public DispatchRisk Risk(JobState j)
        {
            if (j == null || j.stage == JobStage.Failed) return DispatchRisk.Failed;
            if (j.stage == JobStage.ReadyToSubmit) return DispatchRisk.Ready;
            int left = j.dueDay - state.day;
            if (left < 0) return DispatchRisk.Overdue;
            if (left == 0) return DispatchRisk.High;
            if (left <= 2) return DispatchRisk.Watch;
            return DispatchRisk.Normal;
        }

        public int DaysRemaining(JobState j) => j == null ? 0 : j.dueDay - state.day;

        public float PriorityScore(JobState j)
        {
            if (j == null || !IsActive(j)) return float.MinValue;
            float score = 0f;
            if (j.stage == JobStage.ReadyToSubmit) score += 10000f;
            else if (j.stage == JobStage.InProgress) score += 400f;
            else score += 200f;
            int left = j.dueDay - state.day;
            if (left < 0) score += 5000f + Mathf.Abs(left) * 500f;
            else score += Mathf.Max(0, 5 - left) * 700f;
            score += Mathf.Clamp(j.reward, 0f, 5000f) * .08f;
            if (SpecialistJobService.IsSpecialist(j)) score += 80f;
            return score;
        }

        public DispatchSnapshot Snapshot()
        {
            List<JobState> active = ActiveJobs;
            return new DispatchSnapshot
            {
                capacityUnits = CapacityUnits,
                usedUnits = UsedUnits,
                freeUnits = FreeUnits,
                activeJobs = active.Count,
                overdueJobs = active.Count(x => Risk(x) == DispatchRisk.Overdue),
                recommended = RecommendedJob()
            };
        }

        public static bool TryKind(JobState j, out SpecialistContractKind kind)
        {
            kind = default(SpecialistContractKind);
            if (!SpecialistJobService.IsSpecialist(j)) return false;
            string marker = j.requiredPartCategories.FirstOrDefault(x => x.StartsWith("SPECIALIST:", StringComparison.Ordinal));
            return !string.IsNullOrEmpty(marker) && Enum.TryParse(marker.Substring("SPECIALIST:".Length), out kind);
        }
    }
}
