using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Attack Function/Melee")]
public class MeleeAttackFunction : AttackFunction
{
    [Range(0, 30)]
    public int enemiesPerHit = 4;
    [Range(0,30)]
    public float attackRange = 3f;
    [Range(0, 10)]
    public float coneOffset = 0;
    [Range(0,360)]
    public float angle = 90;
    

    public override void DoAttackDamage(Character attacker, float damage)
    {
        Damage.MeleeDamage(attacker.GetAttackSource(), attacker.GetAttackDirection(0), damage, attackRange, coneOffset, angle, attacker, enemiesPerHit);
    }
}
