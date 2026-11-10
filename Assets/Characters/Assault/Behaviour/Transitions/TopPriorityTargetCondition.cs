using UnityEngine;

namespace FSM
{
    [CreateAssetMenu(fileName = "TopPriorityTargetCondition", menuName = "FSM/Conditions/Example/TopPriorityTargetCondition")]
    public class TopPriorityTargetCondition : Condition
    {
        public enum TeamPreference
        {
            None = 0,
            Same = 1,
            Different = 2
        }

        [SerializeField] private bool trueOnNoTarget = false;
        [SerializeField] private TeamPreference teamPreference = TeamPreference.None;

        public override bool Evaluate()
        {
            // If we have a target to evaluate AND we don't want to return true on no target, check the teams.
            if (ParentStateMachine.CombatController.HasPriorityTarget && !trueOnNoTarget)
            {
                if (teamPreference == TeamPreference.None) return true;

                GameObject targetObject = ParentStateMachine.CombatController.TopPriorityInfo.TargetObject;

                // If we have a team preference, but our target doesn't have a team, it doesn't matter.
                if (!ParentStateMachine.TryGetComponent(out CharacterInfo ourInfo) || !targetObject.TryGetComponent(out CharacterInfo theirInfo)) return true;

                if (teamPreference == TeamPreference.Same) return ourInfo.teamIndex == theirInfo.teamIndex;
                return ourInfo.teamIndex != theirInfo.teamIndex;
            }

            // If we don't want to return true when we have no target (so, normal mode), and we have no target, return false.
            else if (!trueOnNoTarget && !ParentStateMachine.CombatController.HasPriorityTarget) return false;

            // If we want to return true when we have no target (inverted mode) and we don't have a target, return true.
            else if (trueOnNoTarget && !ParentStateMachine.CombatController.HasPriorityTarget) return true;

            return false;
        }

    }
}
