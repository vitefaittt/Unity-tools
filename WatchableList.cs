namespace System.Collections.Generic
{
    public class WatchableList<T> : List<T>
    {
        public event Action ItemAdded, ItemRemoved, FirstItemAdded, LastItemRemoved;

        new public void Add(T item)
        {
            if (!Contains(item))
            {
                base.Add(item);
                ItemAdded?.Invoke();
                if (Count == 1)
                    FirstItemAdded?.Invoke();
            }
        }

        new public void Remove(T item)
        {
            if (Contains(item))
            {
                base.Remove(item);
                ItemRemoved?.Invoke();
                if (Count < 1)
                    LastItemRemoved?.Invoke();
            }
        }
    }
}