using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class RuntimeRenderBudgetTests
    {
        [TearDown]
        public void ResetTier(){RuntimeRenderBudget.SetTier(2);}

        [TestCase(0,2,0,1,.62f)]
        [TestCase(1,3,32,1,.78f)]
        [TestCase(2,5,64,0,1f)]
        [TestCase(3,8,128,0,1.18f)]
        public void Tier_MapsToConcretePresentationBudgets(int tier,int lights,int probeResolution,int mipLimit,float detailFactor)
        {
            RuntimeRenderBudget.SetTier(tier);
            Assert.AreEqual(tier,RuntimeRenderBudget.CurrentTier);
            Assert.AreEqual(lights,RuntimeRenderBudget.RealtimeLightBudget);
            Assert.AreEqual(probeResolution,RuntimeRenderBudget.ReflectionProbeResolution);
            Assert.AreEqual(mipLimit,RuntimeRenderBudget.TextureMipmapLimit);
            Assert.AreEqual(detailFactor,RuntimeRenderBudget.DetailDistanceFactor,.001f);
        }

        [Test]
        public void Tier_IsClampedToSupportedRange()
        {
            RuntimeRenderBudget.SetTier(-50);
            Assert.AreEqual(0,RuntimeRenderBudget.CurrentTier);
            RuntimeRenderBudget.SetTier(50);
            Assert.AreEqual(3,RuntimeRenderBudget.CurrentTier);
        }

        [Test]
        public void HigherTier_NeverReducesVisualBudgets()
        {
            int lastLights=0,lastProbe=0;
            float lastDistance=0f,lastLod=0f;
            for(int tier=0;tier<=3;tier++)
            {
                RuntimeRenderBudget.SetTier(tier);
                Assert.GreaterOrEqual(RuntimeRenderBudget.RealtimeLightBudget,lastLights);
                Assert.GreaterOrEqual(RuntimeRenderBudget.ReflectionProbeResolution,lastProbe);
                Assert.GreaterOrEqual(RuntimeRenderBudget.DetailDistanceFactor,lastDistance);
                Assert.GreaterOrEqual(RuntimeRenderBudget.LodBias,lastLod);
                lastLights=RuntimeRenderBudget.RealtimeLightBudget;
                lastProbe=RuntimeRenderBudget.ReflectionProbeResolution;
                lastDistance=RuntimeRenderBudget.DetailDistanceFactor;
                lastLod=RuntimeRenderBudget.LodBias;
            }
        }
    }
}
