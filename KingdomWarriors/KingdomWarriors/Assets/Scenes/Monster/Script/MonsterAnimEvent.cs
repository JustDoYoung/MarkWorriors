using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimEvent : MonoBehaviour
{
    MonsterAttackPatternCommon monster;
    MonsterAttackActivate monsterAttack;
    void Start()
    {
        monster = GetComponentInParent<MonsterAttackPatternCommon>();
        GameObject obj = transform.parent.gameObject;
        monsterAttack = obj.GetComponentInChildren<MonsterAttackActivate>();
    }

    // public void OnMonsterAttackAnimHit()
    // {
    //     monster.OnMonsterAttackHit();
    // }
    public void OnMonsterReactAnimFinished()
    {
        monster.OnMonsterReactAnimFinished();
    }
    public void OnMonsterDeathAnimFinished()
    {
        monster.OnMonsterDeathAnimFinished();
    }
    public void MonsterAttackActivation()
    {
        monsterAttack.MonsterAttackActivation();
    }
    public void MonsterAttackInActivation()
    {
        monsterAttack.MonsterAttackInActivation();
    }
}
