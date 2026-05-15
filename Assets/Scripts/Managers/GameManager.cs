using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Transform[] spawnPositions;
    [SerializeField] private int countDownTime = 3;

    private List<Player> players = new List<Player>();

    [Networked] public PlayerRef Winner { get; private set; } = PlayerRef.None;
    [Networked] public int CountDownCounter { get; private set; } = 0;
    [Networked] public bool IsInCountDown { get; private set; } = false;

    private bool wasSpawn = false;

    private void Awake()
    {
        ServiceProvider.Instance.AddService<EventBus>(new EventBus());
    }

    private void OnGUI()
    {
        if (wasSpawn && IsInCountDown)
        {
            GUI.Label(new Rect(0, 0, 200, 40), "CountDown: " + (countDownTime - CountDownCounter));
        }
        if (wasSpawn && (Winner != PlayerRef.None))
        {
            GUI.Label(new Rect(0, 0, 200, 40), "The Winner is: " + Winner);
        }
    }

    IEnumerator StartCountDown()
    {
        IsInCountDown = true;
        float countDownTimer = 2.0f;
        CountDownCounter = 0;
        while (IsInCountDown)
        {
            if (countDownTimer < 0.0f)
            {
                CountDownCounter++;
                countDownTimer = 1.0f;
            }
            countDownTimer -= Time.deltaTime;
            if (CountDownCounter == countDownTime)
            {
                IsInCountDown = false;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        foreach (Player p in players)
        {
            p.CanMove = true;
        }
    }

    public override void Spawned()
    {
        wasSpawn = true;
    }

    public void PlayerJoined(PlayerRef playerRef)
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        Vector3 spawnPosition = spawnPositions[players.Count].position;
        spawnPosition.y = 1.0f;
        Player player = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, playerRef);
        Runner.SetPlayerObject(playerRef, player.Object);
        players.Add(player);
        //if (players.Count == spawnPositions.Length)
        {
            StartCoroutine(StartCountDown());
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
}

