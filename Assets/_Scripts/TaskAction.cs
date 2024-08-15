using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TaskAction : MonoBehaviour
{
    public Task task;

    public event Action<Task> onSuccess;
    public event Action<Task> onFail;

    protected virtual void TaskSuccess()
    {
        onSuccess(task);
        print("TaskSuccess");
    }

    protected virtual void TaskFail()
    {
        onFail(task);
        print("TaskFail");
    }
}
