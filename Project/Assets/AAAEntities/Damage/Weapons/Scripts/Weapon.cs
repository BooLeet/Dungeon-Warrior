using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Weapon")]
public class Weapon : ScriptableObject {
    [Header("Weapon presentation")]
    public GameObject viewModelPrefab;
    public float knockBackAngle = 0;
    [Header("Ammunition")]
    public CharacterAmmunition.AmmoType ammoType;
    public uint ammoPerShot = 1;
    [Header("Damage")]
    public float baseDamage = 10;
    public AttackFunction attackFunction;
    

    public void DoAttackDamage(Character attacker)
    {
        attackFunction.DoAttackDamage(attacker, baseDamage);
    }
}
