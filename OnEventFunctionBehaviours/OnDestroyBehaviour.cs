using UnityEngine;
using UnityEngine.Events;

public class OnDestroyBehaviour : MonoBehaviour
{
    public UnityEvent Destroyed;


    private void OnDestroy()
    {
        Destroyed?.Invoke();
    }
}

public static class OnDestroyBehaviourUtilities
{
    public static void AddOnDestroyBehaviour(this GameObject caller, UnityAction action)
    {
        caller.GetOrAddComponent<OnDestroyBehaviour>().Destroyed.AddListener(action);
    }
}
