using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    CharacterController cc;
    public float speed = 5;
    float yVelocity;
    public float gravity = -9.8f;
    public int hp = 100;
    Color playerColor;

    bool isDamage;

    MeshRenderer meshes;
    private void Awake()
    {
        Application.targetFrameRate = 40;
    }
    void Start()
    {
        cc = this.gameObject.GetComponent<CharacterController>();
        meshes = this.gameObject.GetComponent<MeshRenderer>();
        playerColor = meshes.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, 0, v);
        dir = Camera.main.transform.TransformDirection(dir).normalized;
        dir.y = 0;
        Vector3 velocity = dir * speed;


        if (cc.isGrounded)
        {

        }
        else
        {
            yVelocity += gravity * Time.deltaTime;
        }
        velocity.y = yVelocity;
        //transform.LookAt(transform.position + dir);
        if (dir != Vector3.zero)
            transform.forward = dir;

        cc.Move(velocity * Time.deltaTime);
    }

    IEnumerator OnDamage()
    {
        isDamage = true;

            meshes.material.color = Color.red;
        
        yield return new WaitForSeconds(0.5f);
        isDamage = false;

            meshes.material.color = playerColor;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "MonsterAttack")
        {
            if (!isDamage)
            {
                AttackArea monsterAttack = other.gameObject.GetComponent<AttackArea>();
                hp -= monsterAttack.damage;
                StartCoroutine(OnDamage());
            }
        }
    }
}
