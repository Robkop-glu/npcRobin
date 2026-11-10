using UnityEngine;


[CreateAssetMenu(fileName = "Death Condition", menuName = "FSM/Conditions/Example/Death Condition")]
public class OnDeathCondition : Condition
{
    public override bool Evaluate()
    {
        return !ParentStateMachine.HealthController.Alive;
    }
}
