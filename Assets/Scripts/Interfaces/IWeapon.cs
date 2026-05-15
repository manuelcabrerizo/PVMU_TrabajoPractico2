using UnityEngine;

public interface IWeapon
{
    public float RateOfFire { get; }
    public int MaxAmmo { get; }
    public int CurrentAmmo { get; }
}
