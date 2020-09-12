List<int> GetMatchesIndexes(string input, string pattern)
{
    List<int> result = new List<int>();
    MatchCollection matches = Regex.Matches(input, pattern);
    foreach (Match match in matches)
        foreach (Group group in match.Groups)
            result.Add(group.Index);
    return result;
}