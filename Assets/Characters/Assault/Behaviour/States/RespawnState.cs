using FSM;
using UnityEngine;

[CreateAssetMenu(fileName = "Respawn State", menuName = "FSM/States/Example/Respawn State")]
public class RespawnState : StateBase
{
    public override void Enter()
    {
        Executor.HealthController.Respawn();

        Executor.AnimationController.Respawn(); // Set the right animation

        // Teleport the character to the right position
        int teamIndex = ParentGameObject.GetComponent<CharacterInfo>().teamIndex;
        Executor.CharacterMotor.Teleport(SpawnManager.Instance.GetSpawnPoint(teamIndex));

        // Unfreeze the character so it can move again.
        Executor.CharacterMotor.Unfreeze();

        Executor.CombatController.enabled = true;

        base.Enter();
    }
}
