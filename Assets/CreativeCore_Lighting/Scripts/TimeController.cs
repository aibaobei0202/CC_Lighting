using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    //控制时间流速，单位（秒）
    [SerializeField]
    private float timeMultiplier;

    //添加序列化字段，用来表示UI中的富文本框，用来显示24小时制的时间
    [SerializeField]
    private TextMeshProUGUI timeText;

    //添加序列化字段，用来起始时间
    [SerializeField]
    private float startHour;

    //表示当前时刻时间
    private DateTime currentTime;
    //表示日出时间
    private TimeSpan sunriseTime;
    //表示日落时间
    private TimeSpan sunsetTime;

    //添加序列化字段，从unity中获取输入的日出时间
    [SerializeField]
    private float sunriseHour;

    //添加序列化字段，从unity中获取输入的日落时间
    [SerializeField]
    private float sunsetHour;

    //添加序列化字段，从unity中获取方向光作为太阳光
    [SerializeField]
    private Light sunlight;

    // Start is called before the first frame update
    void Start()
    {
        //DateTime指固定的时间，而TimeSpan则代表一段时间间隔
        //DateTime.Now.Date指系统当前时间(年/月/日），不包含小时、分、秒，返回类型为DateTime
        //TimeSpan.FromHours(startHour),从startHour中获取其来自小时的部分。
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

    //旋转定向光
    private void RotateSunLight()
    {
        //阳光下的旋转
        float sunLightRotation;

        //TimeOfDay获取此实例的当天的时间。
        //如果当前时间大于日出时间,小于日落时间,则判定现在是白天 
        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime)
        {
            //计算日出时间到日落时间的时间差
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            //从日出到当前时间的时间差
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);
            //计算当前时间/白天总时间
            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            //基于浮点数percentage返回0-180之间的插值，percentage限制在0-1之间，当percentage=0时、返回0，percentage=1时、返回180.
            sunLightRotation = Mathf.Lerp(0, 180, (float)percentage);
        } else
        {
            TimeSpan sunsetToSuniseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            //从日升到当前时间的时间差
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSuniseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float)percentage);
        }
        Debug.Log($"旋转：{sunLightRotation}");
        //将sunlight定向光方向进行设定
        //Quaternion:四元数，计算方位，旋转的
        //AngleAxis：表示旋转一定的角度，围绕某个轴
        sunlight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    //计算时间差
    private TimeSpan CalculateTimeDifference(TimeSpan fromTime,TimeSpan toTime)
    {
        //设置时间间隔 末时间-初始时间
        TimeSpan diff = toTime - fromTime;
        //若结果小于0，则加上24小时，补足一天
        if (diff.TotalSeconds < 0)
        {
            diff += TimeSpan.FromHours(24);
        }
        return diff;
    }
}
