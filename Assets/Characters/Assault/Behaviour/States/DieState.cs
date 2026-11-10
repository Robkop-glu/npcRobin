using FSM;
using UnityEngine;


[CreateAssetMenu(fileName = "DieState", menuName = "FSM/States/Example/Die State", order = 0)]
public class DieState : StateBase
{
    public override void Enter()
    {
        base.Enter();
        Executor.CharacterMotor.Freeze();
        Executor.CombatController.enabled = false;
        Executor.AnimationController.Die();
        
        if (ParentGameObject.TryGetComponent(out Targeter targeter))
        {
            targeter.ClearAll();
        }
    }
}
