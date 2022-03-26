using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRegenerator : MonoBehaviour
{
    //������ �ð��� ���Ͱ� �����ǵ��� �ϰ� �ʹ�.
    float curtime;
    float createTime;
    public GameObject[] monsterFactory;
    public float min = 1f;
    public float max = 3f;
    void Start()
    {
        createTime = Random.Range(min, max);
    }

    void Update()
    {
        curtime += Time.deltaTime;
        //�����ð��� �ʰ��ϸ�
        if(curtime > createTime)
        {
            print("�����ð� : " + createTime);
            //���忡�� ���͸� �޾ƿ��� �ʹ�.
            int randValue = Random.Range(0, monsterFactory.Length);
            GameObject monster = Instantiate(monsterFactory[randValue]);
            //�޾ƿ� ���͸� ���� ��ġ�� ���� �ʹ�.
            monster.transform.position = transform.position;
            //����ð��� 0���� �����ϰ� �ʹ�.
            curtime = 0;
            //�����ð��� �����ϰ� �������ϰ� �ʹ�.
            createTime = Random.Range(min, max);
        }
    }
}
