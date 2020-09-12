using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class VRRadialMenuEvents : MonoBehaviour
{
    [SerializeField]
    List<UnityEvent> selectionEvents = new List<UnityEvent>();


    void Reset()
    {
        selectionEvents = new UnityEvent[transform.childCount].ToList();
    }

    void Start()
    {
        if (!Application.isPlaying)
            return;
        GetComponent<VRRadialMenu>().Selected += delegate (int selection)
        {
            selectionEvents[selection].Invoke();
        };
    }


    public void UpdateEventsCount()
    {
#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Update events count");
#endif
        selectionEvents = new UnityEvent[transform.childCount].ToList();
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(VRRadialMenuEvents))]
class VRRadialMenuEventsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        VRRadialMenuEvents script = (VRRadialMenuEvents)target;
        GUILayout.Space(5);

        if (GUILayout.Button("Update events count"))
            script.UpdateEventsCount();
    }
}
#endif