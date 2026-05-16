using System;
using UnityEngine;

class PlayingState : FsmState<UIManager>
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    public override void OnEnter()
    {
        EventBus.Subscribe<OnCountDownChangeEvent>(OnCountDownChange);
        EventBus.Subscribe<OnMachBeginEvent>(OnMachBegin);
        EventBus.Subscribe<OnMachEndEvent>(OnMachEnd);
        EventBus.Subscribe<OnMachTimerDownChangeEvent>(OnMachTimerDownChange);
        EventBus.Subscribe<OnHealthChangeEvent>(OnHealthChange);
        owner.PlayingPanel.SetActive(true);
        owner.PlayingGamplayUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnExit()
    {
        owner.PlayingPanel.SetActive(false);
        EventBus.Unsubscribe<OnHealthChangeEvent>(OnHealthChange);
        EventBus.Unsubscribe<OnMachTimerDownChangeEvent>(OnMachTimerDownChange);
        EventBus.Unsubscribe<OnMachEndEvent>(OnMachEnd);
        EventBus.Unsubscribe<OnMachBeginEvent>(OnMachBegin);
        EventBus.Unsubscribe<OnCountDownChangeEvent>(OnCountDownChange);
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnCountDownChange(in OnCountDownChangeEvent onCountDownChangeEvent)
    {
        owner.PlayingCountDown.text = onCountDownChangeEvent.CountDown.ToString();
    }

    private void OnMachBegin(in OnMachBeginEvent onMachBeginEvent)
    {
        owner.PlayingGamplayUI.SetActive(true);
        owner.PlayingCountDown.gameObject.SetActive(false);
    }

    private void OnMachEnd(in OnMachEndEvent onMachEndEvent)
    {
        owner.PlayingGamplayUI.SetActive(false);
        owner.PlayingCountDown.gameObject.SetActive(true);
    }

    private void OnHealthChange(in OnHealthChangeEvent onHealthChangeEvent)
    {
        owner.PlayingLifebarImage.fillAmount = (float)onHealthChangeEvent.CurrentHealth / (float)onHealthChangeEvent.MaxHealth;
    }

    private void OnMachTimerDownChange(in OnMachTimerDownChangeEvent onMachTimerDownChangeEvent)
    {
        TimeSpan time = TimeSpan.FromSeconds(onMachTimerDownChangeEvent.Seconds);
        owner.PlayingMachTimer.text = string.Format("{0:00}:{1:00}", (int)time.TotalMinutes, time.Seconds);
    }
}