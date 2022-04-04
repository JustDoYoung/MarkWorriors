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
        SetCollidersTrigger(false);
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
        SetCollidersTrigger(true);
    }

    private void SetCollidersTrigger(bool isTrigger){
        this.SetCollidersTrigger(isTrigger, this.weaponStatus);
    }
    private void SetCollidersTrigger(bool isTrigger, WeaponStatus weapon){
        if(weapon == null){
            return;
        }
        this.SetCollidersTrigger(isTrigger, weapon.GetComponents<Collider>());
    }
    private void SetCollidersTrigger(bool isTrigger, Collider[] targetColliders){
        if(targetColliders == null){
            return;
        }
        foreach(Collider collider in targetColliders){
            collider.isTrigger = isTrigger;
        }
    }

    public void SetChangeWeapon(WeaponStatus newWeapon){
        ThrowOldWeapon(weaponStatus);
        RaiseNewWeapon(newWeapon);
    }

    private void ThrowOldWeapon(WeaponStatus oldWeapon){
        SetCollidersTrigger(true, oldWeapon);

        oldWeapon.transform.rotation = Quaternion.identity;
        oldWeapon.transform.parent = null;
        oldWeapon.InitWeapon();

        weaponStatus = null;
        weaponColliders = null;
    }

    private void RaiseNewWeapon(WeaponStatus newWeapon){
        newWeapon.transform.position = transform.position;
        newWeapon.transform.rotation = transform.rotation;
        newWeapon.transform.parent = transform;
        newWeapon.InitWeapon();

        weaponStatus = newWeapon;
        weaponColliders = weaponStatus.GetComponents<Collider>();
        weaponStatus.gameObject.SetActive(true);
    }
}
