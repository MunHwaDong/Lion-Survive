using System;
using System.Collections.Generic;
using System.Reflection;

public static class DataTypes
{
    public static bool IsInitialized = false;
    
    private static readonly HashSet<Type> MembersType = new()
    {
        typeof(Status),
        typeof(Item),
        typeof(StatusAttri),
        typeof(Skill)
    };
    
    private static readonly Dictionary<Type, (Func<Enum, object>, Action<Enum, object>)> ContainerGetSetter = new();
    
    public static bool IsThisDataType(Type type) => MembersType.Contains(type);

    public static void Register(Type type, Func<Enum, object> getter, Action<Enum, object> setter)
    {
        if (ContainerGetSetter.ContainsKey(type) is false)
        {
            ContainerGetSetter.Add(type, (getter, setter));
        }
        else
        {
            throw new InvalidOperationException($"Type '{type.FullName}'은 이미 등록되어 있습니다.");
        }
    }

    public static T Get<T>(this Enum dataType)
    {
        if(IsThisDataType(dataType.GetType()) is false)
            throw new ArgumentException(dataType + " 타입은 Data Type이 아닙니다.");

        if (ContainerGetSetter.TryGetValue(dataType.GetType(), out var callbacks))
        {
            var result = (T)callbacks.Item1(dataType);

            return result ?? throw new Exception("제네릭 타입과 일치하지 않습니다.");
        }
        else
        {
            throw new Exception($"요청한 Type {dataType}에 등록된 Getter가 존재하지 않습니다.");
        }
    }
    
    //where T : Enum을 통해 제약을 걸어 boxing 문제를 회피함
    public static void Set<T>(this T dataType, object value) where T : Enum
    {
        if (IsThisDataType(dataType.GetType()) is false)
            throw new ArgumentException(dataType + " 타입은 Data Type이 아닙니다.");

        if (ContainerGetSetter.TryGetValue(dataType.GetType(), out var callbacks))
        {
            callbacks.Item2?.Invoke(dataType, value);
        }
        else
        {
            throw new Exception($"요청한 Type {dataType}에 등록된 Setter가 존재하지 않습니다.");
        }
    }

    public static void LevelUp(this Status dataType, int level = 1)
    {
        DataController.Instance.UserData.IncreaseLevel(dataType, level);
    }

    public static void Apply(this Item dataType)
    {
        DataController.Instance.ApplyItemCallback?.Invoke((int)dataType);
    }
}
