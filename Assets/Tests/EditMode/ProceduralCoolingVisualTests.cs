using System.Collections.Generic;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class ProceduralCoolingVisualTests
    {
        [TestCase("cooler_rad240", "AIO 240", 360, 240, 2)]
        [TestCase("cooler_rad280", "AIO 280", 360, 280, 2)]
        [TestCase("cooler_rad360", "AIO 360", 240, 360, 3)]
        public void ExplicitRadiatorIdentity_WinsOverNormalizedFallback(string id, string model, int normalized, int expected, int expectedFans)
        {
            HardwareDefinition d = new HardwareDefinition
            {
                id = id,
                model = model,
                radiatorSupportMm = normalized,
                tags = new List<string> { "aio" }
            };
            Assert.AreEqual(expected, ProceduralCoolingVisuals.ResolveRadiatorMm(d));
            Assert.AreEqual(expected, HardwarePresentationLayout.ResolveRadiatorMm(d));
            Assert.AreEqual(expectedFans, HardwarePresentationLayout.AioFanCount(d));
        }

        [Test]
        public void RadiatorFallback_MapsToSupportedFamilies()
        {
            HardwareDefinition d = new HardwareDefinition { id = "cooler_x", model = "Liquid X", radiatorSupportMm = 275, tags = new List<string> { "aio" } };
            Assert.AreEqual(280, ProceduralCoolingVisuals.ResolveRadiatorMm(d));
            Assert.AreEqual(280, HardwarePresentationLayout.ResolveRadiatorMm(d));
            d.radiatorSupportMm = 340;
            Assert.AreEqual(360, ProceduralCoolingVisuals.ResolveRadiatorMm(d));
            Assert.AreEqual(360, HardwarePresentationLayout.ResolveRadiatorMm(d));
            d.radiatorSupportMm = 220;
            Assert.AreEqual(240, ProceduralCoolingVisuals.ResolveRadiatorMm(d));
            Assert.AreEqual(240, HardwarePresentationLayout.ResolveRadiatorMm(d));
        }

        [Test]
        public void FanIdentity_WinsOverDefaultedFanSize()
        {
            HardwareDefinition d = new HardwareDefinition
            {
                id = "fan_large",
                model = "Flow 140",
                fanSizeMm = 120,
                tags = new List<string> { "140mm" }
            };
            Assert.AreEqual(140, ProceduralCoolingVisuals.ResolveFanMm(d));
            Assert.AreEqual(140, HardwarePresentationLayout.ResolveFanMm(d));
            Assert.Greater(HardwarePresentationLayout.FanVisualDiameter(d), .22f);
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
            Assert.IsTrue(HardwarePresentationLayout.IsAio(d));
            Assert.IsTrue(ProceduralCoolingVisuals.HasRgb(d));
        }

        [Test]
        public void NonAio_DoesNotInventRadiatorFans()
        {
            HardwareDefinition d = new HardwareDefinition
            {
                id = "cooler_tower",
                model = "Tower 140",
                radiatorSupportMm = 360,
                tags = new List<string> { "air" }
            };
            Assert.IsFalse(HardwarePresentationLayout.IsAio(d));
            Assert.AreEqual(0, HardwarePresentationLayout.AioFanCount(d));
        }
    }
}