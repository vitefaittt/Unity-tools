using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XREquippable : MonoBehaviour
{
    XRGrabInteractable grabInteractable;
    Rigidbody rigidbody;
    [SerializeField]
    Vector3 equipLocalPos;
    [SerializeField]
    Vector3 equipLocalRot;
    [SerializeField]
    string receiverNameToCheck;
    public System.Func<XREquippableReceiver, bool> EquipCondition;
    XREquippableReceiver currentTargetReceiver;
    public XREquippableReceiver CurrentReceiver { get; private set; }
    public bool IsEquipped => CurrentReceiver != null;

    public delegate bool ReceiverEquipRequest(XREquippableReceiver receiver);
    public ReceiverEquipRequest ReceivedEquipRequest;
    public System.Action<XREquippableReceiver> Equipped;


    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        grabInteractable.onSelectExit.AddListener(delegate { StartCoroutine(TryEquipOnTargetReceiver()); });
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsEquipped)
            return;
        TryEquipStartOnOther(collision.collider);
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsEquipped)
            return;
        TryEquipStartOnOther(other);
    }


    void TryEquipStartOnOther(Collider other)
    {
        XREquippableReceiver targetEquipable = other.GetComponent<XREquippableReceiver>();

        // Stop if the other is not an acceptable receiver.
        if (!targetEquipable || (!string.IsNullOrWhiteSpace(receiverNameToCheck) && receiverNameToCheck != targetEquipable.ReceiverName))
            return;

        // Stop if we are not grabbed.
        if (!grabInteractable.selectingInteractor)
            return;

        // Stop if our equip condition isn't met.
        if (EquipCondition == null || EquipCondition(targetEquipable))
            return;

        // Ask the receiver to drop the object. We should equip when we see that we are released.
        currentTargetReceiver = targetEquipable;
        grabInteractable.selectingInteractor.DropObjectNow();
    }

    IEnumerator TryEquipOnTargetReceiver()
    {
        if (!currentTargetReceiver)
            yield break;
        yield return null;

        // Check to see if we can equip this receiver.
        if (ReceivedEquipRequest?.Invoke(currentTargetReceiver) == false)
            yield break;

        // Equip to the receiver.
        rigidbody.isKinematic = true;
        transform.parent = currentTargetReceiver.transform;
        transform.localPosition = equipLocalPos;
        transform.localEulerAngles = equipLocalRot;

        // Set the current receiver and forget the receiver target.
        CurrentReceiver = currentTargetReceiver;
        currentTargetReceiver = null;

        // Disable grab.
        Destroy(grabInteractable);

        // Call event.
        Equipped?.Invoke(CurrentReceiver);
    }
}
