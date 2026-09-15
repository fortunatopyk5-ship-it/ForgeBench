using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class ProceduralSurfaceLibraryTests
    {
        [TestCase("ProductionMachine3D/Motherboard", ProceduralSurfaceProfile.PcbGreen)]
        [TestCase("ProductionMachine3D/RAMContacts_1", ProceduralSurfaceProfile.Copper)]
        [TestCase("ProductionMachine3D/GPUBackplate", ProceduralSurfaceProfile.BrushedAluminum)]
        [TestCase("Main Assembly Bench/Top", ProceduralSurfaceProfile.EsdTeal)]
        [TestCase("Workshop/Floor", ProceduralSurfaceProfile.Concrete)]
        [TestCase("Workshop/BackWall", ProceduralSurfaceProfile.WarmWhitePaint)]
        [TestCase("ProductionMachine3D/ATX24Cable", ProceduralSurfaceProfile.Rubber)]
        public void SemanticClassifier_SelectsExpectedSurface(string name, ProceduralSurfaceProfile expected)
        {
            Assert.AreEqual(expected, ProceduralSurfaceLibrary.Classify(name));
        }

        [Test]
        public void Noise_IsDeterministicAndBounded()
        {
            float a = ProceduralSurfaceLibrary.Noise01(17, 29, 73);
            float b = ProceduralSurfaceLibrary.Noise01(17, 29, 73);
            Assert.AreEqual(a, b);
            Assert.That(a, Is.InRange(0f, 1f));
        }

        [Test]
        public void Noise_ChangesAcrossCoordinates()
        {
            float a = ProceduralSurfaceLibrary.Noise01(1, 1, 41);
            float b = ProceduralSurfaceLibrary.Noise01(2, 1, 41);
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void DynamicVisuals_AreNotMisclassifiedAsGenericSurfaces()
        {
            Assert.AreEqual(ProceduralSurfaceProfile.None, ProceduralSurfaceLibrary.Classify("PowerButton"));
            Assert.AreEqual(ProceduralSurfaceProfile.None, ProceduralSurfaceLibrary.Classify("RAMGhost_Slot1"));
        }
    }
}
