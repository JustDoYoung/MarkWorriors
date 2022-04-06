using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
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

    void Update()
    {
        setTime -= Time.deltaTime;
        TimeText();
    }
    void TimeText()
    {
        int min = (int)setTime / 60;
        int second = (int)setTime % 60;
        if (setTime < 0)
        {
            return;
        }

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
}
