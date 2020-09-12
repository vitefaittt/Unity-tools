using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SearchKeeper : EditorWindow
{
    SearchKeeperData dataHolder;
    SearchKeeperData Data => DataObject.Get(ref dataHolder, "/Editor/Search keeper");
    SearchableEditorWindow hierarchy;
    string field;


    private void Awake()
    {
        titleContent = new GUIContent("? Search keeper");
    }

    private void OnGUI()
    {
        // Search field.
        GUILayout.BeginHorizontal();
        field = GUILayout.TextField(field);
        if (GUILayout.Button("Search", GUILayout.Width(80)))
        {
            if (!Data.searches.Contains(field))
            {
                Data.searches.Add(field);
                Data.searches.Sort((a, b) => string.Compare(a, b));
                Data.Save();
            }
            SetSearchFilter(field, 0);
        }
        GUILayout.EndHorizontal();

        // Previous searches buttons.
        GUILayout.Space(5);
        for (int i = 0; i < Data.searches.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Data.searches[i]))
                SetSearchFilter(Data.searches[i], 0);
            if (GUILayout.Button("✖", GUILayout.Width(20)))
            {
                Data.searches.RemoveAt(i);
                Data.Save();
            }
            GUILayout.EndHorizontal();
        }
    }

    public void SetSearchFilter(string filter, int filterMode)
    {

        SearchableEditorWindow[] windows = (SearchableEditorWindow[])Resources.FindObjectsOfTypeAll(typeof(SearchableEditorWindow));

        foreach (SearchableEditorWindow window in windows)
            if (window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow")
            {
                hierarchy = window;
                break;
            }

        if (!hierarchy)
            return;

        MethodInfo setSearchType = typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
        object[] parameters = new object[] { filter, filterMode, true, true };

        setSearchType.Invoke(hierarchy, parameters);
    }

    [MenuItem("Window/Tools/Search keeper")]
    public static void Open()
    {
        GetWindow(typeof(SearchKeeper));
    }
}
