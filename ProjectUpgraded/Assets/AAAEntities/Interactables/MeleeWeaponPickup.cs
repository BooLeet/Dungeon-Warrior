using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponPickup : Interactable
{
    public MeleeWeapon meleeWeapon;

    private void Start()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; ++i)
            DestroyImmediate(transform.GetChild(0).gameObject);

        Instantiate(meleeWeapon.prefab, transform);
    }

    public override string GetPrompt(Character interactingCharacter)
    {
        return "[PLACEHOLDER]";
    }

    protected override void _Interact(Character interactingCharacter)
    {
        PlayerCharacter player = interactingCharacter as PlayerCharacter;
        if (!player)
            return;
        player.EquipMeleeWeapon(meleeWeapon);
        RemoveInteractable();
    }
}
