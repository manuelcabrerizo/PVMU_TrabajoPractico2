using UnityEngine.Events;

public class FsmStateMachine<T>
{
    private DobleEntryTable<FsmState<T>, UnityEvent, FsmState<T>> transitionsTable;
    private FsmState<T> currentState;
    public FsmStateMachine(FsmState<T>[] states, UnityEvent[] transitions, FsmState<T> startState)
    {
        transitionsTable = new DobleEntryTable<FsmState<T>, UnityEvent, FsmState<T>>(states, transitions);
        currentState = startState;
        currentState.OnEnter();
    }

    public void OnTriggerTransition(UnityEvent transition)
    {
        FsmState<T> targetState = transitionsTable[currentState, transition];
        if (targetState == null)
            return;

        currentState.OnExit();
        currentState = targetState;
        currentState.OnEnter();
    }

    public void ConfigureTransition(FsmState<T> from, FsmState<T> to, UnityEvent transition)
    {
        transitionsTable[from, transition] = to;
        transition.AddListener(() => OnTriggerTransition(transition));
    }
}