using UnityEngine;

public class WebVRBooleanInput : VRBooleanInputProxy
{
    [SerializeField]
    WebVRController controller;
    [SerializeField]
    string action = "Trigger";


    void Reset()
    {
        controller = GetComponent<WebVRController>();
        if (!controller)
            controller = GetComponentInParent<WebVRController>();
    }


    protected override bool GetClickDown()
    {
        return controller.GetButtonDown(action);
    }

    protected override bool GetClickUp()
    {
        return controller.GetButtonUp(action);
    }
}
