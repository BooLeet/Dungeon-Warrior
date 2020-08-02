using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory {
    private HashSet<LootScriptable> inventory;

    public CharacterInventory()
    {
        inventory = new HashSet<LootScriptable>();
    }

    public bool AddLoot(LootScriptable loot)
    {
        return inventory.Add(loot);
    }

    public bool HasLoot(LootScriptable loot)
    {
        return inventory.Contains(loot);
    }
}
