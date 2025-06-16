using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossData
{
    public int phase;
    public float min_hp;
    public int attack_power;
    public int defence_power;
    public int pattern_name;
    public string animation_name;
    public float duration;
    public float cooldown;
    public float cast_time;
    public float min_attack_distance;
    public float max_attack_distance;
    public float max_health;
    public float move_speed;
}

[System.Serializable]
public class BossDataRow
{
    public int id;
    public int phase;
    public float min_hp;
    public int attack_power;
    public int defence_power;
    public int pattern_name;
    public string animation_name;
    public string duration;
    public string cooldown;
    public string cast_time;
    public string min_attack_distance;
    public string max_attack_distance;
    public string max_health;
    public string move_speed;
}

public class BossTable : BaseTable
{
    public override void Parsing(string jsonPath)
    {
        base.Parsing(jsonPath);

        bossTableRows = JsonHelper.FromJson<BossDataRow>(json);
        bossDataDict = ConvertListToDict();
    }
    
    private Dictionary<int, List<BossData>> ConvertListToDict()
    {
        Dictionary<int, List<BossData>> dict = new();

        foreach(var row in bossTableRows)
        {
            BossData bossData = new BossData();

            if (dict.ContainsKey(row.id) is false)
            {
                dict.Add(row.id, new List<BossData>());
            }
            
            bossData.phase = row.phase;
            bossData.min_hp = row.min_hp;
            bossData.attack_power = row.attack_power;
            bossData.defence_power = row.defence_power;
            bossData.pattern_name = row.pattern_name;
            bossData.animation_name = row.animation_name;
            bossData.duration = Convert.ToSingle(row.duration);
            bossData.cooldown = Convert.ToSingle(row.cooldown);
            bossData.cast_time = Convert.ToSingle(row.cast_time);
            bossData.min_attack_distance = Convert.ToSingle(row.min_attack_distance);
            bossData.max_attack_distance = Convert.ToSingle(row.max_attack_distance);
            bossData.max_health = Convert.ToSingle(row.max_health);
            bossData.move_speed = Convert.ToSingle(row.move_speed);
            
            dict[row.id].Add(bossData);
        }

        return dict;
    }

    public List<BossData> GetBossDatas(int id)
    {
        if(bossDataDict.TryGetValue(id, out List<BossData> bossData))
            return bossData;
        else
        {
            throw new Exception("테이블에 없는 id 입니다.");
        }
    }
    
    BossDataRow[] bossTableRows;
    Dictionary<int, List<BossData>> bossDataDict = new();
}