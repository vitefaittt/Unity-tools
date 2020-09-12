using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TiroirGrab : MonoBehaviour
{
    [SerializeField]
    XRGrabInteractable grabbable;
    Vector3 GrabbableLocalPosition => transform.parent.InverseTransformPoint(grabbable.transform.position);
    [SerializeField]
    bool reverse;


    void Reset()
    {
        grabbable = transform.parent.GetComponentInChildren<XRGrabInteractable>();
    }

    private void Update()
    {
        if (grabbable.isSelected)
        {
            if (GrabbableLocalPosition.z > 0 && GrabbableLocalPosition.z < .3f)
                transform.localPosition = new Vector3(0, 0, GrabbableLocalPosition.z);
        }
        else
        {
            grabbable.transform.localPosition = transform.localPosition;
            grabbable.transform.localEulerAngles = transform.localEulerAngles;
        }
    }
}
