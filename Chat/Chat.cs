using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public Transform speakerTextParent, responseTextParent;
    [SerializeField]
    ChatMessage speakerTextTemplate, responseTextTemplate;
    [SerializeField]
    string speakerPrefix = "You: ";
    [SerializeField]
    string responsePrefix = "Client: ";
    [SerializeField]
    [Tooltip("Will be hidden when the chat is empty.")]
    public bool multipleViewports;
    public GameObject viewport, speakerViewport, responseViewport;

    public static Chat Instance { get; private set; }


    private void Reset()
    {
        this.RenameFromType();
        if (transform.Find("Viewport"))
            viewport = transform.Find("Viewport").gameObject;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Clear();
    }

    private void Update()
    {
        // Hide when empty.
        if (!multipleViewports)
        {
            if (viewport)
                viewport.SetActive(viewport.transform.childCount > 0);
        }
        else
        {
            speakerViewport.SetActive(speakerViewport.transform.childCount > 0);
            responseViewport.SetActive(responseViewport.transform.childCount > 0);
        }
    }


    public void SendResponseText(string input, System.Action<ChatMessage> ActionWithSentText = null)
    {
        this.Timer(.5f, delegate
        {
            ChatMessage newMessage = Instantiate(responseTextTemplate, responseTextParent, false);
            newMessage.Text = responsePrefix + input;
            ActionWithSentText?.Invoke(newMessage);

            // Rebuild the RectTransform parent.
            if (responseTextParent is RectTransform)
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)responseTextParent);
        });
    }

    public void SendSpeakerText(string input, System.Action<ChatMessage> ActionWithSentText = null)
    {
        ChatMessage newMessage = Instantiate(speakerTextTemplate, speakerTextParent, false);
        newMessage.Text = speakerPrefix + input;
        ActionWithSentText?.Invoke(newMessage);

        // Rebuild the RectTransform parent.
        if (speakerTextParent is RectTransform)
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)speakerTextParent);
    }

    public void Clear()
    {
        responseTextParent.DestroyChildren();
        speakerTextParent.DestroyChildren();
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(Chat)), UnityEditor.CanEditMultipleObjects]
public class ChatEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        Chat script = target as Chat;
        List<string> excludedProperties = new List<string>();

        if (script.multipleViewports)
            excludedProperties.Add("viewport");
        else
        {
            excludedProperties.Add("speakerViewport");
            excludedProperties.Add("responseViewport");
        }

        DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
        serializedObject.ApplyModifiedProperties();
    }
}
#endif