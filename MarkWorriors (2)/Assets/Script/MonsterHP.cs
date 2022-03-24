using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHP : MonoBehaviour
{
    public int hp;
    public int maxHP = 5;

    public int HP
    {
        get { return hp; }
        set
        {
            this.hp = value;
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        HP = maxHP;
    }
}
