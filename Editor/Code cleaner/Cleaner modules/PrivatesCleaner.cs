using System.Collections.Generic;
using UnityEngine;

public class PrivatesCleaner : CleanerModule
{
    List<int> privates = new List<int>();
    string input;


    public override void Start(string input)
    {
        privates = RegexUtilities.GetMatchesIndexes(input, @"private \w+ \w+");
        this.input = input;

        // Get preview.
        Preview = input.Highlight(privates, p => p, p => p + "private ".Length, "red");
    }

    public override string Clean()
    {
        for (int i = privates.Count - 1; i >= 0; i--)
            input = input.Remove(privates[i], "private ".Length);
        return input;
    }

    public override void DrawUI()
    {
        GUILayout.Label(privates.Count + " private(s) found.");
    }
}
