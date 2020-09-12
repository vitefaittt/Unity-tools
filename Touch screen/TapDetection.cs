using UnityEngine;
using UnityEngine.Events;

public class TapDetection : MonoBehaviour
{
    [SerializeField]
    [Tooltip("OnTap will fire on mouse button up.")]
    bool dirtyEmulationWithMouse = true;

    bool hasTouch = false;
    [SerializeField]
    UnityEvent OnTap;
    float lastTouch;
    Vector2 firstTouchPos;
    Vector2 lastTouchPos;


    private void Update()
    {
        if (Input.touchCount == 1)
        {
            if (!hasTouch)
            {
                hasTouch = true;
                lastTouch = Time.time;
                firstTouchPos = Input.GetTouch(0).position;
            }
            lastTouchPos = Input.GetTouch(0).position;
        }
        else if (hasTouch)
        {
            hasTouch = false;
            if (Time.time - lastTouch < .5f && Vector2.Distance(firstTouchPos, lastTouchPos) < 10)
                OnTap.Invoke();
        }

        if (dirtyEmulationWithMouse && Input.GetMouseButtonUp(0))
            OnTap.Invoke();
    }
}