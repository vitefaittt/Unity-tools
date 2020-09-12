using UnityEngine;
using Valve.VR;

public class SteamVRBooleanInput : VRBooleanInputProxy
{
    [SerializeField]
    SteamVR_Input_Sources hand = SteamVR_Input_Sources.RightHand;
    [SerializeField]
    string actionSetName = "default";
    [SerializeField]
    string actionName = "GrabPinch";
    SteamVR_Action_Boolean clickAction;


    protected override bool GetClickDown()
    {
        return clickAction.GetStateDown(hand);
    }

    protected override bool GetClickUp()
    {
        return clickAction.GetStateUp(hand);
    }
}
