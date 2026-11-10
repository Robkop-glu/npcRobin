using UnityEngine;

namespace FSM
{
    public abstract class StateBase : ScriptableObject
    {
        /// <summary>
        /// Whether we are currently in this state.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// The GameObject of the statemachine this state is a part of. Use this to get components or properties.
        /// </summary>
        public GameObject ParentGameObject { get; private set; }

        /// <summary>
        /// The statemachine this state is a part of. Use this to get access to components or change variables.
        /// </summary>
        public StatemachineExecutor Executor { get; private set; }

        public Statemachine ParentStatemachine { get; private set; }

        /// <summary>
        /// Call this to initialize the state. 
        /// </summary>
        public virtual void Initialize(GameObject parentGameObject, StatemachineExecutor executor, Statemachine parentStatemachine)
        {
            ParentGameObject = parentGameObject;
            Executor = executor;
            ParentStatemachine = parentStatemachine;
        }

        /// <summary> This is called right as the state is entered by the statemachine. </summary>
        public virtual void Enter()
        {
            IsActive = true;
        }

        /// <summary>
        /// This method is called every time the statemachine updates.
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void UpdateState(float deltaTime)
        {
            return;
        }

        public virtual void Exit()
        {
            IsActive = false;
        }

        public virtual StateBase Copy()
        {
            return Instantiate(this);
        }
    }
}

