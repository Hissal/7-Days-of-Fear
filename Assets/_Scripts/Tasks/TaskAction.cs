using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class TaskAction : MonoBehaviour
{
    public Task task;

    protected virtual void TaskSuccess()
    {
        task.TaskSuccess();
        print("TaskSuccess");

        DestroyTask();
    }

    protected virtual void TaskFail()
    {
        task.TaskFail();
        print("TaskFail");

        DestroyTask();
    }

    private void DestroyTask()
    {
        Destroy(gameObject);
    }
}
