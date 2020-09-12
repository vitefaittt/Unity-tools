using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class SomeClass : EditorWindow
{
    public void OnGUI()
    {
        // Settings button with right click.
        if (GUI.Button(
            EditorGUILayout.GetControlRect(false, 20, GUILayout.MaxWidth(20)),
            EditorGUIUtility.IconContent("_Popup"),
            GUI.skin.FindStyle("IconButton")))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Settings"), false, () =>
            {
                Debug.Log("hey");
            });
            menu.ShowAsContext();
        }


        // Other version:
        Event current = Event.current;

        if (position.Contains(current.mousePosition) && current.button == 1)
        {
            //Do a thing, in this case a drop down menu

            GenericMenu menu = new GenericMenu();

            menu.AddDisabledItem(new GUIContent("I clicked on a thing"));
            menu.AddItem(new GUIContent("Do a thing"), false, () => { Debug.Log("hey"); });
            menu.ShowAsContext();

            current.Use();
        }
    }
}