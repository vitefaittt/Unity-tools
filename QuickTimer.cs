using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class QuickTimer : MonoBehaviour
{
    public float seconds = 3;
    public UnityEvent OnStart;


    IEnumerator Start()
    {
        yield return new WaitForSeconds(seconds);
    }
}
