using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
public static class OfflineCanvasNavigationKeyListener
{
    static OfflineCanvasNavigation navigation;
    static readonly float inputDelay = .3f;
    static float lastInputTime;
    static bool WantsToClick => Event.current != null && Event.current.keyCode == KeyCode.Quote;
    static bool CanClick => FakeTime.Time - lastInputTime > inputDelay;

    static OfflineCanvasNavigationKeyListener()
    {
        SceneView.duringSceneGui += view =>
        {
            if (WantsToClick)
            {
                if (CanClick)
                    TriggerNavigation();
                lastInputTime = FakeTime.Time;
            }
        };
    }

    static void TriggerNavigation()
    {
        if (!navigation)
            navigation = (OfflineCanvasNavigation)GameObject.FindObjectsOfType(typeof(OfflineCanvasNavigation))[0];
        navigation.Navigate();
    }
}

class FakeTime
{
    public static float Time => DateTime.Now.Second + DateTime.Now.Millisecond * .001f;
}
#endif