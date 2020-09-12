using UnityEngine;

public class AutoAimCursor : MonoBehaviour
{
    [SerializeField]
    AutoAim autoAim;
    [SerializeField]
    Camera playerCamera;
    OnScreenPositionFinder screenPosFinder;


    void Reset()
    {
        autoAim = FindObjectOfType<AutoAim>();
        playerCamera = Camera.main;
    }

    void Start()
    {
        // Create a new screenPosFinder from our parent canvas.
        screenPosFinder = new OnScreenPositionFinder(GetComponentInParent<Canvas>().GetComponent<RectTransform>(), playerCamera);
    }

    void Update()
    {
        // Move to the position of autoAim's targeted object.
        if (autoAim.FollowsTarget)
            transform.localPosition = screenPosFinder.GetPosition(autoAim.Target);
        else
            transform.localPosition = Vector3.zero;
    }
}
