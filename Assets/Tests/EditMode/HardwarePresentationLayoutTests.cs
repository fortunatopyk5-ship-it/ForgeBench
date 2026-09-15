using NUnit.Framework;
using UnityEngine;

namespace ForgeBench.Tests
{
    public sealed class HardwarePresentationLayoutTests
    {
        [Test]
        public void RamPresentationUsesPersistedDualChannelSlots()
        {
            MachineState machine = new MachineState();
            machine.ramItemIds.Add("ram-a");
            machine.ramItemIds.Add("ram-b");
            machine.ramSlotIndices.Add(1);
            machine.ramSlotIndices.Add(3);

            Assert.AreEqual(-1, HardwarePresentationLayout.RamItemIndexAtSlot(machine, 0, 4));
            Assert.AreEqual(0, HardwarePresentationLayout.RamItemIndexAtSlot(machine, 1, 4));
            Assert.AreEqual(-1, HardwarePresentationLayout.RamItemIndexAtSlot(machine, 2, 4));
            Assert.AreEqual(1, HardwarePresentationLayout.RamItemIndexAtSlot(machine, 3, 4));
        }

        [Test]
        public void RamPresentationFallsBackOnlyWhenSlotMetadataIsAbsent()
        {
            MachineState machine = new MachineState();
            machine.ramItemIds.Add("ram-a");
            machine.ramItemIds.Add("ram-b");
            machine.ramSlotIndices.Clear();

            Assert.AreEqual(0, HardwarePresentationLayout.RamItemIndexAtSlot(machine, 0, 4));
            Assert.AreEqual(1, HardwarePresentationLayout.RamItemIndexAtSlot(machine, 1, 4));
            Assert.AreEqual(-1, HardwarePresentationLayout.RamItemIndexAtSlot(machine, 2, 4));
        }

        [Test]
        public void RecommendedRamSlotMatchesRuntimePopulationOrder()
        {
            HardwareDefinition board = new HardwareDefinition { dimmSlots = 4 };
            MachineState machine = new MachineState();
            Assert.AreEqual(1, HardwarePresentationLayout.NextRecommendedRamSlot(machine, board));
            machine.ramItemIds.Add("ram-a");
            machine.ramSlotIndices.Add(1);
            Assert.AreEqual(3, HardwarePresentationLayout.NextRecommendedRamSlot(machine, board));
        }

        [TestCase(190f, 75f, 1)]
        [TestCase(230f, 160f, 2)]
        [TestCase(310f, 300f, 3)]
        public void GpuCoolingSilhouetteTracksPhysicalClass(float lengthMm, float watts, int expectedFans)
        {
            HardwareDefinition gpu = new HardwareDefinition { lengthMm = lengthMm, powerWatts = watts };
            Assert.AreEqual(expectedFans, HardwarePresentationLayout.GpuFanCount(gpu));
        }

        [TestCase(240, 2)]
        [TestCase(280, 2)]
        [TestCase(360, 3)]
        public void AioVisualFanCountTracksRadiatorSize(int radiatorMm, int expectedFans)
        {
            HardwareDefinition cooler = new HardwareDefinition { radiatorSupportMm = radiatorMm };
            cooler.tags.Add("aio");
            Assert.AreEqual(expectedFans, HardwarePresentationLayout.AioFanCount(cooler));
        }

        [Test]
        public void StoragePresentationSeparatesNvmeFromSata()
        {
            HardwareDefinition nvme = new HardwareDefinition { storageInterface = "NVMe" };
            HardwareDefinition sata = new HardwareDefinition { storageInterface = "SATA" };
            Assert.IsTrue(HardwarePresentationLayout.IsNvme(nvme));
            Assert.IsFalse(HardwarePresentationLayout.IsNvme(sata));
        }

        [Test]
        public void FanDiameterReflects120And140MillimetreDefinitions()
        {
            float d120 = HardwarePresentationLayout.FanVisualDiameter(new HardwareDefinition { fanSizeMm = 120 });
            float d140 = HardwarePresentationLayout.FanVisualDiameter(new HardwareDefinition { fanSizeMm = 140 });
            Assert.Greater(d140, d120);
            Assert.That(d120, Is.EqualTo(.22f).Within(.001f));
        }

        [Test]
        public void PresentationSlotCountsRespectNormalizedBoardSpecs()
        {
            HardwareDefinition board = new HardwareDefinition { dimmSlots = 2, pcieX16Slots = 3, m2Slots = 4 };
            Assert.AreEqual(2, HardwarePresentationLayout.DimmSlotCount(board));
            Assert.AreEqual(3, HardwarePresentationLayout.PcieSlotCount(board));
            Assert.AreEqual(4, HardwarePresentationLayout.M2SlotCount(board));
        }
    }
}
