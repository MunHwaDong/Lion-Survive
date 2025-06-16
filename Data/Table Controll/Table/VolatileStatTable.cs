using System;
using System.Collections.Generic;
using UnityEngine;

public class VolatileStatData
{
    public int non_vola_id;
    public float value;
    public int maxUpGradeCount;
    public string name;
    public string description;
    public Sprite icon;
}

[System.Serializable]
public class VolatileStatDataRow
{
    public int id;
    public int non_vola_id;
    public string value;
    public int max_upgrade_count;
    public string name;
    public string description;
    public string path;
}

public class VolatileStatTable : BaseTable
{
    public override void Parsing(string jsonPath)
    {
        base.Parsing(jsonPath);

        volatileStatTableRows = JsonHelper.FromJson<VolatileStatDataRow>(json);
        volatileStatDataDict = ConvertListToDict();
    }

    private Dictionary<int, VolatileStatData> ConvertListToDict()
    {
        Dictionary<int, VolatileStatData> dict = new Dictionary<int, VolatileStatData>();

        foreach (var row in volatileStatTableRows)
        {
            VolatileStatData volatileStatData = new VolatileStatData();

            int id = row.id;

            volatileStatData.non_vola_id = row.non_vola_id;
            volatileStatData.value = Convert.ToSingle(row.value);

            volatileStatData.maxUpGradeCount = row.max_upgrade_count;
            volatileStatData.name = row.name;
            volatileStatData.description = row.description;
            volatileStatData.icon = Resources.Load<Sprite>(row.path);

            dict.TryAdd(id, volatileStatData);
        }

        return dict;
    }

    public VolatileStatData GetVolatileStatData(int id)
    {
        if (volatileStatDataDict.TryGetValue(id, out VolatileStatData volatileStatData))
            return volatileStatData;
        else
        {
            throw new Exception("테이블에 없는 id 입니다.");
        }
    }

    VolatileStatDataRow[] volatileStatTableRows;
    Dictionary<int, VolatileStatData> volatileStatDataDict = new Dictionary<int, VolatileStatData>();
}