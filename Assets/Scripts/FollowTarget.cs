using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform _targetTransform;
    
    public void SetTarget(Player player)
    {
        _targetTransform = player.transform;
    }

    private void LateUpdate()
    {
        if (!_targetTransform) return;
        
        var newPosition = transform.position;

        newPosition.z = _targetTransform.position.z;

        transform.position = newPosition;
    }
}
