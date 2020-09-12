using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPhantomTrigger : MonoBehaviour
{
    [SerializeField]
    float minTimeBeforeNextTrigger = .5f;
    float lastTriggerTime;
    [SerializeField]
    UnityEvent Triggered;


    void OnTriggerEnter(Collider other)
    {
        // Trigger when a controller enters.
        if (other.GetComponentInParent<XRController>() && Time.time - lastTriggerTime > minTimeBeforeNextTrigger)
        {
            lastTriggerTime = Time.time;
            Triggered.Invoke();
        }
    }
}
