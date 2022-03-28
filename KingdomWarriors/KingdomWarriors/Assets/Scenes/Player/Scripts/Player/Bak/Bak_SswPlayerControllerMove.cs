using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bak_SswPlayerControllerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
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

    private float xRotate = 0.0f;

    private Transform mainCameraTf;
    
    private CharacterController cc;

   // private MoveState moveState = MoveState.Normal;

    public MoveState CurrnetMoveState {get; set;}

    private Dictionary<MoveState, AbstractMover> Movers;

    private Animator anim;

    public enum MoveState{
        Normal, Dash, Jump
    }

    void SetMoveStateNormal(){
        yVelocity = 0;
        currentJumpCount = 0;
        currentDashTime = 0f;
        CurrnetMoveState = MoveState.Normal;
     //   anim.ResetTrigger("Run");
        anim.Rebind();
        anim.SetTrigger("Walk");
    }

    void SetMoveStateJump(){
        yVelocity = jumpPower;
        currentJumpCount++;
        CurrnetMoveState = MoveState.Jump;
    }

    void SetMoveStateDash(KeyCode pressedKey){
        SetMoveStateDash(DashVectorByKeyCode[pressedKey]);
      //  anim.ResetTrigger("Walk");
        anim.Rebind();
        anim.SetTrigger("Run");
    }

    void SetMoveStateDash(Vector3 dashVector){
        this.dashVector = dashVector;
        CurrnetMoveState = MoveState.Dash;
    }

    public bool IsDash(){ return CurrnetMoveState == MoveState.Dash;}
    public bool IsJump(){ return CurrnetMoveState == MoveState.Jump;}

    // Start is called before the first frame update
    void Start()
    {
        this.mainCameraTf = Camera.main.transform;
        this.cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Gravity();
        KeyMove();
    }

    void Gravity(){
        if(cc.isGrounded){
            SetMoveStateNormal();
            return;
        }
        yVelocity += gravity * Time.deltaTime;
    }

    void KeyMove(){ 
        JumpCheck();       
        DashCheck();
        AbstractMover mover = GetMover();
        if(mover == null){
            return;
        }
        mover.Move();
    }

    void JumpCheck(){
        if( Input.GetButtonDown("Jump") && 
            currentJumpCount < maxJumpCount &&
            CurrnetMoveState == MoveState.Normal)
        {
            SetMoveStateJump();
        }
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

        SetMoveStateDash(pressedKey);
    }

    AbstractMover GetMover(){
        if(Movers == null){
            this.Movers = new Dictionary<MoveState, AbstractMover>{
                {MoveState.Normal, new NormalMover(this)},
                {MoveState.Jump, new JumpMover(this)},
                {MoveState.Dash, new DashMover(this)},
            };
        }
        return Movers[this.CurrnetMoveState];
    }





    void DashEndCheck(){
        if(CurrnetMoveState != MoveState.Dash){
            return;
        }
        currentDashTime += Time.deltaTime;
        if(currentDashTime >= dashTime){
            SetMoveStateNormal();
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

    abstract class AbstractMover{
        private CharacterController cc;
        protected AbstractMover(CharacterController cc){
            this.cc = cc;
        }

        public void Move(){
            Vector3 velocity = GetMoveVector3();
            if(velocity == null || velocity == Vector3.zero){
                 return;
            }
            cc.Move(velocity * Time.deltaTime);
        }

        abstract protected Vector3 GetMoveVector3();
    }

    class NormalMover : AbstractMover{
        public int testtest = 0;
        private readonly Bak_SswPlayerControllerMove playerCtrl;
        public NormalMover(Bak_SswPlayerControllerMove playerCtrl) : base(playerCtrl.cc){
            this.playerCtrl = playerCtrl;
        }
        protected override Vector3 GetMoveVector3(){ 
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            Vector3 dir = playerCtrl.mainCameraTf.TransformDirection(new Vector3(h, 0, v));
            dir.y = 0;
            dir.Normalize();
            return dir * playerCtrl.moveSpeed;
        }
    }

    class JumpMover : NormalMover{
        private readonly Bak_SswPlayerControllerMove playerCtrl;
        public JumpMover(Bak_SswPlayerControllerMove playerCtrl) : base(playerCtrl){
            this.playerCtrl = playerCtrl;
        }
        protected override Vector3 GetMoveVector3(){
            Vector3 velocity = base.GetMoveVector3();
            velocity.y = playerCtrl.yVelocity;
            return velocity;
        }
    }

    class DashMover : AbstractMover{
        private readonly Bak_SswPlayerControllerMove playerCtrl;
        public DashMover(Bak_SswPlayerControllerMove playerCtrl) : base(playerCtrl.cc){
            this.playerCtrl = playerCtrl;
        }
        protected override Vector3 GetMoveVector3(){
            Vector3 velocity = playerCtrl.dashVector * playerCtrl.dashSpeed;
            velocity = playerCtrl.mainCameraTf.TransformDirection(velocity);
            velocity.y = 0;
            return velocity;
        }
    }
}