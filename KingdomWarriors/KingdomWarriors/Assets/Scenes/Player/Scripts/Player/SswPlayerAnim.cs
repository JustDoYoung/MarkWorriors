using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerAnim : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIdle(){
        anim.SetTrigger("Idle1");
    }

    public void SetRun(Vector3 velocity){
        SetRun(velocity.z, velocity.x);
    }

    public void SetRun(float v, float h){
        anim.SetFloat("RunV", v);
        anim.SetFloat("RunH", h);
    }

    public void SetRun(bool isRun){
        anim.SetBool("Run", isRun);
    }

    public void SetWalk(Vector3 velocity){
        SetWalk(velocity.z, velocity.x);
    }

    public void SetWalk(float v, float h){
        anim.SetFloat("WalkV", v);
        anim.SetFloat("WalkH", h);
    }

    public void SetWalk(bool isWalk){
        anim.SetBool("Walk", isWalk);
    }

    public void Jump(){
        anim.SetTrigger("Jump");
    }

    public void NoramlAttack(){
        anim.SetTrigger("Attack1");
    }

    public void TempComboAttack(){
        print("attack2 anim");
        anim.SetTrigger("Attack2");
    }

    public void HeavyAttack(){
        anim.SetTrigger("Attack3");
    }
}