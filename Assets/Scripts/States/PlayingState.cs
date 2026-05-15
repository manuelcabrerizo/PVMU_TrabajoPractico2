class PlayingState : FsmState<UIMenu>
{
    public override void OnEnter()
    {
        owner.PlayingPanel.SetActive(true);
    }

    public override void OnExit()
    {
        owner.PlayingPanel.SetActive(false);
    }
}