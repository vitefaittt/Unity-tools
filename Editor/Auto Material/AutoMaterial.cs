using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AutoMaterial {

    const string DefaultMenuPath = "Assets/Tools/Auto Material/";

    // A group of textures with same name and same directory.
    struct TextureGroup {
        public Dictionary<string, Texture2D> textures;
        public string Name { get; private set; }
        public string Path { get; private set; }
        public TextureGroup(Dictionary<string, Texture2D> _textures, string _name, string _path)
        {
            textures = _textures;
            Name = _name;
            Path = _path;
        }
    }

    [MenuItem(DefaultMenuPath + "Materials from textures", false)]
    static void MaterialsFromTextures()
    {
        // Get all textures in the selected folder.
        string currentFolderPath = GetCurrentFolderPath();

        // Check if there are any textures.
        List<Texture2D> texturesInFolder = GetTexturesAtPath(currentFolderPath);
        if (texturesInFolder.Count < 1)
        {
            EditorUtility.DisplayDialog("Error", "No textures were found.", "Ok");
            return;
        }
        // Get groups of textures with fitting suffixes, stop if none was found.
        List<TextureGroup> textureGroups = GetTextureGroups(ref texturesInFolder);
        if (texturesInFolder.Count < 1)
        {
            EditorUtility.DisplayDialog("Error", "No textures with suffixes were found.", "Ok");
            return;
        }
        // Prompt continue if we found more than one texture group.
        if (textureGroups.Count > 1)
        {
            bool canContinue = PromptContinueWithResults(textureGroups);
            if (!canContinue)
                return;
        }

        // Go into material generation.
        int editedMatsCount = GenerateMaterialsFromGroups(textureGroups);

        // If multiple materials were edited, log results.
        if (textureGroups.Count > 1)
            Log("[AutoMaterial] Edited " + editedMatsCount + " materials.");
    }

    [MenuItem(DefaultMenuPath + "Single material from texture", false)]
    static void MaterialFromTexture()
    {
        string filePath = AssetDatabase.GetAssetPath(Selection.activeObject);
        Texture2D loadedTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(filePath, typeof(Texture2D));
        if (loadedTexture == null)
        {
            Log("No texture selected.");
            return;
        }
        Material newMat = new Material(Shader.Find("Standard"));
        newMat.name = loadedTexture.name;
        newMat.SetTexture("_MainTex", loadedTexture);
        AssetDatabase.CreateAsset(newMat, AssetDatabase.GenerateUniqueAssetPath(Path.GetDirectoryName(filePath) + "\\" + loadedTexture.name + ".mat"));
    }

    [MenuItem(DefaultMenuPath + "☸ Settings")]
    static void SelectSuffixesFile()
    {
        Selection.activeObject = GetSettingsFile();
    }

    #region Utilities ---------------
    private static int GenerateMaterialsFromGroups(List<TextureGroup> textureGroups)
    {
        int editedMatsCount = 0;
        // Create the materials with their textures.
        foreach (TextureGroup group in textureGroups)
        {
            // Don't create the material if it already exists.
            string newMatPath = group.Path + "/" + group.Name + ".mat";
            if (!File.Exists(newMatPath))
            {
                CreateMaterial(group, newMatPath, textureGroups.Count == 1);
                editedMatsCount++;
                continue;
            }
            // The material already exists. Override? Complete? Ignore?
            Material alreadyExistingMat = (Material)AssetDatabase.LoadAssetAtPath(newMatPath, typeof(Material));
            int overridingChoice = EditorUtility.DisplayDialogComplex(
                "Material already exists",
                Path.GetDirectoryName(newMatPath) + "/\n" +
                group.Name + ".mat already exists." + "\n",
                "Override material", "Ignore", "Complete material");
            switch (overridingChoice)
            {
                case 0:
                    // Override material.
                    AssetDatabase.DeleteAsset(newMatPath);
                    CreateMaterial(group, newMatPath, textureGroups.Count == 1);
                    editedMatsCount++;
                    break;
                case 2:
                    // Complete material.
                    CompleteMaterial(alreadyExistingMat, group, textureGroups.Count == 1);
                    editedMatsCount++;
                    break;
                default:
                    // Ignore.
                    break;
            }
        }
        return editedMatsCount;
    }

    static void CreateMaterial(TextureGroup group, string path, bool setSelected)
    {
        // Create the material and fill it with textures from the group.
        Material newMat;
        // Check if we need and emissive material.
        bool needsEmission = false;
        foreach (KeyValuePair<string, Texture2D> texture in group.textures)
            if (texture.Key == "_EmissionMap")
                needsEmission = true;
        if (!needsEmission)
            newMat = new Material(Shader.Find("Standard"));
        else
            newMat = new Material(GetSettingsFile().emissiveMatTemplate);
        newMat.name = group.Name;
        // Assign textures and create asset.
        foreach (KeyValuePair<string, Texture2D> texture in group.textures)
            newMat.SetTexture(texture.Key, texture.Value);
        AssetDatabase.CreateAsset(newMat, AssetDatabase.GenerateUniqueAssetPath(path));
        if (setSelected)
            Selection.activeObject = newMat;
    }

    private static List<Texture2D> GetTexturesAtPath(string folderPath)
    {
        // Get all the textures in the specified folder path.
        List<Texture2D> texturesInFolder = new List<Texture2D>();
        string[] texturesGUIDs = AssetDatabase.FindAssets("t:texture2D", new[] { folderPath });
        foreach (string textureGUID in texturesGUIDs)
        {
            string texturePath = AssetDatabase.GUIDToAssetPath(textureGUID);
            Texture2D textureAtPath = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));
            texturesInFolder.Add(textureAtPath);
        }
        return texturesInFolder;
    }

    private static List<TextureGroup> GetTextureGroups(ref List<Texture2D> texturesInFolder)
    {
        // Get groups from texturesInFolder, using textures name & path.
        List<TextureGroup> textureGroups = new List<TextureGroup>();
        AutoMaterialSettings suffixesFile = GetSettingsFile();
        foreach (Texture2D textureInFolder in texturesInFolder)
        {
            foreach (SuffixGroup knownSuffix in suffixesFile.suffixGroups)
            {
                // See if the texture has any known suffix.
                string resultSuffix = "";
                foreach (string databaseSuffix in knownSuffix.textureSuffixes)
                {
                    if (textureInFolder.name.HasSuffix(databaseSuffix))
                        resultSuffix = databaseSuffix;
                }
                // The texture has a known suffix, store it in a group.
                if (resultSuffix != "")
                {
                    string textureName = textureInFolder.name.NameWithoutSuffix(resultSuffix);
                    string textureFolderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(textureInFolder));
                    bool wasStored = false;
                    foreach (TextureGroup texGroup in textureGroups)
                    {
                        if (texGroup.Name == textureName && texGroup.Path == textureFolderPath)
                        {
                            if (!texGroup.textures.ContainsKey(knownSuffix.materialKeyword)) // (quick fix) In case there is two textures with the same name.
                                texGroup.textures.Add(knownSuffix.materialKeyword, textureInFolder);
                            wasStored = true;
                        }
                    }
                    if (!wasStored)
                    {
                        TextureGroup newGroup = new TextureGroup(new Dictionary<string, Texture2D>() { { knownSuffix.materialKeyword, textureInFolder } }, textureName, textureFolderPath);
                        textureGroups.Add(newGroup);
                    }
                }
            }
        }
        return textureGroups;
    }

    private static bool PromptContinueWithResults(List<TextureGroup> textureGroups)
    {
        // Show a message with our results.
        int texturesCount = 0;
        foreach (TextureGroup group in textureGroups)
            texturesCount += group.textures.Count;
        bool ok = EditorUtility.DisplayDialog(
            "Search result",
            "Found " + texturesCount + " textures with correct suffixes, will try to create " + textureGroups.Count + " materials.",
            "Continue",
            "Cancel");
        return ok;
    }

    static void CompleteMaterial(Material mat, TextureGroup group, bool setSelected)
    {
        // If we have an emission texture, create a new material.
        foreach (KeyValuePair<string, Texture2D> texture in group.textures)
            if (texture.Key == "_EmissionMap" && mat.GetTexture(texture.Key) == null)
            {
                Log(mat.name + " was overriden to set an emission texture.");
                string pathToOverride = group.Path + "\\" + mat.name + ".mat";
                AssetDatabase.DeleteAsset(pathToOverride);
                CreateMaterial(group, pathToOverride, setSelected);
                return;
            }

        // Assign textures where the material has none.
        foreach (KeyValuePair<string, Texture2D> texture in group.textures)
        {
            if (mat.GetTexture(texture.Key) == null)
                mat.SetTexture(texture.Key, texture.Value);
        }
    }

    static bool HasSuffix(this string name, string suffix)
    {
        if (name.Length < suffix.Length)
            return false;
        return name.Substring(name.Length - suffix.Length, suffix.Length) == suffix;
    }

    static string NameWithoutSuffix(this string name, string suffix)
    {
        if (name.Length < suffix.Length)
            return "";
        else
            return name.Substring(0, name.Length - suffix.Length);
    }

    private static AutoMaterialSettings GetSettingsFile()
    {
        // Return the existing suffixes file or create a new one.
        AutoMaterialSettings settingsFile;
        string[] textureSuffixesGUIDs = AssetDatabase.FindAssets("t:" + typeof(AutoMaterialSettings).Name, new[] { AutoMaterialSettings.defaultFolderPath });
        if (textureSuffixesGUIDs.Length > 0)
            settingsFile = (AutoMaterialSettings)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(textureSuffixesGUIDs[0]), typeof(AutoMaterialSettings));
        else
            settingsFile = AutoMaterialSettings.Create();
        return settingsFile;
    }

    static string GetCurrentFolderPath()
    {
        // Get the selected folder in the Unity project.
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
            path = "Assets";
        else if (Path.GetExtension(path) != "")
            path = Path.GetDirectoryName(path);
        return path;
    }

    static void Log(object message)
    {
        if (GetSettingsFile().outputResults)
            Debug.Log("[AutoMaterial] " + message);
    }

    #region Auto assign materials tests ----------------------------
    //[MenuItem(rClickPath + "/Auto assign materials")]
    static void AutoAssignMaterials()
    {
        string currentFolder = GetCurrentFolderPath();

        // Get models and materials in folder.
        //List<Renderer> rendsInFolder = GetRenderersAtPath(currentFolder);
        List<Material> matsInFolder = GetObjectsAtPath<Material>(currentFolder, "Material");
        //List<GameObject> objectsInFolder = GetObjectsAtPath<GameObject>(currentFolder, "Model");

        Log(matsInFolder.Count);
        foreach (Material mat in matsInFolder)
            Log(mat.name);

        /*
        for (int i = 0; i < objectsInFolder.Count; i++)
        {
            Material matToBeAssigned = matsInFolder[1 + i * 2];
            objectsInFolder[i].GetComponent<Renderer>().material = matToBeAssigned;
            Log(matToBeAssigned.name);
        }*/
        /*
        for (int i = 0; i < rendsInFolder.Count; i++)
        {
            rendsInFolder[i].material = matsInFolder[1 + i*2];
            Log(matsInFolder[1 + i * 1]);
        }*/
    }

    private static List<T> GetObjectsAtPath<T>(string currentFolder, string filterKeyword) where T : Object
    {
        List<T> objectsInFolder = new List<T>();
        string[] objectsGUIDs = AssetDatabase.FindAssets("t:" + filterKeyword, new[] { currentFolder });
        foreach (string objectGUID in objectsGUIDs)
        {
            string matPath = AssetDatabase.GUIDToAssetPath(objectGUID);
            T objectAtPath = (T)AssetDatabase.LoadAssetAtPath(matPath, typeof(T));
            objectsInFolder.Add(objectAtPath);
        }
        return objectsInFolder;
    }

    private static List<Material> GetMaterialsAtPath(string currentFolder)
    {
        List<Material> matsInFolder = new List<Material>();
        string[] matsGUIDs = AssetDatabase.FindAssets("t:Material", new[] { currentFolder });
        foreach (string matGUID in matsGUIDs)
        {
            string matPath = AssetDatabase.GUIDToAssetPath(matGUID);
            Material matAtPath = (Material)AssetDatabase.LoadAssetAtPath(matPath, typeof(Material));
            matsInFolder.Add(matAtPath);
        }
        return matsInFolder;
    }

    private static List<Renderer> GetRenderersAtPath(string currentFolder)
    {
        List<Renderer> modelsInFolder = new List<Renderer>();
        string[] modelGUIDs = AssetDatabase.FindAssets("t:Model", new[] { currentFolder });
        foreach (string modelGUID in modelGUIDs)
        {
            string modelPath = AssetDatabase.GUIDToAssetPath(modelGUID);
            GameObject modelAtPath = (GameObject)AssetDatabase.LoadAssetAtPath(modelPath, typeof(GameObject));
            if (modelAtPath.GetComponent<Renderer>() != null)
                modelsInFolder.Add(modelAtPath.GetComponent<Renderer>());
        }
        return modelsInFolder;
    }
    #endregion --------------------------------------------------------
    #endregion ------------------------------
}