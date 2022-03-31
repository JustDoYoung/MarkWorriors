using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAttackPatternCommon : MonoBehaviour
{
    protected enum State
    {
        Idle,
        Patrol,
        Rush,
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
    protected void setState(State next, string animationName)
    {
        if (state != next)
        {
            state = next;
            anim.Rebind();
            anim.SetTrigger(animationName);
            //anim.CrossFade(animationName, 0.1f);
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

    public void GetDamageFromPlayer()
    {
        CharacterStatus characterStatus = gameObject.GetComponent<CharacterStatus>();
        int monsterHP = characterStatus.HP;
        //이미 몬스터의 체력이 0이면 타격을 무시한다.
        if (state == State.Death) return;
        nvAgent.enabled = false;
        //몬스터의 체력이 0이 되면
        if (monsterHP <= 0)
        {
            setState(State.Death, "Death");
        }//몬스터의 체력이 0이 아니라면 
        else
        {
            setState(State.React, "React");
        }
    }

    internal void OnMonsterReactAnimFinished()
    {
        print("React Off");
        ////추적을 다시 시작하고 싶다.
        nvAgent.enabled = true;
        nvAgent.isStopped = false;
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리
        //공격범위 안에 플레이어가 있다면
        if (distToPlayer <= nvAgent.stoppingDistance)
        {
            // //공격상태로 전이하고 싶다.
            setState(State.Attack, "Attack");
        }
        else
        {
            // //추적상태로 전이하고 싶다.
            setState(State.Chase, "Chase");
        }
    }

    internal void OnMonsterDeathAnimFinished()
    {
        Destroy(gameObject);
    }
}
