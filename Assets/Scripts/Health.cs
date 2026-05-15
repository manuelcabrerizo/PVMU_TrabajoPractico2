using Fusion;
using System;
using UnityEngine;

public class Health : NetworkBehaviour
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    public int MaxHealth = 100;
    [Networked] public int CurrentHealth { get; private set; } = 0;
    public bool IsAlive => CurrentHealth > 0;

    public override void Spawned()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth = Math.Max(CurrentHealth - damage, 0);
        Debug.Log("Damage Take: " + CurrentHealth);
    }
}