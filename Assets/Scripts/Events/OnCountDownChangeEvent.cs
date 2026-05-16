public struct OnCountDownChangeEvent : IEvent
{
    public int CountDown;

    public void Assign(params object[] parameters)
    {
        CountDown = (int)parameters[0];
    }

    public void Reset()
    {
        CountDown = 0;
    }
}
