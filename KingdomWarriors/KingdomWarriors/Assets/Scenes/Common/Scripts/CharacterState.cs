using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoveState = CharacterState.Move.State;
using ActionState = CharacterState.Action.State;

using MoveReceiver = CharacterState.Move.Receiver;
using ActionReceiver = CharacterState.Action.Receiver;

public interface CharacterState{
    public interface Move{
        public enum State{
            Stay, Walk, Run, Dash
        }
        public interface Receiver{
            public void OnStayStartEvent();
            public void OnWalkStartEvent();
            public void OnRunStartEvent();
            public void OnDashStartEvent();

            public void OnStayEndEvent();
            public void OnWalkEndEvent();
            public void OnRunEndEvent();
            public void OnDashEndEvent();
        }
    }
    
    public interface Action{
        public enum State{
            Idle, Jump, Landing, GetDamage, Death, NoramlAttack, HeavyAttack, FinishAttack
        }
        public interface Receiver{
            public void OnIdleStartEvent();
            public void OnJumpStartEvent();
            public void OnLandingStartEvent();
            public void OnAttackStartEvent();

            public void OnIdleEndEvent();
            public void OnJumpEndEvent();
            public void OnLandingEndEvent();
            public void OnAttackEndEvent();
        }
    }
}

// T  : 상태 enum 타입
// T2 : 리시버 이벤트 인터페이스 타입
public abstract class CharacterState<T, T2>{
    protected delegate void StateChangeEvent();

    private List<T2> eventReceiverList = new List<T2>();

   // private T prevState;
    public T CurrentState {get; private set;}

    public void AddReceiver(T2 receiver){
        this.eventReceiverList.Add(receiver);
    }

    public void RemoveReceiver(T2 receiver){
        this.eventReceiverList.Remove(receiver);
    }

    public void ClearReceiverList(){
        this.eventReceiverList.Clear();
    }

    public bool IsCheckState(T state){
        return state.Equals(CurrentState);
    }

    public void SetState(T state){
        if(state == null || eventReceiverList == null || CurrentState.Equals(state)){
            
            return;
        }
        StateChangeEvent stateChangeEvent = null;
        stateChangeEvent += GetStateEndEvents(CurrentState);
        stateChangeEvent += GetStateStartEvents(state);
        CurrentState = state;

        if(stateChangeEvent == null){
            return;
        }
        stateChangeEvent();
    }

    private StateChangeEvent GetStateEndEvents(T prevState){
        StateChangeEvent stateChangeEvent = null;
        foreach(T2 receiver in eventReceiverList){
            if(receiver == null){
                continue;
            }
            stateChangeEvent += GetStateEndEvent(prevState, receiver);
        }
        return stateChangeEvent;
    }

    private StateChangeEvent GetStateStartEvents(T currnetState){
        StateChangeEvent stateChangeEvent = null;
        foreach(T2 receiver in eventReceiverList){
            if(receiver == null){
                continue;
            }
            stateChangeEvent += GetStateStartEvent(currnetState, receiver);
        }
        return stateChangeEvent;
    }

    protected abstract StateChangeEvent GetStateStartEvent(T currentState, T2 receiver);
    protected abstract StateChangeEvent GetStateEndEvent(T prevState, T2 receiver);
}

public class CharacterMoveState : CharacterState<MoveState, MoveReceiver>
{
    protected override StateChangeEvent GetStateEndEvent(MoveState prevState, MoveReceiver receiver)
    {
        if(receiver == null){
            return null;
        }
        switch (prevState){
            case MoveState.Stay : return receiver.OnStayEndEvent;
            case MoveState.Walk : return receiver.OnWalkEndEvent;
            case MoveState.Run : return receiver.OnRunEndEvent;
            case MoveState.Dash : return receiver.OnDashEndEvent;
            default: return null;
        }
    }

    protected override StateChangeEvent GetStateStartEvent(MoveState currentState, MoveReceiver receiver)
    {
        if(receiver == null){
            return null;
        }
        switch (currentState){
            case MoveState.Stay : return receiver.OnStayStartEvent;
            case MoveState.Walk : return receiver.OnWalkStartEvent;
            case MoveState.Run : return receiver.OnRunStartEvent;
            case MoveState.Dash : return receiver.OnDashStartEvent;
            default: return null;
        }
    }
}

public class CharacterActionState : CharacterState<ActionState, ActionReceiver>
{
    protected override StateChangeEvent GetStateEndEvent(ActionState prevState, ActionReceiver receiver)
    {
        if(receiver == null){
            return null;
        }

        switch (prevState){
            case ActionState.Idle : return receiver.OnIdleEndEvent;
            case ActionState.Jump : return receiver.OnJumpEndEvent;
            case ActionState.Landing : return receiver.OnLandingEndEvent;
            case ActionState.NoramlAttack : 
            case ActionState.FinishAttack : return receiver.OnAttackEndEvent;
            default: return null;
        }
    }

    protected override StateChangeEvent GetStateStartEvent(ActionState currentState, ActionReceiver receiver)
    {
        if(receiver == null){
            return null;
        }
        switch (currentState){
            case ActionState.Idle : return receiver.OnIdleStartEvent;
            case ActionState.Jump : return receiver.OnJumpStartEvent;
            case ActionState.Landing : return receiver.OnLandingStartEvent;
            case ActionState.NoramlAttack : 
            case ActionState.FinishAttack : return receiver.OnAttackStartEvent;
            default: return null;
        }
    }
}