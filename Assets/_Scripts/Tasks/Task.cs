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

    [SerializeField] private float mentalHealthGainedOnSuccess = 10f;
    [SerializeField] private float mentalHealthLostOnFail = 10f;

    public virtual void BeginTask()
    {

    }

    public void TaskSuccess()
    {
        OnSuccess.Invoke(this);
        MentalHealth.Instance.IncreaseMentalHealth(mentalHealthGainedOnSuccess);
    }

    public void TaskFail()
    {
        OnFail.Invoke(this);
        MentalHealth.Instance.ReduceMentalHealth(mentalHealthLostOnFail);
    }
}
