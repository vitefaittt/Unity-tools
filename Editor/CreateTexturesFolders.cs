using System.IO;
using UnityEditor;
using UnityEngine;

public static class CreateTexturesFolders
{
    #region Creating the folders.
    [MenuItem("Assets/Tools/Create textures folders/in directory")]
    static void CreateFolderInCurrentDirectory()
    {
        // Try to create a Textures folder in the current directory.
        if (!CreateFolderInDirectory(SavingUtilities.GetCurrentFolderPath()))
            Debug.Log("No textures in directory.");
    }

    [MenuItem("Assets/Tools/Create textures folders/in children directories")]
    static void CreateFoldersInCurrentChildrenDirectories()
    {
        // Create Textures folders under all first-child directories.
        string[] directories = Directory.GetDirectories(SavingUtilities.GetCurrentFolderPath());
        foreach (var directory in directories)
            CreateFolderInDirectory(directory);
    }

    static bool CreateFolderInDirectory(string directory)
    {
        // Get all texture paths in current folder. Stop if none are found.
        string[] texturePaths = SavingUtilities.GetAssetsPathsInFolder(AssetFilter.texture2D, SavingUtilities.GetCurrentFolderPath());
        if (texturePaths.Length < 0)
            return false;

        // Move all textures to a new (or existing) Textures folder.
        string newTexturesDirectory = directory + "/Textures/";
        if (!Directory.Exists(newTexturesDirectory))
        {
            Directory.CreateDirectory(newTexturesDirectory);
            AssetDatabase.Refresh();
        }
        foreach (var texturePath in texturePaths)
            AssetDatabase.MoveAsset(texturePath, newTexturesDirectory + Path.GetFileName(texturePath));
        return true;
    }
    #endregion

    #region Undoing the folders creation.
    [MenuItem("Assets/Tools/Create textures folders/undo in directory")]
    static void UndoFolderInCurrentDirectory()
    {
        // Try to undo the Textures folder in the current directory.
        if (!UndoFolderInDirectory(SavingUtilities.GetCurrentFolderPath()))
            Debug.Log("No Textures folder found to undo.");
    }

    [MenuItem("Assets/Tools/Create textures folders/undo in children directories")]
    static void UndoFolderInCurrentChildrenDirectories()
    {
        // Remove all Textures folders under first-child directories.
        string[] directories = Directory.GetDirectories(SavingUtilities.GetCurrentFolderPath());
        foreach (var directory in directories)
            UndoFolderInDirectory(directory);
    }

    static bool UndoFolderInDirectory(string directory)
    {
        // Get the Textures folder. Stop if there isn't one.
        string texturesDirectory = directory + "/Textures";
        if (!Directory.Exists(texturesDirectory))
            return false;

        // Move all textures out of the Textures folder.
        string[] texturePaths = SavingUtilities.GetAssetsPathsInFolder(AssetFilter.texture2D, SavingUtilities.GetCurrentFolderPath());
        foreach (var texturePath in texturePaths)
            AssetDatabase.MoveAsset(texturePath, directory + "/" + Path.GetFileName(texturePath));

        // Delete the Textures folder if it is now empty.
        if (Directory.GetFiles(texturesDirectory).Length < 1)
            Directory.Delete(texturesDirectory);
        AssetDatabase.Refresh();
        return true;
    }
    #endregion
}
