using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlySpikes : DeadlyObject {
    public Transform bodyStickTransform;
    private bool isActive = true;
    public AudioSource source;
    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;

        AICharacter ai = other.GetComponent<AICharacter>();
        if (!ai || !ai.IsStunned)
            return;

        source.Play();
        isActive = false;
        DeadlyObjectRegistry.GetInstance().Unregister(this);
        ai.transform.position = bodyStickTransform.position;
        ai.StickToSurfaceOnDeath = true;
        Destroy(ai.navAgent);
        Destroy(ai.controller);
        Damage.SendDamageFeedback(ai.stunGiver, ai, ai.TakeDamage(ai.maxHealth, ai.stunGiver, ai.stunGiver.Position));
    }

}
