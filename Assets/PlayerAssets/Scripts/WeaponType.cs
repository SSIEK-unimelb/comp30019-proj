using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// attach this script to a weapon to set its type
public class WeaponType : MonoBehaviour
{
    public enum Weapon { Knife, Crossbow }
    public Weapon currentWeapon;
}