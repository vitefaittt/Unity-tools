using System;
using UnityEditor;
using UnityEngine;

public interface IOpenClose
{
    void Open();
    void Close();
}

/// <summary>
/// Draws Open/Close buttons in the Inspector.
/// </summary>
[Serializable]
public class OpenCloseProperty
{
    IOpenClose savedOpenClose;
    IOpenClose OpenClose {
        get {
#if UNITY_EDITOR
            if (savedOpenClose == null)
                savedOpenClose = Selection.activeGameObject.GetComponent<IOpenClose>();
#endif
            return savedOpenClose;
        }
    }

    public void Open()
    {
        OpenClose.Open();
    }

    public void Close()
    {
        OpenClose.Close();
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(OpenCloseProperty))]
public class OpenClosePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Inline child fields.
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        float buttonWidth = position.width * .5f - 2.5f;
        Rect openRect = new Rect(position.x, position.y, buttonWidth, position.height);
        Rect closeRect = new Rect(position.x + buttonWidth + 5, position.y, buttonWidth, position.height);

        if (GUI.Button(openRect, "Open"))
            ((OpenCloseProperty)fieldInfo.GetValue(property.serializedObject.targetObject)).Open();

        if (GUI.Button(closeRect, "Close"))
            ((OpenCloseProperty)fieldInfo.GetValue(property.serializedObject.targetObject)).Close();

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
#endif
