using UnityEngine;

public class AutoAimHintCursor : MonoBehaviour
{
    [SerializeField]
    AutoAim autoAim;
    [SerializeField]
    Camera playerCamera;
    OnScreenPositionFinder positionFinder;


    void Reset()
    {
        autoAim = FindObjectOfType<AutoAim>();
        playerCamera = Camera.main;
    }

    void Start()
    {
        // Create a position finder to follow the target's position on screen.
        positionFinder = new OnScreenPositionFinder(GetComponentInParent<Canvas>().GetComponent<RectTransform>(), playerCamera);
    }

    void Update()
    {
        // Follow the target or don't.
        if (playerCamera.SeesTarget(autoAim.Target))
            transform.localPosition = positionFinder.GetPosition(autoAim.Target);
        else
            transform.localPosition = Vector3.zero;
    }
}
