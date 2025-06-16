using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public abstract class IMonster : MonoBehaviour, IDamageable
{
    private readonly int attackHashInAnimator = Animator.StringToHash("Attack01");
    private readonly int dieAnimationHashInAnimator = Animator.StringToHash("Die");
    private readonly int takenDamageAnimationHashInAnimator = Animator.StringToHash("GetHit");
    public readonly float OBSTACLE_DETECTION_DISTANCE = 5f;
    
    public IObjectPool<IMonster> pool;

    protected BTNode _root;
    protected bool _isDead;
    protected EnemyStatus eachMonsterSpecificStatusData;

    [SerializeField] protected Animator animator;
    [SerializeField] protected Collider hitBoxCollider;
    [SerializeField] protected Collider damageBoxCollider;
    [SerializeField] protected Blackboard blackboard;
    
    public Blackboard Blackboard => blackboard;
    public Bounds MonsterBound => hitBoxCollider.bounds;
    public Collider DamageBoxCollider => damageBoxCollider;
    
    //죽을 때 Pool에 돌아가기전 자원들을 정리하기 위한 Call-back
    public Action OnDeath;

    public Action<Vector3> ItemDropMethod;
    
    protected abstract void InitializeBlackboard();
    //상속 받는 자식에서 BT 구조를 생성해준다.
    //TODO: 나중에는 BT 노드를 ScriptableObject로 만들어서 Pool을 구현하는 의미에 맞게(메모리 할당과 인스턴스 생성을 줄이자) 수정하자
    protected abstract void ConstructBehaviourTree();

    public void OnDamaged(DamageInfo damageInfo)
    {
        var currentHp = blackboard.Get<int>(MonsterDataType.HP);
        
        blackboard.Set(MonsterDataType.HP, (int)(currentHp - damageInfo.damage));

        if (blackboard.Get<int>(MonsterDataType.HP) <= 0)
        {
            _isDead = true;
            
            animator.Rebind();
            
            Die();
        }
        
        animator.SetTrigger(takenDamageAnimationHashInAnimator);
    }

    public virtual async void Die()
    {
        animator.SetTrigger(dieAnimationHashInAnimator);
        
        //길 찾기 중이였다면 토큰을 통해 Task를 강제 종료 후 자원을 정리한다.
        OnDeath?.Invoke();

        ResetMonster();
        
        ItemDropMethod?.Invoke(transform.position);

        float deathAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        
        await UniTask.WaitForSeconds(deathAnimationDuration);
        
        pool.Release(this);
    }

    public virtual async UniTask Attack()
    {
        animator.CrossFade(attackHashInAnimator, 0.2f);
        
        animator.SetTrigger(attackHashInAnimator);
        
        float attackAnimationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        
        await UniTask.WaitForSeconds(attackAnimationDuration);
    }

    private void ResetMonster()
    {
        hitBoxCollider.enabled = false;
        damageBoxCollider.enabled = false;
        
        Blackboard.ResetData();
    }

    private void InitializeMonsterSetting()
    {
        _isDead = false;
        
        hitBoxCollider.enabled = true;
        damageBoxCollider.enabled = true;
        
        animator.Rebind();
        
        InitializeBlackboard();
    }

    void OnEnable()
    {
        eachMonsterSpecificStatusData = DataController.Instance.GetMonsterData(1);
        
        InitializeMonsterSetting();
    }
    
    void Start()
    {
        ConstructBehaviourTree();
    }

    void Update()
    {
        if(_isDead is false)
            _root.EvaluateBehaviour();
    }
    
    public void EnableHitBox()
    {
        damageBoxCollider.enabled = true;
    }

    public void DisableHitBox()
    {
        damageBoxCollider.enabled = false;
    }
    
    //Not Used -> 추후 팀원들과 삭제에 대해서 이야기하기
    public void OnDamaged(float damage, Vector3 dir)
    {
        throw new NotImplementedException();
    }
}