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
        print("TaskSuccess");
        task.TaskSuccess();

        DisableTaskAction();
    }

    protected virtual void TaskFail()
    {
        print("TaskFail");
        task.TaskFail();

        DisableTaskAction();
    }

    protected void DisableTaskAction()
    {
        gameObject.SetActive(false);
    }
}
