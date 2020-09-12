using UnityEngine;
using UnityEngine.Events;

public class OnStartBehaviour : MonoBehaviour
{
    public UnityEvent Started;


    private void Start()
    {
        Started?.Invoke();
    }
}

public static class OnStartBehaviourUtilities
{
    public static void AddOnStartBehaviour(this GameObject caller, UnityAction action)
    {
        caller.GetOrAddComponent<OnStartBehaviour>().Started.AddListener(action);
    }
}
