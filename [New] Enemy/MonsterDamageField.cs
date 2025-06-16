using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDamageField : MonoBehaviour
{
    [SerializeField] private IMonster monster;

    void Awake()
    {
        monster ??= GetComponentInParent<IMonster>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        enabled = false;
        
        DamageInfo damageInfo = new DamageInfo()
        {
            damage = monster.Blackboard.Get<int>(MonsterDataType.ATTACK),
            atkPos = monster.transform.position,
            knockbackPower = 0
        };
        
        other.GetComponent<IDamageable>().OnDamaged(damageInfo);
    }
}
