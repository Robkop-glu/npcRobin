using FSM;
using UnityEngine;

[CreateAssetMenu(fileName ="IdleState", menuName = "FSM/States/Example/Idle State", order = 0)]
public class IdleState : StateBase
{
    public override void Enter()
    {
        Executor.AnimationController.SetAimTarget(null);
        Executor.CharacterMotor.StopMovement();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
