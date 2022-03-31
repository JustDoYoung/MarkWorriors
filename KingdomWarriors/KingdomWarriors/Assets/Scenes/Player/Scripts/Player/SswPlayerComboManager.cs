using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerComboManager : MonoBehaviour
{
    private SswPlayerMediator playerMediator;

    // Start is called before the first frame update
    void Start()
    {
        playerMediator = GetComponent<SswPlayerMediator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckAttack();
    }

    void CheckAttack(){
        if(playerMediator.IsCheckState(CharacterState.Action.State.Jump)){
            return;
        }
        
        if(Input.GetButtonDown("Fire1")){
            playerMediator.OnNormalAttack();
        }

        if(Input.GetButtonDown("Fire2")){
            playerMediator.OnHeavyAttack();
        }
    }
}
