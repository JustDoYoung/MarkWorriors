using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndeadKnight_Meteor : MonoBehaviour
{
    public int damage = 2;

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
}
