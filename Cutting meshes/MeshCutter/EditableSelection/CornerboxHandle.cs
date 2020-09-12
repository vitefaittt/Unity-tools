using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CornerboxHandle : MonoBehaviour {

    public enum HandleType { Left, Right, Front, Back, Top, Bottom }
    [SerializeField]
    HandleType handleType;

    Interactable interactable;

    [SerializeField]
    Transform parentToStretch;
    Transform visualizer;
    Transform transformReminder;

    //Hand trackedHand;
    // DEBUG
    [SerializeField]
    Transform trackedHand;
    private void Start()
    {
        StartUpdatingYPos();
    }

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.onAttachedToHand += delegate { StartUpdatingYPos(); };
        interactable.onDetachedFromHand += delegate { StopUpdatingYPos(); };
        transformReminder = transform.GetChild(0);
        visualizer = transform.GetChild(0).GetChild(0);
    }

    void StartUpdatingYPos()
    {
        updatingYPos = UpdateYPos();
        StartCoroutine(updatingYPos);
    }

    void StopUpdatingYPos()
    {
        if (updatingYPos != null)
        {
            StopCoroutine(updatingYPos);
            updatingYPos = null;
        }
        transformReminder.transform.position = visualizer.position;
        visualizer.localPosition = Vector3.zero;
        transform.position = visualizer.position;
        transform.rotation = visualizer.rotation;
        transformReminder.parent = transform;
    }

    IEnumerator updatingYPos;
    IEnumerator UpdateYPos()
    {
        transformReminder.parent = null;
        yield return null;
        //trackedHand = interactable.attachedToHand;
        while (true)
        {
            transformReminder.position = transformReminder.position.SetY(trackedHand.transform.position.y);

            float controllerDistance = Vector3.Distance(trackedHand.transform.position, transformReminder.position);
            float controllerAngle = Vector3.Angle(trackedHand.transform.forward, transformReminder.position - trackedHand.transform.position);
            float topAngle = 90 - controllerAngle;

            float shift = controllerDistance * (Mathf.Sin(controllerAngle * Mathf.Deg2Rad) / Mathf.Sin(topAngle * Mathf.Deg2Rad));

            Debug.DrawRay(trackedHand.transform.position, trackedHand.transform.forward);
            Debug.DrawRay(transformReminder.transform.position, transformReminder.up);
            Debug.DrawRay(transformReminder.transform.position, trackedHand.transform.position - transformReminder.position);

            visualizer.transform.position = visualizer.transform.position.SetY(transformReminder.position.y + shift);

            parentToStretch.transform.localScale = parentToStretch.transform.localScale.SetY(1 + shift);
            yield return null;
        }
    }

}
