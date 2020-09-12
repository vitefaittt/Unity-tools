using System.Collections.Generic;
using System.Text.RegularExpressions;

public abstract class RegexUtilities
{
    public struct RegexMatch
    {
        public int index;
        public string value;

        public RegexMatch(int index, string value)
        {
            this.index = index;
            this.value = value;
        }
    }

    public static List<RegexMatch> GetMatches(string input, string pattern)
    {
        List<RegexMatch> result = new List<RegexMatch>();
        MatchCollection matches = Regex.Matches(input, pattern);
        foreach (Match match in matches)
            foreach (Group group in match.Groups)
                result.Add(new RegexMatch(group.Index, group.Value));
        return result;
    }

    public static List<int> GetMatchesIndexes(string input, string pattern)
    {
        List<int> result = new List<int>();
        MatchCollection matches = Regex.Matches(input, pattern);
        foreach (Match match in matches)
            foreach (Group group in match.Groups)
                result.Add(group.Index);
        return result;
    }
}