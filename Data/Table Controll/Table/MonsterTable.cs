using System;
using System.Collections;
using System.Collections.Generic;
using JH;
using UnityEngine;
//MonsterData 클래스 내용이 EnemyStatus 내용과 똑같아서 일괄 수정하고 코드 적용시킴
namespace JH
{
    public enum AttackType
    {
        Melee,
        Ranged
    };
}


[System.Serializable]
public class MonsterDataRow
{
    public int id;
    public string hp;
    public string speed;
    public string attack;
    public string exp;
    public string point;
    public string range;
    public int attack_type;
    public string specialAttackCooldown;
    public string hasSpecialAttack;
}

public class MonsterTable : BaseTable
{
    public override void Parsing(string jsonPath)
    {
        base.Parsing(jsonPath);

        monsterTableRows = JsonHelper.FromJson<MonsterDataRow>(json);
        monsterDataDict = ConvertListToDict();
    }
    
    private Dictionary<int, EnemyStatus> ConvertListToDict()
    {
        Dictionary<int, EnemyStatus> dict = new Dictionary<int, EnemyStatus>();
        
        foreach(var row in monsterTableRows)
        {
            int key = row.id;

            EnemyStatus monsterData = new EnemyStatus();
            
            monsterData.maxHealth = Convert.ToInt32(row.hp);
            monsterData.moveSpeed = Convert.ToSingle(row.speed);
            monsterData.attackPower = Convert.ToInt32(row.attack);
            monsterData.experience = Convert.ToSingle(row.exp);
            monsterData.point = Convert.ToSingle(row.point);
            monsterData.attackRange = Convert.ToSingle(row.range);
            monsterData.attackType = (EnemyAttackType)row.attack_type;
            monsterData.specialAttackCooldown = Convert.ToSingle(row.specialAttackCooldown);
            monsterData.hasSpecialAttack = Convert.ToBoolean(row.hasSpecialAttack);
            dict.TryAdd(key, monsterData);
        }

        return dict;
    }

    public EnemyStatus GetMonsterData(int monsterNo)
    {
        if(monsterDataDict.TryGetValue(monsterNo, out var data))
            return data;
        else
        {
            throw new Exception("해당 id가 Monster Table에 존재하지 않습니다.");
        }
    }
    
    MonsterDataRow[] monsterTableRows;
    Dictionary<int, EnemyStatus> monsterDataDict = new Dictionary<int, EnemyStatus>();
}
