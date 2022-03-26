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
        if (other.gameObject.tag == "Demon")
        {
            DemonAttackPattern monster = other.gameObject.GetComponent<DemonAttackPattern>();
            CharacterStatus characterStatus = other.gameObject.GetComponent<CharacterStatus>();
            monster.GetDamageFromPlayer();
            characterStatus.SetDamage(damage);
        }
        if (other.gameObject.tag == "UndeadKnight")
        {
            UndeadKnightAttackPattern monster = other.gameObject.GetComponent<UndeadKnightAttackPattern>();
            CharacterStatus characterStatus = other.gameObject.GetComponent<CharacterStatus>();
            monster.GetDamageFromPlayer();
            characterStatus.SetDamage(damage);
        }
        if (other.gameObject.tag == "Skeleton")
        {
            SkeletonAttackPattern monster = other.gameObject.GetComponent<SkeletonAttackPattern>();
            CharacterStatus characterStatus = other.gameObject.GetComponent<CharacterStatus>();
            monster.GetDamageFromPlayer();
            characterStatus.SetDamage(damage);
        }
    }
}
