using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlySpikes : DeadlyObject {
    public Transform bodyStickTransform;
    private bool isActive = true;
    public AudioSource source;
    public GameObject indicator;

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
        ai.DestroyMovementComponents();
        Damage.SendDamageFeedback(ai.stunGiver, ai, ai.TakeDamage(ai.GetMaxHealth(), ai.stunGiver, ai.stunGiver.Position));

        GameMode.gameModeInstance.OnSpikesKill(this);
        if (indicator)
            Destroy(indicator);
    }

}
