using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPhantomGrab : MonoBehaviour
{
    Vector3 awakeLocalPosition;
    Quaternion awakeLocalRotation;
    XRGrabInteractable grab;

    public UnityEvent OnGrab, OnRelease;


    void Reset()
    {
        this.GetOrAddComponent<XRGrabInteractable>();

        // Set rigidbody and layer.
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        gameObject.layer = LayerMask.NameToLayer("PhantomGrab");
    }

    void Awake()
    {
        awakeLocalPosition = transform.localPosition;
        awakeLocalRotation = transform.localRotation;
        grab = GetComponent<XRGrabInteractable>();
    }

    void Start()
    {
        GetComponent<XRGrabInteractable>().onActivate.AddListener((b) => OnGrab.Invoke());
        GetComponent<XRGrabInteractable>().onDeactivate.AddListener((b) => OnRelease.Invoke());
    }

    void Update()
    {
        // Maintain our start local transform when we are not grabbed.
        if (!grab.isSelected)
        {
            transform.localPosition = awakeLocalPosition;
            transform.localRotation = awakeLocalRotation;
        }
    }
}
