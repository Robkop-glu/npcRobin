using System;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [Serializable]
    public class Transition
    {
        [Tooltip("Display name for the transition. Only used in the editor.")]
        public string name = "Transition Name";

        [Tooltip("The states from which we can transition. Leave empty to allow for any state.")]
        [SerializeField] private List<StateBase> from;

        [Tooltip("The state we will transition to when all conditions are met.")]
        public StateBase to;

        [Tooltip("The list of conditions that need to be met in order for this transition to come into effect.")]
        public List<Condition> conditions;

        [Tooltip("Whether this transition allows a state to transition to itself.")]
        [SerializeField] private bool allowTransitionToSameState = false;

        [Tooltip("A disabled transition will always evaluate to false. This field is there mostly for debugging.")]
        [SerializeField] private bool disabled = false;

        /// <summary>
        /// The statemachine this transition is a part of. This is mostly used for the transition to get variables
        /// from its parent.
        /// </summary>
        public Statemachine ParentStatemachine { get; private set; }

        /// <summary> Initialize this transition. Called once at statemachine startup. Also duplicates conditions. </summary>
        /// <param name="executor"></param>
        public virtual void Initialize(StatemachineExecutor executor, Statemachine parent)
        {
            ParentStatemachine = parent;

            if (to == null)
            {
                Debug.LogError("Transition " + name + " is invalid, since no 'to' state has been defined!");
            }

            if (conditions == null || conditions.Count == 0)
            {
                Debug.LogWarning("Transition " + name + " has no conditions, meaning it will always be valid for transitioning!");
            }

            for (int i = 0; i < conditions.Count; i++)
            {
                Condition condition = conditions[i];
                conditions[i] = UnityEngine.Object.Instantiate(condition);
                conditions[i].Initialize(executor);
            }
        }

        /// <summary> Called when this transition is relevant for the current state of the statemachine. 
        /// Use this to setup or reset the transition and conditions before they are used. </summary>
        public virtual void Prime()
        {
            foreach (var condition in conditions)
            {
                condition.Prime();
            }
        }

        /// <summary> Evaluate this transition, checking whether its valid to use for transitioning. </summary>
        /// <returns> Whether the transition is satisfied; all its conditions evaluate to true. </returns>
        public virtual bool Evaluate()
        {
            if (disabled) return false;

            foreach (var condition in conditions)  if (!condition.Evaluate()) return false;
            return true;
        }

        /// <summary> Check whether this transition supports a state for transitioning away from. </summary>
        /// <param name="state"> The state to check for. </param>
        /// <returns> Whether this transitions supports transitioning away from the state. Always returns true when 'from' list is empty. </returns>
        public bool CanTransitionFromState(StateBase state)
        {
            // Check whether we allow transitioning to the state regarding the  'to' / target state.
            // If we allow to transition to the same state, always set this to true.
            // Otherwise, the current state and the target state must be different.
            bool allowedRegardingTargetState = allowTransitionToSameState || state != to;

            if (from == null || from.Count == 0) return allowedRegardingTargetState;
            if (from.Contains(state)) return allowedRegardingTargetState;
            return false;
        }

        public void ReplaceReferences(StateBase original, StateBase replacement)
        {
            if (to == original) to = replacement;
            for (int i = 0; i < from.Count; i++)
            {
                if (from[i] == original) from[i] = replacement;
            }
        }

        public Transition Copy()
        {
            Transition copy = new()
            {
                name = name,
                from = from != null ? new List<StateBase>(from) : null,
                to = to,
                allowTransitionToSameState = allowTransitionToSameState,
                disabled = disabled,
                ParentStatemachine = ParentStatemachine
            };

            if (conditions != null)
            {
                copy.conditions = new List<Condition>(conditions.Count);
                foreach (var condition in conditions)
                {
                    copy.conditions.Add(UnityEngine.Object.Instantiate(condition));
                }
            }

            return copy;
        }
    }
}

