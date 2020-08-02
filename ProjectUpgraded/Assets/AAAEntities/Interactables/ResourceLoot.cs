using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoot : Interactable
{
    public LootScriptable lootScriptable;
    [Range(1, 9999)]
    public int resourceAmount = 1;

    public override void Interact(Character interactingCharacter)
    {
        interactingCharacter.inventory.AddResource(lootScriptable, resourceAmount);
        RemoveInteractable();
    }

    public override string GetPrompt(Character interactingCharacter)
    {
        return GameManager.instance.languagePack.GetString("lootPickUp");
    }
}
