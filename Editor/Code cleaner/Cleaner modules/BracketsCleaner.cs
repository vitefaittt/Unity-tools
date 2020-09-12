using System.Collections.Generic;
using UnityEngine;
using static RegexUtilities;

public class BracketsCleaner : CleanerModule
{
    List<int> openingBracketsIndexes = new List<int>();
    List<int> closingBracketsIndexes = new List<int>();
    List<int> semicolonIndexes = new List<int>();

    public struct Bracket
    {
        public BracketType type;
        public int index;
        public int containerIndex;
        public int semicolonItem;

        public Bracket(BracketType type, int index, int containerIndex, int semicolonItem)
        {
            this.type = type;
            this.index = index;
            this.containerIndex = containerIndex;
            this.semicolonItem = semicolonItem;
        }
    }
    public enum BracketType { Opening, Closing }
    public List<Bracket> brackets = new List<Bracket>();
    string input;

    public override void Start(string input)
    {
        this.input = input;
        brackets.Clear();

        openingBracketsIndexes = GetMatchesIndexes(input, "{");
        closingBracketsIndexes = GetMatchesIndexes(input, "}");
        semicolonIndexes = GetMatchesIndexes(input, ";");

        // Find facultative brackets following common containers.
        List<RegexMatch> containers = new List<RegexMatch>();
        containers.AddRange(GetMatches(input, @"(?<!else )(?<!#)if *\("));
        containers.AddRange(GetMatches(input, @"(?<!#)else"));
        containers.AddRange(GetMatches(input, @"for *\(.*\)"));
        containers.AddRange(GetMatches(input, @"while"));
        containers.AddRange(GetMatches(input, @"foreach"));
        FindFacultativeBrackets(containers);

        // Set preview.
        Preview = RichTextUtilities.Highlight(input, brackets, bracket => bracket.index, "red");
    }

    public override string Clean()
    {
        List<int> nNewlineIndexes = GetMatchesIndexes(input, "\n");
        List<int> rNewlineIndexes = GetMatchesIndexes(input, "\r");
        for (int i = brackets.Count - 1; i >= 0; i--)
        {
            // Remove the bracket.
            input = input.Remove(brackets[i].index, 1);

            // Remove the n newline before the bracket.
            int newline = FindBracketNewline(brackets[i], semicolonIndexes[brackets[i].semicolonItem], nNewlineIndexes);
            if (newline > -1)
                input = input.Remove(nNewlineIndexes[newline], 2);

            // Remove the r newline before the bracket.
            newline = FindBracketNewline(brackets[i], semicolonIndexes[brackets[i].semicolonItem], rNewlineIndexes);
            if (newline > -1)
                input = input.Remove(rNewlineIndexes[newline], 2);
        }
        return input;
    }

    static int FindBracketNewline(Bracket bracket, int semicolonIndex, List<int>newlines)
    {
        for (int j = newlines.Count - 1; j >= 0 ; j--)
            if ((bracket.type == BracketType.Closing && newlines[j].IsBetween(semicolonIndex, bracket.index)) ||
                (bracket.type == BracketType.Opening && newlines[j].IsBetween(bracket.containerIndex, bracket.index)))
                return j;
        return -1;
    }

    public override void DrawUI()
    {
        GUILayout.Label(brackets.Count + " optional bracket(s) found.");
    }

    void FindFacultativeBrackets(List<RegexMatch> containerMatches)
    {
        for (int i = 0; i < containerMatches.Count; i++)
        {
            // Find the next semicolon.
            int semicolonItem = -1;
            for (int j = 0; j < semicolonIndexes.Count && semicolonItem < 0; j++)
                if (semicolonIndexes[j] > containerMatches[i].EndIndex)
                    semicolonItem = j;

            // Find opening brackets between the container and the semicolon.
            List<int> openingBracketItems = new List<int>();
            for (int j = 0; j < openingBracketsIndexes.Count; j++)
                if (openingBracketsIndexes[j] >= containerMatches[i].EndIndex && openingBracketsIndexes[j] <= semicolonIndexes[semicolonItem])
                    openingBracketItems.Add(j);

            int closingBracketsCount = 0;
            // Find the next closing bracket.
            int closingBracketItem = -1;
            for (int j = 0; j < closingBracketsIndexes.Count && closingBracketItem < 0; j++)
                if (closingBracketsIndexes[j] > semicolonIndexes[semicolonItem])
                {
                    closingBracketsCount++;
                    if (closingBracketsCount >= openingBracketItems.Count)
                        closingBracketItem = j;
                }

            // If the next semicolon isn't between the two brackets, add these brackets.
            int nextSemicolonItem = semicolonIndexes.Count - 1 > semicolonItem ? semicolonItem + 1 : -1;
            if (nextSemicolonItem < 0 || semicolonIndexes[nextSemicolonItem] > closingBracketsIndexes[closingBracketItem])
            {
                brackets.Add(new Bracket(BracketType.Opening, openingBracketsIndexes[openingBracketItems[0]], containerMatches[i].index, semicolonItem));
                brackets.Add(new Bracket(BracketType.Closing, closingBracketsIndexes[closingBracketItem], containerMatches[i].index, semicolonItem));
            }
        }

        // Sort result brackets.
        brackets.Sort((bA, bB) => bA.index.CompareTo(bB.index));
    }
}
