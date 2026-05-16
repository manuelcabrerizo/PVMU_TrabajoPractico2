using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    private EventBus EventBus => ServiceProvider.Instance.GetService<EventBus>();
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private int countDownTime = 3;

    private List<Player> players = new List<Player>();
    private bool isMachBegin = false;

    public void PlayerJoined(PlayerRef playerRef)
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        Vector3 spawnPosition = spawnPositions[players.Count].position;
        spawnPosition.y += 1.0f;
        Player player = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, playerRef);
        Runner.SetPlayerObject(playerRef, player.Object);
        players.Add(player);
        Rpc_RaiseOnPlayerJoinEvent(playerRef, players.Count, spawnPositions.Length);
        if (!isMachBegin && players.Count > 0 && players.Count == spawnPositions.Length)
        {
            StartCoroutine(PreMachCountDown());
            isMachBegin = true;
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
        }
    }

    private IEnumerator PreMachCountDown()
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
        Rpc_RaiseOnMachBegin();
        StartCoroutine(DuringMatchTimer());
    }

    public IEnumerator DuringMatchTimer()
    {
        float countDownTimer = 2.0f;
        int machDurection = (5 * 60);
        int countDownCounter = 0;
        Rpc_RaiseOnMachTimerChangeEvent(machDurection);
        while (true)
        {
            if (countDownTimer < 0.0f)
            {
                countDownCounter++;
                Rpc_RaiseOnMachTimerChangeEvent(machDurection - countDownCounter);
                countDownTimer = 1.0f;
            }
            countDownTimer -= Time.deltaTime;
            if (countDownCounter == machDurection)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        Rpc_RaiseOnMachEnd();
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
    private void Rpc_RaiseOnMachTimerChangeEvent(int seconds)
    {
        EventBus.Raise<OnMachTimerDownChangeEvent>(seconds);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RaiseOnMachBegin()
    {
        EventBus.Raise<OnMachBeginEvent>();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    private void Rpc_RaiseOnMachEnd()
    {
        EventBus.Raise<OnMachEndEvent>();
    }
}