using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class XRDeviceButtonInput : MonoBehaviour
{
    static readonly Dictionary<Button, InputFeatureUsage<bool>> availableButtons = new Dictionary<Button, InputFeatureUsage<bool>>
        {
            {Button.triggerButton, CommonUsages.triggerButton },
            {Button.primary2DAxisClick, CommonUsages.primary2DAxisClick },
            {Button.primary2DAxisTouch, CommonUsages.primary2DAxisTouch },
            {Button.menuButton, CommonUsages.menuButton },
            {Button.gripButton, CommonUsages.gripButton },
            {Button.secondaryButton, CommonUsages.secondaryButton },
            {Button.secondaryTouch, CommonUsages.secondaryTouch },
            {Button.primaryButton, CommonUsages.primaryButton },
            {Button.primaryTouch, CommonUsages.primaryTouch },
        };

    public enum Button
    {
        triggerButton,
        primary2DAxisClick,
        primary2DAxisTouch,
        menuButton,
        gripButton,
        secondaryButton,
        secondaryTouch,
        primaryButton,
        primaryTouch
    };

    [SerializeField]
    InputDeviceCharacteristics device;

    [SerializeField]
    Button button;

    public bool IsPressed { get; private set; }
    public UnityEvent OnPress;
    public UnityEvent OnRelease;

    List<InputDevice> inputDevices = new List<InputDevice>();

    InputFeatureUsage<bool> inputFeature;

    void Awake()
    {
        availableButtons.TryGetValue(button, out inputFeature);
    }


    void Update()
    {
        InputDevices.GetDevicesWithCharacteristics(device, inputDevices);

        for (int i = 0; i < inputDevices.Count; i++)
        {
            if (inputDevices[i].TryGetFeatureValue(inputFeature, out bool inputValue) && inputValue)
            {
                if (!IsPressed)
                {
                    IsPressed = true;
                    OnPress.Invoke();
                }
            }
            else if (IsPressed)
            {
                IsPressed = false;
                OnRelease.Invoke();
            }
        }
    }
}
