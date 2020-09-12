using UnityEngine;

public class XREquippableReceiver : MonoBehaviour
{
    [SerializeField]
    string receiverName;
    public string ReceiverName => receiverName;
}
