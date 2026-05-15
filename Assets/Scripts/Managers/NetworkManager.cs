using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks, IService
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
    public bool IsPersistance => true;

    private NetworkRunner networkRunner = null;
    private NetworkInputData networkInputData;

    private void Awake()
    {
        ServiceProvider.Instance.AddService<NetworkManager>(this);
        ServiceProvider.Instance.AddService<EventBus>(new EventBus());
        networkInputData = new NetworkInputData(Vector2.zero, (char)0);
    }

    public async void JoinLobby(Action onSuccess, Action<StartGameResult> onFailure)
    {
        networkRunner = gameObject.AddComponent<NetworkRunner>();
        gameObject.AddComponent<HitboxManager>();
        networkRunner.ProvideInput = true;
        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, "TrabajoPractico2");
        if (result.Ok)
        {
            onSuccess?.Invoke();
        }
        else
        {
            onFailure?.Invoke(result);
        }
    }

    public async void StartHost(string session, Action onSuccess, Action<StartGameResult> onFailure)
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
            onSuccess?.Invoke();
        }
        else
        {
            onFailure?.Invoke(result);
        }
    }

    public async void StartClient(string session, Action onSuccess, Action<StartGameResult> onFailure)
    {
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            SessionName = session,
        });
        if (result.Ok)
        {
            onSuccess?.Invoke();
        }
        else
        {
            onFailure?.Invoke(result);
        }
    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        EventBus.Raise<OnSessionListUpdatedEvent>(sessionList);
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
