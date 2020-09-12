using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIFadeAnim : MonoBehaviour
{
    Coroutine anim;
    public bool IsPlaying => anim != null;
    public List<Graphic> graphicsToAnimate = new List<Graphic>();
    Dictionary<Graphic, Color> defaultColors = new Dictionary<Graphic, Color>();
    public Color fadeColor = Color.clear;
    public float fadeDuration = 1;
    public UnityEvent FadeInComplete, FadeOutComplete;


    private void Reset()
    {
        graphicsToAnimate.AddRange(GetComponentsInChildren<Graphic>());
    }


    [ContextMenu("Fade in")]
    void FadeIn() => FadeIn(null);
    public void FadeIn(System.Action EndAction = null)
    {
        GetUnknownDefaultColors();

        if (anim.IsRunning())
            StopCoroutine(anim);

        anim = this.ProgressionAnim(fadeDuration, (progression) =>
        {
            for (int i = 0; i < graphicsToAnimate.Count; i++)
                graphicsToAnimate[i].color = Color.Lerp(graphicsToAnimate[i].color, defaultColors[graphicsToAnimate[i]], progression);
        }, () =>
        {
            anim = null;
            EndAction?.Invoke();
            FadeInComplete.Invoke();
        });
    }

    [ContextMenu("Fade out")]
    void FadeOut() => FadeOut(null);
    public void FadeOut(System.Action EndAction = null)
    {
        GetUnknownDefaultColors();

        if (anim.IsRunning())
            StopCoroutine(anim);

        anim = this.ProgressionAnim(fadeDuration, (progression) =>
        {
            for (int i = 0; i < graphicsToAnimate.Count; i++)
                graphicsToAnimate[i].color = Color.Lerp(graphicsToAnimate[i].color, fadeColor, progression);
        }, () =>
        {
            anim = null;
            EndAction?.Invoke();
            FadeOutComplete.Invoke();
        });
    }

    void GetUnknownDefaultColors()
    {
        // Get default colors.
        for (int i = 0; i < graphicsToAnimate.Count; i++)
            if (!defaultColors.ContainsKey(graphicsToAnimate[i]))
                defaultColors.Add(graphicsToAnimate[i], graphicsToAnimate[i].color);
    }
}
