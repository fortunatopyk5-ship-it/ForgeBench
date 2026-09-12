using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    [Serializable]
    public sealed class CustomerProfile
    {
        public string customerId;
        public string name;
        public int jobsCompleted;
        public int jobsFailed;
        public int complaints;
        public int repeatJobs;
        public int referrals;
        public int lastServiceDay;
        public float lifetimeValue;
        public float trust = 50f;
        public float loyalty = 35f;
        public float satisfaction = 70f;
        public bool vip;
        public bool corporate;
        public string preferredDevice;
        public string notes;
        public List<string> history = new List<string>();
    }

    [Serializable]
    public sealed class CustomerRelationsState
    {
        public int schemaVersion = 1;
        public int lastObservedDay;
        public int reviewScore = 50;
        public int fiveStarReviews;
        public int oneStarReviews;
        public int referralCount;
        public List<string> processedJobs = new List<string>();
        public List<CustomerProfile> customers = new List<CustomerProfile>();
        public List<string> history = new List<string>();
    }

    /// <summary>
    /// Persistent customer layer independent from the normal save slots, so profiles
    /// survive workshop save-slot switching while still being local/offline. Completed
    /// and failed jobs are consumed exactly once and may generate repeat/VIP business.
    /// </summary>
    public sealed class CustomerRelationsService
    {
        private readonly string path;
        private readonly GameState game;
        public CustomerRelationsState State { get; private set; }

        public CustomerRelationsService(GameState state)
        {
            game = state;
            path = Path.Combine(Application.persistentDataPath, "ForgeBenchCustomers.json");
            Load();
        }

        public IReadOnlyList<CustomerProfile> Profiles => State.customers;
        public float AverageSatisfaction => State.customers.Count == 0 ? 70f : State.customers.Average(x => x.satisfaction);
        public float AverageTrust => State.customers.Count == 0 ? 50f : State.customers.Average(x => x.trust);
        public int VipCount => State.customers.Count(x => x.vip);
        public int CorporateCount => State.customers.Count(x => x.corporate);

        private void Load()
        {
            State = new CustomerRelationsState();
            try
            {
                if (File.Exists(path))
                {
                    CustomerRelationsState loaded = JsonUtility.FromJson<CustomerRelationsState>(File.ReadAllText(path));
                    if (loaded != null) State = loaded;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[ForgeBench] Customer relations recovery: " + ex.Message);
            }
            if (State.customers == null) State.customers = new List<CustomerProfile>();
            if (State.processedJobs == null) State.processedJobs = new List<string>();
            if (State.history == null) State.history = new List<string>();
            foreach (CustomerProfile p in State.customers) if (p.history == null) p.history = new List<string>();
        }

        public void Save()
        {
            try
            {
                string temp = path + ".tmp";
                File.WriteAllText(temp, JsonUtility.ToJson(State, true));
                if (File.Exists(path)) File.Delete(path);
                File.Move(temp, path);
            }
            catch (Exception ex) { Debug.LogWarning("[ForgeBench] Customer relations save failed: " + ex.Message); }
        }

        public bool ObserveJobs()
        {
            bool changed = false;
            foreach (JobState j in game.jobs.Where(j => j.stage == JobStage.Completed || j.stage == JobStage.Failed).ToList())
            {
                if (string.IsNullOrEmpty(j.jobId) || State.processedJobs.Contains(j.jobId)) continue;
                Process(j);
                State.processedJobs.Add(j.jobId);
                changed = true;
            }
            if (changed) Save();
            return changed;
        }

        private void Process(JobState j)
        {
            CustomerProfile p = GetOrCreate(j.customerName);
            p.lastServiceDay = game.day;
            bool complete = j.stage == JobStage.Completed;
            if (complete)
            {
                p.jobsCompleted++;
                p.lifetimeValue += Mathf.Max(0f, j.reward);
                float punctual = game.day <= j.dueDay ? 8f : Mathf.Max(-14f, -(game.day - j.dueDay) * 4f);
                float specialist = j.requiredPartCategories != null && j.requiredPartCategories.Any(x => x.StartsWith("SPECIALIST:", StringComparison.Ordinal)) ? 4f : 0f;
                float quality = 8f + punctual + specialist;
                p.satisfaction = Mathf.Clamp(p.satisfaction + quality * .65f, 0f, 100f);
                p.trust = Mathf.Clamp(p.trust + quality * .45f, 0f, 100f);
                p.loyalty = Mathf.Clamp(p.loyalty + 7f + Mathf.Max(0f, punctual * .35f), 0f, 100f);
                if (p.satisfaction >= 88f) { State.fiveStarReviews++; State.reviewScore = Mathf.Clamp(State.reviewScore + 2, 0, 100); }
                if (p.trust >= 82f && p.loyalty >= 78f) p.vip = true;
                if (p.jobsCompleted >= 4 && p.lifetimeValue >= 2500f) p.corporate = true;
                if (p.jobsCompleted > 1) p.repeatJobs++;
                if (p.trust >= 85f && ((p.jobsCompleted + game.day) % 3 == 0)) { p.referrals++; State.referralCount++; }
                string entry = "Day " + game.day + ": completed " + j.jobId + " · satisfaction " + p.satisfaction.ToString("0") + "%";
                p.history.Add(entry); State.history.Add(p.name + " · " + entry);
            }
            else
            {
                p.jobsFailed++; p.complaints++;
                p.satisfaction = Mathf.Max(0f, p.satisfaction - 24f);
                p.trust = Mathf.Max(0f, p.trust - 18f);
                p.loyalty = Mathf.Max(0f, p.loyalty - 12f);
                State.oneStarReviews++; State.reviewScore = Mathf.Clamp(State.reviewScore - 5, 0, 100);
                string entry = "Day " + game.day + ": failed " + j.jobId + " · complaint opened";
                p.history.Add(entry); State.history.Add(p.name + " · " + entry);
            }
            Trim(p.history, 40); Trim(State.history, 100);
        }

        public CustomerProfile GetOrCreate(string name)
        {
            string safe = string.IsNullOrWhiteSpace(name) ? "Walk-in customer" : name.Trim();
            CustomerProfile p = State.customers.FirstOrDefault(x => string.Equals(x.name, safe, StringComparison.OrdinalIgnoreCase));
            if (p != null) return p;
            p = new CustomerProfile
            {
                customerId = "CUST-" + Math.Abs(safe.GetHashCode()).ToString("X8"),
                name = safe,
                preferredDevice = GuessPreference(safe),
                notes = "First-time customer"
            };
            State.customers.Add(p);
            Save();
            return p;
        }

        public ActionResult ResolveComplaint(GameRuntime runtime, string customerId)
        {
            CustomerProfile p = State.customers.FirstOrDefault(x => x.customerId == customerId);
            if (p == null || p.complaints <= 0) return ActionResult.Fail("No unresolved customer complaint found.");
            float goodwill = 35f + p.complaints * 12f;
            ActionResult pay = runtime.Economy.Spend(goodwill, "Customer goodwill: " + p.name);
            if (!pay.ok) return pay;
            p.complaints--;
            p.satisfaction = Mathf.Clamp(p.satisfaction + 12f, 0f, 100f);
            p.trust = Mathf.Clamp(p.trust + 8f, 0f, 100f);
            p.history.Add("Day " + game.day + ": complaint resolved with goodwill service");
            State.reviewScore = Mathf.Clamp(State.reviewScore + 1, 0, 100);
            Save();
            return ActionResult.Success("Complaint resolved for " + p.name + ". Trust partially restored.");
        }

        public ActionResult InviteRepeatCustomer(CustomerProfile p)
        {
            if (p == null) return ActionResult.Fail("Customer profile missing.");
            if (game.jobs.Any(j => j.stage == JobStage.Accepted || j.stage == JobStage.InProgress || j.stage == JobStage.ReadyToSubmit))
                return ActionResult.Fail("Finish the active job before scheduling a repeat customer.");
            if (game.jobs.Count(j => j.stage == JobStage.Offered && j.customerName == p.name) > 0)
                return ActionResult.Fail("This customer already has an open offer.");
            if (p.trust < 55f) return ActionResult.Fail("Customer trust is too low for proactive repeat business.");

            int serial = game.nextJobSerial++;
            int tier = Mathf.Clamp(1 + Mathf.RoundToInt((p.trust + p.loyalty) / 45f), 1, 5);
            JobType type = p.preferredDevice == "Network/NAS" ? JobType.Network : (serial % 2 == 0 ? JobType.Upgrade : JobType.Repair);
            JobState j = new JobState
            {
                jobId = "R" + serial.ToString("D5"), customerName = p.name, type = type, stage = JobStage.Offered,
                deviceCategory = type == JobType.Network ? DeviceCategory.NAS : DeviceCategory.Desktop,
                title = p.vip ? "VIP priority return service" : "Repeat customer service",
                description = "Returning customer · trust " + p.trust.ToString("0") + "% · loyalty " + p.loyalty.ToString("0") + "%.",
                dueDay = game.day + Mathf.Max(3, 6 - tier), budget = 650 + tier * 320,
                reward = (p.vip ? 1.35f : p.corporate ? 1.22f : 1.12f) * (300 + tier * 160),
                targetBenchmark = type == JobType.Network ? 0 : 330 + tier * 110,
                maxNoiseDb = 48 - tier, requireOs = type != JobType.Network, requireDrivers = type != JobType.Network,
                requireStable = type != JobType.Network, requireNoFaults = true, hiddenPreferenceLowNoise = p.vip
            };
            j.optionalObjectives.Add("Preserve established customer trust");
            if (p.vip) j.optionalObjectives.Add("VIP: priority completion before deadline");
            if (p.corporate) j.optionalObjectives.Add("Corporate account: zero unresolved faults");
            game.jobs.Add(j);
            p.history.Add("Day " + game.day + ": invited back as repeat customer");
            Save();
            return ActionResult.Success("Repeat offer created for " + p.name + ".");
        }

        public void DailyTick(GameRuntime runtime)
        {
            if (State.lastObservedDay == game.day) return;
            int elapsed = State.lastObservedDay <= 0 ? 1 : Mathf.Clamp(game.day - State.lastObservedDay, 1, 30);
            State.lastObservedDay = game.day;
            foreach (CustomerProfile p in State.customers)
            {
                if (game.day - p.lastServiceDay > 18)
                    p.loyalty = Mathf.Max(0f, p.loyalty - .20f * elapsed);
                if (p.complaints > 0)
                    p.trust = Mathf.Max(0f, p.trust - .15f * elapsed);
            }
            if (runtime != null && State.reviewScore >= 80 && game.day % 5 == 0)
            {
                runtime.State.reputation += 1;
                State.history.Add("Day " + game.day + ": strong review score generated organic reputation");
            }
            Save();
        }

        private static string GuessPreference(string name)
        {
            int v = Math.Abs((name ?? string.Empty).GetHashCode()) % 5;
            return v == 0 ? "Quiet desktop" : v == 1 ? "Performance desktop" : v == 2 ? "Portable" : v == 3 ? "Network/NAS" : "Repair";
        }
        private static void Trim(List<string> list, int max) { if (list != null && list.Count > max) list.RemoveRange(0, list.Count - max); }
    }
}
