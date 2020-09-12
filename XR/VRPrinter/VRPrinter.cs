using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPrinter
{
    static bool noCamWarningMade = false;
    static bool lastWasSlash = false;

    static TextMesh textDisplay;
    public static string defaultMeshName = "OnScreenPrinter output";

    struct PendingMessage
    {
        public enum PrintStyle { Print, CleanPrint }
        public PrintStyle printStyle;
        public object message;
        public Object sender;
        public PendingMessage(object message, PrintStyle printStyle)
        {
            this.printStyle = printStyle;
            this.message = message;
            sender = null;
        }
        public PendingMessage(object message, PrintStyle printStyle, Object sender)
        {
            this.printStyle = printStyle;
            this.message = message;
            this.sender = sender;
        }
    }
    static List<PendingMessage> pendingMessages = new List<PendingMessage>();

    #region Print.
    /// <summary>
    /// Spawn a textMesh with a message in front of the main camera.
    /// </summary>
    /// <param name="caller">Optional: the object sending the message. This will print the message to the console.</param>
    public static void Print(object input, Object caller = null)
    {
        // Add pending debug message if no camera exists.
        if (!HasValidCam())
        {
            LogNoCamWarning();
            AddPendingMessage(input, PendingMessage.PrintStyle.Print, caller);
            return;
        }

        // Print message on display with time and oscillating slash to show movement.
        string slash = (lastWasSlash = !lastWasSlash) ? "\\" : "/";
        AddToDisplay("[" + Time.time.ToString("00.0") + "]" + slash + " " + input.ToString());
        if (caller)
            Debug.Log(input + " (" + caller.GetType() + " - " + caller.name + ")", caller);
    }

    /// <summary>
    /// Print without debug infos.
    /// </summary>
    public static void CleanPrint(object input)
    {
        // Add pending message if no camera exists.
        if (!HasValidCam())
        {
            LogNoCamWarning();
            AddPendingMessage(input, PendingMessage.PrintStyle.CleanPrint, null);
            return;
        }

        // Add to our display.
        AddToDisplay(input.ToString());
    }

    static void AddToDisplay(string text)
    {
        if (textDisplay == null)
            CreateDisplay();
        textDisplay.text += SliceText(text, 30);
        textDisplay.text += "\n";
        if (textDisplay.text.Length > 2000)
            textDisplay.text = textDisplay.text.Remove(0, 1000);
    }

    public static void Clear()
    {
        if (Camera.main == null || textDisplay == null)
            return;
        textDisplay.text = "";
    }
    #endregion

    static void CreateDisplay()
    {
        // Setup display transform.
        Transform newObject = new GameObject(defaultMeshName).transform;
        newObject.parent = Camera.main.transform;
        newObject.localPosition = new Vector3(0, -.2f, .5f);
        newObject.localRotation = Quaternion.identity;
        newObject.localScale = Vector3.one * .01f;

        // Setup display text mesh.
        textDisplay = newObject.gameObject.AddComponent<TextMesh>();
        textDisplay.alignment = TextAlignment.Center;
        textDisplay.anchor = TextAnchor.LowerCenter;
        textDisplay.fontSize = 30;
    }

    static string SliceText(string text, int maxLength)
    {
        if (text.Length <= maxLength)
            return text;

        List<string> slices = new List<string>();
        int storedChars = 0;
        int lastBreak = 0;
        while (storedChars < text.Length)
        {
            bool sliced = false;
            string newSlice = "";
            if (text.Length < lastBreak + maxLength)
                // We reached the end of the text, add the remaining part.
                newSlice = text.Substring(lastBreak, text.Length - lastBreak);
            else
            {
                // Go backwards until the last break and search for a whitespace.
                for (int i = maxLength; i > 0; i--)
                    if (text.Length > lastBreak + i && text[lastBreak + i] == ' ')
                    {
                        int newSliceLength = Mathf.Clamp(i, 0, text.Length);
                        newSlice = text.Substring(lastBreak, newSliceLength);
                        lastBreak += newSliceLength;
                        sliced = true;
                        break;
                    }

                // If no whitespaces were found, slice arbitrarily.
                if (!sliced)
                {
                    int newSliceLength = Mathf.Clamp(lastBreak + maxLength, 0, text.Length - 1) - lastBreak;
                    newSlice = text.Substring(lastBreak, newSliceLength);
                    lastBreak += newSliceLength;
                }
            }

            // Store the slice.
            slices.Add(newSlice + '\n');
            storedChars += newSlice.Length;
        }

        // Return slices in a string.
        string result = "";
        foreach (string str in slices)
            result += str;
        return result;
    }

    static bool HasValidCam()
    {
        // Return whether we found a camera or not.
        if (Camera.main == null)
        {
            if (!noCamWarningMade)
            {
                Debug.Log("No main camera found. Putting messages on hold.");
                noCamWarningMade = true;
            }
            return false;
        }
        return true;
    }

    static void LogNoCamWarning()
    {
        Debug.Log("OnScreenPrinter: No MainCamera was found. \n" +
            "The message will be printed when a MainCamera appears in the scene. To have a MainCamera, add the tag MainCamera to your primary camera.");
    }

    #region Print pending messages.
    static void AddPendingMessage(object message, PendingMessage.PrintStyle printStyle, Object sender)
    {
        PendingMessage newPendingMessage;
        if (!sender)
            newPendingMessage = new PendingMessage(message, printStyle);
        else
            newPendingMessage = new PendingMessage(message, printStyle, sender);
        pendingMessages.Add(newPendingMessage);

        // Start waiting for the mainCam.
        if (waitingForValidCam == null)
        {
            waitingForValidCam = WaitForValidCam();
            CoroutineStarter.StartCoroutine(waitingForValidCam);
        }
    }

    static IEnumerator waitingForValidCam;
    static IEnumerator WaitForValidCam()
    {
        // Print pending messages when we find a main camera.
        while (!Camera.main)
            yield return null;

        foreach (PendingMessage pendingMessage in pendingMessages)
            if (pendingMessage.printStyle == PendingMessage.PrintStyle.CleanPrint)
                CleanPrint(pendingMessage.message);
            else
                Print(pendingMessage.message, pendingMessage.sender);
        pendingMessages.Clear();
        waitingForValidCam = null;
    }
    #endregion

    #region Display at position.
    static float xOffset = .35f;
    static float yOffset = .15f;

    public static TextMesh CreateDisplay(TextAnchor position)
    {
        Transform displayTransform = new GameObject("Display").transform;
        displayTransform.parent = Camera.main.transform;
        displayTransform.localPosition = new Vector3(0, -.2f, .5f);
        displayTransform.localRotation = Quaternion.identity;
        displayTransform.localScale = Vector3.one * .01f;
        TextMesh display = displayTransform.gameObject.AddComponent<TextMesh>();
        display.fontSize = 30;

        // Start waiting for a valid cam or send a display to the valid cam.
        if (!HasValidCam())
        {
            display.gameObject.SetActive(false);
            CoroutineStarter.StartCoroutine(DisplayWaitForCamRoutine(display, position));
        }
        else
        {
            display.transform.parent = Camera.main.transform;
            SetupDisplayAtPosition(display, position);
        }
        return display;
    }

    static IEnumerator DisplayWaitForCamRoutine(TextMesh display, TextAnchor position)
    {
        yield return new WaitUntil(() => Camera.main != null);
        display.gameObject.SetActive(true);
        SetupDisplayAtPosition(display, position);
    }

    static void SetupDisplayAtPosition(TextMesh display, TextAnchor position)
    {
        Vector3 localPosition = new Vector3(0, 0, .5f);

        // Set x offset.
        if ((int)position % 3 == 0)
            localPosition.x = -xOffset;
        else if ((int)position == 2 || (int)position == 5 || (int)position == 8)
            localPosition.x = xOffset;

        // Set y offset.
        if ((int)position > 5)
            localPosition.y = -yOffset;
        else if ((int)position < 3)
            localPosition.y = yOffset;

        // Set position and anchor.
        display.transform.localPosition = localPosition;
        display.anchor = position;
    }
    #endregion
}

public class CoroutineStarter : MonoBehaviour
{
    public static CoroutineStarter Instance { get; private set; }


    void OnDestroy()
    {
        Instance = null;
    }


    public new static Coroutine StartCoroutine(IEnumerator routine)
    {
        if (!Instance)
            Instance = new GameObject("Coroutine starter").AddComponent<CoroutineStarter>();
        return ((MonoBehaviour)Instance).StartCoroutine(routine);
    }
}
