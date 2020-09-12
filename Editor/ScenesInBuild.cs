using System.IO;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesInBuild : EditorWindow
{
    string previousScene;
    string currentScene;
    string nextScene;
    string CurrentScenePath => SceneManager.GetActiveScene().path;
    Vector2 scrollPosition;


    void Awake()
    {
        titleContent = new GUIContent("✂ Scenes in Build");
    }

    void OnGUI()
    {
        bool displayedAScene = false;

        if (currentScene != CurrentScenePath)
            UpdateCurrentScene();
        DrawPreviousNext();

        // Draw scene buttons.
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GUIStyle sceneButtonStyle = new GUIStyle(GUI.skin.button);
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
        {
            if (!File.Exists(Application.dataPath.Remove(Application.dataPath.Length - "Assets/".Length) + "/" + SceneUtility.GetScenePathByBuildIndex(i)))
                continue;

            GUILayout.BeginHorizontal();
            sceneButtonStyle.fontStyle = SceneUtility.GetBuildIndexByScenePath(SceneUtility.GetScenePathByBuildIndex(i)) < 0 ? FontStyle.Italic : FontStyle.Normal;
            if (SceneUtility.GetScenePathByBuildIndex(i) == CurrentScenePath)
                GUI.enabled = false;
            if (GUILayout.Button(Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)), sceneButtonStyle) && CanSwitchScenes())
                EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(i));
            GUI.enabled = true;
            GUILayout.Space(-4);
            if (GUILayout.Button("↗", GUILayout.Width(18)))
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(SceneUtility.GetScenePathByBuildIndex(i), typeof(Object)));
                i--;
            }
            GUILayout.EndHorizontal();
            displayedAScene = true;
        }
        EditorGUILayout.EndScrollView();

        if (!displayedAScene)
            GUILayout.Label("No scenes to display.");
    }


    bool CanSwitchScenes()
    {
        if (SceneManager.GetActiveScene().isDirty)
            switch (EditorUtility.DisplayDialogComplex("Scene Has Been Modified",
                @"Do you want to save the changes you made in the current scene?

Your changes will be lost if you don't save them.", "Save", "Cancel", "Don't Save"))
            {
                case 0:
                    string[] path = SceneManager.GetActiveScene().path.Split(char.Parse("/"));
                    bool saveOK = EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), string.Join("/", path));
                    break;
                case 1:
                    return false;
            }
        return true;
    }

    [MenuItem("Window/Tools/Scenes in Build")]
    public static void Open()
    {
        GetWindow(typeof(ScenesInBuild));
    }

    void UpdateCurrentScene()
    {
        previousScene = currentScene;
        currentScene = CurrentScenePath;
    }

    void DrawPreviousNext()
    {
        bool canPrevious = !string.IsNullOrEmpty(previousScene) && CurrentScenePath != previousScene;
        bool canNext = !string.IsNullOrEmpty(nextScene) && CurrentScenePath != nextScene && nextScene != previousScene;

        GUILayout.BeginHorizontal();
        if (canPrevious)
            if (GUILayout.Button("← " + Path.GetFileNameWithoutExtension(previousScene)) && CanSwitchScenes())
            {
                nextScene = CurrentScenePath;
                EditorSceneManager.OpenScene(previousScene);
            }
        if (canNext)
            if (GUILayout.Button(Path.GetFileNameWithoutExtension(nextScene) + " →") && CanSwitchScenes())
                EditorSceneManager.OpenScene(nextScene);
        GUILayout.Space(position.width * .25f);
        GUILayout.EndHorizontal();
    }
}
