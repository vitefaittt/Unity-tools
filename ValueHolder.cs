public class ValueHolder<T>
{
    T value;
    public T Value {
        get => value;
        set {
            if (Value == null)
                ValueChanged = value != null;
            else
                ValueChanged = !Value.Equals(value);
            if (ValueChanged)
                PreviousValue = Value;
            this.value = value;
            if (ValueChanged)
                OnValueChanged?.Invoke(this.value);
        }
    }
    public T PreviousValue { get; private set; }
    public bool ValueChanged { get; private set; }
    public event System.Action<T> OnValueChanged;
}
