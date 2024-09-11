using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayTMP;

    private void OnEnable()
    {
        TimeManager.OnDayChanged += updateDay;
    }

    private void OnDisable()
    {
        TimeManager.OnDayChanged -= updateDay;
    }

    private void updateDay(int day)
    {
        dayTMP.text = $"Aug {day}";
    }
}
