using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class TGAToPNG
{
    [MenuItem("Assets/TGA to PNG/in directory", false, 900)]
    static void ToPNGInCurrentDirectory()
    {
        ToPNGInDirectory(SavingUtilities.GetCurrentFolderPath());
    }

    static void ToPNGInDirectory(string directory)
    {
        // Get all .tga textures.
        string[] texturePaths = SavingUtilities.GetAssetsPathsInFolder(AssetFilter.texture2D, directory);
        List<string> tgaTexturesPaths = new List<string>();
        foreach (string texturePath in texturePaths)
            if (Path.GetExtension(texturePath).Equals(".tga", StringComparison.InvariantCultureIgnoreCase))
                tgaTexturesPaths.Add(texturePath);

        // Save all .tga textures to .png textures.
        List<Texture2D> tgaTextures = SavingUtilities.LoadAssetsInFolder<Texture2D>(tgaTexturesPaths.ToArray());
        for (int i = 0; i < tgaTextures.Count; i++)
            File.WriteAllBytes(directory + "/" + Path.GetFileName(tgaTexturesPaths[i]) + ".png",ImageConversion.EncodeToPNG(tgaTextures[i]));
    }
}
