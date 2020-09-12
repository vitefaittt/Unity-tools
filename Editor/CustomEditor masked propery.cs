#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(MyClass)), UnityEditor.CanEditMultipleObjects]
public class MyClassEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        MyClass script = target as MyClass;
        System.Collections.Generic.List<string> excludedProperties = new System.Collections.Generic.List<string>();

        if (script.aBool)
            excludedProperties.Add("aField");

        DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
        serializedObject.ApplyModifiedProperties();
    }
}
#endif