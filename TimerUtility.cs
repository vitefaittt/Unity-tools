using System;
using System.Collections;
using UnityEngine;

public static class TimerUtility
{
    /// <summary>
    /// Execute a function after a duration.
    /// </summary>
    /// <param name="duration">Amount of seconds to wait for.</param>
    public static Coroutine Timer(this MonoBehaviour caller, float duration, Action EndAction)
    {
        Coroutine routine = caller.StartCoroutine(TimerRoutine(duration, EndAction));
        return routine;
    }

    static IEnumerator TimerRoutine(float duration, Action EndAction)
    {
        yield return new WaitForSeconds(duration);
        EndAction();
    }
}