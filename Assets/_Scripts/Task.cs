using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Task
{
    public enum TaskType
    {
        QTE,
        ButtonSpam,
        PrecicionBar,
    }
    public TaskType type;

    public event Action<Task> OnSuccess = delegate { };
    public event Action<Task> OnFail = delegate { };

    public virtual void BeginTask()
    {

    }

    public void TaskSuccess()
    {
        OnSuccess.Invoke(this);
    }

    public void TaskFail()
    {
        OnFail.Invoke(this);
    }
}
