using UnityEngine;
using UnityEngine.Events;

public class InputFieldFloat : InputFieldParsedEvent
{
    [SerializeField]
    FloatEvent OnValueChanged, OnEndEdit;

    protected override bool InvokeParsedValueChange(string input)
    {
        OnValueChanged?.Invoke(float.Parse(input));
        return OnValueChanged.GetPersistentEventCount() > 0;
    }

    protected override bool InvokeParsedEndEdit(string input)
    {
        OnEndEdit?.Invoke(float.Parse(input));
        return OnEndEdit.GetPersistentEventCount() > 0;
    }
}

[System.Serializable]
class FloatEvent : UnityEvent<float> { }