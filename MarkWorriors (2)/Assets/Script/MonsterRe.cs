using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterRe : MonoBehaviour
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
    AttackAreaActivate attackArea;
    public GameObject traceZone; //������ �������� ����(sphere������Ʈ)
    public Animator anim;

    float traceRadius; //traceZone�� ������
    int patrolIndex;
    bool isChase;
    bool isAttack;
    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        traceRadius = traceZone.transform.localScale.x * 0.5f;
        state = State.Patrol;
        patrolIndex = 0;
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
        print("???");
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target���� �Ÿ�

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
            //�ٽ� ������ �ϰ� �ʹ�.
            nvAgent.isStopped = false;
            //���¸� Chase�� �ٲٰ� �ʹ�.
            state = State.Chase;
        }
    }
    internal void OnMonsterAttackHit()
    {
        //���Ͱ� ������ ���� ���� �ݶ��̴��� Ȱ��ȭ��Ű�� �ʹ�.
        attackArea = GetComponentInChildren<AttackAreaActivate>();
        if (attackArea.isAttack)
        {
            attackArea.MonsterAttack();
        }
    }
    internal void OnMonsterAttackAnimHitFinish()
    {
        //������ ������ ���� ���� �ƴ϶�� �˷��ְ� �ʹ�.
        isAttack = false;
    }

    private void UpdateChase()
    {
        //���� ���°� �������°� �ƴϾ�����
        if (!isChase)
        {
            anim.SetTrigger("Chase");
            isChase = true;
            isAttack = false;
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
        //���� ���°� �̵��� �ƴ϶��
        if (!isChase)
        {
            //�̵��ϴ� ����� �־��ְ� �ʹ�.
            anim.SetTrigger("Chase");
            isChase = true;
            isAttack = false;
        }
        ////UpdateChase���� ���� ���� �� nvAgent�� �ߴܽ�Ų ���±� ������ nvAgent.isStopped = false�� ���ش�
        //nvAgent.isStopped = false;
        //������ �����ϰ� �ʹ�.
        Vector3 patrolTarget = PatrolLocation.instance.patrolPoints[patrolIndex].transform.position;
        //���� ��ȯ �̵��ϰ� �ʹ�.
        nvAgent.destination = patrolTarget;
        //���� �����ߴٸ�? -> ���� ��������
        float dist = Vector3.Distance(transform.position, patrolTarget);
        if (dist <= 1.5f)
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

    MonsterHP monsterHP;
    public void GetDamageFromPlayer(int playerAttackValue)
    {
        monsterHP = GetComponent<MonsterHP>();
        //�̹� ������ ü���� 0�̸� Ÿ���� �����Ѵ�.
        if (state == State.Death) return;
        //�÷��̾�� ������ ������ playerAttackValue��ŭ ������ ü���� ��� �ʹ�.
        monsterHP.HP -= playerAttackValue;
        nvAgent.isStopped = true;
        //������ ü���� 0�� �Ǹ�
        if (monsterHP.hp <= 0)
        {
            //���¸� Death�� �ϰ� �ʹ�.
            state = State.Death;
            //Death �ִϸ��̼� ����
        }//������ ü���� 0�� �ƴ϶�� 
        else
        {
            //�����ϴ� �ִϸ��̼��� �ְ� �ʹ�.
            //���¸� React�� �ϰ� �ʹ�.
            state = State.React;
            anim.SetTrigger("React");
            isAttack = false;
            isChase = false;
        }
    }

    internal void OnMonsterReactAnimFinished()
    {
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
