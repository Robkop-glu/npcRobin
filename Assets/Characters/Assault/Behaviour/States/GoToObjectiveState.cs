using FSM;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName ="GoToObjective", menuName = "FSM/States/Example/GoToObjective", order = 2)]
public class GoToObjectiveState : StateBase
{
    [SerializeField] private float maxRandomRange = 7.5f;

    public override void Enter()
    {
        Vector2 randomXZOffset = Random.insideUnitCircle * maxRandomRange;
        Vector3 targetPosition = ObjectiveManager.Instance.CapturePointPosition + new Vector3(randomXZOffset.x, 0f, randomXZOffset.y);
        Executor.CharacterMotor.MoveToPosition(targetPosition, CharacterMotor.MovementState.Run);
    }
}
