using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerAnim : MonoBehaviour
{
    private Animator Anim {get; set;}
    private CharacterMediator PlayerMediator {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponentInChildren<Animator>();
        PlayerMediator = GetComponentInParent<CharacterMediator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void StopMove(){
        SetRun(false);
        SetWalk(false);
    }

    public void SetStay(){
        Anim.SetTrigger("Idle1");
    }

    public void SetRun(Vector3 velocity){
        SetRun(velocity.z, velocity.x);
    }

    public void SetRun(float v, float h){
        Anim.SetFloat("RunV", v);
        Anim.SetFloat("RunH", h);
    }

    public void SetRun(bool isRun){
        Anim.SetBool("Run", isRun);
    }

    public void SetWalk(Vector3 velocity){
        SetWalk(velocity.z, velocity.x);
    }

    public void SetWalk(float v, float h){
        Anim.SetFloat("WalkV", v);
        Anim.SetFloat("WalkH", h);
    }

    public void SetWalk(bool isWalk){
        Anim.SetBool("Walk", isWalk);
    }

    public void Jump(){
        StopMove();
        Anim.SetTrigger("Jump");
    }

    public void Landing(){
        // 아직 랜딩 없음
        PlayerMediator.Stay();
        PlayerMediator.Idle();
    }

    public void NoramlAttack(){
        StopMove();
        Anim.SetTrigger("Attack1");
    }

    public void TempComboAttack(){
        StopMove();
        print("attack2 Anim");
        Anim.SetTrigger("Attack2");
    }

    public void HeavyAttack(){
        StopMove();
        Anim.SetTrigger("Attack3");
    }
}