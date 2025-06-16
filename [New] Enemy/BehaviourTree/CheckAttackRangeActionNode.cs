using System;
using UnityEngine;

public class CheckAttackRangeActionNode : BTNode
{
    public CheckAttackRangeActionNode(Blackboard blackboard, IMonster monster)
    {
        Blackboard = blackboard;
        Monster = monster;
    }

    public override NodeState EvaluateBehaviour()
    {
        if (Blackboard.Get<bool>(MonsterDataType.IS_ENCOUNTER))
        {
            Debug.Log("Find Player");
            
            Blackboard.Set(MonsterDataType.IS_ENCOUNTER, true);
            Blackboard.Set(MonsterDataType.IS_MOVE_ON_PATH, false);
            
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}