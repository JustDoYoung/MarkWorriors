using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponWearableHand : MonoBehaviour
{
    private SswPlayerMediator playerMediator;
    private WeaponStatus weaponStatus;
    private Collider[] weaponColliders;

    // Start is called before the first frame update
    void Start()
    {
        playerMediator = GetComponentInParent<SswPlayerMediator>();
        weaponStatus = GetComponentInChildren<WeaponStatus>();
        weaponColliders = weaponStatus.GetComponents<Collider>();
    }

    // Update is called once per frame
    void Update(){

    }

    public void Attack(bool isAttack){
        if(isAttack == false){
            weaponStatus.AttackedListClear();
        }
        if(weaponColliders == null){
            return;
        }
        foreach(Collider collider in weaponColliders){
            collider.isTrigger = isAttack;
        }
    }
}
