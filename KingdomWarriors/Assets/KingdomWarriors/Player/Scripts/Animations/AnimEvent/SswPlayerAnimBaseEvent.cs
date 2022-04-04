using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SswPlayerAnimBaseEvent : StateMachineBehaviour
{
    private SswPlayerMediator PlayerMediator;
    protected SswPlayerMediator GetPlayerMediator(Animator animator){
        return (PlayerMediator == null) ? animator.GetComponentInParent<SswPlayerMediator>() : PlayerMediator;
    }
}