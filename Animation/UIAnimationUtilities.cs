using UnityEngine;
using UnityEngine.UI;

public static class UIAnimationUtilities
{
    public static void Flash(this Text text, MonoBehaviour caller, Color color, float duration = 1.5f, float delay = 0)
    {
        // Hide the text after some time.
        text.color = color;
        caller.Timer(delay, () =>
            caller.ProgressionAnim(duration, (progression) =>
                text.color = Color.Lerp(color, Color.clear, AniMath.SmoothStart(progression))
        ));
    }
}
