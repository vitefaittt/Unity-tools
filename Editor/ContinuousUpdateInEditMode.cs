using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
class ContinuousUpdateInEditMode
{
    static ContinuousUpdateInEditMode()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
        Debug.Log("Updating");
    }
}