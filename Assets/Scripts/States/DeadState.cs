using UnityEngine;

public class DeadState : FsmState<StateManager>
{
    private GameManager GameManager => ServiceProvider.Instance.GetService<GameManager>();

    public DeadState(StateManager owner) : base(owner)
    {
        owner.DeadRespawnButton.onClick.AddListener(OnRespawnButtonClick);
        owner.DeadBackButton.onClick.AddListener(OnBackButtonClick);
    }

    public override void Dispose()
    {
        owner.DeadBackButton.onClick.RemoveListener(OnBackButtonClick);
        owner.DeadRespawnButton.onClick.RemoveListener(OnRespawnButtonClick);
    }

    public override void OnEnter()
    {
        owner.DeadPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public override void OnExit()
    {
        owner.DeadPanel.SetActive(false);
    }

    private void OnRespawnButtonClick()
    {
        owner.OnGoToPlaying?.Invoke();
        GameManager.RevivePlayer(GameManager.LocalPlayer);
    }

    private void OnBackButtonClick()
    {
        owner.OnGoToMainMenu?.Invoke();
    }
}