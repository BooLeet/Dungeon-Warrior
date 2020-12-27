using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment
{
    private MeleeWeapon meleeWeapon;

    public void SetMeleeWeapon(MeleeWeapon weapon)
    {
        meleeWeapon = weapon;
    }

    public MeleeWeapon GetMeleeWeapon()
    {
        return meleeWeapon;
    }
}
