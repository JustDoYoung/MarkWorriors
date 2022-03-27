using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonAttackPattern : MonoBehaviour
{
    //Player�� target���� �����Ѵ�.
    //�������� �ۿ� target�� ������ Idle ����
    //�������� �ȿ� target�� ������ target�� ���� �̵��ϰ� �ʹ�.(Chase ����)
    //���ݹ��� �ȿ� ������ �����ϰ� �ʹ�.(Attack ����)
    //hp�� 0�� �Ǹ�(Death ����)

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
    public GameObject traceZone; //������ �������� ����(sphere������Ʈ)
    public Animator anim;

    float traceRadius; //traceZone�� ������
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
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target���� �Ÿ�

        //���� �õ� �� �÷��̾ ���� ������ ������ �̲������� �����ϰ� �ȴ�.(��������)
        //�����ϴ� ���� ������ ���߰� �ʹ�.
        nvAgent.isStopped = true;
        //���� ���� �ƴ϶��
        if (!isAttack)
        {
            //�����ϴ� ����� �־��ְ� �ʹ�.
            anim.SetTrigger("Attack");
            isAttack = true;
            isChase = false;
        }

        //���Ͱ� �÷��̾ �ٶ󺸰� ����� �ʹ�.
        Vector3 monsterLookForward = target.transform.position;
        monsterLookForward.y = transform.position.y;
        transform.LookAt(monsterLookForward);

        //Attack ���� ������ target�� ������
        if (distToPlayer > nvAgent.stoppingDistance)
        {
            //���� ���� �ƴ϶�� �� �˸��� �ʹ�.
            isAttack = false;
            //���¸� Chase�� �ٲٰ� �ʹ�.
            state = State.Chase;
        }
    }
    internal void OnMonsterAttackHit()
    {
        //���Ͱ� ������ ���� ���� �ݶ��̴��� Ȱ��ȭ��Ű�� �ʹ�.
        attackArea = GetComponentInChildren<MonsterAttackActivate>();
        if (attackArea.isAttack)
        {
            attackArea.MonsterAttack();
        }
    }

    private void UpdateChase()
    {
        //������ �ٽ� �����ϰ� �ʹ�.
        nvAgent.isStopped = false;
        //������ �̵��ӵ��� 4�� �ϰ� �ʹ�.
        nvAgent.speed = 4;
        //���� ���°� �������°� �ƴϾ�����
        if (!isChase)
        {
            anim.SetTrigger("Chase");
            anim.ResetTrigger("Attack");
            anim.ResetTrigger("React");
            isChase = true;
            isAttack = false;
            isPatrol = false;
        }
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target���� �Ÿ�
        //target ������ ������ �ϰ� �ʹ�.
        nvAgent.destination = target.transform.position;

        //�������� ������ target�� �����
        if (distToPlayer > traceRadius)
        {
            ////Player�� ���������� ����� ���� �Ÿ� �̻� ���Ͱ� �Ѿƿ´�.
            ////���������� ��� ���� �ٷ� ������ ���߰� �ʹ�.
            //nvAgent.isStopped = true;
            //���¸� Patrol�� �ٲٰ� �ʹ�.
            state = State.Patrol;
        }
        else if (distToPlayer <= nvAgent.stoppingDistance)
        {
            //���ݻ��·� �����ϰ� �ʹ�.
            state = State.Attack;
        }
    }

    private void UpdatePatrol()
    {   
        //������ �̵��ӵ��� 2�� �ϰ� �ʹ�.
        nvAgent.speed = 2;
        //���� ���°� �̵��� �ƴ϶��
        if (!isPatrol)
        {
            print("Patrol ON");
            //�̵��ϴ� ����� �־��ְ� �ʹ�.
            anim.SetTrigger("Patrol");
            anim.ResetTrigger("Chase");
            isChase = false;
            isAttack = false;
            isPatrol = true;
        }

        //������ �����ϰ� �ʹ�.
        Vector3 patrolTarget = PatrolLocation.instance.patrolPoints[patrolIndex].transform.position;
        //���� ��ȯ �̵��ϰ� �ʹ�.
        nvAgent.destination = patrolTarget;
        //���� �����ߴٸ�? -> ���� ��������
        float dist = Vector3.Distance(transform.position, patrolTarget);
        if (dist <= 2f)
        {
            patrolIndex++;
        }
        //�ε����� ������ ������ �Ѿ��
        if (patrolIndex >= PatrolLocation.instance.patrolPoints.Length)
        {
            //0���� �ʱ�ȭ�ϰ� �ʹ�.
            patrolIndex = 0;
        }
        int layerMask = 1 << LayerMask.NameToLayer("Player"); //Player�� �̸��� ���� Layer �ε���
        //�������� traceZone �ȿ� Player�� �ִ��� ��� Ž���ϰ� �ʹ�.
        Collider[] cols = Physics.OverlapSphere(transform.position, traceRadius, layerMask);

        //�������� �ȿ� Player�� �ִٸ�
        if (cols.Length > 0)
        {
            target = cols[0].gameObject; //target�� Player�� �Ҵ�.  
            //���¸� Chase�� �ٲٰ� �ʹ�.
            state = State.Chase;
        }
    }

    public void GetDamageFromPlayer()
    {
        ////�÷��̾�� ������ ������ playerAttackValue��ŭ ������ ü���� ��� �ʹ�.
        //monsterHP = GetComponent<MonsterHP>();
        //monsterHP.HP -= playerAttackValue;

        CharacterStatus characterStatus = gameObject.GetComponent<CharacterStatus>();
        int monsterHP = characterStatus.HP;
        //�̹� ������ ü���� 0�̸� Ÿ���� �����Ѵ�.
        if (state == State.Death) return;

        nvAgent.isStopped = true;
        //������ ü���� 0�� �Ǹ�
        if (monsterHP <= 0)
        {
            print("Death On");
            //���¸� Death�� �ϰ� �ʹ�.
            state = State.Death;
            //Death �ִϸ��̼� ����
            anim.SetTrigger("Death");
        }//������ ü���� 0�� �ƴ϶�� 
        else
        {
            print("React On");
            //�����ϴ� �ִϸ��̼��� �ְ� �ʹ�.
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
        ////������ �ٽ� �����ϰ� �ʹ�.
        nvAgent.isStopped = false;
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target���� �Ÿ�
        //���ݹ��� �ȿ� �÷��̾ �ִٸ�
        if (distToPlayer <= nvAgent.stoppingDistance)
        {
            print("���ݻ��·� ����");
            //���ݻ��·� �����ϰ� �ʹ�.
            state = State.Attack;
        }
        else
        {
            print("�������·� ����");
            //�������·� �����ϰ� �ʹ�.
            state = State.Chase;
        }
    }
    
    internal void OnMonsterDeathAnimFinished()
    {
        Destroy(gameObject);
    }
}
