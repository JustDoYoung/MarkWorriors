using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerAnim : MonoBehaviour
{
    public class AnimTriggerNameNotFound : Exception{
        public AnimTriggerNameNotFound(string msg){}
    }

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
        //Logger.Log("무브체크", IsMove());
    }

    public bool IsMove(){
        return Anim.GetBool("Run") || Anim.GetBool("Walk");
    }

    private void SetTrigger(string name){
        if(Anim == null){
            //throw new NullReferenceException("Anim null");
        }
        Anim.SetTrigger(name);
        if(Anim.GetBool(name) == false){
            //throw new AnimTriggerNameNotFound("input name : " + name);
        }
    }

    private void StopMove(){
        SetRun(false);
        SetWalk(false);
    }

    public void SetStay(){
        SetRun(false);
        SetWalk(false);
        SetTrigger("Idle");
    }

    public void SetMove(Vector3 velocity){
        SetMove(velocity.z, velocity.x);
    }

    public void SetMove(float v, float h){
        Anim.SetFloat("MoveV", v);
        Anim.SetFloat("MoveH", h);
    }

    public void SetRun(bool isRun){
        Anim.SetBool("Run", isRun);
    }

    public void SetWalk(bool isWalk){
        Anim.SetBool("Walk", isWalk);
    }

    public void Jump(){
        StopMove();
        Anim.SetBool("Jump", true);
    }

    public void Dash(){
        Anim.SetBool("Dash", true);
    }

    public void DashEnd(){
        Anim.SetBool("Dash", false);
    }

    public void Landing(){
        Anim.SetBool("Jump", false);
        // 아직 랜딩 없음
        PlayerMediator.Stay();
        PlayerMediator.Idle();
    }

    public void NoramlAttack(){
        Attack();
        SetTrigger("NormalAttack");
    }

    public void TempComboAttack(){
        NoramlAttack();
    }

    public void HeavyAttack(){
        Attack();
        SetTrigger("HeavyAttack");
    }

    public void ResetComboCount(){
        Anim.ResetTrigger("NormalAttack");
        Anim.ResetTrigger("HeavyAttack");
        Anim.SetInteger("ComboCount", 0);
        SetStay();
    }

    private void Attack(){
      //  StopMove();
        Anim.ResetTrigger("NormalAttack");
        Anim.ResetTrigger("HeavyAttack");
        Anim.ResetTrigger("Attack");

        Anim.SetInteger("ComboCount", Anim.GetInteger("ComboCount")+1);
        SetTrigger("Attack");
    }
}