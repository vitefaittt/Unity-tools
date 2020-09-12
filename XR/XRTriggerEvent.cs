using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class XRTriggerEvent : MonoBehaviour
{
    public UnityEvent Triggered;
    bool leftWasDown, rightWasDown;


    void Update()
    {
        if (TryGetTriggerDown())
            Triggered.Invoke();
    }


    bool TryGetTriggerDown()
    {
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (leftHand.isValid && leftHand.TryGetFeatureValue(CommonUsages.trigger, out float leftTrigger) && leftTrigger > .85f)
        {
            if (!leftWasDown)
            {
                leftWasDown = true;
                return true;
            }
        }
        else
            leftWasDown = false;
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHand.isValid && rightHand.TryGetFeatureValue(CommonUsages.trigger, out float rightTrigger) && rightTrigger > .85f)
        {
            if (!rightWasDown)
            {
                rightWasDown = true;
                return true;
            }
        }
        else
            rightWasDown = false;
        return false;
    }
}
