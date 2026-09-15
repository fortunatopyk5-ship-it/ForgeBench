using System.Collections.Generic;
using NUnit.Framework;

namespace ForgeBench.Tests
{
    public sealed class ToolCalibrationTests
    {
        [TestCase("tool_driver",true)]
        [TestCase("tool_meter",true)]
        [TestCase("tool_psu",true)]
        [TestCase("tool_solder",true)]
        [TestCase("tool_air",false)]
        public void CalibrationRequirement_IsToolSpecific(string id,bool expected)
        {
            Assert.AreEqual(expected,ToolCalibrationRules.RequiresCalibration(id));
        }

        [Test]
        public void UnsafeTool_IsDetectedFromConditionAndContamination()
        {
            ToolCalibrationRecord low=new ToolCalibrationRecord{definitionId="tool_driver",condition=.20f,calibration=1f};
            ToolCalibrationRecord dirty=new ToolCalibrationRecord{definitionId="tool_meter",condition=1f,calibration=1f,contamination=.90f};
            Assert.AreEqual(ToolHealthState.Unsafe,ToolCalibrationRules.Health(low,1));
            Assert.AreEqual(ToolHealthState.Unsafe,ToolCalibrationRules.Health(dirty,1));
        }

        [Test]
        public void OverdueCalibration_ReducesAccuracy()
        {
            ToolCalibrationRecord r=new ToolCalibrationRecord{definitionId="tool_meter",condition=1f,calibration=1f,drift=.02f,calibrationDueDay=10};
            float fresh=ToolCalibrationRules.Accuracy(r,8);
            float overdue=ToolCalibrationRules.Accuracy(r,25);
            Assert.Greater(fresh,overdue);
            Assert.AreEqual(ToolHealthState.CalibrationDue,ToolCalibrationRules.Health(r,25));
        }

        [Test]
        public void BoardRepair_WeightsSolderingMoreThanAirCleaning()
        {
            Assert.Greater(ToolCalibrationRules.Workload(JobType.BoardRepair,"tool_solder"),ToolCalibrationRules.Workload(JobType.BoardRepair,"tool_air"));
            Assert.Greater(ToolCalibrationRules.Workload(JobType.Cleaning,"tool_air"),ToolCalibrationRules.Workload(JobType.Cleaning,"tool_driver"));
        }

        [Test]
        public void BenchReadiness_PenalizesUnsafeRegister()
        {
            List<ToolCalibrationRecord> good=new List<ToolCalibrationRecord>
            {
                new ToolCalibrationRecord{definitionId="tool_driver",condition=.95f,calibration=.96f,calibrationDueDay=20},
                new ToolCalibrationRecord{definitionId="tool_meter",condition=.94f,calibration=.97f,calibrationDueDay=20}
            };
            List<ToolCalibrationRecord> mixed=new List<ToolCalibrationRecord>
            {
                good[0],
                new ToolCalibrationRecord{definitionId="tool_meter",condition=.18f,calibration=.30f,calibrationDueDay=1,lockedOut=true}
            };
            Assert.Greater(ToolCalibrationRules.BenchReadiness(good,5),ToolCalibrationRules.BenchReadiness(mixed,5));
        }

        [Test]
        public void CalibrationIntervals_ArePositiveAndReasonable()
        {
            foreach(string id in new[]{"tool_driver","tool_meter","tool_psu","tool_solder","tool_air"})
            {
                float days=ToolCalibrationRules.CalibrationIntervalDays(id);
                Assert.GreaterOrEqual(days,7f);
                Assert.LessOrEqual(days,45f);
            }
        }
    }
}