using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class Enemy : MonoBehaviour
{
    //agent를 이용해서 길찾기를 하고싶다.
    //순찰, 추적 상태로 제어하고 싶다.
    NavMeshAgent agent;

    public float detectedRadius = 5;
    public float chaseDistance = 15;
    GameObject chaseTarget; //reference 타입
    enum State
    {
        Patrol,
        Chase
    }
    State state;
    //목적지 인덱스 번호
    int targetIndex;
    void Start()
    {
        state = State.Patrol;
        agent = GetComponent<NavMeshAgent>();
        //targetIndex = 0;
        //처음 향하는 목적지를 랜덤으로 하고 싶다.
        targetIndex = Random.Range(0, PathInfo.instance.wayPoints.Length);
    }


    void Update()
    {
        //FSM 템플릿 : enum, state, switch
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
        //만약에 chaseTarget과의 거리가 chaseDistance를 벗어나면
        if (dist > chaseDistance)
        {
        //순찰상태로 전이하고 싶다.
            state = State.Patrol;
        }
    }

    private void UpdatePatrol()
    {
        //주변 반경 5m 구 안에 플레이어가 있다면
        int layerMask = 1 << LayerMask.NameToLayer("Player"); //int는 4byte, <<(shift) : 비트연산, 10000000
        //int layerMask = ~(1 << LayerMask.NameToLayer("Player")); // ~ : 비트를 반전시킴, Player 빼고 모든 애들 
        //int layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("다른레이어"); // | : 비트연산(더하기), 2개 중에~
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

        //추격상태로 만들고 싶다.
        Vector3 target = PathInfo.instance.wayPoints[targetIndex].transform.position;
        //길을 순환이동하고 싶다.
        agent.destination = target;
        //만약 도착했다면? -> 다음 목적지로
        float dist = Vector3.Distance(transform.position, target);
        if(dist <= 1f)
        {
            //인덱스를 1증가시키고 싶다
            targetIndex++;
            //만약 targetIndex가 PathInfo.instance.wayPoints의 배열 크기 이상이라면 0으로 되돌린다.
            //List의 경우 Count(배열은 하나로 이어져 나열된 상태, 리스트는 하나하나 연결된 상태)
            if (targetIndex >= PathInfo.instance.wayPoints.Length)
            {
                targetIndex = 0;
            }

            ////심플코드(순방향)
            //targetIndex = (targetIndex + 1) % PathInfo.instance.wayPoints.Length;

            ////심플코드(역방향)
            //targetIndex = (targetIndex + PathInfo.instance.wayPoints.Length - 1) % PathInfo.instance.wayPoints.Length;
        }


    }
}
