using NUnit.Framework;
using UnityEngine;

namespace ForgeBench.Tests
{
    public sealed class ProceduralSurfaceLibraryTests
    {
        [Test]
        public void SurfaceSample_IsDeterministic()
        {
            Color32 a = ProceduralSurfaceLibrary.Sample(ProceduralSurfaceKind.BrushedMetal, 17, 23);
            Color32 b = ProceduralSurfaceLibrary.Sample(ProceduralSurfaceKind.BrushedMetal, 17, 23);
            Assert.AreEqual(a.r, b.r);
            Assert.AreEqual(a.g, b.g);
            Assert.AreEqual(a.b, b.b);
            Assert.AreEqual(a.a, b.a);
        }

        [Test]
        public void SurfaceSample_HasUsefulSubtleVariation()
        {
            Color32 a = ProceduralSurfaceLibrary.Sample(ProceduralSurfaceKind.Graphite, 1, 1);
            Color32 b = ProceduralSurfaceLibrary.Sample(ProceduralSurfaceKind.Graphite, 29, 41);
            Assert.AreNotEqual(a.r, b.r);
            Assert.That(a.r, Is.InRange(160, 255));
            Assert.That(b.r, Is.InRange(160, 255));
            Assert.AreEqual(255, a.a);
            Assert.AreEqual(255, b.a);
        }

        [Test]
        public void DifferentSurfaceFamilies_DoNotCollapseToSamePattern()
        {
            Color32 metal = ProceduralSurfaceLibrary.Sample(ProceduralSurfaceKind.BrushedMetal, 12, 37);
            Color32 pcb = ProceduralSurfaceLibrary.Sample(ProceduralSurfaceKind.Pcb, 12, 37);
            Color32 rubber = ProceduralSurfaceLibrary.Sample(ProceduralSurfaceKind.Rubber, 12, 37);
            Assert.IsTrue(metal.r != pcb.r || pcb.r != rubber.r || metal.r != rubber.r);
        }

        [TestCase("CPU", ProceduralSurfaceKind.BrushedMetal)]
        [TestCase("Motherboard", ProceduralSurfaceKind.Pcb)]
        [TestCase("Trace_4", ProceduralSurfaceKind.Copper)]
        [TestCase("SATADrive_0", ProceduralSurfaceKind.Graphite)]
        [TestCase("DIMMSlot_2", ProceduralSurfaceKind.Polymer)]
        [TestCase("AIOTubeA", ProceduralSurfaceKind.Rubber)]
        public void SurfaceMapping_UsesExpectedFamily(string objectName, ProceduralSurfaceKind expected)
        {
            Assert.AreEqual(expected, HardwareSurfaceUpgradeDirector.ResolveKindForName(objectName));
        }

        [Test]
        public void SurfaceMapping_UnknownDecorationStaysUntouched()
        {
            Assert.IsNull(HardwareSurfaceUpgradeDirector.ResolveKindForName("CustomerPoster"));
        }

        [Test]
        public void GeneratedMeshOwner_OwnsOnlyUniqueRebuildMeshes()
        {
            Mesh fins = new Mesh { name = "ForgeBench_RadiatorFins" };
            Mesh tower = new Mesh { name = "ForgeBench_TowerFins" };
            Mesh tube = new Mesh { name = "ForgeBench_AioTube" };
            Mesh sharedBlade = new Mesh { name = "ForgeBench_FanBlade" };
            Mesh sharedShroud = new Mesh { name = "ForgeBench_FanShroud" };
            try
            {
                Assert.IsTrue(ProceduralGeneratedMeshOwner.Owns(fins));
                Assert.IsTrue(ProceduralGeneratedMeshOwner.Owns(tower));
                Assert.IsTrue(ProceduralGeneratedMeshOwner.Owns(tube));
                Assert.IsFalse(ProceduralGeneratedMeshOwner.Owns(sharedBlade));
                Assert.IsFalse(ProceduralGeneratedMeshOwner.Owns(sharedShroud));
            }
            finally
            {
                Object.DestroyImmediate(fins);
                Object.DestroyImmediate(tower);
                Object.DestroyImmediate(tube);
                Object.DestroyImmediate(sharedBlade);
                Object.DestroyImmediate(sharedShroud);
            }
        }
    }
}