using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//지금까지 먹은 아이템 리스트
//각 아이템의 등급 = 획득 개수

//TODO: 스킬 쿨타임과 레벨 분리해서 관리하기
public class InGameDataContainer : IDataContainer
{
    private const int MAX_ITEMS = 8;
    
    private readonly Dictionary<Enum, (float, int)> _curItems = new();
    
    private readonly Action<Enum, object> _publishMethod;

    public InGameDataContainer(Action<Enum, object> publishMethod)
    {
        _publishMethod = publishMethod;

        SceneLoader.OnAnySceneLoadStart += Clear;

        Init(typeof(Item));
        Init(typeof(Skill));
    }
    
    public override object GetData(Enum type)
    {
        if (_curItems.TryGetValue(type, out var val))
        {
            return val;
        }
        else
        {
            Debug.LogError($"플레이어가 획득하지 못한 {type}에 접근 했습니다.");
            return (0f, 1);
        }
    }

    //param인 data로 Skill.LEVEL_UP이 들어오면, 스킬의 레벨을 올림.
    //LEVEL_UP이 아니면, 쿨 타임 수정을 함
    public override void SetData(Enum type, object data)
    {
        //스킬 레벨 업 처리
        if (_curItems.ContainsKey(type) && (data is Skill.LEVEL_UP || type is Item))
        {
            _curItems[type] = (_curItems[type].Item1, _curItems[type].Item2 + 1);
            _publishMethod?.Invoke(type, _curItems[type].Item2 + 1);
            return;
        }
        
        float value = Convert.ToSingle(data);
        
        if (_curItems.Count > MAX_ITEMS)
        {
            _publishMethod?.Invoke(type, false);
        }
        else if (_curItems.ContainsKey(type))
        {
            _curItems[type] = (value, _curItems[type].Item2);
            
            _publishMethod?.Invoke(type, _curItems[type]);
        }
        else
        {
            _curItems.Add(type, (value, 1));
        }
    }
    
    public List<(Enum, int)> GetItems()
    {
        List<(Enum, int)> items = new();

        foreach (var (key, value) in _curItems)
        {
            items.Add((key, value.Item2));
        }

        return items;
    }
    
    public void Clear()
    {
        _curItems.Clear();
    }
}
