using System;

public class Timestamp
{
    public int hours, minutes, seconds;

    public Timestamp(int hours, int minutes, int seconds)
    {
        this.hours = hours;
        this.minutes = minutes;
        this.seconds = seconds;
    }

    public Timestamp(DateTime time)
    {
        hours = time.Hour;
        minutes = time.Minute;
        seconds = time.Second;
    }


    public static implicit operator DateTime(Timestamp timestamp)
    {
        DateTime result = new DateTime();
        result += new TimeSpan(timestamp.hours, timestamp.minutes, timestamp.seconds);
        result.AddMinutes(timestamp.minutes);
        result.AddSeconds(timestamp.seconds);
        return result;
    }
    public static implicit operator Timestamp(DateTime date) => new Timestamp(date);

    public static TimeSpan GetDuration(Timestamp timeA, Timestamp timeB)
    {
        if (timeA == null || timeB == null)
            return new TimeSpan();
        return (DateTime)timeB - timeA;
    }

    public override string ToString()
    {
        return hours + ":" + minutes + ":" + seconds;
    }
}