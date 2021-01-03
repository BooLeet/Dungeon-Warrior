using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Attack Function/Hitscan")]
public class HitscanAttackFunction : AttackFunction
{
    [Range(0,300)]
    public float range = 30;
    public float spreadAngleDeg = 0;

    public override void DoAttackDamage(Character attacker, float damage)
    {
        Damage.HitscanDamage(attacker.GetAttackSource(), attacker.GetAttackDirection(spreadAngleDeg), damage, range, attacker);
    }
}
