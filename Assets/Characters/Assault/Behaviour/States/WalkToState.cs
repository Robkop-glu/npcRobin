using FSM;
using UnityEngine;

[CreateAssetMenu(fileName ="WalkToState", menuName = "FSM/States/Example/Walk To Random Position", order = 1)]
public class WalkToState : StateBase
{
    public override void Enter()
    {
        Vector2 randomCirclePos = Random.insideUnitCircle * 20f;
        Vector3 randomPosition = ParentGameObject.transform.position + new Vector3(randomCirclePos.x, 0f, randomCirclePos.y);

        Executor.CharacterMotor.MoveToPosition(randomPosition, CharacterMotor.MovementState.Walk);
    }
}
