using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public static class ConstraintUtilities
{
    public static void SetSource(this IConstraint constraint, Transform target)
    {
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = target;
        source.weight = 1;
        constraint.SetSources(new List<ConstraintSource>() { source });
    }

    public static void SetSource(this ParentConstraint constraint, Transform target, bool snapToTarget)
    {
        constraint.SetSource(target);
        if (!snapToTarget)
        {
            // Setup target offset.
            Vector3 posOffset = target.InverseTransformPoint(constraint.transform.position);
            Vector3 rotOffset = (Quaternion.Inverse(target.rotation) * constraint.transform.rotation).eulerAngles;
            constraint.SetTranslationOffset(0, posOffset);
            constraint.SetRotationOffset(0, rotOffset);
        }
    }
}
