using UnityEngine;

public class VRPrinterFullLog : MonoBehaviour
{
    [SerializeField]
    [TextArea]
    string about = "Sends all Console messages to VRPrinter.";
    [Space]

    [SerializeField]
    bool onlyErrors;


    void Reset()
    {
        this.RenameFromType();
    }

    void Awake()
    {
        if (onlyErrors)
            Application.logMessageReceived += delegate (string condition, string stackTrace, LogType type)
            {
                if (type == LogType.Error)
                    VRPrinter.Print(condition);
            };
        else
            Application.logMessageReceived += delegate (string condition, string stackTrace, LogType type)
            {
                VRPrinter.Print(condition);
            };
    }
}
