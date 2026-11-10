using UnityEngine;
using UnityEngine.Events;

public class CharacterAnimationController : MonoBehaviour
{
    public UnityEvent<bool> onFootEvent;

    private Animator animator;
    [SerializeField] private CharacterMotor characterMotor;

    [SerializeField] private bool ignoreIK = false;

    [SerializeField] private bool isAiming = true;
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private float aimRotationAngleOffsetY;
    [SerializeField] private float aimRotationAngleOffsetX;
    [SerializeField] private Transform leftHandGripTarget;

    [Tooltip("Whether to ignore the orientation/rotation of this transform when animating and using directional blend trees." +
        "Check this if NOT rotating in the direction of movement, i.e. looking and moving are seperate, as with player-controlled characters.")]
    [SerializeField] private bool ignoreTransformRotation = false;

    private readonly static int VELOCITY = Animator.StringToHash("Velocity");
    private readonly static int DEATH = Animator.StringToHash("Death");
    private readonly static int RESPAWN = Animator.StringToHash("Respawn");
    private readonly static int VELOCITY_X = Animator.StringToHash("VelocityX");
    private readonly static int VELOCITY_Z = Animator.StringToHash("VelocityZ");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetFloat(VELOCITY, characterMotor.MeasuredVelocity);

        if (ignoreTransformRotation)
        {
            Vector3 workingVelocity = Vector3.ClampMagnitude(characterMotor.MeasuredVelocityXZ, 1f);
            animator.SetFloat(VELOCITY_X, workingVelocity.x);
            animator.SetFloat(VELOCITY_Z, workingVelocity.z);
        }
        else
        {
            Vector3 rotatedVelocity = transform.InverseTransformDirection(characterMotor.MeasuredVelocityXZ);
            rotatedVelocity = Vector3.ClampMagnitude(rotatedVelocity, 1f);
            animator.SetFloat(VELOCITY_X, rotatedVelocity.x);
            animator.SetFloat(VELOCITY_Z, rotatedVelocity.z);
        } 
    }

    public void SetAimTarget(Transform target)
    {
        aimTarget = target;
        isAiming = target != null;
    }

    public void RemoveAimTarget()
    {
        SetAimTarget(null);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (ignoreIK) return;

        if (isAiming)
        {
            animator.SetLookAtWeight(1);
            animator.SetLookAtPosition(aimTarget.position);

            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            Vector3 aimDirection = (aimTarget.position - rightHand.position).normalized;
            Quaternion aimRotation = Quaternion.LookRotation(aimDirection, transform.right);
            aimRotation *= Quaternion.Euler(aimRotationAngleOffsetX, aimRotationAngleOffsetY, 0f);

            animator.SetIKRotation(AvatarIKGoal.RightHand, aimRotation);

            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandGripTarget.position);
        }

        else
        {
            animator.SetLookAtWeight(0f);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
        }
    }

    public void Die()
    {
        animator.SetTrigger(DEATH);
        RemoveAimTarget();
    }

    public void Respawn()
    {
        animator.SetTrigger(RESPAWN);
    }

    public void FootLeft()
    {
        onFootEvent?.Invoke(true);
    }

    public void FootRight()
    {
        onFootEvent?.Invoke(false);
    }
}
