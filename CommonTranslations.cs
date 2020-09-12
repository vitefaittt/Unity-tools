using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;

class CommonTranslations
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);
    [DllImport("kernel32")]
    static extern IntPtr LoadLibrary(string lpFileName);

    static void Test()
    {
        StringBuilder sb = new StringBuilder(256);
        IntPtr user32 = LoadLibrary(Environment.SystemDirectory + "\\User32.dll");
        LoadString(user32, 805, sb, sb.Capacity);
        YES = sb.ToString().Replace("&", "");
        LoadString(user32, 806, sb, sb.Capacity);
        NO = sb.ToString().Replace("&", "");
    }

    public static string YES;
    public static string NO;


    [MenuItem("Window/Translations")]
    static void TestTranslations()
    {
        Test();
        Debug.Log(YES);
    }
}