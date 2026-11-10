using FSM;
using UnityEngine;

public abstract class Condition : ScriptableObject
{
    private StatemachineExecutor parentStateMachine;

    /// <summary> The statemachine that contains and manages this transition and this condition. </summary>
    protected StatemachineExecutor ParentStateMachine => parentStateMachine;

    public virtual void Initialize(StatemachineExecutor statemachine)
    {
        parentStateMachine = statemachine;
    }

    /// <summary>
    /// 'Prime' this condition. Can be used to reset a condition before its evaluated for the current statemachine state.
    /// </summary>
    public virtual void Prime()
    {
        return;
    }

    public abstract bool Evaluate();
}