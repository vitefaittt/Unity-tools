using System;
using UnityEditor;
using UnityEngine;
using Valve.VR;

[ExecuteInEditMode]
public class PlatformData : ScriptableObject
{
    public enum Platform { Desktop, Android, SteamVR, Quest, WebVR }
    [HideInInspector]
    public Platform platform;

#if UNITY_EDITOR
    readonly static string platformDataPath = "Assets/Resources/Platform Data.asset";
    SteamVR_Settings SteamVR_Settings => AssetDatabase.LoadAssetAtPath("Assets/SteamVR_Resources/Resources/SteamVR_Settings.asset", typeof(SteamVR_Settings)) as SteamVR_Settings;
#endif

#if UNITY_EDITOR
    [MenuItem("Platform switch/Platform switch")]
    public static void ShowPlatformPromptWindow()
    {
        Selection.SetActiveObjectWithContext(GetDataObject(), null);
    }

    public void SetPlatform(Platform platform)
    {
        this.platform = platform;
        EditorUtility.SetDirty(this);

        // SteamVR.
        if (platform == Platform.SteamVR)
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        SteamVR_Settings.autoEnableVR = platform == Platform.SteamVR;

        // Quest.
        if (platform == Platform.Quest)
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        // Android.
        if (platform == Platform.Android)
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

        // VR.
        if (PlayerSettings.GetVirtualRealitySupported(BuildTargetGroup.Standalone) != (platform == Platform.SteamVR || platform == Platform.Quest))
            PlayerSettings.SetVirtualRealitySupported(BuildTargetGroup.Standalone, platform == Platform.SteamVR || platform == Platform.Quest);

        // Standalone.
        if (platform == Platform.Desktop)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
            Debug.Log("build platform should be on windows");
        }
    }
#endif

    public static PlatformData GetDataObject()
    {
#if UNITY_EDITOR
        PlatformData data = AssetDatabase.LoadAssetAtPath(platformDataPath, typeof(PlatformData)) as PlatformData;
        if (!data)
        {
            data = CreateInstance(typeof(PlatformData)) as PlatformData;
            AssetDatabase.CreateAsset(data, platformDataPath);
        }
        return data;
#endif
        return null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlatformData)), CanEditMultipleObjects]
class PlatformDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlatformData script = (PlatformData)target;
        GUILayout.Space(5);

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Set current platform", EditorStyles.boldLabel);
        GUIStyle selectedButtonStyle = new GUIStyle(GUI.skin.button);
        selectedButtonStyle.normal.textColor = Color.grey;
        PlatformData.Platform[] platforms = (PlatformData.Platform[])Enum.GetValues(typeof(PlatformData.Platform));
        foreach (var platform in platforms)
            if (platform == script.platform)
                GUILayout.Button(platform.ToString(), selectedButtonStyle);
            else if (GUILayout.Button(platform.ToString()))
                SetPlatform(script, platform);
    }


    void SetPlatform(PlatformData data, PlatformData.Platform platform)
    {
        if (EditorUtility.DisplayDialog("Confirmation", "Switch target platform to " + platform.ToString() + "?", "Yes", "Cancel"))
            data.SetPlatform(platform);
    }
}
#endif