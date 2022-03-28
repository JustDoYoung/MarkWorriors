using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAttackPattern : MonoBehaviour
{
    protected enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        React,
        Death
    }

    protected State state;

    protected NavMeshAgent nvAgent;
    protected GameObject target;
    protected MonsterAttackActivate attackArea;
    public GameObject traceZone; //몬스터의 추적범위 공간(sphere오브젝트)
    public Animator anim;

    protected float traceRadius; //traceZone의 반지름
    protected int patrolIndex;

    protected bool isPatrol;
    protected bool isChase;
    protected bool isAttack;
    public void GetDamageFromPlayer()
    {
        CharacterStatus characterStatus = gameObject.GetComponent<CharacterStatus>();
        int monsterHP = characterStatus.HP;
        //이미 몬스터의 체력이 0이면 타격을 무시한다.
        if (state == State.Death) return;

        nvAgent.isStopped = true;
        //몬스터의 체력이 0이 되면
        if (monsterHP <= 0)
        {
            print("Death On");
            //상태를 Death로 하고 싶다.
            state = State.Death;
            //Death 애니메이션 삽입
            anim.SetTrigger("Death");
        }//몬스터의 체력이 0이 아니라면 
        else
        {
            print("React On");
            //움찔하는 애니메이션을 넣고 싶다.
            //상태를 React로 하고 싶다.
            anim.SetTrigger("React");
            anim.ResetTrigger("Attack");
            isAttack = false;
            isChase = false;
            isPatrol = false;
        }
    }

    internal void OnMonsterAttackHit()
    {
        //몬스터가 때리는 순간 공격 콜라이더를 활성화시키고 싶다.
        attackArea = GetComponentInChildren<MonsterAttackActivate>();
        if (attackArea.isAttack)
        {
            attackArea.MonsterAttack();
        }
    }
    internal void OnMonsterReactAnimFinished()
    {
        print("React Off");
        ////추적을 다시 시작하고 싶다.
        nvAgent.isStopped = false;
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리
        //공격범위 안에 플레이어가 있다면
        if (distToPlayer <= nvAgent.stoppingDistance)
        {
            print("공격상태로 전이");
            //공격상태로 전이하고 싶다.
            state = State.Attack;
        }
        else
        {
            print("추적상태로 전이");
            //추적상태로 전이하고 싶다.
            state = State.Chase;
        }
    }

    internal void OnMonsterDeathAnimFinished()
    {
        Destroy(gameObject);
    }
}
