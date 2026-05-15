using UnityEngine;

class JoinSessionState : FsmState<UIMenu>
{
    private NetworkManager NetworkManager => ServiceProvider.Instance.GetService<NetworkManager>();
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    public override void OnEnter()
    {
        EventBus.Subscribe<OnSessionListUpdatedEvent>(OnSessionListUpdated);
        owner.JoinSessionBackButton.onClick.AddListener(OnBackButtonClick);
        owner.JoinSessionPanel.SetActive(true);
        NetworkManager.JoinLobby(
            () =>
            {
                Debug.Log("Wellcome to the Lobby!");
            },
            (result) =>
            {
                Debug.Log($"Failed to Join: {result.ShutdownReason}");
            });
    }

    public override void OnExit()
    {
        owner.JoinSessionPanel.SetActive(false);
        owner.JoinSessionBackButton.onClick.RemoveListener(OnBackButtonClick);
        EventBus.Unsubscribe<OnSessionListUpdatedEvent>(OnSessionListUpdated);

    }

    private void OnSessionListUpdated(in OnSessionListUpdatedEvent onSessionListUpdatedEvent)
    {
        owner.ClearSessionsButtons();
        owner.CreateSessionButtons(onSessionListUpdatedEvent.SessionInfoList, OnSessionButtonClick);
    }

    private void OnSessionButtonClick(string session)
    {
        NetworkManager.StartClient(session,
            () =>
            {
                owner.OnGoToWaitForPlayers?.Invoke();
            },
            (result) =>
            {
                Debug.Log($"Failed to Start Client: {result.ShutdownReason}");
            });
    }

    private void OnBackButtonClick()
    {
        // TODO: Leave the lobby
        owner.OnGoToMainMenu?.Invoke();
    }
}