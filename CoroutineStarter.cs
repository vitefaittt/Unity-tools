using System.Collections;
using UnityEngine;

public class CoroutineStarter : MonoBehaviour
{
    public static CoroutineStarter Instance { get; private set; }


    void OnDestroy()
    {
        Instance = null;
    }


    public new static Coroutine StartCoroutine(IEnumerator routine)
    {
        if (!Instance)
            Instance = new GameObject("Coroutine starter", typeof(CoroutineStarter)).GetComponent<CoroutineStarter>();
        return ((MonoBehaviour)Instance).StartCoroutine(routine);
    }
}
