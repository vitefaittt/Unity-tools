using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField]
    Text caption, text;
    [SerializeField]
    Transform buttonsParent;
    MessageBoxCallback[] callbacks;

    void SetCallbacks(params MessageBoxCallback[] callbacks)
    {
        // Show the correct buttons.
        foreach (Transform button in buttonsParent)
            button.gameObject.SetActive(false);
        for (int i = 0; i < callbacks.Length; i++)
            buttonsParent.Find(callbacks[i].CallbackType.ToString()).gameObject.SetActive(true);

        // Set callbacks.
        this.callbacks = callbacks;

        // Rename the buttons.
        Button[] buttons = buttonsParent.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
            button.GetComponentInChildren<Text>().text = Translator.Translate(button.name, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
    }

    public void ButtonAction(Button button)
    {
        MessageBoxCallbackType callbackType = (MessageBoxCallbackType)Enum.Parse(typeof(MessageBoxCallbackType), button.name);
        MessageBoxCallback callback = Array.Find(callbacks, (c) => c.CallbackType == callbackType);
        callback.Action?.Invoke();
        Close();
    }

    public void Close() => Destroy(gameObject);

    void SetTextAndCaption(string text, string caption)
    {
        this.text.text = text;
        this.caption.text = caption;
    }

    #region Show.
    public static void Show(string text, string caption = "")
    {
        MessageBox box = Instantiate(Resources.Load("MessageBox") as GameObject).GetComponent<MessageBox>();
        box.SetTextAndCaption(text, caption);
        box.SetCallbacks(new MessageBoxCallback(MessageBoxCallbackType.Ok));
    }

    public static void Show(string text, params MessageBoxCallback[] callbacks) => Show(text, "", callbacks);
    public static void Show(string text, string caption, params MessageBoxCallback[] callbacks)
    {
        MessageBox box = Instantiate(Resources.Load("MessageBox") as GameObject).GetComponent<MessageBox>();
        box.SetTextAndCaption(text, caption);
        box.SetCallbacks(callbacks);
    }
    #endregion
}

[Flags]
public enum MessageBoxCallbackType { Retry, Ok, Cancel, Yes, No }

public class MessageBoxCallback
{
    public MessageBoxCallbackType CallbackType { get; private set; }
    public Action Action;

    public MessageBoxCallback(MessageBoxCallbackType callbackType, Action Action = null)
    {
        CallbackType = callbackType;
        this.Action = Action;
    }
}