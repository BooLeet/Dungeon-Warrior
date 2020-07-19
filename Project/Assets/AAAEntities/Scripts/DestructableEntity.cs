using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableEntity : Entity
{
    public OnHitList onDistruction;

    protected override void DeathEffect()
    {
        onDistruction.Execute(null, Position);
        Destroy(gameObject);
    }

    protected override void OnDamageTaken(float rawDamage, Entity damageGiver, Vector3 sourcePosition)
    {
        
    }
}
