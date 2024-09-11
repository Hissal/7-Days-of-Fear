using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager instance;
    void Awake()
    {
        instance = this;
    }

    public static event Action<int> OnMinuteChanged = delegate { };
    public static event Action<int> OnHourChanged = delegate { };
    public static event Action<int> OnDayChanged = delegate { };
    public static event Action OnMorning = delegate { };
    public static event Action OnEvening = delegate { };

    public static int minute { get; private set; }
    public static int hour { get; private set; }
    public static int day { get; private set; }

    [SerializeField] private float realTimeSecondsToInGameMinute = 1f;
    private float timer;

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
        SetTime(1, 0, 0);
    }

    public static void SetTime(int day, int hour, int minute)
    {
        if (IsMorning() && hour > 15) OnEvening?.Invoke();
        else if (!IsMorning() && hour < 16) OnMorning?.Invoke();

        if (TimeManager.day != day) OnDayChanged?.Invoke(day);
        if (TimeManager.hour != hour) OnHourChanged?.Invoke(hour);
        if (TimeManager.minute != minute) OnMinuteChanged?.Invoke(minute);

        TimeManager.day = day;
        TimeManager.hour = hour;
        TimeManager.minute = minute;
        instance.timer = instance.realTimeSecondsToInGameMinute;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            minute++;

            if (minute >= 60)
            {
                if (hour == 16) OnEvening?.Invoke();
                else if (hour == 0) OnMorning?.Invoke();

                hour++;
                minute = 0;

                if (hour >= 24)
                {
                    day++;
                    hour = 0;
                    minute = 0;
                    OnDayChanged?.Invoke(day);
                }

                OnHourChanged?.Invoke(hour);
            }

            OnMinuteChanged?.Invoke(minute);

            timer = realTimeSecondsToInGameMinute + timer;
        }
    }
}
