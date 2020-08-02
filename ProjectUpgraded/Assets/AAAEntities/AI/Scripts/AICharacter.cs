using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AI_FSM;

public class AICharacter : Character {
    public bool HasAttackToken { get; private set; }
    public bool EnemyIsVisible { get; private set; }
    public float DistanceToEnemy { get; private set; }
    public bool IsAlert { get; private set; }
    public bool CanAttack { get; private set; }
    public bool IsStunned { get; private set; }
    private float stunCurrentTime;
    public bool StickToSurfaceOnDeath { get; set; }
    public bool IsWalking { get; set; }

    public AICharacterAnimator characterAnimator;
    public Pullable pullableBehaviour;
    private Entity lastDamageGiver;
    public Entity stunGiver { get; private set; }

    private AIDirector director;
    private Character currentEnemy;
    private AI_FSM_State currentState;

    private bool canAttack = true;

    [Header("AI Stats")]
    public AIStats aiStats;

    private float attackTokenCooldown;
    private bool attackCooledDown = true;
    private uint attacksPerTokenLeft;

    [Space]
    public string currentStateName;

    private Vector2 rapidMovementVector;
    private Vector2 targetRapidMovementVector;

    protected override void ControllerStart()
    {
        currentState = new PatrolState();
        director = AIDirector.GetInstance();
        director.Register(this);

        StartCoroutine(RapidMovementVectorGeneration());
    }

    protected override void ControllerUpdate()
    {
        if (!IsDead)
        {
            AttackTokenCooldownUpdate();
            UpdateCurrentEnemy();
            PropertiesUpdate();
            RapidMovementVectorSmoothing();
            StunUpdate();
            StateMachine();

            if (canAttack)
                characterAnimator.WalkingAnimation(IsWalking && canAttack && IsNearGround());

            Gravity();
            ApplyMovement();
        }
    }

    #region AI functions
    private void StateMachine()
    {
        currentState.Action(this);
        AI_FSM_State newState;

        if (IsStunned && (currentState as StunnedState) == null)
            newState = new StunnedState();
        else
            newState = currentState.Transition(this);

        if (newState != null)
        {
            currentState = newState;
            currentState.Init(this);
        }

        currentStateName = currentState.ToString();
    }
    private void PropertiesUpdate()
    {
        CanAttack = canAttack && attackTokenCooldown == 0;
        if(IsAlert)
            IsAlert = currentEnemy && !currentEnemy.IsDead;

        if (currentEnemy)
        {
            int layerMask = 1 << gameObject.layer;
            layerMask = ~layerMask;
            EnemyIsVisible = Utility.WithinAngle(head.position,head.forward, currentEnemy.gameObject.transform.position + Vector3.up * currentEnemy.verticalTargetingOffset,aiStats.visibilityAngle)
                          && Utility.IsVisible(head.position, currentEnemy.gameObject, aiStats.visibilityDistance, currentEnemy.verticalTargetingOffset, layerMask);
            DistanceToEnemy = Vector3.Distance(Position, currentEnemy.Position);
        }

    }
    private void UpdateCurrentEnemy()
    {
        if (currentEnemy && !currentEnemy.IsDead)
            return;

        var closestVisibleEnemyCharacters = from entity in EntityRegistry.GetInstance().GetClosestEntities(Position, aiStats.visibilityDistance, this)
                                            where entity as Character &&
                                            (entity as Character).stats.alliance != stats.alliance &&
                                            Utility.IsVisible(head.position, entity.gameObject, aiStats.visibilityDistance, entity.verticalTargetingOffset) &&
                                            Utility.WithinAngle(head.position, head.forward, entity.gameObject.transform.position + Vector3.up * entity.verticalTargetingOffset, aiStats.visibilityAngle)
                                            select new { character = entity as Character, distance = Vector3.Distance(Position, entity.Position) };

        if (closestVisibleEnemyCharacters.Count() == 0)
            return;

        var closestEnemyCharacter = from x in closestVisibleEnemyCharacters
                                    where x.distance == closestVisibleEnemyCharacters.Min(y => y.distance)
                                    select x.character;
        currentEnemy = closestEnemyCharacter.First();
        if (!IsAlert)
            Alarm(currentEnemy);
    }

    public void FollowEnemy(float stoppingDistance, bool lookAtEnemy)
    {
        if (!currentEnemy)
            return;
        
        Ray ray = new Ray(currentEnemy.Position, Vector3.down);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray,out hitInfo,float.MaxValue))
            Follow(hitInfo.point, stoppingDistance, lookAtEnemy);
            
    }

    public void Alarm()
    {
        IsAlert = true;
        director.Alarm(Position, currentEnemy);
    }

    public void Alarm(Character enemyCharacter)
    {
        currentEnemy = enemyCharacter;
        Alarm();
    }

    public void LookAtEnemy()
    {
        if (currentEnemy)
            LookAt(currentEnemy.Position);
    }

    private void Attack()
    {
        if (!canAttack)
            return;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        if (characterAnimator.attackAnimation.attackDamageDelay > characterAnimator.attackAnimation.attackDuration)
            Debug.LogError("attackDamageDelay > attackDuration isn't permitted");

        canAttack = false;
        characterAnimator.Attack();
        yield return new WaitForSeconds(characterAnimator.attackAnimation.attackDamageDelay);
        if (!IsStunned)
        {
            aiStats.attackFunction.DoAttackDamage(this, aiStats.attackDamage);

            yield return new WaitForSeconds(characterAnimator.attackAnimation.attackDuration - characterAnimator.attackAnimation.attackDamageDelay);
            canAttack = true;
        }
        
    }

    // Makes character to do an attack and starts the attack token cooldown
    public void AIAttack()
    {
        SpendAttackToken();
        Attack();
    }

    #endregion

    #region Attack Tokens
    private void AttackTokenCooldownUpdate()
    {
        if (attackTokenCooldown > 0)
            attackTokenCooldown -= Time.deltaTime;
        else if (!attackCooledDown)
        {
            attackCooledDown = true;
            attackTokenCooldown = 0;
            ReturnAttackToken();
        }
    }

    public void RequestAttackToken()
    {
        if (HasAttackToken)
            return;
        if (attackTokenCooldown == 0)
        {
            attacksPerTokenLeft = aiStats.attacksPerToken;
            HasAttackToken = director.RequestAttackToken(this);
        }
    }

    public void ReturnAttackToken()
    {
        HasAttackToken = false;
        director.ReturnAttackToken(this);
    }

    private void SpendAttackToken()
    {
        --attacksPerTokenLeft;
        if (attacksPerTokenLeft == 0)
        {
            attackCooledDown = false;
            attackTokenCooldown = aiStats.attackTokenCooldownTime;
        }

    }

    #endregion

    #region Rapid Movement
    private void RapidMovementVectorSmoothing()
    {
        rapidMovementVector = Vector2.Lerp(rapidMovementVector, targetRapidMovementVector, Time.deltaTime * 5);
    }

    private IEnumerator RapidMovementVectorGeneration()
    {
        while (!IsDead)
        {
            targetRapidMovementVector = Random.insideUnitCircle;
            yield return new WaitForSeconds(1);
        }
    }

    public void RapidMovement()
    {
        ManualMovement(rapidMovementVector * aiStats.rapidMovementSpeed, transform.forward, transform.right);
    }

    #endregion

    #region Stun

    public void Stun(float duration, Entity stunGiver)
    {
        //if (IsStunned)
        //    return;
        this.stunGiver = stunGiver;
        stunCurrentTime = Mathf.Max(stunCurrentTime, duration);
        //StartCoroutine(StunRoutine(duration));
    }

    private void StunUpdate()
    {
        if(stunCurrentTime > 0)
        {
            if(!IsStunned) 
                characterAnimator.PlayStunStartAnimation();
            IsStunned = true;
            canAttack = false;
            stunCurrentTime -= Time.deltaTime;
        }
        else
        {
            stunCurrentTime = 0;
            if(IsStunned)
                StartCoroutine(StunRecoveryRoutine());
        }

    }

    private IEnumerator StunRecoveryRoutine()
    {
        characterAnimator.PlayStunEndAnimation();
        yield return new WaitForSeconds(characterAnimator.stunEndDuration);
        canAttack = true;
        IsStunned = false;
    }

    private IEnumerator StunRoutine(float duration)
    {
        IsStunned = true;
        canAttack = false;
        characterAnimator.PlayStunStartAnimation();
        yield return new WaitForSeconds(duration + characterAnimator.stunStartDuration);
        characterAnimator.PlayStunEndAnimation();
        yield return new WaitForSeconds(characterAnimator.stunEndDuration);
        canAttack = true;
        IsStunned = false;
    }

    #endregion

    #region Overrides
    

    public override Vector3 GetAttackDirection(float spreadAngleDeg)
    {
        Vector3 insideUnitSphere = Random.insideUnitSphere * spreadAngleDeg / 90;

        return insideUnitSphere + (currentEnemy ? (currentEnemy.Position - characterAnimator.attackAnimation.damageSource.position).normalized : transform.forward);
    }

    public override Vector3 GetAttackSource()
    {
        return characterAnimator.attackAnimation.damageSource.position;
    }

    

    protected override void DeathEffect()
    {
        if (HasAttackToken)
            director.ReturnAttackToken(this);
        director.Unregister(this);
        if(pullableBehaviour)
            pullableBehaviour.Unregister();
        Destroy(controller);
        characterAnimator.DeathEffect(lastDamageGiver ? Position + 4 * (lastDamageGiver.Position - Position).normalized : Position, StickToSurfaceOnDeath);
        Destroy(gameObject);
    }

    protected override void OnDamageTaken(float rawDamage, Entity damageGiver, Vector3 sourcePosition)
    {
        characterAnimator.PlayDamageEffect();
        lastDamageGiver = damageGiver;

        if (currentEnemy == null && damageGiver as Character)
            Alarm(damageGiver as Character);
    }
    #endregion

}
