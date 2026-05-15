class WaitForPlayersState : FsmState<UIMenu>
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    private int playerCount = 0;
    private int targetPlayerCount = 0;

    public override void OnEnter()
    {
        EventBus.Subscribe<OnPlayerJoinEvent>(OnPlayerJoin);
        owner.WaitForPlayersBackButton.onClick.AddListener(OnBackButtonClick);
        owner.WaitForPlayersPanel.SetActive(true);
    }

    public override void OnExit()
    {
        owner.WaitForPlayersPanel.SetActive(false);
        owner.WaitForPlayersBackButton.onClick.RemoveListener(OnBackButtonClick);
        EventBus.Unsubscribe<OnPlayerJoinEvent>(OnPlayerJoin);
    }

    public override void OnUpdate(float deltaTime)
    {
        if (playerCount > 0 && playerCount == targetPlayerCount)
        {
            owner.OnGoToPlaying?.Invoke();
        }
    }

    private void OnPlayerJoin(in OnPlayerJoinEvent onPlayerJoinEvent)
    {
        owner.ClearPlayerImages();
        owner.CreatePlayerImages(onPlayerJoinEvent.PlayerCount);
        playerCount = onPlayerJoinEvent.PlayerCount;
        targetPlayerCount = onPlayerJoinEvent.TargetPlayerCount;
    }

    private void OnBackButtonClick()
    {
        // TODO: Disconect
        owner.OnGoToMainMenu?.Invoke();
    }
}