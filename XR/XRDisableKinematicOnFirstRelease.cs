using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRDisableKinematicOnGrab : MonoBehaviour
{
    void Start()
    {
        GetComponent<XRGrabInteractable>().onSelectExit.AddListener(SetKinematicOffOnNextDetach);
    }


    void SetKinematicOffOnNextDetach(XRBaseInteractor baseInteractor)
    {
        // Remove kinematic and unsubscribe to the event.
        StartCoroutine(SetKinematicOffAfterFrame());
        GetComponent<XRGrabInteractable>().onSelectExit.RemoveListener(SetKinematicOffOnNextDetach);
    }

    IEnumerator SetKinematicOffAfterFrame()
    {
        yield return null;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
