using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public int damage = 2;
    private void Awake()
    {
        Application.targetFrameRate = 40;
    }

}
