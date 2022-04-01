using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonAttackPattern : MonsterAttackPatternCommon
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nvAgent = GetComponent<NavMeshAgent>();
        traceRadius = traceZone.transform.localScale.x * 0.5f;
        setState(State.Idle, "Idle");
        //        patrolIndex = UnityEngine.Random.Range(0, PatrolLocation.instance.patrolPoints.Length);
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
        //공격 시도 중 플레이어가 범위 밖으로 나가면 미끄러지듯 접근하게 된다.(추적중지)
        //공격하는 동안 추적을 멈추고 싶다.
        nvAgent.isStopped = true;

        setState(State.Attack, "Attack");

        //몬스터가 플레이어를 바라보게 만들고 싶다.
        Vector3 monsterLookForward = target.transform.position;
        monsterLookForward.y = transform.position.y;
        transform.LookAt(monsterLookForward);
        Vector3 dir = transform.position - target.transform.position;

        // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 2);
    }
    IEnumerator IERush()
    {

        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리
        isAttack = true;
        state = State.Rush;
        anim.SetTrigger("Rush");
        yield return new WaitForSeconds(1f);
        nvAgent.enabled = false;
        rb.isKinematic = false;
        rb.AddForce(transform.forward * 30, ForceMode.Impulse);
        yield return new WaitForSeconds(0.7f);
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        if (distToPlayer > nvAgent.stoppingDistance)
        {
            print("Idle");
            nvAgent.enabled = true;
            isAttack = false;
            setState(State.Idle, "Idle");
            yield break;
        }
        // rb.isKinematic = true;
        nvAgent.enabled = true;
        isAttack = false;
        setState(State.Chase, "Chase");
    }

    private void UpdateChase()
    {
        //print("Chase");
        //추적을 다시 시작하고 싶다.
        nvAgent.isStopped = false;

        //몬스터의 이동속도를 4로 하고 싶다.
        nvAgent.speed = 4;

        // setState(State.Chase, "Chase");
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리

        //target 쪽으로 추적을 하고 싶다.
        nvAgent.destination = target.transform.position;



        //몬스터의 정면에 플레이어가 있다면 돌진을 하고 싶다.
        RaycastHit[] attackTarget = Physics.SphereCastAll(transform.position, 3f, transform.forward, traceRadius - 5, 1 << LayerMask.NameToLayer("Player"));

        if (distToPlayer >= traceRadius - 5 && attackTarget.Length > 0 && isAttack == false)
        {
            //돌진하고 싶다.
            StartCoroutine(IERush());
        }
        else if (distToPlayer <= nvAgent.stoppingDistance)
        {
            StopCoroutine(IERush());
            setState(State.Attack, "Attack");
        }


    }

    private void UpdateIdle()
    {

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