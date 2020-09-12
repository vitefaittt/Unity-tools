static Stream stream;


void OnDestroy()
{
    if (Data != null)
        Save();

    if (stream != null)
    {
        stream.Close();
        stream.Dispose();
        stream = null;
    }
}

public static void Save()
{
    if (Data.items.Count < 1)
    {
        File.Delete(dataPath);
        File.WriteAllText(dataPath, "");
        return;
    }

    if (stream == null)
        stream = File.Open(dataPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

    stream.Seek(0, SeekOrigin.Begin);
    int previousDataCharacterCount = 0;
    using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, true, 64, true))
    {
        string previousData = streamReader.ReadToEnd();
        int lastCharIndex = 0;
        for (int i = previousData.Length - 1; i >= 0; i--)
            if (previousData[i] != ' ')
            {
                lastCharIndex = i;
                i = -1;
            }
        previousDataCharacterCount = lastCharIndex + 1;
    }

    stream.Seek(0, SeekOrigin.Begin);
    using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8, 64, true))
    {
        string jsonData = JsonUtility.ToJson(Data);
        int charactersToDelete = previousDataCharacterCount - jsonData.Length;
        streamWriter.Write(jsonData);
        for (int i = 0; i < charactersToDelete; i++)
            streamWriter.Write(" ");
    }

    AssetDatabase.Refresh();
}