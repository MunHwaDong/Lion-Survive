using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DataController : Singleton<DataController>
{
    [Header("유저 데이터 경로")]
    [SerializeField] private string playerDataPath = "SO/StatusLevelSO";
    
    private readonly Dictionary<Enum, UnityEvent<object>> _listeners = new();
    
    private UserDataSO _userDataSO;
    public UserDataSO UserData
    {
        get
        {
            if (_userDataSO is null)
            {
                _userDataSO = Resources.Load<UserDataSO>(playerDataPath);

                if (_userDataSO is null)
                    throw new Exception("잘 못된 경로에서 UserData를 불러옵니다.");
                
                DataTypes.Register(typeof(StatusAttri), UserData.GetData, UserData.SetData);
                UserData.SetPublishMethod(NotifyObservers);
            }

            return _userDataSO;
        }
        
        private set => _userDataSO = value;
    }

    private GameDataTables _gameDataTables;

    private PlayerDataContainer _playerDataContainer;
    public PlayerDataContainer PlayerDataContainer
    {
        get
        {
            if (_playerDataContainer is null)
            {
                if (_gameDataTables is null)
                    _gameDataTables = new();
                    
                _playerDataContainer = new(LoadPlayerBaseStatusData(), NotifyObservers);

                foreach (StatusAttri attri in Enum.GetValues(typeof(StatusAttri)))
                {
                    ObserveData(attri, _ =>
                    {
                        var stat = (Status)attri;
                        
                        stat.Set(_gameDataTables.GetNonVolatileStatus(stat));
                    });
                }
            }
            
            return _playerDataContainer;
        }
    }

    private InGameDataContainer _inGameDataContainer;
    public InGameDataContainer InGameDataContainer
    {
        get
        {
            if (_inGameDataContainer is null)
            {
                _inGameDataContainer = new(NotifyObservers);
            }
            
            return _inGameDataContainer;
        }
    }

    public Action<int> ApplyItemCallback;

    new void Awake()
    {
        base.Awake();
        
        Debug.Log(GetInstanceID() + " - " + nameof(DataController));
        
        if(_gameDataTables is null  && DataTypes.IsInitialized is false)
            Init();
    }
    
    private void Init()
    {
        //Scene 전환 할 때 마다 모든 Observer들을 초기화함
        //SceneLoader.OnAnySceneLoadStart += ClearObservers;
        
        SceneLoader.OnSceneLoadFinish += ClearPlayerData;
        
        ApplyItemCallback = ApplyItem;
        DataTypes.IsInitialized = true;
        
        _ = PlayerDataContainer;
        _ = InGameDataContainer;
    }

    private void ClearPlayerData(SceneId sceneId)
    {
        if (sceneId is SceneId.TitleScene)
            PlayerDataContainer.ResetStatuses(LoadPlayerBaseStatusData());
        else if (sceneId is SceneId.GameScene)
        {
            _playerDataContainer.SetData(Status.LEVEL, 1);
        }
    }

    public void ObserveData<T>(T type, UnityAction<object> listener) where T : Enum
    {
        if (DataTypes.IsThisDataType(type.GetType()) is false)
            throw new Exception("해당 데이터 타입이 존재하지 않습니다.");

        if (_listeners.TryGetValue(type, out var evnt) is false)
        {
            _listeners[type] = new UnityEvent<object>();
        }
        
        _listeners[type].AddListener(listener);
    }

    public void UnObserveData(Enum type, UnityAction<object> listener)
    {
        if (DataTypes.IsThisDataType(type.GetType()) is false)
            throw new Exception("해당 데이터 타입이 존재하지 않습니다.");

        if (_listeners.TryGetValue(type, out var evnt) is false)
        {
            Debug.LogWarning("해당 데이터 타입에 전달 받은 Listener가 없습니다.");
        }
        else
        {
            evnt.RemoveListener(listener);
        }
    }
    
    private void NotifyObservers(Enum type, object value)
    {
        if (_listeners.TryGetValue(type, out var evnt))
        {
            evnt?.Invoke(value);
        }
        else
        {
            Debug.LogWarning($"해당 {type} 이벤트가 존재하지 않습니다.");
        }
    }

    private void ClearObservers()
    {
        foreach (var (key, val) in _listeners)
        {
            if(key is Status.LEVEL) continue;
            
            val.RemoveAllListeners();
        }
    }
    
    //플레이어 스탯 레벨에 따라 다른 수치가 적용되는 것은 Table에서 해준다.
    private List<(Status, float)> LoadPlayerBaseStatusData()
    {
        List<(Status, float)> baseStats = new();
        
        foreach (Status status in Enum.GetValues(typeof(Status)))
        {
            if (status == Status.NONE) continue;
            
            baseStats.Add((status, _gameDataTables.GetNonVolatileStatus(status)));
        }

        return baseStats;
    }

    /// <summary>
    /// 보스에 대한 정보를 List<BossData> 형태로 반환합니다. idx와 phase인자는 선택적으로 넣을 수 있습니다. 단 둘 다 넣으면 null을 반환합니다.
    /// </summary>
    /// <param name="bossId">찾으려는 보스의 ID</param>
    /// <param name="idx">해당 보스ID의 특정 Row만을 반환합니다. 이 경우 List에는 단 한개의 원소만 존재합니다.</param>
    /// <param name="phase">해당 보스ID에서 특정 phase들을 가지는 Row들을 반환합니다.</param>
    /// <returns>idx와 phase 둘 다 입력하지 않는다면, 해당 보스 ID의 모든 데이터를 List 형태로 반환합니다.</returns>
    public List<BossData> GetBossData(int bossId, int idx = -1, int phase = -1)
    {
        if (idx != -1 && phase != -1)
        {
            Debug.LogWarning("idx인자와 phase인자 둘 다 입력할 수 없습니다.");
            return null;
        }
        else if (idx != -1)
        {
            return new List<BossData>() { _gameDataTables.GetBossStatuses(bossId)[idx] };
        }
        else if (phase != -1)
        {
            return _gameDataTables.GetBossStatuses(bossId).Where(p => p.phase == phase).ToList();
        }
        else
        {
            return _gameDataTables.GetBossStatuses(bossId);
        }
    }

    private void ApplyItem(int itemID)
    {
        var targetItem = _gameDataTables.GetVolatileStatus(itemID);
        
        if (targetItem is null) return;

        var applyTarget = (Status)targetItem.non_vola_id;

        applyTarget.Set(applyTarget.Get<float>() + targetItem.value);
    }

    public EnemyStatus GetMonsterData(int monsterId)
    {
        return _gameDataTables.GetMonsterData(monsterId);
    }

    public VolatileStatData GetVolatileStatData(int id)
    {
        return _gameDataTables.GetVolatileStatus(id);
    }
    
    // 추가
    public NonVolatileStatData GetNonVolatileStatData(int id)
    {
        return _gameDataTables.GetNonVolatileStatus(id);
    }

    /// <summary>
    /// ID를 기준으로 스킬 데이터를 찾습니다.
    /// </summary>
    /// <param name="id">찾으려는 스킬의 Obj ID</param>
    /// <param name="level">같은 Obj ID를 가지는 것들 중 특정 Level을 찾는다.</param>
    /// <returns>입력 받은 ID와 일치하는 모든 SkillData를 반환합니다. 두 번째 파라미터를 입력했다면, 단 하나의 Data를 List로 반환합니다.(List의 길이가 1)</returns>
    public List<SkillDataStat> GetSkillDataStat(int id, int level = -1)
    {
        return _gameDataTables.GetSkillData(id, level);
    }
}
