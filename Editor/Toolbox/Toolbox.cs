using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class Toolbox : EditorWindow
{
    static ToolboxData dataHolder;
    public static ToolboxData Data => DataObject.Get(ref dataHolder, "/Editor/Toolbox");
    public const string ComponentsPath = "/Editor/Toolbox/Components";
    Dictionary<int, EditorWindow> componentInstances = new Dictionary<int, EditorWindow>();

    void Awake()
    {
        Data.shouldReimport = true;
    }

    void OnGUI()
    {
        // Reimport components if we need to.
        if (Data.shouldReimport && !EditorApplication.isCompiling)
        {
            GetComponents();
            Data.shouldReimport = false;
            Data.Save();
        }

        // No components, draw message and stop.
        if (Data.components.Count < 1)
        {
            GUILayout.Label("No components to show");
            return;
        }

        // Draw the components buttons.
        DrawComponentsButtons();

        // Draw the panel of the first component.
        MethodInfo onGUIMethod = Type.GetType(Data.components[Data.index]).GetMethod("OnGUI", BindingFlags.NonPublic | BindingFlags.Instance);
        onGUIMethod.Invoke(GetComponentInstance(Data.index), null);
    }


    [MenuItem("Window/Tools/Toolbox")]
    public static void Open()
    {
        GetWindow(typeof(Toolbox));
    }

    void DrawComponentsButtons()
    {
        // Draw a button for each component.
        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"), GUILayout.Width(position.width));
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        for (int i = 0; i < Data.components.Count; i++)
        {
            if (Data.index == i)
                GUI.enabled = false;
            string buttonTitle = GetComponentInstance(i).titleContent.text;
            if (buttonTitle.Length > 1 && buttonTitle[1] == ' ')
            {
                buttonTitle = buttonTitle[0].ToString();
                buttonStyle.fixedWidth = 30;
            }
            else
                buttonStyle.fixedWidth = -1;
            if (GUILayout.Button(new GUIContent( buttonTitle, GetComponentInstance(i).titleContent.text), buttonStyle))
            {
                Data.index = Mathf.Clamp(i, 0, Data.components.Count - 1);
                Data.Save();
            }
            GUI.enabled = true;
        }
        GUILayout.EndHorizontal();
    }

    void GetComponents()
    {
        Data.components.Clear();
        Data.index = 0;

        // Get all EditorWindow types in the components folder.
        string[] files = GetFiles(Application.dataPath + ComponentsPath).ToArray();
        Regex regex = new Regex(@"class \w+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        for (int i = 0; i < files.Length; i++)
        {
            if (Path.GetExtension(files[i]) != ".cs")
                continue;
            MatchCollection matches = regex.Matches(File.ReadAllText(files[i]));
            foreach (Match match in matches)
            {
                string component = match.Groups[0].Value.Substring(6);
                Type componentType = Type.GetType(component);
                if (componentType.IsSubclassOf(typeof(EditorWindow)) && componentType != typeof(Toolbox))
                    Data.components.Add(component);
            }
        }
        Data.Save();
    }

    EditorWindow GetComponentInstance(int index)
    {
        if (!componentInstances.ContainsKey(index))
            componentInstances.Add(index, (EditorWindow)CreateInstance(Data.components[index]));
        return componentInstances[index];
    }

    static IEnumerable<string> GetFiles(string path)
    {
        Queue<string> queue = new Queue<string>();
        queue.Enqueue(path);
        while (queue.Count > 0)
        {
            path = queue.Dequeue();
            foreach (string subDir in Directory.GetDirectories(path))
                queue.Enqueue(subDir);
            string[] files = Directory.GetFiles(path);
            if (files != null)
                for (int i = 0; i < files.Length; i++)
                    yield return files[i];
        }
    }
}

class ToolboxPostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // Tell the toolbox's data to reimport components if something changed in its components folder.
        for (int i = 0; i < importedAssets.Length; i++)
            if (Regex.IsMatch(importedAssets[i], Toolbox.ComponentsPath))
            {
                Toolbox.Data.shouldReimport = true;
                return;
            }
        for (int i = 0; i < deletedAssets.Length; i++)
            if (Regex.IsMatch(deletedAssets[i], Toolbox.ComponentsPath))
            {
                Toolbox.Data.shouldReimport = true;
                return;
            }
        for (int i = 0; i < movedAssets.Length; i++)
            if (Regex.IsMatch(movedAssets[i], Toolbox.ComponentsPath))
            {
                Toolbox.Data.shouldReimport = true;
                return;
            }
        for (int i = 0; i < movedFromAssetPaths.Length; i++)
            if (Regex.IsMatch(movedFromAssetPaths[i], Toolbox.ComponentsPath))
            {
                Toolbox.Data.shouldReimport = true;
                return;
            }
    }
}