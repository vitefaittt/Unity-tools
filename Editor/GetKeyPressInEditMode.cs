using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GetKeyPressInEditMode
{
    static GetKeyPressInEditMode()
    {
        SceneView.duringSceneGui += view =>
        {
            if (Event.current != null && Event.current.keyCode != KeyCode.None)
                Debug.Log("Key pressed in editor: " + Event.current.keyCode);
        };
    }
}