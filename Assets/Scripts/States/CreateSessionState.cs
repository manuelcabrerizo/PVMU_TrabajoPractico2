using UnityEngine;

class CreateSessionState : FsmState<UIManager>
{
    private NetworkManager NetworkManager => ServiceProvider.Instance.GetService<NetworkManager>();
    public CreateSessionState(UIManager owner) : base(owner)
    {
        owner.CreateSessionCreateButton.onClick.AddListener(OnCreateButtonClick);
        owner.CreateSessionBackButton.onClick.AddListener(OnBackButtonClick);
    }
    public override void Dispose()
    {
        owner.CreateSessionBackButton.onClick.RemoveListener(OnBackButtonClick);
        owner.CreateSessionCreateButton.onClick.RemoveListener(OnCreateButtonClick);
    }

    public override void OnEnter()
    {
        owner.CreateSessionPanel.SetActive(true);
    }

    public override void OnExit()
    {
        owner.CreateSessionPanel.SetActive(false);
    }

    private void OnCreateButtonClick()
    {
        if (owner.CreateSessionInputField.text.Length > 0)
        {
            NetworkManager.StartHost(owner.CreateSessionInputField.text,
                () => {
                    owner.OnGoToWaitForPlayers?.Invoke();
                },
                (result) => {
                    Debug.Log($"Failed to Start: {result.ShutdownReason}");
                });
        }
    }

    private void OnBackButtonClick()
    { 
        owner.OnGoToMainMenu?.Invoke();
    }
}

