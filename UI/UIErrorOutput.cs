using UnityEngine;
using UnityEngine.UI;

public class UIErrorOutput : MonoBehaviour
{
    Text text;


    private void Reset()
    {
        text = this.GetOrAddComponent<Text>();
        text.fontStyle = FontStyle.Italic;
        text.text = "Error output";
        text.color = Color.red;
        this.RenameFromType(false);
    }

    private void Start()
    {
        text = GetComponent<Text>();
        Clear();
    }

    private void OnDisable()
    {
        Clear();
    }


    public void Log(string input)
    {
        text.text = input;
    }

    public void Clear()
    {
        text.text = "";
    }
}
