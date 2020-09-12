using UnityEngine;

public class FrontEndLog
{
    public string message;
    public enum LogType { Sucess, Error }
    public LogType type;
    public Color Color => type == LogType.Sucess ? Color.green : Color.red;
    public string StylizedMessage => (type == LogType.Sucess ? "✔ " : "x ") + message;

    public FrontEndLog(string message, LogType type)
    {
        this.message = message;
        this.type = type;
    }
}
