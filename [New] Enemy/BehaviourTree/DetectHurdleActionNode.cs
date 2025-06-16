using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DetectHurdleActionNode : BTNode
{
    public DetectHurdleActionNode(Blackboard blackboard, IMonster monster)
    {
        Blackboard = blackboard;
        Monster = monster;
    }

    public override NodeState EvaluateBehaviour()
    {
        if(Physics.Raycast(Monster.transform.position, GameManager.Instance.PlayerPosition.transform.position - Monster.transform.position, out var hit, Monster.OBSTACLE_DETECTION_DISTANCE, 1 << LayerMask.NameToLayer("Obstacle")))
        {
            Blackboard.Set(MonsterDataType.IS_FIND_PLAYER, false);
            return NodeState.FAILURE;
        }
        
        if(Blackboard.Get<bool>(MonsterDataType.IS_ENCOUNTER) is false)
        {
            Blackboard.Set(MonsterDataType.IS_FIND_PLAYER, true);
            
            Debug.DrawRay(Monster.transform.position, GameManager.Instance.PlayerPosition.transform.position - Monster.transform.position, Color.green);
            
            Monster.transform.position = Vector3.MoveTowards(Monster.transform.position, GameManager.Instance.PlayerPosition.transform.position, (Time.deltaTime * 6f));
            Monster.transform.rotation = Quaternion.LookRotation(GameManager.Instance.PlayerPosition.transform.position - Monster.transform.position);
            
            return NodeState.SUCCESS;
        }
        
        return NodeState.FAILURE;
    }
}