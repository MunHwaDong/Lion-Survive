using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonVolatileStatData
{
    public int id;
    public List<float> attriValues = new();
    public int maxUpGradeCount;
    public int cost;
    public string name;
    public string description;
    public Sprite icon;
}

[System.Serializable]
public class NonVolatileStatDataRow
{
    public int id;
    public string level_1;
    public string level_2;
    public string level_3;
    public string level_4;
    public string level_5;
    public string level_6;
    public string level_7;
    public string level_8;
    public string level_9;
    public string level_10;
    public int max_upgrade_count;
    public int cost;
    public string name;
    public string description;
    public string path;
}

public class NonVolatileStatTable : BaseTable
{
    public override void Parsing(string jsonPath)
    {
        base.Parsing(jsonPath);

        nonVolatileStatRows = JsonHelper.FromJson<NonVolatileStatDataRow>(json);
        stageDataDict = ConvertListToDict();
    }
    
    private Dictionary<int, NonVolatileStatData> ConvertListToDict()
    {
        Dictionary<int, NonVolatileStatData> dict = new Dictionary<int, NonVolatileStatData>();
        
        foreach(var row in nonVolatileStatRows)
        {
            NonVolatileStatData nonVolatileStatData = new NonVolatileStatData();
            
            int id = row.id;
            
            nonVolatileStatData.attriValues.Add(Convert.ToSingle(row.level_1));
            nonVolatileStatData.attriValues.Add(Convert.ToSingle(row.level_2));
            nonVolatileStatData.attriValues.Add(Convert.ToSingle(row.level_3));
            nonVolatileStatData.attriValues.Add(Convert.ToSingle(row.level_4));
            nonVolatileStatData.attriValues.Add(Convert.ToSingle(row.level_5));
            nonVolatileStatData.attriValues.Add(Convert.ToSingle(row.level_6));
            nonVolatileStatData.attriValues.Add(Convert.ToSingle(row.level_7));
            nonVolatileStatData.attriValues.Add(Convert.ToSingle(row.level_8));
            nonVolatileStatData.attriValues.Add(Convert.ToSingle(row.level_9));
            nonVolatileStatData.attriValues.Add(Convert.ToSingle(row.level_10));
            
            nonVolatileStatData.maxUpGradeCount = row.max_upgrade_count;
            nonVolatileStatData.cost = row.cost;
            nonVolatileStatData.name = row.name;
            nonVolatileStatData.description = row.description;
            nonVolatileStatData.icon = Resources.Load<Sprite>(row.path);

            dict.TryAdd(id, nonVolatileStatData);
        }

        return dict;
    }

    public NonVolatileStatData GetNonVolatileStatData(int id)
    {
        if (stageDataDict.TryGetValue(id, out NonVolatileStatData nonVolatileStatData))
            return nonVolatileStatData;
        else
        {
            throw new Exception($"테이블에 없는 {id} 입니다.");
        }
    }
    
    NonVolatileStatDataRow[] nonVolatileStatRows;
    Dictionary<int, NonVolatileStatData> stageDataDict = new Dictionary<int, NonVolatileStatData>();
}
