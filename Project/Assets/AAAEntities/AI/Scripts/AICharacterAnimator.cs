using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterAnimator : CharacterAnimator 
{
    public AttackAnimationInfo attackAnimation;
    private int attackTriggerIndex = 0;

    public void Attack()
    {
        animator.SetTrigger(attackAnimation.attackTriggers[attackTriggerIndex++]);
        attackTriggerIndex %= attackAnimation.attackTriggers.Length;
    }

    public void PlayAttackSound()
    {
        PlayAttackSound(attackAnimation);
    }
}
