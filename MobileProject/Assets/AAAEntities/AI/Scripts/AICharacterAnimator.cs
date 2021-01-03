using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterAnimator : CharacterAnimator 
{
    public AttackAnimationInfo attackAnimation;
    private int attackTriggerIndex = 0;
    [Space]
    public string stunStartTrigger = "StunStart";
    public float stunStartDuration;
    public string stunEndTrigger = "StunEnd";
    public float stunEndDuration;
    [Header("Model Destruction")]
    public DestructableObject destructableObject;
    public float explosionForce = 1000;
    public float explosionRadius = 5;
    
    public void PlayStunStartAnimation()
    {
        animator.SetTrigger(stunStartTrigger);
        animator.ResetTrigger(stunEndTrigger);
    }

    public void PlayStunEndAnimation()
    {
        animator.SetTrigger(stunEndTrigger);
    }

    public void Attack()
    {
        animator.SetTrigger(attackAnimation.attackTriggers[attackTriggerIndex++]);
        attackTriggerIndex %= attackAnimation.attackTriggers.Length;
    }

    public void PlayAttackSound()
    {
        PlayAttackSound(attackAnimation);
    }

    public void DeathEffect(Vector3 explosionPosition,bool sticky)
    {
        destructableObject.Destruct(explosionForce, explosionPosition, explosionRadius, sticky);
    }
}
