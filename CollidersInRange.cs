using System;
using System.Collections.Generic;
using UnityEngine;

public class CollidersInRange : MonoBehaviour
{
    public Func<Collider, bool> Condition;
    List<Collider> colliders = new List<Collider>();
    public event Action<Collider> FirstColliderAdded, ColliderAdded, ColliderRemoved, LastColliderRemoved;
    public Collider ClosestCollider => Utilities.GetClosestItemFromPosition(colliders, transform.position);


    void OnTriggerEnter(Collider other)
    {
        // Remember a new colllider.
        if (Condition == null || Condition(other))
        {
            colliders.Add(other);
            ColliderAdded?.Invoke(other);
            FirstColliderAdded?.Invoke(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Forget a previous collider.
        if (Condition == null || Condition(other) && colliders.Contains(other))
        {
            colliders.Remove(other);
            ColliderRemoved?.Invoke(other);
            LastColliderRemoved?.Invoke(other);
        }
    }

    void OnDisable()
    {
        Clear();
    }


    public void Clear()
    {
        colliders.Clear();
    }
}
