using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIUtilities
{
    /// <summary>
    /// Clamp the position of the rect to the area. (only works on XY)
    /// </summary>
    /// <param name="rect">The rect to clamp.</param>
    /// <param name="area">The area in which the rect must stay.</param>
    public static void ClampToRect(this RectTransform rect, RectTransform area)
    {
        Vector3[] corners = new Vector3[4];
        area.GetWorldCorners(corners);
        Vector2 newPosition = new Vector2(Mathf.Clamp(rect.position.x, corners[0].x, corners[2].x), Mathf.Clamp(rect.position.y, corners[0].y, corners[2].y));
        rect.position = newPosition;
    }

    /// <summary>
    /// Get the size of the rect on the screen.
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    static public Rect GetScreenRect(this RectTransform rect)
    {
        Vector2 size = Vector2.Scale(rect.rect.size, rect.lossyScale);
        return new Rect((Vector2)rect.position - (size * 0.5f), size);
    }

    public static void SetNormalColor(this Button button, Color color)
    {
        var colors = button.colors;
        colors.normalColor = color;
        button.colors = colors;
    }

    public static void SetTexture(this Image image, Texture texture)
    {
        image.sprite = Sprite.Create((Texture2D)texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(.5f, .5f));
    }

    public static RectTransform CreateBackground(RectTransform parent, Color? color = null)
    {
        RectTransform background = new GameObject("Background", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        background.parent = parent;
        background.MoveToBack();
        background.StretchToFill();
        background.GetComponent<Image>().color = color ?? Color.white;
        return background;
    }

    /// <summary>
    /// Create a background on top of a RectTransform.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="color"></param>
    /// <param name="siblingIndex">The sibling index of the new blocker. Will be at the bottom if left empty.</param>
    /// <returns></returns>
    public static RectTransform CreateBlocker(RectTransform parent, Color color, int siblingIndex = -1)
    {
        RectTransform blocker = CreateBackground(parent, color);
        blocker.name = "Blocker";
        if (siblingIndex < 0)
            blocker.MoveToFront();
        else
            blocker.SetSiblingIndex(siblingIndex);
        return blocker;
    }

    public static float GetAspectRatio(this RectTransform transform)
    {
        return transform.rect.width / transform.rect.height;
    }

    /// <summary>
    /// Set rectTransform to fill its parent.
    /// </summary>
    /// <param name="rectTransform"></param>
    public static void StretchToFill(this RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector3.zero;
        rectTransform.anchorMax = Vector3.one;
        rectTransform.anchoredPosition = Vector3.zero;
        rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
    }

    /// <summary>
    /// Releases the previous renderTexture and creates a new one with a new resolution.
    /// </summary>
    /// <param name="renderCamera">The camera which has the render texture.</param>
    /// <param name="resolution">The new resolution in pixels.</param>
    /// <returns></returns>
    public static RenderTexture SetRenderTextureResolution(Camera renderCamera, Vector2 resolution)
    {
        renderCamera.targetTexture.Release();
        RenderTexture newTexture = new RenderTexture((int)resolution.x, (int)resolution.y, 24);
        newTexture.name = "runtime texture (" + resolution.x + "; " + resolution.y + ")";
        renderCamera.targetTexture = newTexture;
        return newTexture;
    }


    #region Move to front/to back --------------
    /// <summary>
    /// Move the transform to the bottom of its parent hierarchy.
    /// </summary>
    /// <param name="transform"></param>
    public static void MoveToFront(this Transform transform)
    {
        if (transform.parent == null)
            return;
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// Move the transform to the bottom of its parent hierarchy.
    /// </summary>
    /// <param name="transform"></param>
    public static void MoveToBack(this Transform transform)
    {
        if (transform.parent == null)
            return;
        transform.SetAsFirstSibling();
    }
    #endregion ----------------------------


    #region OnPointer selectable functions ----------------
    public static void OnPointerEnter(this Selectable selectable)
    {
        selectable.OnPointerEnter(new PointerEventData(null));
    }
    public static void OnPointerClick(this IPointerClickHandler pointerClickable)
    {
        pointerClickable.OnPointerClick(new PointerEventData(null));
    }
    public static void OnSelect(this Selectable selectable)
    {
        selectable.OnSelect(new PointerEventData(null));
    }
    public static void OnPointerExit(this Selectable selectable)
    {
        selectable.OnPointerExit(new PointerEventData(null));
    }
    #endregion ------------------------------------------


    public static void SetGraphicsClickable(Selectable[] selectables, bool state)
    {
        // Disable all selectable image targets in the vocable view.
        foreach (Selectable selectable in selectables)
            foreach (Graphic graphic in selectable.GetComponentsInChildren<Graphic>(true))
                graphic.raycastTarget = state;
    }
    public static void SetGraphicsClickable(Transform parent, bool state)
    {
        SetGraphicsClickable(parent.GetComponentsInChildren<Selectable>(true), state);
    }

    public static Bounds GetBounds(this RectTransform rectTransform)
    {
        return new Bounds(rectTransform.position, new Vector3(rectTransform.rect.width, rectTransform.rect.height, 3));
    }

    public static BoxCollider GetOrAddFittingBoxCollider(this RectTransform rectTransform)
    {
        rectTransform.gameObject.GetOrAddComponent<BoxCollider>().size = rectTransform.GetBounds().size;
        return rectTransform.GetComponent<BoxCollider>();
    }

#if UNITY_EDITOR
    [MenuItem("CONTEXT/RectTransform/Add fitting box collider", false, 1000)]
    static void AddFittingBoxCollider()
    {
        ((RectTransform)Selection.activeTransform).GetOrAddFittingBoxCollider();
    }

    [MenuItem("CONTEXT/RectTransform/Add background", false, 1000)]
    static void AddBackground()
    {
        CreateBackground((RectTransform)Selection.activeTransform);
    }

    [MenuItem("CONTEXT/Button/Setup", false, 1000)]
    static void SetupButton()
    {
        // Modify images.
        List<Image> imagesToModify = new List<Image>();
        foreach (GameObject gameObject in Selection.gameObjects)
            if (gameObject.GetComponent<Image>())
                imagesToModify.Add(gameObject.GetComponent<Image>());
        Undo.RecordObjects(imagesToModify.ToArray(), "Setup button images");
        foreach (Image image in imagesToModify)
            image.sprite = null;

        // Modify text.
        List<Text> textsToModify = new List<Text>();
        foreach (GameObject gameObject in Selection.gameObjects)
            if (gameObject.GetComponentInChildren<Text>())
                textsToModify.Add(gameObject.GetComponentInChildren<Text>());
        Undo.RecordObjects(textsToModify.ToArray(), "Setup button texts");
        RenameChildTextInSelection();
        foreach (var textToModify in textsToModify)
        {
            textToModify.verticalOverflow = VerticalWrapMode.Overflow;
            RectTransform textTransform = textToModify.GetComponent<RectTransform>();
            textTransform.offsetMin = new Vector2(5, 5);
            textTransform.offsetMax = new Vector2(-5, -5);
        }
    }


    #region Executing a button's OnClick event manually ------------------------------
    [MenuItem("CONTEXT/Button/Execute OnClick", false, 1000)]
    static void ExecuteOnClickManuallyOnSelection()
    {
        foreach (GameObject gameObject in Selection.gameObjects)
            if (gameObject.GetComponent<Button>())
                ExecuteOnClickManually(gameObject.GetComponent<Button>());
    }

    public static void ExecuteOnClickManually(Button button)
    {
        SerializedProperty onClickCalls = new SerializedObject(button).FindProperty("m_OnClick.m_PersistentCalls.m_Calls");

        // Execute every onClick calls.
        for (int i = 0; i < onClickCalls.arraySize; i++)
        {
            SerializedProperty onClickCall = onClickCalls.GetArrayElementAtIndex(i);

            // Try to cast the target to a monoBehaviour variable.
            MonoBehaviour callTarget = button.onClick.GetPersistentTarget(i) as MonoBehaviour;
            if (!callTarget)
                continue;

            // Tries to invoke the corresponding method on the target, for all types until success.
            string methodName = button.onClick.GetPersistentMethodName(i);
            if (callTarget.FindAndInvokeMethod(methodName, onClickCall.FindPropertyRelative("m_Arguments.m_ObjectArgument").objectReferenceValue)
                || callTarget.FindAndInvokeMethod(methodName, onClickCall.FindPropertyRelative("m_Arguments.m_IntArgument").intValue)
                || callTarget.FindAndInvokeMethod(methodName, onClickCall.FindPropertyRelative("m_Arguments.m_FloatArgument").floatValue)
                || callTarget.FindAndInvokeMethod(methodName, onClickCall.FindPropertyRelative("m_Arguments.m_BoolArgument").boolValue)
                || callTarget.FindAndInvokeMethod<object>(methodName))
                continue;
        }
    }
    #endregion ----------------------------------------------------------------------


    [MenuItem("CONTEXT/RectTransform/Rename text", false, 1000)]
    static void RenameChildTextInSelection()
    {
        // Get text components in selection.
        List<KeyValuePair<string, Text>> textsToRename = new List<KeyValuePair<string, Text>>();
        List<Text> affectedTexts = new List<Text>();
        foreach (GameObject gameobject in Selection.gameObjects)
        {
            textsToRename.Add(new KeyValuePair<string, Text>(gameobject.name, gameobject.GetComponentInChildren<Text>()));
            if (textsToRename[textsToRename.Count - 1].Value != null)
                affectedTexts.Add(textsToRename[textsToRename.Count - 1].Value);
        }
        if (textsToRename.Count < 1)
            return;

        // Modify text components.
        Undo.RecordObjects(affectedTexts.ToArray(), "Rename child text");
        foreach (KeyValuePair<string, Text> textToRename in textsToRename)
        {
            textToRename.Value.text = textToRename.Key;
            EditorUtility.SetDirty(textToRename.Value);
        }
    }

    [MenuItem("CONTEXT/RectTransform/Rename title", false, 1000)]
    static void RenameTitle()
    {
        try
        {
            Selection.activeTransform.Find("Title").GetComponentInChildren<Text>().text = Selection.activeTransform.name;
            EditorUtility.SetDirty(Selection.activeTransform.Find("Title").GetComponentInChildren<Text>());
        }
        catch { }
    }

    [MenuItem("CONTEXT/Canvas/To world canvas", false, 1000)]
    static void ToWorldCanvas()
    {
        Canvas canvas = Selection.activeTransform.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        if (!canvas.worldCamera)
            canvas.worldCamera = GameObject.FindObjectOfType<Camera>();
        Selection.activeTransform.localScale = Vector3.one * .0015f;
        Selection.activeTransform.localPosition = Vector3.zero;
    }

    [MenuItem("CONTEXT/Text/Add size fitter", false, 1000)]
    static void AddSizeFitter()
    {
        ContentSizeFitter sizeFitter = Selection.activeGameObject.GetOrAddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }


    #region Vertical layout setup -----------------
    enum VerticalLayoutDynamicDirection { Top, Bottom }
    [MenuItem("CONTEXT/VerticalLayoutGroup/Dynamic top", false, 1000)]
    static void VerticalLayoutTop()
    {
        SetDynamicVerticalLayout(Selection.activeTransform.GetOrAddComponent<VerticalLayoutGroup>(), VerticalLayoutDynamicDirection.Top);
    }

    [MenuItem("CONTEXT/VerticalLayoutGroup/Dynamic bottom", false, 1000)]
    static void VerticalLayoutBottom()
    {
        SetDynamicVerticalLayout(Selection.activeTransform.GetOrAddComponent<VerticalLayoutGroup>(), VerticalLayoutDynamicDirection.Bottom);
    }

    static void SetDynamicVerticalLayout(VerticalLayoutGroup layout, VerticalLayoutDynamicDirection direction)
    {
        layout.childAlignment = direction == VerticalLayoutDynamicDirection.Top ? TextAnchor.UpperCenter : TextAnchor.LowerCenter;
        layout.spacing = 5;
        layout.GetOrAddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.MinSize;
        layout.GetComponent<RectTransform>().pivot = direction == VerticalLayoutDynamicDirection.Top ? new Vector2(0, 1) : new Vector2(0, 0);
    }

    [MenuItem("CONTEXT/VerticalLayoutGroup/Add space", false, 1000)]
    static void AddSpaceToVerticalLayout()
    {
        RectTransform padding = new GameObject("Space").AddComponent<RectTransform>();
        padding.sizeDelta = Vector2.one * 10;
        padding.parent = Selection.activeTransform;
        padding.SetSiblingIndex(Selection.activeTransform.childCount > 1 ? Selection.activeTransform.childCount - 2 : 0);
    }
    #endregion ----------------------------------


    [MenuItem("CONTEXT/Dropdown/Setup", false, 1000)]
    static void SetupDropdown()
    {
        Selection.activeTransform.FindRecursive("Item Label").GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
        RectTransform scrollbar = ((RectTransform)Selection.activeTransform.FindRecursive("Scrollbar"));
        scrollbar.sizeDelta = scrollbar.sizeDelta.SetX(10);
    }

    [MenuItem("GameObject/UI/Title", false, 10000)]
    static void CreateTitle()
    {
        // Create title and setup transform.
        RectTransform titleTransform = new GameObject("Title", typeof(RectTransform), typeof(Text)).GetComponent<RectTransform>();
        Undo.RegisterCreatedObjectUndo(titleTransform.gameObject, "Create " + titleTransform.gameObject.name);
        titleTransform.parent = Selection.activeTransform;
        titleTransform.pivot = titleTransform.anchorMin = titleTransform.anchorMax = new Vector2(.5f, 1);
        titleTransform.Reset();
        titleTransform.anchoredPosition = titleTransform.localPosition.SetY(-30);
        titleTransform.sizeDelta = new Vector2(400, 30);

        // Setup the title text.
        Text titleText = titleTransform.GetComponent<Text>();
        titleText.text = "Title";
        titleText.fontSize = 23;
        titleText.color = Color.black;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.verticalOverflow = VerticalWrapMode.Overflow;

        // Set active selection.
        Selection.activeTransform = titleTransform;
    }
#endif

    #region Dropdown options -------------
    public static void SetOptions(this Dropdown dropdown, string[] options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    public static void AddOptions(this Dropdown dropdown, string[] options)
    {
        List<Dropdown.OptionData> newOptions = new List<Dropdown.OptionData>();
        foreach (var value in options)
            newOptions.Add(new Dropdown.OptionData() { text = value });
        dropdown.AddOptions(newOptions);
    }
    #endregion ---------------------------

    #region Getting values out of input fields ---------------------
    public static int GetIntInput(this InputField inputField)
    {
        if (int.TryParse(inputField.text, out int result))
            return result;
        else
            return 0;
    }

    public static int GetFloatInput(this InputField inputField)
    {
        if (int.TryParse(inputField.text, out int result))
            return result;
        else
            return 0;
    }
    #endregion ------------------------------------------------------

    public static string GetStringValue(this Dropdown dropdown)
    {
        return dropdown.options[dropdown.value].text;
    }
}
