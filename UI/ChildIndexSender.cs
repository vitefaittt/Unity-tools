using UnityEngine;
using UnityEngine.UI;

public class ChildIndexSender : MonoBehaviour
{
    public UnityIntEvent Event;


    private void Reset()
    {
        if (GetComponent<Button>())
            GetComponent<Button>().onClick.AddPersistentEvent(this, Send);
    }


    public void Send()
    {
        Event.Invoke(transform.GetSiblingIndex());
    }
}

//[System.Serializable]
//public class UnityIntEvent : UnityEvent<int> { }