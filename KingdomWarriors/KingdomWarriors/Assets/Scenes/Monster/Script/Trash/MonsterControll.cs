using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Trash{
    public class MonsterControll : MonoBehaviour
    {
        //���Ͱ� �ֺ��ݰ��� �����Ѵ�.
        //�ݰ濡 �÷��̾ ������ �����Ѵ�.
        public enum CurrentState { idle, trace, attack, dead };
        public enum MonsterType { A, B, C };

        public CurrentState curState = CurrentState.idle;
        public GameObject traceRange;
        public GameObject attackRange;
        public BoxCollider monsterCollider;
        public MonsterType monsterType;
        public int monsterHP;
        public float gravity = -9.8f;

        private Transform monsterTransform;
        private Transform playerTransform;
        private NavMeshAgent nvAgent;

        Vector3 velocity;
        Vector3 monsterLook;
        Material mat;
        CharacterController monsterCC;

        bool isAttack;
        bool isAttackDone;

        //���� �����Ÿ�
        private float traceDist;
        //���� �����Ÿ�
        private float attackDist;
        //��� ����
        private bool isDead = false;
        private Animator anim;
        // Start is called before the first frame update
        private void Awake()
        {
            Application.targetFrameRate = 40;
        }
        void Start()
        {
            monsterTransform = this.gameObject.GetComponent<Transform>();
            playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
            nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
            //anim = this.gameObject.GetComponent<Animator>();
            mat = this.gameObject.GetComponentInChildren<MeshRenderer>().material;
            monsterCC = this.gameObject.GetComponent<CharacterController>();

            traceDist = traceRange.transform.localScale.x * 0.5f;
            attackDist = attackRange.transform.localScale.x * 0.5f;

            StartCoroutine(this.CheckState());
            StartCoroutine(this.CheckStateForAction());
        }
        private void Update()
        {
            if (isAttackDone)
            {
                monsterCC.Move(-monsterTransform.forward * nvAgent.speed * Time.deltaTime);
            }
            if (isDead)
            {
                nvAgent.isStopped = true;
                if (!monsterCC.isGrounded)
                {
                    velocity.y += gravity * Time.deltaTime;
                    monsterCC.Move(velocity * Time.deltaTime);
                }
                else
                {
                    //StopCoroutine("OnDamge");
                }
            }
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
            MonsterMove();

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
            while (!isDead)
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
                        break;
                    case MonsterType.B:
                        yield return new WaitForSeconds(0.1f);
                        nvAgent.speed = 3.5f;
                        nvAgent.speed *= 3;
                        nvAgent.acceleration = 100f;
                        monsterCollider.enabled = true;
                        yield return new WaitForSeconds(0.5f);
                        nvAgent.speed = 3.5f;
                        nvAgent.acceleration = 8f;
                        monsterCollider.enabled = false;
                        isAttackDone = true;
                        yield return new WaitForSeconds(0.5f);
                        break;
                    case MonsterType.C:
                        break;
                }
                isAttackDone = false;
                isAttack = false;
                nvAgent.isStopped = false;
            }

        }
        void MonsterMove()
        {
            nvAgent.isStopped = false;
            nvAgent.SetDestination(playerTransform.position);
            monsterLook = playerTransform.position;
            monsterLook.y = transform.position.y;
            transform.LookAt(monsterLook);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Melee")
            {
                Weapon weapon = other.GetComponent<Weapon>();
                monsterHP -= weapon.damage;

                StartCoroutine(OnDamage());
            }
        }
        IEnumerator OnDamage()
        {
            Color monsterColor = mat.color;
            mat.color = Color.red;
            yield return new WaitForSeconds(0.1f);

            if (monsterHP > 0)
            {
                mat.color = monsterColor;
            }
            else
            {
                isAttack = false;
                isDead = true;
                //nvAgent.enabled = false; //��� ���׼��� �����ϱ� ����(nav�� ���������� mesh �������� ������.)

                mat.color = Color.gray;
                gameObject.layer = 10;

                velocity = (-monsterTransform.forward + monsterTransform.up) * 3;
                //�׾��� �� ���� ��¦ Ƣ������� ����߰��ؾ� ��.
                Destroy(gameObject, 3);
            }
        }
    }
}