using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trash
{
    public class Weapon : MonoBehaviour
    {
        public enum Type { melee }
        public Type type;
        public int damage = 1;

        private void OnTriggerEnter(Collider other)
        {

            // SswCharacterStatus characterStatus = other.gameObject.GetComponent<SswCharacterStatus>();
            // MonsterAttackPatternCommon monster = other.gameObject.GetComponent<MonsterAttackPatternCommon>();
            // monster.GetDamageFromPlayer();
            // characterStatus.SetDamage(damage);
        }
    }
}