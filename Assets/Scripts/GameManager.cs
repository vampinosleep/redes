using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject _winImage;
    [SerializeField] private GameObject _loseImage;

    [SerializeField] private GameObject _waitingPanel; // Panel de espera opcional

    private List<PlayerRef> _players;

    [Networked] public bool GameStarted { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        _players = new List<PlayerRef>();
    }

    private void Update()
    {
        // Mostrar panel "esperando jugadores"
        if (_waitingPanel != null)
        {
            _waitingPanel.SetActive(!GameStarted);
        }
    }

    public void AddToList(Player player)
    {
        var playerRef = player.Object.StateAuthority;

        if (_players.Contains(playerRef)) return;

        _players.Add(playerRef);
    }

    void RemoveFromList(PlayerRef player)
    {
        _players.Remove(player);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartGame()
    {
        GameStarted = true;
        Debug.Log("¡El juego ha comenzado!");
    }

    [Rpc]
    public void RPC_Defeat(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Defeat();
        }

        RemoveFromList(player);

        if (_players.Count == 1 && HasStateAuthority)
        {
            RPC_Win(_players[0]);
        }
    }

    [Rpc]
    void RPC_Win([RpcTarget] PlayerRef player)
    {
        Win();
    }

    void Win()
    {
        if (_winImage != null)
            _winImage.SetActive(true);
    }

    void Defeat()
    {
        if (_loseImage != null)
            _loseImage.SetActive(true);
    }
}