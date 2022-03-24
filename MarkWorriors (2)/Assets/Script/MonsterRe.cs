using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterRe : MonoBehaviour
{
    //Player�� target���� �����Ѵ�.
    //�������� �ۿ� target�� ������ Idle ����
    //�������� �ȿ� target�� ������ target�� ���� �̵��ϰ� �ʹ�.(Move ����)
    //���ݹ��� �ȿ� ������ �����ϰ� �ʹ�.(Attack ����)
    //hp�� 0�� �Ǹ�(Death ����)

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
    public GameObject traceZone; //������ �������� ����(sphere������Ʈ)
    float traceRadius; //traceZone�� ������
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
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target���� �Ÿ�

        //���Ͱ� �÷��̾ �ٶ󺸰� ����� �ʹ�.
        Vector3 monsterLookForward = target.transform.position;
        monsterLookForward.y = transform.position.y;
        transform.LookAt(monsterLookForward);

        //���ݽ���(���߿� �ִϸ��̼� �̺�Ʈ�� ���࿹��)
        attackArea = GetComponentInChildren<AttackArea>();
        if (attackArea.isAttack)
        {
            attackArea.MonsterAttack();
        }

        //Attack ���� ������ target�� ������
        if (distToPlayer > nvAgent.stoppingDistance)
        {
            //���¸� Move�� �ٲٰ� �ʹ�.
            state = State.Move;
        }
    }

    private void UpdateMove()
    {
        nvAgent.isStopped = false;
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target���� �Ÿ�
        //target ������ ������ �ϰ� �ʹ�.
        nvAgent.destination = target.transform.position;

        //�������� ������ target�� �����
        if (distToPlayer > traceRadius)
        {
            //Player�� ���������� ����� ���� �Ÿ� �̻� ���Ͱ� �Ѿƿ´�.
            //���������� ��� ���� �ٷ� ������ ���߰� �ʹ�.
            nvAgent.isStopped = true;
            //���¸� Patrol�� �ٲٰ� �ʹ�.
            state = State.Patrol;
        }
        //���ݹ��� ������ target�� ������
        else if (distToPlayer < nvAgent.stoppingDistance)
        {
            //���¸� Attack���� �ٲٰ� �ʹ�.
            state = State.Attack;
        }
    }

    private void UpdatePatrol()
    {
        ////UpdateMove���� ���� ���� �� nvAgent�� �ߴܽ�Ų ���±� ������ nvAgent.isStopped = false�� ���ش�
        //nvAgent.isStopped = false;
        ////������ �����ϰ� �ʹ�.
        //Vector3 patrolTarget = PatrolLocation.instance.patrolPoints[patrolIndex].transform.position;
        ////���� ��ȯ �̵��ϰ� �ʹ�.
        //nvAgent.destination = patrolTarget;
        ////���� �����ߴٸ�? -> ���� ��������
        //float dist = Vector3.Distance(transform.position, patrolTarget);
        //if (dist <= 2f)
        //{
        //    patrolIndex++;
        //}
        ////�ε����� ������ ������ �Ѿ��
        //if(patrolIndex >= PatrolLocation.instance.patrolPoints.Length)
        //{
        //    //0���� �ʱ�ȭ�ϰ� �ʹ�.
        //    patrolIndex = 0;
        //}
        int layerMask = 1 << LayerMask.NameToLayer("Player"); //Player�� �̸��� ���� Layer �ε���
        //�������� traceZone �ȿ� Player�� �ִ��� ��� Ž���ϰ� �ʹ�.
        Collider[] cols = Physics.OverlapSphere(transform.position, traceRadius, layerMask);

        //�������� �ȿ� Player�� �ִٸ�
        if (cols.Length > 0)
        {
            target = cols[0].gameObject; //target�� Player�� �Ҵ�.  
            //���¸� Move�� �ٲٰ� �ʹ�.
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
        //�̹� ������ ü���� 0�̸� Ÿ���� �����Ѵ�.
        if (monsterHP.hp <= 0) return;
        //�÷��̾�� ������ ������ playerAttackValue��ŭ ������ ü���� ��� �ʹ�.
        monsterHP.HP -= playerAttackValue;
        nvAgent.isStopped = true;
        //������ ü���� 0�� �Ǹ�
        if(monsterHP.hp == 0)
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
        }
    }
}
