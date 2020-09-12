using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EquipablesSameRigCondition : MonoBehaviour
{
    [SerializeField]
    XREquippable[] otherEquippables;


    void Start()
    {
        GetComponent<XREquippable>().EquipCondition = IsOnSameRigAsOtherEquippables;
    }


    bool IsOnSameRigAsOtherEquippables(XREquippableReceiver targetReceiver)
    {
        // Try to find an XRRig in other equippables.
        XRRig othersRig = null;
        foreach (var otherEquippable in otherEquippables)
        {
            try
            {
                othersRig = otherEquippable.CurrentReceiver.GetComponentInParent<XRRig>();
                break;
            }
            catch { }
        }

        // If we found a rig, check if it is the same rig as the one we are targetting.
        if (othersRig)
            return othersRig == targetReceiver.GetComponentInParent<XRRig>();
        else
            return true;
    }
}
