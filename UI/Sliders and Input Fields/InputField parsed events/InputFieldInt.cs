using UnityEngine;
using UnityEngine.Events;

public class InputFieldInt : InputFieldParsedEvent
{
    [SerializeField]
    IntEvent OnValueChanged, OnEndEdit;

    protected override bool InvokeParsedValueChange(string input)
    {
        OnValueChanged?.Invoke(int.Parse(input));
        return OnValueChanged.GetPersistentEventCount() > 0;
    }

    protected override bool InvokeParsedEndEdit(string input)
    {
        OnEndEdit?.Invoke(int.Parse(input));
        return OnEndEdit.GetPersistentEventCount() > 0;
    }
}

[System.Serializable]
class IntEvent : UnityEvent<int> { }