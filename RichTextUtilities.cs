using System;
using System.Collections.Generic;
using System.Linq;

public static class RichTextUtilities
{
    public static string Highlight(this string input, int[] indexes, string color)
    {
        Array.Sort(indexes);
        for (int i = indexes.Length - 1; i >= 0; i--)
            input = Highlight(input, indexes[i], color);
        return input;
    }

    public static string Highlight<T>(this string input, T[] indexes, string color, Func<T, int> selector) => Highlight(input, indexes.ToList(), selector, color);
    public static string Highlight<T>(this string input, List<T> indexes, Func<T, int> start, string color)
    {
        indexes.Sort((iA, iB) => start(iA).CompareTo(start(iB)));
        for (int i = indexes.Count - 1; i >= 0; i--)
            input = Highlight(input, start(indexes[i]), color);
        return input;
    }

    public static string Highlight<T>(this string input, T[] indexes, Func<T, int> start, Func<T, int> end, string color) => Highlight(input, indexes.ToList(), start, end, color);
    public static string Highlight<T>(this string input, List<T> indexes, Func<T, int> start, Func<T, int> end, string color)
    {
        indexes.Sort((iA, iB) => start(iA).CompareTo(start(iB)));
        for (int i = indexes.Count - 1; i >= 0; i--)
            input = Highlight(input, start(indexes[i]), end(indexes[i]), color);
        return input;
    }

    public static string Highlight(this string input, int start, string color) => Highlight(input, start, start + 1, color);
    public static string Highlight(this string input, int start, int end, string color)
    {
        input = input.Insert(end, "</color>");
        input = input.Insert(start, "<color=" + color + ">");
        return input;
    }
}
