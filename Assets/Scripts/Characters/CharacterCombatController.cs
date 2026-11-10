using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatController : MonoBehaviour
{
    // Properties that need to be assigned in the editor
    [SerializeField] private BasicHitscanWeapon weapon;
    public BasicHitscanWeapon Weapon => weapon;

    [SerializeField] private CharacterAnimationController animationController;

    // Standard exposed fields and their related private fields.
    [SerializeField] [Range(0.1f, 10f)] 
    private float reprioritizeTargetsInterval = 1.0f;
    private float reprioritizeTargetsTimer = 0f;

    [SerializeField] private AnimationCurve accuracyFalloffOverVelocity;
    [SerializeField] private bool printDebug = false;

    // Required GetComponent fields.
    private CharacterMotor motor;
    private Targeter targeter;
    private CharacterInfo characterInfo;

    // private variables and their getters/setters
    private List<TargetingInfo> liveTargets = new();
    private List<TargetingInfo> prioritizedTargets = new();

    private TargetingInfo topPriorityInfo;
    private bool hasPriorityTarget = false;

    /// <summary> Whether we have a priority target: i.e. a target that is most important given the prioritize strategy. </summary>
    public bool HasPriorityTarget => hasPriorityTarget;

    /// <summary> The priority target: i.e. the target that is most important given the prioritize strategy. </summary>
    public TargetingInfo TopPriorityInfo
    {
        get { return topPriorityInfo; }
        private set
        {
            if (topPriorityInfo == value) return;
            topPriorityInfo = value;
            hasPriorityTarget = topPriorityInfo != null;

            if (printDebug)
            {
                if (topPriorityInfo != null) Debug.Log("Got a new top priority target " + topPriorityInfo.TargetObject.name);
                else Debug.Log("Got a new top priority target: null");
            }

        }
    }
  
    public enum PrioritizeStrategy
    {
        Closest = 0,
        Farthest = 1,
        MostRecent = 2,
        None = 3
    }

    [SerializeField] private PrioritizeStrategy currentPrioritizeStrategy = PrioritizeStrategy.Closest;

    private void Awake()
    {
        motor = GetComponent<CharacterMotor>();
        targeter = GetComponent<Targeter>();
        characterInfo = GetComponent<CharacterInfo>();
        targeter.onTargetFound.AddListener(OnTargetFound);
        targeter.onTargetLost.AddListener(OnTargetLost);
        targeter.onTargetForgotten.AddListener(OnTargetForgotten);
    }

    private void Update()
    {
        reprioritizeTargetsTimer += Time.deltaTime;
        if (reprioritizeTargetsTimer >= reprioritizeTargetsInterval)
        {
            ReprioritizeTargets();
            reprioritizeTargetsTimer = 0f;
        }
    }

    private void ReprioritizeTargets()
    {
        prioritizedTargets = new List<TargetingInfo>(liveTargets);
        prioritizedTargets.Sort(new TargeterInfoComparer(currentPrioritizeStrategy, gameObject));
        TopPriorityInfo = prioritizedTargets.Count == 0 ? null : prioritizedTargets[0];
    }

    private void ForceReprioritize()
    {
        reprioritizeTargetsTimer = 0f;
        ReprioritizeTargets();
    }

    private void OnTargetFound(TargetingInfo info)
    {
        // We ignore targets of the same team. 
        if (info.TargetObject.TryGetComponent(out CharacterInfo detectedCharacterInfo) && 
            detectedCharacterInfo.teamIndex == characterInfo.teamIndex)
        {
            return;
        }

        liveTargets.Add(info);
        ForceReprioritize();
    }

    private void OnTargetLost(TargetingInfo info)
    {
        liveTargets.Remove(info);
        ForceReprioritize();
    }

    private void OnTargetForgotten(TargetingInfo info)
    {
        liveTargets.Remove(info);
        ForceReprioritize();
    }

    public bool TryShootAtPrimaryTarget()
    {
        if (!hasPriorityTarget) return false;

        animationController.SetAimTarget(TopPriorityInfo.TargetObject.transform);

        Vector3 targetPosition = TopPriorityInfo.TargetObject.transform.position;
        if (TopPriorityInfo.TargetObject.TryGetComponent(out CharacterController cc)) targetPosition += cc.center;
        Vector3 direction = (targetPosition - weapon.firePoint.transform.position).normalized;

        // Evaluate the accuracy curve. The higher the velocity, the lower the accuracy
        float accuracy = accuracyFalloffOverVelocity.Evaluate(motor.MeasuredVelocity / motor.MaxMovementVelocity);
        weapon.TryShoot(direction, accuracy, TopPriorityInfo.TargetObject);
        return true;
    }

    public void TryShoot(Vector3 direction)
    {
        // Evaluate the accuracy curve. The higher the velocity, the lower the accuracy
        float accuracy = accuracyFalloffOverVelocity.Evaluate(motor.MeasuredVelocity / motor.MaxMovementVelocity);
        weapon.TryShoot(direction, accuracy);
    }

    public void OnDisable()
    {
        TopPriorityInfo = null;
        liveTargets.Clear();
        prioritizedTargets.Clear();
    }
}

public class TargeterInfoComparer : IComparer<TargetingInfo>
{
    private CharacterCombatController.PrioritizeStrategy strategy;
    private GameObject subject;

    public TargeterInfoComparer(CharacterCombatController.PrioritizeStrategy strategy, GameObject subject) 
    { 
        this.strategy = strategy; 
        this.subject = subject;
    }

    public int Compare(TargetingInfo x, TargetingInfo y)
    {
        return strategy switch
        {
            CharacterCombatController.PrioritizeStrategy.Closest => CompareClosest(x, y),
            CharacterCombatController.PrioritizeStrategy.Farthest => CompareFarthest(x, y),
            CharacterCombatController.PrioritizeStrategy.MostRecent => CompareMostRecent(x, y),
            _ => 0,
        };
    }

    private int CompareClosest(TargetingInfo x, TargetingInfo y)
    {
        // Return 1 if distance X < distance Y, since less is better!
        float distanceX = (subject.transform.position - x.LastConfirmedPosition).sqrMagnitude;
        float distanceY = (subject.transform.position - y.LastConfirmedPosition).sqrMagnitude;
        if (Mathf.Approximately(distanceX, distanceY)) return 0;
        if (distanceX < distanceY) return 1;
        return -1;
    }

    private int CompareFarthest(TargetingInfo x, TargetingInfo y)
    {
        // Return 1 if distance X < distance Y, since less is better!
        float distanceX = (subject.transform.position - x.LastConfirmedPosition).sqrMagnitude;
        float distanceY = (subject.transform.position - y.LastConfirmedPosition).sqrMagnitude;
        if (Mathf.Approximately(distanceX, distanceY)) return 0;
        if (distanceX < distanceY) return -1;
        return 1;
    }

    private int CompareMostRecent(TargetingInfo x, TargetingInfo y) 
    {
        if (Mathf.Approximately(x.TimeSinceLastSeen, y.TimeSinceLastSeen)) return 0;
        if (x.TimeSinceLastSeen < y.TimeSinceLastSeen) return 1;
        return -1;
    }
}
