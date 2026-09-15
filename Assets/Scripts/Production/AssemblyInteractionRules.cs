using UnityEngine;

namespace ForgeBench
{
    public enum SnapPreviewState
    {
        None,
        TooFar,
        Incompatible,
        WrongOrientation,
        Valid
    }

    public readonly struct SnapPreviewResult
    {
        public readonly SnapPreviewState state;
        public readonly float distance;
        public readonly float orientationError;
        public readonly float score;

        public SnapPreviewResult(SnapPreviewState state, float distance, float orientationError, float score)
        {
            this.state = state;
            this.distance = distance;
            this.orientationError = orientationError;
            this.score = score;
        }

        public bool CanSnap => state == SnapPreviewState.Valid;
    }

    /// <summary>
    /// Pure rules for physical socket preview and release decisions. Keeping these rules
    /// independent from scene objects makes interaction behavior deterministic and testable.
    /// </summary>
    public static class AssemblyInteractionRules
    {
        public static SnapPreviewResult Evaluate(
            float distance,
            float snapRadius,
            float orientationError,
            float orientationTolerance,
            bool compatible,
            bool requireOrientation)
        {
            snapRadius = Mathf.Max(.01f, snapRadius);
            orientationTolerance = Mathf.Clamp(orientationTolerance, 1f, 180f);
            distance = Mathf.Max(0f, distance);
            orientationError = Mathf.Clamp(orientationError, 0f, 180f);

            float score = distance / snapRadius;
            if (requireOrientation) score += .35f * orientationError / orientationTolerance;

            if (distance > snapRadius)
                return new SnapPreviewResult(SnapPreviewState.TooFar, distance, orientationError, score);
            if (!compatible)
                return new SnapPreviewResult(SnapPreviewState.Incompatible, distance, orientationError, score);
            if (requireOrientation && orientationError > orientationTolerance)
                return new SnapPreviewResult(SnapPreviewState.WrongOrientation, distance, orientationError, score);
            return new SnapPreviewResult(SnapPreviewState.Valid, distance, orientationError, score);
        }

        public static float OrientationError(Quaternion held, Quaternion target)
        {
            return Quaternion.Angle(held, target);
        }

        public static bool IsWorthPreviewing(float distance, float snapRadius)
        {
            return distance <= Mathf.Max(.01f, snapRadius) * 1.75f;
        }

        public static string StatusText(SnapPreviewState state)
        {
            switch (state)
            {
                case SnapPreviewState.Valid: return "READY TO INSTALL";
                case SnapPreviewState.WrongOrientation: return "ROTATE TO ALIGN";
                case SnapPreviewState.Incompatible: return "INCOMPATIBLE";
                case SnapPreviewState.TooFar: return "MOVE CLOSER";
                default: return "NO INSTALLATION POINT";
            }
        }
    }
}
