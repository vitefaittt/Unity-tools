public class StateListener
{
    public bool State { get; private set; }
    public bool StateChanged { get; private set; }

    public void Update(bool value)
    {
        StateChanged = State != value;
        State = value;
    }
}
