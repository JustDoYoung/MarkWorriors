using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAnimEvent : MonoBehaviour
{
    SkeletonAttackPattern skeleton;
    void Start()
    {
        skeleton = GetComponentInParent<SkeletonAttackPattern>();
    }

    public void OnMonsterAttackAnimHit()
    {
        skeleton.OnMonsterAttackHit();
    }
    public void OnMonsterReactAnimFinished()
    {
        skeleton.OnMonsterReactAnimFinished();
    }
    public void OnMonsterDeathAnimFinished()
    {
        skeleton.OnMonsterDeathAnimFinished();
    }
}
