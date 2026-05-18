public class MatchEndState : FsmState<StateManager>
{
    private GameManager GameManager => ServiceProvider.Instance.GetService<GameManager>();

    public MatchEndState(StateManager owner) : base(owner)
    {
        owner.MatchEndBackButton.onClick.AddListener(OnBackButtonClick);
    }

    public override void Dispose()
    {
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
    }
}