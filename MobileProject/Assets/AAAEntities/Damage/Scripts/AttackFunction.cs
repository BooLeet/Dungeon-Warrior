using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackFunction : ScriptableObject {
    public abstract void DoAttackDamage(Character attacker, float damage);
}
