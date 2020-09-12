#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(MyProperty))]
public class MyPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MyProperty objectProperty = (MyProperty)fieldInfo.GetValue(property.serializedObject.targetObject);

        EditorGUI.BeginProperty(position, label, property);

        // Inline child fields.
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Draw the property name.
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        float buttonWidth = position.width * .5f - 2.5f;
        Rect openRect = new Rect(position.x, position.y, buttonWidth, position.height);
        Rect closeRect = new Rect(position.x + buttonWidth + 5, position.y, buttonWidth, position.height);

        if (GUI.Button(openRect, "Open"))
            ((MyProperty)fieldInfo.GetValue(property.serializedObject.targetObject)).Open();

        if (GUI.Button(closeRect, "Close"))
            ((MyProperty)fieldInfo.GetValue(property.serializedObject.targetObject)).Close();

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
#endif
