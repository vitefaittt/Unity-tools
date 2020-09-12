using System;

public static class JavascriptDateUtility
{
    public static string ToJavascript(DateTime date)
    {
        return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds.ToString();
    }

    public static DateTime ToDateTime(string date)
    {
        return new DateTime(1970, 1, 1).AddMilliseconds(double.Parse(date)).ToLocalTime();
    }
}
