using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackRange : MonoBehaviour
{
    [SerializeField] private IMonster monster;

    void Awake()
    {
        monster ??= GetComponentInParent<IMonster>();
    }

    private void OnTriggerEnter(Collider other)
    {
        monster.Blackboard.Set(MonsterDataType.IS_ENCOUNTER, true);
    }

    private void OnTriggerExit(Collider other)
    {
        monster.Blackboard.Set(MonsterDataType.IS_ENCOUNTER, false);
    }
}
