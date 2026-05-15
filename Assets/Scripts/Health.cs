using Fusion;
using System;
using UnityEngine;

public class Health : NetworkBehaviour
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    [SerializeField] private int MaxHealth = 100;
    [Networked] public int CurrentHealth { get; private set; } = 0;
    public bool IsAlive => CurrentHealth > 0;
    
    public override void Spawned()
    {
        CurrentHealth = MaxHealth;
        if (HasStateAuthority)
        {
            Rpc_RaiseOnHealthChangeEvent();
        }
    }
    
    public void TakeDamage(int damage)
    {
        CurrentHealth = Math.Max(CurrentHealth - damage, 0);
        if (HasStateAuthority)
        {
            Rpc_RaiseOnHealthChangeEvent();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RaiseOnHealthChangeEvent()
    {
        EventBus.Raise<OnHealthChangeEvent>(CurrentHealth, MaxHealth);
    }
}

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