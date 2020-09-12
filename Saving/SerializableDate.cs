using System;

[Serializable]
public class SerializableDate
{
    public int year;
    public int month;
    public int day;
    public int hour, minutes, seconds;

    public SerializableDate(DateTime date)
    {
        year = date.Year;
        month = date.Month;
        day = date.Day;
        hour = date.Hour;
        minutes = date.Minute;
        seconds = date.Second;
    }

    public DateTime ToDateTime()
    {
        return new DateTime(year, month, day, hour, minutes, seconds);
    }
}
