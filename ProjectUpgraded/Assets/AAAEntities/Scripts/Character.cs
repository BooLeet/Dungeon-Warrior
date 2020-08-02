using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : Entity {
    public CharacterController controller;
    public NavMeshAgent navAgent;
    public Transform head;

    [Header("Stats")]
    public CharacterStats stats;

    [Header("Movement")]
    protected Vector3 verticalVelocity;
    private Vector3 inertialMovementVector = Vector3.zero;
    private Vector3 inertialSmoothVelocity = Vector3.zero;
    private Vector3 movementVector;
    private float trueMoveSpeed;

    private NavMeshPath path;
    private float manualMovementMagnitude = 0;
    private readonly float manualMovementMagnitudeLerp = 10;

    public CharacterInventory inventory;

    void Start()
    {
        navAgent.enabled = false;
        path = new NavMeshPath();
        inventory = new CharacterInventory();
        ControllerStart();
    }

    void Update()
    {
        CalculateTrueMoveSpeed();
        ControllerUpdate();
    }

    protected abstract void ControllerStart();

    protected abstract void ControllerUpdate();

    #region Public Movement

    // Calculates true movement speed
    private void CalculateTrueMoveSpeed()
    {
        float moveSpeedMultiplier = 1;
        //if (IsAttacking)
        //    moveSpeedMultiplier = stats.attackSpeedMultiplier;
        //else if (IsRunning)
        //    moveSpeedMultiplier = stats.runSpeedMultiplier;

        trueMoveSpeed = stats.moveSpeed * moveSpeedMultiplier;
    }

    // Tells character to move to a given position
    public void MoveToPosition(Vector3 position)
    {
        UpdatePath(position);
        FollowPath(0);
    }

    // Tells character to follow the position with stopping distance
    public void Follow(Vector3 positionToFollow, float stoppingDistance, bool lookAtPosition = true)
    {
        UpdatePath(positionToFollow);
        FollowPath(stoppingDistance);

        if (Vector3.Distance(positionToFollow, transform.position) > stoppingDistance)
            return;
        if (lookAtPosition)
            LookAt(positionToFollow);
    }

    // Updates a path variable
    private void UpdatePath(Vector3 targetPosition)
    {
        if (!IsNearGround())
            return;

        navAgent.enabled = true;
        navAgent.CalculatePath(targetPosition, path);
        navAgent.enabled = false;
    }

    // Follows a path
    private void FollowPath(float stoppingDistance)
    {
        if (path.corners.Length == 0)
            return;

        LookAt(path.corners[1]);

        float currentDistance = 0;
        for (int i = 0; i < path.corners.Length - 1; ++i)
            currentDistance += Vector3.Distance(path.corners[i], path.corners[i + 1]);

        Vector2 inputVector = currentDistance > stoppingDistance ? Vector2.up : Vector2.zero;
        ManualMovement(inputVector, transform.forward, transform.right);
    }


    // Inertial movement relative to the direction that character is facing 
    public void ManualMovement(Vector2 moveVector,Vector3 forward,Vector3 right,float inAirMoveSpeedMultiplier = 0)
    {
        if (moveVector.magnitude > 1)
            moveVector.Normalize();

        Vector2 nonSmoothedMoveVector = moveVector;

        manualMovementMagnitude = Mathf.Lerp(manualMovementMagnitude, moveVector.magnitude, Time.deltaTime * manualMovementMagnitudeLerp);
        moveVector = moveVector.normalized * manualMovementMagnitude;

        InertialMovement(forward * moveVector.y + right * moveVector.x);
        if (!IsNearGround())
            NonInertialMovement(trueMoveSpeed * inAirMoveSpeedMultiplier * (forward * nonSmoothedMoveVector.y + right * nonSmoothedMoveVector.x));
    }
    #endregion

    #region Physical Movement

    // Returns true in character is near ground
    public bool IsNearGround()
    {
        return Physics.Raycast(new Ray(transform.position, Vector3.down), 0.3f) || controller.isGrounded;
    }

    // Applies inertial movement to the movement vector
    protected void InertialMovement(Vector3 moveVector, bool checkGround = true, float multiplier = 1, float smoothTime = 0.05f)
    {
        if (controller.isGrounded || IsNearGround() || !checkGround)
        {
            moveVector *= Time.deltaTime * trueMoveSpeed * multiplier;

            Vector3 horizontalVelocity = controller.velocity;
            horizontalVelocity.y = 0;
            float velocityMultiplier = Mathf.Clamp(horizontalVelocity.magnitude / trueMoveSpeed, 0, 1);

            //inertialMovementVector = Vector3.SmoothDamp(inertialMovementVector, moveVector * (0.4f + 0.6f * velocityMultiplier), ref inertialSmoothVelocity, smoothTime);
            inertialMovementVector = moveVector * (0.4f + 0.6f * velocityMultiplier);
        }
        movementVector += inertialMovementVector;
        inertialMovementVector -= inertialMovementVector * Time.deltaTime * 0.05f;
    }

    // Redirects inertia relative to the direction that character is facing 
    public void RedirectInertia(Vector2 moveInput,Vector3 forward, Vector3 right)
    {
        if (moveInput.magnitude == 0)
            return;

        moveInput = moveInput.normalized * inertialMovementVector.magnitude;
        inertialMovementVector = forward * moveInput.y + right * moveInput.x;
    }

    // Resets all inertia and movement
    public void ResetInertia()
    {
        inertialMovementVector = Vector3.zero;
        verticalVelocity = Vector3.zero;
        movementVector = Vector3.zero;
    }

    // Sets vertical velocity
    public void SetVerticalVelocity(float val)
    {
        verticalVelocity = Vector3.up * val;
    }

    // Applies non-inertial movement to the movement vector
    public void NonInertialMovement(Vector3 moveSpeedVector)
    {
        moveSpeedVector *= Time.deltaTime;
        if (moveSpeedVector.magnitude == 0)
            return;

        movementVector += moveSpeedVector;
    }

    // Applies gravity to the movement vector
    public void Gravity()
    {
        if (controller.isGrounded && verticalVelocity.y < 0)
            verticalVelocity = Vector3.zero;

        verticalVelocity += stats.gravityModifier * Physics.gravity * Time.deltaTime;
        Vector3 deltaPosition = verticalVelocity * Time.deltaTime;
        Vector3 move = Vector3.up * deltaPosition.y;

        movementVector += move;
    }

    // Applies jump force
    public void Jump()
    {
        if (!controller.isGrounded)
            return;

        verticalVelocity.y = stats.jumpVelocity;
    }

    // Applies the movement vector
    public void ApplyMovement(float multiplier = 1)
    {
        controller.Move(movementVector * multiplier);
        movementVector = Vector3.zero;
    }

    #endregion

    #region Rotation
    // Tells character to look at a target poisition
    public void LookAt(Vector3 target)
    {
        target.y = transform.position.y;
        Vector3 direction = target - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
    }



    #endregion

    #region Attack

    public abstract Vector3 GetAttackSource();

    public abstract Vector3 GetAttackDirection(float spreadAngleDeg);

    public virtual void DamageFeedback(Entity damagedEntity,DamageOutcome damageOutcome) { }

    #endregion

    protected override float RecalculateRawDamage(float rawDamage, Entity damageGiver)
    {
        if (damageGiver == this)
            return rawDamage * stats.selfDamageMultiplier;
        return rawDamage;
    }
}
