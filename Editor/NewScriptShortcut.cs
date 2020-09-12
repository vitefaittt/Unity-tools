using System.IO;
using UnityEditor;
using UnityEngine;

public class NewScriptShortcut : EditorWindow
{
    readonly string scriptfield = "scriptName";
    readonly string scriptTemplate = @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class # : MonoBehaviour 
{
	
}
";
    public string inputText;
    public string directory;

    bool IsPressingReturn { get { return Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter; } }


    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUI.SetNextControlName(scriptfield);
        inputText = EditorGUILayout.TextField(inputText);
        GUI.FocusControl("");
        GUI.FocusControl(scriptfield);
        GUILayout.Label(".cs", GUILayout.Width(20));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Cancel"))
            Close();
        if (GUILayout.Button("Create") || IsPressingReturn && !string.IsNullOrWhiteSpace(inputText))
        {
            string path = directory + "\\" + inputText + ".cs";
            File.WriteAllText(path, scriptTemplate.Replace("#", inputText));
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);
            Close();
        }
        GUILayout.EndHorizontal();
    }


    [MenuItem("Assets/Tools/New C# Script %&n")]
    static void NewScriptDialog()
    {
        NewScriptShortcut window = new NewScriptShortcut();
        window.titleContent = new GUIContent("New C# Script");
        window.ShowUtility();
        window.minSize = window.maxSize = new Vector2(240, 50);
        string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(selectionPath))
            selectionPath = "Assets";
        window.directory = Directory.Exists(selectionPath) ? selectionPath : Path.GetDirectoryName(selectionPath);
        window.inputText = Path.GetFileName(window.directory);
    }
}
