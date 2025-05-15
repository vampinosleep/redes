using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Obstacle : NetworkBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _delayTime;

    [Networked] private int Modifier { get; set; }

    [Networked]
    private TickTimer DirectionDelay { get; set; }

    public override void Spawned()
    {
        Modifier = 1;
        ResetTimer();
    }

    public override void FixedUpdateNetwork()
    {
        Movement();

        //Si NO termino de correr el timer
        if (!DirectionDelay.ExpiredOrNotRunning(Runner))
            return;
        
        ResetTimer();
        Modifier *= -1;
    }

    void Movement()
    {
        transform.position += Vector3.up * (_speed * Modifier * Runner.DeltaTime);
    }

    void ResetTimer()
    {
        DirectionDelay = TickTimer.CreateFromSeconds(Runner,_delayTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority) return;

        if (other.TryGetComponent(out Player player))
        {
            player.RPC_TakeDamage(25);
        }
    }
}
