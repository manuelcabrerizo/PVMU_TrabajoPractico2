using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft, IService
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    public bool IsPersistance => true;
    public bool IsSpawned { get; private set; } = false;
    [Networked] public bool IsMatchBegin { get; private set; } = false;
    [Networked] public int CurrentPlayerCount { get; private set; } = 0;
    [Networked] public bool IsInCountDown { get; private set; } = false;
    [Networked, Capacity(10)]
    public NetworkLinkedList<Player> ScoreBoard => default;

    public Player LocalPlayer { get; set; } = null;

    [SerializeField] private Player playerPrefab;

    public int TargetPlayerCount = 2;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private int countDownTime = 3;
    
    private List<Player> players = new List<Player>();

    private void Awake()
    {
        if (ServiceProvider.Instance.ContainsService<GameManager>())
        {
            ServiceProvider.Instance.RemoveService<GameManager>();
        }
        ServiceProvider.Instance.AddService<GameManager>(this);
    }

    public override void Spawned()
    {
        IsSpawned = true;
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        LocalPlayer = null;
        IsSpawned = false;
    }

    public void PlayerJoined(PlayerRef playerRef)
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        Vector3 spawnPosition = spawnPositions[players.Count % spawnPositions.Length].position;
        spawnPosition.y += 1.0f;
        Player player = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, playerRef);
        Runner.SetPlayerObject(playerRef, player.Object);
        players.Add(player);

        CurrentPlayerCount++;
        Rpc_RaiseOnPlayerJoinEvent(playerRef, CurrentPlayerCount, TargetPlayerCount);
        if (!IsMatchBegin && CurrentPlayerCount == TargetPlayerCount)
        {
            IsMatchBegin = true;
            StartCoroutine(PreMatchCountDown());
        }
        else if (IsMatchBegin)
        {
            player.CanMove = true;
        }
    }

    public void PlayerLeft(PlayerRef playerRef)
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        int index = players.FindIndex(t => t.Object.InputAuthority == playerRef);
        if (index >= 0)
        {
            Runner.Despawn(players[index].Object);
            players.RemoveAt(index);
            CurrentPlayerCount--;
            Rpc_RaiseOnPlayerJoinEvent(playerRef, CurrentPlayerCount, TargetPlayerCount);
        }
    }

    private void OnMatchEnd()
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        List<Player> sortedPlayers = players.OrderByDescending((p) => p.Score).ToList();
        ScoreBoard.Clear();
        foreach (Player player in sortedPlayers)
        {
            ScoreBoard.Add(player);
        }
        Rpc_RaiseOnMatchEnd();
    }

    public void RevivePlayer(Player player)
    {
        if (player.Object.HasInputAuthority == false)
        {
            return;
        }
        player.Rpc_RevivePlayer();
    }

    public Vector3 GetRandomSpawnPosition()
    {
        return spawnPositions[Random.Range(0, spawnPositions.Length)].position;
    }

    private IEnumerator PreMatchCountDown()
    {
        IsInCountDown = true;
        float countDownTimer = 2.0f;
        int countDownCounter = 0;
        while (true)
        {
            if (countDownTimer < 0.0f)
            {
                countDownCounter++;
                Rpc_RaiseOnCountDownChangeEvent(countDownTime - countDownCounter);
                countDownTimer = 1.0f;
            }
            countDownTimer -= Time.deltaTime;
            if (countDownCounter == countDownTime)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        foreach (Player p in players)
        {
            p.CanMove = true;
        }
        IsInCountDown = false;
        Rpc_RaiseOnMatchBegin();
        StartCoroutine(DuringMatchTimer());
    }

    public IEnumerator DuringMatchTimer()
    {
        float countDownTimer = 2.0f;
        int matchDurection = 20;// (5 * 60);
        int countDownCounter = 0;
        Rpc_RaiseOnMatchTimerChangeEvent(matchDurection);
        while (true)
        {
            if (countDownTimer < 0.0f)
            {
                countDownCounter++;
                Rpc_RaiseOnMatchTimerChangeEvent(matchDurection - countDownCounter);
                countDownTimer = 1.0f;
            }
            countDownTimer -= Time.deltaTime;
            if (countDownCounter == matchDurection)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        OnMatchEnd();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RaiseOnPlayerJoinEvent(PlayerRef playerRef, int playerCount, int targetPlayerCount)
    {
        EventBus.Raise<OnPlayerJoinEvent>(playerRef, playerCount, targetPlayerCount);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RaiseOnCountDownChangeEvent(int countDown)
    {
        EventBus.Raise<OnCountDownChangeEvent>(countDown);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RaiseOnMatchTimerChangeEvent(int seconds)
    {
        EventBus.Raise<OnMatchTimerDownChangeEvent>(seconds);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RaiseOnMatchBegin()
    {
        EventBus.Raise<OnMatchBeginEvent>();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RaiseOnMatchEnd()
    {
        EventBus.Raise<OnMatchEndEvent>();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void Rpc_RaiseOnHostDisconect()
    {
        EventBus.Raise<OnHostDisconectEvent>();
    }
}