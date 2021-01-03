using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeWeaponPickup : Interactable
{
    private MeleeWeapon meleeWeapon;

    protected void SetWeapon(MeleeWeapon meleeWeapon)
    {
        this.meleeWeapon = meleeWeapon;

        int childCount = transform.childCount;
        for (int i = 0; i < childCount; ++i)
            DestroyImmediate(transform.GetChild(0).gameObject);

        Instantiate(meleeWeapon.prefab, transform);
    }

    protected abstract bool InteractThreshold(PlayerCharacter player);

    protected abstract void OnEquip(PlayerCharacter player);

    protected override void _Interact(Character interactingCharacter)
    {
        PlayerCharacter player = interactingCharacter as PlayerCharacter;
        if (!player || !InteractThreshold(player))
            return;

        player.EquipMeleeWeapon(meleeWeapon);
        OnEquip(player);
        RemoveInteractable();
    }
}
