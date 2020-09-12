using System;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton){ }
    public static DialogResult Show(string text){ }
    public static DialogResult Show(string text, string caption){ }
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons){ }
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon){ }
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton){ }
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options){ }
}


public enum DialogResult
{
    None = 0,
    OK = 1,
    Cancel = 2,
    Abort = 3,
    Retry = 4,
    Ignore = 5,
    Yes = 6,
    No = 7
}

public enum MessageBoxButtons
{
    OK = 0,
    OKCancel = 1,
    AbortRetryIgnore = 2,
    YesNoCancel = 3,
    YesNo = 4,
    RetryCancel = 5
}

public enum MessageBoxIcon
{
                None = 0,
                    Hand = 16,
                    Stop = 16,
                    Error = 16,
                                        Question = 32,
                    Exclamation = 48,
                    Warning = 48,
                Asterisk = 64,
                Information = 64
}

public enum MessageBoxDefaultButton
{
                Button1 = 0,
                Button2 = 256,
                Button3 = 512
}

[Flags]
public enum MessageBoxOptions
{
                DefaultDesktopOnly = 131072,
                RightAlign = 524288,
                RtlReading = 1048576,
                ServiceNotification = 2097152
}

public enum HelpNavigator
{
                Topic = -2147483647,
                TableOfContents = -2147483646,
                Index = -2147483645,
                Find = -2147483644,
                AssociateIndex = -2147483643,
                    KeywordIndex = -2147483642,
                TopicId = -2147483641
}