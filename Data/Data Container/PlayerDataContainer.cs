using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDataContainer : IDataContainer
{
    private const int INIT_NEXT_LEVEL = 8;
    
    private readonly Dictionary<Status, float> _statuses = new();
    
    private readonly Action<Enum, object> _publishMethod;

    public PlayerDataContainer(List<(Status, float)> list, Action<Enum, object> publishMethod)
    {
        _publishMethod = publishMethod;
        
        ResetStatuses(list);
        
        Init(typeof(Status));
    }
    
    public override void SetData(Enum status, object value)
    {
        if (_statuses.ContainsKey((Status)status))
        {
            _statuses[(Status)status] = Convert.ToSingle(value);
            _publishMethod?.Invoke(status, value);
        }
        else
            throw new Exception("플레이어는 해당 Status Type을 가지지 않습니다.");
    }
    
    public override object GetData(Enum status)
    {
        if (_statuses.TryGetValue((Status)status, out var val))
        {
            return val;
        }
        else
            throw new Exception("플레이어는 해당 Status Type을 가지지 않습니다.");
    }

    private async void CheckLevelUp(object currentEXP)
    {
        if (_statuses.TryGetValue(Status.NEXT_EXP, out var val) is false)
            throw new Exception($"Player Container에서 {Status.NEXT_EXP}를 찾을 수 없습니다.");

        if ((currentEXP is float currentExp ? currentExp : throw new InvalidCastException()) >= val)
        {
            SetData(Status.NEXT_EXP, val + 2);
            
            await UniTask.Yield();
            
            SetData(Status.EXP, currentExp - val);
            
            SetData(Status.LEVEL, _statuses[Status.LEVEL] + 1);
        }
    }

    public void ResetStatuses(List<(Status, float)> statuses)
    {
        foreach (var (type, value) in statuses)
        {
            //해당 클래스를 메모리에서 지우지 않으므로 Key를 포함하고 있는지 확인해야함
            if (_statuses.ContainsKey(type))
            {
                _statuses[type] = value;
            }
            else
            {
                _statuses.Add(type, value);
            }

            _publishMethod?.Invoke(type, value);
        }
        
        _statuses[Status.CURRENT_HP] = _statuses[Status.MAX_HP];
        _statuses[Status.NEXT_EXP] = INIT_NEXT_LEVEL;
        
        DataController.Instance.ObserveData(Status.EXP, CheckLevelUp);
        
        SetData(Status.LEVEL, 1);
    }
}

/*
 * 1. 스킬을 먹었을 떄 정보를 누가 줄 것인가?
 * Q1. 스킬은 드랍 아이템으로만 얻을 수 있나요?? -> no, Level Up과 Drop Item
 * Q2. 보물 상자에서만 "현재 가지고 있는 스킬들이 나타난다.
*/