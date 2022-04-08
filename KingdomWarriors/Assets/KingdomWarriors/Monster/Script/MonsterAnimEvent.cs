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
    internal void OnMonsterReactAnimFinished()
    {
        monster.OnMonsterReactAnimFinished();
    }
    internal void OnMonsterDeathAnimFinished()
    {
        monster.OnMonsterDeathAnimFinished();
    }
    internal void MonsterAttackFinish()
    {
        monster.MonsterAttackFinish();
    }
    internal void MonsterAttackActivation()
    {
        monsterAttack.MonsterAttackActivation();
    }
    internal void MonsterAttackInActivation()
    {
        monsterAttack.MonsterAttackInActivation();
    }
    internal void MonsterGrowlSoundActivation()
    {
        monster.MonsterGrowlSoundActivation();
    }
    internal void MonsterReactSoundActivation()
    {
        monster.MonsterReactSoundActivation();
    }
    internal void DemonRushSoundActivation()
    {
        monster.DemonRushSoundActivation();
    }
    internal void UndeadKnigthSlashSoundActivation()
    {
        monster.UndeadKnigthSlashSoundActivation();
    }
}
