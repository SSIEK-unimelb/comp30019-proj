using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AmmoManager : MonoBehaviour
{
    [SerializeField] int currentAmmo;
    [SerializeField] public UnityEvent<int> onAmmoChange;
    
    public void SetAmmo(int ammo)
    {
        currentAmmo = ammo;
        // Notify subscribers of ammo change
        onAmmoChange.Invoke(currentAmmo);
    }

    public void ChangeAmmo(int amount)
    {
        currentAmmo += amount;
        // Notify subscribers of ammo change
        onAmmoChange.Invoke(currentAmmo);
    }
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

}
