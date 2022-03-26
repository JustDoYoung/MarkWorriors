using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonAnimEvent : MonoBehaviour
{
    DemonAttackPattern demon;
    void Start()
    {
        demon = GetComponentInParent<DemonAttackPattern>();
    }

    public void OnMonsterAttackAnimHit()
    {
        demon.OnMonsterAttackHit();
    }
    public void OnMonsterReactAnimFinished()
    {
        demon.OnMonsterReactAnimFinished();
    }
    public void OnMonsterDeathAnimFinished()
    {
        demon.OnMonsterDeathAnimFinished();
    }
}
