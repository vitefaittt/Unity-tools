#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(MyClass)), UnityEditor.CanEditMultipleObjects]
class MyClassEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        MyClass script = (MyClass)target;

        if (GUILayout.Button("Button"))
            script.SomeFunction();
    }
}
#endif