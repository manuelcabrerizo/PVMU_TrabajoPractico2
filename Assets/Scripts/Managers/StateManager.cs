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
    public GameObject PlayerScorePrefab;
    public GameObject PlayerImage;

    public GameObject MainMenuPanel;
    public GameObject CreateSessionPanel;
    public GameObject JoinSessionPanel;
    public GameObject WaitForPlayersPanel;
    public GameObject PlayingPanel;
    public GameObject DeadPanel;
    public GameObject MatchEndPanel;

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
    public TMP_Text PlayingScoreText;
    // Dead UI
    public Button DeadRespawnButton;
    public Button DeadBackButton;
    // Match End UI
    public Button MatchEndBackButton;
    public Transform MatchEndContent;
    public TMP_Text MatchEndTitleText;

    public UnityEvent OnGoToMainMenu;
    public UnityEvent OnGoToCreateSession;
    public UnityEvent OnGoToJoinSession;
    public UnityEvent OnGoToWaitForPlayers;
    public UnityEvent OnGoToPlaying;
    public UnityEvent OnGoToDead;
    public UnityEvent OnGoToMatchEnd;

    public FsmState<StateManager> CurrentState => fsm.CurrentState;

    private FsmStateMachine<StateManager> fsm = null;
    public MainMenuState MainMenuState = null;
    public CreateSessionState CreateSessionState = null;
    public JoinSessionState JoinSessionState = null;
    public WaitForPlayersState WaitPlayersState = null;
    public PlayingState PlayingState = null;
    public DeadState DeadState = null;
    public MatchEndState MatchEndState = null;

    private void Start()
    {
        MainMenuState = new MainMenuState(this);
        CreateSessionState = new CreateSessionState(this);
        JoinSessionState = new JoinSessionState(this);
        WaitPlayersState = new WaitForPlayersState(this);
        PlayingState = new PlayingState(this);
        DeadState = new DeadState(this);
        MatchEndState = new MatchEndState(this);

        MainMenuPanel.SetActive(false);
        CreateSessionPanel.SetActive(false);
        JoinSessionPanel.SetActive(false);
        WaitForPlayersPanel.SetActive(false);
        PlayingPanel.SetActive(false);
        DeadPanel.SetActive(false);
        MatchEndPanel.SetActive(false);

        fsm = new FsmStateMachine<StateManager>(
            new FsmState<StateManager>[] { MainMenuState, CreateSessionState, JoinSessionState, WaitPlayersState, PlayingState, DeadState, MatchEndState },
            new UnityEvent[] { OnGoToMainMenu, OnGoToCreateSession, OnGoToJoinSession, OnGoToWaitForPlayers, OnGoToPlaying, OnGoToDead, OnGoToMatchEnd },
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
        fsm.ConfigureTransition(PlayingState, MatchEndState, OnGoToMatchEnd);
        fsm.ConfigureTransition(DeadState, MatchEndState, OnGoToMatchEnd);
    }

    private void OnDestroy()
    {
        MainMenuState.Dispose();
        CreateSessionState.Dispose();
        JoinSessionState.Dispose();
        WaitPlayersState.Dispose();
        PlayingState.Dispose();
        DeadState.Dispose();
        MatchEndState.Dispose();
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

    public void ShowPlayerScoreTexts(NetworkLinkedList<Player> scoreBoard)
    {
        for (int i = MatchEndContent.childCount - 1; i >= 0; i--)
        {
            Destroy(MatchEndContent.GetChild(i).gameObject);
        }
        foreach (Player player in scoreBoard)
        {
            GameObject go = Instantiate(PlayerScorePrefab, MatchEndContent);
            TMP_Text text = go.GetComponentInChildren<TMP_Text>();
            text.text = player.Object.InputAuthority.ToString() + ": " + player.Score;
        }
    }
}