using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public abstract class InputFieldParsedEvent : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Send a message to the console when our events are fired (only when our events aren't empty).")]
    protected bool logEvents;


#if UNITY_EDITOR
    protected void Reset()
    {
        InputField field = GetComponent<InputField>();
        field.contentType = InputField.ContentType.DecimalNumber;
        UnityEditor.Events.UnityEventTools.AddPersistentListener(field.onValueChanged, OnValueChangedCaller);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(field.onEndEdit, OnEndEditCaller);
    }
#endif


    public void OnValueChangedCaller(string input)
    {
        if (logEvents && InvokeParsedValueChange(input))
            print("OnValueChanged fired");
    }

    public void OnEndEditCaller(string input)
    {
        if (logEvents && InvokeParsedEndEdit(input))
            print("OnEndEdit fired");
    }

    protected abstract bool InvokeParsedValueChange(string input);
    protected abstract bool InvokeParsedEndEdit(string input);
}