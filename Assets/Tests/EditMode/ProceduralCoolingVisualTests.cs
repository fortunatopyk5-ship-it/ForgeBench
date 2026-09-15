using System.Collections.Generic;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class ProceduralCoolingVisualTests
    {
        [TestCase("cooler_rad240", "AIO 240", 360, 240)]
        [TestCase("cooler_rad280", "AIO 280", 360, 280)]
        [TestCase("cooler_rad360", "AIO 360", 360, 360)]
        public void ExplicitRadiatorIdentity_WinsOverNormalizedFallback(string id, string model, int normalized, int expected)
        {
            HardwareDefinition d = new HardwareDefinition
            {
                id = id,
                model = model,
                radiatorSupportMm = normalized,
                tags = new List<string> { "aio" }
            };
            Assert.AreEqual(expected, ProceduralCoolingVisuals.ResolveRadiatorMm(d));
        }

        [Test]
        public void RadiatorFallback_MapsToSupportedFamilies()
        {
            HardwareDefinition d = new HardwareDefinition { id = "cooler_x", model = "Liquid X", radiatorSupportMm = 275, tags = new List<string> { "aio" } };
            Assert.AreEqual(280, ProceduralCoolingVisuals.ResolveRadiatorMm(d));
            d.radiatorSupportMm = 340;
            Assert.AreEqual(360, ProceduralCoolingVisuals.ResolveRadiatorMm(d));
            d.radiatorSupportMm = 220;
            Assert.AreEqual(240, ProceduralCoolingVisuals.ResolveRadiatorMm(d));
        }

        [Test]
        public void FanTag_WinsOverDefaultedFanSize()
        {
            HardwareDefinition d = new HardwareDefinition
            {
                id = "fan_large",
                model = "Flow 140",
                fanSizeMm = 120,
                tags = new List<string> { "140mm" }
            };
            Assert.AreEqual(140, ProceduralCoolingVisuals.ResolveFanMm(d));
        }

        [Test]
        public void AioAndRgbDetection_AreCaseInsensitive()
        {
            HardwareDefinition d = new HardwareDefinition
            {
                id = "c1",
                model = "Aurora Liquid",
                tags = new List<string> { "AIO", "ARGB" }
            };
            Assert.IsTrue(ProceduralCoolingVisuals.IsAio(d));
            Assert.IsTrue(ProceduralCoolingVisuals.HasRgb(d));
        }
    }
}
