using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class WarrantyServiceTests
    {
        private static WarrantySnapshot Healthy()
        {
            return new WarrantySnapshot
            {
                qualityScore = 96f,
                averageCondition = .96f,
                maximumWear = .08f,
                dust = .04f,
                cableQuality = .92f,
                cpuPeakC = 72f,
                gpuPeakC = 70f,
                rippleMv = 38f,
                stable = true,
                postPassed = true,
                osHealthy = true,
                deadlineMet = true
            };
        }

        [Test]
        public void Risk_IsBoundedAndHealthyServiceStaysLow()
        {
            float risk = WarrantyMath.CalculateRisk(Healthy());
            Assert.That(risk, Is.InRange(.01f,.92f));
            Assert.Less(risk,.08f);
        }

        [Test]
        public void Risk_IncreasesForWearThermalsFaultsAndInstability()
        {
            WarrantySnapshot healthy = Healthy();
            WarrantySnapshot bad = Healthy();
            bad.qualityScore = 48f;
            bad.averageCondition = .43f;
            bad.maximumWear = .91f;
            bad.dust = .78f;
            bad.cableQuality = .30f;
            bad.cpuPeakC = 101f;
            bad.gpuPeakC = 96f;
            bad.rippleMv = 142f;
            bad.stable = false;
            bad.postPassed = false;
            bad.osHealthy = false;
            bad.deadlineMet = false;
            bad.unresolvedFaults = 1;
            bad.unresolvedDamage = 1;
            Assert.Greater(WarrantyMath.CalculateRisk(bad),WarrantyMath.CalculateRisk(healthy)+.45f);
            Assert.That(WarrantyMath.CalculateRisk(bad),Is.InRange(.01f,.92f));
        }

        [Test]
        public void DeterministicRoll_IsStableAcrossReloadEquivalentCalls()
        {
            float a = WarrantyMath.DeterministicRoll("save-abc|J00042|warranty-v1");
            float b = WarrantyMath.DeterministicRoll("save-abc|J00042|warranty-v1");
            float c = WarrantyMath.DeterministicRoll("save-abc|J00043|warranty-v1");
            Assert.AreEqual(a,b);
            Assert.AreNotEqual(a,c);
            Assert.That(a,Is.InRange(0f,1f));
            Assert.That(c,Is.InRange(0f,1f));
        }

        [Test]
        public void PrimaryReason_PrioritizesPhysicalThenFaultThenStability()
        {
            WarrantySnapshot s = Healthy();
            s.unresolvedDamage = 1; s.unresolvedFaults = 1; s.stable = false;
            Assert.AreEqual("Latent physical damage",WarrantyMath.PrimaryReason(s));
            s.unresolvedDamage = 0;
            Assert.AreEqual("Recurring component fault",WarrantyMath.PrimaryReason(s));
            s.unresolvedFaults = 0;
            Assert.AreEqual("Intermittent stability failure",WarrantyMath.PrimaryReason(s));
        }
    }
}
