using UnityEngine;

[CreateAssetMenu(fileName = "TimerCondition", menuName = "FSM/Conditions/Example/Timer Condition", order = 2)]
public class TimerCondition : Condition
{
    [Tooltip("The time threshold until this condition evaluates to true.")]
    [SerializeField] private float timeThreshold = 5.0f;

    [Tooltip("Whether to randomize the time threshold every time this condition is primed.")]
    [SerializeField] private bool randomizeTime = false;

    [Tooltip("The maximum time threshold. Only used when using randomization.")]
    [SerializeField] private float maxTimeThreshold = 10f;

    private float targetTime = 0f; 

    public override void Prime()
    {
        if (randomizeTime)
        {
            targetTime = Time.time + Random.Range(timeThreshold, maxTimeThreshold);
        }
        else
        {
            targetTime = Time.time + timeThreshold;
        }

        base.Prime();
    }

    public override bool Evaluate()
    {
        return Time.time >= targetTime;
    }
}
