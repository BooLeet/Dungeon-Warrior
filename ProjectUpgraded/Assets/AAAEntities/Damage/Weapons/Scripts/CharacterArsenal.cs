using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Arsenal")]
public class CharacterArsenal : ScriptableObject {
    [Header("Weapons")]
    public Weapon[] weapons;
    [Range(1, 9)]
    public int preferedWeaponSlot = 1;
    [Header("Ammunition")]
    public uint maxBullets = 300;
    public uint maxShells = 60;
    public uint maxNails = 300;
    public uint maxRockets = 20;
    public uint maxEnergyCells = 40;
    [Space]
    public uint startingBullets = 60;
    public uint startingShells = 20;
    public uint startingNails = 60;
    public uint startingRockets = 5;
    public uint startingEnergyCells = 10;

    // Creates a new ammunition filled with starting ammo
    public CharacterAmmunition GetAmmunition()
    {
        CharacterAmmunition ammunition = new CharacterAmmunition(maxBullets, maxShells, maxNails, maxRockets, maxEnergyCells);

        ammunition.AddAmmo(CharacterAmmunition.AmmoType.Bullet, startingBullets);
        ammunition.AddAmmo(CharacterAmmunition.AmmoType.Shell, startingShells);
        ammunition.AddAmmo(CharacterAmmunition.AmmoType.Nail, startingNails);
        ammunition.AddAmmo(CharacterAmmunition.AmmoType.Rocket, startingRockets);
        ammunition.AddAmmo(CharacterAmmunition.AmmoType.EnergyCell, startingEnergyCells);

        return ammunition;
    }
}
