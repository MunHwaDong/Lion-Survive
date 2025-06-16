using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameDataTables
{
    private readonly string _gameTablePath = "GameTable";
    private readonly UserDataSO _userData = DataController.Instance.UserData;

    private readonly NonVolatileStatTable _nonVolatileStatDataTable = new();
    private readonly VolatileStatTable _volatileStatDataTable = new();
    private readonly MonsterTable _monsterDataTable = new();
    private readonly BossTable _bossTable = new();
    private readonly SkillDataStatTable _skillDataStatTable = new();

    public GameDataTables()
    {
        TryParsingTable();
    }
    
    /// <summary>
    /// 현재 플레이어의 Level에 맞는 스테이터스값을 반환한다.
    /// </summary>
    /// <param name="status">반환 받을 Status 종류(Enum)</param>
    /// <returns>해당 Status 수치</returns>
    public float GetNonVolatileStatus(Status status)
    {
        //테이블에서 Parsing하지 않는 타입
        if ((int)status <= -1)
            return 0;
        
        if (status == Status.MONEY)
            return _userData.money;
        
        var data = _nonVolatileStatDataTable.GetNonVolatileStatData((int)status);

        try
        {
            return data.attriValues[_userData.attriLevel[(int)status]];
        }
        catch (Exception e)
        {
            Debug.LogError($"param is {status}");
            Debug.LogException(e);
            throw;
        }
    }
    
    public NonVolatileStatData GetNonVolatileStatus(int status)
    {
        return _nonVolatileStatDataTable.GetNonVolatileStatData(status);
    }

    /// <summary>
    /// 인자로 입력한 item id에 맞는 (영향을 줄 영구 스테이터스 id, 수치값)을 반환합니다.
    /// </summary>
    /// <param name="item">item의 id</param>
    /// <returns>인자로 입력한 item id에 맞는 (영향을 줄 영구 스테이터스 id, 수치값)을 반환합니다.</returns>
    public VolatileStatData GetVolatileStatus(int item)
    {
        return _volatileStatDataTable.GetVolatileStatData(item);
    }

    public List<BossData> GetBossStatuses(int bossId)
    {
        return _bossTable.GetBossDatas(bossId);
    }

    public EnemyStatus GetMonsterData(int monsterId)
    {
        return _monsterDataTable.GetMonsterData(monsterId);
    }

    public List<SkillDataStat> GetSkillData(int skillId, int level = -1)
    {
        return _skillDataStatTable.GetSkillDataStat(skillId, level);
    }
    
    public bool TryParsingTable()
    {
        try
        {
            _nonVolatileStatDataTable.Parsing(_gameTablePath + "/NonVolatileStat");
            _volatileStatDataTable.Parsing(_gameTablePath + "/VolatileStat");
            _bossTable.Parsing(_gameTablePath + "/BossPhases");
            _monsterDataTable.Parsing(_gameTablePath + "/Monster");
            _skillDataStatTable.Parsing(_gameTablePath + "/SkillDataStat");
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
    }
}