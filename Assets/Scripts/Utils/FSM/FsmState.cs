public abstract class FsmState<T>
{
    protected T owner;
    public void Initialize(T owner)
    {
        this.owner = owner;
    }
    public abstract void OnEnter();
    public abstract void OnExit();
}
