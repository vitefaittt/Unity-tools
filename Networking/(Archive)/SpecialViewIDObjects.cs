using Photon.Pun;
using UnityEngine;

public class SpecialViewIDObjects : MonoBehaviour
{
    public int startIDIndex = 500;
    public PhotonView[] objects;


    void Reset()
    {
        this.RenameFromType();
    }


    public void SetIDs()
    {
        int currentIndex = startIDIndex;
        foreach (var @object in objects)
        {
            @object.ViewID = currentIndex;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorUtility.SetDirty(@object);
#endif
            currentIndex++;
        }
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(SpecialViewIDObjects)), UnityEditor.CanEditMultipleObjects]
class SpecialViewIDObjectsEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        SpecialViewIDObjects script = (SpecialViewIDObjects)target;

        if (GUILayout.Button("Set IDs for objects"))
            script.SetIDs();

        foreach (var view in script.objects)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(view ? view.gameObject.name : "(null)", GUILayout.MaxWidth(UnityEditor.EditorGUIUtility.currentViewWidth * .368f));
            GUILayout.Label(view ? view.ViewID.ToString() : "(null)");
            GUILayout.EndHorizontal();
        }
    }
}
#endif
