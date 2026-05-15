using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MenuState
{
    MainMenu,
    CreateSession,
    SelectSession,
    Playing
}

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner networkRunner = null;
    private List<SessionInfo> sessionInfoList = null;
    private MenuState menuState;
    private string sessionName = "";
    private NetworkInputData networkInputData;

    private void Awake()
    {
        networkInputData = new NetworkInputData(Vector2.zero, (char)0);
        menuState = MenuState.MainMenu;
    }

    private void OnGUI()
    {
        switch (menuState)
        {
            case MenuState.MainMenu: OnMainMenu(); break;
            case MenuState.CreateSession: OnCreateSession(); break;
            case MenuState.SelectSession: OnSelectSession(); break;
        }
    }

    private void OnMainMenu()
    {
        if (GUI.Button(new Rect(0, 0, 200, 40), "Create Session"))
        {
            menuState = MenuState.CreateSession;
        }
        if (GUI.Button(new Rect(0, 40, 200, 40), "Join Session"))
        {
            JoinLobby();
            menuState = MenuState.SelectSession;
        }
    }

    private void OnCreateSession()
    {
        GUI.Label(new Rect(0, 0, 200, 40), "Create Session!");
        sessionName = GUI.TextField(new Rect(0, 40, 200, 40), sessionName);
        if (GUI.Button(new Rect(0, 80, 200, 40), "Create"))
        {
            StartHost(sessionName);
        }

    }

    private void OnSelectSession()
    {
        GUI.Label(new Rect(0, 0, 200, 40), "Select Session!");
        if (sessionInfoList != null && sessionInfoList.Count > 0)
        {
            for(int i = 0; i < sessionInfoList.Count; i++)
            {
                SessionInfo info = sessionInfoList[i];
                if (GUI.Button(new Rect(0, 40 * (i + 1), 200, 40), info.Name))
                {
                    StartClient(info.Name);
                }
            }
        }
        else
        {
            GUI.Label(new Rect(0, 40, 200, 40), "No session found!");
        }
    }

    public async void JoinLobby()
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        gameObject.AddComponent<HitboxManager>();
        networkRunner.ProvideInput = true;
        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, "TrabajoPractico2");
        if (result.Ok)
        {
            // All good
        }
        else
        {
            Debug.Log($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public async void StartHost(string session)
    { 
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        gameObject.AddComponent<HitboxManager>();
        networkRunner.ProvideInput = true;
        SceneRef scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        var result = await networkRunner.StartGame(new StartGameArgs() 
        {
            GameMode = GameMode.Host,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            CustomLobbyName = "TrabajoPractico2",
            SessionName = session,
            PlayerCount = 10
        });
        if (result.Ok)
        {
            menuState = MenuState.Playing;
        }
        else
        {
            Debug.Log($"Failed to Start: {result.ShutdownReason}");
        }
    }

    public async void StartClient(string session)
    {
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            SessionName = session,
        });
        if (result.Ok)
        {
            menuState = MenuState.Playing;
        }
        else
        {
            Debug.Log($"Failed to Start: {result.ShutdownReason}");
        }
    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        sessionInfoList = sessionList;
    }

    // TODO: move this a PlayerInput class and Use IBeforeUpdate y IAfterTick
    private void Update()
    {
        networkInputData.ClearActions();
        if (Input.GetKey(KeyCode.W))
            networkInputData.AddAction(InputAction.MoveForward);
        if (Input.GetKey(KeyCode.S))
            networkInputData.AddAction(InputAction.MoveBackward);
        if (Input.GetKey(KeyCode.A))
            networkInputData.AddAction(InputAction.MoveLeft);
        if (Input.GetKey(KeyCode.D))
            networkInputData.AddAction(InputAction.MoveRight);
        if (Input.GetMouseButton(0))
            networkInputData.AddAction(InputAction.Shoot);
        if (Input.GetKey(KeyCode.Space))
            networkInputData.AddAction(InputAction.Jump);

        Vector2 lookRotation = networkInputData.GetLookRotation();
        lookRotation += new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y")) * 4.0f;
        float twoPI = 360.0f;
        if (lookRotation.x > twoPI)
        {
            lookRotation.x -= twoPI;
        }
        else if (lookRotation.x < 0)
        {
            lookRotation.x += twoPI;
        }
        float limit = 89.0f;
        if (lookRotation.y > limit)
        {
            lookRotation.y = limit;
        }
        else if (lookRotation.y < -limit)
        {
            lookRotation.y = -limit;
        }
        networkInputData.SetLookRotation(lookRotation);
    }

    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(networkInputData);
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
}
