public struct OnPlayerDieEvent : IEvent
{
    public void Assign(params object[] parameters)
    {
    }
    public void Reset()
    {
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