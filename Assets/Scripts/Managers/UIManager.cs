using Fusion;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject SessionPrefab;
    public GameObject PlayerImage;

    public GameObject MainMenuPanel;
    public GameObject CreateSessionPanel;
    public GameObject JoinSessionPanel;
    public GameObject WaitForPlayersPanel;
    public GameObject PlayingPanel;

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

    public UnityEvent OnGoToMainMenu;
    public UnityEvent OnGoToCreateSession;
    public UnityEvent OnGoToJoinSession;
    public UnityEvent OnGoToWaitForPlayers;
    public UnityEvent OnGoToPlaying;

    private FsmStateMachine<UIManager> fsm = null;
    private MainMenuState mainMenuState = null;
    private CreateSessionState createSessionState = null;
    private JoinSessionState joinSessionState = null;
    private WaitForPlayersState waitPlayersState = null;
    private PlayingState playingState = null;

    private void Awake()
    {
        mainMenuState = new MainMenuState();
        createSessionState = new CreateSessionState();
        joinSessionState = new JoinSessionState();
        waitPlayersState = new WaitForPlayersState();
        playingState = new PlayingState();
        mainMenuState.Initialize(this);
        createSessionState.Initialize(this);
        joinSessionState.Initialize(this);
        waitPlayersState.Initialize(this);
        playingState.Initialize(this);
    }

    private void Start()
    {
        MainMenuPanel.SetActive(false);
        CreateSessionPanel.SetActive(false);
        JoinSessionPanel.SetActive(false);
        WaitForPlayersPanel.SetActive(false);
        fsm = new FsmStateMachine<UIManager>(
            new FsmState<UIManager>[] { mainMenuState, createSessionState, joinSessionState, waitPlayersState, playingState },
            new UnityEvent[] { OnGoToMainMenu, OnGoToCreateSession, OnGoToJoinSession, OnGoToWaitForPlayers, OnGoToPlaying },
            mainMenuState);
        fsm.ConfigureTransition(mainMenuState, createSessionState, OnGoToCreateSession);
        fsm.ConfigureTransition(mainMenuState, joinSessionState, OnGoToJoinSession);
        fsm.ConfigureTransition(createSessionState, mainMenuState, OnGoToMainMenu);
        fsm.ConfigureTransition(createSessionState, waitPlayersState, OnGoToWaitForPlayers);
        fsm.ConfigureTransition(joinSessionState, mainMenuState, OnGoToMainMenu);
        fsm.ConfigureTransition(joinSessionState, waitPlayersState, OnGoToWaitForPlayers);
        fsm.ConfigureTransition(waitPlayersState, mainMenuState, OnGoToMainMenu);
        fsm.ConfigureTransition(waitPlayersState, playingState, OnGoToPlaying);
    }

    private void Update()
    {
        fsm.Update(Time.deltaTime);
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