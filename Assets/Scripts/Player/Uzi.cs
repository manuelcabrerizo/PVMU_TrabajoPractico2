using UnityEngine;

public class Uzi : MonoBehaviour,  IWeapon
{
    public float RateOfFire => rateOfFire;
    public int MaxAmmo => maxAmmo;
    public int CurrentAmmo => currentAmmo;

    [SerializeField] private float rateOfFire = 0.1f;
    [SerializeField] private int maxAmmo = 32;
    private int currentAmmo = 0;

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }
}