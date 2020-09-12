using UnityEngine;
using Valve.VR.InteractionSystem;

public class Wearable : MonoBehaviour
{
    Interactable interactable;
    Throwable throwable;
    new Rigidbody rigidbody;
    [SerializeField]
    WearableParenting.BodyPlacement placement;
    [SerializeField]
    Vector3 attachedPosition, attachedRotation;


    void Awake()
    {
        interactable = GetComponent<Interactable>();
        throwable = GetComponent<Throwable>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name != "HeadCollider")
            return;
        // Detach the object if it is grabbed.
        if (interactable.attachedToHand)
            interactable.attachedToHand.DetachObject(gameObject);

        // Attach the object to the head.
        rigidbody.isKinematic = true;
        transform.parent =   WearableParenting.Instance.GetPlacement(placement);
        transform.localPosition = attachedPosition;
        transform.localEulerAngles = attachedRotation;
    }
}
