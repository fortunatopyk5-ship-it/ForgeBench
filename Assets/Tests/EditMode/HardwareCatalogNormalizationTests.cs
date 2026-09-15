using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class HardwareCatalogNormalizationTests
    {
        private static HardwareCatalog LoadCatalog()
        {
            HardwareCatalog catalog = new HardwareCatalog();
            catalog.Load();
            return catalog;
        }

        [Test]
        public void Aio_RadiatorTagsOverridePerformanceHeuristic()
        {
            HardwareCatalog catalog = LoadCatalog();
            Assert.AreEqual(240, catalog.Get("aio_1").radiatorSupportMm);
            Assert.AreEqual(280, catalog.Get("aio_2").radiatorSupportMm);
            Assert.AreEqual(360, catalog.Get("aio_3").radiatorSupportMm);
        }

        [Test]
        public void Aio_FanSizeTracksRadiatorFamily()
        {
            HardwareCatalog catalog = LoadCatalog();
            Assert.AreEqual(120, catalog.Get("aio_1").fanSizeMm);
            Assert.AreEqual(140, catalog.Get("aio_2").fanSizeMm);
            Assert.AreEqual(120, catalog.Get("aio_3").fanSizeMm);
        }

        [Test]
        public void StandaloneFan_SizeComesFromCatalogTag()
        {
            HardwareCatalog catalog = LoadCatalog();
            Assert.AreEqual(140, catalog.Get("fan_3").fanSizeMm);
            Assert.AreEqual(120, catalog.Get("fan_4").fanSizeMm);
            Assert.AreEqual(140, catalog.Get("fan_5").fanSizeMm);
        }

        [Test]
        public void ExplicitSizesAreNeverOverwritten()
        {
            HardwareCatalog catalog = LoadCatalog();
            foreach (HardwareDefinition fan in catalog.ByCategory(PartCategory.Fan))
                Assert.Greater(fan.fanSizeMm, 0, fan.id + " must have a normalized fan size.");
            foreach (HardwareDefinition cooler in catalog.ByCategory(PartCategory.Cooler))
                Assert.Greater(cooler.fanSizeMm, 0, cooler.id + " must have a normalized fan size.");
        }
    }
}
