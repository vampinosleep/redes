using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[RequireComponent(typeof(Player))]
public class PlayerView : NetworkBehaviour
{
    [SerializeField] private ParticleSystem _shotParticle;

    private NetworkMecanimAnimator _mecanim;
    
    public override void Spawned()
    {
        _mecanim = GetComponentInChildren<NetworkMecanimAnimator>();
        
        var m = GetComponent<Player>();

        m.OnShot += RPC_TriggerShotParticles;
        m.OnMovement += MoveAnimation;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_TriggerShotParticles()
    {
        _shotParticle.Play();
    }

    void MoveAnimation(float xAxi)
    {
        //_mecanim.SetTrigger(...);
        _mecanim.Animator.SetFloat("axi", Mathf.Abs(xAxi));
    }
}
