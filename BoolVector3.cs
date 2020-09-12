using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BoolVector3
{
    public bool x, y, z;
    public static BoolVector3 none => new BoolVector3(false, false, false);
    public static  BoolVector3 all => new BoolVector3(true, true, true);


    public BoolVector3(bool x, bool y, bool z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }


    public Vector3 ToVector3()
    {
        return new Vector3(x ? 1 : 0, y ? 1 : 0, z ? 1 : 0);
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(BoolVector3))]
public class BoolVector3Drawer : PropertyDrawer
{
    readonly float labelWidth = 15;
    readonly float toggleWidth = 15;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        BoolVector3 objectProperty = (BoolVector3)fieldInfo.GetValue(property.serializedObject.targetObject);

        EditorGUI.BeginProperty(position, label, property);

        // Inline child fields.
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Create the toggles and their labels.
        int toggleIndex = 0;
        GUIStyle style = GUI.skin.label;
        style.alignment = TextAnchor.MiddleCenter;
        objectProperty.x = EditorGUI.Toggle(GetToggleRect(position, toggleIndex), objectProperty.x);
        EditorGUI.LabelField(GetLabelRect(position, toggleIndex), "X", style);
        toggleIndex++;

        objectProperty.y = EditorGUI.Toggle(GetToggleRect(position, toggleIndex), objectProperty.y);
        EditorGUI.LabelField(GetLabelRect(position, toggleIndex), "Y", style);
        toggleIndex++;

        objectProperty.z = EditorGUI.Toggle(GetToggleRect(position, toggleIndex), objectProperty.z);
        EditorGUI.LabelField(GetLabelRect(position, toggleIndex), "Z", style);

        // Set indent back to what it was.
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }


    Rect GetToggleRect(Rect position, int index)
    {
        return new Rect(position.x + (labelWidth + toggleWidth) * index, position.y, toggleWidth, position.height);
    }

    Rect GetLabelRect(Rect position, int index)
    {
        return new Rect(position.x + (labelWidth + toggleWidth) * index + toggleWidth, position.y, labelWidth, position.height);
    }
}
#endif
