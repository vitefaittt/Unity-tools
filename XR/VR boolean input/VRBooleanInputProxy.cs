using UnityEngine;
using UnityEngine.Events;

public abstract class VRBooleanInputProxy : MonoBehaviour
{
    public UnityEvent ClickDown, ClickUp;
    public bool IsPressed { get; private set; }


    void Update()
    {
        if (GetClickDown())
        {
            IsPressed = true;
            ClickDown.Invoke();
        }
        if (GetClickUp())
        {
            IsPressed = false;
            ClickUp.Invoke();
        }
    }


    protected abstract bool GetClickDown();
    protected abstract bool GetClickUp();
}
