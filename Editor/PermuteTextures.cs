using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PermuteTextures
{
    #region Menu items.
    [MenuItem("Assets/Tools/Permute Textures/use .pngs")]
    static void PermuteToPNGs()
    {
        PermuteTexturesOnSelectionObjects(Selection.objects, ".png", SavingUtilities.GetCurrentFolderPath());
    }
    [MenuItem("Assets/Tools/Permute Textures/use .tgas")]
    static void PermuteToTGAsOnCurrentMaterial()
    {
        PermuteTexturesOnSelectionObjects(Selection.objects, ".tga", SavingUtilities.GetCurrentFolderPath());
    }

    [MenuItem("Assets/Tools/Permute Textures/use .pngs on current material", true)]
    static bool PermuteToPNGsOnCurrentMaterialValidation() { return ValidationFromSelection(); }
    [MenuItem("Assets/Tools/Permute Textures/use .tgas on current material", true)]
    static bool PermuteToTGAsOnCurrentMaterialValidation() { return ValidationFromSelection(); }
    static bool ValidationFromSelection()
    {
        for (int i = 0; i < Selection.objects.Length; i++)
            if (Selection.objects[i].GetType() != typeof(Material))
                return false;
        return true;
    }
    #endregion

    static void PermuteTexturesOnSelectionObjects(Object[] materials, string extension, string folderPath)
    {
        // Get all textures in current directory.
        string[] texturePaths = SavingUtilities.GetAssetsPathsInFolder(AssetFilter.texture2D, folderPath);
        for (int i = 0; i < materials.Length; i++)
            PermuteTexturesOnMaterialWithPaths((Material)materials[i], extension, folderPath, texturePaths);
    }

    static void PermuteTexturesOnMaterialWithPaths(Material material, string extension, string folderPath, string[] texturePaths)
    {
        // Find all textures on the material and try to replace them.
        Shader shader = material.shader;
        List<string> permutedTextures = new List<string>();
        for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
        {
            if (ShaderUtil.GetPropertyType(shader, i) != ShaderUtil.ShaderPropertyType.TexEnv)
                continue;
            string texturePropertyName = ShaderUtil.GetPropertyName(shader, i);
            Texture texture = material.GetTexture(texturePropertyName);
            if (!texture)
                continue;
            foreach (var texturePath in texturePaths)
                if (Path.GetFileNameWithoutExtension(texturePath) == texture.name && Path.GetExtension(texturePath).Equals(extension, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    material.SetTexture(texturePropertyName, (Texture)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture)));
                    permutedTextures.Add(Path.GetFileName(texturePath));
                }
        }

        // Log permuted textures.
        if (permutedTextures.Count < 1)
        {
            Debug.Log("No textures found to permute...");
            return;
        }
        string log = "["+material.name + "] Permuted " + permutedTextures.Count + ":";
        foreach (var permutedTexture in permutedTextures)
            log += " [" + permutedTexture + "]";
        Debug.Log(log + ".");
    }
}
