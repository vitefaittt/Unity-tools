using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(DomainPanel))]
public class RichTextTagsWithRegex : MonoBehaviour
{
    DomainPanel saved_panel;
    DomainPanel Panel
    {
        get
        {
            if (!saved_panel)
                saved_panel = GetComponent<DomainPanel>();
            return saved_panel;
        }
    }
    Text Content { get { return Panel.content; } }
    [SerializeField]
    string[] styleTags = new string[0];


    private void Start()
    {
        // Apply our styling when the panel opens.
        Panel.Opened += StyleContentTitles;
    }

    private void Update()
    {
        // Style the titles live when we are selected in the editor.
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;
        if (UnityEditor.Selection.activeGameObject == gameObject)
            StyleContentTitles();
#endif
    }


    public void StyleContentTitles()
    {
        List<KeyValuePair<string, string>> titlesAndContents = GetCleanTitlesAndContents();
        Content.text = "";
        // Style each title and add the new line to the content.
        foreach (KeyValuePair<string, string> titleAndContent in titlesAndContents)
        {
            string title = titleAndContent.Key;
            string content = titleAndContent.Value;
            // Add the style tag to the title.
            foreach (string styleTag in styleTags)
            {
                if (string.IsNullOrEmpty(styleTag))
                    continue;
                string endStyleTag = styleTag.Split('=')[0];
                title = '<' + styleTag + '>' + title + "</" + endStyleTag + '>';
            }
            // Add the new line.
            Content.text += title + content + '\n';
        }
    }

    List<KeyValuePair<string, string>> GetCleanTitlesAndContents()
    {
        List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
        string[] lines = Content.text.Split('\n');
        foreach (string line in lines)
        {
            // Get the title and the content of the line.
            string title = "";
            string content = "";
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ':')
                {
                    title = line.Substring(0, i + 1);
                    // Clean the title from tags.
                    title = Regex.Replace(title, "<((?!>).)*>", "");
                    content = line.Substring(i + 1, line.Length - i -1);
                    i = line.Length;
                }
            }
            result.Add(new KeyValuePair<string, string>(title, content));
        }
        return result;
    }

    public void Clean()
    {
        List<KeyValuePair<string, string>> titlesAndContents = GetCleanTitlesAndContents();
        Content.text = "";
        foreach (KeyValuePair<string, string> titleAndContent in titlesAndContents)
            Content.text += titleAndContent.Key + titleAndContent.Value + '\n';
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(DomainPanelContentTitles))]
class DomainPanelContentTitlesEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DomainPanelContentTitles script = (DomainPanelContentTitles)target;

        GUILayout.Space(5);

        if (GUILayout.Button("Clean"))
            script.Clean();
    }
}
#endif