﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAnimator : MonoBehaviour 
{
    [System.Serializable]
    public class AttackAnimationInfo
    {
        public AudioClip attackSound;
        public string[] attackTriggers;
        public float attackDamageDelay;
        public float attackDuration;
        public Transform damageSource;
        public GameObject attackEffectPrefab;
    }

    public Animator animator;

    public string idleTrigger = "Idle", walkTrigger = "Walk";
    private bool wasWalking = false;

    // Handles walking animation
    public void WalkingAnimation(bool isWalking)
    {
        if (isWalking && !wasWalking)
        {
            animator.ResetTrigger(idleTrigger);
            animator.SetTrigger(walkTrigger);
        }
        else if (!isWalking && wasWalking)
        {
            animator.ResetTrigger(walkTrigger);
            animator.SetTrigger(idleTrigger);
        }

        wasWalking = isWalking;
    }

    public void ResetWalkingAnimation()
    {
        animator.ResetTrigger(idleTrigger);
        animator.ResetTrigger(walkTrigger);
        wasWalking = false;
    }

    // Plays the idle animation
    public void Idle()
    {
        animator.SetTrigger(idleTrigger);
    }

    // Plays the walking animation
    public void Walk()
    {
        animator.SetTrigger(walkTrigger);
    }

    // Plays an attack sound
    protected void PlayAttackSound(AttackAnimationInfo attackAnimationInfo, float spacialBlend = 0.66f)
    {
        Utility.PlayAudioClipAtPoint(attackAnimationInfo.attackSound, attackAnimationInfo.damageSource.position, attackAnimationInfo.damageSource, spacialBlend);
    }
}