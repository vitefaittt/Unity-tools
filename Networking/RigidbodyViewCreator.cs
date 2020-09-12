using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RigidbodyViewCreator : EditorWindow
{
    List<PhotonView> targetedViews = new List<PhotonView>();
    Vector2 scrollPosition;




    private void OnGUI()
    {
        // Find views that have a rigidbody component.
        if (GUILayout.Button("Find"))
        {
            targetedViews = FindObjectsOfType<PhotonView>().Where((view) => view.GetComponent<Rigidbody>() != null).ToList();
            targetedViews.Sort((a, b) => String.Compare(a.gameObject.name, b.gameObject.name));
        }

        // Add rigidbody views to the photon views.
        if (GUILayout.Button("Add"))
            for (int i = 0; i < targetedViews.Count; i++)
            {
                PhotonRigidbodyView rigidbodyView = targetedViews[i].GetOrAddComponent<PhotonRigidbodyView>();
                if (!targetedViews[i].ObservedComponents.Contains(rigidbodyView))
                {
                    Undo.RegisterCompleteObjectUndo(targetedViews[i], "Setup rigidbody view");
                    targetedViews[i].ObservedComponents.Add(rigidbodyView);
                }
            }

        // Draw the targeted views.
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < targetedViews.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (targetedViews[i] == null || GUILayout.Button("✖", GUILayout.Width(20)))
            {
                targetedViews.RemoveAt(i);
                GUILayout.EndHorizontal();
                continue;
            }
            UnityEngine.Object viewProperty = targetedViews[i];
            viewProperty = EditorGUILayout.ObjectField(viewProperty, typeof(PhotonView), true);
            GUILayout.Label(targetedViews[i].ViewID.ToString(), GUILayout.Width(50));
            if (GUILayout.Button("↗", GUILayout.Width(18)))
                Selection.activeGameObject = targetedViews[i].gameObject;
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }


    [MenuItem("Window/Tools/Rigidbody view creator")]
    public static void Open()
    {
        GetWindow(typeof(RigidbodyViewCreator)).titleContent = new GUIContent("Rigidbody view creator");
    }
}
