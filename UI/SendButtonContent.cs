using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SendButtonContent : MonoBehaviour
{
    [SerializeField]
    Text buttonText;
    public enum ParseTarget { Int, Float, String }
    [SerializeField]
    public ParseTarget contentType;

    public UnityIntEvent IntEvent;
    public UnityFloatEvent FloatEvent;
    public UnityStringEvent StringEvent;


    void Reset()
    {
        buttonText = GetComponentInChildren<Text>();
        GetComponent<Button>().onClick.AddPersistentEvent(this, SendContent);
    }


    public void SendContent()
    {
        switch (contentType)
        {
            case ParseTarget.Int:
                IntEvent.Invoke(int.Parse(buttonText.text));
                break;
            case ParseTarget.Float:
                FloatEvent.Invoke(float.Parse(buttonText.text));
                break;
            case ParseTarget.String:
                StringEvent.Invoke(buttonText.text);
                break;
            default:
                break;
        }
    }
}

[System.Serializable]
public class UnityIntEvent : UnityEvent<int> { }
[System.Serializable]
public class UnityFloatEvent : UnityEvent<float> { }
[System.Serializable]
public class UnityStringEvent : UnityEvent<string> { }


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SendButtonContent)), UnityEditor.CanEditMultipleObjects]
class SendButtonContentEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        SendButtonContent script = (SendButtonContent)target;
        List<string> excludedEvents = new List<string>() { "IntEvent", "FloatEvent", "StringEvent" };
        switch (script.contentType)
        {
            case SendButtonContent.ParseTarget.Int:
                excludedEvents.Remove("IntEvent");
                break;
            case SendButtonContent.ParseTarget.Float:
                excludedEvents.Remove("FloatEvent");
                break;
            case SendButtonContent.ParseTarget.String:
                excludedEvents.Remove("StringEvent");
                break;
            default:
                break;
        }

        DrawPropertiesExcluding(serializedObject, excludedEvents.ToArray());
        if (excludedEvents.Contains("IntEvent"))
            script.IntEvent = new UnityIntEvent();
        if (excludedEvents.Contains("FloatEvent"))
            script.FloatEvent = new UnityFloatEvent();
        if (excludedEvents.Contains("StringEvent"))
            script.StringEvent = new UnityStringEvent();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
