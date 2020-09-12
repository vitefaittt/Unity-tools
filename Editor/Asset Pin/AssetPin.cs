using UnityEditor;
using UnityEngine;

public class AssetPin : EditorWindow
{
    static AssetPinData dataHolder;
    static AssetPinData Data => DataObject.Get(ref dataHolder, "/Editor/Asset Pin");


    void Awake()
    {
        titleContent = new GUIContent("↘ Pinned Assets");
    }

    void OnGUI()
    {
        GUILayout.Space(5);
        for (int i = 0; i < Data.pinnedAssets.Count; i++)
        {
            GUILayout.BeginHorizontal();
            Object property = Data.pinnedAssets[i];
            property = EditorGUILayout.ObjectField(property, typeof(Object), true);
            if (GUILayout.Button("↗", GUILayout.Width(18)))
            {
                EditorUtility.FocusProjectWindow();
                EditorGUIUtility.PingObject(Data.pinnedAssets[i]);
                i--;
            }
            if (GUILayout.Button("✖", GUILayout.Width(20)))
            {
                Data.pinnedAssets.RemoveAt(i);
                i--;
                Data.Save();
            }
            GUILayout.EndHorizontal();
        }
        if (Data.pinnedAssets.Count < 1)
            GUILayout.Label("No pinned assets.");

        // Drop area to pin assets.
        DrawDropArea<Object>("Drop an asset to pin here", Pin);
    }


    [MenuItem("Window/Tools/Asset Pin")]
    public static void Open()
    {
        GetWindow(typeof(AssetPin));
    }

    #region Pin.
    static void Pin(Object @object)
    {
        if (!Data.pinnedAssets.Contains(@object))
        {
            Data.pinnedAssets.Add(@object);
            Data.Save();
        }
    }

    [MenuItem("Assets/Tools/AssetPin/Pin")]
    public static void PinSelection()
    {
        Undo.RecordObject(Data, "Pin Assets");
        foreach (var selectedObject in Selection.objects)
            Pin(selectedObject);
        Data.Save();
        GetWindow(typeof(AssetPin));
    }

    [MenuItem("Assets/Tools/AssetPin/Pin", true)]
    static bool Pin_Validation()
    {
        foreach (var selectedObject in Selection.objects)
            if (!Data.pinnedAssets.Contains(selectedObject))
                return true;
        return false;
    }
    #endregion

    #region Unpin.
    [MenuItem("Assets/Tools/AssetPin/Unpin")]
    public static void UnpinSelection()
    {
        Undo.RecordObject(Data, "Unpin Assets");
        for (int i = 0; i < Selection.objects.Length; i++)
            if (Data.pinnedAssets.Contains(Selection.objects[i]))
                Data.pinnedAssets.Remove(Selection.objects[i]);
        Data.Save();
        GetWindow(typeof(AssetPin));
    }

    [MenuItem("Assets/Tools/AssetPin/Unpin", true)]
    static bool Unpin_Validation()
    {
        foreach (var selectedObject in Selection.objects)
            if (Data.pinnedAssets.Contains(selectedObject))
                return true;
        return false;
    }
    #endregion

    void DrawDropArea<T>(string boxText, System.Action<T> ObjectAction)
    {
        Event evt = Event.current;
        Rect boxRect = GUILayoutUtility.GetRect(50, 30, GUILayout.ExpandWidth(true));
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Box(boxRect, boxText, boxStyle);

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!boxRect.Contains(evt.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object droppedObject in DragAndDrop.objectReferences)
                        if (droppedObject is T target)
                            ObjectAction(target);
                }
                break;
        }
    }
}
