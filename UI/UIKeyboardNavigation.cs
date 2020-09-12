using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIKeyboardNavigation : MonoBehaviour
{
    EventSystem System => EventSystem.current;


    void Update()
    {
        // Navigate between selectable using tab and shift.
        if (!Input.GetKeyDown(KeyCode.Tab))
            return;

        Selectable next;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            next = System.currentSelectedGameObject?.GetComponent<Selectable>().FindSelectableOnUp();
        else
            next = System.currentSelectedGameObject?.GetComponent<Selectable>().FindSelectableOnDown();
        if (!next)
            return;

        if (next.GetComponent<InputField>())
            next.GetComponent<InputField>().OnPointerClick(new PointerEventData(System));

        System.SetSelectedGameObject(next.gameObject, new BaseEventData(System));
    }
}
