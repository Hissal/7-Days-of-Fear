using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventSystem : MonoBehaviour
{
    private int currentDay;
    public int CurrentDay => currentDay;

    private float currentTime;
    public float CurrentTime => currentTime;

    [SerializeField] private TextMeshProUGUI timeText;

    public void NextDay()
    {
        //TODO Play Day Change Animation
        currentDay++;
    }

    private void CountTime()
    {
        currentTime += Time.deltaTime;
    }

    public static EventSystem Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
