using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OnHitList {
    public OnHit[] onHitFunctors;

    public void Execute(Character attacker, Vector3 hitPoint)
    {
        if (onHitFunctors == null)
            return;
        foreach (OnHit onHit in onHitFunctors) 
        {
            onHit.Execute(attacker,hitPoint);
        }
    }
}
