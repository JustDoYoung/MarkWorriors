using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public int damage = 2;

    Collider AttackTrigger;
    public bool isAttack = true;

    public void MonsterAttack()
    {
        AttackTrigger = GetComponent<Collider>();
        AttackTrigger.enabled = false;
        StopCoroutine(Attack());
        StartCoroutine(Attack());
    }
    IEnumerator Attack()
    {
        print("Attack");
        isAttack = false;
        AttackTrigger.enabled = true;
        yield return new WaitForSeconds(1f);
        AttackTrigger.enabled = false;
        isAttack = true;
    }
}
