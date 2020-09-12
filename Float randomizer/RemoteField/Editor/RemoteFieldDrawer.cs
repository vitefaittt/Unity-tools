using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(RemoteFloatField))]
public class RemoteFloatFieldDrawer : RemoteFieldDrawer<float>
{
    protected override string PrintValue(SerializedProperty property)
    {
        return property.FindPropertyRelative("_Value").floatValue.ToString();
    }
}

public abstract class RemoteFieldDrawer<T> : PropertyDrawer
{
    float lineHeight;
    int numLines = 6;
    bool isOut = true;
    List<string> options;
    List<int> selectedIndexes = new List<int>();
    char[] split = { '.' };


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = lineHeight + 1;
        float baseWidth = position.width;

        isOut = EditorGUI.Foldout(position, isOut, label);
        if (property.FindPropertyRelative("_Value") != null)
        {
            position.xMin += baseWidth / 3;
            EditorGUI.LabelField(position, property.FindPropertyRelative("fieldName").stringValue);
            position.xMin += baseWidth / 3;
            EditorGUI.LabelField(position, PrintValue(property));
        }

        // Indent.
        position.xMin = 20;
        position.y += lineHeight + 1;

        if (!isOut)
            return;

        selectedIndexes.Clear();

        // Draw gameObject field.
        EditorGUI.PropertyField(position, property.FindPropertyRelative("targetGameObject"));
        GameObject target = (GameObject)property.FindPropertyRelative("targetGameObject").objectReferenceValue;
        if (!target)
        {
            numLines = 2;
            return;
        }

        // Get names of components.
        List<string> componentNames = new List<string>();
        componentNames.Add("");
        foreach (Component component in target.GetComponents<Component>())
            componentNames.Add(component.GetType().ToString());

        object targetComponent = property.FindPropertyRelative("targetComponent").objectReferenceValue;
        selectedIndexes.Add(0);
        if (targetComponent != null)
            selectedIndexes[0] = componentNames.IndexOf(targetComponent.GetType().ToString());
        if (selectedIndexes[0] == -1)
            selectedIndexes[0] = 0;

        position.y += lineHeight + 1;

        // Set target component from popup.
        selectedIndexes[0] = EditorGUI.Popup(position, "Component", selectedIndexes[0], componentNames.ToArray());
        if (selectedIndexes[0] != 0)
        {
            property.FindPropertyRelative("targetComponent").objectReferenceValue = target.GetComponents<Component>()[selectedIndexes[0] - 1];
            targetComponent = target.GetComponents<Component>()[selectedIndexes[0] - 1];
        }
        if (targetComponent == null)
        {
            numLines = 3;
            return;
        }

        // Get field name.
        string fieldName = "";
        string[] splitString = property.FindPropertyRelative("fieldName").stringValue.Split(split);
        numLines = 3 + splitString.Length + 1;

        // Get target member.
        Type currentType = targetComponent.GetType();
        for (int i = 0; i < splitString.Length; i++)
        {
            selectedIndexes.Add(0);

            options = new List<string>();
            options.Add("");
            MemberInfo[] members = currentType.GetMembers(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MemberInfo member in members)
                if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
                {
                    object[] atributes = member.GetCustomAttributes(typeof(ObsoleteAttribute), true);
                    if (atributes.Length == 0)
                        options.Add(member.Name);
                }

            if (options.Count > 1)
            {
                selectedIndexes[i + 1] = options.IndexOf(splitString[i]);
                if (selectedIndexes[i + 1] == -1)
                    selectedIndexes[i + 1] = 0;
                position.y += lineHeight + 1;

                selectedIndexes[i + 1] = EditorGUI.Popup(position, "Member Name", selectedIndexes[i + 1],
                    options.ToArray());
                if (selectedIndexes[i + 1] != 0)
                {
                    if (i != 0) fieldName += ".";
                    fieldName += options[selectedIndexes[i + 1]];
                }
                MemberInfo[] info = currentType.GetMember(
                    options[selectedIndexes[i + 1]], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (info.Length > 0)
                {
                    if (info[0].MemberType == MemberTypes.Field)
                        currentType = ((FieldInfo)info[0]).FieldType;
                    else if (info[0].MemberType == MemberTypes.Property)
                        currentType = ((PropertyInfo)info[0]).PropertyType;
                }
            }
            else
            {
                position.y += lineHeight + 1;
                EditorGUI.LabelField(position, "No watchable data");
            }
        }

        if (currentType != typeof(T))
        {
            if (fieldName.Length > 0 && fieldName[fieldName.Length - 1] != '.')
                fieldName += ".";
        }
        property.FindPropertyRelative("fieldName").stringValue = fieldName;
    }


    protected abstract string PrintValue(SerializedProperty property);

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        lineHeight = base.GetPropertyHeight(property, label);

        return isOut ? (lineHeight + 1) * numLines : lineHeight;
    }
}
