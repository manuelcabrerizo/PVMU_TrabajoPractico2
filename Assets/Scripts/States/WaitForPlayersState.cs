public class WaitForPlayersState : FsmState<StateManager>
{
    private NetworkManager NetworkManager => ServiceProvider.Instance.GetService<NetworkManager>();
    private GameManager GameManager => ServiceProvider.Instance.GetService<GameManager>();
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    public WaitForPlayersState(StateManager owner) : base(owner)
    {
        EventBus.Subscribe<OnPlayerJoinEvent>(OnPlayerJoin);
        owner.WaitForPlayersBackButton.onClick.AddListener(OnBackButtonClick);
    }
    public override void Dispose()
    {
        owner.WaitForPlayersBackButton.onClick.RemoveListener(OnBackButtonClick);
        EventBus.Unsubscribe<OnPlayerJoinEvent>(OnPlayerJoin);
    }

    public override void OnEnter()
    {
        owner.WaitForPlayersPanel.SetActive(true);
        if (GameManager.IsSpawned && (GameManager.IsMatchBegin || (GameManager.CurrentPlayerCount >= GameManager.TargetPlayerCount)))
        {
            owner.OnGoToPlaying?.Invoke();
        }
    }

    public override void OnExit()
    {
        owner.ClearPlayerImages();
        owner.WaitForPlayersPanel.SetActive(false);
    }

    private void OnPlayerJoin(in OnPlayerJoinEvent onPlayerJoinEvent)
    {
        if (owner.CurrentState != owner.WaitPlayersState)
            return;

        owner.ClearPlayerImages();
        owner.CreatePlayerImages(onPlayerJoinEvent.PlayerCount);
        if (onPlayerJoinEvent.PlayerCount >= onPlayerJoinEvent.TargetPlayerCount)
        {
            owner.OnGoToPlaying?.Invoke();
        }
    }

    private void OnBackButtonClick()
    {
        NetworkManager.Disconect();
        owner.OnGoToMainMenu?.Invoke();
    }
}