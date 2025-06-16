// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class AvoidObstacleActionNode : BTNode
// {
//     public AvoidObstacleActionNode(Blackboard blackboard, IMonster monster)
//     {
//         Blackboard = blackboard;
//         Monster = monster;
//     }
//     
//     public override NodeState EvaluateBehaviour()
//     {
//         var path = Blackboard.Get<List<NavNode>>(MonsterDataType.PATH);
//         
//         if (path.Count <= 0 || idx >= path.Count)
//         {
//             return NodeState.FAILURE;
//         }
//
//         var nextNode = path[idx];
//         
//         Monster.transform.position = Vector3.MoveTowards(Monster.transform.position, nextNode.worldPosition,(Time.deltaTime * 4f));
//
//         Monster.transform.rotation = Quaternion.LookRotation(Monster.transform.position - nextNode.worldPosition, Vector3.up);
//         
//         if ((Monster.transform.position - nextNode.worldPosition).sqrMagnitude <= 0.1f)
//         {
//             Blackboard.Set(MonsterDataType.NEXT_NODE_IDX, idx + 1);
//         }
//
//         return NodeState.SUCCESS;
//     }
// }
