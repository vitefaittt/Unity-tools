using UnityEngine;
using UnityEngine.Events;

public class OnEnableBehaviour : MonoBehaviour
{
    public UnityEvent Enabled;

    
    private void OnEnable()
    {
        Enabled?.Invoke();
    }
}

public static class OnEnableBehaviourUtilities
{
    public static void AddOnEnableBehaviour(this GameObject caller, UnityAction action)
    {
        OnEnableBehaviour onEnable = caller.GetOrAddComponent<OnEnableBehaviour>();
        onEnable.Enabled = new UnityEvent();
        onEnable.Enabled.AddListener(action);
    }
}
