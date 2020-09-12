    void Save()
    {
        BuildingLoop(builder, filepath);
    }

    static void BuildingLoop(CSVBuilder builder, string filepath)
    {
        if (!builder.Write(filepath, true))
            MessageBox.Show("Idea could not be saved. Maybe the file is open somewhere.", new MessageBoxCallback[]{
                new MessageBoxCallback(MessageBoxCallbackType.Cancel),
                new MessageBoxCallback(MessageBoxCallbackType.Retry, ()=>BuildingLoop(builder, filepath))
            });
        else
            Debug.Log("Wrote idea to " + filepath);
}