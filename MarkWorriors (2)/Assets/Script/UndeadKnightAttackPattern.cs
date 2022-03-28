using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UndeadKnightAttackPattern : MonsterAttackPattern
{
    //Player�� target���� �����Ѵ�.
    //�������� �ۿ� target�� ������ Idle ����
    //�������� �ȿ� target�� ������ target�� ���� �̵��ϰ� �ʹ�.(Chase ����)
    //���ݹ��� �ȿ� ������ �����ϰ� �ʹ�.(Attack ����)
    //hp�� 0�� �Ǹ�(Death ����)

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
        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target���� �Ÿ�

        //���� �õ� �� �÷��̾ ���ݹ��� ������ ������ �̲������� �����ϰ� �ȴ�.(������� ������ �׷���)
        //�����ϴ� ���� ������ ���߰� �ʹ�.
        nvAgent.isStopped = true;
        //���� ���� �ƴ϶��
        if (!isAttack)
        {
            //�����ϴ� ����� �־��ְ� �ʹ�.
            anim.SetTrigger("Attack");
            anim.ResetTrigger("Chase");
            isAttack = true;
            isChase = false;
            isIdle = false;
        }

        //���ݹ��� �ȿ��� �÷��̾ ������ �������� �̵��ϸ� ���Ͱ� �÷��̾��� ������ �������� ���ϰ� ����� ����(��������� ���� �׷���)
        //���ݹ��� �ȿ��� ���Ͱ� �÷��̾��� ������ ��� �����ϰ� �ʹ�.
        Vector3 monsterLookForward = target.transform.position;
        monsterLookForward.y = transform.position.y;
        transform.LookAt(monsterLookForward);

        //Attack ���� ������ target�� ������
        if (distToPlayer > nvAgent.stoppingDistance)
        {
            print("���� ���� ������ ����.");
            //���� ���� �ƴ϶�� �� �˸��� �ʹ�.
            isAttack = false;
            //���¸� Chase�� �ٲٰ� �ʹ�.
            state = State.Chase;
        }
    }

    private void UpdateChase()
    {
        //������ �ٽ� �����ϰ� �ʹ�.
        nvAgent.isStopped = false;

        //���� ���°� �������°� �ƴϾ�����
        if (!isChase)
        {
            print("isChase on");
            //���� �ִϸ��̼��� ���ϰ� �ʹ�.
            anim.SetTrigger("Chase");
            anim.ResetTrigger("Idle");
            anim.ResetTrigger("Attack");
            isChase = true;
            isAttack = false;
            isIdle = false;
        }

        //target ������ ������ �ϰ� �ʹ�.
        nvAgent.destination = target.transform.position;

        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target���� �Ÿ�
        //�������� ������ target�� �����
        if (distToPlayer >= traceRadius)
        {
            print("�������� ���");
            //���¸� Idle�� �ٲٰ� �ʹ�.
            state = State.Idle;
        }
        else if (distToPlayer < nvAgent.stoppingDistance)
        {
            //���ݻ��·� �����ϰ� �ʹ�.
            state = State.Attack;
        }
    }

    private void UpdateIdle()
    {
        nvAgent.isStopped = true;
        //���� ���°� Idle�� �ƴ϶��
        if (!isIdle)
        {
            print("Idle ON");
            //Idle ����� �־��ְ� �ʹ�.
            anim.SetTrigger("Idle");
            isChase = false;
            isAttack = false;
            isIdle = true;
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
}
