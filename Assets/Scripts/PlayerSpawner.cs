using UnityEngine;
using Fusion;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private Transform[] _spawnTransforms;

    private bool _initialized;
    
    //Se ejecuta por CADA cliente conectado
    public void PlayerJoined(PlayerRef player)
    {
        var playersCount = Runner.SessionInfo.PlayerCount;
        
        if (_initialized && playersCount >= 2)
        {
            CreatePlayer(0);
            return;
        }
        
        //Si el cliente que entro, es el mismo cliente donde corre este codigo, entonces:
        if (player == Runner.LocalPlayer)
        {
            if (playersCount < 2)
            {
                _initialized = true;
            }
            else
            {
                CreatePlayer(playersCount - 1);
            }
            
            //if (playersCount - 1 >= _spawnTransforms.Length) return;
        }
    }

    void CreatePlayer(int spawnPointIndex)
    {
        _initialized = false;
        
        var newPosition = _spawnTransforms[spawnPointIndex].position;
        var newRotation = _spawnTransforms[spawnPointIndex].rotation;
        
        
        Runner.Spawn(_playerPrefab, newPosition, newRotation);
    }
}
