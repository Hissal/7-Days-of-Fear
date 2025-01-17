﻿using System;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager instance;
    void Awake()
    {
        Debug.Log("Creating TimeManager Instance");
        instance = this;
    }

    //public static event Action<int> OnMinuteChanged = delegate { };
    //public static event Action<int> OnHourChanged = delegate { };
    public static event Action<int> OnDayChanged = delegate { };
    public static event Action OnMorning = delegate { };
    public static event Action OnEvening = delegate { };

    public static int minute { get; private set; }
    public static int hour { get; private set; }
    public static int day { get; private set; }

    [SerializeField] private float realTimeSecondsToInGameMinute = 1f;
    private float timer;

    public static bool evening { get; private set; }


    public static bool IsMorning()
    {
        if (hour < 13)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Start()
    {
        //SetTime(1, 0, 0, false);
    }

    public static void SetDayDirty(int day)
    {
        TimeManager.day = day;
    }

    public static void SetTime(int day, int hour, int minute, bool callMorning, bool retry)
    {
        //if (IsMorning() && hour > 15) OnEvening?.Invoke();
        //else if (!IsMorning() && hour < 16) OnMorning?.Invoke();

        if (TimeManager.day != day || retry) OnDayChanged?.Invoke(day);
        //if (TimeManager.hour != hour) OnHourChanged?.Invoke(hour);
        //if (TimeManager.minute != minute) OnMinuteChanged?.Invoke(minute);

        TimeManager.day = day;
        TimeManager.hour = hour;
        TimeManager.minute = minute;
        instance.timer = instance.realTimeSecondsToInGameMinute;

        if (day == 1) OnEvening?.Invoke();
        if (callMorning) OnMorning?.Invoke();
    }

    public static void OnMorningInvoke()
    {
        OnMorning?.Invoke();
        evening = false;
    }
    public static void OnEveningInvoke()
    {
        OnEvening?.Invoke();
        evening = true;
    }

    private void Update()
    {

#if UNITY_EDITOR
        // For Debugging purposes
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetTime(1, 0, 0, false, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetTime(2, 0, 0, false, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetTime(3, 0, 0, false, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetTime(4, 0, 0, false, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetTime(5, 0, 0, false, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SetTime(6, 0, 0, false, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SetTime(7, 0, 0, false, false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            OnEveningInvoke();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            OnMorningInvoke();
        }
#endif

        //timer -= Time.deltaTime;

        //if (timer <= 0)
        //{
        //    minute++;

        //    if (minute >= 60)
        //    {
        //        hour++;
        //        minute = 0;

        //        if (hour >= 24)
        //        {
        //            day++;
        //            hour = 0;
        //            minute = 0;
        //            OnDayChanged?.Invoke(day);
        //        }

        //        OnHourChanged?.Invoke(hour);
        //    }

        //    OnMinuteChanged?.Invoke(minute);

        //    timer = realTimeSecondsToInGameMinute + timer;
        //}
    }
}
