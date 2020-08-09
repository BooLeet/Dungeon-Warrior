using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : Interactable
{
    public float healAmount = 40;
    public AudioClip quafSound;

    public override string GetPrompt(Character interactingCharacter)
    {
        return GameManager.instance.languagePack.GetString("healthPotionUse");
    }

    public override void Interact(Character interactingCharacter)
    {
        interactingCharacter.GiveHealth(healAmount);
        if(quafSound)
            Utility.PlayAudioClipAtPoint(quafSound, ButtonPosition, null, 1, GameManager.instance.GetSfxMixerGroup());
        RemoveInteractable();
    }
}
