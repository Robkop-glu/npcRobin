using System;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [RequireComponent(typeof(CharacterMotor))]
    [RequireComponent(typeof(CharacterCombatController))]
    [RequireComponent(typeof(HealthController))]
    public sealed class StatemachineExecutor : MonoBehaviour
    {
        /// <summary> Dictates when a statemachine is updated. </summary>
        public enum UpdateProtocol
        {
            /// <summary> Update the statemachine during every frame. </summary>
            Update = 0,

            /// <summary> Update the statemachine after every frame. </summary>
            LateUpdate = 1,

            /// <summary> Update the statemachine manually, for example, using an external timer. </summary>
            Manual = 2
        }

        [SerializeField] private UpdateProtocol updateProtocol = UpdateProtocol.Update;

        [SerializeField] private Statemachine statemachine;

        public StateBase CurrentState { get; private set; }
        public bool HasCurrentState { get; private set;  }

        private readonly List<Transition> relevantTransitions = new();

        private CharacterMotor characterMotor;
        public CharacterMotor CharacterMotor => characterMotor;

        private CharacterCombatController combatController;
        /// <summary> The combat controller of the character that this statemachine is attached to. </summary>
        public CharacterCombatController CombatController => combatController;

        [SerializeField] private CharacterAnimationController animationController;
        public CharacterAnimationController AnimationController => animationController;

        private HealthController healthController;
        public HealthController HealthController => healthController;

        private void Awake()
        {
            characterMotor = GetComponent<CharacterMotor>();
            combatController = GetComponent<CharacterCombatController>();
            healthController = GetComponent<HealthController>();
        }

        private void Start()
        {
            statemachine = statemachine.Copy() as Statemachine;
            statemachine.Initialize(gameObject, this, null);

            if (statemachine.InitialStateIndex >= 0 && statemachine.InitialStateIndex < statemachine.States.Count)
            {
                GoToState(statemachine.States[statemachine.InitialStateIndex]);
            }
        }

        private void Update()
        {
            if (HasCurrentState && updateProtocol == UpdateProtocol.Update) UpdateStateMachine();
        }

        private void LateUpdate()
        {
            if (HasCurrentState && updateProtocol == UpdateProtocol.LateUpdate) UpdateStateMachine();
        }

        private void UpdateStateMachine()
        {
            CurrentState.UpdateState(Time.deltaTime);

            if (EvaluateTransitions(out StateBase nextState))
            {
                GoToState(nextState);
            }
        }

        /// <summary> Go to a certain state, exiting the current state and entering the next. </summary>
        /// <param name="nextState"> The state to go to. </param>
        private void GoToState(StateBase nextState)
        {
            // Order is important!
            ExitCurrentState();
            EnterState(nextState);
            RebuildRelevantTransitions();
        }

        private void ExitCurrentState()
        {
            if (!HasCurrentState) return;

            CurrentState.Exit();
            CurrentState = null;
            HasCurrentState = false;
        }

        private void EnterState(StateBase nextState)
        {
            if (nextState == null)
            {
                Debug.LogWarning("Statemachine " + gameObject.name + " got null state to enter. Aborting...");
                return;
            }

            if (!statemachine.States.Contains(nextState))
            {
                Debug.LogWarning("Statemachine " + gameObject.name + " got state to enter that wasn't" +
                    " present in states list: " + nextState.name);
            }

            CurrentState = nextState;
            HasCurrentState = true;
            CurrentState.Enter();
        }

        private void RebuildRelevantTransitions()
        {
            relevantTransitions.Clear();
            foreach(Transition transition in statemachine.Transitions)
            {
                if (transition.CanTransitionFromState(CurrentState))
                {
                    relevantTransitions.Add(transition);
                    transition.Prime();
                }
            }
            
            if (relevantTransitions.Count == 0)
            {
                Debug.LogWarning("There are no transitions that allow the statemachine to continue. It will stay in this state.");
            }
        }

        /// <summary> Evaluate the relevant transitions that allow to continue to another state. </summary>
        /// <param name="newState"> The state that we can transition to. </param>
        /// <returns> Whether any state should be transitioned to. </returns>
        private bool EvaluateTransitions(out StateBase newState)
        {
            foreach(Transition transition in relevantTransitions)
            {
                if (transition.Evaluate())
                {
                    newState = transition.to;
                    return true;
                }
            }

            newState = null;
            return false;
        }
    }
}


