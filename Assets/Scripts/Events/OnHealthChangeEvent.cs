public struct OnHealthChangeEvent : IEvent
{
    public int CurrentHealth;
    public int MaxHealth;

    public void Assign(params object[] parameters)
    {
        CurrentHealth = (int)parameters[0];
        MaxHealth = (int)parameters[1];
    }

    public void Reset()
    {
        CurrentHealth = 0;
        MaxHealth = 0;
    }
}
