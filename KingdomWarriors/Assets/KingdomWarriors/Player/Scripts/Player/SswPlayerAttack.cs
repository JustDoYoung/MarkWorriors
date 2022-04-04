using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerAttack : MonoBehaviour
{
    private CharacterMediator playerMediator;
    private const float AttackDelayTime = 0.5f;
    private bool isAttackLock = false;

    // Start is called before the first frame update
    void Start()
    {
        playerMediator = GetComponent<CharacterMediator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckAttack();
    }

    void CheckAttack(){
        if(isAttackLock || playerMediator.IsCheckState(CharacterState.Action.State.Jump)){
            return;
        }
        
        bool isNormalAttack = Input.GetButtonDown("Fire1");
        bool isHeavyAttack = Input.GetButtonDown("Fire2");

        if(isNormalAttack == false && isHeavyAttack == false){
            return;
        }
        isAttackLock = true;
        StartCoroutine(CRAttackDelay());
        if(isNormalAttack){ playerMediator.NormalAttack(); }
        if(isHeavyAttack){ playerMediator.HeavyAttack(); }
    }

    private IEnumerator CRAttackDelay(){
        yield return new WaitForSeconds(AttackDelayTime);
        isAttackLock = false;
    }
}
