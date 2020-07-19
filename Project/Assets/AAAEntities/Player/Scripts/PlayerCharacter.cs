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

    public float cameraSensitivity = 1;
    public float verticalRotationSpeedMultiplier = 1;
    [Range(0, 90)]
    public float maxVerticalRotationAngle = 90;
    private float verticalAimAngle = 0;
    public float knockBackLerpParameter = 5;
    private float knockBackVerticalAngle = 0;

    [Header("Attack")]
    public AttackFunction rangeAttackFunction;
    public float rangeAttackDamage = 5;
    public float rangeAttackHeadKnockBackAngle = 2;
    [Space]
    public AttackFunction meleeAttackFunction;
    public float meleeAttackDamage = 30;
    public float meleeAttackHeadKnockBackAngle = 2;
    [Space]
    public float targetingRaycastRange = 25;
    [Header("Zoom")]
    public float zoomAbilityMotionMultiplier = 0.3f;
    public float zoomAbilityDuration = 3f;
    public float ZoomAbilityMeter { get; private set; }
    public bool ZoomAbilityInUse { get; set; }

    [Header("Aim Assist")]
    public bool enableAimAssist = true;
    public float aimAssistAngle = 45;
    public float aimAssistDistance = 40;
    public Entity AimAssistedEntity { get; private set; }

    public bool CanAttack { get; private set; }
    private Transform attackSource;

    public bool EnableInput { get; set; }

    [Header("Interaction")]
    public float interactionDistance = 5;

    [Header("Melee Leap")]
    public float leapMeleeDamageMultiplier = 1.5f;
    public float leapRange = 20;
    public float leapSpeed = 50;
    public float leapCameraAngleThreshold = 50;

    [Header("Dash")]
    public float dashSpeed = 30;
    public float dashDuration = 0.3f;
    public byte dashCapacity = 2;
    public float dashCooldownTime = 1;
    [Range(60, 90)]
    public float dashInversionMinAngle = 70;
    public float DashMeter { get; private set; }

    //FSM
    private PlayerState currentState;

    protected override void ControllerStart()
    {
        if (instance)
            Destroy(gameObject);
        instance = this;

        Utility.DisbleCursor();
        EnableInput = true;
        CanAttack = true;

        DashMeter = dashCapacity;
        ZoomAbilityMeter = zoomAbilityDuration;

        currentState = new FreeWalkState();
    }

    protected override void ControllerUpdate()
    {
        StateMachine();
        UpdateAimAssist();
        ZoomAbilityMeterUpdate();
        HeadKnockBackUpdate();
        DashCooldown();
        playerCamera.UpdateCamera();
    }

    private void StateMachine()
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
        float vertical = rotationVector.y * cameraSensitivity * verticalRotationSpeedMultiplier;
        verticalAimAngle = Mathf.Clamp(verticalAimAngle - vertical, -maxVerticalRotationAngle, maxVerticalRotationAngle);
        float angle = Mathf.Clamp(verticalAimAngle - knockBackVerticalAngle, -maxVerticalRotationAngle, maxVerticalRotationAngle);
        playerCamera.angleV = angle;


        float horizontal = rotationVector.x * cameraSensitivity;
        playerCamera.angleU = (playerCamera.angleU - horizontal) % 360;
        //transform.Rotate(new Vector3(0, horizontal, 0));
    }

    // Makes the character face the direction of the camera
    public void FaceCameraDirection()
    {
        transform.rotation = Quaternion.Euler(0, -playerCamera.angleU, 0);
    }

    public void FaceWalkingDirection(Vector2 moveVector)
    {
        Vector3 direction = playerCamera.CameraForward * moveVector.y + playerCamera.CameraRight * moveVector.x;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 10);
    }

    public void FaceWalkingDirectionImediate(Vector2 moveVector)
    {
        Vector3 direction = playerCamera.CameraForward * moveVector.y + playerCamera.CameraRight * moveVector.x;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    #endregion

    #region Attack
    public void MeleeAttack()
    {
        if (!CanAttack)
            return;
        attackSource = animator.meleeAttack.damageSource;
        FaceCameraDirection();
        StartCoroutine(AttackRoutine(animator.meleeAttack.attackDamageDelay, animator.meleeAttack.attackDuration, meleeAttackFunction, animator.MeleeAttack, meleeAttackDamage, meleeAttackHeadKnockBackAngle));
    }

    public void RangeAttack()
    {
        if (!CanAttack)
            return;
        attackSource = animator.rangeAttack.damageSource;
        FaceCameraDirection();
        StartCoroutine(AttackRoutine(animator.rangeAttack.attackDamageDelay, animator.rangeAttack.attackDuration, rangeAttackFunction, animator.RangeAttack, rangeAttackDamage, rangeAttackHeadKnockBackAngle));
    }


    private IEnumerator AttackRoutine(float attackDamageDelay, float attackDuration, AttackFunction attackFunction, Utility.VoidFunction animationCall, float damage, float headKnockBackAngle = 0)
    {
        if (attackDamageDelay > attackDuration)
            Debug.LogError("attackDamageDelay > attackDuration isn't permitted");

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

    #region Cooldowns and Meters

    private void DashCooldown()
    {
        DashMeter += Time.deltaTime * 1 / dashCooldownTime;
        DashMeter = Mathf.Clamp(DashMeter, 0, dashCapacity);
    }

    // Manages the zoom ability meter depending on the ability use
    private void ZoomAbilityMeterUpdate()
    {
        if (ZoomAbilityInUse && !IsNearGround())
            ZoomAbilityMeter -= Time.unscaledDeltaTime;
        else if (IsNearGround())
            ZoomAbilityMeter = zoomAbilityDuration;

        ZoomAbilityMeter = Mathf.Clamp(ZoomAbilityMeter, 0, zoomAbilityDuration);
    }

    #endregion

    #region Zoom Ability
    // Slow motion effect and camera zoom update
    public void ZoomAbilityUpdate(bool zoomInput)
    {
        ZoomAbilityInUse = zoomInput;

        Time.timeScale = (ZoomAbilityInUse && ZoomAbilityMeter > 0 && !IsNearGround()) ? zoomAbilityMotionMultiplier : 1;
        
        playerCamera.SetZoom(ZoomAbilityInUse);
    }

    #endregion

    #region Overrides
    public override void DamageFeedback(Entity damagedEntity)
    {
        if (damagedEntity == this)
            return;

        hud.hitmarker.Hitmarker();
    }

    protected override void OnDamageTaken(float rawDamage, Entity damageGiver, Vector3 sourcePosition)
    {
        if (rawDamage == 0)
            return;

        hud.DamageIndicator(sourcePosition, this);
        //hud.damageVignette.StartEffect();
        HeadKnockBack(1);
        PlayerCamera.ScreenShake(0.2f);
    }

    protected override void DeathEffect()
    {
        //GameMode.instance.PlayerDeath();
    }

    #endregion
}
