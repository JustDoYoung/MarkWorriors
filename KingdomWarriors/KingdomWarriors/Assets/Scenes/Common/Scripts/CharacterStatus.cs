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
            //print("체력 세팅 : " + value);
            hp = value;
            if(hpSlider != null){hpSlider.value = value;}
            if(IsDeath()){
                // 임시
                if(playerMediator == null){
                  //  Death();
                    return;
                }
                playerMediator.Death();
            }
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

    public GameObject levelUpEffectFactory;

    private SswPlayerMediator playerMediator;

    private Slider hpSlider;
    private Slider mpSlider;

    // Start is called before the first frame update
    protected void Start()
    {
        playerMediator = GetComponent<SswPlayerMediator>();
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
    protected void Update()
    {
        CheckState();
        Billboard();
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
        stateChangeEvent += GetStateStartEvent(State);
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
        if(damage < 0){
            return;
        }
        HP -= damage;

        // if(HP <= 0){
        //     //Death();
        //     //죽음 애니메이션 처리 때문에 MonsterRe.cs에서 애니메이션 이벤트 함수로 우선 대신함.(3월 26일 - 김도영)
        // }
    }

    public void SetRecoveryHP(int amount){
        if(amount < 0){
            return;
        }

        int recoveredHP = HP+amount;
        HP = (MaxHP <= recoveredHP) ? MaxHP : recoveredHP;
    }

    // 임시
    private void Death(){
        //GameManager.instance.ExpIncrease(deathExp);
        Destroy(this.gameObject, 1f);
        print("죽음");
        
    }

    public bool IsDeath(){
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



    private void OnDestroy() {
  
    }

}
