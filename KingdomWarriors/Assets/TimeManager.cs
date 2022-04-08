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
        textTime.text = "00 : 00";
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
            GameOverManager.instance.GameOverUI.SetActive(true);
        }

        //00:00 자릿수에 맞춰 시간을 표현하고 싶다.
        //10:00 이상일 때 0을 붙이지 않는다.
        //10분 미만일 때는 분 단위에 0을 붙인다.

        //초 단위가 10초 이하일 때는 0을 붙인다.
        //10초 이상일 때는 0을 붙이지 않는다.

        if (setTime >= 600)
        {
            if (second < 10)
            {
                textTime.text = min + " : 0" + second;
            }
            else
            {
                textTime.text = min + " : " + second;
            }

            // textTime.text = "0" + min + " : 0" + second;
        }
        else if (setTime > 0)
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

    }
}
