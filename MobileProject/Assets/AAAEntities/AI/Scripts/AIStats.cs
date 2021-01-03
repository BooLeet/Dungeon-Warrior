using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Stats/AI")]
public class AIStats : CharacterStats
{
    [Header("Attack")]
    public AttackFunction attackFunction;
    public AIDirector.TokenType attackTokenType;
    public float attackDamage = 10;
    
    [Range(0, 100)]
    public float attackDistance = 5f;
    [Space]
    [Range(0, 10)]
    public float attackTokenCooldownTime = 1;
    [Range(0, 20)]
    public uint attacksPerToken = 1;

    [Header("Detection system")]
    [Range(0, 100)]
    public float visibilityDistance = 25f;
    [Range(0, 180)]
    public float visibilityAngle = 90;

    [Range(0, 20)]
    public float rapidMovementSpeed = 5;

    [Space]
    public bool canBeStunned = false;
    [Range(0, 5)]
    public float pushDurationMultiplier = 1;

    [Header("Leveling")]
    public float healthPerLevel = 10;
    public float moveSpeedPerLevel = 0.5f;
    public float attackDamagePerLevel = 5;
}
