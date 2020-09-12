using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class SavingUtilities
{
    public static string GetSafePath(string filePath)
    {
        FileInfo targetFile = new FileInfo(filePath);
        FileInfo file = new FileInfo(filePath);
        int fileIndex = 0;
        while (file.Exists)
        {
            fileIndex++;
            file = new FileInfo(targetFile.DirectoryName + "\\" + targetFile.Name.Insert(targetFile.Name.Length - targetFile.Extension.Length, " (" + fileIndex + ")"));
        }
        return file.ToString();
    }

    public static string GetSafeFileName(string directory, string desiredFileName)
    {
        FileInfo file = new FileInfo(directory + desiredFileName);
        int fileIndex = 0;
        while (file.Exists)
        {
            fileIndex++;
            file = new FileInfo(directory + desiredFileName.Insert(desiredFileName.Length - file.Extension.Length, " (" + fileIndex + ")"));
        }
        return file.Name;
    }

    #region Indent ----------
    /// <summary>
    /// Add an amount of tabulations to the beginning of the string.
    /// </summary>
    /// <param name="s">String to indent. If negative, will unindent.</param>
    /// <param name="amount">Amount of tabulations to add.</param>
    /// <returns></returns>
    public static string Indent(this string s, int amount = 1)
    {
        if (amount < 0)
            return s.Unindent(-amount);
        string indents = "";
        for (int i = 0; i < amount; i++)
            indents += "\t";
        return indents + s.Replace("\n", "\n" + indents);
    }

    /// <summary>
    /// Removes an amount of tabulations (or all) to the beginning of the string.
    /// </summary>
    /// <param name="s">String to unindent. If positive, will indent.</param>
    /// <param name="amount">Amount of tabulations to remove.</param>
    /// <param name="full">Remove all tabulations ?</param>
    /// <returns></returns>
    public static string Unindent(this string s, int amount = 1)
    {
        if (amount == 0)
            return s;
        else if (amount < 0)
            return s.Indent(-amount);
        for (int i = 0; i < amount; i++)
            s = s.Replace("\n\t", "\n");
        return s;
    }

    public static string UnindentFull(this string s)
    {
        // Remove all tabs and spaces after a carriage return.
        return Regex.Replace(s, @"\n(\t| )+", "\n");
    }
    #endregion -----------------

    #region Quick save -------
    /// <summary>
    /// Save to a file with a safe name (name will be incremented (1)) to the current directory or to a custom directory.
    /// </summary>
    /// <returns>The path of the created file.</returns>
    public static string QuickSave(List<string> lines, string fileName, string directory = "")
    {
        string filePath = GetSafePath((directory == "" ? (Directory.GetCurrentDirectory() + "\\") : directory) + fileName);
        StreamWriter writer = File.CreateText(filePath);
        foreach (string row in lines)
            writer.WriteLine(row);
        writer.Close();
        return filePath;
    }

    /// <summary>
    /// Save to a file with a safe name (name will be incremented (1)) to the current directory or to a custom directory.
    /// </summary>
    /// <returns>The path of the created file.</returns>
    public static string QuickSave(string[] lines, string fileName, string directory = "")
    {
        return QuickSave(lines.ToList(), fileName);
    }

    /// <summary>
    /// Save to a file with a safe name (name will be incremented (1)) to the current directory or to a custom directory.
    /// </summary>
    /// <returns>The path of the created file.</returns>
    public static string QuickSave(string text, string fileName, string directory = "")
    {
        string filePath = GetSafePath((directory == "" ? (Directory.GetCurrentDirectory() + "\\") : directory) + fileName);
        StreamWriter writer = File.CreateText(filePath);
        writer.Write(text);
        writer.Close();
        return filePath;
    }
    #endregion --------------

    /// <summary>
    /// Save to a file with a safe name (name will be incremented (1)) to the current directory or to a custom directory.
    /// </summary>
    /// <param name="fileName">Name of the texture, without an extension</param>
    public static string QuickSaveToJPG(this Texture2D inTex, string textureName, string directory = "")
    {
        string filePath = GetSafePath((directory == "" ? (Directory.GetCurrentDirectory() + "\\") : directory) + textureName + ".jpg");
        Texture2D texToEncode = new Texture2D(inTex.width, inTex.height);
        texToEncode.SetPixels(inTex.GetPixels());
        byte[] bytes = texToEncode.EncodeToJPG();
        Object.DestroyImmediate(texToEncode);
        File.WriteAllBytes(filePath, bytes);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        return filePath;
    }

    public static string GetCurrentFolderPath()
    {
        // Get the selected folder in the Unity project.
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
            path = "Assets";
        else if (Path.GetExtension(path) != "")
            path = Path.GetDirectoryName(path);
        return path;
#endif
        return Application.dataPath;
    }

    public static List<T> LoadAssetsAtPaths<T>(string[] assetPaths) where T : Object
    {
#if UNITY_EDITOR
        List<T> loadedAssets = new List<T>();
        foreach (string assetPath in assetPaths)
        {
            T assetAtPath = (T)AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
            loadedAssets.Add(assetAtPath);
        }
        return loadedAssets;
#endif
        return new List<T>();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Get all asset paths in folder using a filter.
    /// </summary>
    /// <param name="filter">For example: "t:texture2D".</param>
    /// <param name="folderPath"></param>
    /// <returns></returns>
    public static string[] GetAssetsPathsInFolder(string filter, string folderPath)
    {
        string[] gUIDs = AssetDatabase.FindAssets(filter, new[] { folderPath });
        string[] assetPaths = new string[gUIDs.Length];
        for (int i = 0; i < gUIDs.Length; i++)
            assetPaths[i] = AssetDatabase.GUIDToAssetPath(gUIDs[i]);
        return assetPaths;
    }
#endif

    public static bool IsFileLocked(FileInfo file)
    {
        try
        {
            using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                stream.Close();
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }

        //file is not locked
        return false;
    }
}

public static class AssetFilter
{
    public static readonly string texture2D = "t:texture2D";
}