using System;
using UnityEngine;

public class PlayingState : FsmState<StateManager>
{
    private GameManager GameManager => ServiceProvider.Instance.GetService<GameManager>();
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    public PlayingState(StateManager owner) : base(owner)
    {
        EventBus.Subscribe<OnCountDownChangeEvent>(OnCountDownChange);
        EventBus.Subscribe<OnMatchBeginEvent>(OnMatchBegin);
        EventBus.Subscribe<OnMatchEndEvent>(OnMatchEnd);
        EventBus.Subscribe<OnMatchTimerDownChangeEvent>(OnMatchTimerDownChange);
        EventBus.Subscribe<OnHealthChangeEvent>(OnHealthChange);
        EventBus.Subscribe<OnPlayerDieEvent>(OnPlayerDie);
        EventBus.Subscribe<OnScoreChangeEvent>(OnScoreChange);
    }

    public override void Dispose()
    {
        EventBus.Unsubscribe<OnScoreChangeEvent>(OnScoreChange);
        EventBus.Unsubscribe<OnPlayerDieEvent>(OnPlayerDie);
        EventBus.Unsubscribe<OnHealthChangeEvent>(OnHealthChange);
        EventBus.Unsubscribe<OnMatchTimerDownChangeEvent>(OnMatchTimerDownChange);
        EventBus.Unsubscribe<OnMatchEndEvent>(OnMatchEnd);
        EventBus.Unsubscribe<OnMatchBeginEvent>(OnMatchBegin);
        EventBus.Unsubscribe<OnCountDownChangeEvent>(OnCountDownChange);
    }

    public override void OnEnter()
    {
        owner.PlayingPanel.SetActive(true);
        owner.PlayingGamplayUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        if (GameManager.IsMatchBegin && !GameManager.IsInCountDown)
        {
            BeginMatch();
        }
    }

    public override void OnExit()
    {
        Cursor.lockState = CursorLockMode.None;
        owner.PlayingPanel.SetActive(false);
    }

    private void OnCountDownChange(in OnCountDownChangeEvent onCountDownChangeEvent)
    {
        if (owner.CurrentState != owner.PlayingState)
            return;
        owner.PlayingCountDown.text = onCountDownChangeEvent.CountDown.ToString();
    }

    private void OnMatchBegin(in OnMatchBeginEvent onMachBeginEvent)
    {
        if (owner.CurrentState != owner.PlayingState)
            return;
        BeginMatch();
    }

    private void OnMatchEnd(in OnMatchEndEvent onMachEndEvent)
    {
        if (owner.CurrentState != owner.PlayingState)
            return;
        owner.PlayingGamplayUI.SetActive(false);
        owner.PlayingCountDown.gameObject.SetActive(true);
    }

    private void OnHealthChange(in OnHealthChangeEvent onHealthChangeEvent)
    {
        if (owner.CurrentState != owner.PlayingState)
            return;
        owner.PlayingLifebarImage.fillAmount = (float)onHealthChangeEvent.CurrentHealth / (float)onHealthChangeEvent.MaxHealth;
    }

    private void OnMatchTimerDownChange(in OnMatchTimerDownChangeEvent onMachTimerDownChangeEvent)
    {
        if (owner.CurrentState != owner.PlayingState)
            return;
        TimeSpan time = TimeSpan.FromSeconds(onMachTimerDownChangeEvent.Seconds);
        owner.PlayingMachTimer.text = string.Format("{0:00}:{1:00}", (int)time.TotalMinutes, time.Seconds);
    }

    private void OnPlayerDie(in OnPlayerDieEvent onPlayerDieEvent)
    {
        if (owner.CurrentState != owner.PlayingState)
            return;
        owner.OnGoToDead?.Invoke();
    }

    private void OnScoreChange(in OnScoreChangeEvent onScoreChangeEvent)
    {
        if (owner.CurrentState != owner.PlayingState)
            return;
        owner.PlayingScoreText.text = "Kills: " + onScoreChangeEvent.Score;
    }

    private void BeginMatch()
    {
        owner.PlayingGamplayUI.SetActive(true);
        owner.PlayingCountDown.gameObject.SetActive(false);
    }
}