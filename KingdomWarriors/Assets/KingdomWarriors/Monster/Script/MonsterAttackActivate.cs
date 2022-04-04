using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackActivate : MonoBehaviour
{
    public int damage = 2;

    Collider AttackTrigger;

    private void Start()
    {
        AttackTrigger = GetComponent<Collider>();
        AttackTrigger.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // CharacterStatus status = other.gameObject.GetComponent<CharacterStatus>();
            Interaction.Attack attack = other.gameObject.GetComponent<Interaction.Attack>();
            attack.OnAttackHit(damage);
            //attack.OnAttack(damage);
        }
        print("Player Attacked");
    }

    IEnumerator RushAttack()
    {
        AttackTrigger.enabled = true;
        yield return new WaitForSeconds(2f);
        AttackTrigger.enabled = false;
    }
    internal void MonsterAttackInActivation()
    {
        AttackTrigger.enabled = false;
    }

    internal void MonsterAttackActivation()
    {
        StartCoroutine(RushAttack());
    }
}
