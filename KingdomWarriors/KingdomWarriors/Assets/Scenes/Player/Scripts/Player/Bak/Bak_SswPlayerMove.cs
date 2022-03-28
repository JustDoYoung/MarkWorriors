using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bak_SswPlayerMove : MonoBehaviour
{

    public float moveSpeed = 5f;

    public float turnSpeed = 5f;

    public float jumpPower = 300f;
    public float dashBatterTerm = 0.15f;

    private const int dateTimeBinaryOffset = 10000000;

    private readonly Dictionary<KeyCode, Vector3> DashVectorByKeyCode = new Dictionary<KeyCode, Vector3> {
        {KeyCode.W, Vector3.forward},
        {KeyCode.A, Vector3.left}, {KeyCode.S, Vector3.back}, {KeyCode.D, Vector3.right},
    };

    private KeyCode prevPressedMoveKey = KeyCode.None;

    private long prevPressedKeyTime = System.DateTime.Now.ToBinary();

    private bool isJumping = false;

    private Rigidbody rigidbody;

    private float xRotate = 0.0f;

    private string MAP_NAME = "Map";

    // Start is called before the first frame update
    void Start()
    {  
        rigidbody = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        KeyMove();
        MouseRotation();
    }

    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.name == MAP_NAME){
            this.isJumping = false;
        }
    }

    void KeyMove(){
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        transform.position += new Vector3(h, 0, v).normalized * moveSpeed * Time.deltaTime;
        
        Jump();
        Dash();
    }

    void Jump(){
        if(isJumping || Input.GetButtonDown("Jump") == false){
            return;
        }

        rigidbody.AddForce(transform.up * jumpPower);
        isJumping = true;
    }

    void Dash(){
        KeyCode pressedKey = GetPressedMoveKey();
        if(pressedKey == KeyCode.None || isPressedDash(pressedKey) == false){
            return;
        }

        print("대시");
    }

    bool isPressedDash(KeyCode pressedKeyCode){
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

    KeyCode GetPressedMoveKey(){
        foreach (KeyCode keyCode in DashVectorByKeyCode.Keys){
            if(Input.GetKeyDown(keyCode)){
                return keyCode;
            }
        }
        return KeyCode.None;
    }

    void MouseRotation(){
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        // 좌우로 움직인 마우스의 이동량 * 속도에 따라 카메라가 좌우로 회전할 양 계산
        float yRotateSize = Input.GetAxis("Mouse X") * turnSpeed;
        // 현재 y축 회전값에 더한 새로운 회전각도 계산
        float yRotate = transform.eulerAngles.y + yRotateSize;

        // 위아래로 움직인 마우스의 이동량 * 속도에 따라 카메라가 회전할 양 계산(하늘, 바닥을 바라보는 동작)
        float xRotateSize = -Input.GetAxis("Mouse Y") * turnSpeed;
        // 위아래 회전량을 더해주지만 -45도 ~ 80도로 제한 (-45:하늘방향, 80:바닥방향)
        // Clamp 는 값의 범위를 제한하는 함수
        xRotate = Mathf.Clamp(xRotate + xRotateSize, -45, 80);
    
        // 카메라 회전량을 카메라에 반영(X, Y축만 회전)
      //  transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
        transform.eulerAngles = new Vector3(0, yRotate, 0);
    }
}