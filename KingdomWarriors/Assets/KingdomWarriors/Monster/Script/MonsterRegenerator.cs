using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRegenerator : MonoBehaviour
{
    //랜덤한 시간에 몬스터가 생성되도록 하고 싶다.
    float curtime;
    float createTime;
    public GameObject[] monsterFactory;
    public float min = 1f;
    public float max = 3f;
    void Start()
    {
        createTime = Random.Range(min, max);
        print( transform.localPosition);
    }

    void Update()
    {
        curtime += Time.deltaTime;
        //생성시간을 초과하면
        if(curtime > createTime)
        {
            //공장에서 몬스터를 받아오고 싶다.
            int randValue = Random.Range(0, monsterFactory.Length);
            GameObject monster = Instantiate(monsterFactory[randValue]);
            //받아온 몬스터를 현재 위치에 놓고 싶다.

            monster.transform.position = transform.localPosition;
            //현재시간을 0으로 리셋하고 싶다.
            curtime = 0;
            //생성시간을 랜덤하게 재지정하고 싶다.
            createTime = Random.Range(min, max);
        }
    }
}
