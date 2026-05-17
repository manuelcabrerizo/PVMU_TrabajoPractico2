using UnityEngine;

public class CreateSessionState : FsmState<StateManager>
{
    private NetworkManager NetworkManager => ServiceProvider.Instance.GetService<NetworkManager>();
    public CreateSessionState(StateManager owner) : base(owner)
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
        owner.CreateSessionCreateButton.interactable = true;
        owner.CreateSessionBackButton.interactable = true;
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
            owner.CreateSessionCreateButton.interactable = false;
            owner.CreateSessionBackButton.interactable = false;
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

