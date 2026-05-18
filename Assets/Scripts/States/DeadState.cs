public class DeadState : FsmState<StateManager>
{
    private GameManager GameManager => ServiceProvider.Instance.GetService<GameManager>();
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    public DeadState(StateManager owner) : base(owner)
    {
        owner.DeadRespawnButton.onClick.AddListener(OnRespawnButtonClick);
        EventBus.Subscribe<OnMatchEndEvent>(OnMatchEnd);
    }

    public override void Dispose()
    {
        EventBus.Subscribe<OnMatchEndEvent>(OnMatchEnd);
        owner.DeadRespawnButton.onClick.RemoveListener(OnRespawnButtonClick);
    }

    public override void OnEnter()
    {
        owner.DeadPanel.SetActive(true);
    }

    public override void OnExit()
    {
        owner.DeadPanel.SetActive(false);
    }

    private void OnRespawnButtonClick()
    {
        owner.OnGoToPlaying?.Invoke();
        GameManager.RevivePlayer(GameManager.LocalPlayer);
    }

    private void OnMatchEnd(in OnMatchEndEvent callback)
    {
        if (owner.CurrentState != owner.DeadState)
            return;
        owner.OnGoToMatchEnd?.Invoke();
    }
}
