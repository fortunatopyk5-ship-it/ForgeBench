using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ForgeBench
{
    /// <summary>
    /// Deterministic reputation-scaled procedural contract generator. Generated jobs
    /// persist as ordinary JobState and therefore use the normal accept/save/submit loop.
    /// </summary>
    public sealed class AdvancedJobGeneratorService
    {
        private readonly GameState state;
        private static readonly string[] Customers={"Ari Vale","Mila North","Taras Bond","Lena Falk","Marko Venn","Oksana Reed","Theo Glass","Yana Shore","Sam Kestrel","Nika Sol"};
        public AdvancedJobGeneratorService(GameState s){state=s;}

        public void EnsureOffers(int desired=3)
        {
            foreach(JobState old in state.jobs.Where(x=>x.stage==JobStage.Offered&&IsAdvanced(x)&&x.dueDay<state.day).ToList())state.jobs.Remove(old);
            while(state.jobs.Count(x=>x.stage==JobStage.Offered&&IsAdvanced(x))<desired)state.jobs.Add(Generate());
        }

        private JobState Generate()
        {
            int serial=state.nextJobSerial++;int tier=Mathf.Clamp(1+state.reputation/120,1,7);int seed=serial*7919+state.day*104729+state.reputation*17;System.Random rng=new System.Random(seed);
            JobType[] types={JobType.Repair,JobType.CustomBuild,JobType.Upgrade,JobType.Software,JobType.Cleaning,JobType.Diagnostics};JobType type=types[Math.Abs(seed)%types.Length];
            bool lowNoise=rng.NextDouble()<.34;bool clean=rng.NextDouble()<.32||type==JobType.Cleaning;bool stable=type!=JobType.Cleaning;int target=type==JobType.Cleaning?0:Mathf.RoundToInt(250+tier*95+(float)rng.NextDouble()*110);float budget=type==JobType.CustomBuild?650+tier*330:380+tier*150;float reward=220+tier*125+(type==JobType.Repair?110:0)+(lowNoise?55:0);
            JobState j=new JobState{jobId="A"+serial.ToString("D5"),customerName=Customers[Math.Abs(seed/11)%Customers.Length],type=type,stage=JobStage.Offered,deviceCategory=DeviceCategory.Desktop,dueDay=state.day+3+tier+rng.Next(0,3),budget=budget,reward=reward,targetBenchmark=target,maxNoiseDb=Mathf.Clamp(51-tier-rng.Next(0,4),36,50),requireOs=type!=JobType.Cleaning,requireDrivers=type!=JobType.Cleaning,requireClean=clean,requireStable=stable,requireNoFaults=type==JobType.Repair||type==JobType.Diagnostics,hiddenPreferenceLowNoise=lowNoise};
            j.optionalObjectives.Add("ADVANCED_CONTRACT");
            if(lowNoise)j.optionalObjectives.Add("Acoustic target ≤ "+j.maxNoiseDb.ToString("0")+" dB");
            if(rng.NextDouble()<.45)j.optionalObjectives.Add("Preserve customer-owned components where practical");
            if(rng.NextDouble()<.35)j.optionalObjectives.Add("Complete at least one day before deadline");
            if(type==JobType.CustomBuild){j.title=Pick(rng,"Quiet creator workstation","Compact gaming build","Balanced production PC","Efficient daily workstation");j.description="Build a complete compatible PC within $"+budget.ToString("0")+" and reach "+target+" benchmark points.";j.requiredPartCategories.AddRange(new[]{"Case","Motherboard","CPU","RAM","Storage","PSU","Cooler"});}
            else if(type==JobType.Upgrade){j.title=Pick(rng,"Frame-rate uplift","Creator performance refresh","Storage and responsiveness upgrade","Thermal-safe performance upgrade");j.description="Upgrade the customer desktop to at least "+target+" points without introducing instability.";}
            else if(type==JobType.Software){j.title=Pick(rng,"Driver recovery and validation","ForgeOS stability service","Boot and software repair");j.description="Restore the software stack, drivers and stable benchmark validation.";}
            else if(type==JobType.Cleaning){j.title=Pick(rng,"Dust remediation","Preventive cleaning service","Airflow restoration");j.description="Remove heavy contamination, verify cooling airflow and return the system clean.";}
            else if(type==JobType.Diagnostics){j.title=Pick(rng,"Intermittent crash investigation","Boot fault diagnosis","Thermal instability investigation");j.description="Diagnose the reported instability and return the machine fault-free and stress-stable.";}
            else{j.title=Pick(rng,"Unstable desktop repair","POST failure investigation","Random shutdown repair");j.description="Find and correct the hardware fault, then prove stability and reach "+target+" points.";}
            return j;
        }

        private static string Pick(System.Random r,params string[] values)=>values[r.Next(values.Length)];
        public static bool IsAdvanced(JobState j)=>j!=null&&j.optionalObjectives!=null&&j.optionalObjectives.Contains("ADVANCED_CONTRACT");
    }
}
