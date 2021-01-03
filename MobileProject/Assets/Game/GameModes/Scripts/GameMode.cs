using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
    public static GameMode gameModeInstance;

    private void Awake()
    {
        if (gameModeInstance == null)
            gameModeInstance = this;
        else
            Destroy(gameObject);

        AwakeFunction();
    }

    protected abstract void AwakeFunction();

    public abstract void OnAIKilled(AICharacter character);

    public abstract void OnInteraction(Interactable interactable);

    public abstract void OnPlayerDamaged(float rawDamage);

    public abstract void OnPlayerDeath();

    public abstract void OnSpikesKill(DeadlySpikes spikes);

    public abstract int GetEnemyLevel();

    public abstract string GetGameOverText();
}
