using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface OnAttackEvent{
    public void OnAttackDetect(GameObject hitTarget, WeaponStatus weaponStatus);
}

public class WeaponStatus : GameItem
{
    public int power = 1;
    private OnAttackEvent[] attackEvents;
    private List<Collider> attackedColliders = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        InitWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitWeapon(){
        attackEvents = GetComponentsInParent<OnAttackEvent>();
        AttackedListClear();
    }

    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Enemy") == false || attackedColliders.Contains(other)){
            return;
        }
        Logger.Log("attack", attackEvents.ToString());
        foreach(OnAttackEvent evt in attackEvents){
            evt.OnAttackDetect(other.gameObject, this);
            attackedColliders.Add(other);
        }
    }

    public void AttackedListClear(){
        attackedColliders.Clear();
    }
}
