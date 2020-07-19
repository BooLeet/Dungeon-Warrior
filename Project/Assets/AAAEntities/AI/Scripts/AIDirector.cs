using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIDirector : MonoBehaviour {
    private static AIDirector instance;
    private List<AICharacter> aICharacters = new List<AICharacter>();

    public int maxTokens = 5;
    private int tokensAvailable;
    
    void Awake()
    {
        if (instance)
            Destroy(this);
        instance = this;
        RefillTokens();
    }

    public void RefillTokens()
    {
        tokensAvailable = maxTokens;
    }

    public static AIDirector GetInstance()
    {
        if (instance)
            return instance;

        instance = new GameObject("AI Director").AddComponent<AIDirector>();
        instance.RefillTokens();
        return instance;
    }


    public void Register(AICharacter aICharacter)
    {
        aICharacters.Add(aICharacter);
    }

    public void Unregister(AICharacter aICharacter)
    {
        aICharacters.Remove(aICharacter);
    }

    public bool RequestAttackToken(AICharacter aICharacter)
    {
        if (tokensAvailable == 0)
            return false;

        var aiWithoutTokens = from ai in aICharacters
                              where !ai.HasAttackToken && ai.IsAlert
                              select ai;

        float minDistance = aiWithoutTokens.Min(x => x.DistanceToEnemy);

        if(aICharacter.DistanceToEnemy == minDistance)
        {
            --tokensAvailable;
            return true;
        }
        return false;
    }

    public void ReturnAttackToken()
    {
        if (tokensAvailable < maxTokens)
            ++tokensAvailable;
    }

    public void Alarm(Vector3 position,Character enemyCharacter)
    {
        foreach (AICharacter ai in aICharacters)
            if (!ai.IsAlert && Vector3.Distance(position, ai.Position) <= ai.aiStats.visibilityDistance && Utility.IsVisible(position, ai.gameObject, ai.aiStats.visibilityDistance)) 
                ai.Alarm(enemyCharacter);
    }
}
