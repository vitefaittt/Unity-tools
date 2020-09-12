using UnityEditor;
using UnityEngine;

public class GetMousePositionInGameWindow
{
    public static Vector2 GetPosition()
    {
        Vector2 mousePositionOnScreen = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        EditorWindow gameWindow = EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView, UnityEditor"));
        EditorWindow.GetWindow(Type.GetType("UnityEditor.SceneView, UnityEditor"));
        Vector2 mousePositionInGameWindow = mousePositionOnScreen - gameWindow.position.position;
        return mousePositionInGameWindow.SetY(mousePositionInGameWindow.y - 20);
    }
}