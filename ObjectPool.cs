using System;
using System.Collections.Generic;

public class ObjectPool<T>
{
    List<T> items = new List<T>();
    public int Amount {
        get {
            return items.Count;
        }
        set {
            // Add or remove items.
            if (items.Count < value)
                AddAmount(value - items.Count);
            else if (items.Count > value)
                RemoveAmount(value < 0 ? 0 : (items.Count - value));
        }
    }
    Func<T> Instantiate;
    Action<T> Destroy;

    /// <param name="Instantiate">Function used to create a new object.</param>
    /// <param name="Destroy">Function used to destroy an object.</param>
    public ObjectPool(Func<T> Instantiate, Action<T> Destroy)
    {
        this.Instantiate = Instantiate;
        this.Destroy = Destroy;
    }

    public T GetItem(int index)
    {
        if (index > items.Count + 1)
            AddAmount(index - (items.Count - 1));
        return items[index];
    }

    void AddAmount(int amount)
    {
        for (int i = 0; i < amount; i++)
            items.Add(Instantiate());
    }

    void RemoveAmount(int amount)
    {
        for (int i = items.Count; i > amount; i--)
        {
            Destroy(items[i - 1]);
            items.RemoveAt(i - 1);
        }
    }
}
