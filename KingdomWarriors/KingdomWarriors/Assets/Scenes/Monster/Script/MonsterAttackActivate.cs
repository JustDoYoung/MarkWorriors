using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackActivate : MonoBehaviour
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
        isAttack = false;
        AttackTrigger.enabled = true;
        yield return new WaitForSeconds(0.5f);
        AttackTrigger.enabled = false;
        isAttack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CharacterStatus status = other.gameObject.GetComponent<CharacterStatus>();
            status.SetDamage(damage);
        }
        print("Player Attacked");

    }
}
