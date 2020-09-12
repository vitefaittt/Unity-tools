using UnityEngine;

public class ComponentIsEnabledLink : MonoBehaviour
{
    public MonoBehaviour watcher, watched;


    private void Reset()
    {
        watcher = GetComponent<MonoBehaviour>();
    }

    private void Update()
    {
        watcher.enabled = watched.enabled;
    }
}
