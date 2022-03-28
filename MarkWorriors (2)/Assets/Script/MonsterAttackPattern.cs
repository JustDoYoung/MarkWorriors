using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAttackPattern : MonoBehaviour
{
    protected enum State
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        React,
        Death
    }

    protected State state;

    protected NavMeshAgent nvAgent;
    protected GameObject target;
    protected MonsterAttackActivate attackArea;
    public GameObject traceZone; //������ �������� ����(sphere������Ʈ)
    public Animator anim;

    protected float traceRadius; //traceZone�� ������
    protected int patrolIndex;

    protected bool isPatrol;
    protected bool isChase;
    protected bool isAttack;
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
            //���¸� React�� �ϰ� �ʹ�.
            anim.SetTrigger("React");
            anim.ResetTrigger("Attack");
            isAttack = false;
            isChase = false;
            isPatrol = false;
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
