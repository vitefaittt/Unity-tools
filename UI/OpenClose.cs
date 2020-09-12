using UnityEngine;

public abstract class OpenClose : MonoBehaviour
{
    public abstract void Open();
    public abstract void Close();
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(OpenClose)), UnityEditor.CanEditMultipleObjects]
class IOpenCloseEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        OpenClose script = (OpenClose)target;
        GUILayout.Space(5);

        if (GUILayout.Button("Open"))
            script.Open();
        if (GUILayout.Button("Close"))
            script.Close();
    }
}
#endif
