using UnityEngine;

public class JoinSessionState : FsmState<StateManager>
{
    private NetworkManager NetworkManager => ServiceProvider.Instance.GetService<NetworkManager>();
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    public JoinSessionState(StateManager owner) : base(owner)
    {
        EventBus.Subscribe<OnSessionListUpdatedEvent>(OnSessionListUpdated);
        owner.JoinSessionBackButton.onClick.AddListener(OnBackButtonClick);
    }
    public override void Dispose()
    {
        owner.JoinSessionBackButton.onClick.RemoveListener(OnBackButtonClick);
        EventBus.Unsubscribe<OnSessionListUpdatedEvent>(OnSessionListUpdated);
    }

    public override void OnEnter()
    {
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
    }

    private void OnSessionListUpdated(in OnSessionListUpdatedEvent onSessionListUpdatedEvent)
    {
        if (owner.CurrentState != owner.JoinSessionState)
            return;

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