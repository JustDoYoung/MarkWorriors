using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public GameObject model;

    public float speed = 10f;
    public float rotSpeed = 5f;

    private Transform player;
    private Camera mainCamera;
    private Animator anim;
    private Vector3 stickDirection;
    private Vector3 velocity;

    private CharacterController cc;
    void Start()
    {
        player = transform;
        mainCamera = Camera.main;
        cc = GetComponent<CharacterController>();
        anim = model.GetComponent<Animator>();
    }


    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        stickDirection = new Vector3(h, 0, v);
        stickDirection.Normalize();

        velocity = mainCamera.transform.TransformDirection(stickDirection) * speed;

        cc.SimpleMove(velocity);

        HandlerInput();
        HandlerStandardLocomotionRotation();
    }

    void HandlerStandardLocomotionRotation()
    {
        Vector3 rotDirection = mainCamera.transform.forward;
        rotDirection.y = 0;
        player.forward = Vector3.Lerp(player.forward, rotDirection, rotSpeed * Time.deltaTime);
    }

    void HandlerInput()
    {
        anim.SetFloat("Speed", Vector3.ClampMagnitude(stickDirection, 1).magnitude);
        anim.SetFloat("Horizontal", stickDirection.x);
        anim.SetFloat("Vertical", stickDirection.z);

    }
}
