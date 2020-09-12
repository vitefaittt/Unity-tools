using UnityEditor;
using UnityEngine;

public class ShowApplicationDataPaths
{
#if UNITY_EDITOR
    [MenuItem("Help/Show Data Paths/Application.datapath")]
    public static void ShowDataPath()
    {
        Debug.Log("Application.dataPath: " + Application.dataPath);
    }

    [MenuItem("Help/Show Data Paths/Application.persistentDatapath")]
    public static void ShowPersistentDataPath()
    {
        Debug.Log("Application.persistentDataPath: " + Application.persistentDataPath);
    }
#endif
}
