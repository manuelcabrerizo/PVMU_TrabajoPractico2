class MainMenuState : FsmState<UIManager>
{

    public MainMenuState(UIManager owner) : base(owner)
    {
        owner.MainMenuCreateButton.onClick.AddListener(OnCreateButtonClick);
        owner.MainMenuJoinButton.onClick.AddListener(OnJoinButtonClick);
        owner.MainMenuExitButton.onClick.AddListener(OnExitButtonClick);
    }
    public override void Dispose()
    {
        owner.MainMenuExitButton.onClick.RemoveListener(OnExitButtonClick);
        owner.MainMenuJoinButton.onClick.RemoveListener(OnJoinButtonClick);
        owner.MainMenuCreateButton.onClick.RemoveListener(OnCreateButtonClick);
    }

    public override void OnEnter()
    {
        owner.MainMenuPanel.SetActive(true);
    }

    public override void OnExit()
    {
        owner.MainMenuPanel.SetActive(false);
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