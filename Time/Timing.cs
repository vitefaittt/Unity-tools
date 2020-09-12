using System;

public class Timing
{
    public Timestamp start, end;

    public Timing()
    {
        start = DateTime.Now;
    }

    public TimeSpan GetDuration() => Timestamp.GetDuration(start, end);
}
