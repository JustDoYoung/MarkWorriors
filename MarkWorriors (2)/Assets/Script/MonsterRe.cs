using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterRe : MonoBehaviour
{
    //Player을 target으로 설정한다.
    //추적범위 밖에 target이 있으면 Idle 상태
    //추적범위 안에 target이 들어오면 target을 향해 이동하고 싶다.(Move 상태)
    //공격범위 안에 들어오면 공격하고 싶다.(Attack 상태)
    //hp가 0이 되면(Death 상태)

    //state : Idle, Patrol, Move, Attack, Death
    enum State
    {
        Idle,
        Patrol,
        Move,
        Attack,
        React,
        Death
    }

    State state;

    NavMeshAgent nvAgent;

    GameObject target;
    public GameObject traceZone; //몬스터의 추적범위 공간(sphere오브젝트)
    float traceRadius; //traceZone의 반지름
    AttackArea attackArea;
    int patrolIndex;
    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        traceRadius = traceZone.transform.localScale.x * 0.5f;
        state = State.Patrol;
        patrolIndex = 0;
    }

    void Update()
    {
        switch (state)
        {
            case State.Patrol:
                UpdatePatrol();
                break;
            case State.Move:
                UpdateMove();
                break;
            case State.Attack:
                UpdateAttack();
                break;
        }
    }

    private void UpdateAttack()
    {
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리

        //몬스터가 플레이어를 바라보게 만들고 싶다.
        Vector3 monsterLookForward = target.transform.position;
        monsterLookForward.y = transform.position.y;
        transform.LookAt(monsterLookForward);

        //공격실행(나중에 애니메이션 이벤트로 실행예정)
        attackArea = GetComponentInChildren<AttackArea>();
        if (attackArea.isAttack)
        {
            attackArea.MonsterAttack();
        }

        //Attack 범위 밖으로 target이 나가면
        if (distToPlayer > nvAgent.stoppingDistance)
        {
            //상태를 Move로 바꾸고 싶다.
            state = State.Move;
        }
    }

    private void UpdateMove()
    {
        nvAgent.isStopped = false;
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리
        //target 쪽으로 추적을 하고 싶다.
        nvAgent.destination = target.transform.position;

        //추적범위 밖으로 target이 벗어나면
        if (distToPlayer > traceRadius)
        {
            //Player가 추적범위를 벗어나도 일정 거리 이상 몬스터가 쫓아온다.
            //추적범위를 벗어난 순간 바로 추적을 멈추고 싶다.
            nvAgent.isStopped = true;
            //상태를 Patrol로 바꾸고 싶다.
            state = State.Patrol;
        }
        //공격범위 안으로 target이 들어오면
        else if (distToPlayer < nvAgent.stoppingDistance)
        {
            //상태를 Attack으로 바꾸고 싶다.
            state = State.Attack;
        }
    }

    private void UpdatePatrol()
    {
        ////UpdateMove에서 빠져 나올 때 nvAgent를 중단시킨 상태기 때문에 nvAgent.isStopped = false를 해준다
        //nvAgent.isStopped = false;
        ////거점을 지정하고 싶다.
        //Vector3 patrolTarget = PatrolLocation.instance.patrolPoints[patrolIndex].transform.position;
        ////길을 순환 이동하고 싶다.
        //nvAgent.destination = patrolTarget;
        ////만약 도착했다면? -> 다음 목적지로
        //float dist = Vector3.Distance(transform.position, patrolTarget);
        //if (dist <= 2f)
        //{
        //    patrolIndex++;
        //}
        ////인덱스가 거점의 개수를 넘어가면
        //if(patrolIndex >= PatrolLocation.instance.patrolPoints.Length)
        //{
        //    //0으로 초기화하고 싶다.
        //    patrolIndex = 0;
        //}
        int layerMask = 1 << LayerMask.NameToLayer("Player"); //Player의 이름을 가진 Layer 인덱스
        //추적범위 traceZone 안에 Player가 있는지 계속 탐색하고 싶다.
        Collider[] cols = Physics.OverlapSphere(transform.position, traceRadius, layerMask);

        //추적범위 안에 Player가 있다면
        if (cols.Length > 0)
        {
            target = cols[0].gameObject; //target을 Player로 할당.  
            //상태를 Move로 바꾸고 싶다.
            state = State.Move;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        GetDamageFromPlayer(1);
    }

    MonsterHP monsterHP;
    private void GetDamageFromPlayer(int playerAttackValue)
    {
        //이미 몬스터의 체력이 0이면 타격을 무시한다.
        if (monsterHP.hp <= 0) return;
        //플레이어에게 공격을 받으면 playerAttackValue만큼 몬스터의 체력을 깎고 싶다.
        monsterHP.HP -= playerAttackValue;
        nvAgent.isStopped = true;
        //몬스터의 체력이 0이 되면
        if(monsterHP.hp == 0)
        {
            //상태를 Death로 하고 싶다.
            state = State.Death;
            //Death 애니메이션 삽입
        }//몬스터의 체력이 0이 아니라면 
        else
        {
            //움찔하는 애니메이션을 넣고 싶다.
            //상태를 React로 하고 싶다.
            state = State.React;
        }
    }
}
