using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_FSM
{
    public abstract class AI_FSM_State
    {
        public abstract void Init(AICharacter character);
        public abstract void Action(AICharacter character);
        public abstract AI_FSM_State Transition(AICharacter character);
    }

    public class PatrolState : AI_FSM_State
    {
        public override void Action(AICharacter character)
        {
            
        }

        public override void Init(AICharacter character)
        {
            
        }

        public override AI_FSM_State Transition(AICharacter character)
        {
            if (character.IsAlert)
                return new AlertState();
            return null;
        }
    }

    public class AlertState : AI_FSM_State
    {
        public override void Action(AICharacter character)
        {
            character.LookAtEnemy();
            character.RapidMovement();
        }

        public override void Init(AICharacter character)
        {

        }

        public override AI_FSM_State Transition(AICharacter character)
        {
            if (!character.IsAlert)
                return new PatrolState();

            character.RequestAttackToken();
            if (character.HasAttackToken)
            {
                if (character.EnemyIsVisible)
                    return new AproachState();
                else
                    return new FindEnemyState();
            }
            return null;
        }
    }


    public class FindEnemyState : AI_FSM_State
    {
        private float secondsVisible = 0;
        private float maxSecondsVisible = 0.33f;

        public override void Action(AICharacter character)
        {
            character.FollowEnemy(1.5f, true);
            if (character.EnemyIsVisible)
                secondsVisible += Time.deltaTime;
            else
                secondsVisible = 0;
        }

        public override void Init(AICharacter character)
        {
            character.IsWalking = true;
        }

        public override AI_FSM_State Transition(AICharacter character)
        {
            if (secondsVisible >= maxSecondsVisible)
                return new AproachState();
            
            if (!character.HasAttackToken)
            {
                character.IsWalking = false;
                return new AlertState();
            }
            return null;
        }
    }

    public class AproachState : AI_FSM_State
    {
        private float epsilon = 0.5f;

        public override void Action(AICharacter character)
        {
            character.IsWalking = character.DistanceToEnemy > character.aiStats.attackDistance + epsilon;
            character.FollowEnemy(character.aiStats.attackDistance, true);
        }

        public override void Init(AICharacter character)
        {

        }

        public override AI_FSM_State Transition(AICharacter character)
        {
            if (character.DistanceToEnemy <= character.aiStats.attackDistance + epsilon)
            {
                //aICharacter.StopMovement();

                if (!character.EnemyIsVisible) // If enemy went out of sight
                    return new FindEnemyState();

                if (character.CanAttack)
                    return new AttackState();

                if (!character.HasAttackToken)
                {
                    character.IsWalking = false;
                    return new AlertState();
                }
            }

            return null;
        }
    }


    public class AttackState : AI_FSM_State
    {
        public override void Action(AICharacter character)
        {
            character.LookAtEnemy();
        }

        public override void Init(AICharacter character)
        {
            character.IsWalking = false;
            character.AIAttack();
        }

        public override AI_FSM_State Transition(AICharacter character)
        {
            if (character.CanAttack)
                return new PatrolState();
            return null;
        }
    }

    public class StunnedState : AI_FSM_State
    {
        public override void Action(AICharacter character)
        {

        }

        public override void Init(AICharacter character)
        {
            
        }

        public override AI_FSM_State Transition(AICharacter character)
        {
            if (!character.IsStunned)
                return new AlertState();
            return null;
        }
    }
}


