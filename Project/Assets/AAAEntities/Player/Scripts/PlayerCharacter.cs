using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PlayerFSM;

public class PlayerCharacter : Character
{
    public static PlayerCharacter instance;

    public PlayerInput input;
    public PlayerCharacterAnimator animator;
    public PlayerCamera playerCamera;
    public HUD hud;
    //public PlayerViewmodelAnimator viewmodelAnimator;

    [Header("Rotation")]
    private bool enableManualRotation = true;
    public float cameraSensitivity = 1;
    public float verticalRotationSpeedMultiplier = 1;
    [Range(0, 90)]
    public float maxVerticalRotationAngle = 90;
    private float verticalAimAngle = 0;
    public float knockBackLerpParameter = 5;
    private float knockBackVerticalAngle = 0;

    [Header("Attack")]
    public AttackFunction meleeAttackFunction;
    public float meleeAttackDamage = 30;
    public float meleeAttackHeadKnockBackAngle = 2;
    [Space]
    public float targetingRaycastRange = 25;
    public float SpecialAttackMeter { get; set; }

    [Header("Damage bonus mechanic")]
    public float damageBonusMaxMultiplier = 5f;
    public float DamageBonusCurrentMultiplier { get; private set; } 
    public float damageBonusIncrement = 0.5f;
    public int damageBonusFullIncrementHitCount = 3;
    public int damageBonusHalfIncrementHitCount = 5;
    [Space]
    public float damageBonusDecayPerSecond = 0.2f;
    public float damageBonusDecayDelay = 1.5f;
    private float damageBonusTimeCounter = 0;
    public enum AttackType { Basic, Sting, Spin, Null}
    private AttackType currentAttackType = AttackType.Basic;
    private AttackType previousAttackType = AttackType.Basic;
    private int sameAttackTypeCount = 0;
    
    [Header("Sting Attack")]
    public AttackFunction stingAttackFunction;
    public Vector2 stingAttackDmgMultiplier = new Vector2(1.5f,4f);
    public float stingAttackHeadKnockBackAngle = 2;
    public float stingAttackChargeTime = 3;

    [Header("Spin Attack")]
    public float spinAttackRange = 10;
    public Vector2 spinAttackDmgMultiplier = new Vector2(1.5f, 4f);
    public float spinAttackChargeTime = 3;


    [Header("Aim Assist")]
    public bool enableAimAssist = true;
    public float aimAssistAngle = 45;
    public float aimAssistDistance = 40;
    public Entity AimAssistedEntity { get; private set; }

    public bool CanAttack { get; private set; }
    private float attackAnimationTimeLeft = 0;
    public bool AttackAnimationFinished { get { return attackAnimationTimeLeft <= 0; } }
    private Transform attackSource;

    public bool EnableInput { get; set; }
    [Header("Magica")]
    public float maxMagica = 100;
    public float CurrentMagica { get; private set; }

    [Header("Force Push")]
    public AttackFunction forcePushAttackFunction;

    [Header("Interaction And Pulling")]
    public float interactionDistance = 5;
    public float interactionAngle = 30;
    [Space]
    public float pullingDistance = 20;
    public float pullingSpeed = 30;
    public float pullingStunTime = 3;
    public Interactable CurrentInteractable;// { get; private set; }
    

    [Header("Dash")]
    public float dashSpeed = 30;
    public float dashDuration = 0.3f;
    public byte dashCapacity = 2;
    public float dashCooldownTime = 1;
    [Range(60, 90)]
    public float dashInversionMinAngle = 70;
    public float DashMeter { get; private set; }


    //FSM
    private PlayerState currentMovementState;
    private PlayerState currentCombatState;
    public string currentMovementStateName;
    public string currentCombatStateName;

    protected override void ControllerStart()
    {
        if (instance)
            Destroy(gameObject);
        instance = this;

        Utility.DisbleCursor();
        EnableInput = true;
        CanAttack = true;

        CurrentMagica = maxMagica;

        DashMeter = dashCapacity;
        DamageBonusCurrentMultiplier = 1;

        currentMovementState = new DirectMovementState();
        currentCombatState = new BasicAttackState();
    }

    protected override void ControllerUpdate()
    {
        UpdateInteractable();
        StateMachine(ref currentMovementState);
        StateMachine(ref currentCombatState);
        currentMovementStateName = currentMovementState.ToString();
        currentCombatStateName = currentCombatState.ToString();

        DamageBonusDecayHandler();
        AttackAnimationFinishedUpdate();
        UpdateAimAssist();
        HeadKnockBackUpdate();
        DashCooldown();
        playerCamera.UpdateCamera();
    }

    private void StateMachine(ref PlayerState currentState)
    {
        currentState.Action(this);
        PlayerState nextState = currentState.Transition(this);
        if (nextState != null)
        {
            currentState = nextState;
            nextState.Init(this);
        }
    }

    #region Misc Public Methods

    public void DeduceDashMeter()
    {
        --DashMeter;
    }
    #endregion

    #region Rotation
    // Standard FPS rotation 
    public void ManualRotation(Vector2 rotationVector)
    {
        if (!enableManualRotation)
            return;

        float vertical = rotationVector.y * cameraSensitivity * verticalRotationSpeedMultiplier;
        verticalAimAngle = Mathf.Clamp(verticalAimAngle - vertical, -maxVerticalRotationAngle, maxVerticalRotationAngle);
        float angle = Mathf.Clamp(verticalAimAngle - knockBackVerticalAngle, -maxVerticalRotationAngle, maxVerticalRotationAngle);
        head.localRotation = Quaternion.Euler(angle, 0, 0);
        //playerCamera.angleV = angle;


        float horizontal = rotationVector.x * cameraSensitivity;
        //playerCamera.angleU = (playerCamera.angleU - horizontal) % 360;
        transform.Rotate(new Vector3(0, horizontal, 0));
    }

    #endregion

    #region Attack
    public void MeleeAttack()
    {
        if (!CanAttack)
            return;
        attackSource = animator.meleeAttack.damageSource;

        currentAttackType = AttackType.Basic;
        StartCoroutine(AttackRoutine(animator.meleeAttack.attackDamageDelay, animator.meleeAttack.attackDuration, animator.meleeAttack.attackAnimationDuration, meleeAttackFunction, animator.MeleeAttack, GetDamageBonusMultiplier() * meleeAttackDamage, meleeAttackHeadKnockBackAngle));
    }

    public void ForcePush()
    {
        if (!CanAttack)
            return;
        attackSource = animator.meleeAttack.damageSource;
        
        currentAttackType = AttackType.Basic;
        StartCoroutine(AttackRoutine(animator.forcePush.attackDamageDelay, animator.forcePush.attackDuration, animator.forcePush.attackAnimationDuration, forcePushAttackFunction, animator.ForcePush, GetDamageBonusMultiplier() * meleeAttackDamage, meleeAttackHeadKnockBackAngle));
    }

    private IEnumerator AttackRoutine(float attackDamageDelay, float attackDuration, float attackAnimationDuration, AttackFunction attackFunction, Utility.VoidFunction animationCall, float damage, float headKnockBackAngle = 0)
    {
        if (attackDamageDelay > attackDuration || attackDuration > attackAnimationDuration)
            Debug.LogError("attackDamageDelay > attackDuration isn't permitted");

        if (attackDuration > attackAnimationDuration)
            Debug.LogError("attackDuration > attackAnimationDuration isn't permitted");

        attackAnimationTimeLeft = attackAnimationDuration;
        animator.ResetWalkingAnimation();
        CanAttack = false;
        animationCall();
        yield return new WaitForSeconds(attackDamageDelay);


        PlayerCamera.ScreenShake(0.2f, Position);
        PlayerCamera.Recoil();
        HeadKnockBack(headKnockBackAngle);
        // Do damage
        attackFunction.DoAttackDamage(this, damage);

        yield return new WaitForSeconds(attackDuration - attackDamageDelay);
        CanAttack = true;
    }

    private void AttackAnimationFinishedUpdate()
    {
        attackAnimationTimeLeft -= Time.deltaTime;
    }

    public override Vector3 GetAttackSource()
    {
        return attackSource.position;
    }

    private void UpdateAimAssist()
    {
        AimAssistedEntity = null;
        if (enableAimAssist)
        {
            var enemiesInRange = from entity in EntityRegistry.GetInstance().GetClosestEntities(Position, aimAssistDistance, this)
                                 where Utility.IsVisible(playerCamera.transform.position, entity.gameObject, aimAssistDistance, entity.verticalTargetingOffset)
                                 && Utility.WithinAngle(playerCamera.transform.position, playerCamera.transform.forward, entity.Position, aimAssistAngle)
                                 select new { entity, distanceFromCameraCenter = playerCamera.DistanceFromTheCenter(entity.Position) };

            IEnumerable<Entity> closestEntitiesToCenter = from entityInfo in enemiesInRange
                                                          where entityInfo.distanceFromCameraCenter == enemiesInRange.Min(e => e.distanceFromCameraCenter)
                                                          select entityInfo.entity;

            if (closestEntitiesToCenter.Count() > 0)
                AimAssistedEntity = closestEntitiesToCenter.First();
        }
    }

    public override Vector3 GetAttackDirection(float spreadAngleDeg)
    {
        if (AimAssistedEntity)
        {
            Vector3 insideUnitSphere = Random.insideUnitSphere * spreadAngleDeg / 90;
            return insideUnitSphere + (AimAssistedEntity.Position - attackSource.position).normalized;
        }

        Vector2 insideUnitCircle = Random.insideUnitCircle;
        Transform cameraTransform = playerCamera.transform;
        Vector3 forward = cameraTransform.forward +
                  cameraTransform.right * insideUnitCircle.x * (spreadAngleDeg / 90) +
                  cameraTransform.up * insideUnitCircle.y * (spreadAngleDeg / 90);

        Ray ray = new Ray(cameraTransform.position, forward);
        RaycastHit hitInfo;

        Vector3 hitPosition = cameraTransform.position + forward * targetingRaycastRange;
        if (Physics.Raycast(ray, out hitInfo, targetingRaycastRange))
            hitPosition = hitInfo.point;

        return (hitPosition - GetAttackSource()).normalized;
    }

    public void HeadKnockBack(float angleDeg)
    {
        knockBackVerticalAngle = angleDeg;
    }

    private void HeadKnockBackUpdate()
    {
        knockBackVerticalAngle = Mathf.Lerp(knockBackVerticalAngle, 0, Time.deltaTime * knockBackLerpParameter);
    }


    #endregion

    #region Sting
    public void StingAttackStart()
    {
        CanAttack = false;
        animator.StingAttackStart();
    }

    public void StingAttackEnd()
    {
        attackSource = animator.meleeAttack.damageSource;
        float damageMultiplier = GetDamageBonusMultiplier() * Mathf.Lerp(stingAttackDmgMultiplier.x, stingAttackDmgMultiplier.y, SpecialAttackMeter);
        SpecialAttackMeter = 0;

        currentAttackType = AttackType.Sting;
        StartCoroutine(AttackRoutine(animator.stingEndAnimation.attackDamageDelay, animator.stingEndAnimation.attackDuration, animator.stingEndAnimation.attackAnimationDuration, stingAttackFunction, animator.StingAttackEnd, meleeAttackDamage * damageMultiplier, meleeAttackHeadKnockBackAngle));
    }
    #endregion

    #region Spin
    public void SpinAttackStart()
    {
        CanAttack = false;
        animator.SpinAttackStart();
    }

    public void SpinAttackEnd()
    {
        attackSource = animator.meleeAttack.damageSource;
        float damageMultiplier = GetDamageBonusMultiplier() * Mathf.Lerp(spinAttackDmgMultiplier.x, stingAttackDmgMultiplier.y, SpecialAttackMeter);
        SpecialAttackMeter = 0;

        currentAttackType = AttackType.Spin;
        StartCoroutine(SpinAttackRoutine(meleeAttackDamage * damageMultiplier));
    }

    private IEnumerator SpinAttackRoutine(float damage, float enertiaEffectDuration = 0.5f, float enertiaEffectMagnitude = 0.5f)
    {
        animator.SpinAttackEnd();
        animator.ResetWalkingAnimation();
        float timeCounter = animator.spinEndAnimationDuration;
        verticalAimAngle = 0;
        head.localRotation = Quaternion.identity;
        Vector3 upVector = head.transform.up;
        enableManualRotation = false;
        Entity previousEntity = null;
        attackAnimationTimeLeft = animator.spinEndAnimationDuration + enertiaEffectDuration;

        PlayerCamera.ScreenShake(animator.spinEndAnimationDuration, Position);

        while (timeCounter > 0)
        {
            Ray ray = new Ray(head.position, head.forward);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, spinAttackRange))
            {
                Entity hitEntity = rayHit.collider.GetComponent<Entity>();
                if (hitEntity != null && hitEntity != previousEntity)
                {
                    Damage.SendDamageFeedback(this, hitEntity, hitEntity.TakeDamage(damage, this, attackSource.position));
                    previousEntity = hitEntity;
                }
            }

            head.Rotate(upVector, 360 * Time.deltaTime / animator.spinEndAnimationDuration);
            timeCounter -= Time.deltaTime;
            yield return null;
        }
        head.localRotation = Quaternion.identity;

        timeCounter = enertiaEffectDuration;
        while (timeCounter > 0)
        {
            head.Rotate(upVector, enertiaEffectMagnitude * Mathf.Cos(Mathf.PI * (1 - timeCounter / enertiaEffectDuration)));
            timeCounter -= Time.deltaTime;
            yield return null;
        }

        enableManualRotation = true;
        CanAttack = true;
    }
    #endregion

    #region Cooldowns and Meters

    private void DashCooldown()
    {
        DashMeter += Time.deltaTime * 1 / dashCooldownTime;
        DashMeter = Mathf.Clamp(DashMeter, 0, dashCapacity);
    }

    #endregion

    #region Damage Bonus Mechanic
    private float GetDamageBonusMultiplier()
    {
        return (DamageBonusCurrentMultiplier - DamageBonusCurrentMultiplier % 1);
    }

    private void DamageBonusOnEnemyHit()
    {
        if (currentAttackType == previousAttackType)
            ++sameAttackTypeCount;
        else
            sameAttackTypeCount = 1;

        previousAttackType = currentAttackType;
        float damageBonus = 0;
        if (sameAttackTypeCount <= damageBonusFullIncrementHitCount)
            damageBonus = damageBonusIncrement;
        else if (sameAttackTypeCount <= damageBonusHalfIncrementHitCount)
            damageBonus = damageBonusIncrement / 2;

        damageBonusTimeCounter = damageBonusDecayDelay;

        DamageBonusCurrentMultiplier = Mathf.Clamp(DamageBonusCurrentMultiplier + damageBonus, 1, damageBonusMaxMultiplier + 1 - 0.0001f);
    }

    private void DamageBonusOnPlayerHit()
    {
        DamageBonusCurrentMultiplier = Mathf.Clamp(DamageBonusCurrentMultiplier - damageBonusIncrement / 2, 1, damageBonusMaxMultiplier + 1 - 0.0001f);
    }

    private void DamageBonusDecayHandler()
    {
        if (damageBonusTimeCounter > 0)
            damageBonusTimeCounter -= Time.deltaTime;
        else
            DamageBonusCurrentMultiplier = Mathf.Clamp(DamageBonusCurrentMultiplier - Time.deltaTime * damageBonusDecayPerSecond, 1, damageBonusMaxMultiplier + 1 - 0.0001f);
    }

    #endregion

    #region Interaction And Pulling

    // Tells character to interact with a current interactable
    public void Interact()
    {
        if (!CurrentInteractable)
            return;

        float distance = Vector3.Distance(head.position, CurrentInteractable.ButtonPosition);
        if (distance <= interactionDistance)
        {
            CurrentInteractable.Interact(this);
            return;
        }
        
        Pullable pullable = CurrentInteractable as Pullable;
        if (pullable)
            Pull(pullable);
    }

    private void UpdateInteractable()
    {
        IEnumerable<Interactable> closest = InteractableRegistry.GetInstance().GetClosestInteractables(Position, pullingDistance);


        
        var inter = from interactable in closest
                    where Utility.WithinAngle(head.position, head.forward, interactable.ButtonPosition, interactionAngle)
                    && Utility.IsVisible(head.position, interactable.gameObject, pullingDistance + 1,(interactable.ButtonPosition - interactable.gameObject.transform.position).y)
                    select new { interactable, direcionalDistance = Vector3.Distance(head.forward, (interactable.ButtonPosition - head.position).normalized) };

        var temp = from x in inter
                   where x.direcionalDistance == inter.Min(y => y.direcionalDistance)
                   select x.interactable;

        CurrentInteractable = temp.Count() > 0 ? temp.First() : null;
    }

    private void Pull(Pullable pullable)
    {
        if (CanAttack)
            StartCoroutine(PullRoutine(pullable));
    }

    private IEnumerator PullRoutine(Pullable pullable)
    {
        CanAttack = false;
        attackAnimationTimeLeft = animator.forcePull.attackAnimationDuration;
        animator.ResetWalkingAnimation();
        animator.ForcePull();
        yield return new WaitForSeconds(animator.forcePull.attackDamageDelay);

        PlayerCamera.ScreenShake(0.2f, Position);
        PlayerCamera.Recoil();
        AICharacter ai = pullable.gameObject.GetComponent<AICharacter>();
        ai.Stun(pullingStunTime, this);
        pullable.Pull(head.position + transform.forward * interactionDistance, pullingSpeed);
        
        yield return new WaitForSeconds(animator.forcePull.attackDuration - animator.forcePull.attackDamageDelay);

        CanAttack = true;
    }

    #endregion

    #region Overrides
    public override void DamageFeedback(Entity damagedEntity,DamageOutcome damageOutcome)
    {
        if (damagedEntity == this)
            return;
        switch (damageOutcome)
        {
            case DamageOutcome.Invincible:
                break;
            case DamageOutcome.Death:
                hud.deathmarker.Hitmarker();
                hud.hitmarker.Hitmarker(); break;
            case DamageOutcome.Hit:
                hud.hitmarker.Hitmarker(); break;
        }

        // Damage bonus handling
        DamageBonusOnEnemyHit();
    }

    protected override void OnDamageTaken(float rawDamage, Entity damageGiver, Vector3 sourcePosition)
    {
        if (rawDamage == 0)
            return;

        hud.DamageIndicator(sourcePosition, this);
        HeadKnockBack(1);
        PlayerCamera.ScreenShake(0.2f);
        DamageBonusOnPlayerHit();
    }

    protected override void DeathEffect()
    {
        //GameMode.instance.PlayerDeath();
    }

    #endregion
}
