using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimEvent : MonoBehaviour
{
    MonsterAttackPattern monster;
    void Start()
    {
        monster = GetComponentInParent<MonsterAttackPattern>();
    }

    public void OnMonsterAttackAnimHit()
    {
        monster.OnMonsterAttackHit();
    }
    public void OnMonsterReactAnimFinished()
    {
        monster.OnMonsterReactAnimFinished();
    }
    public void OnMonsterDeathAnimFinished()
    {
        monster.OnMonsterDeathAnimFinished();
    }
}
