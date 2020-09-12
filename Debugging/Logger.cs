using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Logs in a text component what the console is saying.
/// </summary>
public class Logger : MonoBehaviour
{
    [SerializeField]
    Text text;


    private void Reset()
    {
        if (!text)
            if (!GetComponent<Text>())
                OverrideGameobjectAsLogger();
            else
                text = GetComponent<Text>();
    }

    private void Start()
    {
        if (!text)
            Reset();
        if (!text)
            return;

        text.text = "[Debug Logger is active]";

        Application.logMessageReceived += delegate (string condition, string stackTrace, LogType type)
        {
            if (text.text.Length > 10000)
                text.text = text.text.Substring(5000);
            text.text += '\n' + condition;
        };
    }


    void OverrideGameobjectAsLogger()
    {
        // Parent ourselves to a new canvas.
        Canvas canvas = new GameObject().AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        canvas.gameObject.AddComponent<CanvasScaler>();
        canvas.gameObject.name = "Debug canvas";
        transform.SetParent(canvas.transform, false);
        gameObject.AddComponent<RectTransform>();
        ((RectTransform)transform).anchorMax = new Vector2(1, 0);
        ((RectTransform)transform).anchorMin = new Vector2(0, 0);
        ((RectTransform)transform).sizeDelta = new Vector2(0, 30);
        ((RectTransform)transform).anchoredPosition = Vector2.up * 15;
        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y);

        // Setup text component.
        text = gameObject.AddComponent<Text>();
        text.color = Color.white;
        text.fontSize = 30;
        text.alignment = TextAnchor.LowerCenter;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;

        print("Converted " + name + " to a " + typeof(Logger).Name + ". ");
        gameObject.name = "Logger";
    }
}