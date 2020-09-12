using UnityEngine;
using UnityEngine.Events;

public class SelectableCollider : MonoBehaviour
{
    public UnityEvent MouseEnter, MouseExit, MouseDown, MouseUp;


    void OnMouseEnter()
    {
        MouseEnter.Invoke();
    }

    void OnMouseExit()
    {
        MouseExit.Invoke();
    }

    void OnMouseDown()
    {
        MouseDown.Invoke();
    }

    void OnMouseUp()
    {
        MouseUp.Invoke();
    }
}
