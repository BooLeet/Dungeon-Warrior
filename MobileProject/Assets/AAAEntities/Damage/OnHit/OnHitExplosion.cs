using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/OnHit")]
public class OnHitExplosion : OnHit
{
    public GameObject explosionEffectPrefab;
    public float explosionDamage = 100;
    public float explosionRange = 10;

    public override void Execute(Character attacker, Vector3 hitPoint)
    {
        Instantiate(explosionEffectPrefab, hitPoint, Quaternion.identity);
        Damage.ExplosiveDamage(hitPoint, explosionDamage, explosionRange, attacker);
    }
}
