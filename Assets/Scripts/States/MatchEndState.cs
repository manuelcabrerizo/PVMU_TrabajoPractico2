public class MatchEndState : FsmState<StateManager>
{
    private NetworkManager NetworkManager => ServiceProvider.Instance.GetService<NetworkManager>();
    private GameManager GameManager => ServiceProvider.Instance.GetService<GameManager>();
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();


    public MatchEndState(StateManager owner) : base(owner)
    {
        owner.MatchEndBackButton.onClick.AddListener(OnBackButtonClick);
        EventBus.Subscribe<OnHostDisconectEvent>(OnHostDisconect);
    }

    public override void Dispose()
    {
        EventBus.Unsubscribe<OnHostDisconectEvent>(OnHostDisconect);
        owner.MatchEndBackButton.onClick.RemoveListener(OnBackButtonClick);
    }

    public override void OnEnter()
    {
        owner.MatchEndTitleText.text = 
            GameManager.LocalPlayer == GameManager.ScoreBoard[0] ? 
            "You Won!" : "You Lose!";
        owner.ShowPlayerScoreTexts(GameManager.ScoreBoard);
        owner.MatchEndPanel.SetActive(true);
    }

    public override void OnExit()
    {
        owner.MatchEndPanel.SetActive(false);
    }

    private void OnBackButtonClick()
    {
        if (GameManager.LocalPlayer.Object.HasStateAuthority)
        {
            GameManager.Rpc_RaiseOnHostDisconect();
        }
        else
        {
            NetworkManager.Disconect();
            owner.OnGoToMainMenu?.Invoke();
        }
    }

    private void OnHostDisconect(in OnHostDisconectEvent callback)
    {
        if (owner.CurrentState != owner.MatchEndState)
            return;
        NetworkManager.Disconect();
        owner.OnGoToMainMenu?.Invoke();
    }
}