using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSequenceNode : BTNode
{
    private List<BTNode> _children = new();
    private int _currentChildIndex = 0;

    public BTSequenceNode(Blackboard blackboard, IMonster monster)
    {
        Blackboard = blackboard;
        Monster = monster;
    }

    public override NodeState EvaluateBehaviour()
    {
        while (_currentChildIndex < _children.Count)
        {
            var result = _children[_currentChildIndex].EvaluateBehaviour();

            if (result == NodeState.RUNNING)
                return NodeState.RUNNING;

            if (result == NodeState.FAILURE)
            {
                _currentChildIndex = 0;
                return NodeState.FAILURE;
            }
            
            _currentChildIndex++;
        }

        _currentChildIndex = 0;
        return NodeState.SUCCESS;
    }

    public void AddChild(BTNode child)
    {
        _children.Add(child);
    }
}
