using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MoveState = CharacterState.Move.State;
using ActionState = CharacterState.Action.State;

public interface CharacterMediator{
    // 상태 확인, 체크
    public bool IsCheckState(MoveState state);
    public bool IsCheckState(ActionState state);
    public bool IsAttacking();
    public MoveState GetMoveState();
    public ActionState GetActionState();

    // 어빌리티
    public void ExpIncrease();
    public void LevelUp();

    // 무브
    public void Stay();
    public void Walk();
    public void Run();
    public void Dash(Vector3 dir, float dashHoldingTime);
    public void UpdateMove(Vector3 dir, Vector3 velocity);

    // 액션
    public void Idle();
    public void Jump();
    public void Landing();
    public void NormalAttack();
    public void HeavyAttack();
    public void EquipmentChange();
    public void Death();

    // 상호작용
    public void GetItem();
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
            print("체력 세팅 : " + value);
            hp = value;
            if(hpSlider != null){hpSlider.value = value;}
            if(IsDeath()){
                // 임시
                if(CharacterMediator == null){
                  //  Death();
                    return;
                }
                CharacterMediator.Death();
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

    public CharacterMoveState MoveState {get;set;} = new CharacterMoveState();
    public CharacterActionState ActionState {get;set;} = new CharacterActionState();

    public GameObject levelUpEffectFactory;

    private CharacterMediator CharacterMediator {get; set;}

    private Slider hpSlider;
    private Slider mpSlider;

    // 임시로 테스트
    public bool isNoramlAttackLock = false;

    // Start is called before the first frame update
    protected void Start()
    {
        CharacterMediator = GetComponent<CharacterMediator>();
        if(CharacterMediator is CharacterState.Move.Receiver){
            MoveState.AddReceiver((CharacterState.Move.Receiver)CharacterMediator);
        }
        if(CharacterMediator is CharacterState.Move.Receiver){
            ActionState.AddReceiver((CharacterState.Action.Receiver)CharacterMediator);
        }

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
        Billboard();
    }

    private void Billboard(){
        if(hpSlider != null){hpSlider.transform.rotation = Camera.main.transform.rotation;}
        if(mpSlider != null){mpSlider.transform.rotation = Camera.main.transform.rotation;}
    }

    public bool IsCheckState(CharacterState.Move.State state){
        return MoveState.IsCheckState(state);
    }

    public bool IsCheckState(CharacterState.Action.State state){
        return ActionState.IsCheckState(state);
    }

    public void SetState(CharacterState.Move.State state){
        MoveState.SetState(state);
    }

    public void SetState(CharacterState.Action.State state){
        ActionState.SetState(state);
    }

    public void SetDamage(int damage){
        if(damage < 0){
            return;
        }
        HP -= damage;
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
