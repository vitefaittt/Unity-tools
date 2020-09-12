using UnityEngine;

public class LightFullParentConstraint : MonoBehaviour
{
    [SerializeField]
    Transform parent;
    public Transform Parent
    {
        get { return parent; }
        set
        {
            parent = value;
            SetOffsets();
        }
    }
    Vector3 positionOffset;
    Quaternion rotationOffset;
    Vector3 scaleOffset;


    private void Start()
    {
        if (parent)
            SetOffsets();
    }

    private void Update()
    {
        transform.position = parent.TransformPoint(positionOffset);
        transform.rotation = parent.rotation * rotationOffset;
        transform.localScale = parent.localScale + scaleOffset;
    }


    void SetOffsets()
    {
        positionOffset = parent.InverseTransformPoint(transform.position);
        rotationOffset = Quaternion.Inverse(parent.rotation) * transform.rotation;
        scaleOffset = transform.localScale - parent.localScale;
    }
}
