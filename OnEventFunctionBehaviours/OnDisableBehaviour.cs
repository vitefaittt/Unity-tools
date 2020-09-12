using UnityEngine;
using UnityEngine.Events;

public class OnDisableBehaviour : MonoBehaviour
{
    public UnityEvent Disabled;


    private void OnDisable()
    {
        Disabled?.Invoke();
    }
}

public static class OnDisableBehaviourUtilities
{
    public static void AddOnDisableBehaviour(this GameObject caller, UnityAction action)
    {
        OnDisableBehaviour onDisable = caller.GetOrAddComponent<OnDisableBehaviour>();
        onDisable.Disabled = new UnityEvent();
        onDisable.Disabled.AddListener(action);
    }
}
