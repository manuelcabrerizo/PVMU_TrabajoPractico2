class PlayingState : FsmState<UIMenu>
{
    GameManager GameManager => ServiceProvider.Instance.GetService<GameManager>();

    public override void OnEnter()
    {
        owner.PlayingPanel.SetActive(true);
        GameManager.StartGame();
    }

    public override void OnExit()
    {
        owner.PlayingPanel.SetActive(false);
    }
}