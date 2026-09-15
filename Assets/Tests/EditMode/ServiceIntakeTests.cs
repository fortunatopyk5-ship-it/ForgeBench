using System.Collections.Generic;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class ServiceIntakeTests
    {
        [Test]
        public void DataConsent_IsRequiredForSensitiveServiceTypes()
        {
            Assert.IsTrue(ServiceIntakeRules.RequiresDataConsent(JobType.Software,DeviceCategory.Desktop,false));
            Assert.IsTrue(ServiceIntakeRules.RequiresDataConsent(JobType.Diagnostics,DeviceCategory.Desktop,false));
            Assert.IsTrue(ServiceIntakeRules.RequiresDataConsent(JobType.Repair,DeviceCategory.Laptop,false));
            Assert.IsFalse(ServiceIntakeRules.RequiresDataConsent(JobType.CustomBuild,DeviceCategory.Desktop,false));
        }

        [Test]
        public void ExteriorCondition_DropsWithWearAndDamage()
        {
            var clean = new List<ItemInstance>
            {
                new ItemInstance { condition=1f, wear=.02f, damage=DamageType.None },
                new ItemInstance { condition=.96f, wear=.08f, damage=DamageType.None }
            };
            var damaged = new List<ItemInstance>
            {
                new ItemInstance { condition=.58f, wear=.72f, damage=DamageType.ImpactDamage },
                new ItemInstance { condition=.68f, wear=.55f, damage=DamageType.Corrosion }
            };
            Assert.Greater(ServiceIntakeRules.ExteriorCondition(clean),ServiceIntakeRules.ExteriorCondition(damaged));
            Assert.AreEqual(2,ServiceIntakeRules.VisibleDamage(damaged));
            Assert.IsTrue(ServiceIntakeRules.HasMoistureRisk(damaged));
        }

        [Test]
        public void CheckIn_RequiresCompleteCustodyEvidence()
        {
            ServiceIntakeRecord r = new ServiceIntakeRecord { dataConsentRequired=true };
            string reason;
            Assert.IsFalse(ServiceIntakeRules.CanCheckIn(r,out reason));
            r.exteriorInspected=true;r.serialVerified=true;r.powerStateRecorded=true;r.accessoriesConfirmed=true;r.estimateApproved=true;
            Assert.IsFalse(ServiceIntakeRules.CanCheckIn(r,out reason));
            r.dataConsent=true;
            Assert.IsTrue(ServiceIntakeRules.CanCheckIn(r,out reason));
        }

        [TestCase(DeviceCategory.Desktop,"power-cable")]
        [TestCase(DeviceCategory.Laptop,"charger")]
        [TestCase(DeviceCategory.Console,"controller")]
        [TestCase(DeviceCategory.Phone,"sim-tray")]
        [TestCase(DeviceCategory.NAS,"power-brick")]
        public void DefaultAccessories_IncludeExpectedDeviceSpecificItem(DeviceCategory device,string expectedKey)
        {
            List<IntakeAccessoryState> accessories = ServiceIntakeRules.DefaultAccessories(device);
            Assert.IsNotEmpty(accessories);
            Assert.That(accessories.Exists(x=>x.key==expectedKey));
        }

        [Test]
        public void DefaultAccessories_DoNotDuplicateKeys()
        {
            foreach(DeviceCategory device in System.Enum.GetValues(typeof(DeviceCategory)))
            {
                List<IntakeAccessoryState> accessories = ServiceIntakeRules.DefaultAccessories(device);
                HashSet<string> keys = new HashSet<string>();
                foreach(IntakeAccessoryState accessory in accessories)
                    Assert.IsTrue(keys.Add(accessory.key),"Duplicate accessory key for "+device+": "+accessory.key);
            }
        }
    }
}