using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerShortcutKeys : MonoBehaviour
{
    private SswPlayerMediator playerMediator;

    int tempHpPotion = 20;

    // Start is called before the first frame update
    void Start()
    {
        playerMediator = GetComponent<SswPlayerMediator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            drinkHpPotion();
        }
    }

    private void drinkHpPotion(){
        playerMediator.RecoveryHp(tempHpPotion);
    }
}
