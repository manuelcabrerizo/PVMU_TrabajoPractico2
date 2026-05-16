public struct OnMachTimerDownChangeEvent : IEvent
{
    public int Seconds;

    public void Assign(params object[] parameters)
    {
        Seconds = (int)parameters[0];
    }

    public void Reset()
    {
        Seconds = 0;
    }
}
