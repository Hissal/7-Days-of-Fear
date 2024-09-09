using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TaskAction : MonoBehaviour
{
    public Task task;

    public virtual void Init()
    {

    }

    protected virtual void TaskSuccess()
    {
        task.TaskSuccess();
        print("TaskSuccess");

        DisableTaskAction();
    }

    protected virtual void TaskFail()
    {
        task.TaskFail();
        print("TaskFail");

        DisableTaskAction();
    }

    protected void DisableTaskAction()
    {
        gameObject.SetActive(false);
    }
}
