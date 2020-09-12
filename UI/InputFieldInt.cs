#if UNITY_EDITOR
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldInt : MonoBehaviour
{
    [SerializeField]
    IntEvent OnValueChanged, OnEndEdit;


    private void Reset()
    {
        InputField field = GetComponent<InputField>();
        field.contentType = InputField.ContentType.IntegerNumber;
        UnityEventTools.AddPersistentListener(field.onValueChanged, OnValChangedCaller);
        UnityEventTools.AddPersistentListener(field.onEndEdit, OnEndEditCaller);
    }

    void OnValChangedCaller(string input)
    {
        OnValueChanged?.Invoke(int.Parse(input));
    }
    void OnEndEditCaller(string input)
    {
        OnEndEdit?.Invoke(int.Parse(input));
    }
}

[System.Serializable]
class IntEvent : UnityEvent<int>
{

}
#endif