using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Interactable
{
    public float healAmount = 40;
    public AudioClip quaffSound;

    public override string GetPrompt(Character interactingCharacter)
    {
        return GameManager.instance.languagePack.GetString("healthPotionUse");
    }

    protected override void _Interact(Character interactingCharacter)
    {
        interactingCharacter.GiveHealth(healAmount);
        if(quaffSound)
            Utility.PlayAudioClipAtPoint(quaffSound, ButtonPosition, null, 1, GameManager.instance.GetSfxMixerGroup());
        RemoveInteractable();
    }
}
