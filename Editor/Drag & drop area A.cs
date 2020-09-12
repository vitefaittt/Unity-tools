Rect DrawDropArea<T>(string boxText, System.Action<T> ObjectAction)
{
    Event evt = Event.current;
    Rect boxRect = GUILayoutUtility.GetRect(50, 30, GUILayout.ExpandWidth(true));
    GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
    boxStyle.alignment = TextAnchor.MiddleCenter;
    if (EditorGUIUtility.isProSkin)
        boxStyle.normal.textColor = Color.white;
    GUI.Box(boxRect, boxText, boxStyle);

    switch (evt.type)
    {
        case EventType.DragUpdated:
        case EventType.DragPerform:
            if (!boxRect.Contains(evt.mousePosition))
                break;

            UnityEditor.DragAndDrop.visualMode = UnityEditor.DragAndDropVisualMode.Copy;
            if (evt.type == EventType.DragPerform)
            {
                UnityEditor.DragAndDrop.AcceptDrag();

                foreach (UnityEngine.Object droppedObject in UnityEditor.DragAndDrop.objectReferences)
                    if (droppedObject is T target)
                        ObjectAction(target);
            }
            break;
    }

    return boxRect;
}