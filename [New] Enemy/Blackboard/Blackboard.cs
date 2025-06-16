using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    //변수명, 변수 값
    private readonly Dictionary<MonsterDataType, object> _data = new();
    
    public void ResetData()
    {
        var keys = _data.Keys.ToList();

        for (int i = 0; i < keys.Count; i++)
        {
            _data[keys[i]] = default;
        }
    }

    public T Get<T>(MonsterDataType dataType)
    {
        if (_data.TryGetValue(dataType, out var value))
        {
            try
            {
                return (T)value;
            }
            catch (Exception e)
            {
                Debug.LogError($"Blackboard에서 아래 예외가 발생했습니다. Req Data Type: {dataType}, Generic Data Type: {typeof(T)}, value : {value}");
                Debug.LogException(e);
                throw;
            }
        }
        else
        {
            throw new ArgumentException("Blackboard에 해당 변수 이름을 가지는 데이터가 없습니다.");
        }
    }

    public void Set<T>(MonsterDataType dataType, T value)
    {
        _data[dataType] = value;
    }

    public bool HasData(MonsterDataType dataType)
    {
        return _data.ContainsKey(dataType);
    }
}
