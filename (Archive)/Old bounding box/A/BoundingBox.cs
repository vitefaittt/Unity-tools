using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SerializeField]
[ExecuteInEditMode]
public class BoundingBox: MonoBehaviour
{
   /*SketchGroup group;
    public SketchGroup Group
    {
        get { return group; }
        set
        {
            group = value;
            SetupConstraint();
        }
    }
    LightFullParentConstraint constraint;*/
    [SerializeField]
    Transform[] corners;
    public float cornersScale = 30;


    private void Reset()
    {
        List<Transform> children = GetComponentsInChildren<Transform>().ToList();
        children.Remove(transform);
        corners = children.ToArray();
    }

    private void Update()
    {
        for (int i = 0; i < corners.Length; i++)
            corners[i].MaintainScale(cornersScale);
    }

    /*
    void SetupConstraint()
    {
        // Set a parent constraint to the group.
        if (!constraint)
            constraint = gameObject.AddComponent<LightFullParentConstraint>();
        constraint.Parent = group.transform;
    }*/
}
