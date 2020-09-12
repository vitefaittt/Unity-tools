using Scripts.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HierarchyNavigator
{
    static HierarchyNavigatorData Data => DataObject.Get<HierarchyNavigatorData>("Assets/Editor/Hierarchy Navigator/Hierarchy navigator data.asset");


    static HierarchyNavigator()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyItemOnGUI;
    }


    private static void HandleHierarchyItemOnGUI(int selectionID, Rect selectionRect)
    {
        if (Event.current.type != EventType.Repaint)
            return;

        GameObject gameObject = EditorUtility.InstanceIDToObject(selectionID) as GameObject;
        if (!gameObject)
            return;

        if (!Data.selectionIds.Contains(selectionID))
            return;


        Rect offset = new Rect(selectionRect.position + new Vector2(18, 0), selectionRect.size);

        EditorGUI.LabelField(offset, gameObject.name, new GUIStyle()
        {
            fontStyle = FontStyle.Bold
        });

        EditorApplication.RepaintHierarchyWindow();
    }

    [MenuItem("GameObject/Tools/Bold", false, 0)]
    public static void AddBoldItem()
    {
        if (Selection.activeGameObject)
        {
            Debug.Log("tring to add " + Selection.activeGameObject.GetInstanceID());
            Data.selectionIds.Add(Selection.activeGameObject.GetInstanceID());
            Data.Save();
        }
    }

    [MenuItem("GameObject/Tools/Unbold", false, 0)]
    static void RemoveBoldItem()
    {
        if (Selection.activeGameObject && Data.selectionIds.Contains(Selection.activeGameObject.GetInstanceID()))
        {
            Data.selectionIds.Remove(Selection.activeGameObject.GetInstanceID());
            Data.Save();
        }
    }
}


class HierarchyNavigatorData : DataObject
{
    public List<int> selectionIds = new List<int>();
}