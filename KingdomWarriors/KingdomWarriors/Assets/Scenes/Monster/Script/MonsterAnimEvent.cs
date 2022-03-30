using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimEvent : MonoBehaviour
{
    MonsterAttackPatternCommon monster;
    void Start()
    {
        monster = GetComponentInParent<MonsterAttackPatternCommon>();
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
