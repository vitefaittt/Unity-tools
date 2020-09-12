using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonBehaviour : MonoBehaviour
{
    protected Button button;


    protected virtual void Reset()
    {
        GetComponent<Button>().onClick.AddPersistentEvent(this, OnClick);
    }

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
    }


    public abstract void OnClick();
}
