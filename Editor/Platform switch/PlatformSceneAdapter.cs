using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformSceneAdapter : MonoBehaviour
{
    public PlatformData.Platform CurrentPlatform { get; private set; }

    // Gameobjects that we want to link to a platform.
    [System.Serializable]
    struct PlatformObjects
    {
        [SerializeField]
        PlatformData.Platform platform;
        public PlatformData.Platform Platform => platform;
        [SerializeField]
        GameObject[] gameObjects;
        [SerializeField]
        public GameObject[] GameObjects => gameObjects;

        public PlatformObjects(PlatformData.Platform platform, params GameObject[] gameObjects)
        {
            this.platform = platform;
            this.gameObjects = gameObjects;
        }
    }
    [SerializeField]
    PlatformObjects[] platformObjects;


    void Reset()
    {
        this.RenameFromType();
        List<PlatformObjects> defaultObjects = new List<PlatformObjects>();
        Utilities.ForeachEnumValue((PlatformData.Platform platform) => defaultObjects.Add(new PlatformObjects(platform)));
        platformObjects = defaultObjects.ToArray();
    }

    void Start()
    {
        SetPlatform(PlatformData.GetDataObject().platform);
    }

    void Update()
    {
        // Update our platform objects when the platform changes.
        if (CurrentPlatform != PlatformData.GetDataObject().platform)
            SetPlatform(PlatformData.GetDataObject().platform);
    }


    void SetPlatform(PlatformData.Platform platform)
    {
        // Set gameobjects active depending on the input platform.
        CurrentPlatform = platform;
        if (platformObjects.Length < 1)
            return;
        foreach (var platformObject in platformObjects)
            if (platformObject.GameObjects.Length > 0)
                foreach (var gameObject in platformObject.GameObjects)
                    gameObject.SetActive(platformObject.Platform == CurrentPlatform);
    }

    public void UpdatePlatformObjects()
    {
        SetPlatform(CurrentPlatform);
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(PlatformSceneAdapter)), UnityEditor.CanEditMultipleObjects]
class PlatformSceneAdapterEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlatformSceneAdapter script = (PlatformSceneAdapter)target;
        GUILayout.Space(5);

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.alignment = TextAnchor.UpperRight;
        labelStyle.fontStyle = FontStyle.Italic;
        GUILayout.Label("Current platform: " + script.CurrentPlatform, labelStyle);

        if (GUILayout.Button("Update platform objects"))
            script.UpdatePlatformObjects();
    }
}
#endif