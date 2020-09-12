using UnityEngine;
using UnityEngine.XR;

public class XRInput : MonoBehaviour
{
    #region Touchpad.
    public static Vector2 Touchpad(XRNode hand)
    {
        return GetFeatureValue(hand, CommonUsages.primary2DAxis);
    }

    public static bool TouchpadPress(XRNode hand)
    {
        return GetFeatureValue(hand, CommonUsages.primary2DAxisClick);
    }

    public static bool TouchpadTouch(XRNode hand)
    {
        return GetFeatureValue(hand, CommonUsages.primary2DAxisTouch);
    }
    #endregion

    #region Trigger.
    public static float Trigger(XRNode hand)
    {
        return GetFeatureValue(hand, CommonUsages.trigger);
    }

    public static bool TriggerButton(XRNode hand)
    {
        return GetFeatureValue(hand, CommonUsages.triggerButton);
    }
    #endregion

    #region Grip.
    public static float Grip(XRNode hand)
    {
        return GetFeatureValue(hand, CommonUsages.grip);
    }

    public static bool GripButton(XRNode hand)
    {
        return GetFeatureValue(hand, CommonUsages.gripButton);
    }
    #endregion

    #region Get device feature value.
    static bool GetFeatureValue(XRNode node, InputFeatureUsage<bool> usageType)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        bool value = false;
        if (device.isValid)
            device.TryGetFeatureValue(usageType, out value);
        return value;
    }

    static float GetFeatureValue(XRNode node, InputFeatureUsage<float> usageType)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        float value = 0;
        if (device.isValid)
            device.TryGetFeatureValue(usageType, out value);
        return value;
    }

    static Vector2 GetFeatureValue(XRNode node, InputFeatureUsage<Vector2> usageType)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        Vector2 value = new Vector2();
        if (device.isValid)
            device.TryGetFeatureValue(usageType, out value);
        return value;
    }
    #endregion
}
