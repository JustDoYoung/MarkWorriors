using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UndeadKnightAttackPattern : MonoBehaviour
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

    bool isChase;
    bool isAttack;
    bool isIdle;

    Vector3 firstPosition;

    void Start()
    {
        nvAgent = GetComponent<NavMeshAgent>();
        traceRadius = traceZone.transform.localScale.x * 0.5f;
        state = State.Idle;
        //firstPosition = transform.position; //������ �ʱ� ������ġ
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
            ////���� ���� �ƴ϶�� �� �˸��� �ʹ�.
            //isAttack = false;
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

        ////Attack ���� ������ target�� ������
        //float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target���� �Ÿ�
        //if (distToPlayer > nvAgent.stoppingDistance)
        //{
        //    print("���� ���� ������ ����.");
        //    //���� ���� �ƴ϶�� �� �˸��� �ʹ�.
        //    isAttack = false;
        //    //���¸� Chase�� �ٲٰ� �ʹ�.
        //    state = State.Chase;
        //}
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
            anim.ResetTrigger("Idle");
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Chase");
            isChase = true;
            isAttack = false;
            isIdle = false;
        }

        //target ������ ������ �ϰ� �ʹ�.
        nvAgent.destination = target.transform.position;

        float distToPlayer = Vector3.Distance(transform.position, target.transform.position); //target���� �Ÿ�
        if (distToPlayer < nvAgent.stoppingDistance)
        {
            //���ݻ��·� �����ϰ� �ʹ�.
            state = State.Attack;
        }
    }

    private void UpdateIdle()
    {
        //float distToFirstPosition = Vector3.Distance(transform.position, firstPosition); //�ʱ� ������ġ���� �Ÿ�
        ////�ʱ� ������ġ���� 1m�� ����ٸ�
        //if (distToFirstPosition > 2f)
        //{
        //    //�ʱ� ������ġ�� ���ư���.
        //    nvAgent.destination = firstPosition;
        //    //������ġ�� ���ư� ������ Idle ��带 ������ �ʴ´�.
        //    return;
        //}

        //Idle ���¿��� ��������� ���߰� �ʹ�.
        nvAgent.isStopped = true;

        //���� ���°� Idle�� �ƴ϶��
        if (!isIdle)
        {
            print("Idle ON");
            //Idle ����� �־��ְ� �ʹ�.
            anim.ResetTrigger("Chase");
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

    public void GetDamageFromPlayer()
    {
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
            isIdle = false;
        }
    }

    //���׼��� ������
    internal void OnMonsterReactAnimFinished()
    {
        print("React Off");
        //////������ �ٽ� �����ϰ� �ʹ�.
        //nvAgent.isStopped = false;
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
