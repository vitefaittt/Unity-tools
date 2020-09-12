using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissingScriptsRemover
{
    [MenuItem("Window/Tools/Missing Scripts/Remove")]
    static void RemoveMissingScriptInSelection()
    {
        if (Selection.objects.Length < 1)
        {
            Debug.Log("No object selected.");
            return;
        }
        int removalCount = 0;
        foreach (var @object in Selection.gameObjects)
        {
            if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(@object) >= 1)
                Undo.RegisterCompleteObjectUndo(@object, "Remove Missing Scripts On " + @object.name);
            removalCount += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(@object);
        }
        Debug.Log("Removed " + removalCount + " missing script(s).");
    }

}

public class MissingScriptsFinderWindow: EditorWindow
{
    public List<GameObject> searchResult = new List<GameObject>();

    void FindGameObjects()
    {
        // Get the current scene and all top-level GameObjects in the scene hierarchy
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        searchResult.Clear();
        foreach (GameObject g in rootObjects)
        {
            // Get all components on the GameObject, then loop through them 
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component currentComponent = components[i];

                // If the component is null, that means it's a missing script!
                if (currentComponent == null)
                {
                    // Add the sinner to our naughty-list
                    searchResult.Add(g);
                    Selection.activeGameObject = g;
                    Debug.Log(g + " has a missing script.");
                    break;
                }
            }
        }

        // Set the selection in the editor
        if (searchResult.Count > 0)
            Selection.objects = searchResult.ToArray();
        else
            Debug.Log("No GameObject in '" + currentScene.name + "' has missing scripts.");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Find"))
            FindGameObjects();

        GUILayout.BeginHorizontal();
        for (int i = 0; i < searchResult.Count; i++)
        {
            searchResult[i]= (GameObject)EditorGUILayout.ObjectField(searchResult[i], typeof(GameObject), true);
            if(GUILayout.Button("↗", GUILayout.Width(18)))
                Selection.activeGameObject = searchResult[i];
        }
        GUILayout.EndHorizontal();
    }


    [MenuItem("Window/Tools/Missing Scripts/Find")]
    static void Open()
    {
        EditorWindow window= GetWindow(typeof(MissingScriptsFinderWindow));
        window.titleContent = new GUIContent("Find Missing Scripts");
    }
}
