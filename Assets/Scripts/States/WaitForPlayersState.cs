class WaitForPlayersState : FsmState<UIMenu>
{
    public override void OnEnter()
    {
        owner.WaitForPlayersBackButton.onClick.AddListener(OnBackButtonClick);
        owner.WaitForPlayersPanel.SetActive(true);
        owner.OnGoToPlaying?.Invoke();
    }

    public override void OnExit()
    {
        owner.WaitForPlayersPanel.SetActive(false);
        owner.WaitForPlayersBackButton.onClick.RemoveListener(OnBackButtonClick);
    }

    private void OnBackButtonClick()
    {
        // TODO: Disconect
        owner.OnGoToMainMenu?.Invoke();
    }
}
