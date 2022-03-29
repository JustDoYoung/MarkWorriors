using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CharacterState = SswCharacterStatus.CharacterState;

public class SswPlayerControllerMove : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpPower = 10f;
    public float gravity = -9.81f;

    public float dashSpeed = 30f;
    public float turnSpeed = 5f;

    public int maxJumpCount = 2;

    public float dashBatterTerm = 0.2f;

    public float dashTime = 0.3f;

    private float currentDashTime = 0f;

    private const int dateTimeBinaryOffset = 10000000;

    private readonly Dictionary<KeyCode, Vector3> DashVectorByKeyCode = new Dictionary<KeyCode, Vector3> {
        {KeyCode.W, Vector3.forward},
        {KeyCode.A, Vector3.left}, {KeyCode.S, Vector3.back}, {KeyCode.D, Vector3.right},
    };

    private KeyCode prevPressedMoveKey = KeyCode.None;

    private long prevPressedKeyTime = System.DateTime.Now.ToBinary();

    private Vector3 dashVector = Vector3.zero;

    private int currentJumpCount = 0;

    private float yVelocity;

    private Transform mainCameraTf;

    private delegate Vector3 GetMoveVector3(Vector3 vector3);
    private GetMoveVector3 GetMoveVector3Func {get; set;}

    private CharacterController playerCharacterCtrl;
    private SswPlayerMediator playerMediator;
  
    // Start is called before the first frame update
    void Start()
    {
        this.playerCharacterCtrl = GetComponent<CharacterController>();
        this.mainCameraTf = Camera.main.transform;
        this.playerMediator = GetComponent<SswPlayerMediator>();
    }

    // Update is called once per frame
    void Update()
    {
        Gravity();
        KeyMove();
//        Logger.Log("state2", ""+ yVelocity);
//      print(currentJumpCount == 0 && Mathf.Abs(yVelocity) < 0.2f);
    }

    void Gravity(){
        bool isGrounded = playerCharacterCtrl.isGrounded;
        yVelocity = isGrounded ? 0 : yVelocity + (gravity * Time.deltaTime);
        
        if(isGrounded && currentJumpCount != 0){
            playerMediator.SetState(CharacterState.Idle);
            ResetForIdle();
            Logger.Log("테스트2", yVelocity+"");
        }

    }

    public void ResetForIdle(){
        Logger.Log("테스트3", yVelocity+"");
        yVelocity = 0;
        currentJumpCount = 0;
        currentDashTime = 0f;
    }

    public void SetMoveVector3GetterType(CharacterState state){
        SetMoveVector3GetterType(GetMoveTypeFuncByState(state));
    }

    private void SetMoveVector3GetterType(GetMoveVector3 getMoveVector3Func){
        GetMoveVector3Func = getMoveVector3Func;
    }

    private GetMoveVector3 GetMoveTypeFuncByState(CharacterState state){
        switch(state){
            case CharacterState.Idle :
            case CharacterState.Run : return GetRunVector3;
            case CharacterState.Walk : return GetWalkVector3;
            case CharacterState.Jump : return GetJumpVector3;
            case CharacterState.Dash : return GetDashVector3;
            default : return null;
        }
    }

    void KeyMove(){ 
        JumpCheck();       
        DashCheck();

        Vector3 input = GetInputAxis();

        if(GetMoveVector3Func == null){
            GetMoveVector3Func = GetRunVector3;
        }

        Vector3 velocity = GetMoveVector3Func(mainCameraTf.TransformDirection(input));
        if(velocity == null){
            velocity = Vector3.zero;
        }
        
        playerMediator.Move(input);
        playerCharacterCtrl.Move(velocity * Time.deltaTime);
    }

    void JumpCheck(){
        if(Input.GetButtonDown("Jump") == false || currentJumpCount >= maxJumpCount){
            return;
        }

        playerMediator.Jump();
        yVelocity = jumpPower;
        currentJumpCount++;
    }

    void DashCheck(){
        KeyCode pressedKey = GetPressedDashKey();
        if( pressedKey == KeyCode.None ||
            IsPressedDash(pressedKey) == false ||
            currentJumpCount != 0)
        {
            DashEndCheck();
            return;
        }

        playerMediator.Dash(pressedKey);
    }

    void DashEndCheck(){
        if(playerMediator.IsCheckState(CharacterState.Dash) == false){
            return;
        }
        currentDashTime += Time.deltaTime;
        if(currentDashTime >= dashTime){
            playerMediator.SetState(CharacterState.Idle);
        }
    }
    
    KeyCode GetPressedDashKey(){
        foreach (KeyCode keyCode in DashVectorByKeyCode.Keys){
            if(Input.GetKeyDown(keyCode)){
                return keyCode;
            }
        }
        return KeyCode.None;
    }

    bool IsPressedDash(KeyCode pressedKeyCode){
        long currentTime = System.DateTime.Now.ToBinary();

        if( (prevPressedMoveKey == pressedKeyCode) && 
            (currentTime - prevPressedKeyTime) < (dashBatterTerm * dateTimeBinaryOffset))
        {
            prevPressedMoveKey = KeyCode.None;
            return true;
        }

        prevPressedKeyTime = currentTime;
        prevPressedMoveKey = pressedKeyCode;
        return false;
    }

    private Vector3 GetInputAxis(){
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        return new Vector3(h, 0, v);
    }

    private Vector3 GetWalkVector3(Vector3 dir) {
        Logger.Log("테스트", "걷기");

        dir.y = yVelocity;
        dir.Normalize();
        return dir * walkSpeed;
    }

    private Vector3 GetRunVector3(Vector3 dir) {
//        Logger.Log("테스트", "달리기, " + dir);
//        Logger.Log("테스트", ""+yVelocity);
        

        dir.y = yVelocity;
        dir.Normalize();
        return dir * runSpeed;
    }

    private Vector3 GetJumpVector3(Vector3 velocity) {
        Logger.Log("테스트", "점프");

        velocity.y = yVelocity;
        return velocity;
    }

    private Vector3 GetDashVector3(Vector3 velocity) {
        Logger.Log("테스트", "대시");

        velocity = dashVector * dashSpeed;
        velocity = mainCameraTf.TransformDirection(velocity);
        velocity.y = 0;
        return velocity;
    }
}