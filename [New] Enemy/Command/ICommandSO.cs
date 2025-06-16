using System.Collections;
using System.Collections.Generic;
using MunHwaDong;
using UnityEngine;

public abstract class ICommandSO : ScriptableObject, ICommand
{
    public IMonster monster;
    
    public abstract void Execute();
}
