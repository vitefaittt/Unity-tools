using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEnterExit : MonoBehaviour
{
    public ColliderEvent OnEnter;
    public ColliderEvent OnExit;


    void OnTriggerEnter(Collider other)
    {
        OnEnter.Invoke(other);
    }

    void OnTriggerExit(Collider other)
    {
        OnExit.Invoke(other);
    }
}

[System.Serializable]
public class ColliderEvent : UnityEvent<Collider> { }