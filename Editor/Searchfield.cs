using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class SomeClass : EditorWindow
{
    string currentSearch;
    SearchField searchField;


    public void OnGUI()
    {
        // Searchbar.
        Rect searchRect = GUILayoutUtility.GetRect(1, 1, 18, 18, GUILayout.ExpandWidth(true));
        searchRect.y += 2;
        if (searchField == null)
            searchField = new SearchField();
        currentSearch = searchField.OnToolbarGUI(searchRect, currentSearch);
    }
}