using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface OnAttackEvent{
    public void OnAttack(GameObject target, WeaponStatus weaponStatus);
}

public class WeaponStatus : MonoBehaviour
{
    public int power = 1;
    private OnAttackEvent[] attackEvents;
    private List<Collider> attackedColliders = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        attackEvents = GetComponentsInParent<OnAttackEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Enemy" || attackedColliders.Contains(other)){
            return;
        }
        foreach(OnAttackEvent evt in attackEvents){
            evt.OnAttack(other.gameObject, this);
            attackedColliders.Add(other);
        }
    }

    public void AttackedListClear(){
        attackedColliders.Clear();
    }
}
