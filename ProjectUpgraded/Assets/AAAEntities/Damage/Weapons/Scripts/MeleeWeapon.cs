using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/MeleeWeapon")]
public class MeleeWeapon : Weapon
{
    public float baseDamageMultiplier = 1;
    public float stingDamageMultiplier = 1;
    public float spinDamageMultiplier = 1;
}
