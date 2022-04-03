using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monster : MonoBehaviour
{
    public float speed = 3;
    public GameObject traceRange;
    public GameObject attackRange;
    private Vector3 dir;
    CharacterController cc;
    // Start is called before the first frame update
    private void Awake()
    {
        Application.targetFrameRate = 40;
    }
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, traceRange.transform.localScale.x*0.5f);

        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].gameObject.name.Contains("Player"))
            {
                GameObject player = cols[i].gameObject;
                float dist = Vector3.Distance(player.transform.position, transform.position);
                if(dist < attackRange.transform.localScale.x*0.5f)
                {
                    print("alert");
                   player.GetComponent<MeshRenderer>().material.color = Color.red;
                }
                dir = cols[i].gameObject.transform.position - transform.position;
                dir.y = 0;
                cc.Move(dir * speed * Time.deltaTime);
            }
        }
    }
    private void attack()
    {

    }
}


/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterControll : MonoBehaviour
{
    //몬스터가 주변반경을 감지한다.
    //반경에 플레이어가 있으면 접근한다.
    public enum CurrentState { idle, trace, attack, dead };
    public enum MonsterType { A, B, C};

    public CurrentState curState = CurrentState.idle;
    public GameObject traceRange;
    public GameObject attackRange;
    public BoxCollider monsterCollider;
    public MonsterType monsterType;

    private Transform monsterTransform;
    private Transform playerTransform;
    private NavMeshAgent nvAgent;

    Vector3 monsterLook;
    Rigidbody rigid;

    bool isAttack;

    //추적 사정거리
    private float traceDist;
    //공격 사정거리
    private float attackDist;
    //사망 여부
    private bool isDead = false;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        monsterTransform = this.gameObject.GetComponent<Transform>();
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        //anim = this.gameObject.GetComponent<Animator>();
        rigid = this.gameObject.GetComponent<Rigidbody>();

        traceDist = traceRange.transform.localScale.x * 0.5f;
        attackDist = attackRange.transform.localScale.x * 0.5f;
        
        StartCoroutine(this.CheckState());
        StartCoroutine(this.CheckStateForAction());
    }

    IEnumerator CheckState()
    {
        while (!isDead)
        {
            yield return null;

            float dist = Vector3.Distance(playerTransform.position, monsterTransform.position);

            //if (dist <= attackDist)
            //{
            //    curState = CurrentState.attack;
            //}
            //else if (dist <= traceDist)
            //{
            //    curState = CurrentState.trace;
            //}
            //else
            //{
            //    curState = CurrentState.idle;
            //}
        if (dist <= traceDist)
        {
            curState = CurrentState.trace;
        }
        else
        {
            curState = CurrentState.idle;
        }
    }
}

    IEnumerator CheckStateForAction()
    {
        while (!isDead)
        {
            switch (curState)
            {
                case CurrentState.idle:
                    nvAgent.isStopped = true;
                    //anim.SetBool("isTrace", false);
                    break;
                case CurrentState.trace:
                    MonsterMove();
                    Targeting();
                    //anim.SetBool("isTrace", true);
                    break;
                //case CurrentState.attack:
                //    nvAgent.isStopped = true;
                //    //anim.SetBool("isAttack", true);
                //    yield return new WaitForSeconds(0.2f);
                //    monsterCollider.enabled = true;
                //    yield return new WaitForSeconds(0.5f);
                //    //anim.SetBool("isAttack", false);
                //    monsterCollider.enabled = false;
                //    break;
            }
            yield return null;
        }
    }
    void Targeting()
    {
        float targetRadius = 0;
        float targetRange = 0;
        switch (monsterType)
        {
            case MonsterType.A:
                targetRadius = 1.5f;
                targetRange = attackDist;
                break;
            case MonsterType.B:
                targetRadius = 1f;
                targetRange = 10f;
                break;
            case MonsterType.C:
                break;
        }

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttack = true;
        nvAgent.isStopped = true;

        switch (monsterType)
        {
            case MonsterType.A:
                yield return new WaitForSeconds(0.2f);
                monsterCollider.enabled = true;

                yield return new WaitForSeconds(1f);
                monsterCollider.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case MonsterType.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                monsterCollider.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                monsterCollider.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case MonsterType.C:
                break;
        }
        isAttack = false;
        nvAgent.isStopped = false;
    }
    void MonsterMove()
    {
        nvAgent.isStopped = false;
        nvAgent.SetDestination(playerTransform.position);
        monsterLook = playerTransform.position;
        monsterLook.y = transform.position.y;
        transform.LookAt(monsterLook);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
*/