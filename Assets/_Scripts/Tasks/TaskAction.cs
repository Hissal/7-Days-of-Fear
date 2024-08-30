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

        DestroyTaskAction();
    }

    protected virtual void TaskFail()
    {
        task.TaskFail();
        print("TaskFail");

        DestroyTaskAction();
    }

    private void DestroyTaskAction()
    {
        Destroy(gameObject);
    }
}
