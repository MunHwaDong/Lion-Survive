using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterObstacleDetector : MonoBehaviour
{
    [SerializeField] private IMonster monster;
    
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Collider thisCollider;
    
    private void OnEnable()
    {
        thisCollider.includeLayers = layerMask;
        thisCollider.excludeLayers = ~layerMask;
    }

    private void OnTriggerEnter(Collider other)
    {
        monster.Blackboard.Set(MonsterDataType.IS_FIND_PLAYER, true);
    }

    private void OnTriggerExit(Collider other)
    {
        monster.Blackboard.Set(MonsterDataType.IS_FIND_PLAYER, false);
    }
}
