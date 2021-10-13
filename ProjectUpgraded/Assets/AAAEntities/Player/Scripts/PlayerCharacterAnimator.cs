using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerCharacterAnimator : CharacterAnimator
{
    [Header("Melee Attack")]
    public AttackAnimationInfo meleeAttack;
    private int meleeAttackTriggerIndex = 0;

    [Header("Revolver Attack")]
    public string revolverStartTrigger = "RevolverStart";
    public float revolverStartAnimationDuration;
    public AttackAnimationInfo revolverAttack;

    [Header("Sting Attack")]
    public string stingStartTrigger = "StingStart";
    public float stingStartAnimationDuration;
    public AttackAnimationInfo stingEndAnimation;

    [Header("Spin Attack")]
    public string spinStartTrigger = "SpinStart";
    public string spinEndTrigger = "SpinEnd";
    public float spinStartAnimationDuration;
    public float spinEndAnimationDuration;

    [Header("Dash")]
    public AudioClip dashSound;

    [Header("Force Push")]
    public AttackAnimationInfo forcePush;

    [Header("Force Pull")]
    public AttackAnimationInfo forcePull;

    [Header("Death")]
    public string deathTrigger = "Death";

    [Header("Equipment")]
    public Transform meleeWeaponSlot;
    public float weaponScale = 10;

    [Header("Equipment")]
    public string equipMeleeTrigger = "EquipMelee";
    public float equipMeleeModelSwitchDelay = 1 / 3f;
    public float equipMeleeDuration = 11 / 6f;
    private MeleeWeapon meleeWeapon;
    [Space]
    public string inspectWeaponTrigger = "Inspect";
    public float inspectAnimationDuration = 2.5f;


    #region Regular Melee Attack
    // Plays the melee attack animation
    public void MeleeAttack()
    {
        animator.SetTrigger(meleeAttack.attackTriggers[meleeAttackTriggerIndex++]);
        meleeAttackTriggerIndex %= meleeAttack.attackTriggers.Length;
    }

    // Plays the melee attack sound
    public void PlayMeleeAttackSound()
    {
        PlayAttackSound(meleeAttack);
    }
    #endregion

    #region Revolver Attack
    // Plays the revolver attack start animation
    public void RevolverAttackStart()
    {
        animator.SetTrigger(revolverStartTrigger);
    }

    // Plays the revolver attack end animation
    public void RevolverAttackEnd()
    {
        animator.SetTrigger(revolverAttack.attackTriggers[0]);
    }

    // Plays the revolver attack sound
    public void PlayRevolverAttackSound()
    {
        PlayAttackSound(revolverAttack);
    }
    #endregion

    #region Sting
    // Plays the sting start animation
    public void StingAttackStart()
    {
        animator.SetTrigger(stingStartTrigger);
    }

    // Plays the sting end animation
    public void StingAttackEnd()
    {
        animator.SetTrigger(stingEndAnimation.attackTriggers[0]);
    }

    // Plays the sting attack sound
    public void PlayStingAttackSound()
    {
        PlayAttackSound(stingEndAnimation);
    }
    #endregion

    #region Spin
    // Plays the spin start animation
    public void SpinAttackStart()
    {
        animator.SetTrigger(spinStartTrigger);
    }

    // Plays the spin end animation
    public void SpinAttackEnd()
    {
        animator.SetTrigger(spinEndTrigger);
    }
    #endregion

    #region Dash
    public void PlayDashSound(float spacialBlend = 0.66f)
    {
        AudioMixerGroup audioMixerGroup = GameManager.instance.sfxMixer.FindMatchingGroups("Master")[0];
        Utility.PlayAudioClipAtPoint(dashSound, transform.position, transform, spacialBlend, audioMixerGroup);
    }
    #endregion

    #region ForcePush

    // Plays the force push animation
    public void ForcePush()
    {
        animator.SetTrigger(forcePush.attackTriggers[0]);
    }

    // Plays the force push sound
    public void PlayForcePushSound()
    {
        PlayAttackSound(forcePush);
    }

    #endregion

    #region ForcePull

    // Plays the force pull animation
    public void ForcePull()
    {
        animator.SetTrigger(forcePull.attackTriggers[0]);
    }

    // Plays the force pull sound
    public void PlayForcePullSound()
    {
        PlayAttackSound(forcePull);
    }

    #endregion

    #region Equipment

    /// <summary>
    /// Plays melee weapon equip animation
    /// </summary>
    public void EquipMeleeAnimation()
    {
        animator.SetTrigger(equipMeleeTrigger);
    }

    /// <summary>
    /// Deletes all objects from the melee weapon slot
    /// </summary>
    private void ClearMeleeWeapon()
    {
        if (meleeWeaponSlot.childCount > 0)
            Destroy(meleeWeaponSlot.GetChild(0).gameObject);    
    }

    public void SetMeleeWeapon(MeleeWeapon meleeWeapon)
    {
        this.meleeWeapon = meleeWeapon;
    }

    /// <summary>
    /// Instantiates a melee weapon model
    /// </summary>
    public void InstantiateMeleeWeapon()
    {
        ClearMeleeWeapon();
        Instantiate(meleeWeapon.prefab, meleeWeaponSlot).transform.localScale = Vector3.one * weaponScale;
    }

    /// <summary>
    /// Plays weapon inspect animation
    /// </summary>
    public void InspectWeapon()
    {
        animator.SetTrigger(inspectWeaponTrigger);
    }

    #endregion

    // Plays the death animation
    public void PlayDeathAnimation()
    {
        animator.SetTrigger(deathTrigger);
    }

    private void PlayAttackEffect(AttackAnimationInfo attackAnimationInfo)
    {
        if (attackAnimationInfo.attackEffectPrefab == null)
            return;
        Instantiate(attackAnimationInfo.attackEffectPrefab, attackAnimationInfo.damageSource);
    }
}
