using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerAnimEventProxy : MonoBehaviour
{
    SswPlayerMediator playerMediator;
    // Start is called before the first frame update
    void Start()
    {
        playerMediator = GetComponentInParent<SswPlayerMediator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAttack1MotionFinishedEvent(){
    //    playerMediator.OnAttack1MotionFinishedEvent();
    }

    public void OnAttack1FinishedEvent(){
    //    playerMediator.OnAttack1FinishedEvent();
    }

    public void OnAttack2FinishedEvent(){
    //    playerMediator.OnAttack2FinishedEvent();
    }

    public void OnAttack3FinishedEvent(){
     //   playerMediator.OnAttack3FinishedEvent();
    }
}
