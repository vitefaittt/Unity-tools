using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPhantomGrabRotate : MonoBehaviour
{
    [SerializeField]
    XRGrabInteractable phantomGrab;
    Quaternion startRotationOffset;


    void Reset()
    {
        phantomGrab = GetComponentInChildren<XRPhantomGrab>().GetComponent<XRGrabInteractable>();
    }

    void Start()
    {
        startRotationOffset = transform.rotation * Quaternion.Inverse(Quaternion.LookRotation(Vector3.up, (transform.position - phantomGrab.transform.position).normalized));
    }

    void Update()
    {
        if (phantomGrab.isSelected)
            transform.rotation = startRotationOffset * Quaternion.LookRotation(Vector3.up, (transform.position - phantomGrab.transform.position).normalized);
    }
}
