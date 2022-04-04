using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 테스트
public class SswCharacterStatus : CharacterStatus
{

    protected override void SetHpByLevel(int level)
    {
        base.SetHpByLevel(level);
        MaxHP += (Level * 100);
        HP = MaxHP;
    }

    protected override void SetMpByLevel(int level)
    {
        base.SetMpByLevel(level);
        MaxHP += (Level * 100);
        MP = MaxMP;
    }

    protected override void SetOffensePowerByLevel(int level)
    {
        base.SetOffensePowerByLevel(level);
        OffensePower += level;
    }

    protected override void SetDefensePowerByLevel(int level)
    {
        base.SetDefensePowerByLevel(level);
        DefensePower += level;
    }
}
