using UnityEngine;

[CreateAssetMenu(fileName = "Reached Target Condition", menuName = "FSM/Conditions/Example/Reached Target Condition")]
public class ReachedTargetCondition : Condition
{
    public override bool Evaluate()
    {
        return ParentStateMachine.CharacterMotor.CurrentTargetState != CharacterMotor.TargetState.MovingTowardsTarget;
    }
}
