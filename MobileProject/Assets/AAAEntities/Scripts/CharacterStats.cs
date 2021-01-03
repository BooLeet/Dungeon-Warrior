using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Character/Stats")]
public class CharacterStats : ScriptableObject {
    public float maxHealth = 100;
    public byte alliance = 0;
    public float selfDamageMultiplier = 1;
    [Space]
    public float moveSpeed = 6;

    [Space]
    public float jumpVelocity = 10;
    public float gravityModifier = 3f;

}
