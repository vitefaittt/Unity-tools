using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class VRRadialMenu : MonoBehaviour
{
    SteamVR_Action_Vector2 trackpadAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("default", "Trackpad");
    Vector2 ThumbPosition => trackpadAction.GetAxis(hand);
    bool IsTouched => ThumbDistance > isTouchedThreshold;
    float ThumbDistance => ThumbPosition.magnitude;
    float previousThumbDistance;
    readonly float isTouchedThreshold = .05f;
    readonly float selectedThreshold = .4f;
    ValueHolder<bool> isTouched = new ValueHolder<bool>();
    [SerializeField]
    SteamVR_Input_Sources hand;
    Dictionary<Transform, IEnumerator> buttonsHintRoutines;
    ValueHolder<int> hoverIndex = new ValueHolder<int>();
    int ItemsCount => transform.childCount;
    public event System.Action Opened, Closed;
    public event System.Action<int> HoverChanged, Selected;


    void Reset()
    {
        if (GetComponentInParent<Hand>())
            hand = GetComponentInParent<Hand>().handType;
    }

    void Start()
    {
        Close();
        // Setup hover and touch listening.
        hoverIndex.OnValueChanged += delegate { OnHoverChange(hoverIndex.Value, hoverIndex.PreviousValue); };
        isTouched.OnValueChanged += delegate
        {
            if (isTouched.Value)
                OnTouchStart();
            else
                OnTouchEnd();
        };
    }

    private void Update()
    {
        // Update hover value.
        hoverIndex.Value = ThumbDistance < selectedThreshold ? -1 : Utilities.IndexFromProgression(GetThumbProgression(), ItemsCount);

        // Update touch value.
        isTouched.Value = IsTouched;

        // Remember thumb distance.
        previousThumbDistance = ThumbDistance;
    }


    void OnTouchStart()
    {
        Open();
    }

    void OnTouchEnd()
    {
        // Close and select element if the thumb was far enough from the center.
        Close();
        if (previousThumbDistance > selectedThreshold)
            Selected?.Invoke(hoverIndex.PreviousValue);
    }

    void OnHoverChange(int currentHover, int previousHover)
    {
        OnHoverEnd(previousHover);
        if (currentHover >= 0)
            transform.GetChild(currentHover).localScale = Vector3.one * 1.3f;
        HoverChanged?.Invoke(currentHover);
    }

    void OnHoverEnd(int hover)
    {
        if (hover >= 0)
            transform.GetChild(hover).localScale = Vector3.one;
    }

    float GetThumbProgression()
    {
        float rawAngle = Vector3.Angle(Vector2.up, ThumbPosition);
        float angleFromStart;
        if (ThumbPosition.x > 0)
            angleFromStart = rawAngle;
        else
            angleFromStart = 360 - rawAngle;
        return angleFromStart / 360f;
    }

    void Open()
    {
        transform.ShowChildren();
    }

    void Close()
    {
        transform.HideChildren();
    }
}
