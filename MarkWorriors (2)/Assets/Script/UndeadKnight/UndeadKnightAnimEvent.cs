using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndeadKnightAnimEvent : MonoBehaviour
{
    UndeadKnightAttackPattern undead;
    void Start()
    {
        undead = GetComponentInParent<UndeadKnightAttackPattern>();
    }

    public void OnMonsterAttackAnimHit()
    {
        undead.OnMonsterAttackHit();
    }
    public void OnMonsterReactAnimFinished()
    {
        undead.OnMonsterReactAnimFinished();
    }
    public void OnMonsterDeathAnimFinished()
    {
        undead.OnMonsterDeathAnimFinished();
    }
}
