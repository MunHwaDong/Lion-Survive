using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MovementActionNode : BTNode
{
    public MovementActionNode(Blackboard blackboard, IMonster monster)
    {
        Blackboard = blackboard;
        Monster = monster;
    }

    public override NodeState EvaluateBehaviour()
    {
        Debug.LogError("MovementActionNode.EvaluateBehaviour");
        return NodeState.SUCCESS;
    }
}