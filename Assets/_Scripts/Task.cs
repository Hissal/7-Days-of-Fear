using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Task
{
    public enum TaskType
    {
        QTE,
        ButtonSpam,
        PrecicionBar,
    }
    public TaskType type;

    public event Action onSucces;
    public event Action onFail;

    public virtual void BeginTask()
    {

    }
}

public class PrecicionBarTask : Task
{
    public override void BeginTask()
    {
        base.BeginTask();
    }
}
