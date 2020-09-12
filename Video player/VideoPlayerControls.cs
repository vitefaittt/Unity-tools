using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerControls : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("CONTEXT/VideoPlayer/Add controls")]
    static void AddControlsToPlayer()
    {
        Undo.AddComponent(Selection.activeGameObject, typeof(VideoPlayerControls));
        EditorGUIUtility.PingObject(Selection.activeGameObject);
    }

    [MenuItem("CONTEXT/VideoPlayer/Add controls", true)]
    static bool AddControlsToPlayerValidation()
    {
        return !Selection.activeGameObject.GetComponent<VideoPlayerControls>();
    }
#endif
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(VideoPlayerControls)), UnityEditor.CanEditMultipleObjects]
class VideoPlayerControlsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        VideoPlayer player = ((MonoBehaviour)target).GetComponent<VideoPlayer>();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("►"))
            player.Play();
        if (GUILayout.Button("ll"))
            player.Pause();
        if (GUILayout.Button("■"))
            player.Stop();
        GUILayout.EndHorizontal();
    }
}
#endif