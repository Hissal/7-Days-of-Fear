using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TaskAction : MonoBehaviour
{
    [HideInInspector] public Task task;

    public virtual void Init()
    {
        task.BeginTask();
    }

    protected virtual void TaskSuccess()
    {
        print("TaskSuccess");
        task.TaskSuccess();
    }

    protected virtual void TaskFail()
    {
        print("TaskFail");
        task.TaskFail();
    }

    protected virtual void DisableTaskAction()
    {
        gameObject.SetActive(false);
    }
}
