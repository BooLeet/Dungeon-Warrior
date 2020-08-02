using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAmmunition {
    public enum AmmoType { Infinite, Bullet, Shell, Nail, Rocket, EnergyCell}
    private Dictionary<AmmoType, uint> currentAmmunition = new Dictionary<AmmoType, uint>();
    private Dictionary<AmmoType, uint> maxAmmunition = new Dictionary<AmmoType, uint>();

    // Constructor
    public CharacterAmmunition(uint maxBullets, uint maxShells, uint maxNails, uint maxRockets, uint maxEnergyCells)
    {
        currentAmmunition.Add(AmmoType.Bullet, 0);
        currentAmmunition.Add(AmmoType.Shell, 0);
        currentAmmunition.Add(AmmoType.Nail, 0);
        currentAmmunition.Add(AmmoType.Rocket, 0);
        currentAmmunition.Add(AmmoType.EnergyCell, 0);

        maxAmmunition.Add(AmmoType.Bullet, maxBullets);
        maxAmmunition.Add(AmmoType.Shell, maxShells);
        maxAmmunition.Add(AmmoType.Nail, maxNails);
        maxAmmunition.Add(AmmoType.Rocket, maxRockets);
        maxAmmunition.Add(AmmoType.EnergyCell, maxEnergyCells);
    }

    // Returns true and removes ammo if character has the required amount
    public bool SpendAmmo(AmmoType type, uint amount)
    {
        if (type == AmmoType.Infinite)
            return true;

        if(currentAmmunition[type] >= amount)
        {
            currentAmmunition[type] -= amount;
            return true;
        }

        return false;
    }

    // Returns the amount of ammo available for the given AmmoType
    public uint GetAmmo(AmmoType type)
    {
        if (type == AmmoType.Infinite)
            return 1;

        return currentAmmunition[type];
    }

    // Returns the amount of max ammo for the given AmmoType
    public uint GetMaxAmmo(AmmoType type)
    {
        if (type == AmmoType.Infinite)
            return 1;

        return maxAmmunition[type];
    }

    // Returns true and adds given amount of ammo if the current amount isn't max
    public bool AddAmmo(AmmoType type, uint amount)
    {
        if (type == AmmoType.Infinite)
            return true;

        if (currentAmmunition[type] >= maxAmmunition[type])
            return false;

        currentAmmunition[type] += amount;

        // clamping
        if (currentAmmunition[type] > maxAmmunition[type])
            currentAmmunition[type] = maxAmmunition[type];

        return true;
    }


}
