using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimEvent : MonoBehaviour
{
    MonsterRe monsterRe;
    void Start()
    {
        monsterRe = GetComponentInParent<MonsterRe>();
    }

    public void OnMonsterAttackAnimHit()
    {
        monsterRe.OnMonsterAttackHit();
    }
    public void OnMonsterAttackAnimHitFinish()
    {
        monsterRe.OnMonsterAttackAnimHitFinish();
    }
    public void OnMonsterReactAnimFinished()
    {
        monsterRe.OnMonsterReactAnimFinished();
    }
    public void OnMonsterDeathAnimFinished()
    {
        monsterRe.OnMonsterDeathAnimFinished();
    }
}
