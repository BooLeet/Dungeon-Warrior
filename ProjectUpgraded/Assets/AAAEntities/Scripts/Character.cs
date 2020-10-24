using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : Entity {
    public Transform head;

    [Header("Collider")]
    public Vector3 center = Vector3.up;
    [Range(0,10)]
    public float radius = 0.5f;
    [Range(0, 10)]
    public float height = 2;
    private CharacterController controller;
    private NavMeshAgent navAgent;
    private CapsuleCollider capsuleCollider;
    private bool useNavAgent = true;

    [Header("Movement")]
    protected Vector3 verticalVelocity;
    private Vector3 inertialMovementVector = Vector3.zero;
    private Vector3 inertialSmoothVelocity = Vector3.zero;
    private Vector3 movementVector;
    //private float trueMoveSpeed;

    private NavMeshPath path;
    private Vector3 manualMovementVector = Vector3.zero;

    public CharacterInventory inventory;
    public CharacterActionState currentActionState;

    void Start()
    {
        controller = gameObject.AddComponent<CharacterController>();
        controller.center = center;
        controller.radius = radius;
        controller.height = height;

        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.center = center;
        capsuleCollider.radius = radius;
        capsuleCollider.height = height;

        navAgent = gameObject.AddComponent<NavMeshAgent>();
        navAgent.radius = radius;
        navAgent.height = height;
        navAgent.speed = GetCharacterStats().moveSpeed;
        navAgent.acceleration = 1000;
        navAgent.angularSpeed = 1440;
        navAgent.enabled = true;//false

        ChangeMovementMode(true);

        //path = new NavMeshPath();
        inventory = new CharacterInventory();

        ControllerStart();
    }

    void Update()
    {
        ControllerUpdate();
        ActionStateMachine();
    }

    protected abstract void ControllerStart();

    protected abstract void ControllerUpdate();

    public abstract CharacterStats GetCharacterStats();

    #region Public Movement
    private void ChangeMovementMode(bool useNavAgent)
    {
        this.useNavAgent = useNavAgent;
        if(navAgent)
            navAgent.enabled = useNavAgent;
        if(capsuleCollider)
            capsuleCollider.enabled = useNavAgent;
        if(controller)
            controller.enabled = !useNavAgent;
    }

    /// <summary>
    /// Returns character's move speed
    /// </summary>
    /// <returns></returns>
    protected abstract float GetMoveSpeed();

    /// <summary>
    /// Returns character's in air move speed
    /// </summary>
    /// <returns></returns>
    public abstract float GetInAirSpeed();

    /// <summary>
    /// Tells character to move to a given position
    /// </summary>
    /// <param name="position"></param>
    public void MoveToPosition(Vector3 position)
    {
        SetDestination(position, 0);
        //UpdatePath(position);
        //FollowPath(0);
    }

    /// <summary>
    /// Tells character to follow the position with stopping distance
    /// </summary>
    /// <param name="positionToFollow"></param>
    /// <param name="stoppingDistance"></param>
    /// <param name="lookAtPosition"></param>
    public void Follow(Vector3 positionToFollow, float stoppingDistance, bool lookAtPosition = true)
    {
        //UpdatePath(positionToFollow);
        //FollowPath(stoppingDistance);
        SetDestination(positionToFollow, stoppingDistance);

        if (Vector3.Distance(positionToFollow, transform.position) > stoppingDistance)
            return;
        if (lookAtPosition)
            LookAt(positionToFollow);
    }

    private void SetDestination(Vector3 position, float stoppingDistance)
    {
        ChangeMovementMode(true);
        navAgent.SetDestination(position);
        navAgent.speed = GetCharacterStats().moveSpeed;
        navAgent.stoppingDistance = stoppingDistance;
    }

    /// <summary>
    /// [DEPRECATED] Updates a path variable
    /// </summary>
    /// <param name="targetPosition"></param>
    private void UpdatePath(Vector3 targetPosition)
    {
        if (!IsNearGround())
            return;

        navAgent.enabled = true;
        navAgent.CalculatePath(targetPosition, path);
        navAgent.enabled = false;
    }

    /// <summary>
    /// [DEPRECATED] Follows a path
    /// </summary>
    /// <param name="stoppingDistance"></param>
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


    /// <summary>
    /// Inertial movement relative to the direction that character is facing 
    /// </summary>
    /// <param name="moveVector"></param>
    /// <param name="forward"></param>
    /// <param name="right"></param>
    /// <param name="inAirMoveSpeedMultiplier"></param>
    /// <param name="inertiaModifier"></param>
    public void ManualMovement(Vector2 moveVector,Vector3 forward,Vector3 right, float inertiaModifier = 7)
    {
        ChangeMovementMode(false);
        if (moveVector.magnitude > 1)
            moveVector.Normalize();

        Vector3 moveVector3 = forward * moveVector.y + right * moveVector.x;
        manualMovementVector += (moveVector3 - manualMovementVector) * inertiaModifier * Time.deltaTime;
        InertialMovement(manualMovementVector);
        if (!IsNearGround())
            NonInertialMovement(GetInAirSpeed() * (forward * moveVector.y + right * moveVector.x));
    }

    
    #endregion

    #region Physical Movement

    /// <summary>
    /// Returns true if character is near ground
    /// </summary>
    /// <returns></returns>
    public bool IsNearGround()
    {
        return Physics.Raycast(new Ray(transform.position, Vector3.down), 0.3f) || controller.isGrounded;
    }

    /// <summary>
    /// Applies inertial movement to the movement vector
    /// </summary>
    /// <param name="moveVector"></param>
    /// <param name="checkGround"></param>
    /// <param name="multiplier"></param>
    /// <param name="smoothTime"></param>
    protected void InertialMovement(Vector3 moveVector, bool checkGround = true, float multiplier = 1, float smoothTime = 0.05f)
    {
        if (controller.isGrounded || IsNearGround() || !checkGround)
        {
            moveVector *= Time.deltaTime * GetMoveSpeed() * multiplier;

            Vector3 horizontalVelocity = controller.velocity;
            horizontalVelocity.y = 0;
            float velocityMultiplier = Mathf.Clamp(horizontalVelocity.magnitude / GetMoveSpeed(), 0, 1);

            //inertialMovementVector = Vector3.SmoothDamp(inertialMovementVector, moveVector * (0.4f + 0.6f * velocityMultiplier), ref inertialSmoothVelocity, smoothTime);
            inertialMovementVector = moveVector * (0.4f + 0.6f * velocityMultiplier);
        }
        movementVector += inertialMovementVector;
        inertialMovementVector -= inertialMovementVector * Time.deltaTime * 0.05f;
    }

    /// <summary>
    /// Redirects inertia relative to the direction that character is facing
    /// </summary>
    /// <param name="moveInput"></param>
    /// <param name="forward"></param>
    /// <param name="right"></param>
    public void RedirectInertia(Vector2 moveInput,Vector3 forward, Vector3 right)
    {
        if (moveInput.magnitude == 0)
            return;

        moveInput = moveInput.normalized * inertialMovementVector.magnitude;
        inertialMovementVector = forward * moveInput.y + right * moveInput.x;
    }

    /// <summary>
    /// Resets all inertia and movement
    /// </summary>
    public void ResetInertia()
    {
        inertialMovementVector = Vector3.zero;
        verticalVelocity = Vector3.zero;
        movementVector = Vector3.zero;
    }

    /// <summary>
    /// Sets vertical velocity
    /// </summary>
    /// <param name="val"></param>
    public void SetVerticalVelocity(float val)
    {
        verticalVelocity = Vector3.up * val;
    }

    /// <summary>
    /// Applies non-inertial movement to the movement vector
    /// </summary>
    /// <param name="moveSpeedVector"></param>
    public void NonInertialMovement(Vector3 moveSpeedVector)
    {
        moveSpeedVector *= Time.deltaTime;
        if (moveSpeedVector.magnitude == 0)
            return;

        movementVector += moveSpeedVector;
    }

    /// <summary>
    /// Applies gravity to the movement vector
    /// </summary>
    public void Gravity()
    {
        if (!IsNearGround() && useNavAgent)
            ChangeMovementMode(false);

        if (controller.isGrounded && verticalVelocity.y < 0)
            verticalVelocity = Vector3.zero;

        verticalVelocity += GetCharacterStats().gravityModifier * Physics.gravity * Time.deltaTime;
        Vector3 deltaPosition = verticalVelocity * Time.deltaTime;
        Vector3 move = Vector3.up * deltaPosition.y;

        movementVector += move;
    }

    /// <summary>
    /// Applies jump force
    /// </summary>
    public void Jump()
    {
        if (!controller.isGrounded)
            return;
        ChangeMovementMode(false);
        verticalVelocity.y = GetCharacterStats().jumpVelocity;
    }

    /// <summary>
    /// Applies the movement vector
    /// </summary>
    /// <param name="multiplier"></param>
    public void ApplyMovement(float multiplier = 1)
    {
        if (!useNavAgent) 
            controller.Move(movementVector * multiplier);
        movementVector = Vector3.zero;
    }

    public void Warp(Vector3 position)
    {
        //ResetInertia();
        controller.enabled = false;
        transform.position = position;
        navAgent.Warp(position);
        controller.enabled = true;
    }

    /// <summary>
    /// Same as calling CharacterController.Move(Vector3 motion);
    /// </summary>
    /// <param name="motion"></param>
    public void JustMove(Vector3 motion)
    {
        if (!controller)
            return;
        ChangeMovementMode(false);
        controller.Move(motion);
    }

    /// <summary>
    /// Destroys NavMeshAgent and CharacterController (Needed in DeadlySpikes for some reason)
    /// </summary>
    public void DestroyMovementComponents()
    {
        Destroy(navAgent);
        Destroy(capsuleCollider);
        Destroy(controller);
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

    #region Action State Machine

    // Unified action system
    private void ActionStateMachine()
    {
        if (currentActionState == null)
            return;

        currentActionState.Action(this);
        CharacterActionState nextState = currentActionState.Transition(this);
        if(nextState != null)
        {
            currentActionState = nextState;
            currentActionState.Init(this);
        }
    }

    public void ChangeActionState(CharacterActionState newState)
    {
        currentActionState = newState;
        currentActionState.Init(this);
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
            return rawDamage * GetCharacterStats().selfDamageMultiplier;
        return rawDamage;
    }
}
