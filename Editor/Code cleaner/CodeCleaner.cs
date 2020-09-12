using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CodeCleaner : EditorWindow
{
    string input;
    Vector2 scroll;
    List<CleanerModule> cleaners = new List<CleanerModule>()
    {
        new BracketsCleaner(),
        new PrivatesCleaner(),
        new RegionsRemover(),
        new NewlinesCleaner(),
        new IfExpressionCleaner()
    };
    int currentCleaner = -1;
    string previewInput;


    void OnGUI()
    {
        // Text.
        scroll = EditorGUILayout.BeginScrollView(scroll);
        GUIStyle areaStyle = new GUIStyle(GUI.skin.textArea)
        {
            richText = true
        };
        if (currentCleaner >= 0)
        {
            areaStyle.normal.background = new Texture2D(0, 0);
            GUILayout.TextArea(cleaners[currentCleaner].Preview, areaStyle, GUILayout.ExpandHeight(true));
        }
        else
            input = GUILayout.TextArea(input, areaStyle, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        // Cleaner interface.
        if (currentCleaner >= 0)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.BeginHorizontal();
            GUIStyle moduleTitle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Italic
            };
            GUILayout.Label(cleaners[currentCleaner].GetType().ToTitle(), moduleTitle);
            if (GUILayout.Button("X", GUILayout.MaxWidth(40)) || Event.current.keyCode == KeyCode.Escape)
                currentCleaner = -1;
            GUILayout.EndHorizontal();
            if (currentCleaner >= 0)
            {
                cleaners[currentCleaner].DrawUI();
                if (GUILayout.Button("Clean"))
                {
                    input = cleaners[currentCleaner].Clean();
                    currentCleaner = -1;
                }
            }
            GUILayout.EndVertical();
        }

        EditorGUILayout.BeginHorizontal();

        // File drop area.
        Rect openFileArea = DrawDropArea<TextAsset>("  Drop file here", (asset) =>
        {
            // Import the text and exit the current cleaner.
            input = asset.text;
            currentCleaner = -1;
            scroll = Vector2.zero;
        });
        GUI.DrawTexture(new Rect(openFileArea.x + openFileArea.width * .5f - 53, openFileArea.position.y + 8, 15, 15), EditorGUIUtility.IconContent("Folder Icon").image);

        // Buttons.
        if (GUILayout.Button("Copy", GUILayout.Height(27)))
            GUIUtility.systemCopyBuffer = input;
        if (GUILayout.Button("Clear", GUILayout.Height(27)))
        {
            Undo.RegisterCompleteObjectUndo(this, "Clear Code cleaner input");
            input = "";
        }
        EditorGUILayout.EndHorizontal();

        // Context menu.
        if (Event.current.button == 1)
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < cleaners.Count; i++)
            {
                int index = i;
                menu.AddItem(new GUIContent(cleaners[i].GetType().ToTitle()), false, () =>
                {
                    currentCleaner = index;
                    cleaners[currentCleaner].Start(input);
                });
            }

            menu.AddSeparator("");
            if (currentCleaner >= 0)
                menu.AddDisabledItem(new GUIContent("Reset example text"));
            else
                menu.AddItem(new GUIContent("Run all cleaners"), false, () =>
                {
                    foreach (CleanerModule cleaner in cleaners)
                        input = cleaner.Clean(input);
                });
            menu.ShowAsContext();

            Event.current.Use();
        }
    }


    [MenuItem("Window/Tools/Code cleaner")]
    public static void Open()
    {
        GetWindow(typeof(CodeCleaner)).titleContent = new GUIContent("Code cleaner");
    }

    Rect DrawDropArea<T>(string boxText, System.Action<T> ObjectAction)
    {
        Event evt = Event.current;
        Rect boxRect = GUILayoutUtility.GetRect(50, 30, GUILayout.ExpandWidth(true));
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter
        };
        if (EditorGUIUtility.isProSkin)
            boxStyle.normal.textColor = Color.white;
        GUI.Box(boxRect, boxText, boxStyle);

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!boxRect.Contains(evt.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object droppedObject in DragAndDrop.objectReferences)
                        if (droppedObject is T target)
                            ObjectAction(target);
                }
                break;
        }

        return boxRect;
    }
}
