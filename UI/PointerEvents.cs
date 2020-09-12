using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEvents : MonoBehaviour, IPointerClickHandler
{
    public event System.Action PointerClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Call event.
        if (PointerClicked != null)
            PointerClicked();
    }
}
