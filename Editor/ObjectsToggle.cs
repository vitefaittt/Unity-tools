using System.Collections.Generic;
using UnityEngine;

public class ObjectsToggle : MonoBehaviour
{
    public bool on;
    public List<GameObject> listOff = new List<GameObject>();
    public List<GameObject> listOn = new List<GameObject>();


    void Reset()
    {
        this.RenameFromType();
    }


    public void Toggle()
    {
        on = !on;
        UpdateActiveStates();
    }

    public void UpdateActiveStates()
    {
        for (int i = 0; i < listOff.Count; i++)
            if (!listOff[i])
                listOff.RemoveAt(i);
            else
                listOff[i].SetActive(!on);
        for (int i = 0; i < listOn.Count; i++)
            if (!listOn[i])
                listOn.RemoveAt(i);
            else
                listOn[i].SetActive(on);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ObjectsToggle)), UnityEditor.CanEditMultipleObjects]
class PermuteObjectsEditor : UnityEditor.Editor
{
    Vector2 scrollPosOn, scrollPosOff;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        ObjectsToggle script = (ObjectsToggle)target;


        float listWidth = UnityEditor.EditorGUIUtility.currentViewWidth * .45f;
        GUIStyle listItemStyle = new GUIStyle(GUI.skin.label);

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("List Off", UnityEditor.EditorStyles.boldLabel);
        scrollPosOff = GUILayout.BeginScrollView(scrollPosOff, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.MaxHeight(150));
        foreach (GameObject listItem in script.listOff)
            GUILayout.Label(listItem ? listItem.name : "null", GetItemStyle(listItem, listItemStyle));
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("List On", UnityEditor.EditorStyles.boldLabel);
        scrollPosOn = GUILayout.BeginScrollView(scrollPosOn, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.MaxHeight(150));
        foreach (GameObject listItem in script.listOn)
            GUILayout.Label(listItem ? listItem.name : "null", GetItemStyle(listItem, listItemStyle));
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Toggle"))
            script.Toggle();
        if (GUILayout.Button("Update active states"))
            script.UpdateActiveStates();
    }


    GUIStyle GetItemStyle(GameObject item, GUIStyle style)
    {
        if (!item || !item.activeInHierarchy)
            style.normal.textColor = Color.grey;
        else
            style.normal.textColor = Color.black;
        style.fontStyle = item ? FontStyle.Normal : FontStyle.Italic;
        return style;
    }
}
#endif

