using System.Collections.Generic;
using UnityEngine;
using static RegexUtilities;

public class IfExpressionCleaner : CleanerModule
{
    string input;
    List<RegexMatch> expressions = new List<RegexMatch>();

    public override void Start(string input)
    {
        this.input = input;
        expressions.Clear();

        expressions.AddRange(GetMatches(input, @"(?<!\w)\w+ *== *true"));
        expressions.AddRange(GetMatches(input, @"(?<!\w)\w+ *== *false"));
        expressions.Sort((a, b) => a.index.CompareTo(b.index));

        // Get preview.
        Preview = input.Highlight(expressions, ex => ex.index, ex => ex.index + ex.value.Length, "red");
    }

    public override string Clean()
    {
        for (int i = expressions.Count - 1; i >= 0; i--)
        {
            RegexMatch variable = GetMatches(expressions[i].value, @"\w+")[0];
            input = input.Remove(expressions[i].index+variable.value.Length, expressions[i].value.Length - variable.value.Length);
            if (expressions[i].value.EndsWith("false"))
                input = input.Insert(expressions[i].index, "!");
        }
        return input;
    }

    public override void DrawUI()
    {
        GUILayout.Label(expressions.Count + " expression(s) found.");
    }
}
