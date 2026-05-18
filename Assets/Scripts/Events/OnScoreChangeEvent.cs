public struct OnScoreChangeEvent : IEvent
{
    public int Score;
    public void Assign(params object[] parameters)
    {
        Score = (int)parameters[0];
    }
    public void Reset()
    {
        Score = 0;
    }
}