using UnityEngine;

public class ToggleActive : MonoBehaviour
{
    [SerializeField]
    public bool toggleThis = true;
    [SerializeField]
    bool toggleOnAwake = true;

    [Header("Objects to toggle")]
    [Tooltip("The gameObjects that will be toggled.")]
    [SerializeField]
    GameObject[] objects;
    [Tooltip("If true, all objects will have the same state after the Toggle.")]
    [SerializeField]
    bool @override = false;
    bool othersState;


    private void Awake()
    {
        if (objects.Length > 0)
            othersState = objects[0].activeSelf;
        if (toggleOnAwake)
            Toggle();
    }
    

    public void Toggle()
    {
        if (toggleThis)
            gameObject.SetActive(!gameObject.activeSelf);
        else if (objects.Length > 0)
        {
            othersState = !othersState;
            foreach (GameObject other in objects)
                other.SetActive(@override ? othersState : !other.activeSelf);
        }
    }


}
#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(ToggleActive))]
public class ToggleActiveEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        ToggleActive myToggleActive = target as ToggleActive;
        System.Collections.Generic.List<string> excludedProperties = new System.Collections.Generic.List<string>();

        if (myToggleActive.toggleThis)
        {
            excludedProperties.Add("objects");
            excludedProperties.Add("override");
        }

        DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
        serializedObject.ApplyModifiedProperties();
    }
}
#endif