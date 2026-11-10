using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController))]
public class CharacterMotor : MonoBehaviour
{
    public enum MovementState
    {
        None = -1,

        /// <summary> The character is standing still. </summary>
        Idle = 0,

        /// <summary> The character is upright walking. </summary>
        Walk = 1,

        /// <summary> The character is running! </summary>
        Run = 2,

        /// <summary> The character is crouching while moving. </summary>
        CrouchingActive = 3,

        /// <summary> The character is croaching but staying still. </summary>
        CrouchingIdle = 4
    }

    public enum TargetState
    {
        NoTarget = 0,
        MovingTowardsTarget = 1,
        TargetReached = 2
    }

    public UnityEvent<MovementState> onMovementStateChanged;

    // Exposed fields.
    [SerializeField] private Transform graphics;
    [SerializeField] private float maxMovementVelocity = 10f;
    [SerializeField] private float crouchVelocity = 1f;
    [SerializeField] private float walkVelocity = 2f;
    [SerializeField] private float runVelocity = 4f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private bool moveRelativeToGraphics = false;
    [SerializeField] private bool rotateTowardsMovement = true;
 
    // Public getter/setter properties.
    public float MeasuredVelocity => measuredVelocity;
    public Vector3 MeasuredVelocityXZ => measuredVelocityXZ;

    /// <summary> The desired (or target) movement velocity. The character tries to move to the current destination using this speed. </summary>
    public float DesiredMovementVelocity => desiredMovementVelocity;

    /// <summary> The actual velocity at which the character is moving. Should always be lesser or equal to the desired movement velocity. </summary>
    public float ActualMovementVelocity => characterController.velocity.magnitude;

    /// <summary> The current maximum movement velocity (magnitude). The character should never be moving faster than this on the XZ plane unless
    /// external forces are in play. </summary>
    public float MaxMovementVelocity => maxMovementVelocity;

    /// <summary> The current movement state the character is currently in. Can be walking, running, idling etc. </summary>
    public MovementState CurrentMovementState
    {
        get { return currentMovementState; }
        set
        {
            if (currentMovementState == value) return;
            currentMovementState = value;
            onMovementStateChanged.Invoke(currentMovementState);
        }
    }
    public TargetState CurrentTargetState => currentTargetState;
    public Transform DesiredLookAtTransform => desiredLookAt;

    // Required GetComponent fields.
    private NavMeshAgent navMeshAgent;
    private CharacterController characterController;

    // Private fields
    private float desiredMovementVelocity = 0f;
    private Transform desiredLookAt = null;
    private Vector3 XZPreviousPosition = Vector3.zero;
    private Vector3 XZPositionDelta = Vector3.zero;
    private float measuredVelocity = 0f;
    private Vector3 measuredVelocityXZ = Vector3.zero;
    private bool frozen = false;
    private float currentVerticalVelocity = 0f;
    private MovementState currentMovementState = MovementState.None;

    /// <summary>
    /// Whether this character is using directional movement. If so, the directional movement input is used. 
    /// This is in contrast to the target based movement, which is dependent on the navmesh agent being given a target position.
    /// </summary>
    public bool usingDirectionalMovement = false;
    private Vector3 directionalMovementInput = Vector3.zero;

    [SerializeField]
    [Tooltip("The current target movement state. This indicates whether the navmeshagent is still moving towards its target or not." +
        "This is ignored when using directional based movement.")]
    private TargetState currentTargetState = TargetState.NoTarget;
    

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        characterController = GetComponent<CharacterController>();

        if (navMeshAgent != null)
        {
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
        }

        XZPreviousPosition = transform.position;
    }

    void Update()
    {
        if (!frozen) HandleMovement();
        if (navMeshAgent != null) SyncAgent();

        if (currentTargetState == TargetState.MovingTowardsTarget && AgentReachedDestination()) 
            currentTargetState = TargetState.TargetReached;

        ManuallyMeasureVelocity();
    }

    private bool AgentReachedDestination()
    {
        // If we don't have a path, we can't reach the end of course.
        if (navMeshAgent.pathPending || navMeshAgent.path.corners.Length == 0)
        {
            // check whether agent has reached target, but agent doesn't have any path (yet)
            return false;
        }

        Vector3 target = navMeshAgent.path.corners[^1];
        float distance = Vector3.Distance(transform.position, target);

        return distance <= navMeshAgent.stoppingDistance + 0.1f;
    }

    private void ManuallyMeasureVelocity()
    {
        XZPositionDelta = transform.position - XZPreviousPosition;
        XZPositionDelta.y = 0f;
        measuredVelocityXZ = XZPositionDelta / Time.deltaTime;
        measuredVelocity = measuredVelocityXZ.magnitude;
        XZPreviousPosition = transform.position;
    }

    private void HandleMovement()
    {
        // Apply gravity
        if (characterController.isGrounded && currentVerticalVelocity < 0) currentVerticalVelocity = -1f;
        else currentVerticalVelocity += gravity * Time.deltaTime;

        Vector3 desiredVelocity = Vector3.zero;
        if (usingDirectionalMovement)
        {
            desiredVelocity = 
                moveRelativeToGraphics ?
                graphics.forward * directionalMovementInput.z + graphics.right * directionalMovementInput.x :
                directionalMovementInput;
        }
        else if (navMeshAgent != null)
        {
            desiredVelocity = navMeshAgent.velocity;
        }

        // Ignore tiny values to prevent jitter
        if (desiredVelocity.magnitude < 0.01f) desiredVelocity = Vector3.zero;
        else desiredVelocity = desiredVelocity.normalized * desiredMovementVelocity;

        if (navMeshAgent != null && navMeshAgent.isStopped)
        {
            CurrentMovementState = MovementState.Idle;
        }

        // Apply the desired velocity and rotate in that movement direction
        Vector3 XZPlanarMovement = desiredVelocity; 
        desiredVelocity.y = currentVerticalVelocity;
        characterController.Move(desiredVelocity * Time.deltaTime);

        if (rotateTowardsMovement) HandleRotation(XZPlanarMovement);
    }

    private void HandleRotation(Vector3 moveDirection)
    {
        Quaternion targetRotation = graphics.rotation;
        if (desiredLookAt == null && moveDirection.sqrMagnitude > 0.001f)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
        }
        else if (desiredLookAt != null)
        {
            Vector3 dirToLookAtTarget = (desiredLookAt.position - transform.position).normalized;
            targetRotation = Quaternion.LookRotation(dirToLookAtTarget);
        }

        graphics.rotation = Quaternion.Slerp(
                graphics.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

    }

    private void SyncAgent()
    {
        navMeshAgent.nextPosition = transform.position;
    }

    /// <summary> Set the desired movement velocity. This determines the maximum speed at which the character moves.
    /// Note: this is a target, the actual velocity might be lower. </summary>
    /// <param name="newMovementVelocity"> The new desired movement velocity. </param>
    private void SetDesiredMovementVelocity(float newMovementVelocity)
    {
        desiredMovementVelocity = Mathf.Clamp(newMovementVelocity, 0f, maxMovementVelocity); // Sanitize input.
        
        if (navMeshAgent != null) navMeshAgent.speed = desiredMovementVelocity; // Synchronize navmeshagent.
    }

    /// <summary> Move to a certain position in a certain state. </summary>
    /// <param name="position"> The target position we need to move to. </param>
    /// <param name="desiredState"> The way the character needs to move towards the target: walking, running or crouching. Idle states are not accepted. </param>
    /// <returns> Whether the command was succesful. </returns>
    public bool MoveToPosition(Vector3 position, MovementState desiredState)
    {
        if (navMeshAgent == null)
        {
            Debug.LogWarning("Cannot move to position if no navmesh agent is present!");
            return false;
        }

        usingDirectionalMovement = false;
        SetMovementStateAndVelocity(desiredState);
        bool setDestinationSuccesfull = navMeshAgent.SetDestination(position);
        currentTargetState = setDestinationSuccesfull ? TargetState.MovingTowardsTarget : TargetState.NoTarget;
        return setDestinationSuccesfull;
    }

    public void MoveInDirection(Vector2 direction, MovementState desiredState)
    {
        usingDirectionalMovement = true;
        SetMovementStateAndVelocity(desiredState);
        currentTargetState = TargetState.NoTarget;
        directionalMovementInput = new Vector3(direction.x, 0f, direction.y);
    }

    private void SetMovementStateAndVelocity(MovementState desiredState)
    {
        switch (desiredState)
        {
            case MovementState.Walk: SetDesiredMovementVelocity(walkVelocity); break;
            case MovementState.Run: SetDesiredMovementVelocity(runVelocity); break;
            case MovementState.CrouchingActive: SetDesiredMovementVelocity(crouchVelocity); break;
            default: Debug.LogWarning("When moving to a position, the movement state must be walking, running or crouch!"); break;
        }

        CurrentMovementState = desiredState;
    }

    /// <summary> Stops all movement of this character, including rotation. Also aborts current pathfinding. Respects crouch. </summary>
    public void StopMovement()
    {
        SetDesiredMovementVelocity(0f);

        // Check this to prevent errors.
        if (navMeshAgent != null) {
            if (navMeshAgent.isOnNavMesh || navMeshAgent.isOnOffMeshLink)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();

            }
        }

        // Make sure that the character stays crouched when stopping movement.
        if (CurrentMovementState == MovementState.CrouchingActive) CurrentMovementState = MovementState.CrouchingIdle;
        else CurrentMovementState = MovementState.Idle;

        currentTargetState = TargetState.NoTarget;
    }

    public void Freeze()
    {
        frozen = true;
        StopMovement();
        characterController.enabled = false;
        if (navMeshAgent != null) navMeshAgent.enabled = false;
    }

    public void Unfreeze()
    {
        frozen = false;
        characterController.enabled = true;
        if (navMeshAgent != null) navMeshAgent.enabled = true;
    }

    public void Teleport(Vector3 targetPosition)
    {
        StopMovement();
        if (navMeshAgent != null) navMeshAgent.nextPosition = targetPosition;
        transform.position = targetPosition;
        XZPreviousPosition = transform.position;
        XZPositionDelta = Vector3.zero;
    }

    public void SetLookAtTarget(Transform lookAt)
    {
        desiredLookAt = lookAt;
    }
}
