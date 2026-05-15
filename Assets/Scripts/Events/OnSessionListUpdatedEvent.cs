using Fusion;
using System.Collections.Generic;

public struct OnSessionListUpdatedEvent : IEvent
{
    public List<SessionInfo> SessionInfoList;
    public void Assign(params object[] parameters)
    {
        SessionInfoList = (List<SessionInfo>)parameters[0];
    }
    public void Reset()
    {
        SessionInfoList = null;
    }
}
