class CreateSessionState : FsmState<UIMenu>
{
    public override void OnEnter()
    {
        owner.CreateSessionCreateButton.onClick.AddListener(OnCreateButtonClick);
        owner.CreateSessionBackButton.onClick.AddListener(OnBackButtonClick);
        owner.CreateSessionPanel.SetActive(true);
    }

    public override void OnExit()
    {
        owner.CreateSessionPanel.SetActive(false);
        owner.CreateSessionBackButton.onClick.RemoveListener(OnBackButtonClick);
        owner.CreateSessionCreateButton.onClick.RemoveListener(OnCreateButtonClick);
    }

    private void OnCreateButtonClick()
    {
        owner.OnGoToWaitForPlayers?.Invoke();
    }

    private void OnBackButtonClick()
    { 
        owner.OnGoToMainMenu?.Invoke();
    }
}

