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
    
    public bool TakeDamage(int damage)
    {
        CurrentHealth = Math.Max(CurrentHealth - damage, 0);
        if (HasStateAuthority)
        {
            Rpc_RaiseOnHealthChangeEvent();
            if (!IsAlive)
            {
                Rpc_RaiseOnPlayerDieEvent();
            }
        }
        return !IsAlive;
    }

    public void Cure()
    {
        CurrentHealth = MaxHealth;
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RaiseOnPlayerDieEvent()
    {
        EventBus.Raise<OnPlayerDieEvent>();
    }
}