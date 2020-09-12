using System;
using System.Collections;
using UnityEngine;

public static class AudioSourceUtilities
{
    public static void PlayClip(this AudioSource audio, AudioClip clip)
    {
        audio.clip = clip;
        audio.Play();

    }

    public static Coroutine OnAudioEnd(this AudioSource audio, MonoBehaviour caller, Action EndAction)
    {
        return caller.StartCoroutine(OnAudioEndRoutine(audio, EndAction));
    }

    static IEnumerator OnAudioEndRoutine(AudioSource audio, Action EndAction)
    {
        yield return new WaitUntil(() => audio.isPlaying);
        yield return new WaitUntil(() => !audio.isPlaying);
        EndAction?.Invoke();
    }

    public static bool IsPlaying(this AudioSource audio, AudioClip clip)
    {
        return audio.isPlaying && audio.clip == clip;
    }
}
