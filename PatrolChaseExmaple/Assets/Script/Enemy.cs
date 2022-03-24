using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class Enemy : MonoBehaviour
{
    //agent�� �̿��ؼ� ��ã�⸦ �ϰ�ʹ�.
    //����, ���� ���·� �����ϰ� �ʹ�.
    NavMeshAgent agent;

    public float detectedRadius = 5;
    public float chaseDistance = 15;
    GameObject chaseTarget; //reference Ÿ��
    enum State
    {
        Patrol,
        Chase
    }
    State state;
    //������ �ε��� ��ȣ
    int targetIndex;
    void Start()
    {
        state = State.Patrol;
        agent = GetComponent<NavMeshAgent>();
        //targetIndex = 0;
        //ó�� ���ϴ� �������� �������� �ϰ� �ʹ�.
        targetIndex = Random.Range(0, PathInfo.instance.wayPoints.Length);
    }


    void Update()
    {
        //FSM ���ø� : enum, state, switch
        switch (state)
        {
            case State.Patrol:
                UpdatePatrol();
                break;
            case State.Chase:
                UpdateChase();
                break;
        }
    }

    private void UpdateChase()
    {
        agent.destination = chaseTarget.transform.position;

        float dist = Vector3.Distance(transform.position, chaseTarget.transform.position);
        //���࿡ chaseTarget���� �Ÿ��� chaseDistance�� �����
        if (dist > chaseDistance)
        {
        //�������·� �����ϰ� �ʹ�.
            state = State.Patrol;
        }
    }

    private void UpdatePatrol()
    {
        //�ֺ� �ݰ� 5m �� �ȿ� �÷��̾ �ִٸ�
        int layerMask = 1 << LayerMask.NameToLayer("Player"); //int�� 4byte, <<(shift) : ��Ʈ����, 10000000
        //int layerMask = ~(1 << LayerMask.NameToLayer("Player")); // ~ : ��Ʈ�� ������Ŵ, Player ���� ��� �ֵ� 
        //int layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("�ٸ����̾�"); // | : ��Ʈ����(���ϱ�), 2�� �߿�~
        Collider[] cols = Physics.OverlapSphere(transform.position, detectedRadius, layerMask);
        //for (int i = 0; i < cols.Length; i++)
        //{
        //    if (cols[i].name.Contains("Player"))
        //    {
        //        state = State.Chase;
        //        chaseTarget = cols[i].gameObject;
        //        return;
        //    }
        //}
        if (cols.Length > 0 )
        {
            state = State.Chase;
            chaseTarget = cols[0].gameObject;
            return;
        }

        //�߰ݻ��·� ����� �ʹ�.
        Vector3 target = PathInfo.instance.wayPoints[targetIndex].transform.position;
        //���� ��ȯ�̵��ϰ� �ʹ�.
        agent.destination = target;
        //���� �����ߴٸ�? -> ���� ��������
        float dist = Vector3.Distance(transform.position, target);
        if(dist <= 1f)
        {
            //�ε����� 1������Ű�� �ʹ�
            targetIndex++;
            //���� targetIndex�� PathInfo.instance.wayPoints�� �迭 ũ�� �̻��̶�� 0���� �ǵ�����.
            //List�� ��� Count(�迭�� �ϳ��� �̾��� ������ ����, ����Ʈ�� �ϳ��ϳ� ����� ����)
            if (targetIndex >= PathInfo.instance.wayPoints.Length)
            {
                targetIndex = 0;
            }

            ////�����ڵ�(������)
            //targetIndex = (targetIndex + 1) % PathInfo.instance.wayPoints.Length;

            ////�����ڵ�(������)
            //targetIndex = (targetIndex + PathInfo.instance.wayPoints.Length - 1) % PathInfo.instance.wayPoints.Length;
        }


    }
}
