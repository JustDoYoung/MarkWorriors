using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { melee }
    public Type type;
    public int damage = 1;
    // Start is called before the first frame update
    private void Awake()
    {
        Application.targetFrameRate = 40;
    }

    private void OnTriggerEnter(Collider other)
    {
        MonsterAttackPattern monster = other.gameObject.GetComponent<MonsterAttackPattern>();
        monster.GetDamageFromPlayer();

        CharacterStatus characterStatus = other.gameObject.GetComponent<CharacterStatus>();
        characterStatus.SetDamage(damage);
    }
}
