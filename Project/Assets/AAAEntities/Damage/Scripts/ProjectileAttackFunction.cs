using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Attack Function/Projectile")]
public class ProjectileAttackFunction : AttackFunction
{
    [Range(0,15)]
    public uint numberOfProjectiles = 1;
    [Range(0, 90)]
    public float spreadAngle = 0;
    public ProjectileInfo info;
    public override void DoAttackDamage(Character attacker, float damage)
    {
        for (uint i = 0; i < numberOfProjectiles; ++i)
        {
            Vector3 forward = attacker.GetAttackDirection(spreadAngle);

            Projectile.Create(damage, info, attacker.GetAttackSource(), Quaternion.LookRotation(forward), attacker);
        }
            
    }
}
