using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UndeadKnightAttackPattern : MonoBehaviour
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

    bool isChase;
    bool isAttack;
    bool isIdle;

    Vector3 firstPosition;

    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        traceRadius = traceZone.transform.localScale.x * 0.5f;
        state = State.Idle;
        //firstPosition = transform.position; //몬스터의 초기 생성위치
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
            ////공격 중이 아니라는 걸 알리고 싶다.
            //isAttack = false;
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

        ////Attack 범위 밖으로 target이 나가면
        //float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리
        //if (distToPlayer > nvAgent.stoppingDistance)
        //{
        //    print("공격 범위 밖으로 나감.");
        //    //공격 중이 아니라는 걸 알리고 싶다.
        //    isAttack = false;
        //    //상태를 Chase로 바꾸고 싶다.
        //    state = State.Chase;
        //}
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
            anim.ResetTrigger("Idle");
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Chase");
            isChase = true;
            isAttack = false;
            isIdle = false;
        }

        //target 쪽으로 추적을 하고 싶다.
        nvAgent.destination = target.transform.position;

        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target과의 거리
        if (distToPlayer < nvAgent.stoppingDistance)
        {
            //공격상태로 전이하고 싶다.
            state = State.Attack;
        }
    }

    private void UpdateIdle()
    {
        //float distToFirstPosition = Vector3.Distance(transform.position, firstPosition); //초기 생성위치와의 거리
        ////초기 생성위치에서 1m를 벗어난다면
        //if (distToFirstPosition > 2f)
        //{
        //    //초기 생성위치로 돌아간다.
        //    nvAgent.destination = firstPosition;
        //    //생성위치로 돌아갈 때까지 Idle 모드를 취하지 않는다.
        //    return;
        //}

        //Idle 상태에선 추적기능을 멈추고 싶다.
        nvAgent.isStopped = true;

        //이전 상태가 Idle이 아니라면
        if (!isIdle)
        {
            print("Idle ON");
            //Idle 모션을 넣어주고 싶다.
            anim.ResetTrigger("Chase");
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
            anim.SetTrigger("React");
            anim.ResetTrigger("Attack");

            isAttack = false;
            isChase = false;
            isIdle = false;
        }
    }

    //리액션이 끝나면
    internal void OnMonsterReactAnimFinished()
    {
        print("React Off");
        //////추적을 다시 시작하고 싶다.
        //nvAgent.isStopped = false;
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
