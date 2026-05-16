using System;
using UnityEngine;

class PlayingState : FsmState<UIManager>
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    public PlayingState(UIManager owner) : base(owner)
    {
        EventBus.Subscribe<OnCountDownChangeEvent>(OnCountDownChange);
        EventBus.Subscribe<OnMatchBeginEvent>(OnMatchBegin);
        EventBus.Subscribe<OnMatchEndEvent>(OnMatchEnd);
        EventBus.Subscribe<OnMatchTimerDownChangeEvent>(OnMatchTimerDownChange);
        EventBus.Subscribe<OnHealthChangeEvent>(OnHealthChange);
    }
    public override void Dispose()
    {
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
    }

    public override void OnExit()
    {
        Cursor.lockState = CursorLockMode.None;
        owner.PlayingPanel.SetActive(false);
    }

    private void OnCountDownChange(in OnCountDownChangeEvent onCountDownChangeEvent)
    {
        owner.PlayingCountDown.text = onCountDownChangeEvent.CountDown.ToString();
    }

    private void OnMatchBegin(in OnMatchBeginEvent onMachBeginEvent)
    {
        owner.PlayingGamplayUI.SetActive(true);
        owner.PlayingCountDown.gameObject.SetActive(false);
    }

    private void OnMatchEnd(in OnMatchEndEvent onMachEndEvent)
    {
        owner.PlayingGamplayUI.SetActive(false);
        owner.PlayingCountDown.gameObject.SetActive(true);
    }

    private void OnHealthChange(in OnHealthChangeEvent onHealthChangeEvent)
    {
        owner.PlayingLifebarImage.fillAmount = (float)onHealthChangeEvent.CurrentHealth / (float)onHealthChangeEvent.MaxHealth;
    }

    private void OnMatchTimerDownChange(in OnMatchTimerDownChangeEvent onMachTimerDownChangeEvent)
    {
        TimeSpan time = TimeSpan.FromSeconds(onMachTimerDownChangeEvent.Seconds);
        owner.PlayingMachTimer.text = string.Format("{0:00}:{1:00}", (int)time.TotalMinutes, time.Seconds);
    }
}