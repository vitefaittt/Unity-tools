using System.Collections.Generic;
using UnityEngine;

public class ObjectsPermute : MonoBehaviour
{
    public List<GameObject> listA = new List<GameObject>();
    public List<GameObject> listB = new List<GameObject>();
    public bool useListA = true;


    void Reset()
    {
        this.RenameFromType();
    }


    public void Permute()
    {
        useListA = !useListA;
        UpdateStates();
    }

    public void UpdateStates()
    {
        for (int i = 0; i < listA.Count; i++)
            if (!listA[i])
                listA.RemoveAt(i);
            else
                listA[i].SetActive(useListA);
        for (int i = 0; i < listB.Count; i++)
            if (!listB[i])
                listB.RemoveAt(i);
            else
                listB[i].SetActive(!useListA);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ObjectsPermute)), UnityEditor.CanEditMultipleObjects]
class PermuteObjectsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        ObjectsPermute script = (ObjectsPermute)target;

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.MaxHeight(50));
        GUILayout.Label("List A", UnityEditor.EditorStyles.boldLabel);
        GUIStyle listItemStyle = new GUIStyle(GUI.skin.label);
        listItemStyle.fixedWidth = UnityEditor.EditorGUIUtility.currentViewWidth * .45f;
        foreach (GameObject listItem in script.listA)
        {
            listItemStyle.normal.textColor = listItem && listItem.activeInHierarchy ? Color.black : Color.grey;
            GUILayout.Label(listItem ? listItem.name : null, listItemStyle);
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUILayout.MaxHeight(50));
        GUILayout.Label("List B", UnityEditor.EditorStyles.boldLabel);
        foreach (GameObject listItem in script.listB)
        {
            listItemStyle.normal.textColor = listItem && listItem.activeInHierarchy ? Color.black : Color.grey;
            GUILayout.Label(listItem ? listItem.name : null, listItemStyle);
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Permute"))
            script.Permute();
        if (GUILayout.Button("Update states"))
            script.UpdateStates();
    }
}
#endif

