using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDataContainer : IDataContainer
{
    private readonly Dictionary<BossStatus, object> _statuses = new();
    
    private readonly Action<Enum, object> _publishMethod;

    public BossDataContainer(List<(BossStatus, float)> list, Action<Enum, object> publishMethod)
    {
        _publishMethod = publishMethod;
        
        foreach (var (type, value) in list)
        {
            _statuses.Add(type, value);
            _publishMethod?.Invoke(type, value);
        }

        Init(typeof(BossStatus));
    }
    
    public override void SetData(Enum status, object value)
    {
        if (_statuses.ContainsKey((BossStatus)status))
        {
            _statuses[(BossStatus)status] = value;
            _publishMethod?.Invoke(status, value);
        }
        else
            throw new Exception("Boss는 해당 Status Type을 가지지 않습니다.");
    }
    
    public override object GetData(Enum status)
    {
        if (_statuses.TryGetValue((BossStatus)status, out var val))
        {
            return val;
        }
        else
            throw new Exception("Boss는 해당 Status Type을 가지지 않습니다.");
    }

    public void InitializeStatuses(List<(BossStatus, object)> list)
    {
        foreach (var (type, value) in list)
        {
            _statuses[type] = value;
        }
    }
}
