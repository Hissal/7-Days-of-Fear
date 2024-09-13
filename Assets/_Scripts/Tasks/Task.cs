using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Task
{
    public enum TaskType
    {
        PrecicionBar,
    }
    public TaskType type;

    public event Action<Task> OnSuccess = delegate { };
    public event Action<Task> OnFail = delegate { };
    public event Action<Task> OnCancelled = delegate { };

    [Header("Mental Health Gained")]
    [SerializeField] private float mentalHealthGainedOnDay1 = 20f;
    [SerializeField] private float mentalHealthGainedOnDay2 = 20f;
    [SerializeField] private float mentalHealthGainedOnDay3 = 5f;
    [SerializeField] private float mentalHealthGainedOnDay4 = 4.5f;
    [SerializeField] private float mentalHealthGainedOnDay5 = 4f;
    [SerializeField] private float mentalHealthGainedOnDay6 = 3.5f;
    [SerializeField] private float mentalHealthGainedOnDay7 = 3f;

    [Header("Mental Health Lost")]
    [SerializeField] private float mentalHealthLostOnDay1 = 1f;
    [SerializeField] private float mentalHealthLostOnDay2 = 1f;
    [SerializeField] private float mentalHealthLostOnDay3 = 5f;
    [SerializeField] private float mentalHealthLostOnDay4 = 10f;
    [SerializeField] private float mentalHealthLostOnDay5 = 15f;
    [SerializeField] private float mentalHealthLostOnDay6 = 20f;
    [SerializeField] private float mentalHealthLostOnDay7 = 25f;

    private float mentalHealthGainedOnSuccess = 10f;
    private float mentalHealthLostOnFail = 10f;

    public bool active { get; private set; }

    private void UpdateValues(int day)
    {
        switch (day)
        {
            case 1:
                mentalHealthGainedOnSuccess = mentalHealthGainedOnDay1;
                mentalHealthLostOnFail = mentalHealthLostOnDay1;
                break;
            case 2:
                mentalHealthGainedOnSuccess = mentalHealthGainedOnDay2;
                mentalHealthLostOnFail = mentalHealthLostOnDay2;
                break;
            case 3:
                mentalHealthGainedOnSuccess = mentalHealthGainedOnDay3;
                mentalHealthLostOnFail = mentalHealthLostOnDay3;
                break;
            case 4:
                mentalHealthGainedOnSuccess = mentalHealthGainedOnDay4;
                mentalHealthLostOnFail = mentalHealthLostOnDay4;
                break;
            case 5:
                mentalHealthGainedOnSuccess = mentalHealthGainedOnDay5;
                mentalHealthLostOnFail = mentalHealthLostOnDay5;
                break;
            case 6:
                mentalHealthGainedOnSuccess = mentalHealthGainedOnDay6;
                mentalHealthLostOnFail = mentalHealthLostOnDay6;
                break;
            case 7:
                mentalHealthGainedOnSuccess = mentalHealthGainedOnDay7;
                mentalHealthLostOnFail = mentalHealthLostOnDay7;
                break;
            default:
                break;
        }
    }

    public virtual void BeginTask()
    {
        active = true;
        UpdateValues(TimeManager.day);
    }

    public void TaskSuccess()
    {
        active = false;
        OnSuccess.Invoke(this);
        MentalHealth.Instance.IncreaseMentalHealth(mentalHealthGainedOnSuccess);
    }

    public void TaskFail()
    {
        active = false;
        OnFail.Invoke(this);
        MentalHealth.Instance.ReduceMentalHealth(mentalHealthLostOnFail);
    }

    public void Deactivate()
    {
        active = false;
        OnCancelled?.Invoke(this);
    }
}
