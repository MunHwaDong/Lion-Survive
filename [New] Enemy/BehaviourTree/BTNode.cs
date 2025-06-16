using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTNode
{
    public Blackboard Blackboard
    {
        get
        {
            if (_blackboard is null)
            {
                throw new NullReferenceException("Blackboard를 할당하지 않았습니다.");
            }
            
            return _blackboard;
        }
        
        set => _blackboard = value;
    }
    
    public IMonster Monster
    {
        get
        {
            if (_monster is null)
            {
                throw new NullReferenceException("Monster를 할당하지 않았습니다.");
            }
            
            return _monster;
        }
        
        set => _monster = value;
    }
    
    private Blackboard _blackboard;
    private IMonster _monster;

    public abstract NodeState EvaluateBehaviour();
}
