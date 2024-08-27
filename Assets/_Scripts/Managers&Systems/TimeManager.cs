using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager instance;
    void Awake()
    {
        instance = this;
    }

    public static event Action OnMinuteChanged = delegate { };
    public static event Action OnHourChanged = delegate { };
    public static event Action OnDayChanged = delegate { };

    public static int minute { get; private set; }
    public static int hour { get; private set; }
    public static int day { get; private set; }

    [SerializeField] private float realTimeSecondsToInGameMinute = 1f;
    private float timer;

    private void Start()
    {
        SetTime(1, 0, 0);
    }

    public static void SetTime(int day, int hour, int minute)
    {
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
                hour++;
                minute = 0;

                if (hour >= 24)
                {
                    day++;
                    hour = 0;
                    minute = 0;
                    OnDayChanged?.Invoke();
                }

                OnHourChanged?.Invoke();
            }

            OnMinuteChanged?.Invoke();

            timer = realTimeSecondsToInGameMinute + timer;
        }
    }
}
