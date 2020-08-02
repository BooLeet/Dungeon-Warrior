using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnHit : ScriptableObject {

    public abstract void Execute(Character attacker,Vector3 hitPoint);
}
