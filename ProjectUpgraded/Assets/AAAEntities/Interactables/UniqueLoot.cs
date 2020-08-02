using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniqueLoot : Interactable {
    public LootScriptable lootScriptable;

    public override void Interact(Character interactingCharacter)
    {
        if (interactingCharacter.inventory.AddLoot(lootScriptable))
            RemoveInteractable();
    }

    public override string GetPrompt(Character interactingCharacter)
    {
        return interactingCharacter.inventory.HasLoot(lootScriptable) ? GameManager.instance.languagePack.GetString("lootAlreadyHave") : GameManager.instance.languagePack.GetString("lootPickUp");
    }
}
