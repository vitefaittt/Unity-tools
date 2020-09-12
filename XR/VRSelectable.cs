using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VRSelectable : MonoBehaviour
{
    Selectable selectable;
    IPointerClickHandler clickHandler;


    void Reset()
    {
        ((RectTransform)transform).GetOrAddFittingBoxCollider();
    }

    void Awake()
    {
        selectable = GetComponent<Selectable>();
        clickHandler = GetComponent<Button>();
    }


    public void OnHoverStart()
    {
        selectable.OnPointerEnter();
    }

    public void OnHoverEnd()
    {
        selectable.OnPointerExit();
    }

    public void OnClickDown()
    {

    }

    public void OnClickUp()
    {
        clickHandler.OnPointerClick();
    }
}
