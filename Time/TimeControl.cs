public static class TimeControl
{
    public static TimeData Time => SecondsToTime(GetCurrentTimeInSeconds());
    static TimeData offset;

    public struct TimeData
    {
        public int hours;
        public int minutes;
        public float seconds;
    }

    public static void AddTimeOffset(float offsetInSeconds)
    {
        float totalOffsetSeconds = offsetInSeconds + offset.hours * 3600 + offset.minutes * 60 + offset.seconds;
        offset = SecondsToTime(totalOffsetSeconds);
    }

    static float GetCurrentTimeInSeconds()
    {
        return System.DateTime.Now.Hour * 3600 + offset.hours * 3600 + System.DateTime.Now.Minute * 60 + offset.minutes * 60 + System.DateTime.Now.Second + offset.seconds;
    }

    static TimeData SecondsToTime(float seconds)
    {
        TimeData result = new TimeData();
        result.hours = (int)seconds / 3600;
        result.minutes = (int)(seconds - result.hours * 3600) / 60;
        result.seconds = seconds - result.hours * 3600 - result.minutes * 60;
        if (result.hours > 23)
            result.hours = result.hours - 23 * (result.hours / 23);
        return result;
    }
}