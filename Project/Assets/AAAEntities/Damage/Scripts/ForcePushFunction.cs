using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Attack Function/Force Push")]
public class ForcePushFunction : AttackFunction
{
    public int enemyLayer = 9;
    [Range(0, 30)]
    public float pushDuration = 5;
    [Range(0, 50)]
    public float pushSpeed = 30;
    [Range(0, 30)]
    public float stunDuration = 3;
    [Range(0, 30)]
    public int enemiesPerHit = 4;
    [Range(0,30)]
    public float attackRange = 3f;
    [Range(0, 50)]
    public float deadlyObjectRange = 30f;
    [Range(0,360)]
    public float angle = 90;

    public override void DoAttackDamage(Character attacker, float damage)
    {
        Damage.ForcePush(attacker.GetAttackSource(), attacker.GetAttackDirection(0), attackRange, deadlyObjectRange, angle, attacker, enemiesPerHit, stunDuration, pushDuration, pushSpeed, enemyLayer);
    }
}
