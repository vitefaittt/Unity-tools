public abstract class CleanerModule
{
    public string Preview { get; protected set; }

    public abstract void Start(string input);
    public abstract string Clean();
    public string Clean(string input)
    {
        Start(input);
        return Clean();
    }
    public abstract void DrawUI();
}
