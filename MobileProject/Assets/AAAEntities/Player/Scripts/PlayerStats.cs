using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Stats/Player")]
public class PlayerStats : CharacterStats
{
    public MeleeWeapon startingMeleeWeapon;
    public float targetingRaycastRange = 25;

    [Header("Interaction")]
    public float interactionDistance = 5;
    public float interactionAngle = 110;

    [Header("Aim Assist")]
    public bool enableAimAssist = false;
    public float aimAssistAngle = 30;
    public float aimAssistDistance = 70;

    [Header("Melee Attack")]
    public AttackFunction meleeAttackFunction;
    public float meleeAttackDamage = 20;
    public float meleeAttackHeadKnockBackAngle = 0;

    [Header("Revolver Attack")]
    public AttackFunction revolverAttackFunction;
    public float revolverAttackDamage = 50;
    public float revolverAttackHeadKnockBackAngle = 2;
    public Vector2 revolverAttackDmgMultiplier = new Vector2(1f, 2.5f);
    public float revolverAttackChargeTime = 1;
    public LootScriptable revolverResource;

    [Header("Sting Attack")]
    public AttackFunction stingAttackFunction;
    public Vector2 stingAttackDmgMultiplier = new Vector2(1.3f, 2.5f);
    public float stingAttackHeadKnockBackAngle = 2;
    public float stingAttackChargeTime = 1;

    [Header("Spin Attack")]
    public float spinAttackRange = 12;
    public Vector2 spinAttackDmgMultiplier = new Vector2(1.3f, 2.5f);
    public float spinAttackChargeTime = 1;

    [Header("Mana")]
    public float maxMana = 100;
    public float manaRegenPerSecond = 10;

    [Header("Force Push")]
    public AttackFunction forcePushAttackFunction;
    public float forcePushManaCost = 40;

    [Header("Dash")]
    public float dashSpeed = 45;
    public float dashDuration = 0.15f;
    public float dashCapacity = 1.25f;
    public float dashCooldownTime = 0.75f;

    //[Header("Upgrades")]

}
