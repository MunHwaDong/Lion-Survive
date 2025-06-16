using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/StatusLevelSO", fileName = "StatusLevelSO")]
public class UserDataSO : ScriptableObject
{
    //idx 0 : NONE
    //idx 1 : ATTACK Level
    //idx 2 : COOLDOWN Level
    //idx 3 : CASTING Level
    //idx 4 : PROJECTILE NUM Level
    //idx 5 : SPEED Level
    //idx 6 : HP Level
    //idx 7 : RANGE Level
    //idx 8 : THROUGH Level
    //idx 9 : PROJECTILE SPEED Level
    //idx 10 : SKILL DURATION Level
    //idx 11 : AVOID Level
    //idx 12 : RESURRECTION Level
    //idx 13 : BLOOD ABSORBING Level
    private const int MAX_LEVEL = 9;

    [Tooltip("0레벨 부터 시작합니다.")]
    [NonVolaStatusNamesAttri(new string[]
    {
        "None", "공격력 증가 Level", "Cooldown Level",
        "투사체 증가 Level", "이동속도 Level", "Max HP Level", "공격 범위 증가 Level",
        "관통력 Level", "투사체 속도 Level", "스킬 지속시간 Level", "회피 Level", 
        "부활 Level", "흡혈 Level", "현재 경험치 습득량"
    })]
    public List<int> attriLevel;

    public int money;
    
    public Action<Enum, object> publishMethod;
    
    /// <summary>
    /// 플레이어의 특정 스테이터스 레벨을 증가시킵니다.
    /// </summary>
    /// <param name="status">증가시킬 스테이터스</param>
    /// <param name="increaseValue">증가값, 해당 인자를 입력하지 않는다면 기본값 1을 증가시킵니다.</param>
    /// <exception cref="IndexOutOfRangeException">Status Enum값의 범위 (1 ~ 14)를 벗어나면 예외를 던집니다.</exception>
    public void IncreaseLevel(Status status, int increaseValue = 1)
    {
        if (status == Status.NONE || (int)status >= attriLevel.Count)
            throw new IndexOutOfRangeException("해당 Status를 찾을 수 없습니다.");

        if (attriLevel[(int)status] >= MAX_LEVEL)
            return;
        
        attriLevel[(int)status] += increaseValue;
        publishMethod?.Invoke(status, attriLevel[(int)status]);
    }

    public void SetData(Enum attri, object value)
    {
        StatusAttri statusAttri = (StatusAttri)attri;

        if (attriLevel[(int)statusAttri] >= MAX_LEVEL)
            return; 
        
        if (CheckCondition(statusAttri) is false)
        {
            int valueInt = (int)value;
            
            attriLevel[(int)statusAttri] = valueInt;
            publishMethod?.Invoke(statusAttri, attriLevel[(int)statusAttri]);
        }
        else
            throw new IndexOutOfRangeException("해당 Status를 찾을 수 없습니다.");
        
    }
    
    public object GetData(Enum attri)
    {
        StatusAttri statusAttri = (StatusAttri)attri;

        if (CheckCondition(statusAttri) is false)
        {
            return attriLevel[(int)statusAttri];
        }
        else
            throw new IndexOutOfRangeException("해당 Status를 찾을 수 없습니다.");
        
    }

    private bool CheckCondition(StatusAttri statusAttri)
    {
        return statusAttri == StatusAttri.NONE || (int)statusAttri >= attriLevel.Count;
    }

    private void SetMoney(object amount)
    {
        money = Convert.ToInt32(amount);
    }

    public void SetPublishMethod(Action<Enum, object> method)
    {
        publishMethod = method;
        
        DataController.Instance.ObserveData(Status.MONEY, SetMoney);
    }
}
