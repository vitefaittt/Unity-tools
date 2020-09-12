using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickCaller : MonoBehaviour
{
    [SerializeField]
    EventSystem eventSystem;


    private void Reset()
    {
        eventSystem = FindObjectOfType<EventSystem>();
    }


    public void OnClick()
    {
        if (eventSystem)
            ExecuteEvents.Execute(gameObject, new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
    }
}
