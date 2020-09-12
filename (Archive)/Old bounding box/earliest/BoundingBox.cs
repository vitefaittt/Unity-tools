using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    SketchGroup group;
    public SketchGroup Group
    {
        get { return group; }
        set
        {
            group = value;
            SetupConstraint();
        }
    }
    LightFullParentConstraint constraint;

    void SetupConstraint()
    {
        // Set a parent constraint to the group.
        if (!constraint)
            constraint = gameObject.AddComponent<LightFullParentConstraint>();
        constraint.Parent = group.transform;
    }
}
