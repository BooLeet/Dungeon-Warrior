using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponUpgrade : MeleeWeaponPickup
{
    private void Start()
    {
        SetWeapon(GetMeleeWeapon());
    }

    private MeleeWeapon GetMeleeWeapon()
    {
        return GameModeDelve.instance.GetMeleeUpgrade();
    }

    public override string GetPrompt(Character interactingCharacter)
    {
        int cost = GameModeDelve.instance.GetMeleeUpgradeCost();
        if (cost > GameModeDelve.instance.Score)
            return GameManager.instance.languagePack.GetString("tooExpensive") + " [ " + cost + " " + GameManager.instance.languagePack.GetString("pts") + " ]";
        return GameManager.instance.languagePack.GetString("buy") + " [ " + cost + " " + GameManager.instance.languagePack.GetString("pts") + " ]";
    }

    protected override bool InteractThreshold(PlayerCharacter player)
    {
        return GameModeDelve.instance.GetMeleeUpgradeCost() <= GameModeDelve.instance.Score;
    }

    protected override void OnEquip(PlayerCharacter player)
    {
        GameModeDelve.instance.OnMeleeUpgrade(ButtonPosition);
    }
}
