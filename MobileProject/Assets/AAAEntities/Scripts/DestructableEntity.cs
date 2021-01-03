using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableEntity : Entity
{
    public float maxHealth;
    public OnHitList onDistruction;

    protected override void DeathEffect()
    {
        onDistruction.Execute(null, Position);
        Destroy(gameObject);
    }

    public override float GetMaxHealth()
    {
        return maxHealth;
    }

    protected override void OnDamageTaken(float rawDamage, Entity damageGiver, Vector3 sourcePosition)
    {
        
    }
}
