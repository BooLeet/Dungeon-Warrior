using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/AI/Stats")]
public class AIStats : ScriptableObject {
    public AttackFunction attackFunction;
    public AIDirector.TokenType attackTokenType;
    public float attackDamage = 10;
    [Space]
    [Range(0, 100)]
    public float visibilityDistance = 25f;
    [Range(0, 100)]
    public float attackDistance = 5f;
    [Range(0, 20)]
    public float rapidMovementSpeed = 5;
    
    [Space]
    [Range(0, 180)]
    public float visibilityAngle = 90;
    [Space]
    [Range(0, 10)]
    public float attackTokenCooldownTime = 1;
    [Range(0, 20)]
    public uint attacksPerToken = 1;
    [Space]
    public bool canBeStunned = false;
}
