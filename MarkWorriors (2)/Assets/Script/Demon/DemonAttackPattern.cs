using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonAttackPattern : MonoBehaviour
{
    //Player을 target으로 설정한다.
    //추적범위 밖에 target이 있으면 Idle 상태
    //추적범위 안에 target이 들어오면 target을 향해 이동하고 싶다.(Chase 상태)
    //공격범위 안에 들어오면 공격하고 싶다.(Attack 상태)
    //hp가 0이 되면(Death 상태)

    //state : Idle, Patrol, Chase, Attack, Death
    enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        React,
        Death
    }

    State state;

    NavMeshAgent nvAgent;
    GameObject target;
    MonsterAttackActivate attackArea;
    public GameObject traceZone; //몬스터의 추적범위 공간(sphere오브젝트)
    public Animator anim;

    float traceRadius; //traceZone의 반지름
    int patrolIndex;

    bool isPatrol;
    bool isChase;
    bool isAttack;
    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        traceRadius = traceZone.transform.localScale.x * 0.5f;
        state = State.Patrol;
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
        print("UpdateAttack");
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리

        //공격 시도 중 플레이어가 범위 밖으로 나가면 미끄러지듯 접근하게 된다.(추적중지)
        //공격하는 동안 추적을 멈추고 싶다.
        nvAgent.isStopped = true;
        //공격 중이 아니라면
        if (!isAttack)
        {
            //공격하는 모션을 넣어주고 싶다.
            anim.SetTrigger("Attack");
            isAttack = true;
            isChase = false;
        }

        //몬스터가 플레이어를 바라보게 만들고 싶다.
        Vector3 monsterLookForward = target.transform.position;
        monsterLookForward.y = transform.position.y;
        transform.LookAt(monsterLookForward);

        //Attack 범위 밖으로 target이 나가면
        if (distToPlayer > nvAgent.stoppingDistance)
        {
            //공격 중이 아니라는 걸 알리고 싶다.
            isAttack = false;
            //상태를 Chase로 바꾸고 싶다.
            state = State.Chase;
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

    private void UpdateChase()
    {
        //추적을 다시 시작하고 싶다.
        nvAgent.isStopped = false;
        //몬스터의 이동속도를 4로 하고 싶다.
        nvAgent.speed = 4;
        //이전 상태가 추적상태가 아니었으면
        if (!isChase)
        {
            anim.SetTrigger("Chase");
            anim.ResetTrigger("Attack");
            anim.ResetTrigger("React");
            isChase = true;
            isAttack = false;
            isPatrol = false;
        }
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리
        //target 쪽으로 추적을 하고 싶다.
        nvAgent.destination = target.transform.position;

        //추적범위 밖으로 target이 벗어나면
        if (distToPlayer > traceRadius)
        {
            ////Player가 추적범위를 벗어나도 일정 거리 이상 몬스터가 쫓아온다.
            ////추적범위를 벗어난 순간 바로 추적을 멈추고 싶다.
            //nvAgent.isStopped = true;
            //상태를 Patrol로 바꾸고 싶다.
            state = State.Patrol;
        }
        else if (distToPlayer <= nvAgent.stoppingDistance)
        {
            //공격상태로 전이하고 싶다.
            state = State.Attack;
        }
    }

    private void UpdatePatrol()
    {   
        //몬스터의 이동속도를 2로 하고 싶다.
        nvAgent.speed = 2;
        //이전 상태가 이동이 아니라면
        if (!isPatrol)
        {
            print("Patrol ON");
            //이동하는 모션을 넣어주고 싶다.
            anim.SetTrigger("Patrol");
            anim.ResetTrigger("Chase");
            isChase = false;
            isAttack = false;
            isPatrol = true;
        }

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
            state = State.Chase;
        }
    }

    public void GetDamageFromPlayer()
    {
        ////플레이어에게 공격을 받으면 playerAttackValue만큼 몬스터의 체력을 깎고 싶다.
        //monsterHP = GetComponent<MonsterHP>();
        //monsterHP.HP -= playerAttackValue;

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
            anim.SetTrigger("React");
            anim.ResetTrigger("Attack");

            isAttack = false;
            isChase = false;
            isPatrol = false;
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
