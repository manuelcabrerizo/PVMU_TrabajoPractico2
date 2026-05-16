class MainMenuState : FsmState<UIManager>
{
    public override void OnEnter()
    {
        owner.MainMenuCreateButton.onClick.AddListener(OnCreateButtonClick);
        owner.MainMenuJoinButton.onClick.AddListener(OnJoinButtonClick);
        owner.MainMenuExitButton.onClick.AddListener(OnExitButtonClick);
        owner.MainMenuPanel.SetActive(true);
    }

    public override void OnExit()
    {
        owner.MainMenuPanel.SetActive(false);
        owner.MainMenuExitButton.onClick.RemoveListener(OnExitButtonClick);
        owner.MainMenuJoinButton.onClick.RemoveListener(OnJoinButtonClick);
        owner.MainMenuCreateButton.onClick.RemoveListener(OnCreateButtonClick);
    }

    private void OnCreateButtonClick()
    {
        owner.OnGoToCreateSession?.Invoke();
    }

    private void OnJoinButtonClick()
    { 
        owner.OnGoToJoinSession?.Invoke();
    }

    private void OnExitButtonClick()
    {
        // TODO: exit!
    }
}