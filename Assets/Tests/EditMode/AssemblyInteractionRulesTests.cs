using NUnit.Framework;
using UnityEngine;

namespace ForgeBench.Tests
{
    public sealed class AssemblyInteractionRulesTests
    {
        [Test]
        public void ValidSocketRequiresRangeCompatibilityAndOrientation()
        {
            SnapPreviewResult result = AssemblyInteractionRules.Evaluate(.18f, .55f, 18f, 85f, true, true);
            Assert.AreEqual(SnapPreviewState.Valid, result.state);
            Assert.IsTrue(result.CanSnap);
        }

        [Test]
        public void TooFarTakesPriorityOutsideSnapRadius()
        {
            SnapPreviewResult result = AssemblyInteractionRules.Evaluate(.60f, .55f, 5f, 85f, false, true);
            Assert.AreEqual(SnapPreviewState.TooFar, result.state);
            Assert.IsFalse(result.CanSnap);
        }

        [Test]
        public void IncompatibleComponentCannotSnapWhenCloseAndAligned()
        {
            SnapPreviewResult result = AssemblyInteractionRules.Evaluate(.08f, .55f, 2f, 85f, false, true);
            Assert.AreEqual(SnapPreviewState.Incompatible, result.state);
        }

        [Test]
        public void MisalignedComponentRemainsBlocked()
        {
            SnapPreviewResult result = AssemblyInteractionRules.Evaluate(.12f, .55f, 96f, 85f, true, true);
            Assert.AreEqual(SnapPreviewState.WrongOrientation, result.state);
        }

        [Test]
        public void SocketMayExplicitlyIgnoreOrientation()
        {
            SnapPreviewResult result = AssemblyInteractionRules.Evaluate(.12f, .55f, 170f, 20f, true, false);
            Assert.AreEqual(SnapPreviewState.Valid, result.state);
        }

        [Test]
        public void PreviewRangeIsLargerThanCommitRangeButBounded()
        {
            Assert.IsTrue(AssemblyInteractionRules.IsWorthPreviewing(.90f, .55f));
            Assert.IsFalse(AssemblyInteractionRules.IsWorthPreviewing(1.10f, .55f));
        }

        [Test]
        public void QuaternionOrientationErrorIsDeterministic()
        {
            float error = AssemblyInteractionRules.OrientationError(Quaternion.identity, Quaternion.Euler(0f, 90f, 0f));
            Assert.That(error, Is.EqualTo(90f).Within(.01f));
        }

        [TestCase(SnapPreviewState.Valid, "READY TO INSTALL")]
        [TestCase(SnapPreviewState.WrongOrientation, "ROTATE TO ALIGN")]
        [TestCase(SnapPreviewState.Incompatible, "INCOMPATIBLE")]
        [TestCase(SnapPreviewState.TooFar, "MOVE CLOSER")]
        public void StatusTextIsStable(SnapPreviewState state, string expected)
        {
            Assert.AreEqual(expected, AssemblyInteractionRules.StatusText(state));
        }
    }
}
