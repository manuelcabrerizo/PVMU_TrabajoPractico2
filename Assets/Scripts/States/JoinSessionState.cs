class JoinSessionState : FsmState<UIMenu>
{
    public override void OnEnter()
    {
        owner.JoinSessionBackButton.onClick.AddListener(OnBackButtonClick);
        owner.JoinSessionPanel.SetActive(true);
    }

    public override void OnExit()
    {
        owner.JoinSessionPanel.SetActive(false);
        owner.JoinSessionBackButton.onClick.RemoveListener(OnBackButtonClick);
    }

    private void OnBackButtonClick()
    {
        owner.OnGoToMainMenu?.Invoke();
    }
}

