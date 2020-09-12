using ToDoMini;
using UnityEditor;
using UnityEngine;

public class ToDoMiniToToDo : EditorWindow
{
    string content;

    ToDoMiniData dataHolder;
    ToDoMiniData Data {
        get {
            if (dataHolder == null)
            {
                ToDoMiniData testDataHolder = AssetDatabase.LoadAssetAtPath(dataPath, typeof(ToDoMiniData)) as ToDoMiniData;
                dataHolder = AssetDatabase.LoadAssetAtPath(dataPath, typeof(ToDoMiniData)) as ToDoMiniData;
                if (dataHolder == null)
                {
                    dataHolder = CreateInstance(typeof(ToDoMiniData)) as ToDoMiniData;
                    AssetDatabase.CreateAsset(dataHolder, dataPath);
                    GUI.changed = true;
                }
            }
            return dataHolder;
        }
    }
    readonly string dataPath = "Assets/Editor/ToDo Mini/ToDoMini data.asset";
    Vector2 scroll;


    void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        content = EditorGUILayout.TextArea(content, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent("Get data 9-1 ✔", "This is the proper way to get Data. AutoHotKey will add items in reverse order."), GUILayout.Height(30)))
        {
            content = ParseTodoDataReversed();
            GUI.FocusControl(null);
        }
        if (GUILayout.Button(new GUIContent("Get data 1-9", "This is an alternative way to get Data. This should not be used with the provided AutoHotKey script, else items will be added in reverse order."), GUILayout.Width(90), GUILayout.Height(30)))
        {
            content = ParseTodoData();
            Repaint();
            GUI.FocusControl(null);
        }
        if (GUILayout.Button("Copy data", GUILayout.Height(30)))
            GUIUtility.systemCopyBuffer = content;
        GUILayout.EndHorizontal();
    }


    string ParseTodoDataReversed()
    {
        string result = "";
        for (int i = Data.items.Count - 1; i >= 0; i--)
            if (!Data.items[i].isComplete)
                result += Data.items[i].task + "\n";
        return result;
    }

    string ParseTodoData()
    {
        string result = "";
        for (int i = 0; i < Data.items.Count; i++)
            if (!Data.items[i].isComplete)
                result += Data.items[i].task + "\n";
        return result;
    }


    [MenuItem("Window/Tools/ToDoMini to ToDo")]
    static void Open()
    {
        GetWindow(typeof(ToDoMiniToToDo)).titleContent = new GUIContent("ToDoMini To ToDo");
    }
}
