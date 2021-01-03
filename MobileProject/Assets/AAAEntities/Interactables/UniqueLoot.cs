using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueLoot : Interactable {
    public LootScriptable lootScriptable;

    protected override void _Interact(Character interactingCharacter)
    {
        if (interactingCharacter.inventory.AddLoot(lootScriptable))
            RemoveInteractable();
    }

    public override string GetPrompt(Character interactingCharacter)
    {
        return interactingCharacter.inventory.HasLoot(lootScriptable) ? GameManager.instance.languagePack.GetString("lootAlreadyHave") : GameManager.instance.languagePack.GetString("lootPickUp");
    }
}
