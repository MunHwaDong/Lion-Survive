using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AttackActionNode : BTNode
{
    private bool _isAttacking;
    
    public AttackActionNode(Blackboard blackboard, IMonster monster)
    {
        Blackboard = blackboard;
        Monster = monster;
    }

    public override NodeState EvaluateBehaviour()
    {
        if (_isAttacking is false)
        {
            _isAttacking = true;
            WaitForAttackAnimation().Forget();
        }
        
        Debug.Log("Attack Action Node");

        return NodeState.SUCCESS;
    }
    
    public async UniTaskVoid WaitForAttackAnimation()
    {
        await Monster.Attack();
        
        _isAttacking = false;
    }
}