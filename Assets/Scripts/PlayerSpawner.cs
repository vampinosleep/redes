using System.Linq; // Necesario para usar .Count()
using UnityEngine;
using Fusion;

public class PlayerSpawner : NetworkBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[] _spawnTransforms;

    private bool _gameStarted = false;

    public override void Spawned()
    {
        Debug.Log("Spawner activo.");
    }

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log($"Jugador unido: {player.PlayerId}");

        // Si hay 2 jugadores y el juego aún no comenzó, lo iniciamos
        if (Runner.ActivePlayers.Count() == 2 && !_gameStarted)
        {
            _gameStarted = true;
            GameManager.Instance.RPC_StartGame();
        }

        // Si el juego ya comenzó, y este es el jugador local, hacemos spawn
        if (GameManager.Instance.GameStarted && player == Runner.LocalPlayer)
        {
            int spawnIndex = player.RawEncoded % _spawnTransforms.Length;
            Vector3 spawnPos = _spawnTransforms[spawnIndex].position;
            Quaternion spawnRot = _spawnTransforms[spawnIndex].rotation;

            Runner.Spawn(_playerPrefab, spawnPos, spawnRot, player);
            Debug.Log($"Jugador {player.PlayerId} instanciado en punto {spawnIndex}");
        }
    }
}