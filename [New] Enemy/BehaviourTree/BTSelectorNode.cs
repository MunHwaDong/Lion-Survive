using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSelectorNode : BTNode
{
    private List<BTNode> _children = new();
    private int _currentChildIndex = 0;

    public BTSelectorNode(Blackboard blackboard, IMonster monster)
    {
        Blackboard = blackboard;
        Monster = monster;
    }

    public override NodeState EvaluateBehaviour()
    {
        while (_currentChildIndex < _children.Count)
        {
            var result = _children[_currentChildIndex].EvaluateBehaviour();
            
            if(result is NodeState.RUNNING)
                return NodeState.RUNNING;
            
            if (result is NodeState.FAILURE)
                _currentChildIndex++;
            
            if(result is NodeState.SUCCESS)
                return NodeState.SUCCESS;
        }

        _currentChildIndex = 0;
        return NodeState.FAILURE;
    }

    public void AddChild(BTNode child)
    {
        _children.Add(child);
    }
}
