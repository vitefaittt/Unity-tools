using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class MessageSender : MonoBehaviour
{
    InputField input;


    private void Awake()
    {
        input = GetComponent<InputField>();
    }


    public void Submit()
    {
        if (!string.IsNullOrWhiteSpace(input.text))
            MessageClient.Instance.Send(input.text);
        input.text = "";
    }
}
