using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonAttackPattern : MonsterAttackPattern
{
    //Player�� target���� �����Ѵ�.
    //�������� �ۿ� target�� ������ Idle ����
    //�������� �ȿ� target�� ������ target�� ���� �̵��ϰ� �ʹ�.(Chase ����)
    //���ݹ��� �ȿ� ������ �����ϰ� �ʹ�.(Attack ����)
    //hp�� 0�� �Ǹ�(Death ����)
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
        if (distToPlayer >= traceRadius)
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
}
