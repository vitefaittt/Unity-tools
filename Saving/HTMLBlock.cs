using UnityEngine;

public class HTMLBlock : MonoBehaviour
{
    public string content = "";
    bool contentWasAdded;

    /// <summary>
    /// Add a new line to the content.
    /// </summary>
    /// <param name="line"></param>
    public void AppendLine(string line)
    {
        if (!contentWasAdded)
            content = line;
        else
            content += "\n" + line;
        contentWasAdded = true;
    }

    /// <summary>
    /// Indent our content except from the first and last line.
    /// </summary>
    public void IndentInnerContent(int amount = 1)
    {
        if (content.Length < 1)
            return;
        // Get first and last carriage returns.
        int firstReturn = 0, lastReturn = 0;
        for (int i = 1; i < content.Length; i++)
            if (content[i] == '\n')
                firstReturn = i;
        for (int i = content.Length - 1; i >= 0; i--)
            if (content[i] == '\n')
                lastReturn = i;
        if (firstReturn == lastReturn || lastReturn < firstReturn)
            return;
        
        string firstLine = content.Substring(0, firstReturn);
        string innerContent = content.Substring(firstReturn, lastReturn - 1 - firstReturn).Indent(amount);
        string lastLine = content.Substring(lastReturn, content.Length - lastReturn);
        content = firstLine + innerContent + lastLine;
    }
}