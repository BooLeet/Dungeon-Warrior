using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour {
    public float verticalTargetingOffset;
    public Vector3 Position { get { return transform.position + transform.up * verticalTargetingOffset; } }

    public float maxHealth = 100;
    [Range(0,1)]
    public float selfDamageMultiplier = 0.2f;
    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsInvincible { get; protected set; }

    public enum DamageOutcome { Hit,Death,Invincible}

    void Awake()
    {
        CurrentHealth = maxHealth;
        EntityRegistry.GetInstance().Register(this);
    }

    // Use it for accounting for damage resistance
    protected virtual float RecalculateRawDamage(float rawDamage,Entity damageGiver) { return rawDamage; }

    public DamageOutcome TakeDamage(float rawDamage,Entity damageGiver, Vector3 sourcePosition)
    {
        if(IsInvincible)
            return DamageOutcome.Invincible;
        if (IsDead)
            return DamageOutcome.Death;
        if (damageGiver == this)
            rawDamage *= selfDamageMultiplier;
        CurrentHealth -= RecalculateRawDamage(rawDamage,damageGiver);
        OnDamageTaken(rawDamage, damageGiver, sourcePosition); 
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            IsDead = true;
            StartCoroutine(DeathRoutine());
            return DamageOutcome.Death;
        }
        return DamageOutcome.Hit;

    }

    // Adds a given amount of health to the entity (negative amount will be ignored)
    public void GiveHealth(float amount)
    {
        if (amount < 0)
            return;

        CurrentHealth += amount;
        if (CurrentHealth > maxHealth)
            CurrentHealth = maxHealth;
    }

    protected abstract void OnDamageTaken(float rawDamage, Entity damageGiver, Vector3 sourcePosition);
    protected abstract void DeathEffect();

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForEndOfFrame();
        EntityRegistry.GetInstance().Unregister(this);
        DeathEffect();
    }

}
