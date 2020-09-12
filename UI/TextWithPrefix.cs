using UnityEngine.UI;

public class TextWithPrefix
{
    public string Prefix { get; private set; }
    Text text;
    public string FullText => text.text;
    public string Text {
        get {
            try { return text.text.Substring(Prefix.Length); }
            catch { return ""; };
        }
    }

    public TextWithPrefix(Text text, bool emptyText = false)
    {
        this.text = text;
        Prefix = text.text;
        if (emptyText)
            this.text.text = "";
    }

    public void SetText(string text)
    {
        this.text.text = Prefix + text;
    }

    public void AddText(string text)
    {
        if (string.IsNullOrEmpty(this.text.text))
            this.text.text = Prefix;
        this.text.text += text;
    }
}
