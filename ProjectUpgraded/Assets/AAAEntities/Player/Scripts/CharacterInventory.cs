using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventory {
    public HashSet<LootScriptable> uniqueLoot;
    public class Resource
    {
        public LootScriptable loot;
        public int amount;

        public Resource(LootScriptable loot, int amount)
        {
            this.loot = loot;
            this.amount = amount;
        }
    }

    public List<Resource> resources;

    public CharacterInventory()
    {
        uniqueLoot = new HashSet<LootScriptable>();
        resources = new List<Resource>();
    }

    public bool AddLoot(LootScriptable loot)
    {
        return uniqueLoot.Add(loot);
    }

    public void AddResource(LootScriptable loot,int amount)
    {
        var tempQuery = GetResources(loot);
        if (tempQuery.Count() > 0)
        {
            tempQuery.First().amount += amount;
            return;
        }
        resources.Add(new Resource(loot, amount));
    }

    public bool HasResource(LootScriptable loot,int amount = 1)
    {
        var tempQuery = GetResources(loot);
        return tempQuery.Count() > 0 && tempQuery.First().amount >= amount;
    }
    
    public void SpendResource(LootScriptable loot, int amount = 1)
    {
        var tempQuery = GetResources(loot);
        if(tempQuery.Count() > 0 && tempQuery.First().amount >= amount)
            tempQuery.First().amount -= amount;
    }

    private IEnumerable<Resource> GetResources(LootScriptable loot)
    {
        return from tuple in resources
               where tuple.loot == loot
               select tuple;
    }

    public bool HasLoot(LootScriptable loot)
    {
        return uniqueLoot.Contains(loot);
    }
}
