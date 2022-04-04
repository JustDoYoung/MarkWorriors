using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UndeadKnightAttackPattern : MonsterAttackPatternCommon
{
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
        if (GameObject.Find("Player") == null)
        {
            setState(State.Idle, "Idle");
            print("player null");
        }

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

        setState(State.Attack, "Attack");

        //공격범위 안에서 플레이어가 몬스터의 뒤쪽으로 이동하면 몬스터가 플레이어의 방향을 인지하지 못하고 허공을 때림(추적기능을 꺼서 그런듯)
        //공격범위 안에서 몬스터가 플레이어의 방향을 계속 추적하고 싶다.
        Vector3 monsterLookForward = target.transform.position;
        monsterLookForward.y = transform.position.y;
        transform.LookAt(monsterLookForward);

        //Attack 범위 밖으로 target이 나가면
        if (distToPlayer > nvAgent.stoppingDistance)
        {
            setState(State.Chase, "Chase");
        }
    }

    private void UpdateChase()
    {
        //추적을 다시 시작하고 싶다.
        nvAgent.isStopped = false;

        //setState(State.Chase, "Chase");

        //target 쪽으로 추적을 하고 싶다.
        nvAgent.destination = target.transform.position;

        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리
        if (distToPlayer < nvAgent.stoppingDistance)
        {
            //공격상태로 전이하고 싶다.
            // state = State.Attack;
            setState(State.Attack, "Attack");
        }
    }

    private void UpdateIdle()
    {
        //Idle 상태에선 추적기능을 멈추고 싶다.
        nvAgent.isStopped = true;

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
