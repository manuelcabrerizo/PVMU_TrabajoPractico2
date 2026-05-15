using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject CreateSessionPanel;
    public GameObject JoinSessionPanel;
    public GameObject WaitForPlayersPanel;

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
    public Button WaitForPlayersBackButton;

    public UnityEvent OnGoToMainMenu;
    public UnityEvent OnGoToCreateSession;
    public UnityEvent OnGoToJoinSession;
    public UnityEvent OnGoToWaitForPlayers;

    private FsmStateMachine<UIMenu> fsm = null;
    private MainMenuState mainMenuState = null;
    private CreateSessionState createSessionState = null;
    private JoinSessionState joinSessionState = null;
    private WaitForPlayersState waitPlayersState = null;

    private void Awake()
    {
        mainMenuState = new MainMenuState();
        createSessionState = new CreateSessionState();
        joinSessionState = new JoinSessionState();
        waitPlayersState = new WaitForPlayersState();
        mainMenuState.Initialize(this);
        createSessionState.Initialize(this);
        joinSessionState.Initialize(this);
        waitPlayersState.Initialize(this);
    }

    private void Start()
    {
        MainMenuPanel.SetActive(false);
        CreateSessionPanel.SetActive(false);
        JoinSessionPanel.SetActive(false);
        WaitForPlayersPanel.SetActive(false);

        fsm = new FsmStateMachine<UIMenu>(
            new FsmState<UIMenu>[] { mainMenuState, createSessionState, joinSessionState, waitPlayersState },
            new UnityEvent[] { OnGoToMainMenu, OnGoToCreateSession, OnGoToJoinSession, OnGoToWaitForPlayers },
            mainMenuState);

        fsm.ConfigureTransition(mainMenuState, createSessionState, OnGoToCreateSession);
        fsm.ConfigureTransition(mainMenuState, joinSessionState, OnGoToJoinSession);

        fsm.ConfigureTransition(createSessionState, mainMenuState, OnGoToMainMenu);
        fsm.ConfigureTransition(createSessionState, waitPlayersState, OnGoToWaitForPlayers);

        fsm.ConfigureTransition(joinSessionState, mainMenuState, OnGoToMainMenu);
        fsm.ConfigureTransition(joinSessionState, waitPlayersState, OnGoToWaitForPlayers);

        fsm.ConfigureTransition(waitPlayersState, mainMenuState, OnGoToMainMenu);
    }
}

