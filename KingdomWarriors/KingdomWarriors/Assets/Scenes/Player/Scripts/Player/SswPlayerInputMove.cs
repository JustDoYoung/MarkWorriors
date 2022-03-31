using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoveState = CharacterState.Move.State;
using ActionState = CharacterState.Action.State;

public class SswPlayerInputMove : MonoBehaviour
{
    // 환경
    private Transform mainCameraTf;
    public float gravity = -9.81f;
    private float yVelocity;
    private const int dateTimeBinaryOffset = 10000000;

    // 이동 속도
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    
    // 점프
    public float jumpPower = 10f;
    public int maxJumpCount = 2;

    private int currentJumpCount = 0;

    // 대쉬
    public float dashSpeed = 30f;
    public float dashCheckTerm = 0.2f;
    public float dashHoldingTime = 0.3f;
    
    private bool isDashChecking = false;
    private KeyCode pressedDashKey = KeyCode.None;
    private readonly Dictionary<KeyCode, Vector3> dashDirByKeyCode = new Dictionary<KeyCode, Vector3> {
        {KeyCode.W, Vector3.forward},
        {KeyCode.A, Vector3.left}, {KeyCode.S, Vector3.back}, {KeyCode.D, Vector3.right},
    };

    // 중재자 객체
    private CharacterMediator PlayerMediator {get; set;}
    
    // 캐릭터 컨트롤러
    private CharacterController PlayerCtrl {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        this.PlayerCtrl = GetComponent<CharacterController>();
        this.PlayerMediator = GetComponent<CharacterMediator>();
        this.mainCameraTf = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Gravity();
        JumpCheck();     
        DashCheck();
        Move(GetInputAxis());
    }

    public Vector3 GetGravityVelocity(){
        return new Vector3(0, yVelocity, 0);
    }

    private void Gravity(){
        if(PlayerCtrl.isGrounded == false){
            yVelocity += gravity * Time.deltaTime;
            return;
        }
        yVelocity = 0;

        if(PlayerMediator.IsCheckState(ActionState.Jump)){
            PlayerMediator.Landing();
        }
    }

    // private bool IsGrounded(){
    //     return currentJumpCount == 0 && Mathf.Abs(yVelocity) < 0.2f || PlayerCtrl.isGrounded;
    // }

    private void JumpCheck(){
        if(Input.GetButtonDown("Jump") == false || currentJumpCount >= maxJumpCount){
            return;
        }
        PlayerMediator.Jump();
    }

    private void DashCheck(){
        if(PlayerMediator.IsCheckState(ActionState.Jump)){
            return;
        }

        KeyCode dashKey = GetPressedDashKey();
        if(dashKey == KeyCode.None){
            return;
        }

        StopCoroutine(CRDashCheck(dashKey));
        StartCoroutine(CRDashCheck(dashKey));
    }

    private IEnumerator CRDashCheck(KeyCode currnetPressedKey){
        if(isDashChecking == true && pressedDashKey == currnetPressedKey){
            Vector3 dashDir = Vector3.zero;
            dashDirByKeyCode.TryGetValue(currnetPressedKey, out dashDir);
            PlayerMediator.Dash(dashDir, dashHoldingTime);
            yield break;
        }

        isDashChecking = true;
        pressedDashKey = currnetPressedKey;
        yield return new WaitForSeconds(dashCheckTerm);
        isDashChecking = false;
        pressedDashKey = KeyCode.None;
    }

    private KeyCode GetPressedDashKey(){
        foreach (KeyCode keyCode in dashDirByKeyCode.Keys){
            if(Input.GetKeyDown(keyCode)){
                return keyCode;
            }
        }
        return KeyCode.None;
    }

    private void Move(Vector3 dir){
        dir.Normalize();
        MoveState characterState = PlayerMediator.GetMoveState();
        CheckMoveState(characterState, GetMotionStateFromInput(dir));

        Vector3 velocity = GetVelocityByMoveState(dir, characterState);
        PlayerCtrl.Move(velocity);
        PlayerMediator.UpdateMove(dir, velocity);
    }
 
    private Vector3 GetInputAxis(){
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        return new Vector3(h, 0, v);
    }

    private void CheckMoveState(MoveState characterState, MoveState inputState){
        if( characterState == inputState || 
            characterState == MoveState.Dash ||
            PlayerMediator.IsCheckState(ActionState.Jump))
        {
            return;
        }

        switch(inputState){
            case MoveState.Run : PlayerMediator.Run(); break;
            case MoveState.Walk : PlayerMediator.Walk(); break;
            case MoveState.Stay : PlayerMediator.Stay(); break;
        }
    }

    private MoveState GetMotionStateFromInput(Vector3 dir){
        if(dir == Vector3.zero){
            return MoveState.Stay;
        }
        return Input.GetKey(KeyCode.LeftShift) ? MoveState.Walk : MoveState.Run;
    }

    private Vector3 GetVelocityByMoveState(Vector3 dir, MoveState characterState){
        float speed = 0f;
        switch(characterState){
            case MoveState.Run : speed = runSpeed; break;
            case MoveState.Walk : speed = walkSpeed; break;
            case MoveState.Dash : speed = dashSpeed; break;
        }

        Vector3 velocity = mainCameraTf.TransformDirection(dir * speed);
        velocity.y = yVelocity;
        return velocity * Time.deltaTime;
    }

    private void LandingCheck(Collider other){
        if(other.CompareTag("Floor") != false){
            return;
        }
        PlayerMediator.Landing();
    }

    public void SetStay(){

    }

    public void SetRun(){

    }

    public void SetWalk(){

    }

    public void SetJump(){
        yVelocity = jumpPower;
        currentJumpCount++;
    }

    public void SetLanding(){
        yVelocity = 0;
        currentJumpCount = 0;
    }

    public void SetDash(){

    }
}