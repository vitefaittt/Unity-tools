using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRGrabbableDestroyUtility : MonoBehaviour
{
    public static void Destroy(GameObject gameObject, System.Action<GameObject> DestroyHandler = null)
    {
        XRGrabInteractable grabComponent = gameObject.GetComponent<XRGrabInteractable>();

        // If the object is not a grabbable or if it is not selected, destroy now.
        if (!grabComponent || !grabComponent.isSelected)
        {
            if (DestroyHandler == null)
                Object.Destroy(gameObject);
            else
                DestroyHandler(gameObject);
            return;
        }

        // Destroy when we get dropped, and force drop.
        grabComponent.onSelectExit.AddListener(delegate
        {
            if (DestroyHandler == null)
                Object.Destroy(gameObject);
            else
                DestroyHandler(gameObject);
        });
        grabComponent.selectingInteractor.DropObjectNow();
    }
}
