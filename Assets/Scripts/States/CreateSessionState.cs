using UnityEngine;

class CreateSessionState : FsmState<UIMenu>
{
    private NetworkManager NetworkManager => ServiceProvider.Instance.GetService<NetworkManager>();

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

