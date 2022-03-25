using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface CharacterMediator{
    // 상태 시작 이벤트
    public void OnIdleStartEvent();
    public void OnWalkStartEvent();
    public void OnRunStartEvent();
    public void OnDashStartEvent();
    public void OnJumpStartEvent();
    public void OnAttackStartEvent();

    // 상태 종료 이벤트
    public void OnIdleEndEvent();
    public void OnWalkEndEvent();
    public void OnRunEndEvent();
    public void OnDashEndEvent();
    public void OnJumpEndEvent();
    public void OnAttackEndEvent();
}

public class CharacterStatus : MonoBehaviour
{
    public int deathExp = 6;

    private int level = 1;
    private int exp;

    private int needExp = 5;

    public int Level{get{return level;}}

    public int Exp{get{return exp;}}

    public int maxHP = 10;
    public int MaxHP{get{return maxHP;}}

    public int hp;
    public int HP{
        get{return hp;}
        private set{
            hp = value;
            if(hpSlider != null){hpSlider.value = value;}
        }
    }
    
    public int maxMP = 10;
    public int MaxMP{get{return maxMP;}}

    public int mp;
    public int MP{
        get{return hp;}
        private set{
            mp = value;
            if(mpSlider != null){mpSlider.value = value;}
        }
    }

    public enum CharacterState{
        Idle, Walk, Run, Dash, Jump, NoramlAttack, FinishAttack
    }

    // 임시로 테스트
    public bool isNoramlAttackLock = false;

    private CharacterState prevState = CharacterState.Idle;
    public CharacterState State {get;set;}

    private delegate void StateChangeEvent();

    public GameObject recoveryHpEffectFactory;
    public GameObject levelUpEffectFactory;

    public CharacterMediator playerMediator{get; set;}

    private Slider hpSlider;
    private Slider mpSlider;

    // Start is called before the first frame update
    void Start()
    {
        playerMediator = GetComponent<CharacterMediator>();
        foreach(Slider slider in GetComponentsInChildren<Slider>()){
            switch(slider.name){
                case "CharacterSliderHP" : hpSlider = slider; break;
                case "CharacterSliderMP" : mpSlider = slider; break;
            }
        }
        if(hpSlider != null){
            hpSlider.maxValue = MaxHP;
            HP = MaxHP;
        }
        if(mpSlider != null){
            hpSlider.maxValue = MaxMP;
            MP = MaxMP;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        Billboard();
        // print(IsDeath());
        // print(hp);
       // print(hpSlider);
    }

    private void Billboard(){
        if(hpSlider != null){hpSlider.transform.rotation = Camera.main.transform.rotation;}
        if(mpSlider != null){mpSlider.transform.rotation = Camera.main.transform.rotation;}
        
    }

    public void CheckState(){
        if(prevState == State){
            return;
        }
        StateChangeEvent stateChangeEvent = GetStateEndEvent(prevState);;
        prevState = State;

        if(stateChangeEvent != null){
            stateChangeEvent();
        }

        stateChangeEvent = GetStateStartEvent(State);
        if(stateChangeEvent != null){
            stateChangeEvent();
        }
    }

    private StateChangeEvent GetStateStartEvent(CharacterState state){
        if(playerMediator == null){
            return null;
        }
        switch (state){
            case CharacterState.Idle : return playerMediator.OnIdleStartEvent;
            case CharacterState.Walk : return playerMediator.OnWalkStartEvent;
            case CharacterState.Run : return playerMediator.OnRunStartEvent;
            case CharacterState.Dash : return playerMediator.OnDashStartEvent;
            case CharacterState.Jump : return playerMediator.OnJumpStartEvent;
            case CharacterState.NoramlAttack : 
            case CharacterState.FinishAttack : return playerMediator.OnAttackStartEvent;
            default: return null;
        }
    }

    private StateChangeEvent GetStateEndEvent(CharacterState state){
        if(playerMediator == null){
            return null;
        }
        switch (state){
            case CharacterState.Idle : return playerMediator.OnIdleEndEvent;
            case CharacterState.Walk : return playerMediator.OnWalkEndEvent;
            case CharacterState.Run : return playerMediator.OnRunEndEvent;
            case CharacterState.Dash : return playerMediator.OnDashEndEvent;
            case CharacterState.Jump : return playerMediator.OnJumpEndEvent;
            case CharacterState.NoramlAttack : 
            case CharacterState.FinishAttack : return playerMediator.OnAttackEndEvent;
            default: return null;
        }
    }

    public void SetDamage(int damage){
        HP -= damage;
        if(HP <= 0){
            Death();
        }
    }

    private void Death(){
        //GameManager.instance.ExpIncrease(deathExp);
        Destroy(this.gameObject, 1f);
    }

    bool IsDeath(){
        return HP <= 0;
    }

    public void LevelUp(){
        exp -= needExp;
        level++;

        GameObject gameObj = Instantiate(levelUpEffectFactory);
        gameObj.transform.position = (transform.position + new Vector3(0, -0.9f, 0));

        CheckExp();
        print("레벨업, 레벨 : " + Level);
    }

    public void ExpIncrease(int getExp){
        exp += getExp;
        CheckExp();
    }

    private void CheckExp(){
        print("필요 경험치 : "+needExp + ", 현재 경험치" + exp);
        if(needExp <= exp){
            LevelUp();
        }
    }

    public void RecoveryHp(int amount){
        print("회복 전 체력 : " + HP);
        
        HP = (MaxHP <= (hp+amount)) ? MaxHP : HP+amount;
        Instantiate(recoveryHpEffectFactory, transform.position, Quaternion.identity);
    
        print("회복 후 체력 : " + HP);
    }

    private void OnDestroy() {
  
    }











    // IEnumerator OnDamage()
    // {
    //     isDamage = true;
    //     meshes.material.color = Color.red;
        
    //     yield return new WaitForSeconds(0.5f);
    //     isDamage = false;

    //         meshes.material.color = playerColor;
    // }
    // private void OnTriggerEnter(Collider other)
    // {
    //     if(other.gameObject.tag == "MonsterAttack")
    //     {
    //         if (!isDamage)
    //         {
    //             MonsterAttack monsterAttack = other.gameObject.GetComponent<MonsterAttack>();
    //             hp -= monsterAttack.damage;
    //             StartCoroutine(OnDamage());
    //         }
    //     }
    // }

    
}
