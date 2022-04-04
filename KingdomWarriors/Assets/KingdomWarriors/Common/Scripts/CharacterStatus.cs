using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ModeState = CharacterState.Mode.State;
using MoveState = CharacterState.Move.State;
using ActionState = CharacterState.Action.State;

public interface CharacterMediator{
    // 상태 확인, 체크
    public bool IsCheckState(ModeState state);
    public bool IsCheckState(MoveState state);
    public bool IsCheckState(ActionState state);
    public bool IsAttacking();
    public ModeState GetModeState();
    public MoveState GetMoveState();
    public ActionState GetActionState();

    // 어빌리티
    public void ExpIncrease(int amount);
    public void LevelUp();

    // 무브
    public void UpdateMove(Vector3 dir, Vector3 velocity);
    public void Stay();
    public void Walk();
    public void Run();
    public void Dash(Vector3 dir, float dashHoldingTime);

    // 액션
    public void Idle();
    public void Jump();
    public void Landing();
    public void NormalAttack();
    public void HeavyAttack();
    public void EquipmentChange(WeaponStatus item);
    public void Death();

    // 상호작용
    public bool PickUpItem(GameItem item);
}

public class CharacterStatus : MonoBehaviour
{
    // 임시
    public int deathDropExp = 6;

    // 레벨
    protected delegate void LevelUpFunc(int level);
    
    private int levelUpNeedExp = 5;
    private int level = 1;
    public int Level{
        get{return level;}
        private set{
            if(IsPossibleLevelUp() == false){
                return;
            }
            exp -= levelUpNeedExp;
            level++;
            Mediator.LevelUp();
            SetAbilityByLevel(Level);

            CheckExpToLevelUp();
            print("레벨업, 레벨 : " + Level);

            
            // GameObject gameObj = Instantiate(levelUpEffectFactory);
            // gameObj.transform.position = (transform.position + new Vector3(0, -0.9f, 0));
        }
    }

    // 경험치
    private int exp;
    public int Exp{
        get{return exp;}
        private set{
            exp = value;
            CheckExpToLevelUp();
        }
    }

    // 체력
    [SerializeField]
    private int maxHP = 10;
    public int MaxHP{
        get{return maxHP;}
        protected set{
            if(value < 0){
                return;
            }
            maxHP = value;
        }
    }

    [SerializeField]
    private int hp;
    public int HP{
        get{return hp;}
        protected set{
            print("체력 세팅 : " + value);
            hp = value;
            if(hpSlider != null){hpSlider.value = value;}
            if(IsDeath()){
                GameManager.instance.ExpIncrease(deathDropExp);
                if(Mediator != null){
                    Mediator.Death();
                };
            }
        }
    }
    
    // 마나
    [SerializeField]
    private int maxMP = 10;
    public int MaxMP{
        get{return maxMP;}
        protected set{
            if(value < 0){
                return;
            }
            maxMP = value;
        }
    }

    [SerializeField]
    private int mp;
    public int MP{
        get{return mp;}
        protected set{
            if(value < 0){
                return;
            }
            mp = value;
            if(mpSlider != null){mpSlider.value = value;}
        }
    }

    // 공격력
    [SerializeField]
    private int offensePower = 1;
    public int OffensePower {
        get {return offensePower;}
        protected set {
            if(value < 0){
                return;
            }
            offensePower = value;
        }
    }
    
    // 방어력
    [SerializeField]
    private int defensePower = 1;
    public int DefensePower {
        get {return defensePower;}
        protected set {
            if(value < 0){
                return;
            }defensePower = value;
        }
    }

    // 상태
    public CharacterModeState ModeState {get;set;} = new CharacterModeState();
    public CharacterMoveState MoveState {get;set;} = new CharacterMoveState();
    public CharacterActionState ActionState {get;set;} = new CharacterActionState();

    private CharacterMediator Mediator {get; set;}

    private Slider hpSlider;
    private Slider mpSlider;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Mediator = GetComponent<CharacterMediator>();
        if(Mediator is CharacterState.Mode.Receiver){
            ModeState.AddReceiver((CharacterState.Mode.Receiver)Mediator);
        }
        if(Mediator is CharacterState.Move.Receiver){
            MoveState.AddReceiver((CharacterState.Move.Receiver)Mediator);
        }
        if(Mediator is CharacterState.Action.Receiver){
            ActionState.AddReceiver((CharacterState.Action.Receiver)Mediator);
        }

        Slider[] sliders = GetComponentsInChildren<Slider>();
        if(sliders == null){
            return;
        }
        foreach(Slider slider in sliders){
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
    protected virtual void Update()
    {
        Billboard();
    }

    private void Billboard(){
        if(hpSlider != null){hpSlider.transform.rotation = Camera.main.transform.rotation;}
        if(mpSlider != null){mpSlider.transform.rotation = Camera.main.transform.rotation;}
    }

    public bool IsCheckState(CharacterState.Mode.State state){
        return ModeState.IsCheckState(state);
    }

    public bool IsCheckState(CharacterState.Move.State state){
        return MoveState.IsCheckState(state);
    }

    public bool IsCheckState(CharacterState.Action.State state){
        return ActionState.IsCheckState(state);
    }

    public void SetState(CharacterState.Mode.State state){
        ModeState.SetState(state);
    }

    public void SetState(CharacterState.Move.State state){
        MoveState.SetState(state);
    }

    public void SetState(CharacterState.Action.State state){
        ActionState.SetState(state);
    }

    public void SetDamage(int damage){
        if(damage > 0) HP -= damage;
    }

    public void SetRecoveryHP(int amount){
        if(amount < 0){
            return;
        }

        int recoveredHP = HP+amount;
        HP = (MaxHP <= recoveredHP) ? MaxHP : recoveredHP;
    }

    public void ExpIncrease(int amount){
        Logger.Log("경험치", amount);
        if(amount > 0) Exp += amount;
    }

    public void ExpDrop(int amount){
        if(amount > 0) Exp -= amount;
    }

    private void CheckExpToLevelUp(){
        if(IsPossibleLevelUp()){
            Level++;
        }
    }

    private bool IsPossibleLevelUp(){
        return levelUpNeedExp <= exp;
    }

    private void SetAbilityByLevel(int level){
        SetHpByLevel(level);
        SetMpByLevel(level);
        SetOffensePowerByLevel(level);
        SetDefensePowerByLevel(level);
    }

    protected virtual void SetHpByLevel(int level){}
    protected virtual void SetMpByLevel(int level){}
    protected virtual void SetOffensePowerByLevel(int level){}
    protected virtual void SetDefensePowerByLevel(int level){}

    public bool IsDeath(){
        return HP <= 0;
    }

    // 임시
    private void Death(){
        //GameManager.instance.ExpIncrease(deathExp);
        Destroy(this.gameObject, 1f);
        print("죽음");
        
    }
    
    private void OnDestroy() {
  
    }
}
