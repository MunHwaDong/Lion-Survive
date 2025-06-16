using System;

public abstract class IDataContainer
{
    public void Init(Type type)
    {
        DataTypes.Register(type, GetData, SetData);
    }
    
    public abstract object GetData(Enum type);
    public abstract void SetData(Enum type, object data);
}
