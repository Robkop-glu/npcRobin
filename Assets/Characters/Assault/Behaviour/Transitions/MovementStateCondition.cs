using UnityEngine;

namespace FSM
{
    [CreateAssetMenu(fileName = "MovementStateCondition", menuName = "FSM/Conditions/Example/MovementStateCondition", order = 1)]
    public class MovementStateCondition : Condition
    {
        [SerializeField] private CharacterMotor.MovementState targetMovementState = CharacterMotor.MovementState.None;

        public override void Initialize(StatemachineExecutor statemachine)
        {
            base.Initialize(statemachine);
        }

        public override bool Evaluate()
        {
            return ParentStateMachine.CharacterMotor.CurrentMovementState == targetMovementState;
        }

    }
}


