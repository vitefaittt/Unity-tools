using System;
using UnityEditor;
using UnityEngine;

public class AutoMaterialSettings : ScriptableObject {
    /// <summary>
    /// Where the settings file is by default.
    /// </summary>
    public static string defaultFolderPath = "Assets/Editor/Auto Material";

    /// <summary>
    /// Texture suffixes and their corresponding material field.
    /// </summary>
    public SuffixGroup[] suffixGroups = new SuffixGroup[]
    {
        new SuffixGroup("_MainTex", "_AlbedoTransparency"),
        new SuffixGroup("_MetallicGlossMap", "_MetallicSmoothness"),
        new SuffixGroup("_OcclusionMap", "_AO"),
        new SuffixGroup("_BumpMap", "_Normal"),
        new SuffixGroup("_EmissionMap", "_Emission"),
    };

    /// <summary>
    /// Create a new settings file with default suffixes.
    /// </summary>
    /// <returns>The new created suffixes file</returns>
    public static AutoMaterialSettings Create()
    {
        AutoMaterialSettings newSuffixes = (AutoMaterialSettings)CreateInstance(typeof(AutoMaterialSettings).Name);
        AssetDatabase.CreateAsset(newSuffixes, AssetDatabase.GenerateUniqueAssetPath(defaultFolderPath + "/Auto Material Settings.asset"));
        return newSuffixes;
    }

    public Material emissiveMatTemplate;
    public bool outputResults = true;
}

[Serializable]
public class SuffixGroup {
    public string materialKeyword = "";
    public string[] textureSuffixes;
    public SuffixGroup(string matKey, string textSufx)
    {
        materialKeyword = matKey;
        textureSuffixes = new string[] { textSufx };
    }
}
