using UnityEditor;
using UnityEngine;

public class AudioSourceControls : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("CONTEXT/AudioSource/Add controls")]
    static void AddControlsToAudio()
    {
        Undo.AddComponent(Selection.activeGameObject, typeof(AudioSourceControls));
        EditorGUIUtility.PingObject(Selection.activeGameObject);
    }

    [MenuItem("CONTEXT/AudioSource/Add controls", true)]
    static bool CanAddControlsToAudio()
    {
        return !Selection.activeGameObject.GetComponent<AudioSourceControls>();
    }
#endif
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(AudioSourceControls)), UnityEditor.CanEditMultipleObjects]
class AudioSourceControlsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        AudioSource audio = ((MonoBehaviour)target).GetComponent<AudioSource>();

        GUI.enabled = false;
        GUILayout.Toggle(audio.isPlaying, "Is Playing");
        GUI.enabled = true;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("►"))
            audio.Play();
        if (GUILayout.Button("ll"))
            audio.Pause();
        if (GUILayout.Button("■"))
            audio.Stop();
        GUILayout.EndHorizontal();
    }
}
#endif