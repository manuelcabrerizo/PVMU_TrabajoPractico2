using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft, IService
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();

    [Networked] public bool IsMatchBegin { get; private set; } = false;
    [Networked] public int CurrentPlayerCount { get; private set; } = 0;
    public int TargetPlayerCount { get; private set; } = 2;
    public bool IsPersistance => true;
    public bool IsSpawned {get; private set; } = false;


    [SerializeField] private Player playerPrefab;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private int countDownTime = 3;
    private List<Player> players = new List<Player>();

    private void Awake()
    {
        ServiceProvider.Instance.AddService<GameManager>(this);
    }

    public override void Spawned()
    {
        IsSpawned = true;
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
            player.Rpc_RaiseOnMatchBegin();
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
        }
    }

    private IEnumerator PreMatchCountDown()
    {
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
        Rpc_RaiseOnMatchBegin();
        StartCoroutine(DuringMatchTimer());
    }

    public IEnumerator DuringMatchTimer()
    {
        float countDownTimer = 2.0f;
        int matchDurection = (5 * 60);
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
        Rpc_RaiseOnMatchEnd();
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
}