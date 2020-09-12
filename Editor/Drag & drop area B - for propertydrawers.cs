public void DrawDropArea(Rect position, SerializedProperty property, float height)
{
    Event evt = Event.current;
    Rect boxRect = new Rect(position.x, position.y + EditorGUI.GetPropertyHeight(property) + 5, position.width, height - 10);
    GUI.Box(boxRect, "Drop items here");

    switch (evt.type)
    {
        case EventType.DragUpdated:
        case EventType.DragPerform:
            if (!boxRect.Contains(evt.mousePosition))
                return;

            UnityEditor.DragAndDrop.visualMode = UnityEditor.DragAndDropVisualMode.Copy;
            if (evt.type == EventType.DragPerform)
            {
                UnityEditor.DragAndDrop.AcceptDrag();

                foreach (Object dragged_object in UnityEditor.DragAndDrop.objectReferences)
                    // Do stuff here. 
                    ;
            }
            break;
    }
}