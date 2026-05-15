class WaitForPlayersState : FsmState<UIMenu>
{
    public override void OnEnter()
    {
        owner.WaitForPlayersBackButton.onClick.AddListener(OnBackButtonClick);
        owner.WaitForPlayersPanel.SetActive(true);
    }

    public override void OnExit()
    {
        owner.WaitForPlayersPanel.SetActive(false);
        owner.WaitForPlayersBackButton.onClick.RemoveListener(OnBackButtonClick);
    }

    private void OnBackButtonClick()
    {
        owner.OnGoToMainMenu?.Invoke();
    }
}