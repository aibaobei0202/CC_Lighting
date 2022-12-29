using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    //����ʱ�����٣���λ���룩
    [SerializeField]
    private float timeMultiplier;

    //������л��ֶΣ�������ʾUI�еĸ��ı���������ʾ24Сʱ�Ƶ�ʱ��
    [SerializeField]
    private TextMeshProUGUI timeText;

    //������л��ֶΣ�������ʼʱ��
    [SerializeField]
    private float startHour;

    //��ʾ��ǰʱ��ʱ��
    private DateTime currentTime;
    //��ʾ�ճ�ʱ��
    private TimeSpan sunriseTime;
    //��ʾ����ʱ��
    private TimeSpan sunsetTime;

    //������л��ֶΣ���unity�л�ȡ������ճ�ʱ��
    [SerializeField]
    private float sunriseHour;

    //������л��ֶΣ���unity�л�ȡ���������ʱ��
    [SerializeField]
    private float sunsetHour;

    //������л��ֶΣ���unity�л�ȡ�������Ϊ̫����
    [SerializeField]
    private Light sunlight;

    // Start is called before the first frame update
    void Start()
    {
        //DateTimeָ�̶���ʱ�䣬��TimeSpan�����һ��ʱ����
        //DateTime.Now.Dateָϵͳ��ǰʱ��(��/��/�գ���������Сʱ���֡��룬��������ΪDateTime
        //TimeSpan.FromHours(startHour),��startHour�л�ȡ������Сʱ�Ĳ��֡�
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sunsetHour);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimeOfDay();
        RotateSunLight();   
    }

    private void UpdateTimeOfDay()
    {
        currentTime = currentTime.AddSeconds(timeMultiplier*Time.deltaTime);
        if(timeText != null)
        {
            timeText.text = currentTime.ToString("dd:HH:mm");
        }
    }

    //��ת�����
    private void RotateSunLight()
    {
        //�����µ���ת
        float sunLightRotation;

        //TimeOfDay��ȡ��ʵ���ĵ����ʱ�䡣
        //�����ǰʱ������ճ�ʱ��,С������ʱ��,���ж������ǰ���
        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            //����������ǰʱ���ʱ���
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        } else
        {
            TimeSpan sunsetToSuniseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            //����������ǰʱ���ʱ���
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSuniseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }
        Debug.Log($"��ת��{sunLightRotation}");
        sunlight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime,TimeSpan toTime)
    {
        TimeSpan diff = toTime - fromTime;
        if (diff.TotalSeconds < 0)
        {
            diff += TimeSpan.FromHours(24);
        }
        return diff;
    }
}
