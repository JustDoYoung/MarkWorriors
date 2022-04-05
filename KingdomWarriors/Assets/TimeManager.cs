using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public GameObject[] hiddenMonster;
    public int demonAwakeTime = 60;
    public int UndeadKnightAwakeTime = 50;
    public static TimeManager instance;
    private void Awake()
    {
        TimeManager.instance = this;
    }
    public Text textTime;
    public float setTime = 65;

    void Start()
    {
        textTime.text = "3 : 00";
    }

    // Update is called once per frame
    void Update()
    {
        setTime -= Time.deltaTime;
        TimeText();
        hiddenMonsterAwake();
    }
    void TimeText()
    {
        int min = (int)setTime / 60;
        int second = (int)setTime % 60;
        if (setTime > 60)
        {
            if (second < 10)
            {
                textTime.text = "0" + min + " : 0" + second;
            }
            else
            {
                textTime.text = "0" + min + " : " + second;
            }
        }
        else
        {
            textTime.text = "00 : " + second;
        }
    }
    void hiddenMonsterAwake()
    {
        if (setTime < UndeadKnightAwakeTime)
        {
            for (int i = 0; i < hiddenMonster.Length; i++)
            {
                if (hiddenMonster[i].name.Contains("Undead"))
                {
                    hiddenMonster[i].SetActive(true);
                }
            }
        }
        else if (setTime < demonAwakeTime)
        {
            for (int i = 0; i < hiddenMonster.Length - 1; i++)
            {
                if (hiddenMonster[i].name.Contains("Demon"))
                {
                    hiddenMonster[i].SetActive(true);
                }
            }
        }
    }
}
