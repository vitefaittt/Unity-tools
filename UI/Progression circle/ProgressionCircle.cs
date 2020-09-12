using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressionCircle : MonoBehaviour
{
    Image savedImage;
    public Image Image
    {
        get
        {
            if (!savedImage)
                savedImage = GetComponentInChildren<Image>();
            return savedImage;
        }
    }
    [Range(0, 1)]
    public float progression;


    private void Reset()
    {
        this.RenameFromType();
    }

    private void Update()
    {
        Image.fillAmount = progression;
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ProgressionCircle)), UnityEditor.CanEditMultipleObjects]
class ProgressionCircleEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        ProgressionCircle script = (ProgressionCircle)target;

        GUI.enabled = false;
        UnityEditor.EditorGUILayout.ObjectField("Image ", script.Image, typeof(Image), true);
        GUI.enabled = true;
    }
}
#endif
