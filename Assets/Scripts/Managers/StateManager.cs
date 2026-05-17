using Fusion;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    public GameObject SessionPrefab;
    public GameObject PlayerImage;

    public GameObject MainMenuPanel;
    public GameObject CreateSessionPanel;
    public GameObject JoinSessionPanel;
    public GameObject WaitForPlayersPanel;
    public GameObject PlayingPanel;
    public GameObject DeadPanel;

    // Main Menu buttons
    public Button MainMenuCreateButton;
    public Button MainMenuJoinButton;
    public Button MainMenuExitButton;
    // Create Session buttons
    public TMP_InputField CreateSessionInputField;
    public Button CreateSessionCreateButton;
    public Button CreateSessionBackButton;
    // Join Session buttons
    public Transform JoinSessionContent;
    public Button JoinSessionBackButton;
    // Wait for Players buttons
    public Transform WaitForPlayerContent;
    public Button WaitForPlayersBackButton;
    // Playing UI
    public GameObject PlayingGamplayUI;
    public TMP_Text PlayingCountDown;
    public Image PlayingLifebarImage;
    public TMP_Text PlayingMachTimer;
    // Dead UI
    public Button DeadRespawnButton;
    public Button DeadBackButton;

    public UnityEvent OnGoToMainMenu;
    public UnityEvent OnGoToCreateSession;
    public UnityEvent OnGoToJoinSession;
    public UnityEvent OnGoToWaitForPlayers;
    public UnityEvent OnGoToPlaying;
    public UnityEvent OnGoToDead;

    public FsmState<StateManager> CurrentState => fsm.CurrentState;

    private FsmStateMachine<StateManager> fsm = null;
    public MainMenuState MainMenuState = null;
    public CreateSessionState CreateSessionState = null;
    public JoinSessionState JoinSessionState = null;
    public WaitForPlayersState WaitPlayersState = null;
    public PlayingState PlayingState = null;
    public DeadState DeadState = null;

    private void Start()
    {
        MainMenuState = new MainMenuState(this);
        CreateSessionState = new CreateSessionState(this);
        JoinSessionState = new JoinSessionState(this);
        WaitPlayersState = new WaitForPlayersState(this);
        PlayingState = new PlayingState(this);
        DeadState = new DeadState(this);

        MainMenuPanel.SetActive(false);
        CreateSessionPanel.SetActive(false);
        JoinSessionPanel.SetActive(false);
        WaitForPlayersPanel.SetActive(false);

        fsm = new FsmStateMachine<StateManager>(
            new FsmState<StateManager>[] { MainMenuState, CreateSessionState, JoinSessionState, WaitPlayersState, PlayingState, DeadState },
            new UnityEvent[] { OnGoToMainMenu, OnGoToCreateSession, OnGoToJoinSession, OnGoToWaitForPlayers, OnGoToPlaying, OnGoToDead },
            MainMenuState);
        
        fsm.ConfigureTransition(MainMenuState, CreateSessionState, OnGoToCreateSession);
        fsm.ConfigureTransition(MainMenuState, JoinSessionState, OnGoToJoinSession);
        fsm.ConfigureTransition(CreateSessionState, MainMenuState, OnGoToMainMenu);
        fsm.ConfigureTransition(CreateSessionState, WaitPlayersState, OnGoToWaitForPlayers);
        fsm.ConfigureTransition(JoinSessionState, MainMenuState, OnGoToMainMenu);
        fsm.ConfigureTransition(JoinSessionState, WaitPlayersState, OnGoToWaitForPlayers);
        fsm.ConfigureTransition(WaitPlayersState, MainMenuState, OnGoToMainMenu);
        fsm.ConfigureTransition(WaitPlayersState, PlayingState, OnGoToPlaying);
        fsm.ConfigureTransition(PlayingState, DeadState, OnGoToDead);
        fsm.ConfigureTransition(DeadState, PlayingState, OnGoToPlaying);
    }

    private void OnDestroy()
    {
        MainMenuState.Dispose();
        CreateSessionState.Dispose();
        JoinSessionState.Dispose();
        WaitPlayersState.Dispose();
        PlayingState.Dispose();
        DeadState.Dispose();
    }

    public void ClearSessionsButtons()
    {
        for (int i = JoinSessionContent.childCount - 1; i >= 0; i--)
        {
            Destroy(JoinSessionContent.GetChild(i).gameObject);
        }
    }

    public void CreateSessionButtons(List<SessionInfo> sessionInfoList, Action<string> onClick)
    {
        foreach (SessionInfo sessionInfo in sessionInfoList)
        {
            string sessionName = sessionInfo.Name;
            GameObject go = Instantiate(SessionPrefab, JoinSessionContent);
            Button button = go.GetComponent<Button>();
            button.onClick.AddListener(() => onClick(sessionName));
            TMP_Text text = go.GetComponentInChildren<TMP_Text>();
            text.text = sessionName;
        }
    }

    public void ClearPlayerImages()
    {
        for (int i = WaitForPlayerContent.childCount - 1; i >= 0; i--)
        {
            Destroy(WaitForPlayerContent.GetChild(i).gameObject);
        }
    }

    public void CreatePlayerImages(int count)
    {
        for (int i = 0; i < count; i++)
        { 
            GameObject go = Instantiate(PlayerImage, WaitForPlayerContent);
        }
    }
}