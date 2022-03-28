using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UndeadKnightAttackPattern : MonsterAttackPattern
{
    //Player을 target으로 설정한다.
    //추적범위 밖에 target이 있으면 Idle 상태
    //추적범위 안에 target이 들어오면 target을 향해 이동하고 싶다.(Chase 상태)
    //공격범위 안에 들어오면 공격하고 싶다.(Attack 상태)
    //hp가 0이 되면(Death 상태)

    bool isIdle;

    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        traceRadius = traceZone.transform.localScale.x * 0.5f;
        state = State.Idle;
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                UpdateIdle();
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
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리

        //공격 시도 중 플레이어가 공격범위 밖으로 나가면 미끄러지듯 접근하게 된다.(추적기능 때문에 그런듯)
        //공격하는 동안 추적을 멈추고 싶다.
        nvAgent.isStopped = true;
        //공격 중이 아니라면
        if (!isAttack)
        {
            //공격하는 모션을 넣어주고 싶다.
            anim.SetTrigger("Attack");
            anim.ResetTrigger("Chase");
            isAttack = true;
            isChase = false;
            isIdle = false;
        }

        //공격범위 안에서 플레이어가 몬스터의 뒤쪽으로 이동하면 몬스터가 플레이어의 방향을 인지하지 못하고 허공을 때림(추적기능을 꺼서 그런듯)
        //공격범위 안에서 몬스터가 플레이어의 방향을 계속 추적하고 싶다.
        Vector3 monsterLookForward = target.transform.position;
        monsterLookForward.y = transform.position.y;
        transform.LookAt(monsterLookForward);

        //Attack 범위 밖으로 target이 나가면
        if (distToPlayer > nvAgent.stoppingDistance)
        {
            print("공격 범위 밖으로 나감.");
            //공격 중이 아니라는 걸 알리고 싶다.
            isAttack = false;
            //상태를 Chase로 바꾸고 싶다.
            state = State.Chase;
        }
    }

    private void UpdateChase()
    {
        //추적을 다시 시작하고 싶다.
        nvAgent.isStopped = false;

        //이전 상태가 추적상태가 아니었으면
        if (!isChase)
        {
            print("isChase on");
            //추적 애니메이션을 취하고 싶다.
            anim.SetTrigger("Chase");
            anim.ResetTrigger("Idle");
            anim.ResetTrigger("Attack");
            isChase = true;
            isAttack = false;
            isIdle = false;
        }

        //target 쪽으로 추적을 하고 싶다.
        nvAgent.destination = target.transform.position;

        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리
        //추적범위 밖으로 target이 벗어나면
        if (distToPlayer >= traceRadius)
        {
            print("추적범위 벗어남");
            //상태를 Idle로 바꾸고 싶다.
            state = State.Idle;
        }
        else if (distToPlayer < nvAgent.stoppingDistance)
        {
            //공격상태로 전이하고 싶다.
            state = State.Attack;
        }
    }

    private void UpdateIdle()
    {
        nvAgent.isStopped = true;
        //이전 상태가 Idle이 아니라면
        if (!isIdle)
        {
            print("Idle ON");
            //Idle 모션을 넣어주고 싶다.
            anim.SetTrigger("Idle");
            isChase = false;
            isAttack = false;
            isIdle = true;
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
}
