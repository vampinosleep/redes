using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Turret : NetworkBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _shotSpawnTransform;
    
    [SerializeField] private float _timeCooldown;
    //[Networked] private float TimePulse { get; set; }
    
    [Networked] private TickTimer _cooldownTimer { get; set; }

    public override void Spawned()
    {
        ResetTimer();
    }

    public override void FixedUpdateNetwork()
    {
        if (!_cooldownTimer.ExpiredOrNotRunning(Runner)) return;

        TurretLogic();
        ResetTimer();

        // if (TimePulse < _timeCooldown)
        // {
        //     TimePulse += Runner.DeltaTime;
        // }
        // else
        // {
        //     TurretLogic();
        //     TimePulse = 0;
        // }
    }

    void TurretLogic()
    {
        Runner.Spawn(_bulletPrefab, _shotSpawnTransform.position, _shotSpawnTransform.rotation);
    }

    void ResetTimer()
    {
        _cooldownTimer = TickTimer.CreateFromSeconds(Runner, _timeCooldown);
    }
}
