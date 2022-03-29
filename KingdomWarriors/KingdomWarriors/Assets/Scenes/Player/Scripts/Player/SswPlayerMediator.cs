using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharacterState = SswCharacterStatus.CharacterState;

public class Logger{
    public enum Tag{
        State
    }

    public static void Log(Tag tag, string msg){
        Debug.Log(tag.ToString() + msg);
    }
    public static void Log(string tag, string msg){
        Debug.Log("[" + tag + "], " + msg);
    }
}

public class SswPlayerMediator : MonoBehaviour, CharacterMediator, OnAttackEvent
{
    private CharacterStatus playerStatus;

    private SswPlayerCameraMove playerCameraMove;
    private SswPlayerControllerMove playerCtrlMove;
    private SswPlayerShortcutKeys playerShortcutKeys;
    private SswPlayerEffect playerEffect;
    private SswPlayerAnim playerAnim;

    private WeaponWearableHand rightWeaponHand;
    
    private void Start() {
        playerStatus = GetComponent<CharacterStatus>(); 
        playerCameraMove = GetComponent<SswPlayerCameraMove>();
        playerCtrlMove = GetComponent<SswPlayerControllerMove>();
        playerEffect = GetComponent<SswPlayerEffect>();
        playerAnim = GetComponent<SswPlayerAnim>();

        rightWeaponHand = GetComponentInChildren<WeaponWearableHand>();
    }

    public bool IsCheckState(CharacterState state){
        return playerStatus.State == state;
    }

    public void SetState(CharacterState state){
        playerStatus.State = state;
    }

    public void Move(Vector3 velocity){
        if(IsCheckState(CharacterState.Jump) ||
            IsCheckState(CharacterState.NoramlAttack) ||
            IsCheckState(CharacterState.FinishAttack))
        {
            return;
        }

        if(velocity == Vector3.zero){
            SetState(CharacterState.Idle);
            return;
        }

        playerStatus.State = Input.GetKey(KeyCode.LeftShift) ? CharacterState.Walk : CharacterState.Run;

        switch(playerStatus.State){
            case CharacterState.Walk : Walk(velocity); break;
            case CharacterState.Run : Run(velocity); break;
        }
    }

    public void Walk(Vector3 velocity){
        playerAnim.SetWalk(velocity);
    }

    public void Run(Vector3 velocity){
        playerAnim.SetRun(velocity);
    }

    public void Jump(){
        SetState(CharacterState.Jump);
        playerCtrlMove.SetMoveVector3GetterType(CharacterState.Jump);
    }

    public void Dash(KeyCode pressedkey){

    }

    public void RecoveryHp(int amount){
        Logger.Log(Logger.Tag.State, "회복 전 체력 : " + playerStatus.HP);
        
        int recoveryHp = (playerStatus.MaxHP <= (playerStatus.HP+amount)) ? playerStatus.MaxHP : playerStatus.HP+amount;

        playerStatus.SetRecoveryHP(amount);
        playerEffect.RecoveryHpEffect();
    
        Logger.Log(Logger.Tag.State, "회복 후 체력 : " + playerStatus.HP);
    }

    public void Death(){

    }

    /*****************************
     * 상태 시작 이벤트
     */

    public void OnIdleStartEvent(){
//        Logger.Log(Logger.Tag.State, "idle start");
        playerAnim.SetIdle();
        
    }

    public void OnWalkStartEvent()
    {
        Logger.Log(Logger.Tag.State, "walk start");
        playerAnim.SetWalk(true);
        playerCtrlMove.SetMoveVector3GetterType(CharacterState.Walk);
    }

    public void OnRunStartEvent()
    {
//        Logger.Log(Logger.Tag.State, "run start");
        playerAnim.SetRun(true);
        playerCtrlMove.SetMoveVector3GetterType(CharacterState.Run);
    }

    public void OnDashStartEvent()
    {
        Logger.Log(Logger.Tag.State, "dash start");
    }

    public void OnJumpStartEvent()
    {
        Logger.Log(Logger.Tag.State, "jump start");
        playerAnim.Jump();
    }

    public void OnAttackStartEvent()
    {
//        Logger.Log(Logger.Tag.State, "attack start");
        rightWeaponHand.Attack(true);
    }



    /*****************************
     * 상태 종료 이벤트
     */

    public void OnIdleEndEvent(){
//        Logger.Log(Logger.Tag.State, "idle end");
    }

    public void OnWalkEndEvent()
    {
        Logger.Log(Logger.Tag.State, "walk end");
        playerAnim.SetWalk(false);
    }

    public void OnRunEndEvent()
    {
//        Logger.Log(Logger.Tag.State, "run end");
        playerAnim.SetRun(false);
    }

    public void OnDashEndEvent()
    {
        Logger.Log(Logger.Tag.State, "dash end");
    }

    public void OnJumpEndEvent()
    {
        Logger.Log(Logger.Tag.State, "jump end");
    }

    public void OnAttackEndEvent()
    {
//        Logger.Log(Logger.Tag.State, "attack end");
        rightWeaponHand.Attack(false);
        playerStatus.isNoramlAttackLock = false;
    }

    public void OnNormalAttack(){
        if(playerStatus.isNoramlAttackLock){
            return;
        }
        if(IsCheckState(CharacterState.FinishAttack)){
            return;
        }
        if(IsCheckState(CharacterState.NoramlAttack)){
            playerAnim.TempComboAttack();
            playerStatus.isNoramlAttackLock = true;
            rightWeaponHand.Attack(true);
            return;
        }

        SetState(CharacterState.NoramlAttack);
        playerAnim.NoramlAttack();
        playerStatus.isNoramlAttackLock = true;
    }

    public void OnHeavyAttack(){
        if(IsCheckState(CharacterState.FinishAttack)){
            return;
        }
        SetState(CharacterState.FinishAttack);
        playerAnim.HeavyAttack();
        playerStatus.isNoramlAttackLock = true;
    }

    public void OnAttack1MotionFinishedEvent(){
        playerStatus.isNoramlAttackLock = false;
        rightWeaponHand.Attack(false);
//        Logger.Log(Logger.Tag.State, "attack motion");
    }

    // 임시
    public void OnAttack1FinishedEvent(){
        SetState(CharacterState.Idle);
    }
    public void OnAttack2FinishedEvent(){
        SetState(CharacterState.Idle);
    }
    public void OnAttack3FinishedEvent(){
        SetState(CharacterState.Idle);
    }

    public void OnAttack(GameObject target, WeaponStatus weaponStatus)
    {
        CharacterStatus targetStatus = target.GetComponent<CharacterStatus>();
        if(targetStatus == null){
            return;
        }
        targetStatus.SetDamage(weaponStatus.power);

//        Logger.Log("ttt2", targetStatus.ToString());

        SkeletonAttackPattern skeleton = target.GetComponent<SkeletonAttackPattern>();
        DemonAttackPattern demon = target.GetComponent<DemonAttackPattern>();
        UndeadKnightAttackPattern undeadKnight = target.GetComponent<UndeadKnightAttackPattern>();

        if(skeleton != null) skeleton.GetDamageFromPlayer();
        if(demon != null) demon.GetDamageFromPlayer();
        if(undeadKnight != null) undeadKnight.GetDamageFromPlayer();
    }
}
