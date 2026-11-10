using FSM;
using UnityEngine;

[CreateAssetMenu(fileName = "ShootAtTargetState", menuName = "FSM/States/Example/Shoot At Target", order = 3)]
public class ShootAtTargetState : StateBase
{
    public override void Enter()
    {
        base.Enter();
        
    }

    public override void UpdateState(float deltaTime)
    {
        Executor.CombatController.TryShootAtPrimaryTarget();

        //ParentStateMachine.CharacterMotor.StopMovement();
        if (Executor.CombatController.HasPriorityTarget)
        {
            Transform targetTransform = Executor.CombatController.TopPriorityInfo.TargetObject.transform;
            if (targetTransform != Executor.CharacterMotor.DesiredLookAtTransform)
            {
                Executor.CharacterMotor.SetLookAtTarget(targetTransform);
            }
        }
    }

    public override void Exit()
    {
        Executor.CharacterMotor.SetLookAtTarget(null);
        base.Exit();
    }
}
