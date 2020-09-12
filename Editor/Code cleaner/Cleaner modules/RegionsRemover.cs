using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static RegexUtilities;

public class RegionsRemover : CleanerModule
{
    GroupFinder regionsFinder = new GroupFinder("#region.*", "#endregion");
    List<bool> groupsRemoveEnabledStates = new List<bool>();
    string input;


    public override void Start(string input)
    {
        regionsFinder.Find(input);
        groupsRemoveEnabledStates = Enumerable.Repeat(true, regionsFinder.groups.Count).ToList();
        this.input = input;

        // Get preview.
        List<RegexMatch> matches = regionsFinder.GetMatches();
        Preview = input.Highlight(matches, m => m.index, m => m.EndIndex, "red");
    }

    public override string Clean()
    {
        List<RegexMatch> matches = regionsFinder.GetMatches();
        for (int i = matches.Count - 1; i >= 0; i--)
            if (groupsRemoveEnabledStates[(int)(i * .5f)])
                input = input.Remove(matches[i].index, matches[i].value.Length);
        return input;
    }

    public override void DrawUI()
    {
        GUILayout.Label(regionsFinder.groups.Count + " region(s) found.");
        for (int i = 0; i < regionsFinder.groups.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            groupsRemoveEnabledStates[i] = EditorGUILayout.Toggle(groupsRemoveEnabledStates[i], GUILayout.Width(12));
            GUILayout.Label("\"" + regionsFinder.groups[i].opening.value + "\" at " + regionsFinder.groups[i].opening.index);
            GUILayout.EndHorizontal();
        }
    }
}
