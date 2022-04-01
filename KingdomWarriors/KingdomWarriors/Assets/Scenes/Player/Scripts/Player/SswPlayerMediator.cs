using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoveState = CharacterState.Move.State;
using ActionState = CharacterState.Action.State;

public class Logger
{
    public enum Tag
    {
        State
    }

    public static void Log(Tag tag, string msg)
    {
        Logger.Log(tag.ToString(), msg);
    }
    public static void Log(string tag, string msg)
    {
        Debug.Log("[" + tag + "], " + msg);
    }
}

public class SswPlayerMediator : MonoBehaviour, CharacterMediator, CharacterState.Move.Receiver, CharacterState.Action.Receiver, OnAttackEvent
{
    private CharacterStatus PlayerStatus { get; set; }

    private SswPlayerCameraMove PlayerCameraMove { get; set; }
    private SswPlayerInputMove PlayerInputMove { get; set; }
    private SswPlayerShortcutKeys PlayerShortcutKeys { get; set; }
    private SswPlayerEffect PlayerEffect { get; set; }
    private SswPlayerAnim PlayerAnim { get; set; }

    private WeaponWearableHand rightWeaponHand;

    private void Start()
    {
        PlayerStatus = GetComponent<CharacterStatus>();
        PlayerCameraMove = GetComponent<SswPlayerCameraMove>();
        PlayerInputMove = GetComponent<SswPlayerInputMove>();
        PlayerEffect = GetComponent<SswPlayerEffect>();
        PlayerAnim = GetComponent<SswPlayerAnim>();

        rightWeaponHand = GetComponentInChildren<WeaponWearableHand>();
    }

    private void SetState(MoveState state)
    {
        PlayerStatus.SetState(state);
    }
    private void SetState(ActionState state)
    {
        PlayerStatus.SetState(state);
    }

    public bool IsCheckState(MoveState state)
    {
        return PlayerStatus.IsCheckState(state);
    }
    public bool IsCheckState(ActionState state)
    {
        return PlayerStatus.IsCheckState(state);
    }

    public bool IsAttacking()
    {
        return IsCheckState(ActionState.NoramlAttack) ||
                IsCheckState(ActionState.HeavyAttack) ||
                IsCheckState(ActionState.FinishAttack);
    }

    public MoveState GetMoveState()
    {
        return PlayerStatus.MoveState.CurrentState;
    }

    public ActionState GetActionState()
    {
        return PlayerStatus.ActionState.CurrentState;
    }

    public void UpdateMove(Vector3 dir, Vector3 velocity)
    {
        switch (GetMoveState())
        {
            case MoveState.Run: PlayerAnim.SetRun(dir); break;
            case MoveState.Walk: PlayerAnim.SetWalk(dir); break;
        }
    }

    public void Idle()
    {
        SetState(ActionState.Idle);
    }

    public void Stay()
    {
        SetState(MoveState.Stay);
        StartCoroutine(CRStayAnim());
    }

    private IEnumerator CRStayAnim()
    {
        yield return null;
        if (IsAttacking())
        {
            yield break;
        }
        if (IsCheckState(MoveState.Stay))
        {
            PlayerAnim.SetStay();
        }
    }

    public void Walk()
    {
        if (IsAttacking())
        {
            return;
        }

        SetState(MoveState.Walk);
        PlayerInputMove.SetWalk();

        if (IsCheckState(ActionState.Jump))
        {
            return;
        }
        PlayerAnim.SetWalk(true);
    }

    public void Run()
    {
        if (IsAttacking())
        {
            return;
        }

        SetState(MoveState.Run);
        PlayerInputMove.SetRun();

        if (IsCheckState(ActionState.Jump))
        {
            return;
        }
        PlayerAnim.SetRun(true);
    }

    public void Jump()
    {
        SetState(ActionState.Jump);
        PlayerAnim.Jump();
        PlayerInputMove.SetJump();
    }

    public void Landing()
    {
        SetState(ActionState.Landing);
        PlayerInputMove.SetLanding();
        PlayerAnim.Landing();
    }

    public void Dash(Vector3 dir, float dashHoldingTime)
    {
        if (IsAttacking())
        {
            return;
        }

        SetState(MoveState.Dash);
        StartCoroutine(CRDashEnd(dashHoldingTime));
    }

    private IEnumerator CRDashEnd(float dashHoldingTime)
    {
        yield return new WaitForSeconds(dashHoldingTime);
        Stay();
    }

    public void RecoveryHp(int amount)
    {
        Logger.Log(Logger.Tag.State, "회복 전 체력 : " + PlayerStatus.HP);

        int recoveryHp = (PlayerStatus.MaxHP <= (PlayerStatus.HP + amount)) ? PlayerStatus.MaxHP : PlayerStatus.HP + amount;

        PlayerStatus.SetRecoveryHP(amount);
        PlayerEffect.RecoveryHpEffect();

        Logger.Log(Logger.Tag.State, "회복 후 체력 : " + PlayerStatus.HP);
    }

    public void Death()
    {

    }

    /*****************************
     * 상태 시작 이벤트
     */

    public void OnStayStartEvent()
    {
        Logger.Log(Logger.Tag.State, "stay start");
    }

    public void OnIdleStartEvent()
    {
        Logger.Log(Logger.Tag.State, "idle start");
    }

    public void OnWalkStartEvent()
    {
        Logger.Log(Logger.Tag.State, "walk start");
    }

    public void OnRunStartEvent()
    {
        Logger.Log(Logger.Tag.State, "run start");
    }

    public void OnDashStartEvent()
    {
        Logger.Log(Logger.Tag.State, "dash start");
    }

    public void OnJumpStartEvent()
    {
        Logger.Log(Logger.Tag.State, "jump start");
    }

    public void OnLandingStartEvent()
    {
        Logger.Log(Logger.Tag.State, "landing start");
    }

    public void OnAttackStartEvent()
    {
        Logger.Log(Logger.Tag.State, "attack start");
        rightWeaponHand.Attack(true);
    }



    /*****************************
     * 상태 종료 이벤트
     */

    public void OnStayEndEvent()
    {
        Logger.Log(Logger.Tag.State, "stay end");
    }

    public void OnIdleEndEvent()
    {
        Logger.Log(Logger.Tag.State, "idle end");
    }

    public void OnWalkEndEvent()
    {
        Logger.Log(Logger.Tag.State, "walk end");
        PlayerAnim.SetWalk(false);
    }

    public void OnRunEndEvent()
    {
        Logger.Log(Logger.Tag.State, "run end");
        PlayerAnim.SetRun(false);
    }

    public void OnDashEndEvent()
    {
        Logger.Log(Logger.Tag.State, "dash end");
    }

    public void OnJumpEndEvent()
    {
        Logger.Log(Logger.Tag.State, "jump end");
    }

    public void OnLandingEndEvent()
    {
        Logger.Log(Logger.Tag.State, "landing end");
    }

    public void OnAttackEndEvent()
    {
        Logger.Log(Logger.Tag.State, "attack end");
        rightWeaponHand.Attack(false);
    }

    public void OnNormalAttack()
    {
        if (PlayerStatus.isNoramlAttackLock)
        {
            return;
        }
        if (IsCheckState(ActionState.FinishAttack))
        {
            return;
        }
        if (IsCheckState(ActionState.NoramlAttack))
        {
            PlayerAnim.TempComboAttack();
            PlayerStatus.isNoramlAttackLock = true;
            rightWeaponHand.Attack(true);
            return;
        }

        SetState(ActionState.NoramlAttack);
        PlayerAnim.NoramlAttack();
        PlayerStatus.isNoramlAttackLock = true;
    }


    private IEnumerator CRNormalAttack()
    {
        yield return null;



    }

    public void OnHeavyAttack()
    {
        if (IsCheckState(ActionState.FinishAttack))
        {
            return;
        }
        SetState(ActionState.FinishAttack);
        PlayerAnim.HeavyAttack();
        PlayerStatus.isNoramlAttackLock = true;
    }

    public void OnAttack1MotionFinishedEvent()
    {
        if (PlayerStatus.IsCheckState(ActionState.NoramlAttack) == false)
        {
            return;
        }

        PlayerStatus.isNoramlAttackLock = false;
        rightWeaponHand.Attack(false);
        Logger.Log(Logger.Tag.State, "attack motion");
    }

    // 임시
    public void OnAttack1FinishedEvent()
    {
        if (PlayerStatus.IsCheckState(ActionState.NoramlAttack) == false)
        {
            return;
        }
        print("111111111111");
        Idle();
    }
    public void OnAttack2FinishedEvent()
    {
        PlayerStatus.isNoramlAttackLock = false;
        print("222222222222");
        Idle();
    }
    public void OnAttack3FinishedEvent()
    {
        if (PlayerStatus.IsCheckState(ActionState.FinishAttack) == false)
        {
            return;
        }

        PlayerStatus.isNoramlAttackLock = false;
        print("333333333333");
        Idle();
    }

    public void OnAttack(GameObject target, WeaponStatus weaponStatus)
    {
        Interaction.Attack targetAttack = target.GetComponent<Interaction.Attack>();
        if (targetAttack == null)
        {
            return;
        }
        targetAttack.OnAttack(weaponStatus.power);
    }







    public void ExpIncrease()
    {
        throw new NotImplementedException();
    }

    public void LevelUp()
    {
        throw new NotImplementedException();
    }




    public void NormalAttack()
    {
        throw new NotImplementedException();
    }

    public void HeavyAttack()
    {
        throw new NotImplementedException();
    }

    public void EquipmentChange()
    {
        throw new NotImplementedException();
    }

    public void GetItem()
    {
        throw new NotImplementedException();
    }


}
