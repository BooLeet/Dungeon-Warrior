using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PlayerFSM;

public class PlayerCharacter : Character
{
    public static PlayerCharacter instance;
    [Header("Stats")]
    public PlayerStats playerStats;

    [Header("References")]
    public PlayerInput input;
    public PlayerCharacterAnimator animator;
    public PlayerCamera playerCamera;
    public HUD hud;
    public MainMenuController menuController;
    [Space]
    public GameObject gameManagerPrefab;
    //public PlayerViewmodelAnimator viewmodelAnimator;

    
    [Header("Rotation")]
    private bool enableManualRotation = true;
    public float verticalRotationSpeedMultiplier = 1;
    [Range(0, 90)]
    public float maxVerticalRotationAngle = 90;
    private float verticalAimAngle = 0;
    public float knockBackLerpParameter = 5;
    private float knockBackVerticalAngle = 0;

    public bool EnableInput { get; set; }
    public float SpecialAttackMeter { get; set; }
    public bool CanAttack { get; private set; }
    private float attackAnimationTimeLeft = 0;
    public bool AttackAnimationFinished { get { return attackAnimationTimeLeft <= 0; } }
    private Transform attackSource;

    
    public Entity AimAssistedEntity { get; private set; }

    public float CurrentMana { get; private set; }

    public Interactable CurrentInteractable { get; private set; }
    
    public float DashMeter { get; private set; }

    //FSM
    private PlayerState currentMovementState;
    private PlayerState currentCombatState;
    public string currentMovementStateName;
    public string currentCombatStateName;

    [Header("Death Effect")]
    public float deathEffectDuration = 1.5f;
    private Entity lastDamageGiver = null;

    protected override void ControllerStart()
    {
        if (instance)
            Destroy(gameObject);
        instance = this;

        if (GameManager.instance == null)
            Instantiate(gameManagerPrefab);

        menuController.DisableMenu();

        Utility.DisbleCursor();
        EnableInput = true;
        CanAttack = true;

        CurrentMana = playerStats.maxMana;

        DashMeter = playerStats.dashCapacity;

        currentMovementState = new DirectMovementState();
        currentCombatState = new BasicAttackState();

        animator.EquipMeleeWeapon(playerStats.startingMeleeWeapon);
    }

    protected override void ControllerUpdate()
    {
        if (IsDead)
            return;

        if (GameManager.instance.IsPaused)
            return;
        input.UpdateValues();
        PauseHandler();

        ManaRegeneration();
        UpdateInteractable();
        StateMachine(ref currentMovementState);
        StateMachine(ref currentCombatState);
        currentMovementStateName = currentMovementState.ToString();
        currentCombatStateName = currentCombatState.ToString();

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


    public void PauseHandler()
    {
        if (!input.Pause || GameManager.instance.IsPaused)
            return;

        GameManager.instance.PauseGame();
        menuController.EnableMenu();
        menuController.ShowMain();
    }

    public void DeduceDashMeter()
    {
        --DashMeter;
        if (DashMeter < 0)
            DashMeter = 0;
    }
    #endregion

    #region Rotation
    // Standard FPS rotation 
    public void ManualRotation(Vector2 rotationVector)
    {
        if (!enableManualRotation)
            return;

        float cameraSensitivity = GameManager.instance.settings.gameSettings.mouseSensitivity;

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

        StartCoroutine(AttackRoutine(animator.meleeAttack.attackDamageDelay, animator.meleeAttack.attackDuration, animator.meleeAttack.attackAnimationDuration,
            playerStats.meleeAttackFunction, animator.MeleeAttack, playerStats.meleeAttackDamage, playerStats.meleeAttackHeadKnockBackAngle));
    }

    public bool CanUseRevolver()
    {
        return inventory.HasResource(playerStats.revolverResource);
    }
    public void ForcePush()
    {
        if (!CanAttack || !ManaAvailable())
            return;
        attackSource = animator.meleeAttack.damageSource;
        
        SpendMana(playerStats.forcePushManaCost);
        StartCoroutine(AttackRoutine(animator.forcePush.attackDamageDelay, animator.forcePush.attackDuration, animator.forcePush.attackAnimationDuration,
            playerStats.forcePushAttackFunction, animator.ForcePush, playerStats.meleeAttackDamage, playerStats.meleeAttackHeadKnockBackAngle));
    }

    private IEnumerator AttackRoutine(float attackDamageDelay, float attackDuration, float attackAnimationDuration, AttackFunction attackFunction, Utility.VoidFunction animationCall, float damage, float headKnockBackAngle = 0,bool shakeScreen = true)
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

        if(shakeScreen)
            PlayerCamera.ScreenShake(0.2f, Position);
        //PlayerCamera.Recoil();
        HeadKnockBack(headKnockBackAngle);
        // Do damage
        if(attackFunction)
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
        if (playerStats.enableAimAssist)
        {
            var enemiesInRange = from entity in EntityRegistry.GetInstance().GetClosestEntities(Position, playerStats.aimAssistDistance, this)
                                 where Utility.IsVisible(playerCamera.transform.position, entity.gameObject, playerStats.aimAssistDistance, entity.verticalTargetingOffset)
                                 && Utility.WithinAngle(playerCamera.transform.position, playerCamera.transform.forward, entity.Position, playerStats.aimAssistAngle)
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

        Vector3 hitPosition = cameraTransform.position + forward * playerStats.targetingRaycastRange;
        int layermask = ~(1 << gameObject.layer);
        if (Physics.Raycast(ray, out hitInfo, playerStats.targetingRaycastRange,layermask))
        {
            if (hitInfo.collider.GetComponent<PlayerCharacter>())
                Debug.LogError("PLAYER CHARACTER ??! 0_o");
            hitPosition = hitInfo.point;
        }

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
        float damageMultiplier = Mathf.Lerp(playerStats.stingAttackDmgMultiplier.x, playerStats.stingAttackDmgMultiplier.y, SpecialAttackMeter);
        SpecialAttackMeter = 0;

        StartCoroutine(AttackRoutine(animator.stingEndAnimation.attackDamageDelay, animator.stingEndAnimation.attackDuration,
            animator.stingEndAnimation.attackAnimationDuration, playerStats.stingAttackFunction, animator.StingAttackEnd, playerStats.meleeAttackDamage * damageMultiplier,
            playerStats.meleeAttackHeadKnockBackAngle));
    }
    #endregion

    #region Revolver
    public void RevolverAttackStart()
    {
        CanAttack = false;
        animator.RevolverAttackStart();
    }

    public void RevolverAttackEnd()
    {
        attackSource = animator.revolverAttack.damageSource;
        float damageMultiplier = Mathf.Lerp(playerStats.revolverAttackDmgMultiplier.x, playerStats.revolverAttackDmgMultiplier.y, SpecialAttackMeter);
        SpecialAttackMeter = 0;
        inventory.SpendResource(playerStats.revolverResource);

        StartCoroutine(AttackRoutine(animator.revolverAttack.attackDamageDelay, animator.revolverAttack.attackDuration, animator.revolverAttack.attackAnimationDuration,
            playerStats.revolverAttackFunction, animator.RevolverAttackEnd, playerStats.revolverAttackDamage * damageMultiplier, playerStats.revolverAttackHeadKnockBackAngle));
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
        float damageMultiplier = Mathf.Lerp(playerStats.spinAttackDmgMultiplier.x, playerStats.spinAttackDmgMultiplier.y, SpecialAttackMeter);
        SpecialAttackMeter = 0;

        StartCoroutine(SpinAttackRoutine(playerStats.meleeAttackDamage * damageMultiplier));
    }

    private IEnumerator SpinAttackRoutine(float damage, float enertiaEffectDuration = 0.5f, float enertiaEffectMagnitude = 30f)
    {
        animator.SpinAttackEnd();
        animator.ResetWalkingAnimation();
        float timeCounter = animator.spinEndAnimationDuration;
        verticalAimAngle = 0;
        head.localRotation = Quaternion.identity;
        Vector3 upVector = head.transform.up;
        enableManualRotation = false;
        //Entity previousEntity = null;
        attackAnimationTimeLeft = animator.spinEndAnimationDuration + enertiaEffectDuration;

        PlayerCamera.ScreenShake(animator.spinEndAnimationDuration, Position);
        bool damageDone = false;
        while (timeCounter > 0)
        {
            if (timeCounter <= animator.spinEndAnimationDuration / 2 && !damageDone)
            {
                damageDone = true;
                var closestEntities = EntityRegistry.GetInstance().GetClosestEntities(head.position, playerStats.spinAttackRange, this);
                foreach (Entity entity in closestEntities)
                {
                    if (Utility.IsVisible(head.position, entity.gameObject, playerStats.spinAttackRange, entity.verticalTargetingOffset))
                        Damage.SendDamageFeedback(this, entity, entity.TakeDamage(damage, this, attackSource.position));
                }

            }

            //RaycastHit rayHit;
            //float sphereCastDistance  = spinAttackRange;

            //if (Physics.Raycast(new Ray(head.position, head.forward), out rayHit, spinAttackRange))
            //{
            //    sphereCastDistance = rayHit.distance;
            //}

            //if(Physics.SphereCast(head.position,1,head.forward,out rayHit, sphereCastDistance))
            //{
            //    Entity hitEntity = rayHit.collider.GetComponent<Entity>();
            //    if (hitEntity != null && hitEntity != previousEntity)
            //    {
            //        Damage.SendDamageFeedback(this, hitEntity, hitEntity.TakeDamage(damage, this, attackSource.position));
            //        previousEntity = hitEntity;
            //    }
            //}

            head.Rotate(upVector, 360 * Time.deltaTime / animator.spinEndAnimationDuration);
            timeCounter -= Time.deltaTime;
            yield return null;
        }
        head.localRotation = Quaternion.identity;

        timeCounter = enertiaEffectDuration;
        while (timeCounter > 0)
        {
            head.Rotate(upVector, Time.deltaTime * enertiaEffectMagnitude * Mathf.Cos(Mathf.PI * (1 - timeCounter / enertiaEffectDuration)));
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
        DashMeter += Time.deltaTime * 1 / playerStats.dashCooldownTime;
        DashMeter = Mathf.Clamp(DashMeter, 0, playerStats.dashCapacity);
    }

    #endregion

    #region Interaction
    /// <summary>
    /// Tells character to interact with a current interactable
    /// </summary>
    public void Interact()
    {
        if (!CurrentInteractable)
            return;

        CurrentInteractable.Interact(this);
    }

    private void UpdateInteractable()
    {
        IEnumerable<Interactable> closest = InteractableRegistry.GetInstance().GetClosestInteractables(Position, playerStats.interactionDistance);



        var inter = from interactable in closest
                    where Utility.WithinAngle(head.position, head.forward, interactable.ButtonPosition, playerStats.interactionAngle)
                    && Utility.IsVisible(head.position, interactable.gameObject, playerStats.interactionDistance + 1,(interactable.ButtonPosition - interactable.gameObject.transform.position).y)
                    select new { interactable, direcionalDistance = Vector3.Distance(head.forward, (interactable.ButtonPosition - head.position).normalized) };

        var temp = from x in inter
                   where x.direcionalDistance == inter.Min(y => y.direcionalDistance)
                   select x.interactable;

        CurrentInteractable = temp.Count() > 0 ? temp.First() : null;
    }

    #endregion

    #region Overrides
    public override CharacterStats GetCharacterStats()
    {
        return playerStats;
    }

    public override float GetMaxHealth()
    {
        return playerStats.maxHealth;
    }

    protected override float GetMoveSpeed()
    {
        return playerStats.moveSpeed;
    }

    public override float GetInAirSpeed()
    {
        return 0;
    }

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
    }

    protected override void OnDamageTaken(float rawDamage, Entity damageGiver, Vector3 sourcePosition)
    {
        if (rawDamage == 0)
            return;

        lastDamageGiver = damageGiver;
        GameMode.gameModeInstance.OnPlayerDamaged(rawDamage);
        hud.DamageIndicator(sourcePosition, this);
        HeadKnockBack(1);
        PlayerCamera.ScreenShake(0.2f);
    }

    protected override void DeathEffect()
    {
        animator.PlayDeathAnimation();
        hud.ShowHide(false);
        StartCoroutine(DeathEffectRoutine());
        GameMode.gameModeInstance.OnPlayerDeath();

        Utility.EnableCursor();
        menuController.EnableMenu();
        menuController.ShowGameOver();
    }

    private IEnumerator DeathEffectRoutine()
    {
        float timeCounter = 0;
        Vector3 accelerationDirection = Vector3.up;
        Vector3 floorPosition = transform.position + Vector3.up * 0.3f;

        Vector3 acceleration = accelerationDirection * 350;
        float headMass = 2;
        bool canStop = false;
        while(timeCounter < deathEffectDuration && !canStop)
        {
            acceleration += Vector3.down * headMass * 10;
            head.position += acceleration * Time.deltaTime * Time.deltaTime;
            if ((head.position - floorPosition).y < 0)
            {
                head.position = new Vector3(head.position.x, floorPosition.y, head.position.z);
                canStop = true;
            }
            timeCounter += Time.deltaTime;
            yield return null;
        }
    }

    #endregion

    #region Mana

    private void ManaRegeneration()
    {
        CurrentMana += playerStats.manaRegenPerSecond * Time.deltaTime;
        if (CurrentMana > playerStats.maxMana)
            CurrentMana = playerStats.maxMana;
    }

    private bool ManaAvailable()
    {
        return CurrentMana > 0;
    }

    private void SpendMana(float amount)
    {
        CurrentMana -= amount;
    }

    #endregion

    #region Misc

    public void InspectWeapon()
    {
        if (!CanAttack)
            return;
        StartCoroutine(AttackRoutine(0, 0, animator.inspectAnimationDuration, null, animator.InspectWeapon, 0, 0,false));
    }

    #endregion
}
