using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _jumpForce;
    [SerializeField] int _maxLife;
    
    [Networked, OnChangedRender(nameof(CurrentLifeChanged))]
    private int CurrentLife { get; set; }

    void CurrentLifeChanged() => Debug.Log(CurrentLife);
    
    // void CurrentLifeChanged()
    // {
    //     Debug.Log(CurrentLife);
    // }
    
    [SerializeField] private LayerMask _shotLayers;
    
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnerTransform;
    
    private bool _isJumpPressed;
    private bool _isShootPressed;
    
    private float _horizontalInput;

    private NetworkRigidbody3D _rb;

    public event Action<float> OnMovement = delegate { };
    public event Action OnShot = delegate { };
    
    public override void Spawned()
    {
        _rb = GetComponent<NetworkRigidbody3D>();

        CurrentLife = _maxLife;
        
        if (HasStateAuthority)
        {
            Camera.main.GetComponent<FollowTarget>().SetTarget(this);
            
            GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.blue;
        }
        else
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
        }
        
        GameManager.Instance.AddToList(this);
    }
    
    void Update()
    {
        if (!HasStateAuthority) return;
        
        _horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.W))
        {
            _isJumpPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            _isShootPressed = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        Movement(_horizontalInput);

        if (_isJumpPressed)
        {
            Jump();
            
            _isJumpPressed = false;
        }
        
        if (_isShootPressed)
        {
            SpawnShot();
            
            _isShootPressed = false;
        }
    }

    void Movement(float xAxi)
    {
        if (xAxi != 0)
        {
            transform.forward = Vector3.forward * Mathf.Sign(xAxi);

            _rb.Rigidbody.velocity += Vector3.forward * (xAxi * _speed * 10 * Runner.DeltaTime);

            if (Mathf.Abs(_rb.Rigidbody.velocity.z) > _speed)
            {
                var velocity = Vector3.ClampMagnitude(_rb.Rigidbody.velocity, _speed);

                velocity.y = _rb.Rigidbody.velocity.y;

                _rb.Rigidbody.velocity = velocity;
            }

            OnMovement(xAxi);
        }
        else
        {
            var velocity = _rb.Rigidbody.velocity;
            velocity.z = 0;

            _rb.Rigidbody.velocity = velocity;
            
            OnMovement(0);
        }
    }

    void Jump()
    {
        _rb.Rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }

    void SpawnShot()
    {
        var bullet = Runner.Spawn(_bulletPrefab, _bulletSpawnerTransform.position, _bulletSpawnerTransform.rotation);
        
        OnShot();
    }

    void RaycastShot()
    {
        Debug.DrawRay(start: _bulletSpawnerTransform.position, dir: _bulletSpawnerTransform.forward, color: Color.red, duration: 1);

        OnShot();
        
        var raycast = Runner.GetPhysicsScene().Raycast(origin: _bulletSpawnerTransform.position, 
                                                            direction: _bulletSpawnerTransform.forward, 
                                                            hitInfo: out var hitInfo, 
                                                            maxDistance: 100f, 
                                                            layerMask: _shotLayers);

        if (!raycast) return;
        
        var enemy = hitInfo.transform.GetComponent<Player>();
            
        enemy?.RPC_TakeDamage(25);

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(int dmg)
    {
        Local_TakeDamage(dmg);
    }

    void Local_TakeDamage(int dmg)
    {
        CurrentLife -= dmg;

        if (CurrentLife <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Debug.Log("Mori :(");

        GameManager.Instance.RPC_Defeat(Runner.LocalPlayer);
        
        Runner.Despawn(Object);
    }
}