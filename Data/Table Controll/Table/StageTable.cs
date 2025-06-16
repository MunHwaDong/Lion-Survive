using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData
{
    public string stageNo;
    public string monsterNo;
    public string stageHP;
    public string stageAttk;
    public string rewardGold;
}

[System.Serializable]
public class StageDataRow
{
    public string stageNo;
    public string monsterNo;
    public string stageHP;
    public string stageAttk;
    public string rewardGold;
}

public class StageTable : BaseTable
{
    public override void Parsing(string jsonPath)
    {
        base.Parsing(jsonPath);

        stageTableRows = JsonHelper.FromJson<StageDataRow>(json);
        stageDataDict = ConvertListToDict();
    }
    
    private Dictionary<int, StageData> ConvertListToDict()
    {
        Dictionary<int, StageData> dict = new Dictionary<int, StageData>();

        foreach(var row in stageTableRows)
        {
            int key = System.Convert.ToInt32(row.stageNo);

            StageData stageData = new StageData();
            stageData.stageNo = row.stageNo;
            stageData.monsterNo = row.monsterNo;
            stageData.stageHP = row.stageHP;
            stageData.stageAttk = row.stageAttk;
            stageData.rewardGold = row.rewardGold;

            if (!dict.ContainsKey(key))
                dict.Add(key, stageData);
        }

        return dict;
    }

    public StageData GetStageData(int stageNo)
    {
        if(stageDataDict.ContainsKey(stageNo))
            return stageDataDict[stageNo];

        return null;
    }
    
    StageDataRow[] stageTableRows;
    Dictionary<int, StageData> stageDataDict = new Dictionary<int, StageData>();
}
