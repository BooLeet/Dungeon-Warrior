using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterAnimator : CharacterAnimator
{
    [Header("Attack")]
    public AttackAnimationInfo rangeAttack;
    public AttackAnimationInfo meleeAttack;

    [Header("Melee Leap")]
    public string leapStartTrigger = "LeapStart";
    public string leapEndTrigger = "LeapEnd";
    public float leapLandingDuration;
    public AudioClip leapLandSound;
    [Space]
    public Transform leapExplosionDamageSource;
    public GameObject leapLandExplosionPrefab;

    private int rangeAttackTriggerIndex = 0;
    private int meleeAttackTriggerIndex = 0;
    [Header("Dash")]
    public Transform dashEffectSource;
    public GameObject dashMovementEffect;
    public GameObject dashEndEffect;
    public AudioClip dashSound;

    // Plays the range attack animation
    public void RangeAttack()
    {
        animator.SetTrigger(rangeAttack.attackTriggers[rangeAttackTriggerIndex++]);
        rangeAttackTriggerIndex %= rangeAttack.attackTriggers.Length;
    }

    // Plays the melee attack animation
    public void MeleeAttack()
    {
        animator.SetTrigger(meleeAttack.attackTriggers[meleeAttackTriggerIndex++]);
        meleeAttackTriggerIndex %= meleeAttack.attackTriggers.Length;
    }
    

    // Plays the range attack sound
    public void PlayRangeAttackSound()
    {
        PlayAttackSound(rangeAttack);
    }

    // Plays the melee attack sound
    public void PlayMeleeAttackSound()
    {
        PlayAttackSound(meleeAttack);
    }

    #region Leap
    // Plays leap start animation
    public void LeapStart()
    {
        animator.SetTrigger(leapStartTrigger);
    }

    // Plays leap end animation
    public void LeapEnd()
    {
        animator.SetTrigger(leapEndTrigger);
    }

    // Play leap explosion effect
    public void PlayLeapExplosion()
    {
        Instantiate(leapLandExplosionPrefab, leapExplosionDamageSource.position, Quaternion.identity);
    }

    // Plays the leap landing sound
    public void PlayLeapLandSound(float spacialBlend = 0.66f)
    {
        Utility.PlayAudioClipAtPoint(leapLandSound, transform.position, null, spacialBlend);
    }
    #endregion

    #region Dash
    public void PlayDashSound(float spacialBlend = 0.66f)
    {
        Utility.PlayAudioClipAtPoint(dashSound, transform.position, transform, spacialBlend);
    }

    public void PlayDashMovementEffect(Vector3 dashDirection,float duration)
    {
        Instantiate(dashMovementEffect, dashEffectSource.position + dashDirection, Quaternion.LookRotation(dashDirection),dashEffectSource).AddComponent<DestroyOnTime>().delay = duration;
    }

    public void PlayDashEndEffect(Vector3 position, Vector3 dashDirection)
    {
        Instantiate(dashEndEffect, position, Quaternion.LookRotation(dashDirection));
    }
    #endregion

    public void PlayRangeAttackEffect()
    {
        PlayAttackEffect(rangeAttack);
    }

    private void PlayAttackEffect(AttackAnimationInfo attackAnimationInfo)
    {
        if (attackAnimationInfo.attackEffectPrefab == null)
            return;
        Instantiate(attackAnimationInfo.attackEffectPrefab, attackAnimationInfo.damageSource);
    }
}
