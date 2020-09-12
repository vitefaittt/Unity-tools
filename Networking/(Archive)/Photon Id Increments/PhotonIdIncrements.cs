using Photon.Pun;
using System;
using UnityEditor;
using UnityEngine;

public class PhotonIdIncrements : EditorWindow
{
    PhotonIdIncrementsData dataHolder;
    PhotonIdIncrementsData Data
    {
        get
        {
            if (dataHolder == null)
            {
                PhotonIdIncrementsData testDataHolder = AssetDatabase.LoadAssetAtPath(dataPath, typeof(PhotonIdIncrementsData)) as PhotonIdIncrementsData;
                dataHolder = AssetDatabase.LoadAssetAtPath(dataPath, typeof(PhotonIdIncrementsData)) as PhotonIdIncrementsData;
                if (dataHolder == null)
                {
                    dataHolder = CreateInstance(typeof(PhotonIdIncrementsData)) as PhotonIdIncrementsData;
                    AssetDatabase.CreateAsset(dataHolder, dataPath);
                    GUI.changed = true;
                }
            }
            return dataHolder;
        }
    }
    readonly string dataPath = "Assets/Editor/Photon Id Increments/PhotonIdIncrements data.asset";
    PhotonView[] views;
    Vector2 scrollPosition;

    enum SortingType { ViewID, SceneHierarchy }
    SortingType sorting;


    private void OnGUI()
    {
        if (GUILayout.Button("Scan"))
        {
            views = FindObjectsOfType<PhotonView>();
            SortViews();
            SpecialViewIDObjects[] specialViewIDObjects = FindObjectsOfType<SpecialViewIDObjects>();
            for (int i = 0; i < specialViewIDObjects.Length; i++)
                if (Data.minSpecialViewIdIndex > specialViewIDObjects[i].startIDIndex || Data.minSpecialViewIdIndex < 0)
                    Data.minSpecialViewIdIndex = 500;
            Save();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Saved count: " + Data.currentCount.ToString());
        if (GUILayout.Button("✖", GUILayout.Width(20)))
            Data.currentCount = 0;
        GUILayout.EndHorizontal();

        if (Data.minSpecialViewIdIndex >= 0)
            GUILayout.Label("Special start index: " + Data.minSpecialViewIdIndex);

        SortingType newSorting = (SortingType)EditorGUILayout.EnumPopup("Sorting: ", sorting);
        if (newSorting != sorting)
        {
            sorting = newSorting;
            SortViews();
        }

        if (views != null)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < views.Length; i++)
            {
                GUILayout.BeginHorizontal();
                if (views[i].ViewID >= Data.minSpecialViewIdIndex)
                    GUI.enabled = false;
                UnityEngine.Object property = views[i];
                property = EditorGUILayout.ObjectField(property, typeof(UnityEngine.Object), true);
                GUILayout.Label(views[i].ViewID.ToString(), GUILayout.Width(30));
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        if (GUILayout.Button("Increment"))
        {
            for (int i = 0; i < views.Length; i++)
            {
                if (views[i].ViewID < Data.minSpecialViewIdIndex)
                {
                    views[i].ViewID = ++Data.currentCount;
                    EditorUtility.SetDirty(views[i]);
                }
            }
            Save();
        }
    }


    [MenuItem("Window/Tools/Photon Id Increments")]
    public static void Open()
    {
        EditorWindow window = GetWindow(typeof(PhotonIdIncrements));
        window.titleContent = new GUIContent("Photon Id Increments");
    }

    void Save()
    {
        EditorUtility.SetDirty(Data);
        AssetDatabase.SaveAssets();
    }

    void SortViews()
    {
        switch (sorting)
        {
            case SortingType.ViewID:
                Array.Sort(views, (a, b) => a.ViewID - b.ViewID);
                break;
            case SortingType.SceneHierarchy:
                Array.Sort(views, (a, b) => String.Compare(a.gameObject.name, b.gameObject.name));
                break;
            default:
                break;
        }
    }


    int CompareInHierarchy(Transform a, Transform b)
    {
        // Compare the root.
        if (a.root != b.root)
            return a.root.GetSiblingIndex() - b.root.GetSiblingIndex();

        // Compare the parent.
        if (a.parent == b.parent)
            return a.GetSiblingIndex() - b.GetSiblingIndex();

        // Is a and b the same thing?
        if (a == b)
            return 0;

        // They are in the same hierarchy, go backwards until we find a common parent.
        int aCountOfParents = GetCountOfParents(a);
        int bCountOfParents = GetCountOfParents(b);
        Transform closestChildToTheRoot;
        if (aCountOfParents > bCountOfParents)
            closestChildToTheRoot = GetClosestChildToTheRoot(a, aCountOfParents, b, bCountOfParents);
        else
            closestChildToTheRoot = GetClosestChildToTheRoot(b, bCountOfParents, a, aCountOfParents);
        if (closestChildToTheRoot == a)
            return -1;
        else
            return 1;
    }

    Transform GetClosestChildToTheRoot(Transform deepestChild, int deepestCountOfParents, Transform lessDeepChild, int lessDeepCountOfParents)
    {
        Transform deepestPreviousParent = deepestChild;
        Transform lessDeepPreviousParent = lessDeepChild;
        for (int i = 0; i < deepestCountOfParents; i++)
            for (int j = 0; j < lessDeepCountOfParents; j++)
            {
                Transform deepParent = GetParent(deepestChild, i);
                Transform lessDeepParent = GetParent(lessDeepChild, j);

                if (deepParent == lessDeepParent)
                    return deepestPreviousParent.GetSiblingIndex()<lessDeepPreviousParent.GetSiblingIndex() ? deepestChild : lessDeepChild;

                deepestPreviousParent = deepParent;
                lessDeepPreviousParent = deepestPreviousParent;
            }
        return null;
    }

    int GetCountOfParents(Transform child)
    {
        Transform backwardsParent = child.parent;
        int count = 0;
        while (backwardsParent)
        {
            backwardsParent = backwardsParent.parent;
            count++;
        }
        return count;
    }

    Transform GetParent(Transform child, int parentIndex)
    {
        Transform parent = child.parent;
        for (int i = 0; i < parentIndex; i++)
            parent = child.parent;
        return parent;
    }
}
