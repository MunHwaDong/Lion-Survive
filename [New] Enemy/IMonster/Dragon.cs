using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Dragon : IMonster
{
    protected override void InitializeBlackboard()
    {
        blackboard.Set(MonsterDataType.ID, 1);
        blackboard.Set(MonsterDataType.HP, eachMonsterSpecificStatusData.maxHealth);
        blackboard.Set(MonsterDataType.SPEED, eachMonsterSpecificStatusData.moveSpeed);
        blackboard.Set(MonsterDataType.ATTACK, eachMonsterSpecificStatusData.attackPower);
        blackboard.Set(MonsterDataType.EXP, eachMonsterSpecificStatusData.experience);
        blackboard.Set(MonsterDataType.POINT, eachMonsterSpecificStatusData.point);
        blackboard.Set(MonsterDataType.ATTACK_RANGE, eachMonsterSpecificStatusData.attackRange);
        
        blackboard.Set(MonsterDataType.IS_FIND_PLAYER, false);
        blackboard.Set(MonsterDataType.IS_MOVE_ON_PATH, false);
        blackboard.Set(MonsterDataType.IS_ENCOUNTER, false);
    }
    
    protected override void ConstructBehaviourTree()
    {
        BTSelectorNode root = new BTSelectorNode(blackboard, this);
        
        BTSequenceNode attackSequence = new BTSequenceNode(blackboard, this);
        
        attackSequence.AddChild(new CheckAttackRangeActionNode(blackboard, this));
        attackSequence.AddChild(new AttackActionNode(blackboard, this));
        
        root.AddChild(attackSequence);
        
        BTSelectorNode decidePathSequence = new BTSelectorNode(blackboard, this);
        
        decidePathSequence.AddChild(new DetectHurdleActionNode(blackboard, this));
        decidePathSequence.AddChild(new PathFindingMovementActionNode(blackboard, this));
        //decidePathSequence.AddChild(new AvoidObstacleActionNode(blackboard, this));
        //decidePathSequence.AddChild(new FindPathActionNode(blackboard, this));
        
        root.AddChild(decidePathSequence);
        
        //root.AddChild(new MovementActionNode(blackboard, this));
        
        _root = root;
    }
}