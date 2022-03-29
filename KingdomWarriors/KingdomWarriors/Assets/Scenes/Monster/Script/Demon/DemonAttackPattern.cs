using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonAttackPattern : MonsterAttackPatternCommon
{
    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        traceRadius = traceZone.transform.localScale.x * 0.5f;
        setState(State.Patrol, "Patrol");
        patrolIndex = UnityEngine.Random.Range(0, PatrolLocation.instance.patrolPoints.Length);
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        switch (state)
        {
            case State.Patrol:
                UpdatePatrol();
                break;
            case State.Chase:
                UpdateChase();
                break;
            case State.Attack:
                UpdateAttack();
                break;
        }
    }

    private void UpdateAttack()
    {
        //공격 시도 중 플레이어가 범위 밖으로 나가면 미끄러지듯 접근하게 된다.(추적중지)
        //공격하는 동안 추적을 멈추고 싶다.
        nvAgent.isStopped = true;

       setState(State.Attack, "Attack");

        //몬스터가 플레이어를 바라보게 만들고 싶다.
        Vector3 monsterLookForward = target.transform.position;
        monsterLookForward.y = transform.position.y;
        transform.LookAt(monsterLookForward);
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

    private void UpdateChase()
    {
        //추적을 다시 시작하고 싶다.
        nvAgent.isStopped = false;
        //몬스터의 이동속도를 4로 하고 싶다.
        nvAgent.speed = 4;

        setState(State.Chase, "Chase");
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리
        //target 쪽으로 추적을 하고 싶다.
        nvAgent.destination = target.transform.position;

        //추적범위 밖으로 target이 벗어나면
        if (distToPlayer > traceRadius)
        {
            //상태를 Patrol로 바꾸고 싶다.
            setState(State.Patrol, "Patrol");
        }
        else if (distToPlayer < nvAgent.stoppingDistance+1)
        {
            print("전이"+isAttack);
            //공격상태로 전이하고 싶다.
            setState(State.Attack, "Attack");
        }
    }

    private void UpdatePatrol()
    {   
        //몬스터의 이동속도를 2로 하고 싶다.
        nvAgent.speed = 2;

        setState(State.Patrol, "Patrol");
     
        //거점을 지정하고 싶다.
        Vector3 patrolTarget = PatrolLocation.instance.patrolPoints[patrolIndex].transform.position;
        //길을 순환 이동하고 싶다.
        nvAgent.destination = patrolTarget;
        //만약 도착했다면? -> 다음 목적지로
        float dist = Vector3.Distance(transform.position, patrolTarget);
        if (dist <= 2f)
        {
            patrolIndex++;
        }
        //인덱스가 거점의 개수를 넘어가면
        if (patrolIndex >= PatrolLocation.instance.patrolPoints.Length)
        {
            //0으로 초기화하고 싶다.
            patrolIndex = 0;
        }
        int layerMask = 1 << LayerMask.NameToLayer("Player"); //Player의 이름을 가진 Layer 인덱스
        //추적범위 traceZone 안에 Player가 있는지 계속 탐색하고 싶다.
        Collider[] cols = Physics.OverlapSphere(transform.position, traceRadius, layerMask);

        //추적범위 안에 Player가 있다면
        if (cols.Length > 0)
        {
            target = cols[0].gameObject; //target을 Player로 할당.  
            //상태를 Chase로 바꾸고 싶다.
            setState(State.Chase, "Chase");
        }
    }
}