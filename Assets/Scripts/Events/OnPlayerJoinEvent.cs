using Fusion;

public struct OnPlayerJoinEvent : IEvent
{
    public PlayerRef PlayerRef;
    public int PlayerCount;
    public int TargetPlayerCount;
    public void Assign(params object[] parameters)
    {
        PlayerRef = (PlayerRef)parameters[0];
        PlayerCount = (int)parameters[1];
        TargetPlayerCount = (int)parameters[2];
    }
    public void Reset()
    {
        PlayerRef = PlayerRef.None;
        PlayerCount = 0;
        TargetPlayerCount= 0;
    }
}
