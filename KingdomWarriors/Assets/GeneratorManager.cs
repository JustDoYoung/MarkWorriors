using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    public static GeneratorManager instance;
    private void Awake()
    {
        GeneratorManager.instance = this;
    }
    public float maxCount = 10;
    public int killCount = 0;
    public int DemonKillCount = 3;
    public int createCount = 0;
    public int demonAwakeCondition = 10;
    public GameObject[] hiddenMonster;
    public int KILLCOUNT
    {
        get { return killCount; }
        set
        {
            killCount = value;
            createCount--;
        }
    }
    public int DEMONKILLCOUNT
    {
        get { return DemonKillCount; }
        set
        {
            DemonKillCount = value;
        }
    }
    void Update()
    {
        HiddenMonsterAwake();
    }
    void HiddenMonsterAwake()
    {
        if (KILLCOUNT == demonAwakeCondition)
        {
            for (int i = 0; i < hiddenMonster.Length; i++)
            {
                if (hiddenMonster[i].name.Contains("Demon"))
                {
                    hiddenMonster[i].SetActive(true);
                }
            }
            KILLCOUNT++;
        }
        else if (DemonKillCount == 0)
        {
            for (int i = 0; i < hiddenMonster.Length; i++)
            {
                if (hiddenMonster[i].name.Contains("Undead"))
                {
                    hiddenMonster[i].SetActive(true);
                    DemonKillCount--;
                }
            }
        }

    }
}
