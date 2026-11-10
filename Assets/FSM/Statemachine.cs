using FSM;
using System.Collections.Generic;
using UnityEngine;


namespace FSM
{
    [CreateAssetMenu(fileName = "Statemachine", menuName = "FSM/Statemachine", order = 1)]
    public class Statemachine : StateBase
    {
        [SerializeField] private List<StateBase> states = new();

        [SerializeField] private int initialStateIndex = 0;
        public int InitialStateIndex => initialStateIndex;

        public List<StateBase> States => states;

        [SerializeField] private List<Transition> transitions = new();

        public IReadOnlyList<Transition> Transitions => transitions;

        public override void Initialize(GameObject parentGameObject, StatemachineExecutor executor, Statemachine parent)
        {
            // Order is important
            base.Initialize(parentGameObject, executor, parent);
            InitializeTransitions();
            InitializeStates();
        }

        /// <summary>
        /// Duplicates the states to allow every instance to have its own states.
        /// </summary>
        private void InitializeStates()
        {
            // Loop through all states and replace it in the list with a copy.
            for (int i = 0; i < states.Count; i++)
            {
                StateBase original = states[i];
                states[i] = states[i].Copy();
                states[i].name = original.name + " (Clone/Copy)";
                states[i].Initialize(ParentGameObject, Executor, this);

                // Make sure the transitions have the correct references as well.
                foreach (Transition transition in transitions)
                {
                    transition.ReplaceReferences(original, states[i]);
                }

            }
        }

        private void InitializeTransitions()
        {
            foreach (Transition transition in transitions)
            {
                transition.Initialize(Executor, this);
            }
        }

        public override StateBase Copy()
        {
            Statemachine copy = Instantiate(this);
            copy.states = new List<StateBase>(states);
            copy.transitions = new List<Transition>();

            foreach (Transition transition in transitions)
            {
                copy.transitions.Add(transition.Copy());
            }
            return copy;
        }
    }
}

