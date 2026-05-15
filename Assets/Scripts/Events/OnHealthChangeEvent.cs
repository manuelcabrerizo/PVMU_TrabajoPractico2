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


/*
[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
private void Rpc_Jump()
{
    Rpc_RelayJump();
}

[Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
private void Rpc_RelayJump()
{
    //IsJumping = true;
}
*/