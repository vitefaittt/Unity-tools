using System.IO;
using System.Text;

public class CSVBuilder
{
    StringBuilder builder = new StringBuilder();
    static char separator = ',';
    public string Content => builder.ToString();

    public CSVBuilder(params object[] items)
    {
        Append(items);
    }

    public void Append(params object[] items) => builder.Append(ItemsToLine(items));
    public void AppendLine(params object[] items) => builder.AppendLine(ItemsToLine(items));
    public void AppendSeparator() => builder.Append(separator);

    static string ItemsToLine(params object[] items)
    {
        string line = "";
        if (items.Length > 0)
            line += items[0];
        if (items.Length > 1)
            for (int i = 1; i < items.Length; i++)
                line += separator + items[i]?.ToString();
        return line;
    }

    public string WriteToFile(string filepath)
    {
        filepath = SavingUtilities.GetSafePath(filepath);
        File.WriteAllText(filepath, Content, Encoding.UTF8);
        return filepath;
    }

    public void AppendToFile(string filepath)
    {
        FileInfo file = new FileInfo(filepath);
        if (SavingUtilities.IsFileLocked(file))
        {
            MessageBox.Show("\"" + file.Name + "\" could not be saved. Maybe it is opened somewhere.", new MessageBoxCallback[]{
                new MessageBoxCallback(MessageBoxCallbackType.Cancel),
                new MessageBoxCallback(MessageBoxCallbackType.Retry, ()=>AppendToFile(filepath)) });
            return;
        }
        using (StreamWriter sw = new StreamWriter(filepath, true))
            sw.WriteLine(Content, Encoding.UTF8);
    }
}
