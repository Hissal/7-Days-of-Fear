using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeTMP;

    private void OnEnable()
    {
        TimeManager.OnMinuteChanged += UpdateTime;
    }

    private void OnDisable()
    {
        TimeManager.OnMinuteChanged -= UpdateTime;
    }

    private void UpdateTime(int minute)
    {
        timeTMP.text = $"Aug {TimeManager.day}. {TimeManager.hour:00}:{TimeManager.minute:00}";
    }
}
