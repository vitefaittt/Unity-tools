using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIFadeAnim))]
public class MessageDisplay : MonoBehaviour
{
    public float messageDuration = 15;
    Canvas savedCanvas;
    public Canvas Canvas
    {
        get
        {
            if (!savedCanvas)
                savedCanvas = GetComponent<Canvas>();
            return savedCanvas;
        }
    }
    public Text text;
    UIFadeAnim fadeAnim;
    public bool dontDestroyOnLoad = true;


    private void Reset()
    {
        this.RenameFromType();
        text = GetComponentInChildren<Text>();
    }

    private void Awake()
    {
        fadeAnim = GetComponent<UIFadeAnim>();
    }

    private void Start()
    {
        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        Canvas.enabled = false;
        MessageClient.Instance.MessageReceived += DisplayMessage;
    }


    void DisplayMessage(string message)
    {
        text.text = message;
        ShowDisplay();
        this.Timer(messageDuration, HideDisplay);
    }

    public void ShowDisplay()
    {
        Canvas.enabled = true;
        if (Application.isPlaying)
            fadeAnim.FadeIn();
    }

    public void HideDisplay()
    {
        if (!Application.isPlaying)
        {
            Canvas.enabled = false;
            return;
        }
        fadeAnim.FadeOut(() => Canvas.enabled = false);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(MessageDisplay)), UnityEditor.CanEditMultipleObjects]
class MessageDisplayEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        MessageDisplay script = (MessageDisplay)target;

        if (GUILayout.Button("Show display"))
            script.ShowDisplay();
        if (GUILayout.Button("Hide display"))
            script.HideDisplay();
    }
}
#endif

