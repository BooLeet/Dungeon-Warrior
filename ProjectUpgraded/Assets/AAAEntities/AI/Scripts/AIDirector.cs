using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIDirector : MonoBehaviour {
    private static AIDirector instance;
    private List<AICharacter> aICharacters = new List<AICharacter>();

    public enum TokenType { GruntMelee,GruntRange,HeavyMelee,HeavyRange}
    [System.Serializable]
    public class TokenContainer
    {
        public TokenType tokenType;
        [Range(1,10)]
        public int maxTokens = 1;
        private int currentTokens = 0;

        public void RefilTokens()
        {
            currentTokens = maxTokens;
        }

        public bool TokenIsAvailable()
        {
            return currentTokens > 0;
        }

        public bool GetToken()
        {
            if (currentTokens == 0)
                return false;
            --currentTokens;
            return true;
        }

        public void ReturnToken()
        {
            currentTokens++;
            if (currentTokens > maxTokens)
                currentTokens = maxTokens;
        }
    }

    public TokenContainer[] tokens;
    
    void Awake()
    {
        if (instance)
            Destroy(this);
        instance = this;
        RefillTokens();
    }

    public void RefillTokens()
    {
        foreach (TokenContainer container in tokens)
            container.RefilTokens();
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
        TokenContainer container = (from token in tokens
                                    where token.tokenType == aICharacter.aiStats.attackTokenType
                                    select token).First();

        if (!container.TokenIsAvailable())
            return false;

        var aiWithoutTokens = from ai in aICharacters
                              where ai.aiStats.attackTokenType == aICharacter.aiStats.attackTokenType 
                              &&!ai.HasAttackToken 
                              && ai.IsAlert
                              select ai;

        float minDistance = aiWithoutTokens.Min(x => x.DistanceToEnemy);

        if(aICharacter.DistanceToEnemy == minDistance)
        {
            container.GetToken();
            return true;
        }
        return false;
    }

    public void ReturnAttackToken(AICharacter aICharacter)
    {
        TokenContainer container = (from token in tokens
                                    where token.tokenType == aICharacter.aiStats.attackTokenType
                                    select token).First();
        container.ReturnToken();
    }

    public void Alarm(Vector3 position,Character enemyCharacter)
    {
        foreach (AICharacter ai in aICharacters)
            if (!ai.IsAlert && Vector3.Distance(position, ai.Position) <= ai.aiStats.visibilityDistance
                && Utility.IsVisible(position, ai.gameObject, ai.aiStats.visibilityDistance,ai.verticalTargetingOffset))
                ai.Alarm(enemyCharacter);
                
    }
}
