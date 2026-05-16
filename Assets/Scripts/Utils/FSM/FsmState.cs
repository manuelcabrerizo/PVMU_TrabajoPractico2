using System;

public abstract class FsmState<T> : IDisposable
{
    protected T owner;

    public FsmState(T owner)
    {
        this.owner = owner;
    }
    public abstract void Dispose();
    public abstract void OnEnter();
    public abstract void OnExit();
}
